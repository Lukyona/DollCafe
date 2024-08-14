using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class SystemManager : MonoBehaviour
{
   public static SystemManager instance;

    public GameObject gameClosingWindow;

    public Button menuButton;
    public Button visitorNoteButton;
    public Button plusHPButton;

    bool completeSave = false;

    bool canTouch = true;

    bool needAction = false;

    bool isUIOpen = false; // 메뉴판, 손님노트, 팁 노트, 설정창, 팝업창 등이 올라온 상태인지 구분

    #region 팁 관련 변수 
    public GameObject bangBubble;
    public GameObject tipMessageWindow;
    public Button tipNoteButton;
    public Animator tipNoteButtonAnimator;
    public GameObject tipNote;
    public Animator tipNoteAnimator;
    public Text tip2;
    public Text tip3;

    int tipNum = 0; // 1 =  팁 1개 봄, 2 = 2개 봄, 3 = 팁 3개 전부 봄, 별-체력 전환 가능

    public Image starLackMessage; // 별 부족 알림 메세지
    public Animator starLackAnimator;
    #endregion

    public Image jejeBubble;
    public Animator jejeBubbleAnimator;
    public Text jejeText;

    public GameObject exchangeMessageWindow;
    bool exchanging = false; //별->하트 간 전환 상태

    #region 이름 설정 관련 변수
    string inputName = ""; //입력한 이름
    public GameObject charNameSettingWindow;
    public GameObject babyNameSettingWindow;
    public GameObject inputField; // 입력창 (무명)
    public GameObject babyInputField; // 입력창 (주인공)
    public GameObject nameCheckingWindow; //이름 확인창
    public Button charNameCheckButton;  //확인버튼
    public Button babyNameCheckButton;

    string nameForNameless; //플레이어가 붙여준 무명이의 이름
    #endregion

    #region 결제 관련 변수  
    public Button fishBreadButton;//붕어빵 버튼
    public GameObject purchasingWindow; //붕어빵 버튼 눌렀을 때 나오는 창
    public Text purchasingText; //창 메세지

    public GameObject completePurchasingWindow;//결제 후 메세지 창
    public Text completePurchasingText;//메세지 내용
    public Text littleFishBreadText;
    #endregion  

    int mainCount = 0; 
    int endingState = 0; // 1 = 엔딩 이벤트 중, 2 = 엔딩 이벤트 완료

    #region 대화 관련 변수
    public GameObject panel; //대화할 때 쓰는 회색 반투명 패널

    public GameObject babyTextBox;
    public GameObject characterTextBox;
    public Animator TBAnimator; //텍스트박스 애니메이터

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
        BgmManager.instance.PlayCafeBgm(); //카페 브금 재생

        LoadDataInfo();//저장된 데이터 불러옴, 체력 및 시간은 타임매니저에서 실행
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))//뒤로가기 버튼 누르면 종료 메세지창
            {
                if (gameClosingWindow.activeSelf == false)
                {
                    if (CanTouch())
                    {
                        SetCanTouch(false);
                    }
                    OnApplicationFocus(false);
                    if(mainCount < 3)
                    {
                        gameClosingWindow.transform.Find("WarningText").gameObject.SetActive(true); // 경고문구 활성
                    }
                    else
                    {
                        if(gameClosingWindow.transform.Find("WarningText").gameObject.activeSelf == true) // 경고문구 켜져있으면 끄기
                            gameClosingWindow.transform.Find("WarningText").gameObject.SetActive(false);
                    }
                    gameClosingWindow.SetActive(true);
                }
            } 
        }
        
        if(Input.GetMouseButtonDown(0))//터치마다 효과음
        {
            SEManager.instance.PlayTouchSound();
        }

        if(!bangBubble.gameObject.activeSelf && !jejeBubble.gameObject.activeSelf) // 제제 말풍선 + 느낌표 말풍선이 나타나있지 않을 때
        {
            if(!IsInvoking(nameof(SetTipState)) && !IsInvoking(nameof(CheckTipState)))
                CheckTipState();
        }  
        else if(bangBubble.gameObject.activeSelf && tipNum == 3 && Menu.instance.IsMenuOpen(8)) //느낌표 말풍선 올라와있고 팁넘버 3 + 마지막 메뉴 잠금 해제 완료
        {
            if(HPManager.instance.GetCurrentHP() == HPManager.instance.GetMaxHP()) //현재 체력이 최대치면 느낌표 말풍선 없애기, 체력으로 교환할 필요가 없기 때문
            {
                bangBubble.gameObject.SetActive(false);
            }
        }
    }

    public void DebuggingCheat()
    {
        //PlayerPrefs.SetInt("PurchaseCount", 0); //인앱 결제 정보 저장
        //PlayerPrefs.Save(); //세이브
        Dialogue.instance.CharacterDC[6] = 3;
        //Dialogue.instance.CharacterDC[12] = 1;
        //Dialogue.instance.CharacterDC[10] = 2;
        //VisitorNote.instance.FriendshipInfo[9] = 10;
        //VisitorNote.instance.FriendshipInfo[10] = 15;
        //VisitorNote.instance.FriendshipInfo[8] = 10;
        //Star.instance.SetStarNum(23);
        for(int i = 0; i < 10; ++i)
            HPManager.instance.AddHP();
    }

    public int GetMainCount()
    {
        return mainCount;
    }

    public void SetUIOpen(bool value)
    {
        isUIOpen = value;
    }

    public bool IsUIOpen()
    {
        return isUIOpen;
    }

    public void OnApplicationFocus(bool value)
    {
        if (value) //게임 복귀
        {            
            if (mainCount > 3)//붕붕이 등장 이후면
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
                if (!CharacterVisit.instance.IsInvoking("RandomVisit") && endingState != 1 && !UI_Assistant1.instance.talking)
                { // 엔딩이벤트 중이 아니어야 하고 대화 중이 아니어야함
                    CharacterVisit.instance.Invoke("RandomVisit", 5f); //캐릭터 랜덤 방문
                }
            }
        }
        else //게임 이탈
        {
            if(!Setting.instance.IsReset())
            {          
                if (mainCount > 3)//붕붕이 등장 이후
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
            PlayerPrefs.SetInt("Reputation", Menu.instance.GetReputation()); //평판 저장
            PlayerPrefs.SetInt("EndingState", endingState); //엔딩 상황 저장
            PlayerPrefs.SetInt("TipNum", tipNum); //팁 넘버 
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
            }
            if(!PlayerPrefs.HasKey("MainCount") || mainCount < 4) // 붕붕이 방문 완료 전
            {
                CantTouchUI(); // 설정 제외한 버튼들 모두 터치 불가
                Setting.instance.DeleteUserInfo();

                mainCount = 0;
                CharacterAppear.instance.SetNextAppearNum(0);
                Menu.instance.SetReputation(0);
                Star.instance.SetStarNum(0);
                PlayerPrefs.Save();

                BeginDialogue(0);
            }   
            else // 붕붕이 방문 이후, 기존 정보가 정상적으로 저장됨
            {
                CharacterAppear.instance.SetNextAppearNum(PlayerPrefs.GetInt("NextAppear"));
                Menu.instance.SetReputation(PlayerPrefs.GetInt("Reputation"));
                Star.instance.SetStarNum(PlayerPrefs.GetInt("StarNum"));
                endingState = PlayerPrefs.GetInt("EndingState");       
                tipNum = PlayerPrefs.GetInt("TipNum");
                Dialogue.instance.SetBabyName(PlayerPrefs.GetString("BabyName"));
                nameForNameless = PlayerPrefs.GetString("NameForNameless");
                Dialogue.instance.LoadCharacterDCInfo();
                VisitorNote.instance.LoadVisitorNoteInfo();
                Menu.instance.LoadUnlockedMenuItemInfo();                 
                Invoke(nameof(SetTipState), 2f);
                SmallFade.instance.SetCharacter(0); //제제 작은 캐릭터 페이드인 

                if (endingState != 3) //엔딩이벤트를 본 게 아니라면
                {
                    CheckEndingCondition(); //엔딩 이벤트 조건 충족 체크
                } 
                if(mainCount > 6) // 또롱이 방문 이후면
                {
                    SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = true; // 제제 터치 가능
                }

                for (int i = 11; i <= mainCount - 2; i++)//재방문 캐릭터 설정
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
                if(mainCount >= 14)//히로디노 이상 등장했을 경우, 따로 히로디노(or 닥터펭, or 롤렝드) 추가(위에서 추가 안됨)
                {
                    int t = mainCount - 1;
                    CharacterVisit.instance.revisit.Enqueue(t);
                    CharacterVisit.instance.CanRevisit();
                }

                Star.instance.Invoke("ActivateStarSystem", 25f);//25초 뒤 별 함수 시작
                //Debug.Log("스타시스템 25초 뒤 시작");
                CharacterVisit.instance.Invoke("RandomVisit", 5f); // 5초 뒤 캐릭터 랜덤 방문
            }                               

        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadDataInfo Failed (" + e.Message + ")");
        }   
    }

    public void BackToCafe() //대화를 끝내고 카페로 복귀
    {
        EndDialogue();

        switch (mainCount)
        {
            case 0: //제제의 튜토리얼 설명이 끝난 상황, 도리 등장 전
                SmallFade.instance.SetCharacter(0); //제제 작은 캐릭터 설정&페이드인
                BeginDialogue(1, 2f); // 2초 뒤 도리 등장
                break;
            case 1: //도리 방문 후
                Popup.instance.SetPopupCharacter(CharacterManager.instance.GetBigCharacter(1)); //새 캐릭터 도리 팝업 세팅
                Popup.instance.OpenPopup(); //팝업 등장, 팝업 닫으면 서빙 튜토리얼 시작
                Dialogue.instance.SetCharacterNum(0); //다음 대화 캐릭터 제제(튜토리얼)
                CantTouchUI();
                break;
            case 2: // 서빙 튜토리얼 완료
                Menu.instance.ReactionFadeIn(Menu.instance.GetSeatNum(),1f); // 도리 리액션 말풍선 페이드인
                UI_Assistant1.instance.panel7.SetActive(false);      
                SmallFade.instance.SetCharacter(0); // 제제 자리로 돌아가기
                Dialogue.instance.SetCharacterNum(2); //다음 등장 캐릭터 붕붕
                Menu.instance.GetBoardCloseButton().interactable = true; //메뉴 닫기 버튼 가능
                BeginDialogue(2, 7f); // 7초 후 붕붕 등장
                break;
            case 3: //붕붕이 등장 후
                AfterFirstMeet(mainCount);
                CanTouchUI();//메뉴판,노트, 체력 충전 버튼 터치 가능
                Star.instance.ActivateStarSystem();//바로 별 활성화 함수 시작
                break;
            case 6: //또롱 등장 후 
                AfterFirstMeet(mainCount);
                VisitorNote.instance.nextPageButton.SetActive(true); //5번째 페이지로 넘어가기 위해 다음 페이지 버튼 보이게함   
                SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = true;//제제 터치 가능
                ShowBangBubble();// 팁 말풍선 등장
                break;
            default:
                AfterFirstMeet(mainCount);
                break;
        }
        mainCount++; //메인 카운트 증가

        PlayerPrefs.SetInt("MainCount", mainCount); //현재 메인카운트 저장
        PlayerPrefs.SetInt("NextAppear", CharacterAppear.instance.GetNextAppearNum()); //다음 캐릭터 등장 번호 저장
        PlayerPrefs.Save(); //세이브
        Dialogue.instance.SaveCharacterDCInfo();
        VisitorNote.instance.SaveVisitorNoteInfo();
    }

    void AfterFirstMeet(int mCount)
    {       
        if (mCount == 9) // 샌디 방문 완료
        {
            Popup.instance.SetPopupCharacter(CharacterManager.instance.GetBigCharacter(15));//샌디 이미지2, 이미지 비율/위치 문제
        }
        else if(mCount == 15) //롤렝드 방문 완료
        {
            Popup.instance.SetPopupCharacter(CharacterManager.instance.GetBigCharacter(16));//롤렝드 이미지2
        }
        else
        {
            Popup.instance.SetPopupCharacter(CharacterManager.instance.GetBigCharacter(mCount-1)); //새 손님 팝업           
        }
        Popup.instance.OpenPopup();

        CharacterAppear.instance.SetNextAppearNum(mCount); //다음 등장 캐릭터 설정 

        int smallCharNum = CharacterManager.instance.GetCharacterNum();
        if(mainCount >= 14)
        {
            ++smallCharNum;
        }
        SmallFade.instance.SetCharacter(smallCharNum); // 작은 캐릭터 설정 후 페이드인까지
        
        if (!CharacterVisit.instance.IsInvoking("RandomVisit"))
        {
            CharacterVisit.instance.Invoke("RandomVisit", 10f); //캐릭터 랜덤 방문
        }
       
        if(mainCount < 6)
        {
            VisitorNote.instance.page[mainCount - 2].SetActive(true); //손님 노트 페이지 오픈
        }
        else if(mainCount >= 6)
        {
            VisitorNote.instance.NextPageOpen[mainCount - 6] = 1; //페이지 열렸음
        }
        VisitorNote.instance.openPage++;
    }

    public void BackToCafe2(int cNum)//이벤트 대화 종료 후, n은 Dialogue의 캐릭터 넘버
    {
        EndDialogue(cNum);
        if(endingState == 1)//엔딩이벤트 본 후
        {
            HPManager.instance.SaveHPInfo();
            TimeManager.instance.SaveAppQuitTime();
            Dialogue.instance.SaveCharacterDCInfo();
            Menu.instance.SaveUnlockedMenuItemInfo();
            VisitorNote.instance.SaveVisitorNoteInfo();           
            endingState = 2;//엔딩이벤트를 봤음
            PlayerPrefs.SetInt("EndingState", endingState);
            SaveDataInfo();
            if (Star.instance.IsInvoking("ActivateStarSystem"))
            {
                Star.instance.CancelInvoke("ActivateStarSystem");//별 활성화 함수 중단
                if (Star.instance.IsStarSystemRunning())
                {
                    Star.instance.DeactivateStarSystem();
                }
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
            SceneChanger.instance.Invoke("GoEndingCreditScene", 1f);//1초 후 엔딩크레딧 화면으로 이동
        }
        else
        {
            CanTouchUI();
            CharacterAppear.instance.eventOn = 0; //친밀도 이벤트 종료됨 
            MenuHint.instance.CanClickMHB();//메뉴힌트버블 터치 가능

            switch (cNum)
            {
                case 10:
                    CharacterManager.instance.SetFaceNum(cNum);
                    if (Dialogue.instance.CharacterDC[cNum] == 2)//찰스1 이벤트
                    {
                        SmallFade.instance.CanClickCharacter(6);//도로시 클릭 가능하게
                        Menu.instance.MenuFadeOut();//메뉴 페이드아웃
                    }
                    else if (Dialogue.instance.CharacterDC[cNum] == 3)//찰스2 이벤트
                    {
                        UI_Assistant1.instance.getMenu = 0;
                        SmallFade.instance.CanClickCharacter(6);//도로시 클릭 가능하게
                        SmallFade.instance.CanClickCharacter(10);//찰스 클릭 가능하게
                        VisitorNote.instance.RePlayButton[cNum - 1].gameObject.SetActive(true);         
                    }
                    break;
                case 11:
                    if (Dialogue.instance.CharacterDC[cNum] == 2)//무명이1 이벤트
                    {
                        CharacterManager.instance.SetFaceNum(11);
                    }
                    else if (Dialogue.instance.CharacterDC[cNum] == 3)//무명이2 이벤트
                    {
                        CharacterManager.instance.SetFaceNum(12);
                        VisitorNote.instance.characterInfo[cNum - 1].GetComponent<Image>().sprite = CharacterManager.instance.CharacterFaceList[cNum - 2].face[3].GetComponent<Image>().sprite;
                        VisitorNote.instance.RePlayButton[cNum - 1].gameObject.SetActive(true);
                    }
                    Menu.instance.ReactionFadeIn();
                    break;
                case 12:
                    VisitorNote.instance.characterInfo[cNum - 1].GetComponent<Image>().sprite = CharacterManager.instance.CharacterFaceList[cNum - 2].face[0].GetComponent<Image>().sprite;
                    Menu.instance.ReactionFadeIn(); // 디노 리액션 페이드인
                    break;
                default:   
                    Menu.instance.ReactionFadeIn();
                    if(cNum == 1 || cNum == 7 || cNum == 9 || cNum == 13)//도리, 루루, 친구, 닥터펭은 표정 비활성화 필요
                    {
                        CharacterManager.instance.SetFaceNum(cNum);
                    }
                    VisitorNote.instance.RePlayButton[cNum - 1].gameObject.SetActive(true);//다시보기 버튼 활성화

                    if(cNum == 1)//도리는 손님노트 이미지를 2번째 표정으로 바꿈
                    {
                        VisitorNote.instance.characterInfo[cNum - 1].GetComponent<Image>().sprite = CharacterManager.instance.CharacterFaceList[cNum].face[1].GetComponent<Image>().sprite;
                    }              
                    else if(cNum == 14)//롤렝드는 따로
                    {
                        VisitorNote.instance.characterInfo[cNum - 1].GetComponent<Image>().sprite = CharacterManager.instance.GetBigCharacter(17).GetComponent<Image>().sprite;
                    }
                    break;
            }

            CheckEndingCondition();

            if (!CharacterVisit.instance.IsInvoking("RandomVisit"))
            {
                CharacterVisit.instance.Invoke("RandomVisit", 12f); //캐릭터 랜덤 방문
               // Debug.Log("랜덤방문 10초 뒤");
            }
        }
        Dialogue.instance.SaveCharacterDCInfo();
        VisitorNote.instance.SaveVisitorNoteInfo();
    }

    public void CheckEndingCondition() //모든 시나리오를 봤는지 확인
    {
        int sum = 0;
        for(int i = 1; i < Dialogue.instance.CharacterDC.Length; i++)
        {
            sum += Dialogue.instance.CharacterDC[i];
        }

        if(sum == 42 && Menu.instance.IsMenuOpen(8)) //캐릭터들 시나리오를 모두 봄 & 마지막 메뉴 잠금 해제 완료
        {
            Debug.Log("엔딩 조건 충족");

            //엔딩이벤트 8초 후 시작
            Invoke("EndingEvent",8f);
            endingState = 1;
            PlayerPrefs.SetInt("EndingState", endingState);
            PlayerPrefs.Save();
            SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = false;//제제 클릭 불가
        }
    }

    public void AfterRePlayStroy()//다시보기를 마친 후 실행
    {
        VisitorNote.instance.replayOn = 2;
        VisitorNote.instance.ClickVNButton(); //노트 올라오기
        EndDialogue();
        if (VisitorNote.instance.fmRP != 0)//첫 만남 다시보기 이후면
        {            
            Dialogue.instance.CharacterDC[VisitorNote.instance.fmRP] = 3;// 원래 값으로 돌려놓기
            VisitorNote.instance.fmRP = 0;
        }
        if (VisitorNote.instance.evRP != 0)
        {
            if(VisitorNote.instance.evRP <= 10)//찰스1 이벤트까지
            {
                if (VisitorNote.instance.evRP == 1 || VisitorNote.instance.evRP == 7 || VisitorNote.instance.evRP == 10)//도리, 루루, 찰스 표정 비활성화 필요
                {
                    CharacterManager.instance.SetFaceNum(VisitorNote.instance.evRP);
                }
                Dialogue.instance.CharacterDC[VisitorNote.instance.evRP] = 3;// 원래 값으로 돌려놓기
            }
            else if(VisitorNote.instance.evRP == 11)//찰스2 이밴트
            {
                CharacterManager.instance.SetFaceNum(10);
                Dialogue.instance.CharacterDC[10] = 3;// 원래 값으로 돌려놓기
            }
            else if(VisitorNote.instance.evRP == 12 || VisitorNote.instance.evRP == 13)//무명이1,2 이벤트
            {
                if (VisitorNote.instance.evRP == 12)//무명이1 이벤트
                {
                    CharacterManager.instance.SetFaceNum(11);
                }
                else if (VisitorNote.instance.evRP == 13)//무명이2 이벤트
                {
                    CharacterManager.instance.SetFaceNum(12);
                }
                Dialogue.instance.CharacterDC[11] = 3;// 원래 값으로 돌려놓기
            }
            else if(VisitorNote.instance.evRP >= 14)//14이상이면
            {
                if (VisitorNote.instance.evRP == 15)//닥터펭 이벤트
                {
                    CharacterManager.instance.SetFaceNum(13);
                }
                Dialogue.instance.CharacterDC[VisitorNote.instance.evRP - 2] = 3;// 원래 값으로 돌려놓기
            }
            
            VisitorNote.instance.evRP = 0;
        }
        Invoke("EndRePlay", 1.1f);
    }

    void EndRePlay()//다시보기가 완전히 끝났음
    {
        VisitorNote.instance.replayOn = 0;
    }

    void EndingEvent()
    {
        if(!isUIOpen && SmallFade.instance.TableEmpty[0] == 0 && SmallFade.instance.TableEmpty[1] == 0 && SmallFade.instance.TableEmpty[2] == 0)
        { //테이블이 모두 비었고 UI가 올라와있지 않은 상태에서 실행
            CantTouchUI();
            jejeBubble.gameObject.SetActive(false);
            bangBubble.gameObject.SetActive(false);
            SmallFade.instance.FadeOutJeje();
            SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = false;//제제 클릭 불가
            BeginDialogue(0);
        }
        else
        {
            Invoke("EndingEvent", 2f);
        }
    }

    #region 대화 관련 함수
    public void BeginDialogue(int cNum = -1, float time = 0f)
    {       
        if (BgmManager.instance.IsInvoking("PlayCafeBgm")) //카페 배경음이 invoke중이면
        {
            BgmManager.instance.CancelInvoke("PlayCafeBgm");//invoke 취소
        }
        if(mainCount == 1) cNum = 1;
        if(mainCount == 3) cNum = 2;

        Dialogue.instance.SetCharacterNum(cNum);
        Dialogue.instance.SetBabyText(false);
        if(cNum == 14 && mainCount == 15)
            Dialogue.instance.SetBabyText(true);

        StartCoroutine(DialogueCoroutine(cNum, time));
    }

    IEnumerator DialogueCoroutine(int cNum, float time)
    {
        if(time != 0f)
        {
            yield return new WaitForSeconds(time);
        }
        if(mainCount > 2)
        {
            BgmManager.instance.StopBgm();
            if(cNum != 14) //롤렝드의 경우 처음에 배경음악이 나오지 않음
                BgmManager.instance.PlayCharacterBGM(cNum);//캐릭터 배경음악 재생 
        }
        if(cNum == 14) //롤렝드의 경우 나중에 등장
        {
            CharacterManager.instance.SetCharacterNum(14);
        }
        else
        {
            CharacterManager.instance.CharacterIn(cNum); 
        }
        panel.SetActive(true); //캐릭터가 들어옴과 동시에 회색 패널 작동
        UpTextBox(); // 대화창 등장

        yield break;
    }

    public void EndDialogue(int cNum = -1) //캐릭터가 화면 밖으로 나가도록
    {
        TBAnimator.SetTrigger("TextBoxDown");       
        CharacterManager.instance.CharacterOut(cNum); //자동으로 나감
        Dialogue.instance.UpdateCharacterDC(cNum);
        panel.SetActive(false); //회색 패널 해제
        Invoke(nameof(DeactivateTextBox), 0.4f);

        if(mainCount > 2)
        {
            BgmManager.instance.BGMFadeOut();
            if(endingState != 1)
                BgmManager.instance.Invoke("PlayCafeBgm", 3f);
        }
    }

    public void UpTextBox()
    {
        if(Dialogue.instance.IsBabayText()) // 주인공 대사면
        {
            babyTextBox.SetActive(true);
        }
        else 
        {
            characterTextBox.SetActive(true);
        }

        TBAnimator.SetTrigger("TextBoxUp");
        if(mainCount == 2 && UI_Assistant1.instance.GetCurrentDialogueCount() != 0)
            UI_Assistant1.instance.Invoke("OpenDialogue2", 0.1f); //다음 대사
        else
            UI_Assistant1.instance.OpenDialogue(); //대화 시작, 대사 띄움
    }

    public void DownTextBox() //서빙 튜토리얼 시 사용
    {
        TBAnimator.SetTrigger("TextBoxDown");
    }

    public void ChangeToBabyTB() // 캐릭터 대사창에서 주인공 대사창으로 전환
    {
        characterTextBox.SetActive(false);
        babyTextBox.SetActive(true);
    }

    public void ChangeToCharacterTB() // 주인공 대사창에서 캐릭터 대사창으로 전환
    {
        babyTextBox.SetActive(false);
        characterTextBox.SetActive(true);   
    }

    void DeactivateTextBox()
    {
        if (characterTextBox.activeSelf == true)
        {
            characterTextBox.SetActive(false);
        }
        if (babyTextBox.activeSelf == true)
        {
            babyTextBox.SetActive(false);
        }
    }
    #endregion
    
    #region 팁 & 제제 말풍선 관련 함수
    public void ShowBangBubble() // 느낌표 말풍선 나타남
    {
        if(Star.instance.GetCurrentStarNum() < 3) return; // 현재 보유한 별이 3 미만이면 리턴

        if(!jejeBubble.gameObject.activeSelf && !bangBubble.gameObject.activeSelf) // 제제의 말풍선 비활성화 + 느낌표 말풍선이 안 떠있을 때만
        {
            bangBubble.gameObject.SetActive(true);
        }
    }

    public void TouchBangBubble() //느낌표 말풍선 터치 시
    {
        bangBubble.gameObject.SetActive(false);
        if(tipNum == 3) // 이미 팁을 다 봄, 별-체력 전환 팁이면
        {
            ShowJejeMessage(tipNum + 100);

        }
        else //마지막 팁만 아니면 = 팁을 아직 다 안 봄
        {
            tipMessageWindow.gameObject.SetActive(true);
        }
    }

    void ShowJejeMessage(int n)
    {
        jejeBubble.gameObject.SetActive(true);
        switch (n)
        {
            case 0:
                jejeText.text = "어서 와!";
                break;
            case 1:
                jejeText.text = "밥은 먹었어?";
                break;
            case 2:
                jejeText.text = "네가 하는 일이\n곧 정답이야.";
                break;
            case 3:
                jejeText.text = "내일은 또 어떤 일이 생길까?";
                break;
            case 4:
                jejeText.text = "샘이 숨겨져 있지 않은\n사막이어도 아름다울 순\n없을까?";
                break;
            case 5:
                jejeText.text = "우린 모두 빛나고 있어.";
                break;
            case 6: //여기까지 기본 대사
                jejeText.text = "네가 웃으면 나도 웃게 돼.";
                break;
            case 7: //1차 해제
                jejeText.text = "내가 너의 친구인 게\n항상 자랑스러워.";
                break;
            case 8:
                jejeText.text = "내가 어디든 갈 수 있다면\n난 너와 함께 갈래.";
                break;
            case 9:
                jejeText.text = "난 항상 네 곁에 있어.";
                break;
            case 10:
                jejeText.text = "진심이 버거울 땐\n가면을 써도 괜찮아.";
                break;
            case 11: //2차 해제
                jejeText.text = "너무 급하게 달리진 마.\n그러다가 중요한 걸\n놓칠지도 몰라.";
                break;
            case 12:
                jejeText.text = "난 항상 네 곁에 있어.";
                break;
            case 13:
                jejeText.text = "널 믿어.";
                break;
            case 14:
                jejeText.text = "네 인생의 주인공은\n너라는 걸 잊지 마.";
                break;
            case 15:
                jejeText.text = "들었던 팁은 메뉴판 왼쪽의\n느낌표 노트를 터치하면\n다시 볼 수 있어.";
                break;
            case 16:
                jejeText.text = nameForNameless + "의 얼굴이\n좀 밝아진 것 같아.";
                break;
            case 100: //여기서부터는 게임 팁
                jejeText.text = "손님이 원하는 메뉴를\n맞추면 3, 맞추지 못하면\n1씩 평판이 증가해!";
                tipNoteButton.gameObject.SetActive(true);
                break;
            case 101: // 팁2
                jejeText.text = "손님들과의 추억과\n특별 메뉴는 손님노트에서\n다시 볼 수 있어!";
                tip2.gameObject.SetActive(true);
                break;
            case 102: // 팁3
                jejeText.text = "게임 화면을 유지하고\n있으면 25~30초 간격으로\n별이 나타나!";
                tip3.gameObject.SetActive(true);
                break;
            case 103:
                jejeText.text = "모은 별을 체력으로 바꿔줄까?";
                jejeBubble.GetComponent<Button>().interactable = true; //제제 말풍선 터치 가능
                break;         
        }
        if(n >= 100 && n < 103) // 팁을 보는 상태라면
        {
            tipNum++;
            PlayerPrefs.SetInt("TipNum", tipNum);
            PlayerPrefs.Save();
        }
        jejeBubbleAnimator.SetTrigger("JejeBubbleIn");
        if(n != 103)//별-체력 전환 메세지 아니면 말풍선 페이드아웃
        {
            Invoke(nameof(JejeBubbleFadeOut), 2.5f);
        }
    }

    void JejeBubbleFadeOut()
    {
        jejeBubbleAnimator.SetTrigger("JejeBubbleOut");
        Invoke(nameof(DeactiveJejeBubble), 1f);
    }

    void DeactiveJejeBubble()
    {
        jejeBubble.gameObject.SetActive(false);
    }

    public void ShowTip() // 별을 소모해 팁 보기, 팁 메세지창에서 '네' 터치 시 실행
    {
        SEManager.instance.PlayUIClickSound(); //효과음           
        tipMessageWindow.gameObject.SetActive(false);

        if(Star.instance.GetCurrentStarNum() >= 3) // 현재 별이 3개 이상
        {
            bangBubble.gameObject.SetActive(false);
            Star.instance.UseStar(3);
            ShowJejeMessage(tipNum + 100);
        }
    }

    public void RejectTip() // 팁 보기를 원치 않음, 팁 메세지창에서 '아니오' 터치 시 실행
    {
        SEManager.instance.PlayUIClickSound(); //효과음           
        tipMessageWindow.gameObject.SetActive(false);

        Invoke(nameof(CheckTipState), 60f);
    }

    public void TouchTipNoteButton() // 팁 노트 버튼 눌렀을 때
    {
        if(isUIOpen) return;
        tipNoteButton.interactable = false;
        isUIOpen = true;
        SEManager.instance.PlayUIClickSound(); //효과음  
        tipNoteButtonAnimator.SetTrigger("TipButton_Up");
        tipNoteAnimator.SetTrigger("TipNote_Up");
    }

    public void CloseTipNote() // 팁 노트 닫기
    {
        tipNoteButton.interactable = true;
        isUIOpen = false;
        SEManager.instance.PlayUICloseSound(); //효과음
        tipNoteButtonAnimator.SetTrigger("TipButton_Down");
        tipNoteAnimator.SetTrigger("TipNote_Down");
    }

    void SetTipState()
    {
        if(tipNum != 0)
        {
            tipNoteButton.gameObject.SetActive(true);
        }
        if(tipNum == 2)
        {
            tip2.gameObject.SetActive(true);
        }
        if(tipNum == 3)
        {
            tip2.gameObject.SetActive(true);
            tip3.gameObject.SetActive(true);
        }

        CheckTipState();
    }

    public void CheckTipState()
    {
        if(tipNum <= 2 && mainCount > 6) //또롱이 등장 이후 & 팁을 다 보지 않았을 때
        {
            ShowBangBubble();
        }
        else if (tipNum == 3 && Menu.instance.IsMenuOpen(8)) // 팁 다 봄 + 마지막 메뉴 잠금 해제 상태
        {
            if(exchangeMessageWindow.activeSelf) return;
            if (Star.instance.GetCurrentStarNum() >= 5 && HPManager.instance.GetCurrentHP() != HPManager.instance.GetMaxHP()) // 별 5개 이상 + 최대 체력 아닐 때
            {
                ShowBangBubble();
            }
        }
    }

    bool isFirstSpeaking = true;
    public void JejeTextMessage() //제제 터치 시 실행
    {
        if (jejeBubble.gameObject.activeSelf == false && bangBubble.gameObject.activeSelf == false) //말풍선 오브젝트가 비활성화일 때만
        {          
            if (isFirstSpeaking) //게임을 켜고 난 후 처음 대사
            {
                ShowJejeMessage(0);
                isFirstSpeaking = false;
            }
            else
            {
                int max = 6;
                if(mainCount > 11 && mainCount <= 13)
                {
                    max = 10;
                }
                else if(mainCount >= 14)
                {
                    max = 14;
                }
                if (tipNum != 0)
                {
                    max = 15;
                }
                if (Dialogue.instance.CharacterDC[11] == 3)
                {
                    max = 16;
                }

                int randomNum = Random.Range(1, max); //랜덤으로 대사 출력
                ShowJejeMessage(randomNum);
            }
        }
    }
    #endregion

    #region 별-체력 전환 관련 함수
    public void WantToExchange() // (별-체력 교환) 제제 말풍선 터치 시
    {
        jejeBubble.GetComponent<Button>().interactable = false;
        exchangeMessageWindow.SetActive(true);
        Invoke(nameof(JejeBubbleFadeOut), 0.5f);
    }

    public void AgreeToExchange() //별-체력 전환 동의
    {
        SEManager.instance.PlayPopupSound();
        exchanging = true;
        exchangeMessageWindow.SetActive(false);
        Star.instance.UseStar(5);//스타 5감소
        HPManager.instance.AddHP(); //체력 증가함수
        exchanging = false;
    } 
    
    public void DisagreeToExchange() //충분한 별이 있는데 전환하지 않음
    {
        SEManager.instance.PlayUICloseSound();
        exchangeMessageWindow.SetActive(false);
        Invoke(nameof(CheckTipState), 60f);
    }

    public bool IsExchanging()
    {
        return exchanging;    
    }
    #endregion

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
        if (mainCount == 0)//아기 이름 설정일 경우
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

            if (mainCount == 0)//아기 이름 설정일 경우
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
        if(mainCount == 0)
        {
            PlayerPrefs.SetString("BabyName", inputName); //아기 이름 저장   
            Dialogue.instance.SetBabyName(inputName);                          
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

        SetCanTouch(true, 1f);
        SetNeedAction(false);
    }

    public void UnconfirmName() // 이름 미확정
    {
        nameCheckingWindow.SetActive(false);//이름 확인창 비활성화
        if(mainCount == 0)
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
    
    #region 결제 관련 함수
    public void TouchHPButton()
    {
        SEManager.instance.PlayUIClickSound();
        plusHPButton.interactable = false;
        AdsManager.instance.SetAdsMessageWindowActive(true);
    }

    public void RejectAds() // 광고 시청 메세지창에서 '아니오' 터치 시 실행
    {
        SEManager.instance.PlayUICloseSound();
        plusHPButton.interactable = true;
        AdsManager.instance.SetAdsMessageWindowActive(false);
    }
    
    public void TouchPurchasingButton() //붕어빵 버튼(결제 버튼) 눌렀을 때 
    {
        SEManager.instance.PlayUIClickSound();
        purchasingWindow.SetActive(true);
        if(PlayerPrefs.GetInt("PurchaseCount") == 0) return;

        // 1번 이상 구매한 적이 있다면
        purchasingWindow.transform.GetChild(1).gameObject.SetActive(false); // 최대 체력 IAP버튼 비활성화
        purchasingWindow.transform.GetChild(2).gameObject.SetActive(true); // 속도 2배 IAP버튼 활성화
    }

    public void PurchasingSuccess()
    {
        SEManager.instance.PlayUIClickSound3();

        int pCount = PlayerPrefs.GetInt("PurchaseCount");
        if(pCount == 0)
            HPManager.instance.HPMax20();
        else if(pCount == 1)
            HPManager.instance.HPSpeedUp();

        PlayerPrefs.SetInt("PurchaseCount", ++pCount); //인앱 결제 정보 저장
        PlayerPrefs.Save(); //세이브

        Invoke(nameof(DeactivatePurchasingWindow), 0.1f); // 바로 비활성화하면 에러 생김
        UpdatePurchasingState(pCount);
    }

    void DeactivatePurchasingWindow()
    {
        purchasingWindow.SetActive(false);
        AdsManager.instance.SetAdsMessageWindowActive(false);
        completePurchasingWindow.SetActive(true);
        plusHPButton.interactable = true;
    }

    public void UpdatePurchasingState(int pCount)
    {
        if(pCount == 1) // 첫 번째 결제 
        {
            //다음 인앱 결제 내용으로 바꾸기
            fishBreadButton.transform.GetChild(0).GetComponent<Text>().text = "체력 회복 속도 2배";
            NumberFormatInfo numberFormat = new CultureInfo("ko-KR", false).NumberFormat;
            purchasingText.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);

            int price = 1000;
            purchasingText.text = "체력 회복 속도를\n2배로 증가시킵니다.\n[5분에 1체력 회복]\n" + price.ToString("c", numberFormat) + "(유료)";      
        }
        else if(pCount == 2)
        {
            littleFishBreadText.text = "배고픈 개발자들이 붕어빵을\n또 먹을 수 있게 되었습니다!";
            completePurchasingText.text = "체력의 회복 속도가\n2배 증가하였습니다!";
            fishBreadButton.interactable = false; // 결제 버튼 터치 불가
            fishBreadButton.image.sprite = fishBreadButton.transform.GetChild(1).GetComponent<Image>().sprite;
            fishBreadButton.transform.GetChild(0).gameObject.SetActive(false); // 이미지 위에 텍스트 안 보이게
        }
    }

    public void PurchasingFailed()
    {
        print("결제 실패");
        plusHPButton.interactable = true;
    }

    public void DisagreeToPurchase() //구매 안 함
    {
        SEManager.instance.PlayUICloseSound();
        purchasingWindow.SetActive(false);
        plusHPButton.interactable = true;
    }

    public void CloseCompletePurchasingWindow()//결제 완료 메세지창 닫기
    {
        SEManager.instance.PlayUICloseSound();
        completePurchasingWindow.SetActive(false);
    }
    #endregion

    #region 터치 관련 함수
    public void SetCanTouch(bool value, float time = 0f)
    {
        if(time == 0f)
            canTouch = value;
        else
        {
            StartCoroutine(UpdateTouchState(value, time));
        }
    }

    IEnumerator UpdateTouchState(bool value, float time)
    {
        yield return new WaitForSeconds(time);
        canTouch = value;
        yield break; //코루틴 종료
    }

    public bool CanTouch()
    {
        return canTouch;
    }

    public void CanTouchUI() // 버튼들 터치 가능, 설정 버튼 제외
    {
        menuButton.GetComponent<Button>().interactable = true;
        visitorNoteButton.GetComponent<Button>().interactable = true;
        plusHPButton.GetComponent<Button>().interactable = true;
    }

    public void CantTouchUI() // 버튼들 터치 불가능
    {
        menuButton.GetComponent<Button>().interactable = false;
        visitorNoteButton.GetComponent<Button>().interactable = false;
        plusHPButton.GetComponent<Button>().interactable = false;
    }

    public void SetNeedAction(bool value)
    {
        needAction = value;
    }

    public bool IsNeedAction()
    {
        return needAction;
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
        
        if(gameClosingWindow.transform.Find("WarningText").gameObject.activeSelf == true) // 경고문구 켜져있으면 끄기
            gameClosingWindow.transform.Find("WarningText").gameObject.SetActive(false);

        gameClosingWindow.SetActive(false);
    }
    #endregion
}
