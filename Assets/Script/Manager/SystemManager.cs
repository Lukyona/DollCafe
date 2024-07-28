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


    #region 이름 설정 관련 변수
    public string inputName; //입력한 이름
    public GameObject charNameSettingWindow;
    public GameObject babyNameSettingWindow;
    public GameObject inputField; // 입력창 (무명)
    public GameObject babyInputField; // 입력창 (주인공)
    public GameObject nameCheckingWindow; //이름 확인창
    public Button charNameCheckButton;  //확인버튼
    public Button babyNameCheckButton;
    #endregion

    string nameForNameless; //플레이어가 붙여준 무명이의 이름


    int mainCount = 0; 
    int endingState = 0; // 1 = 엔딩 이벤트 중, 2 = 엔딩 이벤트 완료
    int tipNum = 0;


   void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        LoadDataInfo();//저장된 데이터 불러옴

        if (GameScript1.instance.mainCount == 0)
        {
            GameScript1.instance.servingTutorial.SetActive(true);
            GameScript1.instance.FirstTutorial(); //게임의 첫 튜토리얼 실행
        }   
        if (GameScript1.instance.mainCount > 3)//붕붕이 등장 이후면
        {
            Star.instance.Invoke("ActivateStarSystem", 25f);//25초 뒤 별 함수 시작
            Debug.Log("스타시스템 25초 뒤 시작");
            if (!CharacterVisit.instance.IsInvoking("RandomVisit"))
            {
                CharacterVisit.instance.Invoke("RandomVisit", 5f); // 5초 뒤 캐릭터 랜덤 방문
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


    public int GetMainCount()
    {
        return mainCount;
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
                    Debug.Log("스타시스템 25초 뒤 시작");
                }
                if (!CharacterVisit.instance.IsInvoking("RandomVisit") && endingState != 1 && !UI_Assistant1.instance.talking)
                { // 엔딩이벤트 중이 아니어야 하고 대화 중이 아니어야함
                    CharacterVisit.instance.Invoke("RandomVisit", 5f); //캐릭터 랜덤 방문
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
                    SaveDataInfo();//데이터 저장
                    HPManager.instance.SaveHPInfo(); //체력, 타이머 정보 저장                
                    Dialogue.instance.SaveCharacterDCInfo();
                    Menu.instance.SaveUnlockedMenuItemInfo();
                    VisitorNote.instance.SaveVisitorNoteInfo();
                    if (Star.instance.IsInvoking("ActivateStarSystem"))
                    {
                        Star.instance.CancelInvoke("ActivateStarSystem");//별 활성화 함수 중단
                        //Debug.Log("스타 인보크 중 종료1");
                    }
                    else
                    {
                        if (Star.instance.IsStarSystemRunning())
                        {
                            Star.instance.DeactivateStarSystem();
                            //Debug.Log("스타 종료2");
                        }
                    }
                    completeSave = true;
                   // Debug.Log("모든 세이브 완료");
                }  
            }
        }
    }  

    public void SaveDataInfo() //게임 데이터 정보 저장
    {
        //Debug.Log("SaveDataInfo");
        try
        {
            PlayerPrefs.SetInt("MainCount", mainCount); //현재 메인카운트 저장
            PlayerPrefs.SetInt("NextAppear", CharacterAppear.instance.GetNextAppearNum()); //다음 캐릭터 등장 번호 저장
            PlayerPrefs.SetInt("Reputation", Menu.instance.reputation); //평판 저장
            PlayerPrefs.SetInt("EndingState", endingState); //엔딩 상황 저장
            PlayerPrefs.SetInt("tipNum", tipNum); //팁 넘버 
            PlayerPrefs.Save(); //세이브
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveDataInfo Failed (" + e.Message + ")");
        }
    }

    public void LoadDataInfo() //게임 데이터 정보 불러옴
    {
        //Debug.Log("LoadDataInfo");
        try
        {
            if (PlayerPrefs.HasKey("MainCount"))
            {
                mainCount = PlayerPrefs.GetInt("MainCount");                
                CharacterAppear.instance.SetNextAppearNum(PlayerPrefs.GetInt("NextAppear"));
                Menu.instance.reputation = PlayerPrefs.GetInt("Reputation");
                Star.instance.SetStarNum(PlayerPrefs.GetInt("StarNum"));
                endingState = PlayerPrefs.GetInt("EndingState");       
                tipNum = PlayerPrefs.GetInt("tipNum");
                if (mainCount > 3)//붕붕이 등장 이후면
                {
                   // Debug.Log("중요 데이터 로드");
                    SmallFade.instance.SetCharacter(0);
                    SmallFade.instance.Invoke("FadeIn", 1f); //제제 작은 캐릭터 페이드인                   
                    Dialogue.instance.LoadCharacterDCInfo();
                    VisitorNote.instance.LoadVisitorNoteInfo();
                    Menu.instance.LoadUnlockedMenuItemInfo();                 
                    //SystemManager.instance.babyName = PlayerPrefs.GetString("BabyName");
                    Invoke("CheckTip", 1.5f);//팁 확인
                    if (endingState != 3) //엔딩이벤트를 본 게 아니라면
                    {
                        CheckEndingCondition(); //엔딩 이벤트 조건 충족 체크
                    } 
                    for (int i = 1; i <= mainCount - 2; i++)//재방문 캐릭터 설정
                    {
                        if(Dialogue.instance.CharacterDC[10] == 3)//찰스2 이벤트를 했을 시에는
                        {
                            if (i == 6)//도로시 넘버에서 찰스도로시 추가
                            {
                                CharacterVisit.instance.revisit.Enqueue(17);
                                CharacterVisit.instance.CanRevisit();
                                continue;
                            }
                            if(i == 10)//찰스는 넘기기
                            {
                                continue;
                            }
                        }                      
                        CharacterVisit.instance.revisit.Enqueue(i);
                        CharacterVisit.instance.CanRevisit();
                    }
                    if(mainCount == 14 || mainCount == 15 || mainCount == 16)//닥터펭까지 등장했을 경우, 따로 닥터펭 추가(위에서 추가 안됨), 히로디노/롤렝드의 경우도 마찬가지
                    {
                        int t = mainCount - 1;
                        CharacterVisit.instance.revisit.Enqueue(t);
                        CharacterVisit.instance.CanRevisit();
                    }
                }                               
                PlayerPrefs.Save();
                if(mainCount > 6)
                {
                    SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = true; // 제제 터치 가능
                }
            }
            else
            {
                mainCount = 0;
                CharacterAppear.instance.SetNextAppearNum(0);
                Menu.instance.reputation = 0;
                Star.instance.SetStarNum(0);
            }
            Menu.instance.reputationText.text = string.Format("{0}", Menu.instance.reputation);
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadDataInfo Failed (" + e.Message + ")");
        }   
    }

    public void CheckEndingCondition() //모든 시나리오를 봤는지 확인
    {
        int sum = 0;
        for(int i = 1; i <= Dialogue.instance.CharacterDC.Length; i++)
        {
            sum += Dialogue.instance.CharacterDC[i];
        }

        if(sum == 42 && Menu.instance.menu8Open) //캐릭터들 시나리오를 모두 봄 & 마지막 메뉴 잠금 해제 완료
        {
            Debug.Log("엔딩 조건 충족");

            //엔딩이벤트 8초 후 시작
            Invoke("EndingEvent",8f);
            endingState = 1;
            SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = false;//제제 클릭 불가
        }
        
        if(sum > 20 && tipNum == 2)
        {
            Invoke("TipBubbleOn", 1.5f);
        }
    }


    #region 이름 설정 관련 함수(주인공, 무명 캐릭터)

    public void ShowBabyNameSettingWindow()
    {
        babyNameSettingWindow.SetActive(true);
    }

    public void ShowCharNameSettingWindow()
    {
        charNameSettingWindow.SetActive(true);
    }

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
            PlayerPrefs.SetString("BabyName", inputName); //아기 이름 저장                             
            babyNameSettingWindow.SetActive(false);//아기 이름 설정창 비활성화          
        }
        else
        {
            nameForNameless = inputName;//입력한 이름 캐릭터 이름에 대입
            PlayerPrefs.SetString("NameForNameless", inputName);//무명이 이름 저장
            VisitorNote.instance.NameInfoOpen(inputName);//손님노트에 이름 정보 활성화
            charNameSettingWindow.SetActive(false);//무명이 이름 설정창 비활성화
        }              

        nameCheckingWindow.SetActive(false);//이름 확인창 비활성화
        PlayerPrefs.Save();

        Invoke("SetCanTouchTrue", 1f);
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

    public string GetNameForNameless()
    {
        return nameForNameless;
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
