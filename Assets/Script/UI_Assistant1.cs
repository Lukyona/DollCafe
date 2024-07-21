using UnityEngine;
using UnityEngine.UI;

public class UI_Assistant1 : MonoBehaviour //대사창 관련
{
    public static UI_Assistant1 instance;

    private TextWriter.TextWriterSingle textWriterSingle;
    public int count = 0; //대사 카운트
    public Text CName; //캐릭터 이름
    public GameObject panel; //캐릭터 뒤의 나타날 회색 패널
    public GameObject panel1; //튜토용 패널, 메뉴판
    public GameObject panel2; //손님노트 
    public GameObject panel3; //체력
    public GameObject panel4; //별
    public GameObject panel5; //서빙 튜토
    public GameObject panel6;
    public GameObject panel7;
    public GameObject panel8; //평판

    public Text characterText; //캐릭터 대사
    public Text babyText; //아기 대사

    public Animator textBoxAnimator;

    public int stop = 0; // 특정한 터치가 필요한 경우에는 1, 대사가 막무가내로 넘어가는 것을 방지하기 위함

    public string namedName; //플레이어가 붙여준 무명이의 이름
    public GameObject NameSetting;
    public GameObject BabyNameSetting;

    public bool talking = false; // true면 대화중

    public int getMenu = 0; //1이면 주인공 아기가 스페셜 메뉴를 가져오는 중, 친밀도 이벤트 시나리오 중 아기 페이드인에 사용

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void BabyNameSet()
    {
        BabyNameSetting.SetActive(true);
    }

    public void OpenDialogue() //대화 시작
    {
        talking = true;
        count = 0;
        UserInputManager.instance.SetCanTouch(false);
        Dialogue1.instance.SelectedFirstDialogue(); //첫 문장 나타남
        UserInputManager.instance.Invoke("SetCanTouchTrue", 1.5f); 
    }  

