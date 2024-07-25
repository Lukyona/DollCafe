using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
   public static SystemManager instance;

    public GameObject GameClosingWindow;

   bool completeSave = false;

  void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        GameScript1.instance.LoadDataInfo();//데이터 정보 불러옴

        if (GameScript1.instance.mainCount == 0)
        {
            GameScript1.instance.servingTutorial.SetActive(true);
            GameScript1.instance.FirstTutorial(); //게임의 첫 튜토리얼 실행
        }   
        if (GameScript1.instance.mainCount > 3)//붕붕이 등장 이후면
        {
            Star.instance.Invoke("ActivateStarSystem", 25f);//25초 뒤 별 함수 시작
           // Debug.Log("스타시스템 25초 뒤 시작");
            if (!CharacterVisit.instance.IsInvoking("RandomVisit"))
            {
                CharacterVisit.instance.Invoke("RandomVisit", 5f); //캐릭터 랜덤 방문
             //  Debug.Log("랜덤방문 수동시작 5초 뒤");
            }
        }
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))//뒤로가기 버튼 누르면 종료 메세지창
            {
                if (GameClosingWindow.activeSelf == false)
                {
                    if (UserInputManager.instance.CanTouch())
                    {
                        UserInputManager.instance.SetCanTouch(false);
                    }
                    OnApplicationFocus(false);
                    if(GameScript1.instance.mainCount <= 3)
                    {
                        GameClosingWindow.transform.Find("WarningText").gameObject.SetActive(true); // 경고문구 활성
                    }
                    else
                    {
                        if(GameClosingWindow.transform.Find("WarningText").gameObject.activeSelf == true) // 경고문구 켜져있으면 끄기
                            GameClosingWindow.transform.Find("WarningText").gameObject.SetActive(false);
                    }
                    GameClosingWindow.SetActive(true);
                }
            } 
        }
        
        if(Input.GetMouseButtonDown(0))//터치마다 효과음
        {
            SEManager.instance.PlayTouchSound();
        }
    }

    public void OnApplicationFocus(bool value)
    {
        //Debug.Log("OnApplicationFocus() : " + value);
        
        if (value) //게임 복귀
        {            
            if (GameScript1.instance.mainCount > 3)//붕붕이 등장 이후면
            {
                if(!AdsManager.instance.IsWatchingAds())//광고보고 온 경우가 아닐 때
                {
                    TimeManager.instance.SetLoadingState(true);
                    if (TimeManager.instance.IsTimerNotNull())
                    {
                        TimeManager.instance.StopTimer();
                    }
                    TimeManager.instance.StartTimer();
                    Star.instance.Invoke("ActivateStarSystem", 25f);//25초 뒤 별 함수 시작
                    //Debug.Log("스타시스템 25초 뒤 시작");
                }
                if (!CharacterVisit.instance.IsInvoking("RandomVisit") && GameScript1.instance.endStory != 1 && !UI_Assistant1.instance.talking)
                {//엔딩이벤트를 보기 전이거나 보고 종료하고 다시 들어왔을 경우,대화 중이 아니어야함
                    CharacterVisit.instance.Invoke("RandomVisit", 5f); //캐릭터 랜덤 방문
                  //  Debug.Log("랜덤방문 수동시작 5초 뒤");
                }
            }
        }
        else //게임 이탈
        {
            if(!GameScript1.instance.delete)
            {          
                if (GameScript1.instance.mainCount > 3)//붕붕이 등장 이후
                {
                    TimeManager.instance.SaveAppQuitTime(); //게임 나간 시간 저장     
                    GameScript1.instance.SaveDataInfo();//데이터 저장
                   // Debug.Log("데이터 세이브");
                    HPManager.instance.SaveHPInfo(); //체력, 타이머 정보 저장                
                    Dialogue1.instance.SaveCharacterDCInfo();
                    Menu.instance.SaveUnlockedMenuItemInfo();
                    VisitorNote.instance.SaveVisitorNoteInfo();
                    if (Star.instance.IsInvoking("ActivateStarSystem"))
                    {
                        Star.instance.CancelInvoke("ActivateStarSystem");//별 활성화 함수 중단
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
                    completeSave = true;
                   // Debug.Log("모든 세이브 완료");
                }  
            }
        }
    }  


    public void AfterWatchingAds()
    {
        TimeManager.instance.SetLoadingState(true);
        if (TimeManager.instance.IsTimerNotNull())
        {
            TimeManager.instance.StopTimer();
        }
        TimeManager.instance.StartTimer();
    }

    public void YesGameClose()
    {
        if(PlayerPrefs.GetInt("MainCount") <= 3 && completeSave)
        {
            Application.Quit();
        }
    }

    public void NoGameClose()
    {
        SEManager.instance.PlayUICloseSound();
        completeSave = false;
        if (UserInputManager.instance.CanTouch() == false)
        {
            UserInputManager.instance.SetCanTouch(true);
        }
        OnApplicationFocus(true);//스타 시스템 다시
        if(GameClosingWindow.transform.Find("WarningText").gameObject.activeSelf == true) // 경고문구 켜져있으면 끄기
            GameClosingWindow.transform.Find("WarningText").gameObject.SetActive(false);
        GameClosingWindow.SetActive(false);
    }
}
