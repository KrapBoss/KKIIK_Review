using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StandbyUI : MonoBehaviour
{
    [Space]
    [Header("게임 종료 패널")]
    [SerializeField] GameObject panel_exit;
    [SerializeField] Button btn_exit;
    [SerializeField] Button btn_nope;

    [Space]
    [Header("옵션")]//켜고 끄고만 할 것
    [SerializeField] GameObject panel_option;
    [SerializeField] Button btn_option;
    bool option_isActive = false;

    [Space]
    [Header("상점")]
    [SerializeField] StandBy_Shop c_shop;
    [SerializeField] Button btn_shop;



    [Space]
    [Header("시작버튼")]
    [SerializeField] Button btn_start;


    [Space]
    [Header("재화")]
    [SerializeField] TMP_Text txt_coin;

    [Space]
    [Header("스테이지 정보")]
    [SerializeField] StandBy_StageInfo stageInfo;


    string mapName = "default";


    private void Start()
    {
        btn_exit.onClick.AddListener(() => { Exit(); });
        btn_nope.onClick.AddListener(() => { Nope(); });

        //누르면 바로 앞에 문을 열고 게임을 시작함
        btn_start.onClick.AddListener(()=> { TouchAction.instance.StartButton(); btn_start.gameObject.SetActive(false); });

        //옵션 패널 켜고 끄고
        btn_option.onClick.AddListener(() => { OptionButton(); });

        //샵 버튼
        btn_shop.onClick.AddListener(() => { ShowShopButton(); });
    }

    private void Update()
    {
        if (GameManager.instance.GameState == GAMESTATE.STANDBY)       //현재 게임 상태가 Idle일 경우에만 활성화
        {
            if (Input.GetKey(KeyCode.Escape)) //뒤로가기 키 입력
            {
                //Exit 패널 활성화 후 게임 상태 변환
                ActiveExitPanel();
            }
        }
    }
    //꺼지면서 비활성화
    private void OnDisable()
    {
        panel_exit.SetActive(false);
        panel_option.SetActive(false);
    }


    //처음 게임을 시작하고, 대기하는 화면에서 나타나는 UI들을 정렬한다.
    public void GameSetting()
    {
        stageInfo.Setting();
        btn_start.gameObject.SetActive(true);
        panel_exit.SetActive(false);
        c_shop.gameObject.SetActive(false);

        txt_coin.text = string.Format("{0:#,##0}", DataManager.instance.GetMoney());

        //기존 맵과 이름이 다를 경우
        if (!mapName.Equals(GameManager.instance.GetStageName()))
        {
            mapName = GameManager.instance.GetStageName();
        }
    }

    #region OptionButton

    //옵션 버튼을 눌렀을 경우
    void OptionButton()
    {
        if (panel_option.activeSelf) option_isActive = true;
        else option_isActive = false;

        if (option_isActive)
        {
            panel_option.SetActive(false);
            option_isActive = false;
        }
        else
        {
            panel_option.SetActive(true);
            option_isActive = true;
        }
    }

    public void ActiveExitPanel()        //종료 패널 활성화
    {
        Debug.Log("ExitPanle");
        panel_exit.SetActive(true);
        if (option_isActive)//옵션이 활성화 되어 있다면 비활성
        {
            panel_option.SetActive(false);
            option_isActive = false;
        }
    }
    void Exit()
    {//종료
        Application.Quit();
    }
    void Nope()
    {//exit 패널 닫기
        panel_exit.SetActive(false);
    }
    #endregion

    public void ShowShopButton()
    {
        c_shop.gameObject.SetActive(true);
        c_shop.Setting();
    }
}
