using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Runtime.ExceptionServices;

public class HPCharge : MonoBehaviour
{
    public static HPCharge instance;
    #region HP

    public Text appQuitTime = null;
    public Text HPRechargeMinTimer = null;  //재충전까지 남은 분, 게임 내에서 보임
    public Text HPRechargeSecTimer = null; //재충전까지 남은 초, 게임 내에서 보임
    public Text HPAmount = null; //체력 양
    public GameObject mid; // 분과 초 사이의 콜론
    public GameObject fullText; // 체력이 맥스일 때 나타나는 텍스트

    public int m_HPAmount = 0; //보유 체력
    private DateTime m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime(); // 게임 나간 시간
    public int MAX_HP = 10; //체력 최대값, 결제 시 20
    public int HPRechargeIntervalMin = 10;// 체력 충전 간격(단위:분) 10분에 1 충전, 결제 시 5분
    private int HPRechargeIntervalSec = 60; 
    private Coroutine m_RechargeTimerCoroutine = null;
    public int m_RechargeRemainMin = 0;
    public int m_RechargeRemainSec = 0;

    private int remainMinTimer = 0; //남은 타이머
    private int remainSecTimer = 0;
    private int calculateRemainMin = 0; //다음 충전까지 남은 시간
    private int calculateRemainSec = 0; //다음 충전까지 남은 초
    private int remainMinTime = 0;
    private int remainSecTime = 0;

    public int off = 0;

    public bool allSave = false;//모든 정보 저장 끝나면 true

    public bool load = false;//true면 로드상태

    #endregion

    public void AddHP() //광고 본 뒤 체력 추가 함수
    {
        if (m_HPAmount < MAX_HP)
        {
            if(!GameScript1.instance.changing)//별하트 전환 상태가 아닐 때, 즉 광고 본 뒤일 때
            {
                m_HPAmount += 2; //체력 + 2     
            }
            else if(GameScript1.instance.changing)//별하트 전환일 때
            {
                m_HPAmount += 1;
                GameScript1.instance.changing = false;
            }  
        }
        if (m_HPAmount >= MAX_HP)
        {
            m_HPAmount = MAX_HP;
            m_RechargeRemainMin = 0;
            m_RechargeRemainSec = 0;
            HPRechargeMinTimer.gameObject.SetActive(false);
            HPRechargeSecTimer.gameObject.SetActive(false);
            mid.SetActive(false);
            fullText.SetActive(true);
            m_RechargeTimerCoroutine = null;
        }
        HPAmount.text = string.Format("{0}", m_HPAmount.ToString());
    }

    private void Awake() //씬 플레이할 때마다 호출
    {
        Init(); //체력 초기화
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()//씬 열릴 때
    {
        off = PlayerPrefs.GetInt("GameOff");//게임 오프 정보를 불러오는 것이므로 필수
        GameScript1.instance.LoadDataInfo();//데이터 정보 불러옴
        if (GameScript1.instance.mainCount > 3)//붕붕이 등장 이후면
        {
            Star.instance.Invoke("StarSystem", 25f);//25초 뒤 별 함수 시작
           // Debug.Log("스타시스템 25초 뒤 시작");
            if (!CharacterVisit.instance.IsInvoking("RandomVisit"))
            {
                CharacterVisit.instance.Invoke("RandomVisit", 5f); //캐릭터 랜덤 방문
             //  Debug.Log("랜덤방문 수동시작 5초 뒤");
            }
        }
        PlayerPrefs.SetInt("GameOff", 0); //다시 게임을 켰으므로 off는 0    
        PlayerPrefs.Save();
        off = PlayerPrefs.GetInt("GameOff");//게임이 중간이탈인지 아예 종료하고 킨 것인지 확인
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))//뒤로가기 버튼 누르면 종료 메세지창
            {
                if (GameScript1.instance.GameClose.activeSelf == false)
                {
                    if (UI_Assistant1.instance.stop == 1)//원래 stop이 1이면
                    {
                        UI_Assistant1.instance.stop = 2;
                    }
                    else if (UI_Assistant1.instance.stop == 0)
                    {
                        UI_Assistant1.instance.stop = 1;
                    }
                    OnApplicationFocus(false);
                    if(GameScript1.instance.mainCount <= 3)
                    {
                        GameScript1.instance.GameCloseWarning.gameObject.SetActive(true);//경고문구 활성
                    }
                    else
                    {
                        GameScript1.instance.GameCloseWarning.gameObject.SetActive(false);
                    }
                    GameScript1.instance.GameClose.SetActive(true);
                }
            } 
            
