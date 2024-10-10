using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    [SerializeField] private GameObject adsConsentWindow;

    private string adUnitId = "ca-app-pub-9549514417727735/8806879959"; //광고 아이디

    private RewardedAd rewardedAd;
    public bool IsWatchingAds {get; set;} //광고 보는 중 true, 다 보면 false
    private bool isLoaded = false; //광고 로드가 되었으면 true

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 새로 생성된 건 파괴
        }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
      //Debug.Log("Loading the rewarded ad.");

      // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) => // 람다식
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " + "with error : " + error);
                    return;
                }
                //Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

                rewardedAd = ad;
                RegisterEventHandlers(rewardedAd);
            });
    }

    public void ShowRewardedAd() 
    {
        //const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            TimeManager.instance.SaveAppQuitTime(); //게임 나간 시간 저장     
            HPManager.instance.SaveHPInfo(); //체력, 타이머 정보 저장                

            rewardedAd.Show((Reward reward) => // 광고 끝나고 닫으면 실행됨
            {
                // TODO: Reward the user.
                //Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                HPManager.instance.AddHP(); // 체력 추가
            });
        }
    }

    public void AgreeToWatchAds() // 광고 보는 것에 동의하는 버튼을 눌렀을 때
    {
        IsWatchingAds = true;
        SystemManager.instance.CantTouchUI(); // 버튼 터치 불가
        adsConsentWindow.SetActive(false);
        WantAds();

    }

    private void WantAds()
    {
        if(rewardedAd.CanShowAd())
        {
            ShowRewardedAd();
        }
        else
        {
            LoadRewardedAd();
            Invoke(nameof(WantAds), 0.3f);
        }
    }

    private void Update()
    {
        if(!isLoaded)
        {
            if (rewardedAd.CanShowAd()) 
            {
                isLoaded = true; // 광고 로드 완료
                SystemManager.instance.CanTouchUI(); // 다시 버튼 터치 가능
            }
        } 
    }

    public void SetAdsConsentWindowActive(bool value)
    {
        adsConsentWindow.SetActive(value);
    }


    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            //Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            //Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            isLoaded = false;
            //Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " + "with error : " + error);
        };
    }
}