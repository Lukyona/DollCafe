using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TimeManager : MonoBehaviour
{ 
   public static TimeManager instance;

    public Coroutine TimeCoroutine;

    bool loading = false; //true면 타이머 및 체력 정보 로드 중

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        SetLoadingState(true); // 로딩 시작
        TimerStart();
    }

    string url = "https://www.google.co.kr/";
    DateTime dateTime;

    public void TimerStart()
    {
        TimeCoroutine = StartCoroutine(WebChk());
    }

     public IEnumerator WebChk()
     {
        while(true)
        {
            UnityWebRequest request = new UnityWebRequest();
            using (request = UnityWebRequest.Get(url)) 
            {
                // yield는 코루틴 내에서 반드시 1번 이상 사용되어야 함
                yield return request.SendWebRequest(); // 코드의 제어권을 유니티에게 잠시 양보했다가 돌려받아서 아래 코드를 계속 진행

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    string date = request.GetResponseHeader("date"); // date 정보를 가져옴
                    dateTime = DateTime.Parse(date);
                 //   Debug.Log("시간은 " + dateTime);
                }
            }

            if (IsLoading())//로드할 때
            {
                Debug.Log("타이머 정보 로드");
                if (!AdsManager.instance.IsWatchingAds())
                {
                    Debug.Log("체력 정보 로드");
                    
                    HPManager.instance.LoadHPInfo(); //체력 정보 불러옴
                }
                HPManager.instance.LoadAppQuitTime(); //게임 종료 시간 불러옴
                HPManager.instance.SetRechargeScheduler(); 
                SetLoadingState(false);

                if(AdsManager.instance.IsWatchingAds())
                {
                Debug.Log("광고 상태 해제");

                    AdsManager.instance.SetWatchingAds(false);
                }
            } 
            yield return new WaitForSeconds(2f);
        }
    }
        
    public bool SaveAppQuitTime() //종료 시간 저장
    {
        //Debug.Log("SaveAppQuitTime");
        bool result = false;
        try
        {
            var appQuitTime = TimeManager.instance.GetDateTime().ToBinary().ToString();
            PlayerPrefs.SetString("AppQuitTime", appQuitTime);
            PlayerPrefs.Save();
            Debug.Log("저장된 시간" + DateTime.FromBinary(Convert.ToInt64(appQuitTime)).ToString());
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveAppQuitTime Failed (" + e.Message + ")");
        }
        return result;
    }

    public DateTime GetDateTime()
    {
        return dateTime;
    }

    public void SetLoadingState(bool value)
    {
        loading = value;
    }

    public bool IsLoading()
    {
        return loading;
    }
}
    