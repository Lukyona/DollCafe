using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    private string url = "https://www.google.co.kr/";
    private DateTime dateTime;

    private Coroutine TimeCoroutine;

    private bool loading = false; //true면 타이머 및 체력 정보 로드 중

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        SetLoadingState(true); // 로딩 시작
        StartTimer();
    }

    public void StartTimer()
    {
        TimeCoroutine = StartCoroutine(WebChk());
    }

    public void StopTimer()
    {
        StopCoroutine(TimeCoroutine);
    }

    public bool IsTimerNotNull()
    {
        return TimeCoroutine != null ? true : false;
    }

    public IEnumerator WebChk()
    {
        while (true)
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
                }
            }

            if (loading)//로드할 때
            {
                if (!AdsManager.instance.IsWatchingAds)
                {
                    HPManager.instance.LoadHPInfo(); //체력 정보 불러옴
                }
                LoadAppQuitTime(); //게임 종료 시간 불러옴
                HPManager.instance.CalculateTime();
                SetLoadingState(false);
                AdsManager.instance.IsWatchingAds = false;
            }
            yield return new WaitForSeconds(2f);
        }
    }

    public void AfterWatchingAds()
    {
        SetLoadingState(true);
        if (IsTimerNotNull())
        {
            StopTimer();
        }
        StartTimer();
    }

    public bool SaveAppQuitTime() //종료 시간 저장
    {
        bool result = false;
        try
        {
            var appQuitTime = GetDateTime().ToBinary().ToString();
            PlayerPrefs.SetString("AppQuitTime", appQuitTime);
            PlayerPrefs.Save();
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveAppQuitTime Failed (" + e.Message + ")");
        }
        return result;
    }

    public void LoadAppQuitTime() // 게임을 종료했던 시간 불러옴
    {
        try
        {
            if (PlayerPrefs.HasKey("AppQuitTime"))
            {
                var appQuitTime = string.Empty;
                appQuitTime = PlayerPrefs.GetString("AppQuitTime");
                DateTime time = DateTime.FromBinary(Convert.ToInt64(appQuitTime));
                HPManager.instance.SetAppQuitTime(time);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadAppQuitTime Failed (" + e.Message + ")");
        }
    }

    public DateTime GetDateTime()
    {
        return dateTime;
    }

    public void SetLoadingState(bool value)
    {
        loading = value;
    }
}
