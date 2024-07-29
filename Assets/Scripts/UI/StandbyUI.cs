using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StandbyUI : MonoBehaviour
{
    [Space]
    [Header("���� ���� �г�")]
    [SerializeField] GameObject panel_exit;
    [SerializeField] Button btn_exit;
    [SerializeField] Button btn_nope;

    [Space]
    [Header("�ɼ�")]//�Ѱ� ���� �� ��
    [SerializeField] GameObject panel_option;
    [SerializeField] Button btn_option;
    bool option_isActive = false;

    [Space]
    [Header("����")]
    [SerializeField] StandBy_Shop c_shop;
    [SerializeField] Button btn_shop;



    [Space]
    [Header("���۹�ư")]
    [SerializeField] Button btn_start;


    [Space]
    [Header("��ȭ")]
    [SerializeField] TMP_Text txt_coin;

    [Space]
    [Header("�������� ����")]
    [SerializeField] StandBy_StageInfo stageInfo;


    string mapName = "default";


    private void Start()
    {
        btn_exit.onClick.AddListener(() => { Exit(); });
        btn_nope.onClick.AddListener(() => { Nope(); });

        //������ �ٷ� �տ� ���� ���� ������ ������
        btn_start.onClick.AddListener(()=> { TouchAction.instance.StartButton(); btn_start.gameObject.SetActive(false); });

        //�ɼ� �г� �Ѱ� ����
        btn_option.onClick.AddListener(() => { OptionButton(); });

        //�� ��ư
        btn_shop.onClick.AddListener(() => { ShowShopButton(); });
    }

    private void Update()
    {
        if (GameManager.instance.GameState == GAMESTATE.STANDBY)       //���� ���� ���°� Idle�� ��쿡�� Ȱ��ȭ
        {
            if (Input.GetKey(KeyCode.Escape)) //�ڷΰ��� Ű �Է�
            {
                //Exit �г� Ȱ��ȭ �� ���� ���� ��ȯ
                ActiveExitPanel();
            }
        }
    }
    //�����鼭 ��Ȱ��ȭ
    private void OnDisable()
    {
        panel_exit.SetActive(false);
        panel_option.SetActive(false);
    }


    //ó�� ������ �����ϰ�, ����ϴ� ȭ�鿡�� ��Ÿ���� UI���� �����Ѵ�.
    public void GameSetting()
    {
        stageInfo.Setting();
        btn_start.gameObject.SetActive(true);
        panel_exit.SetActive(false);
        c_shop.gameObject.SetActive(false);

        txt_coin.text = string.Format("{0:#,##0}", DataManager.instance.GetMoney());

        //���� �ʰ� �̸��� �ٸ� ���
        if (!mapName.Equals(GameManager.instance.GetStageName()))
        {
            mapName = GameManager.instance.GetStageName();
        }
    }

    #region OptionButton

    //�ɼ� ��ư�� ������ ���
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

    public void ActiveExitPanel()        //���� �г� Ȱ��ȭ
    {
        Debug.Log("ExitPanle");
        panel_exit.SetActive(true);
        if (option_isActive)//�ɼ��� Ȱ��ȭ �Ǿ� �ִٸ� ��Ȱ��
        {
            panel_option.SetActive(false);
            option_isActive = false;
        }
    }
    void Exit()
    {//����
        Application.Quit();
    }
    void Nope()
    {//exit �г� �ݱ�
        panel_exit.SetActive(false);
    }
    #endregion

    public void ShowShopButton()
    {
        c_shop.gameObject.SetActive(true);
        c_shop.Setting();
    }
}
