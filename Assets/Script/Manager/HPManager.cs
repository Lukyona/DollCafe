using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Runtime.ExceptionServices;

public class HPManager : MonoBehaviour
{
    public static HPManager instance;

    #region InGameObject
    public Text HPRechargeMinTimer = null;  //재충전까지 남은 분, 게임 내에서 보임
    public Text HPRechargeSecTimer = null; //재충전까지 남은 초, 게임 내에서 보임
    public GameObject mid; // 분과 초 사이의 콜론
    public GameObject fullText; // 체력이 맥스일 때 나타나는 텍스트

    public Text HPAmount = null; // 체력 양, 게임 내에서 보임

    #endregion

    int m_HPAmount = 0; //현재 보유 체력
    int MAX_HP = 10; //체력 최대값, 결제 시 20

    DateTime m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime(); // 게임 나간 시간
    int HPRechargeIntervalMin = 10;// 체력 충전 간격(단위:분) 10분에 1 충전, 결제 시 5분
    int HPRechargeIntervalSec = 60; 

    Coroutine m_RechargeTimerCoroutine = null;
    int m_RechargeRemainMin = 0; // 작동 중인 타이머 시간(분)
    int m_RechargeRemainSec = 0; // 작동 중인 타이머 시간(초)

    #region 타이머 시간 계산에 사용
    private int savedMinTimer = 0; // 저장된 기존 타이머 시간
    private int savedSecTimer = 0;
    private int remainMinTime = 0; 
    private int remainSecTime = 0;
    #endregion

    public void AddHP() // 체력 추가 함수
    {
        if (m_HPAmount < MAX_HP)
        {
            if(!SystemManager.instance.IsExchanging())//별하트 전환 상태가 아닐 때, 즉 광고 본 뒤일 때
            {
                m_HPAmount += 2; //체력 + 2     
            }
            else if(SystemManager.instance.IsExchanging())//별하트 전환일 때
            {
                m_HPAmount += 1;
            }  
            HPAmount.text = string.Format("{0}", m_HPAmount.ToString());
        }
        if (m_HPAmount >= MAX_HP)
        {
            SetFullHP();
            m_RechargeRemainMin = 0;
            m_RechargeRemainSec = 0;
            m_RechargeTimerCoroutine = null;
        }

        TimeManager.instance.AfterWatchingAds();
        SystemManager.instance.CanTouchUI(); // hp버튼 다시 터치 가능
    }

