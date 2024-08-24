using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour 
{
    public static Menu instance;

    [SerializeField] Animator menuButtonAnimator;
    [SerializeField] Animator menuBoardAnimator;
    [SerializeField] Animator NoHPMessageAnimator;

    #region 게임 내 오브젝트 변수
    [SerializeField] Button boardCloseButton;

    [SerializeField] GameObject[] LockedMenuItems; // 잠금 상태인 메뉴 배열, 실루엣 이미지
    [SerializeField] GameObject[] MenuObject; //모든 메뉴, 8개

    [SerializeField] GameObject[] TableMenu; //테이블 위에 보여질 작은 메뉴 이미지

    [SerializeField] GameObject[] SpecialMenu; //친밀도 이벤트에서 서빙되는 스페셜 메뉴 이미지 배열

    [SerializeField] Button[] LockedMenuButtons; // 잠긴 메뉴 해제 버튼 배열, 0 메뉴5버튼 

    [SerializeField] GameObject[] SmileReaction; //미소 리액션 배열
    [SerializeField] GameObject[] HeartReaction; //하트 리액션 배열

    [SerializeField] GameObject[] Reaction; //캐릭터의 리액션 이미지

    [SerializeField] Text reputationText; //평판 텍스트

    [SerializeField] GameObject NoHPMessage; //게임 중간이탈, 종료 시 비활성화됨

    [SerializeField] Image NamelessDessert;//무명이 디저트 2번째 이미지
    #endregion

    Vector3[] MenuPosition = new Vector3[6]; //메뉴 위치 배열
    Vector3[] ReactionPosition = new Vector3[6]; //리액션 위치 배열

    List<GameObject> UnlockedMenuItems = new List<GameObject>(); // 현재 잠금 해제된 메뉴 리스트
    
    int reputation = 0; //평판 

    int seatNum = 0; //자리 넘버

    Queue<int> reactionFadeIn = new Queue<int>();//리액션 페이드인 시 사용
    Queue<int> reactionFadeOut = new Queue<int>();//리액션 페이드아웃 시 사용
    Queue<int> menuFadeIn = new Queue<int>();//메뉴 페이드인 시 사용
    Queue<int> menuFadeOut = new Queue<int>();//메뉴 페이드아웃 시 사용

    bool isDinoServed = false; //디노 히로가 모두 서빙을 받았을 시 페이드아웃 가능
    bool isHeroServed = false;
    bool isPrincessServed = false; //도로시 서빙, 찰스랑 같이 다닐 때부터 사용
    bool isSoldierServed = false; //찰스 서빙

    int characterNum;

    bool canStartEvent = false; //메뉴 서빙 후 친밀도 이벤트가 나오는 캐릭터의 경우, 이벤트 캐릭터가 서빙을 받으면 이벤트 시작

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;      
        }
    }

    void Start()
    {
        UnlockedMenuItems.Add(MenuObject[0]); // 기본 메뉴 3개는 잠금X
        UnlockedMenuItems.Add(MenuObject[1]);
        UnlockedMenuItems.Add(MenuObject[2]);   

        for (int i = 0; i < MenuObject.Length; i++)
        {
            MenuObject[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 1f; // 불투명한 Sprite만 터치가 가능하도록 1로 설정
        }

        // 자리별 테이블 위에 놓일 메뉴의 위치
        {
            MenuPosition[0] = new Vector3(-530,-190,0);
            MenuPosition[1] = new Vector3(-335,-190,0);
            MenuPosition[2] = new Vector3(-70,-350,0);
            MenuPosition[3] = new Vector3(160,-350,0);
            MenuPosition[4] = new Vector3(400,-190,0);
            MenuPosition[5] = new Vector3(600,-190,0);
        }

        // 자리별 리액션 말풍선이 나타날 위치
        {
            ReactionPosition[0] = new Vector3(-840,-40,0);
            ReactionPosition[1] = new Vector3(-20,-50,0);
            ReactionPosition[2] = new Vector3(-450,-225,0);
            ReactionPosition[3] = new Vector3(530,-200,0);
            ReactionPosition[4] = new Vector3(85,-50,0);
            ReactionPosition[5] = new Vector3(860,-15,0);
        }
    }

    public Button GetBoardCloseButton()
    {
        return boardCloseButton;
    }

    public GameObject GetSpecialMenu(int num)
    {
        return SpecialMenu[num];
    }

    public Sprite GetNamelessDessertSprite()
    {
        return NamelessDessert.sprite;
    }

    public void TouchMenuButton() //메뉴판 버튼 눌렀을 때
    {
        if(!SystemManager.instance.IsUIOpen()) //다른 ui가 열려있지 않다면
        {
            MenuHint.instance.CantTouchMHB();//다른 메뉴힌트버블 터치 불가
            SystemManager.instance.SetUIOpen(true);
            SEManager.instance.PlayUITouchSound(); //효과음
            menuButtonAnimator.SetTrigger("MenuButtonOut"); //메뉴 버튼 위로 올라가고
            menuBoardAnimator.SetTrigger("MenuBoardUp"); //메뉴판이 아래에서 올라옴
            for (int i = 0; i < UnlockedMenuItems.Count; i++) //메뉴 터치 불가, 서빙이 아닌 그냥 메뉴판 버튼 클릭 시에는 메뉴 클릭 못함
            {
                UnlockedMenuItems[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    public void TouchMenuCloseButton() //메뉴 닫기 버튼 눌렀을 때
    {
        SystemManager.instance.SetUIOpen(false);
        SEManager.instance.PlayUICloseSound(); //효과음
        menuBoardAnimator.SetTrigger("MenuBoardDown"); //메뉴판 아래로 내려가고
        menuButtonAnimator.SetTrigger("MenuButtonIn"); //메뉴버튼 위에서 내려옴
        MenuHint.instance.CanTouchMHB();
    }

    public void TouchMenuHint(int cNum, int sNum) //캐릭터의 메뉴 힌트 말풍선 눌렀을 때
    {
        if (!SystemManager.instance.IsUIOpen())
        {
            characterNum = cNum;
            seatNum = sNum;

            MenuHint.instance.CantTouchMHB();//다른 메뉴힌트버블 터치 불가
            SystemManager.instance.SetUIOpen(true);
            SEManager.instance.PlayUITouchSound2(); //효과음
            menuButtonAnimator.SetTrigger("MenuButtonOut"); //메뉴 버튼 위로 올라가고
            menuBoardAnimator.SetTrigger("MenuBoardUp"); //메뉴판 올라옴

            if (SystemManager.instance.GetMainCount() == 2) //서빙 튜토리얼일 경우
            {
                // 레이어 순서 원래대로 변경
                CharacterManager.instance.GetSmallCharacter(1).transform.parent.GetComponent<Canvas>().sortingOrder = 2;
                MenuHint.instance.GetHintBubble(1).transform.parent.GetComponent<Canvas>().sortingOrder = 4;

                boardCloseButton.GetComponent<Button>().interactable = false; //닫기 버튼 불가
                Dialogue.instance.SetPanelActive(6,false); //패널 없애고
                Dialogue.instance.OpenDialogue(); //다음 대사 나타남
                Invoke(nameof(CanTouchMenu), 1f);
            }
            else
            {
                CanTouchMenu();         
            }           
        }
        //Debug.Log("함수 ClickMenuHint");
    }

    public void CanTouchMenu()
    {
        if(NoHPMessage.activeSelf)
            NoHPMessage.SetActive(false);

        foreach (GameObject menu in UnlockedMenuItems)
        {
            menu.GetComponent<Button>().interactable = true;
        }
    }

    public void CantTouchMenu()
    {  
        foreach (GameObject menu in UnlockedMenuItems)
        {
            menu.GetComponent<Button>().interactable = false;
        }
    }

    private void NoHPMessageFadeOut() //체력 없음 메세지 페이드아웃
    {
        NoHPMessageAnimator.SetTrigger("NoHPFadeOut");
    }

    public int GetSeatNum()
    {
        return seatNum;
    }

    public void SetReputation(int value)
    {
        reputation = value;
        reputationText.text = string.Format("{0}", reputation);
    }

    public int GetReputation()
    {
        return reputation;
    }

    public void MenuServingFunction(int menuNum)//서빙 함수
    {
        if (HPManager.instance.GetCurrentHP() <= 0) //체력이 0보다 작거나 같으면
        {
            CantTouchMenu();
            NoHPMessage.SetActive(true); //체력 없음 메세지 보이게 함
            NoHPMessageAnimator.SetTrigger("NoHPFadeIn");
            Invoke(nameof(NoHPMessageFadeOut), 1f); //1초 뒤 메세지 페이드아웃
            Invoke(nameof(CanTouchMenu), 1.5f);           
        }
        else //체력이 1이상이면
        {
            CantTouchMenu();
            SystemManager.instance.SetUIOpen(false);
            menuBoardAnimator.SetTrigger("MenuBoardDown"); //메뉴판 아래로 내려가고
            menuButtonAnimator.SetTrigger("MenuButtonIn"); //메뉴버튼 위에서 내려옴
            MenuHint.instance.CanTouchMHB();//다른 메뉴힌트버블 터치 가능

            HPManager.instance.UseHP(); //체력 소모

            if((characterNum == 9 && CharacterManager.instance.GetCurrentEventState() == 9) || (characterNum == 10 && CharacterManager.instance.GetCurrentEventState() == 10)
                || (characterNum == 15 && CharacterManager.instance.GetCurrentEventState() == 16))
            {// 친구,찰스1,롤렝드 친밀도 이벤트일 경우 클릭된 캐릭터와 이벤트 캐릭터가 동일하면
                canStartEvent = true;
                SystemManager.instance.CantTouchUI();
                MenuHint.instance.CantTouchMHB();//뒤에 메뉴판이 떠있는 채로 이벤트 시작하는 걸 방지하기 위함
                if (MenuHint.instance.GetWantedMenuNum(seatNum) == menuNum) //캐릭터가 원하는 메뉴가 n이면, 원하는 메뉴와 플레이어가 고른 메뉴가 일치
                {
                    SEManager.instance.PlayUITouchSound3();
                    VisitorNote.instance.CheckMenuMatch(characterNum, menuNum);
                    if (CharacterManager.instance.GetCurrentEventState() == 10)//찰스의 경우 평판 증가
                    {
                        SetReputation(reputation += 3);
                        VisitorNote.instance.IncreaseFrinedshipGauge(characterNum); //서빙받은 캐릭터의 친밀도 증가
                    }
                }
                else //일치하지 않음
                {
                    SEManager.instance.PlayUITouchSound2();
                    if (CharacterManager.instance.GetCurrentEventState() == 10)
                    {
                        SetReputation(++reputation);
                        VisitorNote.instance.IncreaseFrinedshipGauge(characterNum); //서빙받은 캐릭터의 친밀도 증가
                    }
                }                           
            }
            else // 대부분의 경우
            {
                if (MenuHint.instance.GetWantedMenuNum(seatNum) == menuNum) //캐릭터가 원하는 메뉴와 원하는 메뉴와 플레이어가 고른 메뉴가 일치
                {
                    SEManager.instance.PlayUITouchSound3();
                    if(CharacterManager.instance.GetCurrentEventState() != 14)//히로디노 친밀도 이벤트가 아니면
                    {
                        CorrectMenuReaction(seatNum); //자리에 따라 맞는 메뉴 리액션 이미지 가져오기
                        SetReputation(reputation += 3);
                    }                  
                    VisitorNote.instance.CheckMenuMatch(characterNum, menuNum);
                }
                else //메뉴 불일치
                {
                    SEManager.instance.PlayUITouchSound2();
                    if (CharacterManager.instance.GetCurrentEventState() != 14) // 히로디노 이벤트 아닐 때
                    {
                        WrongMenuReaction(seatNum); //자리에 따라 틀린 메뉴 리액션 이미지 가져오기
                        SetReputation(++reputation); //원하는 메뉴가 아닌 다른 메뉴 서빙 시 평판 1 증가
                    }                  
                }
                Reaction[seatNum].SetActive(true);

                if (SystemManager.instance.GetMainCount() == 2) //서빙 튜토리얼일 경우
                {
                    Dialogue.instance.Invoke("OpenDialogue",0.5f);
                    SystemManager.instance.SetCanTouch(true,1.5f);
                    VisitorNote.instance.IncreaseFrinedshipGauge(characterNum);
                }
                else // 튜토리얼 아닌 경우
                {
                    if(characterNum == 11 && CharacterManager.instance.GetCurrentEventState() == 12)//무명이1 이벤트이고 무명이 서빙을 했을 때
                    {
                        SystemManager.instance.CantTouchUI();
                        MenuHint.instance.CantTouchMHB();//뒤에 메뉴판이 떠있는 채로 이벤트 시작하는 걸 방지하기 위함
                        canStartEvent = true;
                    }
                    else // 그 외의 경우
                    {                   
                        if(CharacterManager.instance.GetCurrentEventState() == 14 && (characterNum == 12 || characterNum == 13))  //히로디노 이벤트
                        {
                            if (characterNum == 12)
                            {
                                isHeroServed = true;
                                if(CharacterManager.instance.GetCurrentEventState() == 14)
                                    CharacterManager.instance.CanTouchCharacter(13);//디노 클릭 가능                           
                            }
                            else if (characterNum == 13)
                            {
                                isDinoServed = true;
                            }

                            if (isDinoServed && isHeroServed)// 둘 다 서빙완료했을 때, 친밀도 이벤트면 UI클릭 금지
                            {
                                MenuHint.instance.CantTouchMHB();//뒤에 메뉴판이 떠있는 채로 이벤트 시작하는 걸 방지하기 위함
                                canStartEvent = true;
                                SystemManager.instance.CantTouchUI();
                            }
                        }
                        else
                        {
                            if (CharacterManager.instance.GetCurrentEventState() != 5)
                            {
                                ReactionFadeIn(seatNum,1f);
                            }  
                        }
                        VisitorNote.instance.IncreaseFrinedshipGauge(characterNum); //서빙받은 캐릭터의 친밀도 증가                        
                    }
                }
            }          
            
            TableMenu[seatNum].SetActive(true);
            SetMenuPosition();//테이블에 올려질 메뉴 위치 설정
            MenuHint.instance.MHFadeOut(seatNum); //메뉴힌트 페이드아웃
            SetTableMenu(menuNum, seatNum); //테이블에 올려질 메뉴 세팅
            menuFadeIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
            MenuFadeIn();
        }
    }

    public void SetMenuPosition() //테이블 메뉴가 나타날 자리 설정
    {
        TableMenu[seatNum].transform.position = MenuPosition[seatNum];
        Reaction[seatNum].transform.position = ReactionPosition[seatNum];
        //Debug.Log("함수 SetMenuPosition");
    }

    public void SetTableMenu(int menuNum, int sNum) //테이블에 올려질 메뉴 이미지 설정
    {
        Vector2 size = new Vector2();
        Sprite menuImage = UnlockedMenuItems[0].GetComponent<Image>().sprite;

        if(menuNum <= 8) // 1~8 일반 메뉴
        {
            size = new Vector2(UnlockedMenuItems[menuNum-1].GetComponent<RectTransform>().rect.width, UnlockedMenuItems[menuNum-1].GetComponent<RectTransform>().rect.height); //사이즈 조정
            menuImage = UnlockedMenuItems[menuNum-1].GetComponent<Image>().sprite;
        }
        else if(menuNum >= 11 && menuNum <= 14) // 스페셜 메뉴 11도리~14개나리
        {
            if (VisitorNote.instance.GetFirstMeetID() == 0 && VisitorNote.instance.GetFriendEventID() == 0)//다시보기가 아닐 때
            {
                size = new Vector2(SpecialMenu[menuNum-11].GetComponent<RectTransform>().rect.width, SpecialMenu[menuNum-11].GetComponent<RectTransform>().rect.height); //사이즈 조정
                menuImage = SpecialMenu[menuNum-11].GetComponent<Image>().sprite;
            }  
        }
        else if(menuNum >= 16 && menuNum <= 19) // 스페셜 메뉴 16도로시~19친구
        {
            if (VisitorNote.instance.GetFirstMeetID() == 0 && VisitorNote.instance.GetFriendEventID() == 0)//다시보기가 아닐 때
            {
                size = new Vector2(SpecialMenu[menuNum-12].GetComponent<RectTransform>().rect.width, SpecialMenu[menuNum-12].GetComponent<RectTransform>().rect.height); //사이즈 조정
                menuImage = SpecialMenu[menuNum-12].GetComponent<Image>().sprite;
            }
        }
        else if(menuNum == 22 || menuNum == 23) // 스페셜 메뉴 히로디노
        {
            if (VisitorNote.instance.GetFirstMeetID() == 0 && VisitorNote.instance.GetFriendEventID() == 0)//다시보기가 아닐 때
            {
                size = new Vector2(SpecialMenu[menuNum-10].GetComponent<RectTransform>().rect.width, SpecialMenu[menuNum-10].GetComponent<RectTransform>().rect.height); //사이즈 조정
                menuImage = SpecialMenu[menuNum-10].GetComponent<Image>().sprite;
            }
        }
        else if(menuNum == 24 || menuNum == 25) // 스페셜 메뉴 24닥터펭 25롤렝드
        {
            if (VisitorNote.instance.GetFirstMeetID() == 0 && VisitorNote.instance.GetFriendEventID() == 0)//다시보기가 아닐 때
            {
                size = new Vector2(SpecialMenu[menuNum-14].GetComponent<RectTransform>().rect.width, SpecialMenu[menuNum-14].GetComponent<RectTransform>().rect.height); //사이즈 조정
                menuImage = SpecialMenu[menuNum-14].GetComponent<Image>().sprite;
            }
        }

        switch(menuNum)
        {      
            case 11://여기서부터는 친밀도 이벤트의 메뉴들, 11은 도리 스페셜 메뉴
                Popup.instance.SetPopupMenu(SpecialMenu[0], "도리", 110f);
                break;
            case 12:
                Popup.instance.SetPopupMenu(SpecialMenu[1], "붕붕이", 50f);
                break;
            case 13:
                Popup.instance.SetPopupMenu(SpecialMenu[2], "빵빵이", 80f);
                break;
            case 14:
                Popup.instance.SetPopupMenu(SpecialMenu[3], "개나리", 90f);
                break;
            case 16:
                Popup.instance.SetPopupMenu(SpecialMenu[4], "도로시", 70f);
                break;
            case 17:
                Popup.instance.SetPopupMenu(SpecialMenu[5], "루루", 90f);
                break;
            case 18:
                Popup.instance.SetPopupMenu(SpecialMenu[6], "샌디", 70f);
                break;
            case 19:
                Popup.instance.SetPopupMenu(SpecialMenu[7], "친구", 80f);
                break;
            case 21: //무명이
            case 31:
                if(menuNum == 31)//디저트 이미지 교체
                {
                    SpecialMenu[8].GetComponent<Image>().sprite = NamelessDessert.sprite;
                }
                if (VisitorNote.instance.GetFirstMeetID() == 0 && VisitorNote.instance.GetFriendEventID() == 0)//다시보기가 아닐 때
                {
                    size = new Vector2(SpecialMenu[8].GetComponent<RectTransform>().rect.width, SpecialMenu[8].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    menuImage = SpecialMenu[8].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[8], SystemManager.instance.GetNameForNameless() , 80f);
                break;
            case 22://히로디노는 똑같은 메뉴
                Popup.instance.SetPopupMenu(SpecialMenu[9], "히로와 디노", 50f);
                break;
            case 24:
                Popup.instance.SetPopupMenu(SpecialMenu[10], "닥터 펭(을)", 50f);
                break;
            case 25:
                Popup.instance.SetPopupMenu(SpecialMenu[11], "롤렝드", 90f);
                break;
        }

        TableMenu[sNum].GetComponent<RectTransform>().sizeDelta = size;
        TableMenu[sNum].GetComponent<Image>().sprite = menuImage;
        //Debug.Log("함수 SetTableMenu");
    }

    public void SetFriendEventMenu(int cNum)//스페셜 메뉴 준비
    {
        seatNum = CharacterManager.instance.GetCharacterSeatNum(cNum);//해당 캐릭터 자리 넘버 대입    
        SetMenuPosition();
        CorrectMenuReaction(seatNum); //하트리액션 
        if(cNum != 13)//디노일때는 생략
        {
            SetReputation(reputation += 5);
        }
        TableMenu[seatNum].SetActive(true);
        Reaction[seatNum].SetActive(true);
        if(cNum == 11 && Dialogue.instance.GetSpecialMenuState() == 2)
        {
            SetTableMenu(31, seatNum); //스페셜 메뉴 이미지 설정
        }
        else
        {
            SetTableMenu(cNum + 10, seatNum); //스페셜 메뉴 이미지 설정
        }
        menuFadeIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
    }

    public void SoldierEvent_ServeToPrincess(int sNum)//찰스2 이벤트, 도로시 자리를 받아와서 도로시에게 서빙
    {
        seatNum = sNum;
        SetMenuPosition();
        TableMenu[seatNum].SetActive(true);
        SetTableMenu(5, seatNum);//메뉴는 딸기스무디
        menuFadeIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
    }

    public void CorrectMenuReaction(int n) //힌트와 맞는 메뉴일 경우 리액션
    {
        Reaction[n].GetComponent<Image>().sprite = HeartReaction[n].GetComponent<Image>().sprite;
    }

    public void WrongMenuReaction(int n) //힌트와 맞지 않는 메뉴일 경우 리액션
    {
       Reaction[n].GetComponent<Image>().sprite = SmileReaction[n].GetComponent<Image>().sprite;
    }

    
    bool isMenuFadeIn = false;//페이드인 중인지 판별
    bool isMenuFadeOut = false;
    public void MenuFadeIn() //메뉴 페이드인
    {
        if(isMenuFadeIn)
        {
            Invoke(nameof(MenuFadeIn), 0.3f);
        }
        else
        {
            StartCoroutine(MenuFadeToFullAlpha()); //페이드인 시작
        }
    }

    public IEnumerator MenuFadeToFullAlpha() // 알파값 0에서 1로 전환, 메뉴 페이드인
    {
        int n = menuFadeIn.Peek();
        isMenuFadeIn = true;
        TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, 0);
        while (TableMenu[n].GetComponent<Image>().color.a < 1.0f)
        {
            TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, TableMenu[n].GetComponent<Image>().color.a + (Time.deltaTime / 0.8f));
            yield return null;
        }
        isMenuFadeIn = false;
        menuFadeIn.Dequeue();
        
        if(canStartEvent)
        {
            if((CharacterManager.instance.GetCurrentEventState() == 9 || CharacterManager.instance.GetCurrentEventState() == 10) && Dialogue.instance.GetSpecialMenuState() == 0)//친구,찰스1 친밀도 이벤트 처음일 때
            {
                SystemManager.instance.BeginDialogue(CharacterManager.instance.GetCurrentEventState());//시나리오 시작
                canStartEvent = false;
            }
            if(CharacterManager.instance.GetCurrentEventState() == 12)//무명이1 친밀도 이벤트의 경우
            {
                SystemManager.instance.BeginDialogue(11);
                canStartEvent = false;
            }
            if (CharacterManager.instance.GetCurrentEventState() == 14 && isHeroServed && isDinoServed && Dialogue.instance.GetSpecialMenuState() == 0)//히로디노 친밀도 이벤트의 경우
            {
                SystemManager.instance.BeginDialogue(12);
                canStartEvent = false;
            }
            if(CharacterManager.instance.GetCurrentEventState() == 16 && Dialogue.instance.GetSpecialMenuState() == 0)//롤렝드 친밀도 이벤트의 경우
            {
                SystemManager.instance.BeginDialogue(14);
                CharacterManager.instance.SetCharacter(17);
                canStartEvent = false;
            }
        }
    }

    public void MenuFadeOut(int sNum = -1, bool doItNow = true) //메뉴 페이드아웃
    {
        if(sNum == -1)
            menuFadeOut.Enqueue(seatNum);
        else
            menuFadeOut.Enqueue(sNum);

        if(doItNow)
        {
            if(!isMenuFadeOut)
            {
                StartCoroutine(MenuFadeToZero());           
            }
        }
    }

    public IEnumerator MenuFadeToZero()  // 알파값 1에서 0으로 전환, 메뉴 페이드아웃
    {
        isMenuFadeOut = true;
        int n = menuFadeOut.Peek();

        TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, 1);
        while (TableMenu[n].GetComponent<Image>().color.a > 0.0f)
        {
            TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, TableMenu[n].GetComponent<Image>().color.a - (Time.deltaTime / 1.0f));
            yield return null;
        }
        isMenuFadeOut = false;
        menuFadeOut.Dequeue();
        //Debug.Log("메뉴 페이드아웃" + n);

        TableMenu[n].SetActive(false);

        string charName = CharacterManager.instance.GetSittingCharacter(n).name;
        int cNum = TouchableObject.instance.GetNumber(charName.IndexOf("_"), charName);

        if((CharacterManager.instance.GetCurrentEventState() != 9 && CharacterManager.instance.GetCurrentEventState() != 11 && CharacterManager.instance.GetCurrentEventState() != 13 && CharacterManager.instance.GetCurrentEventState() != 14 && CharacterManager.instance.GetCurrentEventState() != 16)
            || (cNum != 9 && CharacterManager.instance.GetCurrentEventState() == 9)
            || (cNum != 6 && cNum != 10 && CharacterManager.instance.GetCurrentEventState() == 11)
            || (cNum != 12 && cNum != 13 && CharacterManager.instance.GetCurrentEventState() == 14)
            || (cNum != 15 && CharacterManager.instance.GetCurrentEventState() == 16))
        {//특정 캐릭터 친밀도 이벤트가 아닐 때, 혹은 특정 캐릭터 이벤트여도 해당 이벤트 캐릭터가 아닐 때
            //Debug.Log("페이드아웃될 자리 " + n);
            CharacterManager.instance.FadeOut(cNum); //작은 캐릭터 페이드아웃   
        }
        else if(CharacterManager.instance.GetCurrentEventState() == 14 && cNum == 12 && Dialogue.instance.GetSpecialMenuState() == 2)
        {//히로디노 이벤트 중에 페이드아웃하는 캐릭터가 히로이고 디노 페이드아웃 전일 때
            CharacterManager.instance.FadeOut(cNum); //작은 캐릭터 페이드아웃
        }

        if (menuFadeOut.Count != 0)
        {
            StartCoroutine(MenuFadeToZero());           
        }
    }

    public void ReactionFadeIn(int sNum = -1, float time = 0f) //리액션 페이드인
    {
        if(sNum == -1)
            reactionFadeIn.Enqueue(seatNum);
        else
            reactionFadeIn.Enqueue(sNum);

        if (isReactionFadeIn && time == 0f)
        {
            time = 1.5f;
            StartCoroutine(RFadeToFullAlpha(time)); //페이드인 시작
        }
        else
        {
            StartCoroutine(RFadeToFullAlpha(time)); //페이드인 시작
        }
        Invoke(nameof(ReactionFadeOut), 2f); //2초 후 페이드아웃
    }

    bool isReactionFadeIn = false;
    IEnumerator RFadeToFullAlpha(float time = 0f) // 알파값 0에서 1로 전환, 리액션 페이드인
    {
        isReactionFadeIn = true;

        if(time != 0f)
            yield return new WaitForSeconds(time);

        int num = reactionFadeIn.Peek();       

        Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, 0);
        while (Reaction[num].GetComponent<Image>().color.a < 1.0f)
        {
            Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, Reaction[num].GetComponent<Image>().color.a + (Time.deltaTime / 0.8f));
            yield return null;
        }
        reactionFadeIn.Dequeue();
        isReactionFadeIn = false;

        reactionFadeOut.Enqueue(num);//리액션 페이드아웃 큐에 정보 추가
        if (reactionFadeIn.Count != 0) //페이드인 할 게 남았다면
        {
            RFadeToFullAlpha();
        }
    }

    bool isReactionFadeOut = false;
    public void ReactionFadeOut() //리액션 페이드아웃
    {
        if (isReactionFadeOut)
        {
            Invoke(nameof(ReactionFadeOut), 0.5f);
        }
        else
        {
            StartCoroutine(RFadeToZero());
            //Debug.Log("함수 ReactionFadeOut");          
        }
    }

    public IEnumerator RFadeToZero()  // 알파값 1에서 0으로 전환, 리액션 페이드아웃
    {
        isReactionFadeOut = true;//페이드아웃 중
        int num = reactionFadeOut.Peek();       

        Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, 1);
        while (Reaction[num].GetComponent<Image>().color.a > 0.0f)
        {
            Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, Reaction[num].GetComponent<Image>().color.a - (Time.deltaTime / 1.0f));
            yield return null;
        }
        isReactionFadeOut = false;
        reactionFadeOut.Dequeue();//페이드아웃 완료했으니 큐에서 정보 삭제
        Reaction[num].SetActive(false);

        if(CharacterManager.instance.GetSittingCharacter(num) != null)//현재 앉아있는 캐릭터가 null이 아닐 때
        {
            string charName = CharacterManager.instance.GetSittingCharacter(num).name;
            if ((charName == "small_12Hero" || charName == "small_13Dinosour") && Dialogue.instance.GetSpecialMenuState() == 0)
            {//히로디노이고, 이벤트가 아닐 때
                if (charName == "small_12Hero")
                {
                    isHeroServed = true;
                }
                if (charName == "small_13Dinosour")
                {
                    isDinoServed = true;
                }

                if (isHeroServed && isDinoServed)//둘 다 서빙완료했으면
                {
                    isDinoServed = false;
                    isHeroServed = false;
                    
                    if(charName.Contains("Hero")) // 서빙된 캐릭터가 히로면
                    {
                        MenuFadeOut(num+1); // 디노 먼저 메뉴 페이드아웃
                    }
                    else if(charName.Contains("Dino"))
                    {
                        MenuFadeOut(num-1); // 히로 먼저

                    }
                    MenuFadeOut(num, false); // 앞의 캐릭터 메뉴 페이드아웃 끝나고 실행
                }
            }
            else if (Dialogue.instance.GetCharacterDC(10) == 3 && (charName == "small_6Princess" || charName == "small_10Soldier"))
            {
                if (charName == "small_6Princess")
                {
                    isPrincessServed = true;
                }
                if (charName == "small_10Soldier")
                {
                    isSoldierServed = true;
                }
                if (isPrincessServed && isSoldierServed)//둘 다 서빙완료했으면
                {
                    isPrincessServed = false;
                    isSoldierServed = false;
                    if(charName.Contains("Princess")) // 서빙된 캐릭터가 도로시면
                    {
                        MenuFadeOut(num+1); // 찰스 먼저 메뉴 페이드아웃
                    }
                    else if(charName.Contains("Soldier"))
                    {
                        MenuFadeOut(num-1); // 도로시 먼저
                    }
                    MenuFadeOut(num, false);
                }
            }
            else //혼자 온 캐릭터
            {
                MenuFadeOut(num);
            }
        }
        else
        {
            Debug.Log("앉아있는 캐릭터 Null! 자리는" + num);
        }
    }

    public bool IsMenuOpen(int menuNum)
    {
        return UnlockedMenuItems.Count >= menuNum? true : false; 
    }

    public void UnlockMenuItems() // 잠금된 메뉴 해제
    {
        int lastMenu = PlayerPrefs.GetInt("UnlockedMenuItems"); // 개수이므로 다음 메뉴의 인덱스가 됨
        if(lastMenu == 0) lastMenu = 3;

        int requiredStar = (lastMenu-2) * 5;

        //만약 스타가 부족하면 열리지 않음
        if(Star.instance.GetCurrentStarNum() >= requiredStar)
        {
            MenuObject[lastMenu].SetActive(true);
            Star.instance.UseStar(requiredStar);
            UnlockedMenuItems.Add(MenuObject[lastMenu]); //UnlockedMenuItems리스트에 메뉴4 추가

            LockedMenuItems[lastMenu - 3].SetActive(false); //실루엣 이미지 없애고
            if(UnlockedMenuItems.Count != 8)
                LockedMenuButtons[lastMenu - 3].gameObject.SetActive(true);//다음 메뉴 해제 버튼 활성화

            SaveUnlockedMenuItemInfo();
        }

        if(UnlockedMenuItems.Count == 8) //마지막 메뉴 잠금 해제 시
            SystemManager.instance.CheckEndingCondition();
    }

    public void SaveUnlockedMenuItemInfo()
    {
        try
        {
            PlayerPrefs.SetInt("UnlockedMenuItems", UnlockedMenuItems.Count); //현재 오픈된 메뉴 크기 저장, 3은 기본 메뉴 3개 오픈된 것
            PlayerPrefs.Save(); //세이브
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveUnlockedMenuItemInfo Failed (" + e.Message + ")");
        }
    }

    public void LoadUnlockedMenuItemInfo() 
    {
        try
        {
            if (PlayerPrefs.HasKey("UnlockedMenuItems"))
            {
                int n = PlayerPrefs.GetInt("UnlockedMenuItems");
                if(n != 3)
                {
                    for(int i = 4; i <= n; i++)
                    {
                        LoadMenu(i);
                    }
                }
            }        
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadDataInfo Failed (" + e.Message + ")");
        }
    }

    void LoadMenu(int menuNum)//오픈 메뉴 리스트 크기에 따라 메뉴 로드
    {
        MenuObject[menuNum-1].SetActive(true);
        LockedMenuItems[menuNum-4].SetActive(false); //실루엣 이미지 없애고
        UnlockedMenuItems.Add(MenuObject[menuNum-1]); //UnlockedMenuItems리스트에 메뉴 추가
        
        if(menuNum != 8)
        {
            LockedMenuButtons[menuNum - 4].gameObject.SetActive(true);//다음 메뉴 해제 버튼 활성화
        }
    }
}
