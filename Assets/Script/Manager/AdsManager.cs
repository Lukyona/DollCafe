using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    private RewardedAd rewardedAd;
    public bool addOn = false; //광고 보는 중 true, 다 보면 false

    string adUnitId = "ca-app-pub-9549514417727735/8806879959"; //광고 아이디

    bool stopLoad = false;//광고로드가 되었으면 true

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => { });

        this.rewardedAd = new RewardedAd(adUnitId);

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);

        this.rewardedAd.OnUserEarnedReward += RewardedAd_OnUserEarnedReward;
        this.rewardedAd.OnAdClosed += RewardedAd_OnAdClosed;

    }

    public void AdsYes()
    {
        addOn = true;
        GameScript1.instance.plusHP.interactable = false;
        GameScript1.instance.AdsMessage.SetActive(false);
        Invoke(nameof(WantAds), 0.3f);
    }

    void WantAds()
    {
        //Debug.Log("광고 보여주기");
        if(rewardedAd.IsLoaded())
        {
            stopLoad = false;
            rewardedAd.Show();
        }
        else
        {
         //   Debug.Log("로드 안됨");
            CreateAndLoadRewardedAd();
            Invoke(nameof(WantAds), 0.3f);
        }
    }

    private void Update()
    {
        if(!stopLoad)
        {
            if (rewardedAd.IsLoaded())
            {
                stopLoad = true;
                Debug.Log(stopLoad);
                GameScript1.instance.plusHP.interactable = true;
            //    Debug.Log("광고 로드 완료");
            }
        } 
    }

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }

    public void CreateAndLoadRewardedAd()
    {        
        this.rewardedAd = new RewardedAd(adUnitId);

        this.rewardedAd.OnUserEarnedReward += RewardedAd_OnUserEarnedReward;
        this.rewardedAd.OnAdClosed += RewardedAd_OnAdClosed;

        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
       
    }

    private void RewardedAd_OnAdClosed(object sender, EventArgs e)
    {
        //유저에 의해 중간에 광고 닫힘
        CreateAndLoadRewardedAd();
    }

    private void RewardedAd_OnUserEarnedReward(object sender, Reward e)
    {
        HPCharge.instance.AddHP();
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }
    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }


}
