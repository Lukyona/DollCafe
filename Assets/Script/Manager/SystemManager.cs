using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour
{
   public static SystemManager instance;

    public GameObject GameClosingWindow;

    bool completeSave = false;

    bool canTouch = true;


    #region 이름 설정 관련 오브젝트 변수
    public string inputName; //입력한 이름
    public GameObject inputField; // 입력창 (무명)
    public GameObject babyInputField; // 입력창 (주인공)
    public GameObject nameCheckingWindow; //이름 확인창
    public Button charNameCheckButton;  //확인버튼
    public Button babyNameCheckButton;
    #endregion


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
                    if (CanTouch())
                    {
                        SetCanTouch(false);
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

    #region 이름 설정 관련 함수(주인공, 무명 캐릭터)
    public void CheckName() //이름 확인
    {
        if (GameScript1.instance.mainCount == 0)//아기 이름 설정일 경우
        {
            inputName = babyInputField.GetComponent<Text>().text; //입력받은 값 넣음
        }
        else
        {
            inputName = inputField.GetComponent<Text>().text; //입력받은 값 넣음
        }
        if (string.IsNullOrWhiteSpace(inputName) == false) //입력받은 게 없거나 완전 공백이 아니면
        {
            nameCheckingWindow.transform.GetChild(0).GetComponent<Text>().text = inputName + "?"; //이름 텍스트에 입력한 이름 넣고
            nameCheckingWindow.SetActive(true);//마지막 확인 창 활성화

            if (GameScript1.instance.mainCount == 0)//아기 이름 설정일 경우
            {
                babyNameCheckButton.interactable = false; // 이전 창의 확인 버튼 못 누르게
            }
            else //무명이일 경우
            {
                charNameCheckButton.interactable = false;
            }
        }     
    }

    public void ConfirmName()// 이름 확정
    {
        if(GameScript1.instance.mainCount == 0)
        {
            Dialogue1.instance.babyName = inputName;//입력한 이름을 아기 이름에 대입
                PlayerPrefs.SetString("BabyName", inputName); //아기 이름 저장                             
                UI_Assistant1.instance.BabyNameSetting.SetActive(false);//아기 이름 설정창 비활성화          
        }
        else
        {
                UI_Assistant1.instance.namedName = inputName;//입력한 이름 캐릭터 이름에 대입
                PlayerPrefs.SetString("NamedName", inputName);//무명이 이름 저장
                VisitorNote.instance.NameInfoOpen(inputName);//손님노트에 이름 정보 활성화
                UI_Assistant1.instance.NameSetting.SetActive(false);//무명이 이름 설정창 비활성화
        }              

        nameCheckingWindow.SetActive(false);//이름 확인창 비활성화
        PlayerPrefs.Save();

        SystemManager.instance.Invoke("SetCanTouchTrue", 1f);
    }

    public void UnconfirmName() // 이름 미확정
    {
        nameCheckingWindow.SetActive(false);//이름 확인창 비활성화
        if(GameScript1.instance.mainCount == 0)
        {
            babyNameCheckButton.interactable = true; //다시 확인 버튼 활성화
        }
        else
        {
            charNameCheckButton.interactable = true;
        }
    }
    #endregion
    
    #region 터치 관련 함수
    public void SetCanTouch(bool value)
    {
        canTouch = value;
    }

    public bool CanTouch()
    {
        return canTouch;
    }

    public void SetCanTouchTrue() // Invoke용
    {
        canTouch = true;
    }
    #endregion

    public void AfterWatchingAds()
    {
        TimeManager.instance.SetLoadingState(true);
        if (TimeManager.instance.IsTimerNotNull())
        {
            TimeManager.instance.StopTimer();
        }
        TimeManager.instance.StartTimer();
    }

    #region 게임 종료 관련 함수
    public void YesGameClose()
    {
        if(PlayerPrefs.GetInt("MainCount") <= 3 && completeSave)
        {
            Application.Quit();
        }
    }

    public void CancelGameClose()
    {
        SEManager.instance.PlayUICloseSound();
        completeSave = false;
        if (CanTouch() == false)
        {
            SetCanTouch(true);
        }
        OnApplicationFocus(true);//스타 시스템 다시
        
        if(GameClosingWindow.transform.Find("WarningText").gameObject.activeSelf == true) // 경고문구 켜져있으면 끄기
            GameClosingWindow.transform.Find("WarningText").gameObject.SetActive(false);

        GameClosingWindow.SetActive(false);
    }
    #endregion
}
