using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//AdmobManager.cs �÷������� 6.0.0�̻���� AddTestDevice�� ���ŵǾ� ������ �����ϼ���.
public class AdMobManager : MonoBehaviour
{
    // �̺�Ʈ �Լ��� �Լ��� ��� ��밡��
    //public UnityEvent OnAdOpeningEvent;
    //public UnityEvent OnAdClosedEvent;

    public static AdMobManager instance;

    // public Text LogText;

    

    string adTestId = "ca-app-pub-3940256099942544/5224354917";
    string adUnitId = "ca-app-pub-4507641890363321/6944733016";

    private RewardedAd rewardedAd;  //������ ���� 

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
        PrintStatus("���� �ε� ����");

        // create new rewarded ad instance
        RewardedAd.Load(adUnitId, CreateAdRequest(),
            (RewardedAd ad, LoadAdError loadError) =>
            {
                //������ ��û�� ������ ���
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
                    PrintStatus("������ ���� �ε�Ϸ�");

                    rewardedAd = ad;

                    //�� ��Ȳ�� �̺�Ʈ �Լ�
                    //rewardedAd������ �� ��Ȳ�� ���� ���ప���� �־��ش�.
                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        PrintStatus("���� ����");
                    };
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        PrintStatus("���� ����");
                    };
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        PrintStatus("���� ���� ���п� ���� : " +
                                   error.GetMessage());
                    };
                }
            });
    }

    IEnumerator RewardReloadCroutine()            //���� �ε� ���� �� ��ε带 ���� �ڷ�ƾ
    {
        yield return new WaitForSeconds(1.0f);
        RequestAndLoadRewardedAd();
    }

    public bool isRewardEnd { get; private set; } = false;

    public void ShowRewardedAd()            //������ ���� ������
    {
        isRewardEnd = false;
        if (rewardedAd != null) // ������ ���� ���� ��쿡 �Ʒ� �ش� �ڵ带 �ۼ�
        {
            rewardedAd.Show((Reward reward) =>
            {
                PrintStatus("������ ���� ���� ���� : " + reward.Amount);      //������� ���� ���� ��� ���޵Ǵ� ���

                RequestAndLoadRewardedAd();

                //�ڿ��� ��׶��� ����
                isRewardEnd = true;
            });
        }
        else
        {
            PrintStatus("Rewarded ad is not ready yet.");
        }
    }

    public bool GetIsADLoaded()               //������ ���� ��� �ִ��� Ȯ��
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
