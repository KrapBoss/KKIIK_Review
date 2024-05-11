using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class InGame_Dead : MonoBehaviour
{
    [SerializeField] Image image_ad;        //광고 표시 이미지
    [SerializeField] Image image_recovery;

    [SerializeField]Button btn_continue;    //이어하기 버튼
    [SerializeField]Button btn_home;        //메인화면 가기 버튼

    [SerializeField] ParticleSystem ps;

    private void Start()
    {
        btn_home.onClick.AddListener(() => { GameManager.instance.GameStateChange(GAMESTATE.FINISH); });     //홈버튼시 대기화면으로 이동
        btn_continue.onClick.AddListener(() => { this.ContinueButton(); });    //죽은 후에 게임을 이어한다.

        // 최종 결산을 하려면 반드시 Finish를 거쳐가야한다.
        GameManager.instance.Action_Finish_StandBy += () => { AD = false; };
    }


    bool ITEM;  //아이템이 있는가?
    [SerializeField]bool AD;    //광고를 봤는가?

    // 초기값 설정
    public void Init()
    {
        ITEM = false;

        ps.Play();
        image_recovery.color = Color.white;

        //1 아이템 중 Recovery 아이템이 존재하는가?
        if (DataManager.instance.GetItem(CODE_ITEM.RECOVERY) > 0)
        {
            ITEM = true;
            image_ad.enabled = false;
        }
        else //2 현재 광고를 보지 않았다.
            if (!AD)
        {
            image_ad.enabled = true;
            ContinueSetting();      //광고가 있는지 설정한다.
        }
        else//아이템도 없고 광고도 이미 봤다.
        {
            btn_continue.interactable = false;
            image_ad.enabled = false;
            ps.Stop();
            image_recovery.color = Color.grey;
        }
    }

    public void ContinueButton()
    {
        if (ITEM)   //아이템이 존재한다.
        {
            DataManager.instance.SetITem(CODE_ITEM.RECOVERY, -1);
            GameManager.instance.GameStateChange(GAMESTATE.STANDBY);
        }
        else
            if (!AD) //광고를 보지 않았다.
        {
            AdMobManager.instance.ShowRewardedAd();
            StartCoroutine(ContinueButtonCroutine());
        }
    }
    IEnumerator ContinueButtonCroutine()    // 리워드가 끝난다면, 게임을 재시작.
    {
        yield return new WaitUntil(() => AdMobManager.instance.isRewardEnd);
        AD = true;
        GameManager.instance.GameStateChange(GAMESTATE.STANDBY);            //게임 재 시작
    }


    void ContinueSetting()  // 광고 활성화
    {
        if (!AdMobManager.instance.GetIsADLoaded()) // 광고가 없다.
        {
            btn_continue.interactable = false;
            StartCoroutine(ContinueSettingCroutine());
        }
        else
        {
            //광고가 있으면 활성화
            btn_continue.interactable = true;
        }
    }
    IEnumerator ContinueSettingCroutine()   //광고가 로드되어있는지 확인한다.
    {
        //광고 로드를 기다린다.
        yield return new WaitUntil(() => AdMobManager.instance.GetIsADLoaded());

        //광고가 있으면 활성화
        btn_continue.interactable = true;
    }
}
