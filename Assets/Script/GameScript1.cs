using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class GameScript1 : MonoBehaviour //전체적인 게임스트립트
{
    public static GameScript1 instance;
    public GameObject panel; //대화할 때 쓰는 회색 반투명 패널
    public GameObject[] BigCharacter; //큰 캐릭터 이미지 배열
    public int textnum = 0; // 0 = character, 1 = baby
    public GameObject babyTextBox;
    public GameObject characterTextBox;



    public GameObject[] Seat; //작은 캐릭터가 앉을 자리 배열
    public GameObject servingTutorial;//서빙 튜토 때만 쓰이는 도리와 메뉴힌트버블

    public static bool jejeOn = false; //현재 캐릭터가 제제일 경우 true

    public GameObject bear;

    public int endStory = 0;//1이면 엔딩 이벤트 실행된 것

    public bool delete = false;


    public GameObject PurchasingWindow;//붕어빵 버튼 눌렀을 때 나오는 창
    public Text PurchasingText; //창 메세지
    public Text FishBreadText; //붕어빵 버튼 텍스트

    public GameObject CompletePurchasingWindow;//결제 후 메세지 창
    public Text CompletePurchasingText;//메세지 내용

    public GameObject pButton1;//체력 증가 버튼
    public GameObject pButton2;//회복 속도 증가 버튼

    public Text littleFishBreadText;

    public Button FishBreadButton;//붕어빵 버튼
    public Image FishBread2;//두 번 결제 후 붕어빵 버튼 이미지

    public Image JejeBubble;
    public Text JejeText;

    public Animator jejeBubbleAnimator;

    public Image Tip;
    public Image TipMessage;

    public Image StarLackMessage;
    public Animator starLackAnimator;
    public Button TipNoteButton;
    public Animator TNBAnimator;
    public Image TipNote;
    public Animator TipNoteAnimator;

    public Text Tip2;
    public Text Tip3;

    public int mainCount = 0; 


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

   


   
    public void BackToCafe() //대화를 끝내고 카페로 복귀
    {
        switch (mainCount)
        {
            case 0: //제제의 튜토리얼 설명이 끝난 상황
                SmallFade.instance.SetCharacter(0); //제제 작은 캐릭터 설정
                DownTextbox(); //대화창 내려가고 캐릭터도 아웃
                CantClickUI(); //그 후 바로 도리가 등장할 것이므로 설정 제외한 버튼들 모두 터치 불가
                SmallFade.instance.Invoke("FadeIn",1f); //제제 작은 캐릭터 페이드인
                Invoke("BearStart", 2f); //도리 등장
                Dialogue.instance.CharacterNum = 1; //다음 캐릭터 곰
                Dialogue.instance.CharacterDC[0]++; //제제 다이얼로그 카운트 증가
                mainCount++; //메인 카운트 증가
                break;
            case 1:
                DownTextbox(); //도리 카페 첫 방문 후
                Popup.instance.SetPopupCharacter(BigCharacter[1]); //새 캐릭터 도리 팝업 세팅
                Popup.instance.OpenPopup(); //팝업 등장
                Dialogue.instance.CharacterNum = 0; //다음 캐릭터 제제(튜토리얼)
                Dialogue.instance.CharacterDC[1]++; //도리 다이얼로그 카운트 증가
                mainCount++; //메인 카운트 증가
                break;
            case 2:
                DownTextbox(); //서빙 튜토 끝남                   
                Menu.instance.Invoke("ReactionFadeIn", 1f);
                UI_Assistant1.instance.panel7.SetActive(false);               
                SmallFade.instance.FadeIn();
                Dialogue.instance.CharacterNum = 2; //다음 등장 캐릭터 붕붕
                Dialogue.instance.CharacterDC[0]++; //제제 다이얼로그 카운트 증가                              
                MenuHint.instance.tuto = false;
                Menu.instance.close.GetComponent<Button>().interactable = true; //메뉴 닫기 버튼 가능
                jejeOn = false;
                mainCount++; //메인 카운트 증가
                Invoke("CarStart", 7f);
                break;
            case 3: //붕붕이 등장 후
                BgmManager.instance.BGMFadeOut();
                DownTextbox();
                SmallFade.instance.Invoke("FadeIn", 1f);
                CanClickUI();//메뉴판,노트, 체력 충전 버튼 터치 가능
                BgmManager.instance.Invoke("PlayCafeBgm",3f);
                Popup.instance.SetPopupCharacter(BigCharacter[2]); //변경되는 것
                Popup.instance.OpenPopup();
                CharacterAppear.instance.SetNextAppearNum(3); //다음 등장은 빵빵
                Dialogue.instance.CharacterDC[2]++; //변경되는 것
                VisitorNote.instance.page[1].SetActive(true); //변경되는 것
                VisitorNote.instance.openPage++;
                SmallFade.instance.SmallCharacter[1] = bear;
                CharacterVisit.instance.Invoke("RandomVisit", 10f); //캐릭터 랜덤 방문
                c = 2;
                Invoke("InActiveBigCharacter", 1f);
                mainCount++;
                Star.instance.ActivateStarSystem();//바로 별 활성화 함수 시작
                break;
            case 6: //또롱 등장 후 
                AfterTalking(mainCount);
                VisitorNote.instance.nextPageButton.SetActive(true); //5번째 페이지로 넘어가기 위해 다음 페이지 버튼 보이게함   
                SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = true;//제제 터치 가능
                TipBubbleOn();//팁 등장
                break;
            case 4://빵빵 등장 후            
            case 5://개나리
            case 7://도로시               
            case 8://루루
            case 9://샌디
            case 10://친구
            case 11://찰스
            case 12://무명이
            case 13://히로디노
            case 14://닥터펭
            case 15://롤렝드
                AfterTalking(mainCount);
                break;
        }
        PlayerPrefs.SetInt("MainCount", mainCount); //현재 메인카운트 저장
        PlayerPrefs.SetInt("NextAppear", CharacterAppear.instance.GetNextAppearNum()); //다음 캐릭터 등장 번호 저장
        PlayerPrefs.Save(); //세이브
        Dialogue.instance.SaveCharacterDCInfo();
        VisitorNote.instance.SaveVisitorNoteInfo();
    }

    void AfterTalking(int n)
    {       
        BgmManager.instance.BGMFadeOut();
        DownTextbox();
        BgmManager.instance.Invoke("PlayCafeBgm", 3f);
        if (n != 9 && n != 15)
        {
            Popup.instance.SetPopupCharacter(BigCharacter[n - 1]); //새 손님 팝업           
        }
        else if (n == 9)
        {
            Popup.instance.SetPopupCharacter(BigCharacter[15]);//샌디 이미지2, 이미지 비율/위치 문제
        }
        else if(n == 15)
        {
            Popup.instance.SetPopupCharacter(BigCharacter[16]);//롤렝드 이미지2
        }
        Popup.instance.OpenPopup();
        CharacterAppear.instance.SetNextAppearNum(n); //다음 등장 캐릭터 설정 
        SmallFade.instance.Invoke("FadeIn", 1f);

        if (!CharacterVisit.instance.IsInvoking("RandomVisit"))
        {
            CharacterVisit.instance.Invoke("RandomVisit", 10f); //캐릭터 랜덤 방문
        }
        if(mainCount >= 6)
        {
            VisitorNote.instance.NextPageOpen[mainCount - 6] = 1; //페이지 열렸음
        }
        else if(mainCount < 6)
        {
            VisitorNote.instance.page[mainCount - 2].SetActive(true); //손님 노트 페이지 오픈
        }
        VisitorNote.instance.openPage++;
        c = n - 1;
        Invoke("InActiveBigCharacter", 1f);//1초 후 큰 캐릭터 비활성화
        mainCount++;
    }

       public void BearStart() //튜토리얼 도리 등장
    {
        textnum = 0; //캐릭터 대사
        CharacterIn(1); //캐릭터 등장
    }

    public void ServingTutorial() //서빙 튜토리얼
    {
        jejeOn = true;
        textnum = 0;
        SmallFade.instance.FadeOut(); //작은 제제 페이드아웃
        CharacterIn(0);
    }

    public void CharacterStart(int n) //제제/도리/붕붕/롤렝드 제외 모든 캐릭터 등장 함수
    {
        gameObject.GetComponent<CharacterAppear>().enabled = false;//캐릭터 등장 비활성화
        if (BgmManager.instance.IsInvoking("PlayCafeBgm"))//카페 배경음이 invoke중이면
        {
            BgmManager.instance.CancelInvoke("PlayCafeBgm");//invoke 취소
        }
        BgmManager.instance.StopBgm();
        BgmManager.instance.PlayCharacterBGM(n);//캐릭터 테마 재생      
        textnum = 0;
        Dialogue.instance.CharacterNum = n;
        CharacterIn(n);
    }

    public void CarStart() //붕붕 등장
    {
        servingTutorial.SetActive(false);
        SmallFade.instance.tutorialBear.SetActive(false);//튜토리얼 오브젝트 삭제
        MenuHint.instance.tutorialBubble.SetActive(false);
        textnum = 0;
        BgmManager.instance.StopBgm();
        BgmManager.instance.PlayCharacterBGM(2);//붕붕 테마 재생
        CharacterIn(2);
        MenuHint.instance.tuto = false;
    }
    
    public void GrandfatherStart() //롤렝드 등장
    {
        if (BgmManager.instance.IsInvoking("PlayCafeBgm"))//카페 배경음이 invoke중이면
        {
            BgmManager.instance.CancelInvoke("PlayCafeBgm");//invoke 취소
            Debug.Log("카페브금 인보크 종료");
        }
        //CharacterManager.instance.SetMovingCharacter(14);
        Dialogue.instance.CharacterNum = 14;
        textnum = 1; //할아버지 캐릭터만 특수하게 이렇게 시작
        panel.SetActive(true);
        UpTextBox();
        BgmManager.instance.BGMFadeOut();
    }

    int c = 0;
    void InActiveBigCharacter()//큰 캐릭터 비활성화하는 함수
    {
        CharacterManager.instance.DeactivateCharacterMoving();
        if(f != 0)//표정 비활성화해야할 것이 있으면
        {
            InActiveFace();
        }
    }

    public void BackToCafe2(int n)//이벤트 대화 종료 후, n은 Dialogue의 캐릭터 넘버
    {
        if(endStory == 1)//엔딩이벤트 본 후
        {
            BgmManager.instance.BGMFadeOut();
            DownTextbox();
            Dialogue.instance.CharacterDC[n] = 3;
            HPManager.instance.SaveHPInfo();
            TimeManager.instance.SaveAppQuitTime();
            Dialogue.instance.SaveCharacterDCInfo();
            Menu.instance.SaveUnlockedMenuItemInfo();
            endStory = 2;//엔딩이벤트를 봤음
            SystemManager.instance.SaveDataInfo();//데이터 저장
            VisitorNote.instance.SaveVisitorNoteInfo();           
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
            SceneChanger.instance.Invoke("GoEndingCreditScreen", 1f);//1초 후 엔딩크레딧 화면으로 이동
        }
        else
        {
            CanClickUI();
            CharacterAppear.instance.eventOn = 0; //친밀도 이벤트 종료됨                                                 
            DownTextbox();
            switch (n)
            {
                case 1://도리               
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 12:
                case 13:
                case 14:      
                    if(n == 1 || n == 7 || n == 9 || n == 13)//도리, 루루, 친구, 닥터펭은 표정 비활성화 필요
                    {
                        f = n;
                    }
                    MenuHint.instance.CanClickMHB();//메뉴힌트버블 터치 가능
                    BgmManager.instance.BGMFadeOut();
                    BgmManager.instance.Invoke("PlayCafeBgm", 3f);
                    Menu.instance.ReactionFadeIn();
                    Dialogue.instance.CharacterDC[n] = 3; // 3이면 더 이상 시나리오 없음
                    VisitorNote.instance.RePlayButton[n - 1].gameObject.SetActive(true);//다시보기 버튼 활성화
                    c = n;
                    Invoke("InActiveBigCharacter", 1f);
                    if(n == 1)//도리는 손님노트 이미지를 2번째 표정으로 바꿈
                    {
                        VisitorNote.instance.characterInfo[n - 1].GetComponent<Image>().sprite = CharacterManager.instance.CharacterFaceList[n].face[1].GetComponent<Image>().sprite;
                        TipBubbleOn();
                    }
                    else if(n == 12 || n == 13)//히로디노, 닥터펭은 1번째 표정으로 바꾸기
                    {
                        VisitorNote.instance.characterInfo[n - 1].GetComponent<Image>().sprite = CharacterManager.instance.CharacterFaceList[n - 2].face[0].GetComponent<Image>().sprite;
                    }
                    else if(n == 14)//롤렝드는 따로
                    {
                        VisitorNote.instance.characterInfo[n - 1].GetComponent<Image>().sprite = BigCharacter[17].GetComponent<Image>().sprite;
                    }
                    SystemManager.instance.CheckEndingCondition();
                    break;
                case 10:
                    f = 10;
                    if (Dialogue.instance.CharacterDC[n] == 1)//찰스1 이벤트
                    {
                        MenuHint.instance.CanClickMHB();//메뉴힌트버블 터치 가능
                        BgmManager.instance.BGMFadeOut();
                        BgmManager.instance.Invoke("PlayCafeBgm", 3f);
                        Dialogue.instance.CharacterDC[10]++;
                        SmallFade.instance.CanClickCharacter(6);//도로시 클릭 가능하게
                        Menu.instance.menuFOut.Enqueue(Menu.instance.tmpNum); //메뉴 페이드아웃 큐에 추가
                        Menu.instance.MenuFadeOut();//메뉴 페이드아웃
                        c = n;
                        Invoke("InActiveBigCharacter", 1f);
                    }
                    else if (Dialogue.instance.CharacterDC[n] == 2)//찰스2 이벤트
                    {
                        MenuHint.instance.CanClickMHB();//메뉴힌트버블 터치 가능
                        BgmManager.instance.BGMFadeOut();
                        BgmManager.instance.Invoke("PlayCafeBgm", 3f);
                        Dialogue.instance.CharacterDC[10] = 3;
                        UI_Assistant1.instance.getMenu = 0;
                        SmallFade.instance.CanClickCharacter(6);//도로시 클릭 가능하게
                        SmallFade.instance.CanClickCharacter(10);//찰스 클릭 가능하게
                        c = 6;
                        Invoke("InActiveBigCharacter", 0.5f);
                        VisitorNote.instance.RePlayButton[n - 1].gameObject.SetActive(true);         
                    }
                    SystemManager.instance.CheckEndingCondition();
                    break;
                case 11:
                    if (Dialogue.instance.CharacterDC[n] == 1)//무명이1 이벤트
                    {
                        f = 11;
                        Dialogue.instance.CharacterDC[11]++;
                    }
                    else if (Dialogue.instance.CharacterDC[n] == 2)//무명이2 이벤트
                    {
                        f = 12;
                        Dialogue.instance.CharacterDC[11] = 3;
                        VisitorNote.instance.characterInfo[n - 1].GetComponent<Image>().sprite = CharacterManager.instance.CharacterFaceList[n - 2].face[3].GetComponent<Image>().sprite;
                        VisitorNote.instance.RePlayButton[n - 1].gameObject.SetActive(true);
                    }
                    MenuHint.instance.CanClickMHB();//메뉴힌트버블 터치 가능
                    BgmManager.instance.BGMFadeOut();
                    BgmManager.instance.Invoke("PlayCafeBgm", 3f);
                    Menu.instance.ReactionFadeIn();
                    c = n;
                    Invoke("InActiveBigCharacter", 1f);
                    SystemManager.instance.CheckEndingCondition();
                    break;

            }
            if (!CharacterVisit.instance.IsInvoking("RandomVisit"))
            {
                CharacterVisit.instance.Invoke("RandomVisit", 12f); //캐릭터 랜덤 방문
               // Debug.Log("랜덤방문 10초 뒤");
            }
        }
        Dialogue.instance.SaveCharacterDCInfo();
        VisitorNote.instance.SaveVisitorNoteInfo();
    }

    int f = 0;//표정 비활성화에 쓰임
    void InActiveFace()
    {
        if (f == 1)//도리
        {
            CharacterManager.instance.CharacterFaceList[1].face[1].SetActive(false);//표정 비활성화
        }
        else if (f == 10)//찰스1,2
        {
            CharacterManager.instance.CharacterFaceList[8].face[2].SetActive(false);
        }
        else if (f == 11 || f == 7)//무명1, 루루
        {
            CharacterManager.instance.CharacterFaceList[f - 2].face[1].SetActive(false);//표정 비활성화
        }
        else if (f == 12)//무명2
        {
            CharacterManager.instance.CharacterFaceList[9].face[3].SetActive(false);
        }
        else if (f == 13 || f == 9)//닥터펭, 친구 이벤트의 경우
        {
            CharacterManager.instance.CharacterFaceList[f - 2].face[0].SetActive(false);
        }
    }

    void EndingEvent()
    {
        if(!Menu.instance.UIOn && SmallFade.instance.TableEmpty[0] == 0 && SmallFade.instance.TableEmpty[1] == 0 && SmallFade.instance.TableEmpty[2] == 0)
        {//테이블이 모두 비었고 UI가 올라와있지 않은 상태에서 실행
            CantClickUI();
            jejeOn = true;
            JejeBubble.gameObject.SetActive(false);
            Tip.gameObject.SetActive(false);
            SmallFade.instance.FadeOut(); //작은 제제 페이드아웃
            CharacterStart(0);
            SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = false;//제제 클릭 불가
        }
        else
        {
            Invoke("EndingEvent", 2f);
        }
    }



    public void AfterRePlaySenario()//시나리오 다시보기를 마친 후 실행
    {
        VisitorNote.instance.replayOn = 2;
        BgmManager.instance.BGMFadeOut();
        VisitorNote.instance.ClickVNButton(); //노트 올라오기
        DownTextbox();//대화상자 내리고 캐릭터 아웃
        BgmManager.instance.Invoke("PlayCafeBgm", 3f);
        if (VisitorNote.instance.fmRP != 0)//첫 만남 다시보기 이후면
        {            
            Dialogue.instance.CharacterDC[VisitorNote.instance.fmRP] = 3;// 원래 값으로 돌려놓기
            c = VisitorNote.instance.fmRP;
            Invoke("InActiveBigCharacter", 0.7f);
            VisitorNote.instance.fmRP = 0;
        }
        if (VisitorNote.instance.evRP != 0)
        {
            if(VisitorNote.instance.evRP <= 10)//찰스1 이벤트까지
            {
                if (VisitorNote.instance.evRP == 1 || VisitorNote.instance.evRP == 7 || VisitorNote.instance.evRP == 10)//도리, 루루, 찰스 표정 비활성화 필요
                {
                    f = VisitorNote.instance.evRP;
                }
                c = VisitorNote.instance.evRP;
                Invoke("InActiveBigCharacter", 0.7f);
                Dialogue.instance.CharacterDC[VisitorNote.instance.evRP] = 3;// 원래 값으로 돌려놓기
            }
            else if(VisitorNote.instance.evRP == 11)//찰스2 이밴트
            {
                f = 10;
                c = 6;
                Invoke("InActiveBigCharacter", 0.5f);
                Dialogue.instance.CharacterDC[10] = 3;// 원래 값으로 돌려놓기
            }
            else if(VisitorNote.instance.evRP == 12 || VisitorNote.instance.evRP == 13)//무명이1,2 이벤트
            {
                if (VisitorNote.instance.evRP == 12)//무명이1 이벤트
                {
                    f = 11;
                }
                else if (VisitorNote.instance.evRP == 13)//무명이2 이벤트
                {
                    f = 12;
                }
                c = 11;
                Invoke("InActiveBigCharacter", 0.5f);
                Dialogue.instance.CharacterDC[11] = 3;// 원래 값으로 돌려놓기
            }
            else if(VisitorNote.instance.evRP >= 14)//14이상이면
            {
                if (VisitorNote.instance.evRP == 15)//닥터펭 이벤트
                {
                    f = 13;
                }
                c = VisitorNote.instance.evRP - 2;
                Invoke("InActiveBigCharacter", 0.7f);
                Dialogue.instance.CharacterDC[c] = 3;// 원래 값으로 돌려놓기
            }
            
            VisitorNote.instance.evRP = 0;
        }
        Invoke("EndRePlay", 1.1f);
    }

    void EndRePlay()//다시보기가 완전히 끝났음
    {
        VisitorNote.instance.replayOn = 0;
    }

   

  

    int price = 1000;












  


    public GameObject exchangeMessageWindow;

 

