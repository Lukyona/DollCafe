using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{
    public static HPManager instance;

    #region InGameObject
    // txt : Text, go : GameObject
    [SerializeField] private Text txtRechargeMinTimer = null;  //재충전까지 남은 분
    [SerializeField] private Text txtRechargeSecTimer = null; //재충전까지 남은 초
    [SerializeField] private GameObject goMid; // 분과 초 사이의 콜론
    [SerializeField] private GameObject goFullText; // 체력이 맥스일 때 나타나는 텍스트

    [SerializeField] private Text txtCurrentHP = null;
    #endregion

    private int currentHP = 0;
    private int maxHP = 10; // 결제 시 20

    private DateTime appQuitTime = new DateTime(1970, 1, 1).ToLocalTime(); // 게임 나간 시간
    private int rechargeIntervalMin = 10;// 체력 충전 간격(단위:분) 10분에 1 충전, 결제 시 5분
    private int rechargeIntervalSec = 60;

    private Coroutine rechargeTimerCoroutine = null;
    private int rechargeRemainMin = 0; // 작동 중인 타이머 시간(분)
    private int rechargeRemainSec = 0; // 작동 중인 타이머 시간(초)

    #region 타이머 시간 계산에 사용
    private int savedMinTimer = 0; // 저장된 기존 타이머 시간
    private int savedSecTimer = 0;
    private int remainMinTime = 0;
    private int remainSecTime = 0;
    #endregion

    public void AddHP()
    {
        if (currentHP < maxHP)
        {
            if (!SystemManager.instance.IsExchanging())//별하트 전환 상태가 아닐 때, 즉 광고 본 뒤일 때
            {
                currentHP += 2;
                TimeManager.instance.AfterWatchingAds();
            }
            else if (SystemManager.instance.IsExchanging())//별하트 전환일 때
            {
                currentHP += 1;
            }
            txtCurrentHP.text = string.Format("{0}", currentHP.ToString());
        }

        SaveHPInfo();

        if (currentHP >= maxHP)
        {
            SetFullHP();
            rechargeRemainMin = 0;
            rechargeRemainSec = 0;
        }

        SystemManager.instance.CanTouchUI();
    }

    private void Awake() //씬 플레이할 때마다 호출
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void LoadHPInfo()
    {
        try
        {
            int pCount = PlayerPrefs.GetInt("PurchaseCount");
            if (pCount != 0)
            {
                maxHP = 20;
                if (pCount == 2)
                    rechargeIntervalMin = 5;

                SystemManager.instance.UpdatePurchasingState(pCount);
            }

            if (PlayerPrefs.HasKey("CurrentHP"))
            {
                currentHP = PlayerPrefs.GetInt("CurrentHP");
                savedMinTimer = PlayerPrefs.GetInt("SavedMinTimer");
                savedSecTimer = PlayerPrefs.GetInt("SavedSecTimer");

                if (currentHP < 0)
                {
                    currentHP = 0;
                }
            }
            else
            {
                //Debug.Log("체력 정보 없음");
            }

            txtCurrentHP.text = currentHP.ToString();
            if (currentHP < maxHP)
            {
                goFullText.SetActive(false);
                txtRechargeMinTimer.gameObject.SetActive(true);
                txtRechargeSecTimer.gameObject.SetActive(true);
                goMid.SetActive(true);
            }
            else
            {
                SetFullHP();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadHPInfo Failed (" + e.Message + ")");
        }
    }

    public void SaveHPInfo() //체력 정보 저장
    {
        try
        {
            PlayerPrefs.SetInt("CurrentHP", currentHP);
            PlayerPrefs.SetInt("SavedMinTimer", rechargeRemainMin);
            PlayerPrefs.SetInt("SavedSecTimer", rechargeRemainSec);
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveHPInfo Failed (" + e.Message + ")");
        }
    }

    public void SetAppQuitTime(DateTime time)
    {
        appQuitTime = time;
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public void SetFullHP()
    {
        currentHP = maxHP;
        txtCurrentHP.text = string.Format("{0}", currentHP.ToString());

        txtRechargeMinTimer.gameObject.SetActive(false);
        txtRechargeSecTimer.gameObject.SetActive(false);
        goMid.SetActive(false);
        goFullText.SetActive(true);
        if (rechargeTimerCoroutine != null)
        {
            StopCoroutine(rechargeTimerCoroutine);
            rechargeTimerCoroutine = null;
        }
    }

    public void CalculateTime() //타이머 시간 계산 함수
    {
        if (rechargeTimerCoroutine != null)
        {
            StopCoroutine(rechargeTimerCoroutine);
        }
        savedMinTimer = PlayerPrefs.GetInt("SavedMinTimer");
        savedSecTimer = PlayerPrefs.GetInt("SavedSecTimer");

        int elapsedMin = (int)(TimeManager.instance.GetDateTime() - appQuitTime).TotalMinutes; // 게임 종료 후 경과된 시간(분)
        int elapsedSec = (int)((TimeManager.instance.GetDateTime() - appQuitTime).TotalSeconds % 60); // 게임 종료 후 경과된 시간(초)
        //print(elapsedMin + "분 " + elapsedSec + "초 지남");

        int calculatedMin = elapsedMin - savedMinTimer; // 분 계산
        int calculatedSec = elapsedSec - savedSecTimer;  // 초 계산
                                                         // Debug.Log("CalculateRemainTime : " + calculatedMin + "m " + calculatedSec + "s");

        // calculatedMin : calculatedSec 로 구분
        if (calculatedMin == 0 && calculatedSec == 0) // 0 : 0 , 저장된 타이머와 나가 있던 시간이 동일할 경우
        {
            currentHP += 1;
            remainMinTime = rechargeIntervalMin - 1; //9분
            remainSecTime = rechargeIntervalSec - 1;//59초
        }
        else if (calculatedMin <= 0 && calculatedSec < 0) // - : -  or  0 : -
        {
            remainMinTime = -calculatedMin;
            remainSecTime = -calculatedSec;
        }
        else if (calculatedMin < 0 && calculatedSec == 0) // - : 0
        {
            remainMinTime = -calculatedMin - 1;
            remainSecTime = rechargeIntervalSec - 1;
        }
        else if (calculatedMin >= 0 && calculatedSec >= 0) // + : + or + : 0 or 0 : +
        {
            currentHP += 1;
            currentHP += calculatedMin / rechargeIntervalMin;
            calculatedMin %= rechargeIntervalMin;

            if (calculatedMin > 0)
                remainMinTime = (rechargeIntervalMin - 1) - calculatedMin;
            else
                remainMinTime = rechargeIntervalMin - 1;

            if (calculatedSec > 0)
                remainSecTime = rechargeIntervalSec - calculatedSec;
            else
                remainSecTime = rechargeIntervalSec - 1;
        }
        else if ((calculatedMin > 0 && calculatedSec < 0) || (calculatedMin < 0 && calculatedSec > 0)) // + : -  or - : +
        {
            if (calculatedMin > 0)
            {
                currentHP += 1;
                currentHP += calculatedMin / rechargeIntervalMin;
                calculatedMin %= rechargeIntervalMin;

                if (calculatedMin > 10)
                {
                    remainMinTime = rechargeIntervalMin - calculatedMin;
                }
                else
                {
                    remainMinTime = Mathf.Abs(calculatedMin);
                }

                remainSecTime = -calculatedSec;

            }
            else
            {
                remainMinTime = -calculatedMin - 1;
                remainSecTime = rechargeIntervalSec - calculatedSec;
            }
        }
        //Debug.Log("RemainTime : " + remainMinTime + "m " + remainSecTime + "s"); //타이머에 표시될 시간

        if (currentHP >= maxHP)
        {
            SetFullHP();
        }
        else
        {
            if (remainMinTime >= 10)
            {
                remainMinTime = 9;
            }
            rechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(remainMinTime, remainSecTime)); // 게임 내에 보여질 타이머 코루틴 실행
        }
        txtCurrentHP.text = string.Format("{0}", currentHP.ToString());
    }

    public void UseHP()
    {
        currentHP--;
        txtCurrentHP.text = string.Format("{0}", currentHP.ToString());

        if (rechargeTimerCoroutine == null) // 코루틴이 동작하지 않고 있었다 = 이전까지 최대 체력이었다
        {
            rechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(rechargeIntervalMin - 1, rechargeIntervalSec));
            goFullText.SetActive(false); //체력 소모했으므로 절대 최대치가 아님, 따라서 풀 텍스트 안 보이게 하고 타이머 보이게
            txtRechargeMinTimer.gameObject.SetActive(true);
            txtRechargeSecTimer.gameObject.SetActive(true);
            goMid.SetActive(true);
        }
    }

    private IEnumerator DoRechargeTimer(int remainMin, int remainSec)
    {
        rechargeRemainMin = remainMin;
        rechargeRemainSec = remainSec;
        //Debug.Log("HPRechargeTimer : " + rechargeRemainMin + "m " + rechargeRemainSec + "s"); 

        txtRechargeMinTimer.text = string.Format("{0}", rechargeRemainMin);
        txtRechargeSecTimer.text = string.Format("{0}", rechargeRemainSec);

        while (rechargeRemainMin >= 0)
        {
            while (rechargeRemainSec > 0)
            {
                rechargeRemainSec -= 1;
                txtRechargeSecTimer.text = string.Format("{0}", rechargeRemainSec);
                yield return new WaitForSeconds(1f);
            }

            rechargeRemainMin -= 1;
            txtRechargeMinTimer.text = string.Format("{0}", rechargeRemainMin);

            rechargeRemainSec = rechargeIntervalSec; //초는 60으로 변경, 어차피 바로 59가 되기 때문에
        }

        currentHP++;
        txtCurrentHP.text = string.Format("{0}", currentHP.ToString());

        if (currentHP >= maxHP)
        {
            SetFullHP();
            rechargeRemainMin = 0;
            rechargeRemainSec = 0;
        }
        else
        {
            rechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(rechargeIntervalMin - 1, rechargeIntervalSec));
        }
    }

    public void HPMax20()
    {
        maxHP = 20;

        if (currentHP != 10)
        {
            SetFullHP();
        }
        currentHP = maxHP;
        txtCurrentHP.text = string.Format("{0}", currentHP.ToString());

        //print("최대 체력 20으로 증가");
    }

    public void HPSpeedUp()
    {
        rechargeIntervalMin = 5; //5분에 1체력 회복
        if (rechargeRemainMin >= 5)//현재 남은 타이머가 5분 이상이면 5분으로 만들기
        {
            rechargeRemainMin = 4;
            rechargeRemainSec = 59;
            txtRechargeMinTimer.text = string.Format("{0}", rechargeRemainMin);
            txtRechargeSecTimer.text = string.Format("{0}", rechargeRemainSec);
        }
        //print("체력 회복 속도 2배 증가");
    }

}