    public void OpenDialogue2() //첫 대사 제외 나머지 대사
    {
        talking = true;
        if (textWriterSingle != null && textWriterSingle.IsActive())
        {
           textWriterSingle.WriteAllAndDestroy();
        }
        else
        {
            DialogueEvent(); //대화 이벤트 있으면 실행
            Dialogue1.instance.SelectedNextDialogue(); //다음 대사 가져오기
            string[] messegeArray = Dialogue1.instance.messegeArray;
            if (count < messegeArray.Length) //대사 카운트가 대사 갯수와 같아질 때까지 반복
            {
                string messege = messegeArray[count];
                if (GameScript1.instance.textnum == 0) //캐릭터 대사일 경우
                {
                     babyText.text = "";
                     GameScript1.instance.ChageToCharacter(); //캐릭터 대사창으로 변경
                     textWriterSingle = TextWriter.AddWriter_Static(characterText, messege, .03f, true, true);
                }
                else //아기 대사일 경우
                {
                     characterText.text = "";
                     GameScript1.instance.ChageToBaby();
                     textWriterSingle = TextWriter.AddWriter_Static(babyText, messege, .03f, true, true);
                }

                count++;
            }
            else if (count == messegeArray.Length) //대사가 모두 끝났을 때
            {
                characterText.text = ""; //대사창 공백으로 만들고
                babyText.text = "";
                if(VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//첫 만남이나 이벤트 다시보기가 아닐 경우
                {
                    if (Dialogue1.instance.CharacterDC[Dialogue1.instance.CharacterNum] == 0 || Dialogue1.instance.CharacterDC[0] == 1)//캐릭터들의 첫 방문이거나 제제 튜토리얼 때는 BackToCafe() 실행
                    {
                        GameScript1.instance.BackToCafe(); //카페로 복귀
                    }
                    else //그 외에 친밀도 이벤트 대화의 경우는 BackToCafe2() 실행
                    {
                        GameScript1.instance.BackToCafe2(Dialogue1.instance.CharacterNum);
                    }
                }
                else//첫 만남 혹은 이벤트 다시보기일 때
                {
                    GameScript1.instance.AfterRePlaySenario();
                }
                
                talking = false;
            }
        }
    }

    
    private void DialogueEvent() //대화 이벤트
    {
        switch (Dialogue1.instance.CharacterNum)
        {
            case 0: //제제
                if (Dialogue1.instance.CharacterDC[0] == 0)
                {
                    switch (count)
                    {
                        case 1:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 2:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 4:
                            UserInputManager.instance.SetCanTouch(false);
                            Invoke("BabyNameSet", 1.3f);//아기 이름 설정
                            break;
                        case 5:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;
                            CName.text = "제제";
                            break;
                        case 7:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 8:
                            CharacterList.instance.CharacterFaceList[0].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[0].face[0].SetActive(false);
                            panel.SetActive(false);
                            panel1.SetActive(true);
                            break;
                        case 12:
                            panel1.SetActive(false);
                            panel2.SetActive(true);
                            break;
                        case 15:
                            panel2.SetActive(false);
                            panel3.SetActive(true);
                            break;
                        case 20:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 21:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 22:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 23:
                            GameScript1.instance.textnum = 0;
                            panel3.SetActive(false);
                            panel4.SetActive(true);
                            break;
                        case 25:
                            panel4.SetActive(false);
                            panel8.SetActive(true);
                            break;
                        case 29:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 30:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            panel8.SetActive(false);
                            panel.SetActive(true);
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[0] == 1)
                {
                    switch (count)
                    {
                        case 1:
                            SmallFade.instance.SetCharacter(99); //튜토용 도리 작은 캐릭터 설정
                            break;
                        case 2:
                            SmallFade.instance.FadeIn();
                            panel.SetActive(false);
                            panel5.SetActive(true);
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.Invoke("TutorialDownBox", 1.3f);
                            break;
                        case 3:
                            UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                            break;
                        case 5:
                            UserInputManager.instance.SetCanTouch(false);;
                            MenuHint.instance.Invoke("ActiveMH", 1f);
                            break;
                        case 7:
                            panel6.SetActive(false);
                            panel7.SetActive(true);
                            break;
                        case 8:
                            SmallFade.instance.SetCharacter(0);
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[0] == 1)
                {
                    if(count == 11)
                    {
                        SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = false;//제제 클릭 불가
                    }
                }
                break;
            case 1: // 도리
                if (Dialogue1.instance.CharacterDC[1] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 4:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 5:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 8:
                            CName.text = "도리";
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[1] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFIn.Enqueue(17);
                                SmallFade.instance.FadeIn();//주인공 아기 페이드인
                            }
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 2:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            CharacterList.instance.CharacterFaceList[1].face[0].SetActive(true);//표정 1
                            GameScript1.instance.textnum = 0;
                            break;
                        case 4:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 5:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[1].face[0].SetActive(false);//표정 기본으로
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 11:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 12:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 13:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 14:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 15:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 16:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 17:
                            CharacterList.instance.CharacterFaceList[1].face[0].SetActive(true);//표정 1
                            break;
                        case 18:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 19:
                            CharacterList.instance.CharacterFaceList[1].face[0].SetActive(false);//표정 기본으로
                            GameScript1.instance.textnum = 0;
                            break;                          
                        case 21:
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 22:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 23:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 25:
                            CharacterList.instance.CharacterFaceList[1].face[1].SetActive(true);//표정 2
                            break;
                        case 26:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(17);//아기 페이드아웃 준비
                            }                              
                            break;
                        case 27://여기서 주인공 아기 메뉴 가지러 페이드아웃
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1; //메뉴 가지러 갔음
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero()); //페이드아웃 실행        
                            }                                                  
                            break;
                        case 28:
                            CharacterList.instance.CharacterFaceList[1].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[1].face[0].SetActive(true);//표정 1
                            GameScript1.instance.textnum = 0;
                            if(VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(17);//주인공 아기 페이드인 준비
                                Menu.instance.FEventMenu(1);//스페셜 메뉴 준비
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(11, 0);
                            }
                            break;
                        case 29:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn(); //주인공 아기 페이드인 실행
                                Menu.instance.MenuFadeIn();//스페셜 메뉴 페이드인
                            }                                
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 30:
                            CharacterList.instance.CharacterFaceList[1].face[0].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2; //메뉴 갖다줬음
                            }     
                            break;
                        case 31:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 32:
                            CharacterList.instance.CharacterFaceList[1].face[1].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 33:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 34:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 35:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 36:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                VisitorNote.instance.OpenNewSentence(1);//도리 손님노트 정보 갱신
                            }
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                    }
                }
                break;

            case 2: //붕붕
                if (Dialogue1.instance.CharacterDC[2] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            break;
                        case 2:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 4:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 5:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 7:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 8:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(2);
                            }
                            break;
                        case 10:
                            CName.text = "붕붕";
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[2] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();//붕붕 페이드인
                                SmallFade.instance.smallFIn.Enqueue(16); //아기 페이드인 준비
                            }                             
                            break;
                        case 2:
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();//아기 페이드인
                            }      
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 8:
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 9:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[2].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[2].face[1].SetActive(true);
                            break;
                        case 13:
                            CharacterList.instance.CharacterFaceList[2].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[2].face[0].SetActive(true);
                            break;
                        case 14:
                            CharacterList.instance.CharacterFaceList[2].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[2].face[1].SetActive(true);
                            break;
                        case 16:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[7].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 17:
                            CharacterList.instance.CharacterFaceList[13].face[7].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 19:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 20:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 21:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 22:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 25:
                            CharacterList.instance.CharacterFaceList[2].face[1].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(16);//아기 페이드아웃 준비
                            }
                            break;
                        case 27:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1; //메뉴 가지러 갔음
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero()); //아기 페이드아웃    
                            }                                                                           
                            break;
                        case 28:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 29:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(16);//주인공 아기 페이드인 준비
                                Menu.instance.FEventMenu(2);//스페셜 메뉴 준비
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(12, 0);
                            }
                            break;
                        case 30:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn(); //주인공 아기 페이드인 실행
                                Menu.instance.MenuFadeIn();//스페셜 메뉴 페이드인
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 31:
                            GameScript1.instance.textnum = 0;
                            break;
                    }
                }
                break;

            case 3: //빵빵
                if (Dialogue1.instance.CharacterDC[3] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            CharacterList.instance.CharacterFaceList[3].face[2].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 2:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            CharacterList.instance.CharacterFaceList[3].face[2].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 5:
                            CName.text = "빵빵";
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(3);
                            }
                            break;
                        case 7:
                            GameScript1.instance.textnum = 0;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[3] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFIn.Enqueue(17);
                                SmallFade.instance.FadeIn();
                            }
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 4:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 5:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[3].face[0].SetActive(false);
                            break;
                        case 12:
                            CharacterList.instance.CharacterFaceList[3].face[0].SetActive(true);
                            break;
                        case 15:
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 16:
                            CharacterList.instance.CharacterFaceList[3].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[3].face[1].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 20:
                            CharacterList.instance.CharacterFaceList[3].face[1].SetActive(false);
                            break;
                        case 21:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 22:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 23:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 27:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 28:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 29:
                            CharacterList.instance.CharacterFaceList[3].face[2].SetActive(true);
                            break;
                        case 30:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 31:
                            CharacterList.instance.CharacterFaceList[3].face[2].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(17);
                            }
                            break;
                        case 32:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }               
                            break;
                        case 33:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 35:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(17);
                                Menu.instance.FEventMenu(3);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(13, 0);
                            }
                            break;
                        case 36:
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 37:
                            CharacterList.instance.CharacterFaceList[3].face[2].SetActive(true);
                            GameScript1.instance.textnum = 0;                            
                            break;
                        case 39:
                            CharacterList.instance.CharacterFaceList[3].face[2].SetActive(false);
                            break;
                    }
                }
                break;

            case 4: //개나리
                if (Dialogue1.instance.CharacterDC[4] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            CName.text = "개나리";
                            break;
                        case 2:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 7:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            break;
                        case 8:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(4);
                            }                          
                            break;
                        case 11:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[4] == 1)
                {
                    switch (count)
                    {
                        case 6:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(16);
                            }              
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }
                            break;
                        case 11:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(16);
                                Menu.instance.FEventMenu(4);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(14, 0);
                            }
                            break;
                        case 12:
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;                           
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 13:
                            GameScript1.instance.textnum = 0;                        
                            break;
                    }
                }
                break;

            case 5: //또롱이
                if (Dialogue1.instance.CharacterDC[5] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 2:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 4:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 5:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 0;
                            CName.text = "또롱";
                            break;
                        case 8:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;      
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 10:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(5);
                            }
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[5] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 2:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 5:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 12:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 13:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 14:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 15:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                VisitorNote.instance.OpenNewSentence(5);//손님노트 정보 갱신
                                getMenu = 2;
                            }
                            break;
                    }
                }
                break;

            case 6: //도로시
                if (Dialogue1.instance.CharacterDC[6] == 0)
                {
                    switch (count)
                    {
                        case 1:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 2:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 5:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 8:
                            CName.text = "도로시";
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 10:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(6);
                            }
                            break;
                        case 12:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[6] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFIn.Enqueue(16);
                                SmallFade.instance.FadeIn();
                            }
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 2:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 5:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(16);
                            }
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }
                            break;
                        case 9:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(16);
                                Menu.instance.FEventMenu(6);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(16, 0);
                            }
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 11:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 12:
                            CharacterList.instance.CharacterFaceList[4].face[0].SetActive(true);
                            break;
                        case 15:
                            CharacterList.instance.CharacterFaceList[4].face[0].SetActive(false);
                            break;
                        case 17:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 18:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                    }
                }
                break;

            case 7: //루루
                if (Dialogue1.instance.CharacterDC[7] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 2:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 4:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 5:
                            GameScript1.instance.textnum = 0;
                            CName.text = "루루";
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(7);
                            }
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[7] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            CharacterList.instance.CharacterFaceList[5].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 2:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 5:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 7:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 8:
                            CharacterList.instance.CharacterFaceList[5].face[0].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 9:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 11:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 20:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 21:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 28:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 29:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 36:
                            CharacterList.instance.CharacterFaceList[5].face[1].SetActive(true);
                            break;
                        case 37:
                            CharacterList.instance.CharacterFaceList[5].face[1].SetActive(false);
                            break;
                        case 38:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 39:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 41:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 42:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 52:
                            CharacterList.instance.CharacterFaceList[5].face[1].SetActive(true);
                            break;
                        case 53:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 54:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(17);
                            }
                            break;
                        case 55:
                            CharacterList.instance.CharacterFaceList[5].face[1].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }
                            break;
                        case 58:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(17);
                                Menu.instance.FEventMenu(7);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(17, 0);
                            }
                            break;
                        case 59:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(true);
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 60:
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 61:
                            CharacterList.instance.CharacterFaceList[5].face[1].SetActive(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;
                                VisitorNote.instance.OpenNewSentence(7);
                            }
                            break;
                    }
                }
                break;

                case 8: //샌디
                if (Dialogue1.instance.CharacterDC[8] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            CName.text = "샌디";
                            break;
                        case 3:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 4:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 7:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 8:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(8);
                            }
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[8] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 2:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 12:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 13:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 44:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 46:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(16);
                            }
                            break;
                        case 47:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }
                            break;
                        case 48:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(16);
                                Menu.instance.FEventMenu(8);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(18, 0);
                            }
                            break;
                        case 49:
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 50:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 51:
                            CharacterList.instance.CharacterFaceList[6].face[0].SetActive(true);
                            break;
                        case 52:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 53:
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 54:
                            CharacterList.instance.CharacterFaceList[6].face[0].SetActive(false);
                            break;
                    }
                }
                    break;

                case 9: //친구
                if (Dialogue1.instance.CharacterDC[9] == 0)
                {
                    switch (count)
                    {
                        case 1:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            CName.text = "친구";
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 10:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(9);
                            }
                            break;
                        case 12:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[9] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[7].face[0].SetActive(true);
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 10:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 14:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 16:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFOut.Enqueue(17);
                                Menu.instance.menuFOut.Enqueue(Menu.instance.tmpNum); //메뉴 페이드아웃 큐에 추가
                                Menu.instance.MenuFadeOut();//원래 있던 메뉴 페이드아웃
                            }
                            break;
                        case 17:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }
                            break;
                        case 19:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(17);
                                Menu.instance.FEventMenu(9);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(19, 0);
                            }
                            break;
                        case 20:
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 21:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 23:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 24:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;
                                VisitorNote.instance.OpenNewSentence(9);
                            }
                            break;
                    }
                }
                break;          

            case 10: //찰스
                if (Dialogue1.instance.CharacterDC[10] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            CName.text = "찰스";
                            break;
                        case 3:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 4:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(10);
                            }
                            break;
                        case 7:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 8:
                            GameScript1.instance.textnum = 0;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[10] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 3:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 4:
                            CharacterList.instance.CharacterFaceList[8].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 5:
                            CharacterList.instance.CharacterFaceList[8].face[0].SetActive(false);
                            break;
                        case 7:
                            CharacterList.instance.CharacterFaceList[8].face[1].SetActive(true);
                            break;
                        case 8:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[8].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[8].face[2].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;//아기 페이드아웃 위해서 일부러 대입
                            }
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[10] == 2)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 5:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 8:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[8].face[1].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 14:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 15:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 16:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;                           
                            break;                            
                        case 18:
                            CharacterList.instance.CharacterFaceList[8].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;                            
                            CharacterMove.instance.soldierEvent = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(16);
                            }
                            break;
                        case 19:
                            CharacterList.instance.CharacterFaceList[8].face[0].SetActive(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }
                            CharacterMove.instance.OutCount();
                            break;
                        case 20:                            
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1.3f);
                                Menu.instance.SoldierEvent(SmallFade.instance.CharacterSeat[5]);//도로시 자리정보를 매개변수로 넣기
                            }
                            break;
                        case 21:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.MenuFadeIn();
                            }
                            break;
                        case 22:
                            GameScript1.instance.BigCharacter[6].SetActive(true);
                            CharacterList.instance.CharacterFaceList[4].face[0].SetActive(true);
                            CharacterMove.instance.SetCharacter(GameScript1.instance.BigCharacter[6]);
                            CharacterMove.instance.InCount();
                            GameScript1.instance.textnum = 0;
                            CName.text = "도로시";
                            break;
                        case 23:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 24:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                Menu.instance.seatInfo.Enqueue(SmallFade.instance.CharacterSeat[9]);
                                SmallFade.instance.FadeOut();//찰스 작은 캐릭터 페이드아웃
                            }
                            break;
                        case 25:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(true);
                            UserInputManager.instance.SetCanTouch(false);;
                            UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 26:
                            UserInputManager.instance.SetCanTouch(false);;
                            UserInputManager.instance.Invoke("SetCanTouchTrue", 1.5f);
                            CharacterList.instance.CharacterFaceList[8].face[2].SetActive(true);
                            CharacterMove.instance.PrincessMove();//찰스등장을 위해 도로시가 옆으로 이동, 바로 찰스 등장
                            GameScript1.instance.textnum = 0;
                            CName.text = "찰스";
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;//찰스 페이드인을 위한 장치                           
                                SmallFade.instance.SetCharacter(10);
                            }
                            break;
                        case 27:
                            CName.text = "도로시";
                            break;
                        case 28:
                            CName.text = "찰스";
                            break;
                        case 30:
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 31:
                            CharacterList.instance.CharacterFaceList[4].face[0].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            CName.text = "도로시";
                            break;
                        case 33:
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.MenuFadeOut();
                            }
                            break;
                        case 34:
                            GameScript1.instance.textnum = 0;
                            CName.text = "찰스";
                            break;
                        case 35:
                            CName.text = "도로시";
                            break;
                        case 36:
                            CName.text = "찰스";
                            break;
                        case 39:
                            CName.text = "도로시";
                            break;
                        case 40:
                            CName.text = "찰스";
                            break;
                        case 41:
                            CName.text = "도로시";
                            break;
                        case 43:
                            CharacterList.instance.CharacterFaceList[8].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[8].face[0].SetActive(true);
                            CName.text = "찰스";
                            break;
                        case 44:
                            CName.text = "도로시";
                            break;
                        case 46:
                            CharacterList.instance.CharacterFaceList[8].face[0].SetActive(false);
                            CName.text = "찰스";
                            break;
                        case 48:
                            CName.text = "도로시";
                            break;
                        case 49:
                            CharacterList.instance.CharacterFaceList[8].face[2].SetActive(true);
                            CName.text = "찰스";
                            break;
                        case 50:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            CharacterMove.instance.OutCount();//찰스 퇴장
                            break;
                        case 51:
                            CharacterMove.instance.SetCharacter(GameScript1.instance.BigCharacter[6]);
                            CharacterMove.instance.Invoke("OutCount", 0.5f);
                            CharacterMove.instance.soldierEvent = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                VisitorNote.instance.OpenNewSentence(10);
                            }
                            break;
                        case 52:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                    }
                }
                break;

            case 11: //무명이
                if (Dialogue1.instance.CharacterDC[11] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 2:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 5:
                            CharacterList.instance.CharacterFaceList[9].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 8:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(11);
                            }
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[11] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 2:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 3:
                            CharacterList.instance.CharacterFaceList[9].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 4:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 8:
                            CharacterList.instance.CharacterFaceList[9].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[9].face[1].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 11:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 13:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 14:
                            CharacterList.instance.CharacterFaceList[9].face[1].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 18:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 19:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 20:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                NameSetting.SetActive(true);
                            }
                            break;
                        case 21:
                            CharacterList.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 22:
                            UserInputManager.instance.SetCanTouch(true);
                            GameScript1.instance.textnum = 0;
                            CName.text = namedName;
                            break;
                        case 23:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 24:
                            CharacterList.instance.CharacterFaceList[9].face[1].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;
                                VisitorNote.instance.OpenNewSentence(11);
                            }
                            break;

                    }
                }
                else if (Dialogue1.instance.CharacterDC[11] == 2)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 15:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[5].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 16:
                            CharacterList.instance.CharacterFaceList[9].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 18:
                            CharacterList.instance.CharacterFaceList[9].face[0].SetActive(false);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 19:
                            CharacterList.instance.CharacterFaceList[13].face[5].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[7].SetActive(true);
                            break;
                        case 22:
                            CharacterList.instance.CharacterFaceList[13].face[7].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 25:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 28:
                            CharacterList.instance.CharacterFaceList[9].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 29:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 30:
                            CharacterList.instance.CharacterFaceList[9].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[9].face[1].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 31:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 32:
                            CharacterList.instance.CharacterFaceList[9].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[9].face[2].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(17);
                            }
                            break;
                        case 34:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }
                            break;
                        case 35:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 36:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(17);
                                Menu.instance.FEventMenu(11);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(21, 0);
                            }
                            break;
                        case 37:
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 38:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 39:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 41:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                Menu.instance.menuFOut.Enqueue(Menu.instance.tmpNum); //메뉴 페이드아웃 큐에 추가
                                Menu.instance.MenuFadeOut();//원래 있던 메뉴 페이드아웃
                            }
                            break;
                        case 42:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 0.8f);
                                Menu.instance.FEventMenu(11);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(31, 0);
                            }
                            break;
                        case 43:
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 44:
                            CharacterList.instance.CharacterFaceList[9].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[9].face[1].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 45:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 51:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 52:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 53:
                            CharacterList.instance.CharacterFaceList[9].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[9].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 54:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 55:
                            CharacterList.instance.CharacterFaceList[9].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 59:
                            CharacterList.instance.CharacterFaceList[9].face[3].SetActive(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                VisitorNote.instance.OpenNewSentence(12);
                            }
                            break;
                    }
                }
                break;

            case 12: //히로디노
                if (Dialogue1.instance.CharacterDC[12] == 0)
                {
                    switch (count)
                    {
                        case 5:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;                          
                            break;
                        case 7:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 8:
                            GameScript1.instance.textnum = 0;
                            CName.text = "히로";
                            break;
                        case 11:
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 12:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 13:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 14:
                            GameScript1.instance.textnum = 0;
                            CName.text = "디노";
                            break;
                        case 15:
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(12);
                            }
                            break;
                        case 16:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            CName.text = "히로";
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[12] == 1)
                {
                    switch (count)
                    {
                        case 1:
                            CharacterList.instance.CharacterFaceList[10].face[0].SetActive(false);
                            CName.text = "디노";
                            break;
                        case 2:
                            CharacterList.instance.CharacterFaceList[10].face[1].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 3:
                            CharacterList.instance.CharacterFaceList[10].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[10].face[2].SetActive(true);
                            CName.text = "디노";
                            break;
                        case 4:
                            CharacterList.instance.CharacterFaceList[10].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[10].face[3].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 5:
                            CName.text = "디노";
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[10].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[10].face[2].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 7:
                            CName.text = "디노";
                            break;
                        case 8:
                            CharacterList.instance.CharacterFaceList[10].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[10].face[3].SetActive(true);
                            break;
                        case 12:
                            CName.text = "히로";
                            break;
                        case 13:
                            CName.text = "디노";
                            break;
                        case 15:
                            CName.text = "히로";
                            break;
                        case 20:
                            CName.text = "디노";
                            break;
                        case 21:
                            CName.text = "히로";
                            break;
                        case 22:
                            CName.text = "디노";
                            break;
                        case 23:
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 2.3f);
                                getMenu = 1;
                                Menu.instance.reactionFIn.Enqueue(Menu.instance.tmpNum); //리액션페이드인 큐에 추가
                                Menu.instance.menuFOut.Enqueue(Menu.instance.tmpNum);//메뉴 페이드아웃큐에 자리 추가
                                Menu.instance.MenuFadeOut();//원래 있던 메뉴 페이드아웃                                
                                Menu.instance.tmpNum++;
                                Menu.instance.menuFOut.Enqueue(Menu.instance.tmpNum);//메뉴 페이드아웃큐에 자리 추가
                                Menu.instance.reactionFIn.Enqueue(Menu.instance.tmpNum); //리액션페이드인 큐에 추가
                               // Menu.instance.Invoke("MenuFadeOut", 0.5f);
                            }
                            break;
                        case 24:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 0.6f);
                                Menu.instance.FEventMenu(12);
                                Menu.instance.FEventMenu(13);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(22, 0);
                            }
                            break;
                        case 25:
                            UserInputManager.instance.SetCanTouch(false);;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.MenuFadeIn();
                                Menu.instance.Invoke("MenuFadeIn", 0.8f);
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 26:
                            CharacterList.instance.CharacterFaceList[10].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[10].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            CName.text = "히로";
                            break;
                        case 27:
                            CharacterList.instance.CharacterFaceList[10].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[10].face[4].SetActive(true);
                            CName.text = "디노";
                            break;
                        case 28:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 29:
                            GameScript1.instance.textnum = 0;
                            CName.text = "디노";
                            break;
                        case 30:
                            CName.text = "히로";
                            break;
                        case 31:
                            CharacterList.instance.CharacterFaceList[10].face[4].SetActive(false);
                            CName.text = "디노";
                            break;
                        case 33:
                            CharacterList.instance.CharacterFaceList[10].face[1].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 34:
                            CharacterList.instance.CharacterFaceList[10].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[10].face[2].SetActive(true);
                            CName.text = "디노";
                            break;
                        case 36:
                            CharacterList.instance.CharacterFaceList[10].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[10].face[3].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 37:
                            CharacterList.instance.CharacterFaceList[10].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[10].face[0].SetActive(true);
                            break;
                        case 38:
                            CharacterList.instance.CharacterFaceList[10].face[0].SetActive(false);
                            CName.text = "디노";
                            break;
                        case 39:
                            CName.text = "히로";                         
                            break;
                        case 42:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.ReactionFadeIn();//히로 리액션 페이드인
                                getMenu = 2;
                            }
                            break;
                        case 43:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            CName.text = "디노";
                            break;
                    }
                }
                break;

            case 13: //닥터 펭
                if (Dialogue1.instance.CharacterDC[13] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 1;                          
                            break;
                        case 5:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 8:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 9:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 10:
                            CName.text = "닥터 펭";
                            break;
                        case 13:
                            CharacterList.instance.CharacterFaceList[11].face[0].SetActive(true);
                            break;
                        case 14:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 15:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 16:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(14);
                            }
                            break;
                        case 18:
                            CharacterList.instance.CharacterFaceList[11].face[0].SetActive(false);
                            GameScript1.instance.textnum = 0;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[13] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 1:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 3:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 4:
                            CharacterList.instance.CharacterFaceList[11].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 5:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 8:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 9:
                            CharacterList.instance.CharacterFaceList[11].face[0].SetActive(false);
                            break;
                        case 17:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 18:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 20:
                            CharacterList.instance.CharacterFaceList[11].face[1].SetActive(true);
                            break;
                        case 23:
                            CharacterList.instance.CharacterFaceList[11].face[1].SetActive(false);
                            break;
                        case 27:
                            CharacterList.instance.CharacterFaceList[11].face[0].SetActive(true);
                            break;
                        case 30:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 31:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.smallFOut.Enqueue(16);
                            }
                            break;
                        case 33:
                            CharacterList.instance.CharacterFaceList[11].face[0].SetActive(false);
                            break;
                        case 34:
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }
                            break;
                        case 35:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 36:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(16);
                                Menu.instance.FEventMenu(14);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(24, 0);
                            }
                            break;
                        case 37:
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 38:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 39:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 40:
                            CharacterList.instance.CharacterFaceList[11].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                    }
                }
                break;


            case 14: //롤렝드
                if (Dialogue1.instance.CharacterDC[14] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(true);
                            break;
                        case 1:
                            BgmManager.instance.PlayCharacterBGM(14);//캐릭터 테마 재생
                            CharacterMove.instance.InCount(); //이 캐릭터만 특수하게 여기서 등장
                            GameScript1.instance.textnum = 0;
                            CName.text = "???";
                            break;
                        case 2:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 3:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 5:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 6:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 8:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 9:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[7].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 11:
                            GameScript1.instance.textnum = 0;                           
                            break;
                        case 13:
                            CharacterList.instance.CharacterFaceList[13].face[7].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 14:
                            GameScript1.instance.textnum = 0;
                            CName.text = "롤렝드";
                            break;
                        case 15:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 16:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 17:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(false);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 18:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 19:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            SEManager.instance.PlayBirdSound();
                            GameScript1.instance.textnum = 1;
                            break;
                        case 20:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 21:
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(15);
                            }
                            break;
                        case 22:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 24:
                            GameScript1.instance.textnum = 0;
                            break;
                    }
                }
                else if (Dialogue1.instance.CharacterDC[14] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            BgmManager.instance.StopBgm();
                            BgmManager.instance.PlayCharacterBGM(14);//캐릭터 테마 재생
                            GameScript1.instance.textnum = 0;
                            CName.text = "롤렝드";
                            break;
                        case 4:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 5:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 6:
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(true);
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(false);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 7:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 10:
                            CharacterList.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 11:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 12:
                            CharacterList.instance.CharacterFaceList[12].face[1].SetActive(true);
                            break;
                        case 14:
                            CharacterList.instance.CharacterFaceList[12].face[1].SetActive(false);
                            break;
                        case 22:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(true);
                            break;
                        case 23:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[12].face[1].SetActive(true);
                            break;
                        case 26:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[7].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 27:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 28:
                            CharacterList.instance.CharacterFaceList[12].face[1].SetActive(false);
                            break;
                        case 29:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 30:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 35:
                            CharacterList.instance.CharacterFaceList[12].face[1].SetActive(true);
                            break;
                        case 36:
                            CharacterList.instance.CharacterFaceList[12].face[1].SetActive(false);
                            break;
                        case 38:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(true);
                            break;
                        case 39:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(false);
                            break;
                        case 41:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(true);
                            break;
                        case 44:
                            CharacterList.instance.CharacterFaceList[13].face[7].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 45:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 46:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(false);
                            break;
                        case 47:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFOut.Enqueue(17);
                                Menu.instance.menuFOut.Enqueue(Menu.instance.tmpNum); //메뉴 페이드아웃 큐에 추가
                                Menu.instance.MenuFadeOut();//원래 있던 메뉴 페이드아웃
                            }
                            break;
                        case 48:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.StartCoroutine(SmallFade.instance.FadeToZero());
                            }
                            break;
                        case 50:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                UserInputManager.instance.SetCanTouch(false);;
                                UserInputManager.instance.Invoke("SetCanTouchTrue", 1f);
                                SmallFade.instance.smallFIn.Enqueue(17);
                                Menu.instance.FEventMenu(15);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(25, 0);
                            }
                            break;
                        case 51:
                            CharacterList.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            UserInputManager.instance.SetCanTouch(false);;
                            GameScript1.instance.textnum = 1;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 52:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 53:
                            GameScript1.instance.textnum = 1;
                            break;
                        case 54:
                            GameScript1.instance.textnum = 0;
                            break;
                        case 55:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(true);
                            break;
                        case 56:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(false);
                            break;
                        case 57:
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(true);
                            GameScript1.instance.textnum = 1;
                            break;
                        case 59:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(true);
                            GameScript1.instance.textnum = 0;
                            break;
                        case 60:
                            GameScript1.instance.textnum = 1;                           
                            break;
                        case 61:
                            GameScript1.instance.textnum = 0;
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;
                                VisitorNote.instance.OpenNewSentence(15);
                            }
                            CharacterList.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterList.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 62:
                            CharacterList.instance.CharacterFaceList[12].face[0].SetActive(false);
                            break;
                    }
                }
                break;
        }
                return;       
    }
}
