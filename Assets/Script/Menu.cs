using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour 
{
    public static Menu instance;

    [SerializeField] Animator menuButtonAnimator;
    [SerializeField] Animator menuBoardAnimator;
    [SerializeField] Animator NoHPMessegeAnimator;

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

    [SerializeField] GameObject NoHPMessege; //게임 중간이탈, 종료 시 비활성화됨

    [SerializeField] Image NamelessDessert;//무명이 디저트 2번째 이미지
    #endregion

    Vector3[] MenuPosition = new Vector3[6]; //메뉴 위치 배열
    Vector3[] ReactionPosition = new Vector3[6]; //리액션 위치 배열

    List<GameObject> UnlockedMenuItems = new List<GameObject>(); // 현재 잠금 해제된 메뉴 리스트

    bool menu4Open = false; //true면 메뉴가 해제된 것
    bool menu5Open = false;
    bool menu6Open = false;
    bool menu7Open = false;
    bool menu8Open = false;

    int reputation = 0; //평판 

    int seatNum = 0; //자리 넘버

    public Queue<int> reactionFadeIn = new Queue<int>();//리액션 페이드인 시 사용
    Queue<int> reactionFadeOut = new Queue<int>();//리액션 페이드아웃 시 사용
    Queue<int> menuFadeIn = new Queue<int>();//메뉴 페이드인 시 사용
    public Queue<int> menuFadeOut = new Queue<int>();//메뉴 페이드아웃 시 사용

    public bool dinoOk = false; //디노 히로가 모두 서빙을 받았을 시 페이드아웃 가능
    public bool heroOk = false;
    public bool pOk = false; //도로시 서빙, 찰스랑 같이 다닐 때부터 사용
    public bool sOk = false; //찰스 서빙

    Queue<int> together1 = new Queue<int>();//히로디노 자리 정보 저장
    Queue<int> together2 = new Queue<int>();//도로시찰스 자리 정보 저장

    int characterNum;

    bool canStartEvent = false; //메뉴 서빙 후 친밀도 이벤트가 나오는 캐릭터의 경우, 이벤트 캐릭터가 서빙을 받으면 이벤트 시작

    public int tmpNum = 0;//메뉴 서빙 후 이벤트 나오는 캐릭터들 메뉴 위치 임시로 저장

    public Queue<int> seatInfo = new Queue<int>(); //자리 정보 큐, 작은 캐릭터 페이드아웃 시 사용

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

    public void SetSeatNum(int value)
    {
        seatNum = value;
    }

    public void SetCharacterNum(int value)
    {
        characterNum = value;
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
            MenuHint.instance.CantClickMHB();//다른 메뉴힌트버블 터치 불가
            SystemManager.instance.SetUIOpen(true);
            SEManager.instance.PlayUIClickSound(); //효과음
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
        MenuHint.instance.CanClickMHB();
    }

    public void TouchMenuHint() //캐릭터의 메뉴 힌트 말풍선 눌렀을 때
    {
        if (!SystemManager.instance.IsUIOpen())
        {
            MenuHint.instance.CantClickMHB();//다른 메뉴힌트버블 터치 불가
            SystemManager.instance.SetUIOpen(true);
            SEManager.instance.PlayUIClickSound2(); //효과음
            menuButtonAnimator.SetTrigger("MenuButtonOut"); //메뉴 버튼 위로 올라가고
            menuBoardAnimator.SetTrigger("MenuBoardUp"); //메뉴판 올라옴

            if (SystemManager.instance.GetMainCount() == 2) //서빙 튜토리얼일 경우
            {
                // 레이어 순서 원래대로 변경
                SmallFade.instance.GetSmallCharacter(1).transform.parent.GetComponent<Canvas>().sortingOrder = 2;
                MenuHint.instance.GetHintBubble(1).transform.parent.GetComponent<Canvas>().sortingOrder = 4;

                boardCloseButton.GetComponent<Button>().interactable = false; //닫기 버튼 불가
                UI_Assistant1.instance.panel6.SetActive(false); //패널 없애고
                UI_Assistant1.instance.OpenDialogue2(); //다음 대사 나타남
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
        if(NoHPMessege.activeSelf)
            NoHPMessege.SetActive(false);

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

    private void NoHPMessegeFadeOut() //체력 없음 메세지 페이드아웃
    {
        NoHPMessegeAnimator.SetTrigger("NoHPFadeOut");
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
            NoHPMessegeAnimator.SetTrigger("NoHPFadeIn");
            Invoke(nameof(NoHPMessegeFadeOut), 1f); //1초 뒤 메세지 페이드아웃
            Invoke(nameof(CanTouchMenu), 1.5f);           
        }
        else //체력이 1이상이면
        {
            CantTouchMenu();
            SystemManager.instance.SetUIOpen(false);
            menuBoardAnimator.SetTrigger("MenuBoardDown"); //메뉴판 아래로 내려가고
            menuButtonAnimator.SetTrigger("MenuButtonIn"); //메뉴버튼 위에서 내려옴
            MenuHint.instance.CanClickMHB();//다른 메뉴힌트버블 터치 가능

            HPManager.instance.UseHP(); //체력 소모

            if((characterNum == 9 && CharacterAppear.instance.eventOn == 9) || (characterNum == 10 && CharacterAppear.instance.eventOn == 10) || (characterNum == 15 && CharacterAppear.instance.eventOn == 16))
            {// 친구,찰스1,롤렝드 친밀도 이벤트일 경우 클릭된 캐릭터와 이벤트 캐릭터가 동일하면
                canStartEvent = true;
                SystemManager.instance.CantTouchUI();
                MenuHint.instance.CantClickMHB();//뒤에 메뉴판이 떠있는 채로 이벤트 시작하는 걸 방지하기 위함
                if (MenuHint.instance.RightMenu[seatNum] == menuNum) //캐릭터가 원하는 메뉴가 n이면, 원하는 메뉴와 플레이어가 고른 메뉴가 일치
                {
                    SEManager.instance.PlayUIClickSound3();
                    VisitorNote.instance.GuessMenuRight(characterNum, menuNum);
                    if (CharacterAppear.instance.eventOn == 10)//찰스의 경우 평판 증가
                    {
                        reputation += 3;
                        reputationText.text = string.Format("{0}", reputation); //평판 표시
                        VisitorNote.instance.IncreaseFrinedshipGauge(characterNum); //서빙받은 캐릭터의 친밀도 증가
                    }
                }
                else //n이 아니면
                {
                    SEManager.instance.PlayUIClickSound2();
                    if (CharacterAppear.instance.eventOn == 10)
                    {
                        reputation++;
                        reputationText.text = string.Format("{0}", reputation); //평판 표시
                        VisitorNote.instance.IncreaseFrinedshipGauge(characterNum); //서빙받은 캐릭터의 친밀도 증가
                    }
                }
                TableMenu[seatNum].SetActive(true);
                SetMenuPosition();//테이블에 올려질 메뉴 위치 설정               
                MenuHint.instance.HintFadeOut(seatNum); //메뉴힌트 페이드아웃
                SetTableMenu(menuNum, seatNum); //테이블에 올려질 메뉴는 n
                menuFadeIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
                tmpNum = seatNum;//대화 중 메뉴 페이드아웃 큐에 자리 정보 추가하기 위함
                MenuFadeIn();                
            }
            else
            {
                if (MenuHint.instance.RightMenu[seatNum] == menuNum) //캐릭터가 원하는 메뉴가 n이면, 원하는 메뉴와 플레이어가 고른 메뉴가 일치
                {
                    SEManager.instance.PlayUIClickSound3();
                    if(CharacterAppear.instance.eventOn != 14)//히로디노 친밀도 이벤트가 아니면
                    {
                        CorrectMenuReaction(seatNum); //자리에 따라 맞는 메뉴 리액션 이미지 가져오기
                        reputation += 3; //원하는 메뉴 서빙 시 평판 3 증가
                    }                  
                    VisitorNote.instance.GuessMenuRight(characterNum, menuNum);
                }
                else //n이 아니면
                {
                    SEManager.instance.PlayUIClickSound2();
                    if (CharacterAppear.instance.eventOn != 14)
                    {
                        WrongMenuReaction(seatNum); //자리에 따라 틀린 메뉴 리액션 이미지 가져오기
                        reputation++; //원하는 메뉴가 아닌 다른 메뉴 서빙 시 평판 1 증가
                    }                  
                }
                TableMenu[seatNum].SetActive(true);
                Reaction[seatNum].SetActive(true);
                SetMenuPosition();//테이블에 올려질 메뉴 위치 설정

                MenuHint.instance.HintFadeOut(seatNum); //메뉴힌트 페이드아웃
                SetTableMenu(menuNum, seatNum); //테이블에 올려질 메뉴는 n                
                menuFadeIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
                MenuFadeIn();
                if (SystemManager.instance.GetMainCount() == 2) //서빙 튜토리얼일 경우
                {
                    reactionFadeIn.Enqueue(seatNum); //리액션 페이드인 큐에 자리 정보 추가
                    UI_Assistant1.instance.Invoke("OpenDialogue2",0.5f);
                    SystemManager.instance.SetCanTouch(true,1.5f);
                    VisitorNote.instance.IncreaseFrinedshipGauge(characterNum);
                }
                else
                {
                    if(characterNum == 11 && CharacterAppear.instance.eventOn == 12)//무명이1 이벤트이고 무명이 서빙을 했을 때
                    {
                        SystemManager.instance.CantTouchUI();
                        MenuHint.instance.CantClickMHB();//뒤에 메뉴판이 떠있는 채로 이벤트 시작하는 걸 방지하기 위함
                        reactionFadeIn.Enqueue(seatNum); //리액션 페이드인 큐에 자리 정보 추가
                        canStartEvent = true;
                    }
                    else if((characterNum == 12 || characterNum == 13) && CharacterAppear.instance.eventOn == 14)//히로디노 이벤트인데 캐릭터가 히로나 디노일 때
                    {
                        if(characterNum == 12)
                        {
                            tmpNum = seatNum;//대화 중 메뉴 페이드아웃 큐에 자리 정보 추가하기 위함
                        }                    
                    }
                    else // 그 외의 경우
                    {                      
                        reactionFadeIn.Enqueue(seatNum); //리액션 페이드인 큐에 자리 정보 추가
                        //Debug.Log("리액션 페이드인큐에 들어간 자리 012345 => " + seatNum);
                        if (CharacterAppear.instance.eventOn != 5)
                        {
                            Invoke("ReactionFadeIn", 1f);
                        }     
                    }

                    if (characterNum == 12 || characterNum == 13)//디노나 히로 서빙이면
                    {
                        if (characterNum == 12 && CharacterAppear.instance.eventOn == 14)//이벤트일 때만 히로디노OK 먼저 하기
                        {
                            heroOk = true;
                            SmallFade.instance.CanClickCharacter(13);//디노 클릭 가능                           
                        }
                        else if (characterNum == 13 && CharacterAppear.instance.eventOn == 14)
                        {
                            dinoOk = true;
                        }

                        if (CharacterAppear.instance.eventOn != 14)//이벤트 아닐 때만
                        {
                            together1.Enqueue(seatNum);//히노디노큐에 자리 저장
                        }

                        if (CharacterAppear.instance.eventOn != 14)//히로디노 친밀도 이벤트가 아니면 친밀도 증가
                        {
                            VisitorNote.instance.IncreaseFrinedshipGauge(characterNum);
                        }
                        else if (CharacterAppear.instance.eventOn == 14 && dinoOk && heroOk)// 둘 다 서빙완료했을 때, 친밀도 이벤트면 UI클릭 금지
                        {
                            MenuHint.instance.CantClickMHB();//뒤에 메뉴판이 떠있는 채로 이벤트 시작하는 걸 방지하기 위함
                            canStartEvent = true;
                            SystemManager.instance.CantTouchUI();
                        }
                    }
                    else if(Dialogue.instance.CharacterDC[10] == 3 && (characterNum == 6 || characterNum == 10))//찰스2이벤트가 끝난 후 찰스나 도로시일 경우
                    {
                        together2.Enqueue(seatNum);//찰스도로시큐에 자리 저장
                    }
                    else//디노히로, 찰스도로시가 아니면
                    {
                        VisitorNote.instance.IncreaseFrinedshipGauge(characterNum); //서빙받은 캐릭터의 친밀도 증가
                    }
                }
            }          
        }
    }

    public void SetMenuPosition() //테이블 메뉴가 나타날 자리 설정
    {
        TableMenu[seatNum].transform.position = MenuPosition[seatNum];
        Reaction[seatNum].transform.position = ReactionPosition[seatNum];
        //Debug.Log("함수 SetMenuPosition");
    }

    public void SetTableMenu(int menuNum, int num) //테이블에 올려질 메뉴 이미지 설정
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
            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
            {
                size = new Vector2(SpecialMenu[menuNum-1].GetComponent<RectTransform>().rect.width, SpecialMenu[menuNum-1].GetComponent<RectTransform>().rect.height); //사이즈 조정
                menuImage = SpecialMenu[menuNum-1].GetComponent<Image>().sprite;
            }  
        }
        else if(menuNum >= 16 && menuNum <= 19) // 스페셜 메뉴 16도로시~19친구
        {
            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
            {
                size = new Vector2(SpecialMenu[menuNum-2].GetComponent<RectTransform>().rect.width, SpecialMenu[menuNum-2].GetComponent<RectTransform>().rect.height); //사이즈 조정
                menuImage = SpecialMenu[menuNum-2].GetComponent<Image>().sprite;
            }
        }
        else if(menuNum == 22 || menuNum == 23) // 스페셜 메뉴 히로디노
        {
            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
            {
                size = new Vector2(SpecialMenu[menuNum-10].GetComponent<RectTransform>().rect.width, SpecialMenu[menuNum-10].GetComponent<RectTransform>().rect.height); //사이즈 조정
                menuImage = SpecialMenu[menuNum-10].GetComponent<Image>().sprite;
            }
        }
        else if(menuNum == 24 || menuNum == 25) // 스페셜 메뉴 24닥터펭 25롤렝드
        {
            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
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
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
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

        TableMenu[num].GetComponent<RectTransform>().sizeDelta = size;
        TableMenu[num].GetComponent<Image>().sprite = menuImage;
        //Debug.Log("함수 SetTableMenu");
    }

    public void FEventMenu(int n)//스페셜 메뉴 준비
    {
        seatNum = SmallFade.instance.CharacterSeat[n - 1];//해당 캐릭터 자리 넘버 대입    
        SetMenuPosition();
        CorrectMenuReaction(seatNum); //하트리액션 
        if(n != 13)//디노일때는 생략
        {
            reputation += 5;
        }
        TableMenu[seatNum].SetActive(true);
        Reaction[seatNum].SetActive(true);
        if(n == 11 && UI_Assistant1.instance.getMenu == 2)
        {
            SetTableMenu(31, seatNum); //스페셜 메뉴 이미지 설정
        }
        else
        {
            SetTableMenu(n + 10, seatNum); //스페셜 메뉴 이미지 설정
        }

        if(n != 12 && n != 13)
        {
            reactionFadeIn.Enqueue(seatNum); //리액션 페이드인 큐에 자리 정보 추가
        }
        if(n == 11 && CharacterAppear.instance.eventOn == 13 && UI_Assistant1.instance.getMenu == 1)//무명이2 이벤트
        {
            tmpNum = seatNum;
        }
        menuFadeIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
    }

    public void SoldierEvent(int s)//찰스2 이벤트, 도로시에게 서빙
    {
        seatNum = s;
        SetMenuPosition();
        TableMenu[seatNum].SetActive(true);
        SetTableMenu(5, seatNum);//메뉴는 딸기스무디
        menuFadeIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
        menuFadeOut.Enqueue(seatNum);//미리 추가
    }

    public void CorrectMenuReaction(int n) //힌트와 맞는 메뉴일 경우 리액션
    {
        Reaction[n].GetComponent<Image>().sprite = HeartReaction[n].GetComponent<Image>().sprite;
    }

    public void WrongMenuReaction(int n) //힌트와 맞지 않는 메뉴일 경우 리액션
    {
       Reaction[n].GetComponent<Image>().sprite = SmileReaction[n].GetComponent<Image>().sprite;
    }

    int rFade = 0;//리액션 페이드인에 쓰이는 정수 0이면 페이드인 중이 아님, 1이면 페이드인 중인 상태
    public void ReactionFadeIn() //리액션 페이드인
    {
        if(rFade == 1)//리액션 페이드인 진행 중일 시
        {
            Invoke("ReactionFadeIn", 0.5f); //0.5초 뒤 이 함수 재실행
        }
        else
        {
            StartCoroutine(RFadeToFullAlpha()); //페이드인 시작
            Invoke("ReactionFadeOut", 2f); //2초 후 페이드아웃
            reputationText.text = string.Format("{0}", reputation); //평판 표시
            //Debug.Log("함수 ReactionFadeIn");
        }
    }

    int rFade2 = 0;//리액션 페이드아웃에 쓰이는 정수 
    public void ReactionFadeOut() //리액션 페이드아웃
    {
        if (rFade2 == 1)
        {
            Invoke("ReactionFadeOut", 0.5f);
        }
        else
        {
            StartCoroutine(RFadeToZero());
            //Debug.Log("함수 ReactionFadeOut");          
        }
        
    }

    public IEnumerator RFadeToFullAlpha() // 알파값 0에서 1로 전환, 리액션 페이드인
    {
        rFade = 1;//리액션 페이드인 중
        int num = reactionFadeIn.Peek();       
        Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, 0);
        while (Reaction[num].GetComponent<Image>().color.a < 1.0f)
        {
            Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, Reaction[num].GetComponent<Image>().color.a + (Time.deltaTime / 0.8f));
            yield return null;
        }
        rFade = 0;//페이드인하는 반복문이 끝남=페이드인이 끝남
        reactionFadeIn.Dequeue();
        reactionFadeOut.Enqueue(num);//리액션 페이드아웃 큐에 정보 추가
        if (reactionFadeIn.Count != 0 && !IsInvoking("ReactionFadeIn"))//리액션 페이드인큐가 아직 있고 페이드인함수가 반복대기중이 아니면
        {
            ReactionFadeIn();//리액션 페이드인 실ㄴ
        }
    }

    public IEnumerator RFadeToZero()  // 알파값 1에서 0으로 전환, 리액션 페이드아웃
    {
        rFade2 = 1;//페이드아웃 중
        int num = reactionFadeOut.Peek();       
        Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, 1);
        while (Reaction[num].GetComponent<Image>().color.a > 0.0f)
        {
            Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, Reaction[num].GetComponent<Image>().color.a - (Time.deltaTime / 1.0f));
            yield return null;
        }
        rFade2 = 0;
        reactionFadeOut.Dequeue();//페이드아웃 완료했으니 큐에서 정보 삭제
        Reaction[num].SetActive(false);
        if(SmallFade.instance.SittingCharacter[num] != null)//현재 앉아있는 캐릭터가 null이 아닐 때
        {
            if (((SmallFade.instance.SittingCharacter[num].name == "sHero" || SmallFade.instance.SittingCharacter[num].name == "sDinosour") && UI_Assistant1.instance.getMenu == 0))
            {//히로디노이고, 이벤트가 아닐 때
                if(SmallFade.instance.SittingCharacter[num].name == "sHero")
                {
                    heroOk = true;
                }
                if(SmallFade.instance.SittingCharacter[num].name == "sDinosour")
                {
                    dinoOk = true;
                }
                if (heroOk && dinoOk)//둘 다 서빙완료했으면
                {
                    dinoOk = false;
                    heroOk = false;
                    menuFadeOut.Enqueue(together1.Peek());    //메뉴 페이드아웃 큐에 데이터 추가 후           
                    MenuFadeOut();                        //메뉴 페이드아웃 실행
                    together1.Dequeue();
                    menuFadeOut.Enqueue(together1.Peek());
                    together1.Dequeue();
                }
            }
            else if (Dialogue.instance.CharacterDC[10] == 3 && (SmallFade.instance.SittingCharacter[num].name == "small_6Princess" || SmallFade.instance.SittingCharacter[num].name == "small_10Soldier"))
            {
                if (SmallFade.instance.SittingCharacter[num].name == "small_6Princess")
                {
                    pOk = true;
                }
                if (SmallFade.instance.SittingCharacter[num].name == "small_10Soldier")
                {
                    sOk = true;
                }
                if (pOk && sOk)//둘 다 서빙완료했으면
                {
                    pOk = false;
                    sOk = false;
                    menuFadeOut.Enqueue(together2.Peek());    //메뉴 페이드아웃 큐에 데이터 추가 후           
                    MenuFadeOut();                        //메뉴 페이드아웃 실행
                    together2.Dequeue();
                    menuFadeOut.Enqueue(together2.Peek());
                    together2.Dequeue();
                }
            }
            else //혼자 온 캐릭터
            {
                menuFadeOut.Enqueue(num);
                MenuFadeOut();
            }
        }
        else if(SmallFade.instance.SittingCharacter[num] == null)
        {
            Debug.Log("앉아있는 캐릭터 널임!! 자리는" + num);
        }
    }

    bool mFade = false;//리액션 페이드 정수와 역할 동일
    bool mFade2 = false;
    public void MenuFadeIn() //메뉴 페이드인
    {
        if(mFade)
        {
            Invoke("MenuFadeIn", 0.3f);
        }
        else
        {
            StartCoroutine(FadeToFullAlpha()); //페이드인 시작
        }
    }

    public void MenuFadeOut() //메뉴 페이드아웃
    {
        if(!mFade2)
        {
            StartCoroutine(FadeToZero());           
        }
    }

    public IEnumerator FadeToFullAlpha() // 알파값 0에서 1로 전환, 메뉴 페이드인
    {
        int n = menuFadeIn.Peek();
        mFade = true;
        TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, 0);
        while (TableMenu[n].GetComponent<Image>().color.a < 1.0f)
        {
            TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, TableMenu[n].GetComponent<Image>().color.a + (Time.deltaTime / 0.8f));
            yield return null;
        }
        mFade = false;
        menuFadeIn.Dequeue();

        if((CharacterAppear.instance.eventOn == 9 || CharacterAppear.instance.eventOn == 10)&& UI_Assistant1.instance.getMenu == 0 && canStartEvent)//친구,찰스1 친밀도 이벤트 처음일 때
        {
            SystemManager.instance.BeginDialogue(CharacterAppear.instance.eventOn);//시나리오 시작
            canStartEvent = false;
        }
        if(CharacterAppear.instance.eventOn == 12 && canStartEvent)//무명이1 친밀도 이벤트의 경우
        {
            SystemManager.instance.BeginDialogue(11);
            canStartEvent = false;
        }
        if (CharacterAppear.instance.eventOn == 14 && heroOk && dinoOk && UI_Assistant1.instance.getMenu == 0 && canStartEvent)//히로디노 친밀도 이벤트의 경우
        {
            SystemManager.instance.BeginDialogue(12);
            canStartEvent = false;
        }
        if(CharacterAppear.instance.eventOn == 16 && UI_Assistant1.instance.getMenu == 0 && canStartEvent)//롤렝드 친밀도 이벤트의 경우
        {
            SystemManager.instance.BeginDialogue(14);
            SmallFade.instance.smallFadeIn.Enqueue(17);
            SmallFade.instance.FadeIn();
            canStartEvent = false;
        }
    }

    public IEnumerator FadeToZero()  // 알파값 1에서 0으로 전환, 메뉴 페이드아웃
    {
        mFade2 = true;
        int n = menuFadeOut.Peek();

        TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, 1);
        while (TableMenu[n].GetComponent<Image>().color.a > 0.0f)
        {
            TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, TableMenu[n].GetComponent<Image>().color.a - (Time.deltaTime / 1.0f));
            yield return null;
        }
        mFade2 = false;
        menuFadeOut.Dequeue();
        TableMenu[n].SetActive(false);
        if((CharacterAppear.instance.eventOn != 9 && CharacterAppear.instance.eventOn != 11 && CharacterAppear.instance.eventOn != 13 && CharacterAppear.instance.eventOn != 14 && CharacterAppear.instance.eventOn != 16)
            || (SmallFade.instance.SittingCharacter[n].name != "small_9Dog" && CharacterAppear.instance.eventOn == 9)
            || (SmallFade.instance.SittingCharacter[n].name != "small_6Princess" && SmallFade.instance.SittingCharacter[n].name != "small_10Soldier" && CharacterAppear.instance.eventOn == 11)
            || (SmallFade.instance.SittingCharacter[n].name != "small_12Hero" && SmallFade.instance.SittingCharacter[n].name != "small_13Dinosour" && CharacterAppear.instance.eventOn == 14)
            || (SmallFade.instance.SittingCharacter[n].name != "small_15Grandfather" && CharacterAppear.instance.eventOn == 16))
        {//특정 캐릭터 친밀도 이벤트가 아닐 때, 혹은 특정 캐릭터 이벤트여도 해당 이벤트 캐릭터가 아닐 때
            seatInfo.Enqueue(n);
            //Debug.Log("페이드아웃될 자리 " + n);
            SmallFade.instance.FadeOut(); //작은 캐릭터 페이드아웃   
        }
        else if(CharacterAppear.instance.eventOn == 14 && SmallFade.instance.SittingCharacter[n].name == "sHero" && UI_Assistant1.instance.getMenu == 2)
        {//히로디노 이벤트 중에 페이드아웃하는 캐릭터가 히로이고 디노 페이드아웃 전일 때
            seatInfo.Enqueue(n);
            SmallFade.instance.FadeOut(); //작은 캐릭터 페이드아웃
        }
        if (menuFadeOut.Count != 0)
        {
            MenuFadeOut();
        }
    }

    public bool IsMenuOpen(int menuNum)
    {
        switch(menuNum)
        {
            case 4:
                return menu4Open;
            case 5:
                return menu5Open;
            case 6:
                return menu6Open;
            case 7:
                return menu7Open;
            case 8:
                return menu8Open; 
        }
        return false;
    }

    public void UnlockMenuItems4() // 메뉴4 해제
    {
        //만약 스타가 부족하면 열리지 않음
        if(Star.instance.GetCurrentStarNum() >= 5)
        {
            MenuObject[3].SetActive(true);
            Star.instance.UseStar(5);
            LockedMenuItems[0].SetActive(false); //실루엣 이미지 없애고
            UnlockedMenuItems.Add(MenuObject[3]); //UnlockedMenuItems리스트에 메뉴4 추가
            menu4Open = true; //1은 메뉴가 해제됐다는 의미
            LockedMenuButtons[0].gameObject.SetActive(true);//메뉴 5 해제 버튼 활성화
            SaveUnlockedMenuItemInfo();
        }
    }

    public void UnlockMenuItems5()
    {
        if (Star.instance.GetCurrentStarNum() >= 10)
        {
            MenuObject[4].SetActive(true);
            Star.instance.UseStar(10);
            LockedMenuItems[1].SetActive(false);
            UnlockedMenuItems.Add(MenuObject[4]);          
            menu5Open = true;
            LockedMenuButtons[1].gameObject.SetActive(true);
            SaveUnlockedMenuItemInfo();
        }
    }

    public void UnlockMenuItems6()
    {
        if (Star.instance.GetCurrentStarNum() >= 15)
        {
            MenuObject[5].SetActive(true);
            Star.instance.UseStar(15);
            LockedMenuItems[2].SetActive(false);
            UnlockedMenuItems.Add(MenuObject[5]);
            menu6Open = true;
            LockedMenuButtons[2].gameObject.SetActive(true);
            SystemManager.instance.ShowBangBubble();
            SaveUnlockedMenuItemInfo();
        }
    }

    public void UnlockMenuItems7()
    {
        if (Star.instance.GetCurrentStarNum() >= 20)
        {
            MenuObject[6].SetActive(true);
            Star.instance.UseStar(20);
            LockedMenuItems[3].SetActive(false);
            UnlockedMenuItems.Add(MenuObject[6]);
            menu7Open = true;
            LockedMenuButtons[3].gameObject.SetActive(true);
            SaveUnlockedMenuItemInfo();
        }
    }

    public void UnlockMenuItems8()
    {
        if (Star.instance.GetCurrentStarNum() >= 25)
        {
            MenuObject[7].SetActive(true);
            Star.instance.UseStar(25);
            LockedMenuItems[4].SetActive(false);
            UnlockedMenuItems.Add(MenuObject[7]);
            menu8Open = true;
            SystemManager.instance.CheckEndingCondition();
            SaveUnlockedMenuItemInfo();
        }
    }

    public bool SaveUnlockedMenuItemInfo()
    {
        bool result = false;
        try
        {
            PlayerPrefs.SetInt("UnlockedMenuItems", UnlockedMenuItems.Count); //현재 오픈된 메뉴 크기 저장, 3은 기본 메뉴 3개 오픈된 것
            PlayerPrefs.Save(); //세이브
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveUnlockedMenuItemInfo Failed (" + e.Message + ")");
        }
        return result;        
    }

    public bool LoadUnlockedMenuItemInfo() 
    {
        bool result = false;
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
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadDataInfo Failed (" + e.Message + ")");
        }
        return result;
    }

    void LoadMenu(int x)//오픈 메뉴 리스트 크기에 따라 메뉴 로드
    {
        MenuObject[x-1].SetActive(true);
        LockedMenuItems[x-4].SetActive(false); //실루엣 이미지 없애고
        UnlockedMenuItems.Add(MenuObject[x-1]); //UnlockedMenuItems리스트에 메뉴 추가
        switch(x)
        {
            case 4:
                menu4Open = true; //1은 메뉴가 해제됐다는 의미
                break;
            case 5:
                menu5Open = true;
                break;
            case 6:
                menu6Open = true;
                break;
            case 7:
                menu7Open = true;
                break;
            case 8:
                menu8Open = true;
                break;
        }      
        if(x != 8)
        {
            LockedMenuButtons[x - 4].gameObject.SetActive(true);//다음 메뉴 해제 버튼 활성화
        }
    }
}
