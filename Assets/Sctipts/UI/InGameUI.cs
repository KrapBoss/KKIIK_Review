using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    [Header("탑오브젝트")]
    public GameObject obj_TopInfo;
    [Header("진행도")]
    public Slider slider_progressbar;
    [Header("점수")]
    public TMP_Text txt_score;
    public TMP_Text txt_mulSocre;

    [Space]
    [Header("Die")]
    [SerializeField]InGame_Dead cs_dead;

    [Space]
    [Header("Fenalty UI")]
    [SerializeField] Image image_fenalty;

    [Space]
    [Header("결과 화면")]
    [SerializeField] GameObject resultUI;

    [Space]
    [Header("아이템")]
    [SerializeField] GameObject itemUI;

    [Space]
    [Header("콤보")]
    [SerializeField] TMP_Text txt_combo;
    RectTransform rect_combo;

    int score = 0; // 실제 적용되는 변수
    
    int created_door = 0; // 만들어진 문의 갯수를 저장한다.
    int count_openDoor = 0; // 문을 연 횟수를 카운트 한다.

    //기본 점수에 곱해지는 수
    float default_mulScore = 0.125f; // 세 자릿수까지 인정
    float apply_mul_score;

    //기본적으로 추가되는 점수
    readonly int default_sub_score = 100;

    // Start is called before the first frame update
    void Start()
    {
        //Set Values
        rect_combo = txt_combo.GetComponent<RectTransform>();


        //Set Event
        EventManager.instance.Action_Panelty += PenaltyEffect;               //패널티 이벤트 함수 
        EventManager.instance.Action_Combo += EventCombo;                   //점수증가 이벤트

        GameManager.instance.Action_ALLClearDoor += AllClearDoor;
        GameManager.instance.Action_Playing_Gameover += Playing_Gameover;       //플레이어 사망
        GameManager.instance.Action_Gameover_Finish += Gameover_Finish;
    }

    //꺼지면서 비활성화
    private void OnDisable()
    {
        Debug.Log("InGameUI Disable");

        cs_dead.gameObject.SetActive(false);
        itemUI.SetActive(false);

        StopAllCoroutines();
        increaseProgressCoroutine = null;
    }

    public void GameSetting()      //게임을 재 셋팅해준다.
    {
        //게임이 대기실에서 시작할 때 초기화를 해준다.
        if (!GameManager.instance.oneDie)
        {
            Debug.Log("인게임 변수 초기화");

            resultUI.SetActive(false);
            obj_TopInfo.SetActive(true);

            created_door = MapManager.instance.GetCreatedObject();  //진행률을 위함.

            count_openDoor = 0; //문을 연 횟수를 카운트한다.
            score = 0;          //점수를 저장한다.
            increaseProgress_SubValue = 0.005f; //기본 증가값
            txt_score.text = score.ToString();  //점수를 책정

            apply_mul_score = Mathf.Floor(default_mulScore * 1000) * 0.001f;
            txt_mulSocre.text = string.Format("{0:0.00}", apply_mul_score);

            slider_progressbar.value = 0;   //눈에 보이는 진행률 초기화
        }

        itemUI.SetActive(true);
        cs_dead.gameObject.SetActive(false);
    }

    public void Playing_Gameover()//게임 중 죽었을 경우
    {
        //Die 패널 활성화
        cs_dead.gameObject.SetActive(true);
        itemUI.SetActive(false);

        //죽은 패널 활성화
        cs_dead.gameObject.SetActive(true);
        cs_dead.Init();

        //Progress를 다 채움
        if(increaseProgressCoroutine != null)
        {
            StopAllCoroutines();
            slider_progressbar.value = (count_openDoor / (float)created_door);
        }
        
    }

    //게임에서 탈락 후 이어하기 X
    public void Gameover_Finish()
    {
        //결과 화면 활성화
        resultUI.SetActive(true);

        itemUI.SetActive(false);
        obj_TopInfo.SetActive(false);
        cs_dead.gameObject.SetActive(false);
    }

    ////진행률 증가에 대한 변수
    IEnumerator increaseProgressCoroutine = null;   // 기존 코루틴을 넘길 변수
    float value_increase_progress = 0;              //진행률의 목표값 [해당 값만 변경해도 진행률이 증가함]
    float increaseProgress_SubValue = 0.001f;                 //진행률의 증가값


    //스코어를 위한 변수
    public void OpenDoor()          //문이 열렸을 때 카운트를 한다.
    {
        count_openDoor++;
        score += (int)(default_sub_score * apply_mul_score);
        txt_score.text = string.Format("{0:#,###}", score);

        Progress();

        // 문을 연 개수와 만들어진 수가 같다면 =[모두 클리어], 진행도가 증가하는 값을 증가
        if (count_openDoor >= created_door) 
        {
            increaseProgress_SubValue = 0.01f;
        }
    }

    void Progress() // 진행률 변경
    {
        //진행률을 구한다.
        float v_progress = (count_openDoor / (float)created_door);
        if (count_openDoor == created_door) v_progress = 1;

        //진행률 증가에 대한 코루틴
        if (increaseProgressCoroutine == null)   //진행률 코루틴이 돌고 있지 않다면 실행
        {
            increaseProgressCoroutine = IncreaseProgressCroutine();
            value_increase_progress = v_progress;
            StartCoroutine(increaseProgressCoroutine);
        }
        else    //이미 코루틴이 돌고 있다면 목표값을 변경한다.
        {
            value_increase_progress = v_progress;
        }
    }

    WaitForSeconds waitTime_progress = new WaitForSeconds(0.03f);
    IEnumerator IncreaseProgressCroutine() // 진행도 증가를 부드럽게 하기 위함.
    {
        float slider_value = slider_progressbar.value;
        while (value_increase_progress > slider_value)
        {
            slider_value += increaseProgress_SubValue;
            slider_progressbar.value = slider_value;

            //Debug.Log($"진행률 코루틴 작동 중 {value_increase_progress} | {slider_value}");

            yield return waitTime_progress;
        }

        //루틴이 끝나면 코루틴을 빈 객체로 만든다.
        increaseProgressCoroutine = null;
    }

    //현재 해당하는 문들을 전부 열면 게임을 해당 UI 초기화
    public void AllClearDoor()
    {
        if (increaseProgressCoroutine != null)
        {
            StopCoroutine(increaseProgressCoroutine);
            increaseProgressCoroutine = null;
        }

        slider_progressbar.value = 1;
        StartCoroutine(AllClearDoorCroutine());
    }
    IEnumerator AllClearDoorCroutine()
    {
        yield return new WaitForSeconds(1.0f);
        //프로그래스바 초기화
        created_door = MapManager.instance.GetCreatedObject();
        count_openDoor = 0;

        increaseProgress_SubValue = 0.005f; //기본 증가값
        slider_progressbar.value = 0;
    }


    //이벤트를 적용한다.
    public void EventCombo(int _combo)
    {
        txt_combo.text = _combo.ToString();
    }

    //콤보에 해당하는 분기점을 넘겨받는 경우에 해당한다.
    public void EventComboQuater(int _quater)
    {
        
    }

    void PenaltyEffect()
    {
        image_fenalty.gameObject.SetActive(true);

        Invoke("DelayPenaltyEffect", 0.5f);
    }
    void DelayPenaltyEffect()
    {
        image_fenalty.gameObject.SetActive(false);
    }

    public int GetScore()
    {
        return score;
    }
}
