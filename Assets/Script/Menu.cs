using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour 
{
    public static Menu instance;

    public Animator menuButtonAnimator;
    public Animator menuBoardAnimator;
    public Animator NoHPMessegeAnimator;

    List<GameObject> UnlockedMenuItems = new List<GameObject>(); // 현재 잠금 해제된 메뉴 리스트
    public GameObject[] LockedMenuItems; // 잠금 상태인 메뉴 배열, 실루엣 이미지

    public GameObject[] MenuObject; //메뉴 배열
    Vector3[] MenuPosition = new Vector3[6]; //메뉴 위치 배열
    Vector3[] ReactionPosition = new Vector3[6]; //리액션 위치 배열

    public GameObject[] TableMenu; //테이블 위에 보여질 작은 메뉴 이미지
   
    public Button close;

    public GameObject[] SmileReaction; //미소 리액션 배열
    public GameObject[] HeartReaction; //하트 리액션 배열

    public GameObject[] Reaction; //캐릭터의 리액션 이미지

    public bool menu4Open = false; //true면 메뉴가 해제된 것
    public bool menu5Open = false;
    public bool menu6Open = false;
    public bool menu7Open = false;
    public bool menu8Open = false;

    public Text reputationText; //평판 텍스트
    public int reputation = 0; //평판 숫자

    public int seatNum = 0; //자리 넘버

    public GameObject NoHPMessege; //게임 중간이탈, 종료 시 비활성화됨

    public bool UIOn = false; // UI가 열려있지 않은 상태, 열려있으면 true

    public Queue<int> reactionFIn = new Queue<int>();//리액션 페이드인 시 사용
    Queue<int> reactionFOut = new Queue<int>();//리액션 페이드아웃 시 사용
    Queue<int> menuFIn = new Queue<int>();//메뉴 페이드인 시 사용
    public Queue<int> menuFOut = new Queue<int>();//메뉴 페이드아웃 시 사용

    public bool dinoOk = false; //디노 히로가 모두 서빙을 받았을 시 페이드아웃 가능
    public bool heroOk = false;
    public bool pOk = false; //도로시 서빙, 찰스랑 같이 다닐 때부터 사용
    public bool sOk = false; //찰스 서빙

    public Queue<int> together1 = new Queue<int>();//히로디노 자리 정보 저장
    public Queue<int> together2 = new Queue<int>();//도로시찰스 자리 정보 저장

    public GameObject[] SpecialMenu; //친밀도 이벤트에서 서빙되는 스페셜 메뉴 이미지 배열

    public Button[] LockedMenu; // 잠긴 메뉴 해제 버튼 배열, 0 메뉴5버튼 

    public int cNum;

    int goEvent = 0; //메뉴 서빙 후 이벤트가 나오는 캐릭터의 경우, 이벤트 캐릭터가 서빙을 받으면 1

    public int tmpNum = 0;//메뉴 서빙 후 이벤트 나오는 캐릭터들 메뉴 위치 임시로 저장

    public Queue<int> seatInfo = new Queue<int>(); //자리 정보 큐, 작은 캐릭터 페이드아웃 시 사용

    public Image NonameDessert;//무명이 디저트 2번째 이미지

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            UnlockedMenuItems.Add(MenuObject[0]); // 기본 메뉴 3개는 잠금X
            UnlockedMenuItems.Add(MenuObject[1]);
            UnlockedMenuItems.Add(MenuObject[2]);         
        }
    }

    void Start()
    {
        for (int i = 0; i < MenuObject.Length; i++)
        {
            MenuObject[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
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

    public void ClickMenuButton() //메뉴판 버튼 눌렀을 때
    {
        if(!UIOn) //다른 ui가 열려있지 않다면
        {
            MenuHint.instance.CantClickMHB();//다른 메뉴힌트버블 터치 불가
            UIOn = true;
            SEManager.instance.PlayUIClickSound(); //효과음
            menuButtonAnimator.SetTrigger("MenuButtonOut"); //메뉴 버튼 위로 올라가고
            menuBoardAnimator.SetTrigger("MenuBoardUp"); //메뉴판이 아래에서 올라옴
            for (int i = 0; i < UnlockedMenuItems.Count; i++) //메뉴 터치 불가, 서빙이 아닌 그냥 메뉴판 버튼 클릭 시에는 메뉴 클릭 못함
            {
                UnlockedMenuItems[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ClickMenuCloseButton() //메뉴 닫기 버튼 눌렀을 때
    {
        UIOn = false; //ui닫음
        SEManager.instance.PlayUICloseSound(); //효과음
        menuBoardAnimator.SetTrigger("MenuBoardDown"); //메뉴판 아래로 내려가고
        menuButtonAnimator.SetTrigger("MenuButtonIn"); //메뉴버튼 위에서 내려옴
        MenuHint.instance.CanClickMHB();
    }

    public void ClickMenuHint() //캐릭터의 메뉴 힌트 말풍선 눌렀을 때
    {
        if (!UIOn)
        {
            MenuHint.instance.CantClickMHB();//다른 메뉴힌트버블 터치 불가
            UIOn = true;
            SEManager.instance.PlayUIClickSound2(); //효과음
            menuButtonAnimator.SetTrigger("MenuButtonOut"); //메뉴 버튼 위로 올라가고
            menuBoardAnimator.SetTrigger("MenuBoardUp"); //메뉴판 올라옴
            close.GetComponent<Button>().interactable = true; //닫기 버튼 가능

            if (MenuHint.instance.tuto) //서빙 튜토리얼일 경우
            {
                close.GetComponent<Button>().interactable = false; //닫기 버튼 불가
                UI_Assistant1.instance.panel6.SetActive(false); //패널 없애고
                SmallFade.instance.InvisibleCharacter(); //캐릭터와 메뉴 힌트 말풍선 안 보이게, 보이게 하면 메뉴판 위로 겹쳐짐
                MenuHint.instance.HintFadeOut(1); //메뉴힌트 페이드아웃
                UI_Assistant1.instance.OpenDialogue2(); //다음 대사 나타남
                Invoke("CanClickMenu", 2f);
                //GameScript1.instance.Invoke("TutorialDownBox", 1.3f);
            }
            else
            {
                CanClickMenu();         
            }           
        }
        //Debug.Log("함수 ClickMenuHint");
    }

    public void CanClickMenu()
    {
        NoHPMessege.SetActive(false);
        for (int i = 0; i < UnlockedMenuItems.Count; i++) //메뉴 터치 가능
        {
            UnlockedMenuItems[i].GetComponent<Button>().interactable = true;
        }
    }

    public void CantClickMenu()
    {  
        for (int i = 0; i < UnlockedMenuItems.Count; i++) //메뉴 터치 가능
        {
            UnlockedMenuItems[i].GetComponent<Button>().interactable = false;
        }
    }

    private void NoHPMessegeFadeOut() //체력 없음 메세지 페이드아웃
    {
        NoHPMessegeAnimator.SetTrigger("NoHPFadeOut");
    }

    public void MenuServingFunction(int n)//서빙 함수
    {
        if (HPManager.instance.GetCurrentHP() <= 0) //체력이 0보다 작거나 같으면
        {
            CantClickMenu();
            NoHPMessege.SetActive(true); //체력 없음 메세지 보이게 함
            NoHPMessegeAnimator.SetTrigger("NoHPFadeIn");
            Invoke("NoHPMessegeFadeOut", 1f); //1초 뒤 메세지 페이드아웃
            Invoke("CanClickMenu", 1.5f);           
        }
        else //체력이 1이상이면
        {
            CantClickMenu();
            UIOn = false;
            menuBoardAnimator.SetTrigger("MenuBoardDown"); //메뉴판 아래로 내려가고
            menuButtonAnimator.SetTrigger("MenuButtonIn"); //메뉴버튼 위에서 내려옴
            MenuHint.instance.CanClickMHB();//다른 메뉴힌트버블 터치 가능

            HPManager.instance.UseHP(); //체력 소모

            if((cNum == 9 && CharacterAppear.instance.eventOn == 9) || (cNum == 10 && CharacterAppear.instance.eventOn == 10) || (cNum == 15 && CharacterAppear.instance.eventOn == 16))//친구,찰스1,롤렝드 친밀도 이벤트일 경우
            {//클릭된 캐릭터와 이벤트 캐릭터가 동일하면
                goEvent = 1;
                GameScript1.instance.CantClickUI();
                MenuHint.instance.CantClickMHB();//뒤에 메뉴판이 떠있는 채로 이벤트 시작하는 걸 방지하기 위함
                if (MenuHint.instance.RightMenu[seatNum] == n) //캐릭터가 원하는 메뉴가 n이면, 원하는 메뉴와 플레이어가 고른 메뉴가 일치
                {
                    SEManager.instance.PlayUIClickSound3();
                    VisitorNote.instance.GuessMenuRight(cNum, n);
                    if (CharacterAppear.instance.eventOn == 10)//찰스의 경우 평판 증가
                    {
                        reputation += 3;
                        reputationText.text = string.Format("{0}", reputation); //평판 표시
                        VisitorNote.instance.IncreaseFrinedshipGauge(cNum); //서빙받은 캐릭터의 친밀도 증가
                    }
                }
                else //n이 아니면
                {
                    SEManager.instance.PlayUIClickSound2();
                    if (CharacterAppear.instance.eventOn == 10)
                    {
                        reputation++;
                        reputationText.text = string.Format("{0}", reputation); //평판 표시
                        VisitorNote.instance.IncreaseFrinedshipGauge(cNum); //서빙받은 캐릭터의 친밀도 증가
                    }
                }
                TableMenu[seatNum].SetActive(true);
                SetMenuPosition();//테이블에 올려질 메뉴 위치 설정               
                MenuHint.instance.HintFadeOut(seatNum); //메뉴힌트 페이드아웃
                SetTableMenu(n, seatNum); //테이블에 올려질 메뉴는 n
                menuFIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
                tmpNum = seatNum;//대화 중 메뉴 페이드아웃 큐에 자리 정보 추가하기 위함
                MenuFadeIn();                
            }
            else
            {
                if (MenuHint.instance.RightMenu[seatNum] == n) //캐릭터가 원하는 메뉴가 n이면, 원하는 메뉴와 플레이어가 고른 메뉴가 일치
                {
                    SEManager.instance.PlayUIClickSound3();
                    if(CharacterAppear.instance.eventOn != 14)//히로디노 친밀도 이벤트가 아니면
                    {
                        CorrectMenuReaction(seatNum); //자리에 따라 맞는 메뉴 리액션 이미지 가져오기
                        reputation += 3; //원하는 메뉴 서빙 시 평판 3 증가
                    }                  
                    VisitorNote.instance.GuessMenuRight(cNum, n);
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
                SetTableMenu(n, seatNum); //테이블에 올려질 메뉴는 n                
                menuFIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
                MenuFadeIn();
                if (MenuHint.instance.tuto) //서빙 튜토리얼일 경우
                {
                    reactionFIn.Enqueue(seatNum); //리액션 페이드인 큐에 자리 정보 추가
                    SmallFade.instance.Invoke("VisibleCharacter",0.3f); //캐릭터 다시 보이게 하고
                                                                        // GameScript1.instance.TutorialUpBox();
                    UI_Assistant1.instance.Invoke("OpenDialogue2",0.5f);
                    SystemManager.instance.Invoke("SetCanTouchTrue", 1.5f);
                    VisitorNote.instance.IncreaseFrinedshipGauge(1);
                }
                else
                {
                    if((cNum == 11 && CharacterAppear.instance.eventOn == 12))//무명이1 이벤트이고 무명이 서빙을 했을 때
                    {
                        GameScript1.instance.CantClickUI();
                        MenuHint.instance.CantClickMHB();//뒤에 메뉴판이 떠있는 채로 이벤트 시작하는 걸 방지하기 위함
                        reactionFIn.Enqueue(seatNum); //리액션 페이드인 큐에 자리 정보 추가
                        goEvent = 1;
                    }
                    else if((cNum == 12 || cNum == 13) && CharacterAppear.instance.eventOn == 14)//히로디노 이벤트인데 캐릭터가 히로나 디노일 때
                    {
                        if(cNum == 12)
                        {
                            tmpNum = seatNum;//대화 중 메뉴 페이드아웃 큐에 자리 정보 추가하기 위함
                        }                    
                    }
                    else // 그 외의 경우
                    {                      
                        reactionFIn.Enqueue(seatNum); //리액션 페이드인 큐에 자리 정보 추가
                        //Debug.Log("리액션 페이드인큐에 들어간 자리 012345 => " + seatNum);
                        if (CharacterAppear.instance.eventOn != 5)
                        {
                            Invoke("ReactionFadeIn", 1f);
                        }     
                    }

                    if (cNum == 12 || cNum == 13)//디노나 히로 서빙이면
                    {
                        if (cNum == 12 && CharacterAppear.instance.eventOn == 14)//이벤트일 때만 히로디노OK 먼저 하기
                        {
                            heroOk = true;
                            SmallFade.instance.CanClickCharacter(13);//디노 클릭 가능                           
                        }
                        else if (cNum == 13 && CharacterAppear.instance.eventOn == 14)
                        {
                            dinoOk = true;
                        }

                        if (CharacterAppear.instance.eventOn != 14)//이벤트 아닐 때만
                        {
                            together1.Enqueue(seatNum);//히노디노큐에 자리 저장
                        }

                        if (CharacterAppear.instance.eventOn != 14)//히로디노 친밀도 이벤트가 아니면 친밀도 증가
                        {
                            VisitorNote.instance.IncreaseFrinedshipGauge(cNum);
                        }
                        else if (CharacterAppear.instance.eventOn == 14 && dinoOk && heroOk)// 둘 다 서빙완료했을 때, 친밀도 이벤트면 UI클릭 금지
                        {
                            MenuHint.instance.CantClickMHB();//뒤에 메뉴판이 떠있는 채로 이벤트 시작하는 걸 방지하기 위함
                            goEvent = 1;
                            GameScript1.instance.CantClickUI();
                        }
                    }
                    else if(Dialogue.instance.CharacterDC[10] == 3 && (cNum == 6 || cNum == 10))//찰스2이벤트가 끝난 후 찰스나 도로시일 경우
                    {
                        together2.Enqueue(seatNum);//찰스도로시큐에 자리 저장
                    }
                    else//디노히로, 찰스도로시가 아니면
                    {
                        VisitorNote.instance.IncreaseFrinedshipGauge(cNum); //서빙받은 캐릭터의 친밀도 증가
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

    public void SetTableMenu(int n, int num) //테이블에 올려질 메뉴 이미지 설정
    {
        switch(n)
        {
            case 1: //1일 경우 오렌지 주스 , 사이즈 조정과 이미지 삽입
                TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(UnlockedMenuItems[0].GetComponent<RectTransform>().rect.width, UnlockedMenuItems[0].GetComponent<RectTransform>().rect.height); //사이즈 조정
                TableMenu[num].GetComponent<Image>().sprite = UnlockedMenuItems[0].GetComponent<Image>().sprite;
                break;
            case 2: //2일 경우 핫초코 , 사이즈 조정과 이미지 삽입
                TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(UnlockedMenuItems[1].GetComponent<RectTransform>().rect.width, UnlockedMenuItems[1].GetComponent<RectTransform>().rect.height); //사이즈 조정
                TableMenu[num].GetComponent<Image>().sprite = UnlockedMenuItems[1].GetComponent<Image>().sprite;
                break;
            case 3: 
                TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(UnlockedMenuItems[2].GetComponent<RectTransform>().rect.width, UnlockedMenuItems[2].GetComponent<RectTransform>().rect.height); //사이즈 조정
                TableMenu[num].GetComponent<Image>().sprite = UnlockedMenuItems[2].GetComponent<Image>().sprite;
                break;
            case 4:
                TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(UnlockedMenuItems[3].GetComponent<RectTransform>().rect.width, UnlockedMenuItems[3].GetComponent<RectTransform>().rect.height); //사이즈 조정
                TableMenu[num].GetComponent<Image>().sprite = UnlockedMenuItems[3].GetComponent<Image>().sprite;
                break;
            case 5:
                TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(UnlockedMenuItems[4].GetComponent<RectTransform>().rect.width, UnlockedMenuItems[4].GetComponent<RectTransform>().rect.height); //사이즈 조정
                TableMenu[num].GetComponent<Image>().sprite = UnlockedMenuItems[4].GetComponent<Image>().sprite;
                break;
            case 6:
                TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(UnlockedMenuItems[5].GetComponent<RectTransform>().rect.width, UnlockedMenuItems[5].GetComponent<RectTransform>().rect.height); //사이즈 조정
                TableMenu[num].GetComponent<Image>().sprite = UnlockedMenuItems[5].GetComponent<Image>().sprite;
                break;
            case 7:
                TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(UnlockedMenuItems[6].GetComponent<RectTransform>().rect.width, UnlockedMenuItems[6].GetComponent<RectTransform>().rect.height); //사이즈 조정
                TableMenu[num].GetComponent<Image>().sprite = UnlockedMenuItems[6].GetComponent<Image>().sprite;
                break;
            case 8:
                TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(UnlockedMenuItems[7].GetComponent<RectTransform>().rect.width, UnlockedMenuItems[7].GetComponent<RectTransform>().rect.height); //사이즈 조정
                TableMenu[num].GetComponent<Image>().sprite = UnlockedMenuItems[7].GetComponent<Image>().sprite;
                break;
            case 11://여기서부터는 친밀도 이벤트의 메뉴들, 11은 도리 스페셜 메뉴
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[0].GetComponent<RectTransform>().rect.width, SpecialMenu[0].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[0].GetComponent<Image>().sprite;
                }  
                Popup.instance.SetPopupMenu(SpecialMenu[0], "도리", 110f);
                break;
            case 12:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[1].GetComponent<RectTransform>().rect.width, SpecialMenu[1].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[1].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[1], "붕붕이", 50f);
                break;
            case 13:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[2].GetComponent<RectTransform>().rect.width, SpecialMenu[2].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[2].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[2], "빵빵이", 80f);
                break;
            case 14:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[3].GetComponent<RectTransform>().rect.width, SpecialMenu[3].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[3].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[3], "개나리", 90f);
                break;
            case 16:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[4].GetComponent<RectTransform>().rect.width, SpecialMenu[4].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[4].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[4], "도로시", 70f);
                break;
            case 17:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[5].GetComponent<RectTransform>().rect.width, SpecialMenu[5].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[5].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[5], "루루", 90f);
                break;
            case 18:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[6].GetComponent<RectTransform>().rect.width, SpecialMenu[6].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[6].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[6], "샌디", 70f);
                break;
            case 19:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[7].GetComponent<RectTransform>().rect.width, SpecialMenu[7].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[7].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[7], "친구", 80f);
                break;
            case 21:
            case 31:
                if(n == 31)//디저트 이미지 교체
                {
                    SpecialMenu[8].GetComponent<Image>().sprite = NonameDessert.sprite;
                }
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[8].GetComponent<RectTransform>().rect.width, SpecialMenu[8].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[8].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[8], SystemManager.instance.GetNameForNameless() , 80f);
                break;
            case 22://히로디노는 똑같은 메뉴
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[12].GetComponent<RectTransform>().rect.width, SpecialMenu[12].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[12].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[9], "히로와 디노", 50f);
                break;
            case 23:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[13].GetComponent<RectTransform>().rect.width, SpecialMenu[13].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[13].GetComponent<Image>().sprite;
                }           
                break;
            case 24:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[10].GetComponent<RectTransform>().rect.width, SpecialMenu[10].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[10].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[10], "닥터 펭(을)", 50f);
                break;
            case 25:
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    TableMenu[num].GetComponent<RectTransform>().sizeDelta = new Vector2(SpecialMenu[11].GetComponent<RectTransform>().rect.width, SpecialMenu[11].GetComponent<RectTransform>().rect.height); //사이즈 조정
                    TableMenu[num].GetComponent<Image>().sprite = SpecialMenu[11].GetComponent<Image>().sprite;
                }
                Popup.instance.SetPopupMenu(SpecialMenu[11], "롤렝드", 90f);
                break;
        }
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
            reactionFIn.Enqueue(seatNum); //리액션 페이드인 큐에 자리 정보 추가
        }
        if(n == 11 && CharacterAppear.instance.eventOn == 13 && UI_Assistant1.instance.getMenu == 1)//무명이2 이벤트
        {
            tmpNum = seatNum;
        }
        menuFIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
    }

    public void SoldierEvent(int s)//찰스2 이벤트, 도로시에게 서빙
    {
        seatNum = s;
        SetMenuPosition();
        TableMenu[seatNum].SetActive(true);
        SetTableMenu(5, seatNum);//메뉴는 딸기스무디
        menuFIn.Enqueue(seatNum);//메뉴 페이드인 큐에 자리 정보 추가
        menuFOut.Enqueue(seatNum);//미리 추가
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
        int num = reactionFIn.Peek();       
        Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, 0);
        while (Reaction[num].GetComponent<Image>().color.a < 1.0f)
        {
            Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, Reaction[num].GetComponent<Image>().color.a + (Time.deltaTime / 0.8f));
            yield return null;
        }
        rFade = 0;//페이드인하는 반복문이 끝남=페이드인이 끝남
        reactionFIn.Dequeue();
        reactionFOut.Enqueue(num);//리액션 페이드아웃 큐에 정보 추가
        if (reactionFIn.Count != 0 && !IsInvoking("ReactionFadeIn"))//리액션 페이드인큐가 아직 있고 페이드인함수가 반복대기중이 아니면
        {
            ReactionFadeIn();//리액션 페이드인 실ㄴ
        }
    }

    public IEnumerator RFadeToZero()  // 알파값 1에서 0으로 전환, 리액션 페이드아웃
    {
        rFade2 = 1;//페이드아웃 중
        int num = reactionFOut.Peek();       
        Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, 1);
        while (Reaction[num].GetComponent<Image>().color.a > 0.0f)
        {
            Reaction[num].GetComponent<Image>().color = new Color(Reaction[num].GetComponent<Image>().color.r, Reaction[num].GetComponent<Image>().color.g, Reaction[num].GetComponent<Image>().color.b, Reaction[num].GetComponent<Image>().color.a - (Time.deltaTime / 1.0f));
            yield return null;
        }
        rFade2 = 0;
        reactionFOut.Dequeue();//페이드아웃 완료했으니 큐에서 정보 삭제
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
                    menuFOut.Enqueue(together1.Peek());    //메뉴 페이드아웃 큐에 데이터 추가 후           
                    MenuFadeOut();                        //메뉴 페이드아웃 실행
                    together1.Dequeue();
                    menuFOut.Enqueue(together1.Peek());
                    together1.Dequeue();
                }
            }
            else if (Dialogue.instance.CharacterDC[10] == 3 && (SmallFade.instance.SittingCharacter[num].name == "sPrincess" || SmallFade.instance.SittingCharacter[num].name == "sSoldier"))
            {
                if (SmallFade.instance.SittingCharacter[num].name == "sPrincess")
                {
                    pOk = true;
                }
                if (SmallFade.instance.SittingCharacter[num].name == "sSoldier")
                {
                    sOk = true;
                }
                if (pOk && sOk)//둘 다 서빙완료했으면
                {
                    pOk = false;
                    sOk = false;
                    menuFOut.Enqueue(together2.Peek());    //메뉴 페이드아웃 큐에 데이터 추가 후           
                    MenuFadeOut();                        //메뉴 페이드아웃 실행
                    together2.Dequeue();
                    menuFOut.Enqueue(together2.Peek());
                    together2.Dequeue();
                }
            }
            else //혼자 온 캐릭터
            {
                menuFOut.Enqueue(num);
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
        int n = menuFIn.Peek();
        mFade = true;
        TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, 0);
        while (TableMenu[n].GetComponent<Image>().color.a < 1.0f)
        {
            TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, TableMenu[n].GetComponent<Image>().color.a + (Time.deltaTime / 0.8f));
            yield return null;
        }
        mFade = false;
        menuFIn.Dequeue();

        if((CharacterAppear.instance.eventOn == 9 || CharacterAppear.instance.eventOn == 10)&& UI_Assistant1.instance.getMenu == 0 && goEvent == 1)//친구,찰스1 친밀도 이벤트 처음일 때
        {
            GameScript1.instance.CharacterStart(CharacterAppear.instance.eventOn);//시나리오 시작
            goEvent = 0;
        }
        if(CharacterAppear.instance.eventOn == 12 && goEvent == 1)//무명이1 친밀도 이벤트의 경우
        {
            GameScript1.instance.CharacterStart(11);
            goEvent = 0;
        }
        if (CharacterAppear.instance.eventOn == 14 && heroOk && dinoOk && UI_Assistant1.instance.getMenu == 0 && goEvent == 1)//히로디노 친밀도 이벤트의 경우
        {
            GameScript1.instance.CharacterStart(12);
            goEvent = 0;
        }
        if(CharacterAppear.instance.eventOn == 16 && UI_Assistant1.instance.getMenu == 0 && goEvent == 1)//롤렝드 친밀도 이벤트의 경우
        {
            GameScript1.instance.GrandfatherStart();
            SmallFade.instance.smallFIn.Enqueue(17);
            SmallFade.instance.FadeIn();
            goEvent = 0;
        }
    }

    public IEnumerator FadeToZero()  // 알파값 1에서 0으로 전환, 메뉴 페이드아웃
    {
        mFade2 = true;
        int n = menuFOut.Peek();

        TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, 1);
        while (TableMenu[n].GetComponent<Image>().color.a > 0.0f)
        {
            TableMenu[n].GetComponent<Image>().color = new Color(TableMenu[n].GetComponent<Image>().color.r, TableMenu[n].GetComponent<Image>().color.g, TableMenu[n].GetComponent<Image>().color.b, TableMenu[n].GetComponent<Image>().color.a - (Time.deltaTime / 1.0f));
            yield return null;
        }
        mFade2 = false;
        menuFOut.Dequeue();
        TableMenu[n].SetActive(false);
        if((CharacterAppear.instance.eventOn != 9 && CharacterAppear.instance.eventOn != 11 && CharacterAppear.instance.eventOn != 13 && CharacterAppear.instance.eventOn != 14 && CharacterAppear.instance.eventOn != 16)
            || (SmallFade.instance.SittingCharacter[n].name != "sDog" && CharacterAppear.instance.eventOn == 9)
            || (SmallFade.instance.SittingCharacter[n].name != "sPrincess" && SmallFade.instance.SittingCharacter[n].name != "sSoldier" && CharacterAppear.instance.eventOn == 11)
            || (SmallFade.instance.SittingCharacter[n].name != "sHero" && SmallFade.instance.SittingCharacter[n].name != "sDinosour" && CharacterAppear.instance.eventOn == 14)
            || (SmallFade.instance.SittingCharacter[n].name != "sGrandfather" && CharacterAppear.instance.eventOn == 16))
        {//특정 캐릭터 친밀도 이벤트가 아닐 때, 혹은 특정 캐릭터 이벤트여도 해당 이벤트 캐릭터가 아닐 때
            seatInfo.Enqueue(n);//EachObject의 seatInfo 큐에 자리 정보 저장, 작은 캐릭터 페이드아웃 시 쓰임
            //Debug.Log("페이드아웃될 자리 " + n);
            SmallFade.instance.FadeOut(); //작은 캐릭터 페이드아웃   
        }
        else if(CharacterAppear.instance.eventOn == 14 && SmallFade.instance.SittingCharacter[n].name == "sHero" && UI_Assistant1.instance.getMenu == 2)
        {//히로디노 이벤트 중에 페이드아웃하는 캐릭터가 히로이고 디노 페이드아웃 전일 때
            seatInfo.Enqueue(n);//EachObject의 seatInfo 큐에 자리 정보 저장, 작은 캐릭터 페이드아웃 시 쓰임
            SmallFade.instance.FadeOut(); //작은 캐릭터 페이드아웃
        }
        if (menuFOut.Count != 0)
        {
            MenuFadeOut();
        }
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
            LockedMenu[0].gameObject.SetActive(true);//메뉴 5 해제 버튼 활성화
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
            LockedMenu[1].gameObject.SetActive(true);
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
            LockedMenu[2].gameObject.SetActive(true);
            GameScript1.instance.TipBubbleOn();
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
            LockedMenu[3].gameObject.SetActive(true);
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
            LockedMenu[x - 4].gameObject.SetActive(true);//다음 메뉴 해제 버튼 활성화
        }
    }
}