            if(GameScript1.instance.close && allSave)
            {
                Debug.Log("게임 클로즈");
                Application.Quit();
            }
        }
        
        if(Input.GetMouseButtonDown(0))//터치마다 효과음
        {
            SEManager.instance.TouchSound();
        }
    }

    //게임 초기화, 중간 이탈, 중간 복귀 시 실행되는 함수
    public void OnApplicationFocus(bool value)
    {
        //Debug.Log("OnApplicationFocus() : " + value);
        
        if (value) //게임 복귀
        {            
            if (GameScript1.instance.mainCount > 3)//붕붕이 등장 이후면
            {
                if(!AdsManager.instance.addOn)//광고보고 온 경우가 아닐 때
                {
                    load = true;
                    if (TimeManager.instance.TimeCoroutine != null)
                    {
                        TimeManager.instance.StopCoroutine(TimeManager.instance.TimeCoroutine);
                    }
                    
                   TimeManager.instance.TimeSaveStart();
                    GameScript1.instance.LoadDataInfo();//데이터 정보 불러옴
                    Star.instance.Invoke("StarSystem", 25f);//25초 뒤 별 함수 시작
                    Debug.Log("스타시스템 25초 뒤 시작");
                }
                else if(AdsManager.instance.addOn)//광고 보고 온 후
                {
                    load = true;
                    if (TimeManager.instance.TimeCoroutine != null)
                    {
                        TimeManager.instance.StopCoroutine(TimeManager.instance.TimeCoroutine);
                    }
                    TimeManager.instance.TimeSaveStart();
                    Debug.Log("광고 종료");
                }
                if (!CharacterVisit.instance.IsInvoking("RandomVisit") && GameScript1.instance.endStory != 1 && !UI_Assistant1.instance.talking)
                {//엔딩이벤트를 보기 전이거나 보고 종료하고 다시 들어왔을 경우,대화 중이 아니어야함
                    CharacterVisit.instance.Invoke("RandomVisit", 5f); //캐릭터 랜덤 방문
                  //  Debug.Log("랜덤방문 수동시작 5초 뒤");
                }
            }
        }
        else //게임 이탈, 광고 볼 때도 포함
        {
            if(!GameScript1.instance.delete)
            {                
                if (GameScript1.instance.mainCount > 3 && !AdsManager.instance.addOn)//붕붕이 등장 이후, 광고를 보는 게 아닐 때만
                {
                    SaveAppQuitTime(); //게임 나간 시간 저장     
                    GameScript1.instance.SaveDataInfo();//데이터 저장
                   // Debug.Log("데이터 세이브");
                    SaveHPInfo(); //체력, 타이머 정보 저장                
                    Dialogue1.instance.SaveCharacterDCInfo();
                    Menu.instance.SaveOpenMenuInfo();
                    VisitorNote.instance.SaveVisitorNoteInfo();
                    if (Star.instance.IsInvoking("StarSystem"))
                    {
                        Star.instance.CancelInvoke("StarSystem");//별 활성화 함수 중단
                       // Debug.Log("스타 인보크 중 종료1");
                    }
                    else
                    {
                        if (Star.instance.starCoroutine != null)
                        {
                            Star.instance.StopCoroutine(Star.instance.starCoroutine);
                          //  Debug.Log("스타 종료2");
                        }
                    }
                    allSave = true;
                   // Debug.Log("모든 세이브 완료");
                }  
                else if(AdsManager.instance.addOn)
                {
                    SaveHPInfo(); //체력 정보 저장  
                    SaveAppQuitTime(); //게임 나간 시간 저장  
                }
            }
        }
    }  

    public void Init() 
    {
        m_HPAmount = MAX_HP; //체력 양 최대
        m_RechargeRemainMin = 0;
        m_RechargeRemainSec = 0;
        m_AppQuitTime = new DateTime(1970, 1, 1).ToLocalTime();
        fullText.SetActive(true); //최대 체력이므로 풀 텍스트 나타남

    }
    public bool LoadHPInfo() //체력 정보 불러옴
    {
        //Debug.Log("LoadHPInfo");
        bool result = false;
        try
        {
            if (PlayerPrefs.HasKey("HPAmount"))
            {
                m_HPAmount = PlayerPrefs.GetInt("HPAmount");
                remainMinTimer = PlayerPrefs.GetInt("RemainMinTimer");
                remainSecTimer = PlayerPrefs.GetInt("RemainSecTimer");
                Debug.Log("남았던 타이머: " + remainMinTimer + "분 " + remainSecTimer);
                int p = PlayerPrefs.GetInt("pNum");
                if(p == 1)
                {
                    MAX_HP = 20;
                    GameScript1.instance.PurchasingOneTime();
                }
                else if(p == 2)
                {
                    MAX_HP = 20;
                    HPRechargeIntervalMin = 5;
                    GameScript1.instance.PurchasingTwoTime();
                    if(remainMinTimer >= 5)
                    {
                        remainMinTimer = 4;
                    }
                }
               
                if (m_HPAmount < 0)
                {
                    m_HPAmount = 0;
                }
            }
            else
            {
                Debug.Log("체력 정보 없고");
                if (PlayerPrefs.HasKey("pNum"))
                {
                    int p = PlayerPrefs.GetInt("pNum");
                    if (p == 1)
                    {
                        MAX_HP = 20;
                        GameScript1.instance.PurchasingOneTime();
                    }
                    else if (p == 2)
                    {
                        MAX_HP = 20;
                        HPRechargeIntervalMin = 5;
                        GameScript1.instance.PurchasingTwoTime();
                        Debug.Log("결제 정보2");
                    }
                }
                m_HPAmount = MAX_HP;

            }
            HPAmount.text = m_HPAmount.ToString(); //가지고 있는 체력 양 표시
            if(m_HPAmount < MAX_HP) //현재 체력이 최대치보다 작으면
            {
                fullText.SetActive(false); //풀 텍스트 안 보임
                HPRechargeMinTimer.gameObject.SetActive(true); // 타이머 보임
                HPRechargeSecTimer.gameObject.SetActive(true);
                mid.SetActive(true);
                
            }
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadHPInfo Failed (" + e.Message + ")");
        }
        return result;
    }
    public bool SaveHPInfo() //체력 정보 저장
    {
        //Debug.Log("SaveHPInfo");
        bool result = false;
        try
        {
            PlayerPrefs.SetInt("HPAmount", m_HPAmount); //현재 체력 양 저장
            PlayerPrefs.SetInt("RemainMinTimer", remainMinTimer); //남은 분 저장
            PlayerPrefs.SetInt("RemainSecTimer", remainSecTimer); //남은 초 저장
            PlayerPrefs.Save(); //세이브
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveHPInfo Failed (" + e.Message + ")");
        }
        return result;
    }
    public bool LoadAppQuitTime() // 게임을 종료했던 시간 불러옴
    {
        //Debug.Log("LoadAppQuitTime");
        bool result = false;
        try
        {
            if (PlayerPrefs.HasKey("AppQuitTime"))
            {
                var appQuitTime = string.Empty;
                appQuitTime = PlayerPrefs.GetString("AppQuitTime");
                m_AppQuitTime = DateTime.FromBinary(Convert.ToInt64(appQuitTime));
            }
           Debug.Log(string.Format("Loaded AppQuitTime : {0}", m_AppQuitTime.ToString()));
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadAppQuitTime Failed (" + e.Message + ")");
        }
        return result;
    }

    public bool SaveAppQuitTime() //종료 시간 저장
    {
          Debug.Log("SaveAppQuitTime");
        bool result = false;
        try
        {
            var appQuitTime = TimeManager.instance.dateTime.ToBinary().ToString();
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
    public void SetRechargeScheduler(Action onFinish = null) //체력 충전 계산
    {
        if (m_RechargeTimerCoroutine != null)
        {
            StopCoroutine(m_RechargeTimerCoroutine);
        }
        var timeDifferenceInMin = (int)((TimeManager.instance.dateTime - m_AppQuitTime).TotalMinutes);
     //   Debug.Log("TimeDifference In Min :" + timeDifferenceInMin + "m");        //게임 하고 현재까지 ~분 지남
        var timeDifferenceInSec = (int)(((TimeManager.instance.dateTime - m_AppQuitTime).TotalSeconds) % 60);
        //  Debug.Log("지난 초 " + timeDifferenceInSec);
        //  Debug.Log("TimeDifference In Sec :" + timeDifferenceInSec + "s"); //게임 종료 후 현재까지 ~초 지남
        //  Debug.Log("RemainTimer : " + remainMinTimer + "m " + remainSecTimer + "s"); //종료 전 남아있던 타이머
        print(timeDifferenceInMin + "분 " + timeDifferenceInSec + "초 지남");
        calculateRemainMin =  timeDifferenceInMin - remainMinTimer;  //현재 남은 타이머 계산
        calculateRemainSec = timeDifferenceInSec - remainSecTimer;
       // Debug.Log("CalculateRemainTime : " + calculateRemainMin + "m " + calculateRemainSec + "s");

        if(calculateRemainMin == 0 && calculateRemainSec == 0) // 0 : 0 , 저장된 타이머와 나가 있던 시간이 동일할 경우
        {
            m_HPAmount += 1; // 체력 1 증가
            //Debug.Log("HPAmount : " + m_HPAmount);
            remainMinTime = HPRechargeIntervalMin - 1; //9분0
            remainSecTime = HPRechargeIntervalSec - 1;//59초
        }
        else if(calculateRemainMin <= 0  && calculateRemainSec < 0) // - : -  or  0 : -
        {
            //Debug.Log("HPAmount : " + m_HPAmount);
            remainMinTime = -(calculateRemainMin);
            remainSecTime = -(calculateRemainSec);
        }
        else if (calculateRemainMin < 0 && calculateRemainSec == 0) // - : 0
        {
            //Debug.Log("HPAmount : " + m_HPAmount);
            remainMinTime = -(calculateRemainMin) - 1;
            remainSecTime = HPRechargeIntervalSec - 1;
        }

        else if( calculateRemainMin >= 0 && calculateRemainSec >= 0) // + : + or + : 0 or 0 : +
        {
            m_HPAmount += 1;
            m_HPAmount += calculateRemainMin / HPRechargeIntervalMin;
            //Debug.Log("HPAmount : " + m_HPAmount);
            if (calculateRemainMin > 0)
            {
                remainMinTime = (HPRechargeIntervalMin - 1) - (calculateRemainMin % HPRechargeIntervalMin);
            }
            else
            {
                remainMinTime = HPRechargeIntervalMin - 1;   
            }
            if (calculateRemainSec > 0)
            {
                remainSecTime = HPRechargeIntervalSec - calculateRemainSec;
            }
            else
            {
                remainSecTime = HPRechargeIntervalSec - 1;
            }
        }
        else if((calculateRemainMin > 0 && calculateRemainSec < 0) || (calculateRemainMin < 0 && calculateRemainSec > 0)) // + : -  or - : +
        {
            if (calculateRemainMin > 0)
            {
                m_HPAmount += 1;
                if(calculateRemainMin >= 11)
                {
                    m_HPAmount += calculateRemainMin / HPRechargeIntervalMin;
                } 
                //Debug.Log("HPAmount : " + m_HPAmount); 
                if(calculateRemainMin != 10)
                {
                    remainMinTime = HPRechargeIntervalMin - (calculateRemainMin % HPRechargeIntervalMin);                
                }
                else if(calculateRemainMin == 10)
                {
                    remainMinTime = 0;
                }
                remainSecTime = -(calculateRemainSec);
            }
            else
            {
                //Debug.Log("HPAmount : " + m_HPAmount);
                remainMinTime = -(calculateRemainMin) - 1;
                remainSecTime = HPRechargeIntervalSec - calculateRemainSec;
            }
        }
        //Debug.Log("RemainTime : " + remainMinTime + "m " + remainSecTime + "s"); //타이머에 표시될 남은 시간

        if (m_HPAmount >= MAX_HP) //현재 체력 양이 최대치보다 높으면
        {
            m_HPAmount = MAX_HP; //최대치로 체력 맞춤
            HPRechargeMinTimer.gameObject.SetActive(false); //타이머 안 보이게
            HPRechargeSecTimer.gameObject.SetActive(false);
            mid.SetActive(false);
            fullText.SetActive(true); //풀 텍스트 보이게
        }
        else
        {
            if(remainMinTime >= 10)
            {
                remainMinTime = 9;
            }
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(remainMinTime, remainSecTime, onFinish));
        }
        HPAmount.text = string.Format("{0}", m_HPAmount.ToString());
        //Debug.Log("HPAmount : " + m_HPAmount);
    }
   
    public void UseHP(Action onFinish = null) //체력 소모
    {
        if (m_HPAmount <= 0) //체력이 0보다 적거나 같으면 리턴
        {
            return;
        }

        m_HPAmount--; //체력 - 1
        HPAmount.text = string.Format("{0}", m_HPAmount.ToString()); 
       
        fullText.SetActive(false); //체력 소모했으므로 절대 최대치가 아님, 따라서 풀 텍스트 안 보이게 하고 타이머 보이게
        HPRechargeMinTimer.gameObject.SetActive(true);
        HPRechargeSecTimer.gameObject.SetActive(true);
        mid.SetActive(true);


        if (m_RechargeTimerCoroutine == null)
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HPRechargeIntervalMin - 1, HPRechargeIntervalSec - 1));
        }
        if (onFinish != null)
        {
            onFinish();
        }
    }

    public void P_Max()//최대 체력 구매할 때 이미 최대체력(10)일 때 타이머 나타나기
    {
        fullText.SetActive(false); //체력 소모했으므로 절대 최대치가 아님, 따라서 풀 텍스트 안 보이게 하고 타이머 보이게
        HPRechargeMinTimer.gameObject.SetActive(true);
        HPRechargeSecTimer.gameObject.SetActive(true);
        mid.SetActive(true);


        if (m_RechargeTimerCoroutine == null)
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HPRechargeIntervalMin - 1, HPRechargeIntervalSec - 1));
        }
    }

    private IEnumerator DoRechargeTimer(int remainMin,int remainSec, Action onFinish = null) //게임 내에서 타이머 가동
    {
        //Debug.Log("DoRechargeTimer");
       
        m_RechargeRemainMin = remainMin;
  
        m_RechargeRemainSec = remainSec;
     
        //Debug.Log("HPRechargeTimer : " + m_RechargeRemainMin + "m " + m_RechargeRemainSec + "s"); //현재 재충전 타이머 상태
        HPRechargeMinTimer.text = string.Format("{0}", m_RechargeRemainMin); //타이머 표시
        HPRechargeSecTimer.text = string.Format("{0}", m_RechargeRemainSec);

        while (m_RechargeRemainMin >= 0) //남은 분이 0보다 크거나 같으면 반복
        {
            remainMinTimer = m_RechargeRemainMin; 
            //Debug.Log("HPRechargeTimer : " + m_RechargeRemainMin + "m "); //현재 타이머 분
            HPRechargeMinTimer.text = string.Format("{0}", m_RechargeRemainMin);           
            while(m_RechargeRemainSec > 0) //남은 초가 0보다 크거나 같으면 반복
            {
                remainSecTimer = m_RechargeRemainSec;
                //Debug.Log("HPRechargeTimer : " + m_RechargeRemainSec + "s"); //현재 타이머 초
                HPRechargeSecTimer.text = string.Format("{0}", m_RechargeRemainSec);
                m_RechargeRemainSec -= 1; //1씩 줄어듬
                yield return new WaitForSeconds(1f); //1초씩 기다린 후 다음 반복 실행
            }
            m_RechargeRemainMin -= 1; //남은 초가 0보다 작아졌을 때 분 - 1
            m_RechargeRemainSec = HPRechargeIntervalSec - 1; //초는 59로 변경
        }
        m_HPAmount++; //남은 분이 0보다 작아졌을 때 체력 1 증가
        if (m_HPAmount >= MAX_HP)
        {
            m_HPAmount = MAX_HP;
            m_RechargeRemainMin = 0;
            m_RechargeRemainSec = 0;
            HPRechargeMinTimer.gameObject.SetActive(false);
            HPRechargeSecTimer.gameObject.SetActive(false);
            mid.SetActive(false);
            fullText.SetActive(true);
            //Debug.Log("HPAmount reached max amount");
            m_RechargeTimerCoroutine = null;
        }
        else
        {
            m_RechargeTimerCoroutine = StartCoroutine(DoRechargeTimer(HPRechargeIntervalMin - 1, HPRechargeIntervalSec - 1, onFinish));
        }
        HPAmount.text = string.Format("{0}", m_HPAmount.ToString());
        //Debug.Log("HPAmount : " + m_HPAmount);
    }

  
}