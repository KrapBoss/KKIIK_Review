using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//������ ���¸� ��Ÿ����. ���� ���濡 ���� ���
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
    [Header("���� �̸��� ���� ���� �ҷ�����")]
    public string NAME;
    [Header("�ִ� �������� �����Ǵ� ���� ����[������ ���� >= Random(Type)]")]
    public DoorType TYPE;
    [Header("������ �������� ����")]
    public int STAGE_NUM;
    //[Header("���� ��Ų �̸�")]
    //public string DOOR_NAME;
}

//���� ���̵��� ���� ��ü���� ������ ������ ���
//���������� �ϳ��� Ŭ����� ������ �������� ������ ���� ���� �� �����Ѵ�.
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int tempStage = 1;       //PC �÷����� ��� ������ ���������� �����Ѵ�.
    public bool test;


    //public Action Action_Panelty { internal get; set; } // �߸��� ��ġ ������ ���� ��� ȣ�� �ȴ�.
    public Action Action_ALLClearDoor { internal get; set; }    //���� ���� Ŭ���� ���� ��츦 ��Ÿ��.

    //���� ���ÿ��� ����� ������ ����
    public Action Action_Gameover_StandBy { internal get; set; }//���� -> ���
    public Action Action_Finish_StandBy {  internal get; set; }// ���� -> ���
    public Action Action_Standby_Playing { internal get; set; }//��� -> �÷���
    public Action Action_Playing_Gameover { internal get; set; }//�÷��� -> ���ӿ���
    public Action Action_Gameover_Finish { internal get; set; }//���ӿ��� -> ���
    
    //������ ���������� ��Ÿ��.
    public int CURRENT_STAGE { get; private set; } = 0;

    [Space]
    [Header("�� ������ ���� ����")]
    public ConceptInfo[] conceptInfos;

    //�÷��̾ �׾��ٰ� ��Ȱ�� ��츦 ����� ��.
    public bool oneDie { get; private set; } = false;  // �ѹ� �׾���.

    public GAMESTATE GameState { get; private set; } = GAMESTATE.FINISH;    //������ ������¸� �����Ѵ�.

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

    //���ӿ� ������ �� �ٷ� �ʰ� �÷��̾� ���� �����Ѵ�.
    IEnumerator Start()
    {
        Debug.Log("Start");

        //�α��� ó���� ���� ��.
        UIManager.instance.ALLFadeIn();

        Action_ALLClearDoor += () => { CURRENT_STAGE++; DataManager.instance.SetStage(CURRENT_STAGE);};

        Application.targetFrameRate = 50;


        yield return new WaitForSeconds(0.5f);

        if (!test)
        {
            #region Login With Google
            Debug.LogWarning("Google Step 1: �α��� ��û");
            GoogleManager.instance.Login();
            yield return new WaitUntil(() => GoogleManager.instance.isLogin);

            Debug.LogWarning("Google Step 2 : DataLoad");
            yield return new WaitUntil(() => GoogleManager.instance.isLoaded);      //�ε���� ��ٸ���.

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

    public void GameStateChange(GAMESTATE _state)   //������ ���¸� �����Ѵ�.
    {
        Debug.Log($"���� ���� ���� {GameState} -> {_state}");
        switch (_state)
        {
            case GAMESTATE.STANDBY:
                if (GameState.Equals(GAMESTATE.GAMEOVER))   //�÷��̾ ���� ���¿��� ���ڸ� ����
                {
                    Action_Gameover_StandBy();
                    GameState = _state;
                }
                else if (GameState.Equals(GAMESTATE.FINISH))   //Ŭ���� �� �Ѿ ���
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
                if (GameState.Equals(GAMESTATE.STANDBY))    //��� ���¿��� �÷��̷� �ٲ� ���
                {
                    //Debug.LogWarning("StandBy To Playing");
                    Action_Standby_Playing();
                    GameState = _state;
                }
                break;
            case GAMESTATE.GAMEOVER:
                if (GameState.Equals(GAMESTATE.PLAYING))    //���� ���� �� �׾��� ���
                {
                    oneDie = true;// �ѹ� �׾���.

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

    public string GetStageName()    //���� ���������� �̸�
    {
        return conceptInfos[GetStep()].NAME;
    }
   
    public int GetStep() //���� ���������� �ش��ϴ� �� ���� 0 ~ N
    {
        int sum = 0;
        int idx = 0;
        for(;idx<conceptInfos.Length; idx++)
        {
            sum += conceptInfos[idx].STAGE_NUM; // �� �ܼ�Ʈ�� �������� ������ �����´�.
            if (CURRENT_STAGE < sum) break;
        }
        
        if (idx >= conceptInfos.Length) idx = conceptInfos.Length - 1; //�迭 �ʰ��� ������ �� ������ �ѱ�
        return idx;
    }

    //�� ������ �������� �����ص� �ִ� ���ǵ带 ��ȯ�Ѵ�
    public float GetMaximumSpeed()
    {
        int type = (int)conceptInfos[GetStep()].TYPE;
        Debug.LogWarning($"���� ��ȯ Ÿ�� : {type}");
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

    public void ThrowError(string e)//���� �߻� �� ȭ�鿡 ǥ��
    {

    }
}