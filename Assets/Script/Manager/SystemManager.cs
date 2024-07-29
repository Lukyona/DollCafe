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
    public Button noteButton;
    public Button plusHPButton;

    bool completeSave = false;

    bool canTouch = true;

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
    public string inputName; //입력한 이름
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

        LoadDataInfo();//저장된 데이터 불러옴
    
        if (mainCount == 0)
        {
            GameScript1.instance.servingTutorial.SetActive(true);
            CharacterManager.instance.CharacterIn(0);
        }   
        if (mainCount > 3)//붕붕이 등장 이후면
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
                if (gameClosingWindow.activeSelf == false)
                {
                    if (CanTouch())
                    {
                        SetCanTouch(false);
                    }
                    OnApplicationFocus(false);
                    if(mainCount <= 3)
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
            if (tipNum == 3 && Menu.instance.menu8Open) // 팁 다 봄 + 마지막 메뉴 잠금 해제 상태
            {
                if (Star.instance.GetCurrentStarNum() >= 5 && HPManager.instance.GetCurrentHP() != HPManager.instance.GetMaxHP()) // 별 5개 이상 + 최대 체력 아닐 때
                {
                    ShowBangBubble();
                }
            }
        }  
        else if(bangBubble.gameObject.activeSelf && tipNum == 3 && Menu.instance.menu8Open) //느낌표 말풍선 올라와있고 팁넘버 3 + 마지막 메뉴 잠금 해제 완료
        {
            if(HPManager.instance.GetCurrentHP() == HPManager.instance.GetMaxHP()) //현재 체력이 최대치면 느낌표 말풍선 없애기, 체력으로 교환할 필요가 없기 때문
            {
                bangBubble.gameObject.SetActive(false);
            }
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
            PlayerPrefs.SetInt("Reputation", Menu.instance.reputation); //평판 저장
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
            if (PlayerPrefs.HasKey("mainCount"))
            {
                mainCount = PlayerPrefs.GetInt("mainCount");                
                CharacterAppear.instance.SetNextAppearNum(PlayerPrefs.GetInt("NextAppear"));
                Menu.instance.reputation = PlayerPrefs.GetInt("Reputation");
                Star.instance.SetStarNum(PlayerPrefs.GetInt("StarNum"));
                endingState = PlayerPrefs.GetInt("EndingState");       
                tipNum = PlayerPrefs.GetInt("TipNum");
                if (mainCount > 3)//붕붕이 등장 이후면
                {
                   // Debug.Log("중요 데이터 로드");
                    SmallFade.instance.SetCharacter(0);
                    SmallFade.instance.Invoke("FadeIn", 1f); //제제 작은 캐릭터 페이드인                   
                    Dialogue.instance.LoadCharacterDCInfo();
                    VisitorNote.instance.LoadVisitorNoteInfo();
                    Menu.instance.LoadUnlockedMenuItemInfo();                 
                    Dialogue.instance.SetBabyName(PlayerPrefs.GetString("BabyName"));
                    SetTipState();
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

    #region 대화 관련 함수
    public void BeginDialogue(int cNum)
    {       
        CharacterManager.instance.CharacterIn(cNum); 
        panel.SetActive(true); //캐릭터가 들어옴과 동시에 회색 패널 작동
        UpTextBox(); // 대화창 등장
    }

    public void EndDialogue(int cNum) //캐릭터가 화면 밖으로 나가도록
    {
        CharacterManager.instance.CharacterOut(cNum); //자동으로 나감
        panel.SetActive(false); //회색 패널 해제
    }

    void UpTextBox()
    {
        if(Dialogue.instance.IsBabayText()) // 주인공 대사면
        {
            babyTextBox.SetActive(true);
        }
        else //baby text
        {
            characterTextBox.SetActive(true);
        }

        TBAnimator.SetTrigger("TextBoxUp");
        UI_Assistant1.instance.OpenDialogue(); //대화 시작, 대사 띄움
    }

    void DownTextbox() //대화창 내려감
    {
        TBAnimator.SetTrigger("TextBoxDown");       
        CharacterManager.instance.CharacterOut(); 
    }

    public void ChageToBabyTB() // 캐릭터 대사창에서 주인공 대사창으로 전환
    {
        characterTextBox.SetActive(false);
        babyTextBox.SetActive(true);
    }

    public void ChageToCharacterTB() // 주인공 대사창에서 캐릭터 대사창으로 전환
    {
        babyTextBox.SetActive(false);
        characterTextBox.SetActive(true);   
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

        Invoke(nameof(CheckTipState), 30f);
    }

    public void TouchTipNoteButton() // 팁 노트 버튼 눌렀을 때
    {
        tipNoteButton.interactable = false;
        SEManager.instance.PlayUIClickSound(); //효과음  
        tipNoteButtonAnimator.SetTrigger("TipButton_Up");
        tipNoteAnimator.SetTrigger("TipNote_Up");
    }

    public void CloseTipNote() // 팁 노트 닫기
    {
        tipNoteButton.interactable = true;
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
        if((mainCount > 6 && tipNum == 0) || (Dialogue.instance.CharacterDC[1] == 3 && tipNum >= 1)) //또롱이 등장 이후 & 팁 넘버 0 or 도리 친밀도 이벤트 후 & 팁 넘버 1이상
        {   
            ShowBangBubble();
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

        purchasingWindow.SetActive(false);
        AdsManager.instance.SetAdsMessageWindowActive(false);
        completePurchasingWindow.SetActive(true);
        plusHPButton.interactable = true;

        UpdatePurchasingState(pCount);
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
            fishBreadButton.interactable = false;
            fishBreadButton.image.sprite = fishBreadButton.transform.GetChild(1).GetComponent<Image>().sprite;
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

    public void CanTouchUI() // 버튼들 터치 가능, 설정 버튼 제외
    {
        menuButton.GetComponent<Button>().interactable = true;
        noteButton.GetComponent<Button>().interactable = true;
        plusHPButton.GetComponent<Button>().interactable = true;
    }

    public void CantTouchUI() // 버튼들 터치 불가능
    {
        menuButton.GetComponent<Button>().interactable = false;
        noteButton.GetComponent<Button>().interactable = false;
        plusHPButton.GetComponent<Button>().interactable = false;
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
