using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//게임의 상태를 나타낸다. 상태 변경에 따라 사용
public enum GAMESTATE
{
    STANDBY = 0,
    PLAYING,
    GAMEOVER ,
    FINISH
}
[System.Serializable]
public struct ConceptInfo
{
    [Header("맵의 이름에 따라 맵이 불러와짐")]
    public string NAME;
    [Header("최대 랜덤으로 생성되는 문의 패턴[지정한 패턴 >= Random(Type)]")]
    public DoorType TYPE;
    [Header("컨셉별 스테이지 개수")]
    public int STAGE_NUM;
    //[Header("문의 스킨 이름")]
    //public string DOOR_NAME;
}

//맵의 난이도에 대해 전체적인 관리와 생성을 담당
//스테이지가 하나씩 클리어될 때마다 스테이지 정보에 따라 맵을 재 생성한다.
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int tempStage = 1;       //PC 플레이의 경우 임의의 스테이지를 지정한다.
    public bool test;


    //public Action Action_Panelty { internal get; set; } // 잘못된 터치 동작을 했을 경우 호출 된다.
    public Action Action_ALLClearDoor { internal get; set; }    //문을 전부 클리어 했을 경우를 나타냄.

    //게임 셋팅에서 사용할 값들을 대입
    public Action Action_Gameover_StandBy { internal get; set; }//죽음 -> 대기
    public Action Action_Finish_StandBy {  internal get; set; }// 완주 -> 대기
    public Action Action_Standby_Playing { internal get; set; }//대기 -> 플레이
    public Action Action_Playing_Gameover { internal get; set; }//플레이 -> 게임오버
    public Action Action_Gameover_Finish { internal get; set; }//게임오버 -> 결과
    
    //현재의 스테이지를 나타냄.
    public int CURRENT_STAGE { get; private set; } = 0;

    [Space]
    [Header("각 컨셉에 대한 정보")]
    public ConceptInfo[] conceptInfos;

    //플레이어가 죽었다가 부활할 경우를 대비한 것.
    public bool oneDie { get; private set; } = false;  // 한번 죽었다.

    public GAMESTATE GameState { get; private set; } = GAMESTATE.FINISH;    //게임의 진행상태를 적용한다.

    [SerializeField] float[] StepByMaximumSpeed;

    private void Awake()
    {
        if (instance == null) instance = this;

        Debug.Log("Awake");
    }

    private void OnDestroy()
    {
        instance = null;
    }

    //게임에 들어왔을 때 바로 맵과 플레이어 등을 셋팅한다.
    IEnumerator Start()
    {
        Debug.Log("Start");

        //로그인 처리를 위한 것.
        UIManager.instance.ALLFadeIn();

        Action_ALLClearDoor += () => { CURRENT_STAGE++; DataManager.instance.SetStage(CURRENT_STAGE);};

        Application.targetFrameRate = 50;


        yield return new WaitForSeconds(0.5f);

        if (!test)
        {
            #region Login With Google
            Debug.LogWarning("Google Step 1: 로그인 요청");
            GoogleManager.instance.Login();
            yield return new WaitUntil(() => GoogleManager.instance.isLogin);

            Debug.LogWarning("Google Step 2 : DataLoad");
            yield return new WaitUntil(() => GoogleManager.instance.isLoaded);      //로드까지 기다린다.

            Debug.LogWarning("Google Step 3 : Get Stage");
            CURRENT_STAGE = DataManager.instance.GetStage();
            #endregion
        }
        else
        {
            CURRENT_STAGE = 0;
        }



        yield return new WaitForSeconds(1f);

        Debug.Log("GAMESETTING GM");

        GameStateChange(GAMESTATE.STANDBY);
    }

    public void GameStateChange(GAMESTATE _state)   //게임의 상태를 변경한다.
    {
        Debug.Log($"게임 상태 변경 {GameState} -> {_state}");
        switch (_state)
        {
            case GAMESTATE.STANDBY:
                if (GameState.Equals(GAMESTATE.GAMEOVER))   //플레이어가 죽은 상태에서 제자리 시작
                {
                    Action_Gameover_StandBy();
                    GameState = _state;
                }
                else if (GameState.Equals(GAMESTATE.FINISH))   //클리어 후 넘어갈 경우
                {
                    UIManager.instance.ALLFadeIn();
                    //Debug.LogWarning("Finish To StandBy");
                    oneDie = false;
                    Action_Finish_StandBy();
                    GameState = _state;
                    StartCoroutine(DelayFadeOut());
                }
                break;
            case GAMESTATE.PLAYING:
                if (GameState.Equals(GAMESTATE.STANDBY))    //대기 상태에서 플레이로 바뀔 경우
                {
                    //Debug.LogWarning("StandBy To Playing");
                    Action_Standby_Playing();
                    GameState = _state;
                }
                break;
            case GAMESTATE.GAMEOVER:
                if (GameState.Equals(GAMESTATE.PLAYING))    //게임 진행 중 죽었을 경우
                {
                    oneDie = true;// 한번 죽었다.

                    //Debug.LogWarning("Playing To Gameover");
                    Action_Playing_Gameover();
                    GameState = _state;
                }
                break;
            case GAMESTATE.FINISH:
                if (GameState.Equals(GAMESTATE.GAMEOVER))
                {
                    //Debug.LogWarning("GameOver To Finish");
                    Action_Gameover_Finish();
                    GameState = _state;
                }
                if (GameState.Equals(GAMESTATE.PLAYING))
                {
                    //Debug.LogWarning("Playing To Finish");
                }
                break;
        }
    }

    WaitForSeconds fadeout_time = new WaitForSeconds(0.2f);
    IEnumerator DelayFadeOut()
    {
        yield return fadeout_time;
        UIManager.instance.ALLFadeOut();
    }

    #region GetSet

    public string GetStageName()    //현재 스테이지의 이름
    {
        return conceptInfos[GetStep()].NAME;
    }
   
    public int GetStep() //현재 스테이지에 해당하는 맵 컨셉 0 ~ N
    {
        int sum = 0;
        int idx = 0;
        for(;idx<conceptInfos.Length; idx++)
        {
            sum += conceptInfos[idx].STAGE_NUM; // 각 콘셉트별 스테이지 개수를 가져온다.
            if (CURRENT_STAGE < sum) break;
        }
        
        if (idx >= conceptInfos.Length) idx = conceptInfos.Length - 1; //배열 초과시 마지막 맵 정보를 넘김
        return idx;
    }

    //각 패턴의 갯수별로 지정해둔 최대 스피드를 반환한다
    public float GetMaximumSpeed()
    {
        int type = (int)conceptInfos[GetStep()].TYPE;
        Debug.LogWarning($"현재 반환 타입 : {type}");
        Debug.LogWarning($"Step : {GetStep()}");
        if ((type - 2) > (StepByMaximumSpeed.Length - 1))
        {
            return StepByMaximumSpeed[StepByMaximumSpeed.Length - 1];
        }
        return StepByMaximumSpeed[type-1];
    }


    public DoorType GetCurrentType()
    {
        return conceptInfos[GetStep()].TYPE;
    }

    #endregion


    #region Ads

    #endregion

    public void ThrowError(string e)//에러 발생 시 화면에 표시
    {

    }
}