    private void Awake() //씬 플레이할 때마다 호출
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void LoadHPInfo() //체력 정보 불러옴
    {
        //Debug.Log("LoadHPInfo");
        try
        {
            int pCount = PlayerPrefs.GetInt("PurchaseCount");
            if(pCount != 0)
            {
                MAX_HP = 20;
                if(pCount == 2)
                    HPRechargeIntervalMin = 5;

                SystemManager.instance.UpdatePurchasingState(pCount);
            }

            if (PlayerPrefs.HasKey("HPAmount"))
            {
                m_HPAmount = PlayerPrefs.GetInt("HPAmount");
                savedMinTimer = PlayerPrefs.GetInt("SavedMinTimer");
                savedSecTimer = PlayerPrefs.GetInt("SavedSecTimer");
                Debug.Log("남았던 타이머: " + savedMinTimer + "분 " + savedSecTimer);
                    
                if (m_HPAmount < 0)
                {
                    m_HPAmount = 0;
                }
            }
            else
            {
                Debug.Log("체력 정보 없음");
            }

            HPAmount.text = m_HPAmount.ToString(); //가지고 있는 체력 양 표시
            if(m_HPAmount < MAX_HP) //현재 체력이 최대치보다 작으면
            {
                fullText.SetActive(false); //풀 텍스트 안 보임
                HPRechargeMinTimer.gameObject.SetActive(true); // 타이머 보임
                HPRechargeSecTimer.gameObject.SetActive(true);
                mid.SetActive(true);
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
        //Debug.Log("SaveHPInfo");
        try
        {
            PlayerPrefs.SetInt("HPAmount", m_HPAmount); //현재 체력 양 저장
            PlayerPrefs.SetInt("SavedMinTimer", m_RechargeRemainMin); //남은 분 저장
            PlayerPrefs.SetInt("SavedSecTimer", m_RechargeRemainSec); //남은 초 저장
            PlayerPrefs.Save(); //세이브
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveHPInfo Failed (" + e.Message + ")");
        }
    }

    public void SetAppQuitTime(DateTime time)
    {
        m_AppQuitTime = time;
    }

    public int GetCurrentHP()
    {
        return m_HPAmount;
    }

    public int GetMaxHP()
    {
        return MAX_HP;
    }

    public void SetFullHP()
    {
        m_HPAmount = MAX_HP;
        HPAmount.text = string.Format("{0}", m_HPAmount.ToString());

        HPRechargeMinTimer.gameObject.SetActive(false);
        HPRechargeSecTimer.gameObject.SetActive(false);
        mid.SetActive(false);
        fullText.SetActive(true);
    }

    public void CalculateTime() //타이머에 표시될 시간 계산 함수
    {
        if (m_RechargeTimerCoroutine != null)
        {
            StopCoroutine(m_RechargeTimerCoroutine);
        }
        savedMinTimer = PlayerPrefs.GetInt("SavedMinTimer");
        savedSecTimer = PlayerPrefs.GetInt("SavedSecTimer");

        int elapsedMin = (int)(TimeManager.instance.GetDateTime() - m_AppQuitTime).TotalMinutes; // 게임 종료 후 경과된 시간(분)
        int elapsedSec = (int)((TimeManager.instance.GetDateTime() - m_AppQuitTime).TotalSeconds % 60); // 게임 종료 후 경과된 시간(초)
        print(elapsedMin + "분 " + elapsedSec + "초 지남");

        int calculatedMin =  elapsedMin - savedMinTimer; // 분 계산
        int calculatedSec = elapsedSec - savedSecTimer;  // 초 계산
       // Debug.Log("CalculateRemainTime : " + calculatedMin + "m " + calculatedSec + "s");

        // calculatedMin : calculatedSec 로 구분
        if(calculatedMin == 0 && calculatedSec == 0) // 0 : 0 , 저장된 타이머와 나가 있던 시간이 동일할 경우
        {
            m_HPAmount += 1; // 체력 1 증가
            remainMinTime = HPRechargeIntervalMin - 1; //9분
            remainSecTime = HPRechargeIntervalSec - 1;//59초
        }
        else if(calculatedMin <= 0  && calculatedSec < 0) // - : -  or  0 : -
        {
            remainMinTime = -calculatedMin;
            remainSecTime = -calculatedSec;
        }
        else if (calculatedMin < 0 && calculatedSec == 0) // - : 0
        {
            //Debug.Log("HPAmount : " + m_HPAmount);
            remainMinTime = -calculatedMin - 1;
            remainSecTime = HPRechargeIntervalSec - 1;
        }
        else if(calculatedMin >= 0 && calculatedSec >= 0) // + : + or + : 0 or 0 : +
        {
            m_HPAmount += 1;
            m_HPAmount += calculatedMin / HPRechargeIntervalMin;
            calculatedMin %= HPRechargeIntervalMin;
            //Debug.Log("HPAmount : " + m_HPAmount);

            if (calculatedMin > 0)
                remainMinTime = (HPRechargeIntervalMin - 1) - calculatedMin;
            else
                remainMinTime = HPRechargeIntervalMin - 1;   

            if (calculatedSec > 0)
                remainSecTime = HPRechargeIntervalSec - calculatedSec;
            else
                remainSecTime = HPRechargeIntervalSec - 1;
        }
        else if((calculatedMin > 0 && calculatedSec < 0) || (calculatedMin < 0 && calculatedSec > 0)) // + : -  or - : +
        {
            if (calculatedMin > 0)
            {
                m_HPAmount += 1;
                m_HPAmount += calculatedMin / HPRechargeIntervalMin;
                calculatedMin %= HPRechargeIntervalMin;

            if (calculatedMin > 10)
            {
                remainMinTime = HPRechargeIntervalMin - calculatedMin;
            }
            else
            {
                remainMinTime = Mathf.Abs(calculatedMin);
            }

            remainSecTime = -calculatedSec;
            
            }
            else
            {
                //Debug.Log("HPAmount : " + m_HPAmount);
                remainMinTime = -calculatedMin - 1;
                remainSecTime = HPRechargeIntervalSec - calculatedSec;
            }
        }
        //Debug.Log("RemainTime : " + remainMinTime + "m " + remainSecTime + "s"); //타이머에 표시될 시간

        if (m_HPAmount >= MAX_HP) //현재 체력 양이 최대치보다 높으면
        {
            SetFullHP();
        }
        else
        {
            if(remainMinTime >= 10)
            {
                remainMinTime = 9;
            }
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(remainMinTime, remainSecTime)); // 게임 내에 보여질 타이머 코루틴 실행
        }
        HPAmount.text = string.Format("{0}", m_HPAmount.ToString());
        //Debug.Log("HPAmount : " + m_HPAmount);
    }
   
    public void UseHP() //체력 소모
    {
        m_HPAmount--; //체력 - 1
        HPAmount.text = string.Format("{0}", m_HPAmount.ToString()); 

        if (m_RechargeTimerCoroutine == null) // 코루틴이 동작하지 않고 있었다 = 이전까지 최대 체력이었다
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HPRechargeIntervalMin - 1, HPRechargeIntervalSec));
            fullText.SetActive(false); //체력 소모했으므로 절대 최대치가 아님, 따라서 풀 텍스트 안 보이게 하고 타이머 보이게
            HPRechargeMinTimer.gameObject.SetActive(true);
            HPRechargeSecTimer.gameObject.SetActive(true);
            mid.SetActive(true);
        }
    }

    private IEnumerator DoRechargeTimer(int remainMin,int remainSec) //타이머 가동
    {
        //Debug.Log("DoRechargeTimer");
        m_RechargeRemainMin = remainMin;
        m_RechargeRemainSec = remainSec;
        //Debug.Log("HPRechargeTimer : " + m_RechargeRemainMin + "m " + m_RechargeRemainSec + "s"); //현재 타이머 상태

        HPRechargeMinTimer.text = string.Format("{0}", m_RechargeRemainMin); //타이머 텍스트 표시
        HPRechargeSecTimer.text = string.Format("{0}", m_RechargeRemainSec);

        while (m_RechargeRemainMin >= 0) //남은 분이 0보다 크거나 같으면 반복
        {
            while(m_RechargeRemainSec > 0) //남은 초가 0보다 크거나 같으면 반복
            {
                m_RechargeRemainSec -= 1; //1씩 줄어듬
                HPRechargeSecTimer.text = string.Format("{0}", m_RechargeRemainSec);
                yield return new WaitForSeconds(1f); //1초씩 기다린 후 다음 반복 실행
            }

            m_RechargeRemainMin -= 1; //남은 초가 0이하일 때 1분 감소
            HPRechargeMinTimer.text = string.Format("{0}", m_RechargeRemainMin);          

            m_RechargeRemainSec = HPRechargeIntervalSec; //초는 60으로 변경, 어차피 바로 59가 되기 때문에
        }

        m_HPAmount++; //남은 분이 0보다 작아졌을 때 체력 1 증가
        HPAmount.text = string.Format("{0}", m_HPAmount.ToString());

        if (m_HPAmount >= MAX_HP)
        {
            SetFullHP();
            m_RechargeRemainMin = 0;
            m_RechargeRemainSec = 0;
            m_RechargeTimerCoroutine = null;
        }
        else
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HPRechargeIntervalMin - 1, HPRechargeIntervalSec));
        }
    }

    public void HPMax20()
    {
        MAX_HP = 20;

        if(m_HPAmount != 10)//구매 전 최대체력(10)이 아니었다면
        {
            SetFullHP();
            m_RechargeTimerCoroutine = null; // 타이머 코루틴 종료
        }
        m_HPAmount = MAX_HP;
        HPAmount.text = string.Format("{0}", m_HPAmount.ToString());

        print("최대 체력 20으로 증가");
    }

    public void HPSpeedUp()
    {
        HPRechargeIntervalMin = 5; //5분에 1체력 회복
        if(m_RechargeRemainMin >= 5)//현재 남은 타이머가 5분 이상이면 5분으로 만들기
        {
            m_RechargeRemainMin = 4;
            m_RechargeRemainSec = 59;
            HPRechargeMinTimer.text = string.Format("{0}", m_RechargeRemainMin); //타이머 표시
            HPRechargeSecTimer.text = string.Format("{0}", m_RechargeRemainSec);
        }
        print("체력 회복 속도 2배 증가");
    }
  
}