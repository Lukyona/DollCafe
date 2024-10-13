using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{
    private const int DefaultMaxHP = 10;
    private const int PurchasedMaxHP = 20;

    public static HPManager instance;

    #region InGameObject
    [SerializeField] private Text rechargeMinText = null;  //재충전까지 남은 분
    [SerializeField] private Text rechargeSecText = null; //재충전까지 남은 초
    [SerializeField] private GameObject colon; // 분과 초 사이의 콜론
    [SerializeField] private GameObject fullText; // 체력이 맥스일 때 나타나는 텍스트

    [SerializeField] private Text currentHPText = null;
    #endregion

    private int currentHP = 0;
    private int maxHP = DefaultMaxHP; // 결제 시 20

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
            if (!SystemManager.instance.IsExchanging)//별하트 전환 상태가 아닐 때, 즉 광고 본 뒤일 때
            {
                currentHP += 2;
                TimeManager.instance.AfterWatchingAds();
            }
            else //별하트 전환일 때
            {
                currentHP += 1;
            }
            UpdateCurrentHPText();
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
                maxHP = PurchasedMaxHP;
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

            currentHPText.text = currentHP.ToString();
            if (currentHP < maxHP)
            {
                fullText.SetActive(false);
                rechargeMinText.gameObject.SetActive(true);
                rechargeSecText.gameObject.SetActive(true);
                colon.SetActive(true);
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

        rechargeMinText.gameObject.SetActive(false);
        rechargeSecText.gameObject.SetActive(false);
        colon.SetActive(false);
        fullText.SetActive(true);
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

        int calculatedMin = elapsedMin - savedMinTimer; // 분 계산
        int calculatedSec = elapsedSec - savedSecTimer;  // 초 계산

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
        UpdateCurrentHPText();
    }

    public void UseHP()
    {
        currentHP--;
        UpdateCurrentHPText();

        if (rechargeTimerCoroutine == null) // 코루틴이 동작하지 않고 있었다 = 이전까지 최대 체력이었다
        {
            rechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(rechargeIntervalMin - 1, rechargeIntervalSec));
            fullText.SetActive(false); //체력 소모했으므로 절대 최대치가 아님, 따라서 풀 텍스트 안 보이게 하고 타이머 보이게
            rechargeMinText.gameObject.SetActive(true);
            rechargeSecText.gameObject.SetActive(true);
            colon.SetActive(true);
        }
    }

    private IEnumerator DoRechargeTimer(int remainMin, int remainSec)
    {
        rechargeRemainMin = remainMin;
        rechargeRemainSec = remainSec;

        UpdateRechargeMinText();
        UpdateRechargeSecText();

        while (rechargeRemainMin >= 0 && currentHP < maxHP)
        {
            while (rechargeRemainSec > 0)
            {
                rechargeRemainSec -= 1;
                UpdateRechargeSecText();
                yield return new WaitForSeconds(1f);
            }

            rechargeRemainMin -= 1;
            UpdateRechargeMinText();

            rechargeRemainSec = rechargeIntervalSec; //초는 60으로 변경, 어차피 바로 59가 되기 때문에
        }

        currentHP++;
        UpdateCurrentHPText();

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

    private void UpdateCurrentHPText()
    {
        currentHPText.text = currentHP.ToString();
    }

    private void UpdateRechargeMinText()
    {
        rechargeMinText.text = rechargeRemainMin.ToString();
    }

    private void UpdateRechargeSecText()
    {
        rechargeSecText.text = rechargeRemainSec.ToString();
    }

    public void SetMaxHP20()
    {
        maxHP = PurchasedMaxHP;

        if (currentHP != DefaultMaxHP)
        {
            SetFullHP();
        }
        currentHP = maxHP;
        UpdateCurrentHPText();
    }

    public void SpeedUpHPRecovery()
    {
        rechargeIntervalMin = 5; //5분에 1체력 회복
        if (rechargeRemainMin >= 5)//현재 남은 타이머가 5분 이상이면 5분으로 만들기
        {
            rechargeRemainMin = 4;
            rechargeRemainSec = 59;
            UpdateRechargeMinText();
            UpdateRechargeSecText();
        }
    }
}