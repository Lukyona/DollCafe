using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class TimeManager : MonoBehaviour
{ 
   public static TimeManager instance;

    public Coroutine TimeCoroutine;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        HPCharge.instance.load = true;
        TimeSaveStart();
    }

    public string url = "";
    public DateTime dateTime;

    public void TimeSaveStart()
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
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    string date = request.GetResponseHeader("date");
                    dateTime = DateTime.Parse(date);
                 //   Debug.Log("시간은 " + dateTime);
                }
            }
            if (HPCharge.instance.load)//로드할 때
            {
             //   Debug.Log("타이머 정보 로드");
                if (!AdsManager.instance.addOn)
                {
                    HPCharge.instance.LoadHPInfo(); //체력 정보 불러옴
                }
                HPCharge.instance.LoadAppQuitTime(); //게임 종료 시간 불러옴
                HPCharge.instance.SetRechargeScheduler();
                HPCharge.instance.load = false;
                if(AdsManager.instance.addOn)
                {
                    AdsManager.instance.addOn = false;
                }
            } 
            yield return new WaitForSeconds(2f);
        }
    }
        
}
    