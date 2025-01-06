using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Text.RegularExpressions;


public class SystemManager : MonoBehaviour
{
    public static SystemManager instance;

    [SerializeField] private GameObject gameClosingWindow;

    [SerializeField] private Button menuButton;
    [SerializeField] private Button visitorNoteButton;
    [SerializeField] private Button plusHPButton;

    private bool completeSave = false;

    public bool CanTouch { get; private set; } = true;

    public bool IsNeedAction { get; private set; } = false;

    public bool IsUIOpen { get; set; } = false; // 메뉴판, 손님노트, 팁 노트, 설정창, 팝업창 등이 올라온 상태인지 구분

    #region 팁 관련 변수 
    [SerializeField] private GameObject bangBubble;
    [SerializeField] private GameObject tipMessageWindow;
    [SerializeField] private Button tipNoteButton;
    [SerializeField] private Animator tipNoteButtonAnimator;
    [SerializeField] private GameObject tipNote;
    [SerializeField] private Animator tipNoteAnimator;
    [SerializeField] private Text tip2;
    [SerializeField] private Text tip3;

    private int tipNum = 0; // 1 =  팁 1개 봄, 2 = 2개 봄, 3 = 팁 3개 전부 봄, 별-체력 전환 가능

    [SerializeField] private Image starLackMessage; // 별 부족 알림 메세지
    [SerializeField] private Animator starLackAnimator;
    #endregion

    #region 제제 관련
    public Image jejeBubble;
    public Animator jejeBubbleAnimator;
    public Text jejeText;
    private List<string> jejeMessages = new List<string> {
        "어서 와!",
        "밥은 먹었어?",
        "네가 하는 일이\n곧 정답이야.",
        "내일은 또 어떤 일이 생길까?",
        "샘이 숨겨져 있지 않은\n사막이어도 아름다울 순\n없을까?",
        "우린 모두 빛나고 있어.",
        "네가 웃으면 나도 웃게 돼.", // 여기까지 기본 대사
        "내가 너의 친구인 게\n항상 자랑스러워.", // 1차 해제
        "내가 어디든 갈 수 있다면\n난 너와 함께 갈래.",
        "난 항상 네 곁에 있어.",
        "진심이 버거울 땐\n가면을 써도 괜찮아.",
        "너무 급하게 달리진 마.\n그러다가 중요한 걸\n놓칠지도 몰라.", //2차 해제
        "난 항상 네 곁에 있어.",
        "널 믿어.",
        "네 인생의 주인공은\n너라는 걸 잊지 마.",
        "들었던 팁은 메뉴판 왼쪽의\n느낌표 노트를 터치하면\n다시 볼 수 있어.",
        "손님이 원하는 메뉴를\n맞추면 3, 맞추지 못하면\n1씩 평판이 증가해!", // 팁1, idx 16
        "손님들과의 추억과\n특별 메뉴는 손님노트에서\n다시 볼 수 있어!", // 팁2
        "게임 화면을 유지하고\n있으면 25~30초 간격으로\n별이 나타나!", // 팁3
        "모은 별을 체력으로 바꿔줄까?" // idx 19
    };
    #endregion

    public GameObject exchangeMessageWindow;
    public bool IsExchanging { get; private set; } = false; //별->하트 간 전환 상태

    #region 이름 설정 관련 변수
    string inputName = ""; //입력한 이름
    [SerializeField] private GameObject charNameSettingWindow;
    [SerializeField] private GameObject babyNameSettingWindow;
    [SerializeField] private GameObject inputField; // 입력창 (무명)
    [SerializeField] private GameObject babyInputField; // 입력창 (주인공)
    [SerializeField] private GameObject nameCheckingWindow; //이름 확인창
    [SerializeField] private Button charNameCheckButton;  //확인버튼
    [SerializeField] private Button babyNameCheckButton;

    string nameForNameless; //플레이어가 붙여준 무명이의 이름
    #endregion

    #region 결제 관련 변수  
    [SerializeField] private Button fishBreadButton;//붕어빵 버튼
    [SerializeField] private GameObject purchasingWindow; //붕어빵 버튼 눌렀을 때 나오는 창
    [SerializeField] private Text purchasingText;

    [SerializeField] private GameObject completePurchasingWindow;//결제 후 메세지 창
    [SerializeField] private Text completePurchasingText;
    [SerializeField] private Text littleFishBreadText;
    #endregion  

    public int MainCount { get; private set; } = 0;
    private int endingState = 0; // 1 = 엔딩 이벤트 중, 2 = 엔딩 이벤트 완료

    #region 대화 관련 변수
    [SerializeField] private GameObject panel; //대화할 때 쓰는 회색 반투명 패널

