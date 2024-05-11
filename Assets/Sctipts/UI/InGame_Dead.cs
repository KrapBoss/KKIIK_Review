using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class InGame_Dead : MonoBehaviour
{
    [SerializeField] Image image_ad;        //���� ǥ�� �̹���
    [SerializeField] Image image_recovery;

    [SerializeField]Button btn_continue;    //�̾��ϱ� ��ư
    [SerializeField]Button btn_home;        //����ȭ�� ���� ��ư

    [SerializeField] ParticleSystem ps;

    private void Start()
    {
        btn_home.onClick.AddListener(() => { GameManager.instance.GameStateChange(GAMESTATE.FINISH); });     //Ȩ��ư�� ���ȭ������ �̵�
        btn_continue.onClick.AddListener(() => { this.ContinueButton(); });    //���� �Ŀ� ������ �̾��Ѵ�.

        // ���� ����� �Ϸ��� �ݵ�� Finish�� ���İ����Ѵ�.
        GameManager.instance.Action_Finish_StandBy += () => { AD = false; };
    }


    bool ITEM;  //�������� �ִ°�?
    [SerializeField]bool AD;    //���� �ô°�?

    // �ʱⰪ ����
    public void Init()
    {
        ITEM = false;

        ps.Play();
        image_recovery.color = Color.white;

        //1 ������ �� Recovery �������� �����ϴ°�?
        if (DataManager.instance.GetItem(CODE_ITEM.RECOVERY) > 0)
        {
            ITEM = true;
            image_ad.enabled = false;
        }
        else //2 ���� ���� ���� �ʾҴ�.
            if (!AD)
        {
            image_ad.enabled = true;
            ContinueSetting();      //���� �ִ��� �����Ѵ�.
        }
        else//�����۵� ���� ���� �̹� �ô�.
        {
            btn_continue.interactable = false;
            image_ad.enabled = false;
            ps.Stop();
            image_recovery.color = Color.grey;
        }
    }

    public void ContinueButton()
    {
        if (ITEM)   //�������� �����Ѵ�.
        {
            DataManager.instance.SetITem(CODE_ITEM.RECOVERY, -1);
            GameManager.instance.GameStateChange(GAMESTATE.STANDBY);
        }
        else
            if (!AD) //���� ���� �ʾҴ�.
        {
            AdMobManager.instance.ShowRewardedAd();
            StartCoroutine(ContinueButtonCroutine());
        }
    }
    IEnumerator ContinueButtonCroutine()    // �����尡 �����ٸ�, ������ �����.
    {
        yield return new WaitUntil(() => AdMobManager.instance.isRewardEnd);
        AD = true;
        GameManager.instance.GameStateChange(GAMESTATE.STANDBY);            //���� �� ����
    }


    void ContinueSetting()  // ���� Ȱ��ȭ
    {
        if (!AdMobManager.instance.GetIsADLoaded()) // ���� ����.
        {
            btn_continue.interactable = false;
            StartCoroutine(ContinueSettingCroutine());
        }
        else
        {
            //���� ������ Ȱ��ȭ
            btn_continue.interactable = true;
        }
    }
    IEnumerator ContinueSettingCroutine()   //���� �ε�Ǿ��ִ��� Ȯ���Ѵ�.
    {
        //���� �ε带 ��ٸ���.
        yield return new WaitUntil(() => AdMobManager.instance.GetIsADLoaded());

        //���� ������ Ȱ��ȭ
        btn_continue.interactable = true;
    }
}
