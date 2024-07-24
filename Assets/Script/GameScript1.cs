using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class GameScript1 : MonoBehaviour //전체적인 게임스트립트
{
    public static GameScript1 instance;
    public GameObject panel; //대화할 때 쓰는 회색 반투명 패널
    public GameObject[] BigCharacter; //큰 캐릭터 이미지 배열
    public int mainCount = 0; 
    public Animator TBAnimator; //텍스트박스 애니메이터
    public int textnum = 0; // 0 = character, 1 = baby
    public GameObject babyTextBox;
    public GameObject characterTextBox;
    public Button menu;
    public Button note;
    public Button plusHP;
    public GameObject[] Seat; //작은 캐릭터가 앉을 자리 배열
    public GameObject servingTutorial;//서빙 튜토 때만 쓰이는 도리와 메뉴힌트버블

    public static bool jejeOn = false; //현재 캐릭터가 제제일 경우 true

    public GameObject bear;

    public int endStory = 0;//1이면 엔딩 이벤트 실행된 것

    public bool delete = false;

    public GameObject AdsMessage;
    public GameObject GameClose;

    public GameObject PurchasingWindow;//붕어빵 버튼 눌렀을 때 나오는 창
    public Text PurchasingText; //창 메세지
    public Text FishBreadText; //붕어빵 버튼 텍스트

    public int pNum = 0; //purchasingNumber, 한번 결제 1, 두번 결제 2

    public GameObject CompletePurchasingWindow;//결제 후 메세지 창
    public Text CompletePurchasingText;//메세지 내용

    public GameObject pButton1;//체력 증가 버튼
    public GameObject pButton2;//회복 속도 증가 버튼

    public Text littleFishBreadText;

    public Button FishBreadButton;//붕어빵 버튼
    public Image FishBread2;//두 번 결제 후 붕어빵 버튼 이미지

    public Text GameCloseWarning;

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        BgmManager.instance.PlayCafeBgm(); //카페 브금 재생

        if (mainCount == 0)
        {
            servingTutorial.SetActive(true);
            FirstTutorial(); //게임의 첫 튜토리얼 실행
        }             
    }

    public void Cheat()
    {
        
       // tipNum = 0;
        //Menu.instance.menu8Open = 1;
        // Star.instance.starCoroutine = Star.instance.StartCoroutine(Star.instance.StarAppear());
        // mainCount = 16;
         // Menu.instance.reputation += 300;
        // pNum = 0;
        //PlayerPrefs.DeleteKey("pNum");
       //    VisitorNote.instance.openPage = 14;
        Dialogue1.instance.CharacterDC[9] = 3; //1도리
        Dialogue1.instance.CharacterDC[10] = 3; //1도리
        Dialogue1.instance.CharacterDC[12] = 3; //1도리
        //endStory = 1;
        //VisitorNote.instance.friendshipGauge[8].GetComponent<Image>().fillAmount = 1f; //0 도리~~ 4도로시~~~
        //VisitorNote.instance.FriendshipInfo[8] = 10;
        //VisitorNote.instance.friendshipGauge[9].GetComponent<Image>().fillAmount = 1f; 
        //VisitorNote.instance.FriendshipInfo[9] = 10;

    }
    public void Reset_P()
    {
        pNum = 0;
        PlayerPrefs.SetInt("pNum", pNum); //인앱 결제 정보 저장
        PlayerPrefs.Save(); //세이브
    }

    public bool SaveDataInfo() //게임 데이터 정보 저장
    {
        //Debug.Log("SaveDataInfo");
        bool result = false;
        try
        {
            PlayerPrefs.SetInt("MainCount", mainCount); //현재 메인카운트 저장
            PlayerPrefs.SetInt("NextAppear", CharacterAppear.instance.GetNextAppearNum()); //다음 캐릭터 등장 번호 저장
            PlayerPrefs.SetInt("Reputation", Menu.instance.reputation); //평판 저장
            PlayerPrefs.SetInt("StarNum", Star.instance.starNum); //별 개수 저장
            PlayerPrefs.SetInt("end", endStory);//엔딩 상황 저장
            PlayerPrefs.SetInt("pNum", pNum); //인앱 결제 정보 저장
            PlayerPrefs.SetInt("tipNum", tipNum);//팁 넘버 
            PlayerPrefs.Save(); //세이브
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveDataInfo Failed (" + e.Message + ")");
        }
        return result;
    }

    public bool LoadDataInfo() //게임 데이터 정보 불러옴
    {
        //Debug.Log("LoadDataInfo");
        bool result = false;
        try
        {
            if (PlayerPrefs.HasKey("MainCount"))
            {
                mainCount = PlayerPrefs.GetInt("MainCount");                
                CharacterAppear.instance.SetNextAppearNum(PlayerPrefs.GetInt("NextAppear"));
                Menu.instance.reputation = PlayerPrefs.GetInt("Reputation");
                Star.instance.starNum = PlayerPrefs.GetInt("StarNum");
                endStory = PlayerPrefs.GetInt("end");       
                pNum = PlayerPrefs.GetInt("pNum");
                tipNum = PlayerPrefs.GetInt("tipNum");
                if (mainCount > 3)//붕붕이 등장 이후면
                {
                   // Debug.Log("중요 데이터 로드");
                    SmallFade.instance.SetCharacter(0);
                    SmallFade.instance.Invoke("FadeIn", 1f); //제제 작은 캐릭터 페이드인                   
                    Dialogue1.instance.LoadCharacterDCInfo();
                    VisitorNote.instance.LoadVisitorNoteInfo();
                    Menu.instance.LoadUnlockedMenuItemInfo();                 
                    Dialogue1.instance.babyName = PlayerPrefs.GetString("BabyName");
                    Invoke("CheckTip", 1.5f);//팁 확인
                    if (endStory != 3)//엔딩이벤트를 본 게 아니라면
                    {
                        AllScenarioEnd();//엔딩 이벤트 충족 조건 확인해주기
                    } 
                    for (int i = 1; i <= mainCount - 2; i++)//재방문 캐릭터 설정
                    {
                        if(Dialogue1.instance.CharacterDC[10] == 3)//찰스2 이벤트를 했을 시에는
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
                    SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = true;
                }
                //Debug.Log("다음 등장 :" + CharacterAppear.instance.GetNextAppearNum());
                //Debug.Log("이벤트 넘버 : " + CharacterAppear.instance.eventOn);
                //Debug.Log("메인카운트 " + mainCount);
            }
            else
            {
                mainCount = 0;
                CharacterAppear.instance.SetNextAppearNum(0);
                Menu.instance.reputation = 0;
                Star.instance.starNum = 0;
                if(PlayerPrefs.HasKey("pNum"))
                {
                    pNum = PlayerPrefs.GetInt("pNum");
                }
            }
            Menu.instance.reputationText.text = string.Format("{0}", Menu.instance.reputation);
            Star.instance.StarNumText.text = string.Format("{0}", Star.instance.starNum.ToString());
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadDataInfo Failed (" + e.Message + ")");
        }   
           // Cheat();
        return result;
    }

    void FirstTutorial()
    {
        gameObject.GetComponent<CharacterMove>().enabled = true;
        BigCharacter[0].SetActive(true);
        CharacterMove.instance.SetCharacter(BigCharacter[0]); //제제로 큰 캐릭터 설정
        CharacterIn();
    }

    public void CharacterIn() //대화할 캐릭터가 화면에 들어오도록
    {       
        CharacterMove.instance.InCount(); //InCount()를 하면 자동으로 캐릭터 들어옴
        panel.SetActive(true); //캐릭터가 들어옴과 동시에 회색 패널 작동
        UpTextBox(); // 대화창 등장
    }

    public void CharacterOut() //캐릭터가 화면 밖으로 나가도록
    {
        CharacterMove.instance.OutCount(); //자동으로 나감
        panel.SetActive(false); //회색 패널 해제
    }

    void UpTextBox()
    {
        if(textnum == 0) //character text
        {
            characterTextBox.SetActive(true);
        }
        else //baby text
        {
            babyTextBox.SetActive(true);
        }

        TBAnimator.SetTrigger("TextBoxUp");
        UI_Assistant1.instance.OpenDialogue(); //대화 시작, 대사 띄움
    }

    void DownTextbox() //대화창 내려감
    {
        TBAnimator.SetTrigger("TextBoxDown");       
        CharacterOut(); // 캐릭터 나가도록
        Invoke("InActiveTextBox", 0.4f);
    }

    public void TutorialDownBox()//서빙 튜토리얼 시 사용
    {
        TBAnimator.SetTrigger("TextBoxDown");
        if(UI_Assistant1.instance.count == 2)
        {
            SmallFade.instance.tutorialBear.GetComponent<Button>().interactable = true;
        }
    }

    public void TutorialUpBox()//서빙 튜토리얼 시 사용
    {
        TBAnimator.SetTrigger("TextBoxUp");
        UI_Assistant1.instance.Invoke("OpenDialogue2", 0.1f); //다음 대사
    }

    void InActiveTextBox()
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
    public void ChageToBaby() // 캐릭터 대사창에서 아기 대사창으로 전환
    {
        characterTextBox.SetActive(false);
        babyTextBox.SetActive(true);
    }

    public void ChageToCharacter() //아기 대사창에서 캐릭터 대사창으로 전환
    {
        babyTextBox.SetActive(false);
        characterTextBox.SetActive(true);   
    }
   
    public void BackToCafe() //대화를 끝내고 카페로 복귀
    {
        switch (mainCount)
        {
            case 0://제제의 튜토리얼 설명이 끝난 상황
                SmallFade.instance.SetCharacter(0); //제제 작은 캐릭터 설정
                DownTextbox(); //대화창 내려가고 캐릭터도 아웃
                CantClickUI(); //그 후 바로 도리가 등장할 것이므로 설정 제외한 버튼들 모두 터치 불가
                SmallFade.instance.Invoke("FadeIn",1f); //제제 작은 캐릭터 페이드인
                Invoke("BearStart", 2f); //도리 등장
                Dialogue1.instance.CharacterNum = 1; //다음 캐릭터 곰
                Dialogue1.instance.CharacterDC[0]++; //제제 다이얼로그 카운트 증가
                mainCount++; //메인 카운트 증가
                break;
            case 1:
                DownTextbox(); //도리 카페 첫 방문 후
                Popup.instance.SetPopupCharacter(BigCharacter[1]); //새 캐릭터 도리 팝업 세팅
                Popup.instance.OpenPopup(); //팝업 등장
                Dialogue1.instance.CharacterNum = 0; //다음 캐릭터 제제(튜토리얼)
                Dialogue1.instance.CharacterDC[1]++; //도리 다이얼로그 카운트 증가
                mainCount++; //메인 카운트 증가
                break;
            case 2:
                DownTextbox(); //서빙 튜토 끝남                   
                Menu.instance.Invoke("ReactionFadeIn", 1f);
                UI_Assistant1.instance.panel7.SetActive(false);               
                SmallFade.instance.FadeIn();
                Dialogue1.instance.CharacterNum = 2; //다음 등장 캐릭터 붕붕
                Dialogue1.instance.CharacterDC[0]++; //제제 다이얼로그 카운트 증가                              
                MenuHint.instance.tuto = false;
                Menu.instance.close.GetComponent<Button>().interactable = true; //메뉴 닫기 버튼 가능
                jejeOn = false;
                mainCount++; //메인 카운트 증가
                Invoke("CarStart", 7f);
                break;
            case 3: //붕붕이 등장 후
                DownTextbox();
                SmallFade.instance.Invoke("FadeIn", 1f);
                CanClickUI();//메뉴판,노트, 체력 충전 버튼 터치 가능
                BgmManager.instance.BGMFadeOut();
                BgmManager.instance.Invoke("PlayCafeBgm",3f);
                Popup.instance.SetPopupCharacter(BigCharacter[2]); //변경되는 것
                Popup.instance.OpenPopup();
                CharacterAppear.instance.SetNextAppearNum(3); //다음 등장은 빵빵
                Dialogue1.instance.CharacterDC[2]++; //변경되는 것
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
        Dialogue1.instance.SaveCharacterDCInfo();
        VisitorNote.instance.SaveVisitorNoteInfo();
    }

    void AfterTalking(int n)
    {       
        DownTextbox();
        BgmManager.instance.BGMFadeOut();
        BgmManager.instance.Invoke("PlayCafeBgm", 3f);
        if (n != 9 && n != 15)
        {
            Popup.instance.SetPopupCharacter(BigCharacter[n - 1]); //새 손님 팝업           
        }
        else if (n == 9)
        {
            Popup.instance.SetPopupCharacter(BigCharacter[15]);//샌디 이미지2
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

    void AppearScriptActive()
    {
        gameObject.GetComponent<CharacterAppear>().enabled = true;
    }

    void CanClickUI()
    {
        menu.GetComponent<Button>().interactable = true;
        note.GetComponent<Button>().interactable = true;
        plusHP.GetComponent<Button>().interactable = true;
    }

    public void CantClickUI()
    {
        menu.GetComponent<Button>().interactable = false;
        note.GetComponent<Button>().interactable = false;
        plusHP.GetComponent<Button>().interactable = false;
    }

    public void BearStart() //튜토리얼 도리 등장
    {
        gameObject.GetComponent<CharacterMove>().enabled = true;
        BigCharacter[0].SetActive(false);
        textnum = 0; //캐릭터 대사
        BigCharacter[1].SetActive(true);
        CharacterMove.instance.SetCharacter(BigCharacter[1]); // 큰 캐릭터 도리로 설정
        CharacterIn(); //캐릭터 등장
    }

    public void ServingTutorial() //서빙 튜토리얼
    {
        gameObject.GetComponent<CharacterMove>().enabled = true;
        BigCharacter[1].SetActive(false);
        jejeOn = true;
        textnum = 0;
        SmallFade.instance.FadeOut(); //작은 제제 페이드아웃
        BigCharacter[0].SetActive(true);
        CharacterMove.instance.SetCharacter(BigCharacter[0]); //큰 제제 등장
        CharacterIn();
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
        Dialogue1.instance.CharacterNum = n;
        BigCharacter[n].SetActive(true);
        gameObject.GetComponent<CharacterMove>().enabled = true;
        CharacterMove.instance.SetCharacter(BigCharacter[n]);
        CharacterIn();
    }

    public void CarStart() //붕붕 등장
    {
        servingTutorial.SetActive(false);
        gameObject.GetComponent<CharacterMove>().enabled = true;
        BigCharacter[0].SetActive(false);
        SmallFade.instance.tutorialBear.SetActive(false);//튜토리얼 오브젝트 삭제
        MenuHint.instance.tutorialBubble.SetActive(false);
        textnum = 0;
        BigCharacter[2].SetActive(true);
        CharacterMove.instance.SetCharacter(BigCharacter[2]);
        BgmManager.instance.StopBgm();
        BgmManager.instance.PlayCharacterBGM(2);//붕붕 테마 재생
        CharacterIn();
        MenuHint.instance.tuto = false;
    }
    
    public void GrandfatherStart() //롤렝드 등장
    {
        if (BgmManager.instance.IsInvoking("PlayCafeBgm"))//카페 배경음이 invoke중이면
        {
            BgmManager.instance.CancelInvoke("PlayCafeBgm");//invoke 취소
            Debug.Log("카페브금 인보크 종료");
        }
        gameObject.GetComponent<CharacterMove>().enabled = true;
        BigCharacter[14].SetActive(true);
        CharacterMove.instance.SetCharacter(BigCharacter[14]);
        Dialogue1.instance.CharacterNum = 14;
        textnum = 1; //할아버지 캐릭터만 특수하게 이렇게 시작
        CharacterMove.instance.OutCount();
        panel.SetActive(true);
        UpTextBox();
        BgmManager.instance.BGMFadeOut();
    }

    int c = 0;
    void InActiveBigCharacter()//큰 캐릭터 비활성화하는 함수
    {
        BigCharacter[c].SetActive(false);
        gameObject.GetComponent<CharacterMove>().enabled = false;
        Invoke("AppearScriptActive", 2f);//캐릭터 등장 스크립트 활성화
        if(f != 0)//표정 비활성화해야할 것이 있으면
        {
            InActiveFace();
        }
    }

    public void BackToCafe2(int n)//이벤트 대화 종료 후, n은 Dialogue1의 캐릭터 넘버
    {
        if(endStory == 1)//엔딩이벤트 본 후
        {
            BgmManager.instance.BGMFadeOut();
            DownTextbox();
            Dialogue1.instance.CharacterDC[n] = 3;
            HPManager.instance.SaveHPInfo();
            TimeManager.instance.SaveAppQuitTime();
            Dialogue1.instance.SaveCharacterDCInfo();
            Menu.instance.SaveUnlockedMenuItemInfo();
            endStory = 2;//엔딩이벤트를 봤음
            SaveDataInfo();//데이터 저장
            VisitorNote.instance.SaveVisitorNoteInfo();           
            if (Star.instance.IsInvoking("ActivateStarSystem"))
            {
                Star.instance.CancelInvoke("ActivateStarSystem");//별 활성화 함수 중단
                if (Star.instance.starCoroutine != null)
                {
                    Star.instance.StopCoroutine(Star.instance.starCoroutine);
                }
                //Debug.Log("스타 인보크 중 종료1");
            }
            else
            {
                if (Star.instance.starCoroutine != null)
                {
                    Star.instance.StopCoroutine(Star.instance.starCoroutine);
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
                    Dialogue1.instance.CharacterDC[n] = 3; // 3이면 더 이상 시나리오 없음
                    VisitorNote.instance.RePlayButton[n - 1].gameObject.SetActive(true);//다시보기 버튼 활성화
                    c = n;
                    Invoke("InActiveBigCharacter", 1f);
                    if(n == 1)//도리는 손님노트 이미지를 2번째 표정으로 바꿈
                    {
                        VisitorNote.instance.characterInfo[n - 1].GetComponent<Image>().sprite = CharacterList.instance.CharacterFaceList[n].face[1].GetComponent<Image>().sprite;
                        TipBubbleOn();
                    }
                    else if(n == 12 || n == 13)//히로디노, 닥터펭은 1번째 표정으로 바꾸기
                    {
                        VisitorNote.instance.characterInfo[n - 1].GetComponent<Image>().sprite = CharacterList.instance.CharacterFaceList[n - 2].face[0].GetComponent<Image>().sprite;
                    }
                    else if(n == 14)//롤렝드는 따로
                    {
                        VisitorNote.instance.characterInfo[n - 1].GetComponent<Image>().sprite = BigCharacter[17].GetComponent<Image>().sprite;
                    }
                    AllScenarioEnd();
                    break;
                case 10:
                    f = 10;
                    if (Dialogue1.instance.CharacterDC[n] == 1)//찰스1 이벤트
                    {
                        MenuHint.instance.CanClickMHB();//메뉴힌트버블 터치 가능
                        BgmManager.instance.BGMFadeOut();
                        BgmManager.instance.Invoke("PlayCafeBgm", 3f);
                        Dialogue1.instance.CharacterDC[10]++;
                        SmallFade.instance.CanClickCharacter(6);//도로시 클릭 가능하게
                        Menu.instance.menuFOut.Enqueue(Menu.instance.tmpNum); //메뉴 페이드아웃 큐에 추가
                        Menu.instance.MenuFadeOut();//메뉴 페이드아웃
                        c = n;
                        Invoke("InActiveBigCharacter", 1f);
                    }
                    else if (Dialogue1.instance.CharacterDC[n] == 2)//찰스2 이벤트
                    {
                        MenuHint.instance.CanClickMHB();//메뉴힌트버블 터치 가능
                        BgmManager.instance.BGMFadeOut();
                        BgmManager.instance.Invoke("PlayCafeBgm", 3f);
                        Dialogue1.instance.CharacterDC[10] = 3;
                        UI_Assistant1.instance.getMenu = 0;
                        SmallFade.instance.CanClickCharacter(6);//도로시 클릭 가능하게
                        SmallFade.instance.CanClickCharacter(10);//찰스 클릭 가능하게
                        BigCharacter[10].SetActive(false);
                        c = 6;
                        Invoke("InActiveBigCharacter", 0.5f);
                        VisitorNote.instance.RePlayButton[n - 1].gameObject.SetActive(true);         
                    }
                    AllScenarioEnd();
                    break;
                case 11:
                    if (Dialogue1.instance.CharacterDC[n] == 1)//무명이1 이벤트
                    {
                        f = 11;
                        Dialogue1.instance.CharacterDC[11]++;
                    }
                    else if (Dialogue1.instance.CharacterDC[n] == 2)//무명이2 이벤트
                    {
                        f = 12;
                        Dialogue1.instance.CharacterDC[11] = 3;
                        VisitorNote.instance.characterInfo[n - 1].GetComponent<Image>().sprite = CharacterList.instance.CharacterFaceList[n - 2].face[3].GetComponent<Image>().sprite;
                        VisitorNote.instance.RePlayButton[n - 1].gameObject.SetActive(true);
                    }
                    MenuHint.instance.CanClickMHB();//메뉴힌트버블 터치 가능
                    BgmManager.instance.BGMFadeOut();
                    BgmManager.instance.Invoke("PlayCafeBgm", 3f);
                    Menu.instance.ReactionFadeIn();
                    c = n;
                    Invoke("InActiveBigCharacter", 1f);
                    AllScenarioEnd();
                    break;

            }
            if (!CharacterVisit.instance.IsInvoking("RandomVisit"))
            {
                CharacterVisit.instance.Invoke("RandomVisit", 12f); //캐릭터 랜덤 방문
               // Debug.Log("랜덤방문 10초 뒤");
            }
        }
        Dialogue1.instance.SaveCharacterDCInfo();
        VisitorNote.instance.SaveVisitorNoteInfo();
    }

    int f = 0;//표정 비활성화에 쓰임
    void InActiveFace()
    {
        if (f == 1)//도리
        {
            CharacterList.instance.CharacterFaceList[1].face[1].SetActive(false);//표정 비활성화
        }
        else if (f == 10)//찰스1,2
        {
            CharacterList.instance.CharacterFaceList[8].face[2].SetActive(false);
        }
        else if (f == 11 || f == 7)//무명1, 루루
        {
            CharacterList.instance.CharacterFaceList[f - 2].face[1].SetActive(false);//표정 비활성화
        }
        else if (f == 12)//무명2
        {
            CharacterList.instance.CharacterFaceList[9].face[3].SetActive(false);
        }
        else if (f == 13 || f == 9)//닥터펭, 친구 이벤트의 경우
        {
            CharacterList.instance.CharacterFaceList[f - 2].face[0].SetActive(false);
        }
    }

    int sum = 0;//합계

    public void AllScenarioEnd()//모든 시나리오를 봤는지 확인
    {
        sum = 0;
        for(int i = 1; i <=14; i++)
        {
            sum += Dialogue1.instance.CharacterDC[i];
        }

        if(sum == 42 && Menu.instance.menu8Open)//캐릭터들 시나리오를 모두 봤으면
        {
            Debug.Log("시나리오 다 끝남");
            Invoke("EndingEvent",8f);
            endStory = 1;
            SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = false;//제제 클릭 불가
            //엔딩이벤트 8초 후 시작
        }
        
        if(sum > 20 && tipNum == 2)
        {
            Invoke("TipBubbleOn", 1.5f);
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

    public void ClickHPButton()
    {
        SEManager.instance.PlayUIClickSound();
        plusHP.interactable = false;
        AdsMessage.SetActive(true);
        Debug.Log("버튼 누름");

    }

    public void ClickNoAds()
    {
        SEManager.instance.PlayUICloseSound();
        plusHP.interactable = true;
        AdsMessage.SetActive(false);
    }

    public void AfterRePlaySenario()//시나리오 다시보기를 마친 후 실행
    {
        VisitorNote.instance.replayOn = 2;
        VisitorNote.instance.ClickVNButton(); //노트 올라오기
        DownTextbox();//대화상자 내리고 캐릭터 아웃
        BgmManager.instance.BGMFadeOut();
        BgmManager.instance.Invoke("PlayCafeBgm", 3f);
        if (VisitorNote.instance.fmRP != 0)//첫 만남 다시보기 이후면
        {            
            Dialogue1.instance.CharacterDC[VisitorNote.instance.fmRP] = 3;// 원래 값으로 돌려놓기
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
                Dialogue1.instance.CharacterDC[VisitorNote.instance.evRP] = 3;// 원래 값으로 돌려놓기
            }
            else if(VisitorNote.instance.evRP == 11)//찰스2 이밴트
            {
                f = 10;
                BigCharacter[10].SetActive(false);
                c = 6;
                Invoke("InActiveBigCharacter", 0.5f);
                Dialogue1.instance.CharacterDC[10] = 3;// 원래 값으로 돌려놓기
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
                Dialogue1.instance.CharacterDC[11] = 3;// 원래 값으로 돌려놓기
            }
            else if(VisitorNote.instance.evRP >= 14)//14이상이면
            {
                if (VisitorNote.instance.evRP == 15)//닥터펭 이벤트
                {
                    f = 13;
                }
                c = VisitorNote.instance.evRP - 2;
                Invoke("InActiveBigCharacter", 0.7f);
                Dialogue1.instance.CharacterDC[c] = 3;// 원래 값으로 돌려놓기
            }
            
            VisitorNote.instance.evRP = 0;
        }
        Invoke("EndRePlay", 1.1f);
    }

    void EndRePlay()//다시보기가 완전히 끝났음
    {
        VisitorNote.instance.replayOn = 0;
    }

   

    public void ClickFishBread()//붕어빵 버튼 눌렀을 때
    {
        SEManager.instance.PlayUIClickSound();
        PurchasingWindow.SetActive(true);
    }

    int price = 1000;

    public void HPMax20()
    {
        SEManager.instance.PlayUIClickSound3();
        pNum = 1;
        PlayerPrefs.SetInt("pNum", pNum); //인앱 결제 정보 저장
        PlayerPrefs.Save(); //세이브
        HPManager.instance.MAX_HP = 20;
        if(HPManager.instance.m_HPAmount == 10)//구매 전 최대체력이었을 
        {
            HPManager.instance.P_Max();

        }
        print("최대 체력20으로 증가");
        Invoke("PurchasingSuccess", 0.1f);
    }

    void PurchasingSuccess()
    {
        PurchasingWindow.SetActive(false);
        AdsMessage.SetActive(false);
        CompletePurchasingWindow.SetActive(true);
        plusHP.interactable = true;
        if (pNum == 1)
        {
            PurchasingOneTime();
        }
        else if(pNum == 2)
        {
            PurchasingTwoTime();
        }
    }

    public void PurchasingOneTime()//첫번째 결제 후 
    {
        pButton1.SetActive(false);
        pButton2.SetActive(true);//다음 인앱 결제 내용으로 바꾸기
        FishBreadText.text = "체력 회복 속도 2배";
        NumberFormatInfo numberFormat = new CultureInfo("ko-KR", false).NumberFormat;
        PurchasingText.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);
        PurchasingText.text = "체력 회복 속도를\n2배로 증가시킵니다.\n[5분에 1체력 회복]\n" + price.ToString("c", numberFormat) + "(유료)";        
    }

    public void HPSpeedUp()
    {
        SEManager.instance.PlayUIClickSound3();
        pNum = 2;
        PlayerPrefs.SetInt("pNum", pNum); //인앱 결제 정보 저장
        PlayerPrefs.Save(); //세이브
        HPManager.instance.HPRechargeIntervalMin = 5; //5분에 1체력 회복
        if(HPManager.instance.m_RechargeRemainMin >= 5)//현재 남은 타이머가 5분 이상이면 5분으로 만들기
        {
            HPManager.instance.m_RechargeRemainMin = 4;
            HPManager.instance.m_RechargeRemainSec = 59;
            HPManager.instance.HPRechargeMinTimer.text = string.Format("{0}", HPManager.instance.m_RechargeRemainMin); //타이머 표시
            HPManager.instance.HPRechargeSecTimer.text = string.Format("{0}", HPManager.instance.m_RechargeRemainSec);
        }
        print("체력 회복 속도 2배 증가");
        Invoke("PurchasingSuccess", 0.1f);
        littleFishBreadText.text = "배고픈 개발자들이 붕어빵을\n또 먹을 수 있게 되었습니다!";
        CompletePurchasingText.text = "체력의 회복 속도가\n2배 증가하였습니다!";
    }

    public void PurchasingTwoTime()//두 번째 결제 후
    {
        FishBreadButton.interactable = false;
        FishBreadButton.image.sprite = FishBread2.sprite;
        FishBreadText.gameObject.SetActive(false);
    }

    public void PurchasingFailed()
    {
        print("결제 실패");
        plusHP.interactable = true;
    }

    public void NoPurchasing()//구매 취소
    {
        SEManager.instance.PlayUICloseSound();
        PurchasingWindow.SetActive(false);
        plusHP.interactable = true;
    }

    public void CloseCompletePurchasing()//결제 완료 메세지창 닫기
    {
        SEManager.instance.PlayUICloseSound();
        CompletePurchasingWindow.SetActive(false);
    }


    bool first = true;
    int m = 6;
    public void JejeTextMessage()//제제 터치 시
    {
        if (JejeBubble.gameObject.activeSelf == false && Tip.gameObject.activeSelf == false)//말풍선 오브젝트가 꺼져있을 때만
        {          
            if (first)//게임을 켜고 난 후 처음 대사
            {
                JejeMessage(0);
                first = false;
            }
            else
            {
                if(mainCount > 11 && mainCount <= 13)
                {
                    m = 10;
                }
                else if(mainCount >= 14)
                {
                    m = 14;
                }
                if (tipNum != 0)
                {
                    m = 15;
                }
                if (Dialogue1.instance.CharacterDC[11] == 3)
                {
                    m = 16;
                }
                int n = Random.Range(1, m);//랜덤으로 대사 출력
                JejeMessage(n);
            }
        }
    }

    void JejeBubbleFadeOut()
    {
        jejeBubbleAnimator.SetTrigger("JejeBubbleOut");
        Invoke("JBubbleInActive", 1f);
    }

    void JBubbleInActive()
    {
        JejeBubble.gameObject.SetActive(false);
    }

    void CheckTip()//팁을 볼 수 있었는데 보지 않은 경우 다시팁 말풍선 나타나게 해줌
    {
        if((mainCount > 6 && tipNum == 0) || (Dialogue1.instance.CharacterDC[1] == 3 && tipNum == 1) || (Menu.instance.menu8Open == true && tipNum == 3 && Star.instance.starNum >= 5))
        {//또롱이 등장 이후&&팁 넘버 0일 때, 도리 친밀도 이벤트 후&&팁 넘버 1일 때, 메뉴8오픈&&팁넘버3&&별이 5개 이상일 때
            TipBubbleOn();
        }
        if(tipNum != 0)
        {
            TipNoteButton.gameObject.SetActive(true);
        }
        if(tipNum == 2)
        {
            Tip2.gameObject.SetActive(true);
        }
        if(tipNum == 3)
        {
            Tip2.gameObject.SetActive(true);
            Tip3.gameObject.SetActive(true);
        }
    }

    public void TipBubbleOn()//느낌표 말풍선이 나타남
    {
        if(!JejeBubble.gameObject.activeSelf)
        {
            change = 1;
            Tip.gameObject.SetActive(true);
        }
        else
        {
            change = 0;
        }
    }

    public void TipButton()//느낌표 말풍선 터치 시
    {
        Tip.gameObject.SetActive(false);
        if(tipNum != 3)//마지막 팁만 아니면
        {
            TipMessage.gameObject.SetActive(true);
        }
        else if(tipNum == 3)//별하트 전환 팁이면
        {
            Tip.gameObject.SetActive(false);
            JejeMessage(tipNum + 20);
        }
    }

    public int tipNum = 0;
    public void ShowTip()//별 소모 팁 메세지에서 네 를 터치했을 때
    {
        SEManager.instance.PlayUIClickSound(); //효과음           
        TipMessage.gameObject.SetActive(false);
        if(Star.instance.starNum >= 3)//
        {
            Tip.gameObject.SetActive(false);
            Star.instance.starNum -= 3;
            Star.instance.StarNumText.text = string.Format("{0}", Star.instance.starNum.ToString());
            JejeMessage(tipNum + 20);
        }
        else//스타가 부족하면
        {
            StarLackMessage.gameObject.SetActive(true);
            starLackAnimator.SetTrigger("FadeStarLack");
            Invoke("CloseStarLack", 1.5f);
        }
    }

    void CloseStarLack()//스타 부족 메세지 비활성화
    {
        StarLackMessage.gameObject.SetActive(false);
        Tip.gameObject.SetActive(false);//팁 말풍선도
    }

    public void NoTip()
    {
        SEManager.instance.PlayUIClickSound(); //효과음           
        TipMessage.gameObject.SetActive(false);
    }

    public void ClickTipNoteButton()//팁노트 버튼 눌렀을 때
    {
        TipNoteButton.interactable = false;
        SEManager.instance.PlayUIClickSound(); //효과음  
        TNBAnimator.SetTrigger("TipButton_Up");
        TipNoteAnimator.SetTrigger("TipNote_Up");
    }

    public void CloseTipNote()
    {
        TipNoteButton.interactable = true;
        SEManager.instance.PlayUICloseSound(); //효과음
        TNBAnimator.SetTrigger("TipButton_Down");
        TipNoteAnimator.SetTrigger("TipNote_Down");
    }

    void JejeMessage(int n)
    {
        JejeBubble.gameObject.SetActive(true);
        switch (n)
        {
            case 0:
                JejeText.text = "어서 와!";
                break;
            case 1:
                JejeText.text = "밥은 먹었어?";
                break;
            case 2:
                JejeText.text = "네가 하는 일이\n곧 정답이야.";
                break;
            case 3:
                JejeText.text = "내일은 또 어떤 일이 생길까?";
                break;
            case 4:
                JejeText.text = "샘이 숨겨져 있지 않은\n사막이어도 아름다울 순\n없을까?";
                break;
            case 5:
                JejeText.text = "우린 모두 빛나고 있어.";
                break;
            case 6://여기까지 기본 대사
                JejeText.text = "네가 웃으면 나도 웃게 돼.";
                break;
            case 7: //1차 해제
                JejeText.text = "내가 너의 친구인 게\n항상 자랑스러워.";
                break;
            case 8:
                JejeText.text = "내가 어디든 갈 수 있다면\n난 너와 함께 갈래.";
                break;
            case 9:
                JejeText.text = "난 항상 네 곁에 있어.";
                break;
            case 10:
                JejeText.text = "진심이 버거울 땐\n가면을 써도 괜찮아.";
                break;
            case 11://2차 해제
                JejeText.text = "너무 급하게 달리진 마.\n그럴 수록 중요한 걸\n놓칠 뿐이야.";
                break;
            case 12:
                JejeText.text = "난 항상 네 곁에 있어.";
                break;
            case 13:
                JejeText.text = "널 믿어.";
                break;
            case 14:
                JejeText.text = "네 인생의 주인공은\n너라는 걸 잊지 마.";
                break;
            case 15:
                JejeText.text = "들었던 팁은 메뉴판 왼쪽의\n느낌표 노트를 터치하면\n다시 볼 수 있어.";
                break;
            case 16:
                JejeText.text = UI_Assistant1.instance.namedName + "의 얼굴이\n좀 밝아진 것 같아.";
                break;
            case 20://여기서부터는 게임 팁
                JejeText.text = "손님이 원하는 메뉴를\n맞추면 3, 맞추지 못하면\n1씩 평판이 증가해!";
                TipNoteButton.gameObject.SetActive(true);
                break;
            case 21:
                JejeText.text = "손님들과의 추억과\n특별 메뉴는 손님노트에서\n다시 볼 수 있어!";
                Tip2.gameObject.SetActive(true);
                break;
            case 22:
                JejeText.text = "게임 화면을 유지하고\n있으면 25~30초 간격으로\n별이 나타나!";
                Tip3.gameObject.SetActive(true);
                break;
            case 23:
                JejeText.text = "모은 별을 체력으로 바꿔줄까?";
                JejeBubble.GetComponent<Button>().interactable = true; //말풍선 터치 가능
                break;         
        }
        if(n >= 20 && n < 23)
        {
            tipNum++;//3이 마지막
            PlayerPrefs.SetInt("tipNum", tipNum);
            PlayerPrefs.Save();
        }
        jejeBubbleAnimator.SetTrigger("JejeBubbleIn");
        if(n != 23)//별 체력 전환 메세지만 아니면 말풍선 저절로 사라지기
        {
            Invoke("JejeBubbleFadeOut", 2.5f);
        }
    }

    public GameObject changeMessage;

    public void ClickLastTip()//마지막 팁, 제제 말풍선 터치 시
    {
        JejeBubble.GetComponent<Button>().interactable = false;
        changeMessage.SetActive(true);
        Invoke("JejeBubbleFadeOut", 0.5f);
    }

    public bool changing = false; //별-하트 간 전환 상태
    public void Change_Yes()
    {
        SEManager.instance.PlayPopupSound();
        changing = true;
        changeMessage.SetActive(false);
        Star.instance.starNum -= 5;//스타 5감소
        Star.instance.StarNumText.text = string.Format("{0}", Star.instance.starNum.ToString());
        HPManager.instance.AddHP(); //체력 증가함수
        Invoke(nameof(ChangeAgain), 3f);//별이 5개 이상 남았다면 3초 뒤 다시 팁말풍선 뜰 것        
    } //별하트 전환하기

    void ChangeAgain()
    {
        change = 0;
    }

    public void Change_No() //충분한 별이 있는데 전환하지 않음
    {
        SEManager.instance.PlayUICloseSound();
        changeMessage.SetActive(false);
        change = 2;
    }

    public int change = 0; // 2는 메세지가 떴으나 바꾸지 않은 상태
    void Update()
    {
        if(!Tip.gameObject.activeSelf && !JejeBubble.gameObject.activeSelf)//팁말풍선이 나타나있지 않을 때
        {
            if (change == 0 && tipNum == 3 && Menu.instance.menu8Open) //마지막 팁의 경우
            {
                if (Star.instance.starNum >= 5 && HPManager.instance.m_HPAmount != HPManager.instance.MAX_HP)
                {
                    change = 3;
                    Invoke(nameof(TipBubbleOn), 2f);//별이 다시 5개 이상이 되었을 때 팁말풍선 등장
                }

            }
            if (change == 2)//전환하지 않았으면
            {
                change = 3;
                Invoke(nameof(TipBubbleOn), 60f); //60초 뒤 다시 팁말풍선 등장
            }
        }  
        else if(Tip.gameObject.activeSelf && tipNum == 3 && Menu.instance.menu8Open)//팁풍선 올라와있고 팁넘버 3일 때
        {
            if(HPManager.instance.m_HPAmount == HPManager.instance.MAX_HP)//현재 체력이 최대치면 팁풍선 없애기
            {
                Tip.gameObject.SetActive(false);
            }
        }
    }
}