    [SerializeField] private GameObject babyTextBox;
    [SerializeField] private GameObject characterTextBox;
    [SerializeField] private Animator TextBoxAnimator; //텍스트박스 애니메이터
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        BgmManager.instance.PlayCafeBgm();

        LoadDataInfo(); // 체력 및 시간은 타임매니저에서 실행
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))//뒤로가기 버튼 누르면 종료 메세지창
            {
                if (gameClosingWindow.activeSelf == false)
                {
                    if (CanTouch)
                    {
                        SetCanTouch(false);
                    }
                    OnApplicationFocus(false);

                    Transform WarningText = gameClosingWindow.transform.Find("WarningText");
                    if (MainCount < 3)
                    {
                        WarningText.gameObject.SetActive(true); // 경고문구 활성
                    }
                    else
                    {
                        if (WarningText.gameObject.activeSelf == true)
                            WarningText.gameObject.SetActive(false);
                    }
                    gameClosingWindow.SetActive(true);
                }
            }
        }

        if (Input.GetMouseButtonDown(0))//터치마다 효과음
        {
            SEManager.instance.PlayTouchSound();
        }

        if (bangBubble.gameObject.activeSelf && tipNum == 3 && Menu.instance.IsMenuOpen(8)) //느낌표 말풍선 올라와있고 팁넘버 3 + 마지막 메뉴 잠금 해제 완료
        {
            if (HPManager.instance.GetCurrentHP() == HPManager.instance.GetMaxHP())
            {
                bangBubble.gameObject.SetActive(false); //느낌표 말풍선 없애기, 체력으로 교환할 필요가 없기 때문
            }
        }
    }

    public void DebuggingCheat()
    {
        SaveInt("PurchaseCount", 0); //인앱 결제 정보 저장

        //PlayerPrefs.Save(); //세이브
        //endingState =1;
        //SaveDataInfo();
        //Dialogue.instance.characterDC[13] = 1;
        //Dialogue.instance.characterDC[10] = 2;
        //VisitorNote.instance.friendshipInfo[8] = 10;
        //VisitorNote.instance.FriendshipInfo[10] = 15;
        //VisitorNote.instance.FriendshipInfo[8] = 10;
        //Star.instance.SetStarNum(23);
        //for (int i = 0; i < 9; ++i)
        //HPManager.instance.AddHP();
    }

    public void OnApplicationFocus(bool value)
    {
        if (value) //게임 복귀
        {
            if (MainCount > 3)//붕붕이 등장 이후면
            {
                if (!AdsManager.instance.IsWatchingAds)//광고보고 온 경우가 아닐 때
                {
                    TimeManager.instance.SetLoadingState(true);
                    if (TimeManager.instance.IsTimerNotNull())
                    {
                        TimeManager.instance.StopTimer();
                    }
                    TimeManager.instance.StartTimer();
                    Star.instance.Invoke("ActivateStarSystem", 25f);//25초 뒤 별 함수 시작
                }
                if (!CharacterManager.instance.IsInvoking("RandomVisit") && endingState != 1 && !Dialogue.instance.IsTalking())
                { // 엔딩이벤트 중이 아니어야 하고 대화 중이 아니어야함
                    CharacterManager.instance.Invoke("RandomVisit", 5f); //캐릭터 랜덤 방문

                }
            }
        }
        else //게임 이탈
        {
            if (!Setting.instance.IsReset())
            {
                if (MainCount > 3)//붕붕이 등장 이후
                {
                    TimeManager.instance.SaveAppQuitTime();
                    SaveDataInfo();
                    HPManager.instance.SaveHPInfo(); //체력, 타이머 정보 저장                
                    if (Star.instance.IsInvoking("ActivateStarSystem"))
                    {
                        Star.instance.CancelInvoke("ActivateStarSystem");//별 활성화 함수 중단
                    }
                    else
                    {
                        if (Star.instance.IsStarSystemRunning())
                        {
                            Star.instance.DeactivateStarSystem();
                        }
                    }
                    completeSave = true;
                }
            }
        }
    }

    public void SaveDataInfo()
    {
        try
        {
            SaveInt("MainCount", MainCount);
            SaveInt("NextAppearNum", CharacterManager.instance.NextAppearNum);
            SaveInt("Reputation", Menu.instance.GetReputation());
            SaveInt("EndingState", endingState);
            SaveInt("TipNum", tipNum);
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveDataInfo Failed (" + e.Message + ")");
        }
    }

    public void LoadDataInfo()
    {
        try
        {
            MainCount = LoadInt("MainCount");
            if (!PlayerPrefs.HasKey("MainCount") || MainCount < 4) // 붕붕이 방문 완료 전
            {
                CantTouchUI(); // 설정 제외한 버튼들 모두 터치 불가
                Setting.instance.DeleteUserInfo();

                MainCount = 0;
                CharacterManager.instance.NextAppearNum = 0;
                Menu.instance.SetReputation(0);
                Star.instance.SetStarNum(0);
                PlayerPrefs.Save();
                CharacterManager.instance.CanCheckTrigger();
                BeginDialogue(0);
            }
            else // 붕붕이 방문 이후, 기존 정보가 정상적으로 저장됨
            {
                CharacterManager.instance.NextAppearNum = LoadInt("NextAppearNum");
                Menu.instance.SetReputation(LoadInt("Reputation"));
                Star.instance.SetStarNum(LoadInt("StarNum"));
                endingState = LoadInt("EndingState");
                tipNum = LoadInt("TipNum");
                Dialogue.instance.SetBabyName(PlayerPrefs.GetString("BabyName"));
                nameForNameless = PlayerPrefs.GetString("NameForNameless");
                Dialogue.instance.LoadCharacterDCInfo();
                VisitorNote.instance.LoadVisitorNoteInfo();
                Menu.instance.LoadUnlockedMenuItemInfo();
                Invoke(nameof(SetTipState), 2f);
                CharacterManager.instance.SetCharacter(0); //제제 작은 캐릭터 페이드인 
                CharacterManager.instance.CanCheckTrigger();

                if (endingState != 3) //엔딩이벤트를 본 게 아니라면
                {
                    CheckEndingCondition(); //엔딩 이벤트 조건 충족 체크
                }
                if (MainCount > 6) // 또롱이 방문 이후면
                {
                    CharacterManager.instance.GetSmallCharacter(0).GetComponent<Button>().interactable = true; // 제제 터치 가능
                }

                for (int i = 1; i <= MainCount - 2; i++)//재방문 캐릭터 설정
                {
                    if (Dialogue.instance.GetCharacterDC(10) == 3)//찰스2 이벤트를 했을 시에는
                    {
                        if (i == 6)//도로시 넘버에서 찰스도로시 추가
                        {
                            CharacterManager.instance.AddToRevisit(17);
                            CharacterManager.instance.EnableRevisit();
                            continue;
                        }
                        if (i == 10)//찰스는 넘기기
                        {
                            continue;
                        }
                    }
                    CharacterManager.instance.AddToRevisit(i);
                    CharacterManager.instance.EnableRevisit();
                }
                if (MainCount >= 14)//히로디노 이상 등장했을 경우, 따로 히로디노(or 닥터펭, or 롤렝드) 추가(위에서 추가 안됨)
                {
                    int t = MainCount - 1;
                    CharacterManager.instance.AddToRevisit(t);
                    CharacterManager.instance.EnableRevisit();
                }

                Star.instance.Invoke("ActivateStarSystem", 25f);
                CharacterManager.instance.Invoke("RandomVisit", 5f);
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadDataInfo Failed (" + e.Message + ")");
        }
    }

    private void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    private int LoadInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : defaultValue;
    }

    public void BackToCafe() //대화를 끝내고 카페로 복귀
    {
        EndDialogue();

        switch (MainCount)
        {
            case 0: //제제의 튜토리얼 설명이 끝난 상황, 도리 등장 전
                CharacterManager.instance.SetCharacter(0); //제제 작은 캐릭터 설정&페이드인
                BeginDialogue(1, 3f); // 3초 뒤 도리 등장
                break;
            case 1: //도리 방문 후
                Popup.instance.SetPopupCharacter(CharacterManager.instance.GetBigCharacter(1)); //새 캐릭터 도리 팝업 세팅
                Popup.instance.OpenPopup(); //팝업 등장, 팝업 닫으면 서빙 튜토리얼 시작
                Dialogue.instance.SetCharacterNum(0); //다음 대화 캐릭터 제제(튜토리얼)
                CantTouchUI();
                break;
            case 2: // 서빙 튜토리얼 완료
                Menu.instance.ReactionFadeIn(Menu.instance.GetSeatNum(), 1f); // 도리 리액션 말풍선 페이드인
                Dialogue.instance.SetPanelActive(7, false);
                CharacterManager.instance.SetCharacter(0); // 제제 자리로 돌아가기
                Dialogue.instance.SetCharacterNum(2); //다음 등장 캐릭터 붕붕
                Menu.instance.GetBoardCloseButton().interactable = true; //메뉴 닫기 버튼 가능
                BeginDialogue(2, 7f); // 7초 후 붕붕 등장
                break;
            case 3: //붕붕이 등장 후
                AfterFirstMeet(MainCount);
                CanTouchUI();//메뉴판,노트, 체력 충전 버튼 터치 가능
                Star.instance.ActivateStarSystem();//바로 별 활성화 함수 시작
                break;
            case 6: //또롱 등장 후 
                AfterFirstMeet(MainCount);
                VisitorNote.instance.SetNextPageButtonActive(true); //5번째 페이지로 넘어가기 위해 다음 페이지 버튼 보이게함   
                CharacterManager.instance.GetSmallCharacter(0).GetComponent<Button>().interactable = true;//제제 터치 가능
                ShowBangBubble();// 팁 말풍선 등장
                break;
            default:
                AfterFirstMeet(MainCount);
                break;
        }
        MainCount++;

        VisitorNote.instance.SaveVisitorNoteInfo();
    }

    private void AfterFirstMeet(int mCount)
    {
        if (mCount == 9) // 샌디 방문 완료
        {
            Popup.instance.SetPopupCharacter(CharacterManager.instance.GetBigCharacter(15));//샌디 이미지2, 이미지 비율/위치 문제
        }
        else if (mCount == 15) //롤렝드 방문 완료
        {
            Popup.instance.SetPopupCharacter(CharacterManager.instance.GetBigCharacter(16));//롤렝드 이미지2
        }
        else
        {
            Popup.instance.SetPopupCharacter(CharacterManager.instance.GetBigCharacter(mCount - 1)); //새 손님 팝업           
        }
        Popup.instance.OpenPopup();

        CharacterManager.instance.NextAppearNum = mCount;

        int smallCharNum = CharacterManager.instance.CurrentCharacterNum;
        if (MainCount >= 14)
        {
            ++smallCharNum;
        }
        CharacterManager.instance.SetCharacter(smallCharNum); // 작은 캐릭터 설정 후 페이드인까지

        if (!CharacterManager.instance.IsInvoking("RandomVisit"))
        {
            CharacterManager.instance.Invoke("RandomVisit", 10f);
        }

        if (MainCount < 6)
        {
            VisitorNote.instance.OpenPage(MainCount - 2); //손님 노트 페이지 오픈
        }
        VisitorNote.instance.UpdateOpenedPages();
    }

    public void BackToCafe2(int cNum)//이벤트 대화 종료 후, n은 Dialogue의 캐릭터 넘버
    {
        EndDialogue(cNum);
        if (endingState == 1)//엔딩이벤트 본 후
        {
            HPManager.instance.SaveHPInfo();
            TimeManager.instance.SaveAppQuitTime();
            endingState = 2;//엔딩이벤트를 봤음
            SaveDataInfo();
            if (Star.instance.IsInvoking("ActivateStarSystem"))
            {
                Star.instance.CancelInvoke("ActivateStarSystem");
                if (Star.instance.IsStarSystemRunning())
                {
                    Star.instance.DeactivateStarSystem();
                }
            }
            else
            {
                if (Star.instance.IsStarSystemRunning())
                {
                    Star.instance.DeactivateStarSystem();
                }
            }
            SceneChanger.instance.Invoke("GoEndingCreditScene", 1f);
            return;
        }
        else
        {
            CanTouchUI();
            CharacterManager.instance.CurrentEventState = 0; //친밀도 이벤트 종료됨 
            MenuHint.instance.CanTouchMHB();//메뉴힌트버블 터치 가능

            switch (cNum)
            {
                case 10:
                    CharacterManager.instance.FaceNum = cNum;
                    if (Dialogue.instance.GetCharacterDC(cNum) == 2)//찰스1 이벤트
                    {
                        CharacterManager.instance.CanTouchCharacter(6);
                        Menu.instance.MenuFadeOut();
                    }
                    else if (Dialogue.instance.GetCharacterDC(cNum) == 3)//찰스2 이벤트
                    {
                        CharacterManager.instance.CanTouchCharacter(6);
                        CharacterManager.instance.CanTouchCharacter(10);
                        VisitorNote.instance.ActivateReplayButton(cNum - 1);
                    }
                    break;
                case 11:
                    if (Dialogue.instance.GetCharacterDC(cNum) == 2)//무명이1 이벤트
                    {
                        CharacterManager.instance.FaceNum = 11;
                    }
                    else if (Dialogue.instance.GetCharacterDC(cNum) == 3)//무명이2 이벤트
                    {
                        CharacterManager.instance.FaceNum = 12;
                        VisitorNote.instance.SetCharacterImage(cNum, CharacterManager.instance.CharacterFaceList[cNum - 2].face[3].GetComponent<Image>().sprite);
                        VisitorNote.instance.ActivateReplayButton(cNum - 1);
                    }
                    Menu.instance.ReactionFadeIn();
                    break;
                case 12:
                    VisitorNote.instance.SetCharacterImage(cNum, CharacterManager.instance.CharacterFaceList[cNum - 2].face[0].GetComponent<Image>().sprite);
                    Menu.instance.ReactionFadeIn(); // 디노 리액션 페이드인
                    VisitorNote.instance.ActivateReplayButton(cNum - 1);
                    break;
                default:
                    Menu.instance.ReactionFadeIn();
                    if (cNum == 1 || cNum == 7 || cNum == 9 || cNum == 13)//도리, 루루, 친구, 닥터펭은 표정 비활성화 필요
                    {
                        CharacterManager.instance.FaceNum = cNum;
                    }
                    VisitorNote.instance.ActivateReplayButton(cNum - 1);

                    if (cNum == 1)//도리는 손님노트 이미지를 2번째 표정으로 바꿈
                    {
                        VisitorNote.instance.SetCharacterImage(cNum, CharacterManager.instance.CharacterFaceList[cNum].face[1].GetComponent<Image>().sprite);
                    }
                    else if (cNum == 14)//롤렝드는 따로
                    {
                        VisitorNote.instance.SetCharacterImage(cNum, CharacterManager.instance.GetBigCharacter(17).GetComponent<Image>().sprite);
                    }
                    break;
            }

            CheckEndingCondition();

            if (!CharacterManager.instance.IsInvoking("RandomVisit"))
            {
                CharacterManager.instance.Invoke("RandomVisit", 10f);
            }
        }
    }

    public void CheckEndingCondition()
    {
        int sum = 0;
        for (int i = 1; i < 15; i++)
        {
            sum += Dialogue.instance.GetCharacterDC(i);
        }

        if (sum == 42 && Menu.instance.IsMenuOpen(8)) //캐릭터들 시나리오를 모두 봄 & 마지막 메뉴 잠금 해제 완료
        {
            if (CharacterManager.instance.IsInvoking("RandomVisit")) CharacterManager.instance.CancelInvoke("RandomVisit");
            Invoke(nameof(EndingEvent), 6f);
            endingState = 1;
            SaveInt("EndingState", endingState);
            PlayerPrefs.Save();
            CharacterManager.instance.GetSmallCharacter(0).GetComponent<Button>().interactable = false;//제제 클릭 불가
        }
    }

    public void AfterRePlayStroy()//다시보기를 마친 후 실행
    {
        VisitorNote.instance.SetReplayState(2);
        VisitorNote.instance.ShowVisitorNote();
        EndDialogue();
        if (VisitorNote.instance.GetFirstMeetID() != 0)//첫 만남 다시보기 이후면
        {
            Dialogue.instance.SetCharacterDC(VisitorNote.instance.GetFirstMeetID(), 3);// 원래 값으로 돌려놓기
            VisitorNote.instance.SetFirstMeetID(0);
        }
        if (VisitorNote.instance.GetFriendEventID() != 0)
        {
            int id = VisitorNote.instance.GetFriendEventID();
            if (id <= 10)//찰스1 이벤트까지
            {
                if (id == 1 || id == 7 || id == 10)//도리, 루루, 찰스 표정 비활성화 필요
                {
                    CharacterManager.instance.FaceNum = id;
                }
                Dialogue.instance.SetCharacterDC(id, 3);// 원래 값으로 돌려놓기
            }
            else if (id == 11)//찰스2 이밴트
            {
                CharacterManager.instance.FaceNum = 10;
                Dialogue.instance.SetCharacterDC(10, 3);// 원래 값으로 돌려놓기
            }
            else if (id == 12 || id == 13)//무명이1,2 이벤트
            {
                if (id == 12)//무명이1 이벤트
                {
                    CharacterManager.instance.FaceNum = 11;
                }
                else if (id == 13)//무명이2 이벤트
                {
                    CharacterManager.instance.FaceNum = 12;
                }
                Dialogue.instance.SetCharacterDC(11, 3);// 원래 값으로 돌려놓기
            }
            else if (id >= 14)//14이상이면
            {
                if (id == 15)//닥터펭 이벤트
                {
                    CharacterManager.instance.FaceNum = 13;
                }
                Dialogue.instance.SetCharacterDC(id - 2, 3);// 원래 값으로 돌려놓기
            }

            VisitorNote.instance.SetFriendEventID(0);
        }
        Invoke(nameof(EndRePlay), 1.1f);
    }

    private void EndRePlay()//다시보기가 완전히 끝났음
    {
        VisitorNote.instance.SetReplayState(0);
    }

    private void EndingEvent()
    {
        if (!IsUIOpen && CharacterManager.instance.IsTableEmpty(1) && CharacterManager.instance.IsTableEmpty(2) && CharacterManager.instance.IsTableEmpty(3))
        { //테이블이 모두 비었고 UI가 올라와있지 않은 상태에서 실행
            CantTouchUI();
            jejeBubble.gameObject.SetActive(false);
            bangBubble.gameObject.SetActive(false);
            CharacterManager.instance.FadeOutJeje();
            CharacterManager.instance.GetSmallCharacter(0).GetComponent<Button>().interactable = false;//제제 클릭 불가
            BeginDialogue(0);
        }
        else
        {
            Invoke(nameof(EndingEvent), 2f);
        }
    }

    #region 대화 관련 함수
    public void BeginDialogue(int cNum = -1, float time = 0f)
    {
        if (BgmManager.instance.IsInvoking("PlayCafeBgm"))
        {
            BgmManager.instance.CancelInvoke("PlayCafeBgm");
        }
        if (MainCount == 1) cNum = 1;
        if (MainCount == 3) cNum = 2;

        Dialogue.instance.SetCharacterNum(cNum);
        Dialogue.instance.SetBabyText(false);
        if (cNum == 14)
            Dialogue.instance.SetBabyText(true);

        StartCoroutine(DialogueCoroutine(cNum, time));
    }

    private IEnumerator DialogueCoroutine(int cNum, float time)
    {
        yield return new WaitForSeconds(time);

        if (MainCount > 2)
        {
            BgmManager.instance.StopBgm();
            if (cNum != 14) //롤렝드의 경우 처음에 배경음악이 나오지 않음
                BgmManager.instance.PlayCharacterBGM(cNum);
        }
        if (cNum == 14) //롤렝드의 경우 나중에 등장
        {
            CharacterManager.instance.CurrentCharacterNum = 14;
        }
        else
        {
            CharacterManager.instance.CharacterIn(cNum);
        }
        panel.SetActive(true); //캐릭터가 들어옴과 동시에 회색 패널 작동
        UpTextBox();

        yield break;
    }

    public void EndDialogue(int cNum = -1) //캐릭터가 화면 밖으로 나가도록
    {
        TextBoxAnimator.SetTrigger("TextBoxDown");
        CharacterManager.instance.CharacterOut(cNum); //자동으로 나감
        Dialogue.instance.UpdateCharacterDC(cNum);
        panel.SetActive(false); //회색 패널 해제
        Invoke(nameof(DeactivateTextBox), 0.4f);
        CharacterManager.instance.Invoke(nameof(CharacterManager.instance.CanCheckTrigger), 5f);

        if (MainCount > 2)
        {
            BgmManager.instance.BgmFadeOut();
            if (endingState != 1)
                BgmManager.instance.Invoke("PlayCafeBgm", 3f);
        }
    }

    public void UpTextBox()
    {
        if (Dialogue.instance.IsBabayText())
        {
            babyTextBox.SetActive(true);
        }
        else
        {
            characterTextBox.SetActive(true);
        }

        TextBoxAnimator.SetTrigger("TextBoxUp");
        if (MainCount == 2 && Dialogue.instance.GetCurrentDialogueCount() != 0)
            Dialogue.instance.Invoke("OpenDialogue", 0.1f); //다음 대사
        else
            Dialogue.instance.OpenDialogue(); //대화 시작, 대사 띄움
    }

    public void DownTextBox() //서빙 튜토리얼 시 사용
    {
        TextBoxAnimator.SetTrigger("TextBoxDown");
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

    private void DeactivateTextBox()
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
        if (Star.instance.GetCurrentStarNum() < 3) return; // 현재 보유한 별이 3 미만이면 리턴

        if (!jejeBubble.gameObject.activeSelf && !bangBubble.gameObject.activeSelf) // 제제의 말풍선 비활성화 + 느낌표 말풍선이 안 떠있을 때만
        {
            bangBubble.gameObject.SetActive(true);
        }
    }

    public void TouchBangBubble() //느낌표 말풍선 터치 시
    {
        bangBubble.gameObject.SetActive(false);
        if (tipNum == 3) // 이미 팁을 다 봄, 별-체력 전환 팁이면
        {
            ShowJejeMessage(tipNum, true);

        }
        else //마지막 팁만 아니면 = 팁을 아직 다 안 봄
        {
            tipMessageWindow.gameObject.SetActive(true);
        }
    }

    private void ShowJejeMessage(int messageIndex, bool isTip = false)
    {
        jejeBubble.gameObject.SetActive(true);

        if (messageIndex >= 0 && messageIndex < jejeMessages.Count)
        {
            jejeText.text = jejeMessages[messageIndex];
        }
        else
        {
            Debug.LogError("Invalid message index: " + messageIndex);
            return;
        }

        if (isTip)
        {
            if (messageIndex == 0)
                tipNoteButton.gameObject.SetActive(true);
            if (messageIndex == 1)
                tip2.gameObject.SetActive(true);
            if (messageIndex == 2)
                tip3.gameObject.SetActive(true);

            if (messageIndex == 3)
                jejeBubble.GetComponent<Button>().interactable = true;
            else
            {
                tipNum++;
                SaveInt("TipNum", tipNum);
                Invoke(nameof(JejeBubbleFadeOut), 2.5f);
            }
        }

        jejeBubbleAnimator.SetTrigger("JejeBubbleIn");
        if (!isTip)
        {
            Invoke(nameof(JejeBubbleFadeOut), 2.5f);
        }
    }

    private void JejeBubbleFadeOut()
    {
        jejeBubbleAnimator.SetTrigger("JejeBubbleOut");
        Invoke(nameof(DeactiveJejeBubble), 1f);
    }

    private void DeactiveJejeBubble()
    {
        jejeBubble.gameObject.SetActive(false);
    }

    public void ShowTip() // 별을 소모해 팁 보기, 팁 메세지창에서 '네' 터치 시 실행
    {
        SEManager.instance.PlayUITouchSound();
        tipMessageWindow.gameObject.SetActive(false);

        if (Star.instance.GetCurrentStarNum() >= 3)
        {
            bangBubble.gameObject.SetActive(false);
            Star.instance.UseStar(3);
            ShowJejeMessage(tipNum, true);
        }
    }

    public void RejectTip() // 팁 보기를 원치 않음, 팁 메세지창에서 '아니오' 터치 시 실행
    {
        SEManager.instance.PlayUITouchSound();
        tipMessageWindow.gameObject.SetActive(false);

        Invoke(nameof(CheckTipState), 60f);
    }

    public void TouchTipNoteButton() // 팁 노트 버튼 눌렀을 때
    {
        if (IsUIOpen) return;
        tipNoteButton.interactable = false;
        IsUIOpen = true;
        SEManager.instance.PlayUITouchSound();
        tipNoteButtonAnimator.SetTrigger("TipButton_Up");
        tipNoteAnimator.SetTrigger("TipNote_Up");
    }

    public void CloseTipNote()
    {
        tipNoteButton.interactable = true;
        IsUIOpen = false;
        SEManager.instance.PlayUICloseSound();
        tipNoteButtonAnimator.SetTrigger("TipButton_Down");
        tipNoteAnimator.SetTrigger("TipNote_Down");
    }

    private void SetTipState()
    {
        if (tipNum != 0)
        {
            tipNoteButton.gameObject.SetActive(true);
        }
        if (tipNum == 2)
        {
            tip2.gameObject.SetActive(true);
        }
        if (tipNum == 3)
        {
            tip2.gameObject.SetActive(true);
            tip3.gameObject.SetActive(true);
        }

        CheckTipState();
    }

    public void CheckTipState()
    {
        if (tipNum <= 2 && MainCount > 6) //또롱이 등장 이후 & 팁을 다 보지 않았을 때
        {
            ShowBangBubble();
        }
        else if (tipNum == 3 && Menu.instance.IsMenuOpen(8)) // 팁 다 봄 + 마지막 메뉴 잠금 해제 상태
        {
            if (exchangeMessageWindow.activeSelf) return;
            if (Star.instance.GetCurrentStarNum() >= 5 && HPManager.instance.GetCurrentHP() != HPManager.instance.GetMaxHP()) // 별 5개 이상 + 최대 체력 아닐 때
            {
                ShowBangBubble();
            }
        }
    }

    private bool isFirstSpeaking = true;
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
                if (MainCount > 11 && MainCount <= 13)
                {
                    max = 10;
                }
                else if (MainCount >= 14)
                {
                    max = 14;
                    if (tipNum != 0) max = 15;
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
        IsExchanging = true;
        exchangeMessageWindow.SetActive(false);
        Star.instance.UseStar(5);
        HPManager.instance.AddHP();
        IsExchanging = false;
    }

    public void DisagreeToExchange() //충분한 별이 있는데 전환하지 않음
    {
        SEManager.instance.PlayUICloseSound();
        exchangeMessageWindow.SetActive(false);
        Invoke(nameof(CheckTipState), 60f);
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
        if (MainCount == 0)//아기 이름 설정일 경우
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

            if (MainCount == 0)//아기 이름 설정일 경우
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
        if (MainCount == 0)
        {
            PlayerPrefs.SetString("BabyName", inputName); //아기 이름 저장   
            Dialogue.instance.SetBabyName(inputName);
            babyNameSettingWindow.SetActive(false);//아기 이름 설정창 비활성화          
        }
        else
        {
            nameForNameless = inputName;//입력한 이름 캐릭터 이름에 대입
            PlayerPrefs.SetString("NameForNameless", inputName);//무명이 이름 저장
            VisitorNote.instance.OpenNameForNameless(inputName);//손님노트에 이름 정보 활성화
            charNameSettingWindow.SetActive(false);//무명이 이름 설정창 비활성화
        }

        nameCheckingWindow.SetActive(false);//이름 확인창 비활성화

        SetCanTouch(true, 1f);
        SetNeedAction(false);
    }

    public void UnconfirmName() // 이름 미확정
    {
        nameCheckingWindow.SetActive(false);//이름 확인창 비활성화
        if (MainCount == 0)
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
        SEManager.instance.PlayUITouchSound();
        plusHPButton.interactable = false;
        AdsManager.instance.SetAdsConsentWindowActive(true);
    }

    public void RejectAds() // 광고 시청 메세지창에서 '아니오' 터치 시 실행
    {
        SEManager.instance.PlayUICloseSound();
        plusHPButton.interactable = true;
        AdsManager.instance.SetAdsConsentWindowActive(false);
    }

    public void TouchPurchasingButton() //붕어빵 버튼(결제 버튼) 눌렀을 때 
    {
        SEManager.instance.PlayUITouchSound();
        purchasingWindow.SetActive(true);
        if (LoadInt("PurchaseCount") == 0) return;

        // 1번 이상 구매한 적이 있다면
        purchasingWindow.transform.GetChild(1).gameObject.SetActive(false); // 최대 체력 IAP버튼 비활성화
        purchasingWindow.transform.GetChild(2).gameObject.SetActive(true); // 속도 2배 IAP버튼 활성화
    }

    public void PurchasingSuccess()
    {
        SEManager.instance.PlayUITouchSound3();

        int pCount = LoadInt("PurchaseCount");
        if (pCount == 0)
            HPManager.instance.SetMaxHP20();
        else if (pCount == 1)
            HPManager.instance.SpeedUpHPRecovery();

        SaveInt("PurchaseCount", ++pCount); //인앱 결제 정보 저장

        Invoke(nameof(DeactivatePurchasingWindow), 0.1f); // 바로 비활성화하면 에러 생김
        UpdatePurchasingState(pCount);
    }

    private void DeactivatePurchasingWindow()
    {
        purchasingWindow.SetActive(false);
        AdsManager.instance.SetAdsConsentWindowActive(false);
        completePurchasingWindow.SetActive(true);
        plusHPButton.interactable = true;
    }

    public void UpdatePurchasingState(int pCount)
    {
        if (pCount == 1) // 첫 번째 결제 
        {
            //다음 인앱 결제 내용으로 바꾸기
            fishBreadButton.transform.GetChild(0).GetComponent<Text>().text = "체력 회복 속도 2배";
            NumberFormatInfo numberFormat = new CultureInfo("ko-KR", false).NumberFormat;
            purchasingText.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);

            int price = 1000;
            purchasingText.text = "체력 회복 속도를\n2배로 증가시킵니다.\n[5분에 1체력 회복]\n" + price.ToString("c", numberFormat) + "(유료)";
        }
        else if (pCount == 2)
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
        if (time == 0f)
            CanTouch = value;
        else
        {
            StartCoroutine(UpdateTouchState(value, time));
        }
    }

    private IEnumerator UpdateTouchState(bool value, float time)
    {
        yield return new WaitForSeconds(time);
        CanTouch = value;
        yield break;
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
        IsNeedAction = value;
    }

    #endregion

    #region 게임 종료 관련 함수
    public void YesGameClose()
    {
        if (LoadInt("MainCount") <= 3 && completeSave)
        {
            Application.Quit();
        }
    }

    public void CancelGameClose()
    {
        SEManager.instance.PlayUICloseSound();
        completeSave = false;
        if (CanTouch == false)
        {
            SetCanTouch(true);
        }
        OnApplicationFocus(true);//스타 시스템 다시

        if (gameClosingWindow.transform.Find("WarningText").gameObject.activeSelf == true) // 경고문구 켜져있으면 끄기
            gameClosingWindow.transform.Find("WarningText").gameObject.SetActive(false);

        gameClosingWindow.SetActive(false);
    }
    #endregion

    public int GetNumber(string name)
    {                   // Regex.Match : 정규 표현식을 사용, 문자열에서 특정 패턴을 찾아주는 기능
        return int.Parse(Regex.Match(name, @"\d+").Value); // \d+ : 하나 이상의 숫자를 의미 
    }
}
