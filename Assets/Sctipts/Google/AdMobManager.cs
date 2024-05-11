using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//AdmobManager.cs 플러그인이 6.0.0이상부턴 AddTestDevice가 제거되어 다음을 참고하세요.
public class AdMobManager : MonoBehaviour
{
    // 이벤트 함수에 함수를 담아 사용가능
    //public UnityEvent OnAdOpeningEvent;
    //public UnityEvent OnAdClosedEvent;

    public static AdMobManager instance;

    // public Text LogText;

    

    string adTestId = "ca-app-pub-3940256099942544/5224354917";
    string adUnitId = "ca-app-pub-4507641890363321/6944733016";

    private RewardedAd rewardedAd;  //리워드 광고 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RequestAndLoadRewardedAd();
    }

    void PrintStatus(string s)
    {
        Debug.LogWarning(s);
    }

    public void RequestAndLoadRewardedAd()
    {
        PrintStatus("광고 로드 시작");

        // create new rewarded ad instance
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                //리워드 요청이 실패할 경우
                if (loadError != null)
                {
                    PrintStatus("Rewarded ad failed to load with error: " +
                                loadError.GetMessage());

                    StartCoroutine(RewardReloadCroutine());
                    return;
                }
                else if (ad == null)
                {
                    PrintStatus("Rewarded ad failed to load.");
                    StartCoroutine(RewardReloadCroutine());
                    return;
                }
                else
                {
                    PrintStatus("리워드 광고 로드완료");

                    rewardedAd = ad;

                    //각 상황별 이벤트 함수
                    //rewardedAd변수에 각 상황에 대한 실행값들을 넣어준다.
                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("광고 오픈");
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("광고 닫음");
                    };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("광고 노출 실패와 이유 : " +
                                   error.GetMessage());
                    };
                }
            });
    }

    IEnumerator RewardReloadCroutine()            //광고 로드 실패 시 재로드를 위한 코루틴
    {
        yield return new WaitForSeconds(1.0f);
        RequestAndLoadRewardedAd();
    }

    public bool isRewardEnd { get; private set; } = false;

    public void ShowRewardedAd()            //리워드 광고를 보여줌
    {
        isRewardEnd = false;
        if (rewardedAd != null) // 리워드 광고가 있을 경우에 아래 해당 코드를 작성
        {
            rewardedAd.Show((Reward reward) =>
            {
                PrintStatus("리워드 광고 보상 제공 : " + reward.Amount);      //리워드는 광고를 보는 즉시 지급되는 방식

                RequestAndLoadRewardedAd();

                //뒤에서 백그라운드 변경
                isRewardEnd = true;
            });
        }
        else
        {
            PrintStatus("Rewarded ad is not ready yet.");
        }
    }

    public bool GetIsADLoaded()               //리워드 광고가 비어 있는지 확인
    {
        return (rewardedAd != null);
    }

    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder()
            .Build();
    }
    #endregion

}
