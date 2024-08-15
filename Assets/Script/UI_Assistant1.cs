using UnityEngine;
using UnityEngine.UI;

public class UI_Assistant1 : MonoBehaviour //대사창 관련
{
    public static UI_Assistant1 instance;

    private TextWriter.TextWriterSingle textWriterSingle;
     int count = 0; //대사 카운트
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

    public bool talking = false; // true면 대화중

    public int getMenu = 0; //1이면 주인공 아기가 스페셜 메뉴를 가져오는 중, 친밀도 이벤트 시나리오 중 아기 페이드인에 사용

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public int GetCurrentDialogueCount()
    {
        return count;
    }

    public void OpenDialogue() //대화 시작
    {
        talking = true;
        SystemManager.instance.SetCanTouch(false);
        Dialogue.instance.SelectedFirstDialogue(); //첫 문장 나타남
        SystemManager.instance.SetCanTouch(true,1.5f); 
    }  

    public void OpenDialogue2() //첫 대사 제외 나머지 대사
    {
        talking = true;
        if (textWriterSingle != null && textWriterSingle.IsActive()) // 대사 출력 도중 터치했을 경우 출력 스킵, 바로 대사 전부 출력
        {
           textWriterSingle.WriteAllAndDestroy();
        }
        else
        {
            DialogueEvent(); //대화 이벤트 있으면 실행
            Dialogue.instance.SelectedNextDialogue(); //다음 대사 가져오기
            string[] messegeArray = Dialogue.instance.messegeArray;
            if (count < messegeArray.Length) //대사 카운트가 대사 갯수와 같아질 때까지 반복
            {
                string messege = messegeArray[count];
                if (Dialogue.instance.IsBabayText()) //주인공 대사일 경우
                {
                    characterText.text = "";
                     SystemManager.instance.ChangeToBabyTB();
                     textWriterSingle = TextWriter.AddWriter_Static(babyText, messege);
                     
                }
                else
                {
                     babyText.text = "";
                     SystemManager.instance.ChangeToCharacterTB(); //캐릭터 대사창으로 변경
                     textWriterSingle = TextWriter.AddWriter_Static(characterText, messege);
                }

                count++;
            }
            else if (count == messegeArray.Length) //대사가 모두 끝났을 때
            {
                count = 0;
                characterText.text = ""; //대사창 공백으로 만들고
                babyText.text = "";
                if(VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//첫 만남이나 이벤트 다시보기가 아닐 경우
                {
                    if (Dialogue.instance.CharacterDC[CharacterManager.instance.GetCharacterNum()] == 0 || Dialogue.instance.CharacterDC[0] == 1)//캐릭터들의 첫 방문이거나 제제 튜토리얼 때는 BackToCafe() 실행
                    {
                        SystemManager.instance.BackToCafe(); //카페로 복귀
                    }
                    else //그 외에 친밀도 이벤트 대화의 경우는 BackToCafe2() 실행
                    {
                        SystemManager.instance.BackToCafe2(CharacterManager.instance.GetCharacterNum());
                    }
                }
                else//첫 만남 혹은 이벤트 다시보기일 때
                {
                    SystemManager.instance.AfterRePlayStroy();
                }
                
                talking = false;
            }
        }
    }

    
    private void DialogueEvent() //대화 이벤트
    {
        int cNum = CharacterManager.instance.GetCharacterNum();
        int babyNum = -1; // 친밀도 이벤트 시에 주인공 페이드인/아웃에 사용
        int babySeatNum = -1;

        if(cNum != 0)
        {
            if(cNum % 2 == 0) // 캐릭터가 짝수 (왼쪽에 앉아있음)
            {
                babyNum = 16;
                babySeatNum = SmallFade.instance.CharacterSeat[cNum-1]+1;
            }
            else
            {
                babyNum = 17;
                babySeatNum = SmallFade.instance.CharacterSeat[cNum-1]-1;
            }
        }

        switch (cNum)
        {
            case 0: //제제
                if (Dialogue.instance.CharacterDC[0] == 0)
                {
                    switch (count)
                    {
                        case 1:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 2:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 4:
                            SystemManager.instance.SetCanTouch(false);
                            SystemManager.instance.SetNeedAction(true);
                            SystemManager.instance.Invoke("ShowBabyNameSettingWindow", 1.3f);//아기 이름 설정
                            break;
                        case 5:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "제제";
                            break;
                        case 7:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 8:
                            CharacterManager.instance.CharacterFaceList[0].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 9:
                            CharacterManager.instance.CharacterFaceList[0].face[0].SetActive(false);
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
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 21:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 22:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 23:
                            Dialogue.instance.SetBabyText(false);
                            panel3.SetActive(false);
                            panel4.SetActive(true);
                            break;
                        case 25:
                            panel4.SetActive(false);
                            panel8.SetActive(true);
                            break;
                        case 29:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 30:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            panel8.SetActive(false);
                            panel.SetActive(true);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[0] == 1)
                {
                    switch (count)
                    {
                        case 1:
                            SmallFade.instance.SetCharacter(1); //도리 작은 캐릭터 설정
                            break;
                        case 2: 
                            SmallFade.instance.FadeIn(); //도리 페이드인
                            panel.SetActive(false);
                            panel5.SetActive(true);
                            SystemManager.instance.SetCanTouch(false);
                            SystemManager.instance.SetNeedAction(true);
                            SystemManager.instance.Invoke("DownTextBox", 1.3f);
                            break;
                        case 3: //도리 터치 후, 메뉴 힌트 말풍선 등장
                            SystemManager.instance.SetNeedAction(false);
                            SystemManager.instance.SetCanTouch(true,1f);
                            break;
                        case 5: // 메뉴 힌트 말풍선 터치 활성화
                            SystemManager.instance.SetNeedAction(true);
                            SystemManager.instance.SetCanTouch(false);
                            MenuHint.instance.Invoke("CanClickMHB",1f);
                            break;
                        case 7:
                            SystemManager.instance.SetNeedAction(false);
                            panel6.SetActive(false);
                            panel7.SetActive(true);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[0] == 1)
                {
                    Debug.Log("?????");
                    if(count == 11)
                    {
                        SmallFade.instance.SmallCharacter[0].GetComponent<Button>().interactable = false;//제제 클릭 불가
                    }
                }
                break;
            case 1: // 도리
                if (Dialogue.instance.CharacterDC[1] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 4:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 8:
                            CName.text = "도리";
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[1] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                            }
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 2:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            CharacterManager.instance.CharacterFaceList[1].face[0].SetActive(true);//표정 1
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 4:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[1].face[0].SetActive(false);//표정 기본으로
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 11:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 12:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 13:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 14:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 15:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 16:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 17:
                            CharacterManager.instance.CharacterFaceList[1].face[0].SetActive(true);//표정 1
                            break;
                        case 18:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 19:
                            CharacterManager.instance.CharacterFaceList[1].face[0].SetActive(false);//표정 기본으로
                            Dialogue.instance.SetBabyText(false);
                            break;                          
                        case 21:
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 22:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 23:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 25:
                            CharacterManager.instance.CharacterFaceList[1].face[1].SetActive(true);//표정 2
                            break;
                        case 27://여기서 주인공 아기 메뉴 가지러 페이드아웃
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1; //메뉴 가지러 갔음
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공(왼쪽) 페이드아웃
                            }                                                  
                            break;
                        case 28:
                            CharacterManager.instance.CharacterFaceList[1].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[1].face[0].SetActive(true);//표정 1
                            Dialogue.instance.SetBabyText(false);
                            if(VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(1);//스페셜 메뉴 준비
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(11, 0);
                            }
                            break;
                        case 29:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();//스페셜 메뉴 페이드인
                            }                                
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 30:
                            CharacterManager.instance.CharacterFaceList[1].face[0].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2; //메뉴 갖다줬음
                            }     
                            break;
                        case 31:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 32:
                            CharacterManager.instance.CharacterFaceList[1].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 33:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 34:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 35:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 36:
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                VisitorNote.instance.OpenNewSentence(1);//도리 손님노트 정보 갱신
                            }
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                    }
                }
                break;
            case 2: //붕붕
                if (Dialogue.instance.CharacterDC[2] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            break;
                        case 2:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 4:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 7:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 10:
                            CName.text = "붕붕";
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[2] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeIn();//붕붕 페이드인
                            }                             
                            break;
                        case 2:
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                            }      
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 8:
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 9:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[2].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[2].face[1].SetActive(true);
                            break;
                        case 13:
                            CharacterManager.instance.CharacterFaceList[2].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[2].face[0].SetActive(true);
                            break;
                        case 14:
                            CharacterManager.instance.CharacterFaceList[2].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[2].face[1].SetActive(true);
                            break;
                        case 16:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[7].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 17:
                            CharacterManager.instance.CharacterFaceList[13].face[7].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 19:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 20:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 21:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 22:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 25:
                            CharacterManager.instance.CharacterFaceList[2].face[1].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 27:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1; //메뉴 가지러 갔음
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃  //아기 페이드아웃    
                            }                                                                           
                            break;
                        case 28:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 29:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(2);//스페셜 메뉴 준비
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(12, 0);
                            }
                            break;
                        case 30:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();//스페셜 메뉴 페이드인
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 31:
                            Dialogue.instance.SetBabyText(false);
                            break;
                    }
                }
                break;

            case 3: //빵빵
                if (Dialogue.instance.CharacterDC[3] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            CharacterManager.instance.CharacterFaceList[3].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 2:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            CharacterManager.instance.CharacterFaceList[3].face[2].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 5:
                            CName.text = "빵빵";
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(false);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[3] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                            }
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 4:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[3].face[0].SetActive(false);
                            break;
                        case 12:
                            CharacterManager.instance.CharacterFaceList[3].face[0].SetActive(true);
                            break;
                        case 15:
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 16:
                            CharacterManager.instance.CharacterFaceList[3].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[3].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 20:
                            CharacterManager.instance.CharacterFaceList[3].face[1].SetActive(false);
                            break;
                        case 21:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 22:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 23:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 27:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 28:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 29:
                            CharacterManager.instance.CharacterFaceList[3].face[2].SetActive(true);
                            break;
                        case 30:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 31:
                            CharacterManager.instance.CharacterFaceList[3].face[2].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 32:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃 
                            }               
                            break;
                        case 33:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 35:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(3);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(13, 0);
                            }
                            break;
                        case 36:
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 37:
                            CharacterManager.instance.CharacterFaceList[3].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(false);                            
                            break;
                        case 39:
                            CharacterManager.instance.CharacterFaceList[3].face[2].SetActive(false);
                            break;
                    }
                }
                break;

            case 4: //개나리
                if (Dialogue.instance.CharacterDC[4] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "개나리";
                            break;
                        case 2:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(false);                    
                            break;
                        case 11:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[4] == 1)
                {
                    switch (count)
                    {
                        case 7:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);       
                            break;
                        case 11:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃 
                            }
                            break;
                        case 12:
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(4);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(14, 0);
                            }
                            break;
                        case 13:
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);                           
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 14:
                            Dialogue.instance.SetBabyText(false);                        
                            break;
                    }
                }
                break;

            case 5: //또롱이
                if (Dialogue.instance.CharacterDC[5] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 2:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 4:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "또롱";
                            break;
                        case 8:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);      
                            break;
                        case 9:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 10:
                            Dialogue.instance.SetBabyText(false);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[5] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 2:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 12:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 13:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 14:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 15:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
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
                if (Dialogue.instance.CharacterDC[6] == 0)
                {
                    switch (count)
                    {
                        case 1:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 2:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 8:
                            CName.text = "도로시";
                            break;
                        case 9:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 10:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 12:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[6] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                            }
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 2:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 5:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃 
                            }
                            break;
                        case 9:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(6);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(16, 0);
                            }
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 11:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 12:
                            CharacterManager.instance.CharacterFaceList[4].face[0].SetActive(true);
                            break;
                        case 15:
                            CharacterManager.instance.CharacterFaceList[4].face[0].SetActive(false);
                            break;
                        case 17:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 18:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                    }
                }
                break;

            case 7: //루루
                if (Dialogue.instance.CharacterDC[7] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 2:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 4:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "루루";
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[7] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            CharacterManager.instance.CharacterFaceList[5].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 2:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 5:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 7:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 8:
                            CharacterManager.instance.CharacterFaceList[5].face[0].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 9:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 11:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 20:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 21:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 28:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 29:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 36:
                            CharacterManager.instance.CharacterFaceList[5].face[1].SetActive(true);
                            break;
                        case 37:
                            CharacterManager.instance.CharacterFaceList[5].face[1].SetActive(false);
                            break;
                        case 38:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 39:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 41:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 42:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 52:
                            CharacterManager.instance.CharacterFaceList[5].face[1].SetActive(true);
                            break;
                        case 53:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 55:
                            CharacterManager.instance.CharacterFaceList[5].face[1].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃 
                            }
                            break;
                        case 58:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(7);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(17, 0);
                            }
                            break;
                        case 59:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(true);
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 60:
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 61:
                            CharacterManager.instance.CharacterFaceList[5].face[1].SetActive(true);
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
                if (Dialogue.instance.CharacterDC[8] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "샌디";
                            break;
                        case 3:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 4:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 7:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[8] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 2:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 12:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 13:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 44:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 46:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 47:
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃 
                            }
                            break;
                        case 48:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(8);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(18, 0);
                            }
                            break;
                        case 49:
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 50:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 51:
                            CharacterManager.instance.CharacterFaceList[6].face[0].SetActive(true);
                            break;
                        case 52:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 53:
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 54:
                            CharacterManager.instance.CharacterFaceList[6].face[0].SetActive(false);
                            break;
                    }
                }
                    break;

            case 9: //친구
                if (Dialogue.instance.CharacterDC[9] == 0)
                {
                    switch (count)
                    {
                        case 1:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "친구";
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 9:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 10:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 12:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[9] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[7].face[0].SetActive(true);
                            break;
                        case 9:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 10:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 14:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 16:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.MenuFadeOut();//원래 있던 메뉴 페이드아웃
                            }
                            break;
                        case 17:
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃 
                            }
                            break;
                        case 19:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(9);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(19, 0);
                            }
                            break;
                        case 20:
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 21:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 23:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 24:
                            Dialogue.instance.SetBabyText(false);
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
                if (Dialogue.instance.CharacterDC[10] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "찰스";
                            break;
                        case 3:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 4:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(false);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[10] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 3:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 4:
                            CharacterManager.instance.CharacterFaceList[8].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 5:
                            CharacterManager.instance.CharacterFaceList[8].face[0].SetActive(false);
                            break;
                        case 7:
                            CharacterManager.instance.CharacterFaceList[8].face[1].SetActive(true);
                            break;
                        case 8:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 9:
                            CharacterManager.instance.CharacterFaceList[8].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[8].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;//아기 페이드아웃 위해서 일부러 대입
                            }
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[10] == 2)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 5:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 9:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[8].face[1].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 14:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 15:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 16:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);                           
                            break;                            
                        case 18:
                            CharacterManager.instance.CharacterFaceList[8].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);                            
                            CharacterManager.instance.SetSoldierEvent(true);
                            break;
                        case 19:
                            CharacterManager.instance.CharacterFaceList[8].face[0].SetActive(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃
                            }
                            CharacterManager.instance.CharacterOut(10);
                            break;
                        case 20:                            
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1.3f);
                                Menu.instance.SoldierEvent_ServeToPrincess(SmallFade.instance.CharacterSeat[5]);//도로시 자리정보를 매개변수로 넣기
                            }
                            break;
                        case 21:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.MenuFadeIn();
                            }
                            break;
                        case 22:
                            Dialogue.instance.SetBabyText(false);
                            CharacterManager.instance.CharacterFaceList[4].face[0].SetActive(true);
                            SystemManager.instance.SetCanTouch(false);
                            CharacterManager.instance.CharacterIn(6);
                            CName.text = "도로시";
                            break;
                        case 23:
                            Dialogue.instance.SetBabyText(true);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            break;
                        case 24:
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                SmallFade.instance.FadeOut(SmallFade.instance.CharacterSeat[9]);//찰스 작은 캐릭터 페이드아웃
                            }
                            break;
                        case 25:
                            Dialogue.instance.SetBabyText(true);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(true);
                            SystemManager.instance.SetCanTouch(false);
                            SystemManager.instance.SetCanTouch(true,1f);
                            break;
                        case 26:
                            SystemManager.instance.SetCanTouch(false);
                            SystemManager.instance.SetCanTouch(true,1.5f);
                            CharacterManager.instance.CharacterFaceList[8].face[2].SetActive(true);
                            CharacterManager.instance.MovePrincess();//찰스등장을 위해 도로시가 옆으로 이동, 바로 찰스 등장
                            Dialogue.instance.SetBabyText(false);
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
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 31:
                            CharacterManager.instance.CharacterFaceList[4].face[0].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "도로시";
                            break;
                        case 33:
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.MenuFadeOut();
                            }
                            break;
                        case 34:
                            Dialogue.instance.SetBabyText(false);
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
                            CharacterManager.instance.CharacterFaceList[8].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[8].face[0].SetActive(true);
                            CName.text = "찰스";
                            break;
                        case 44:
                            CName.text = "도로시";
                            break;
                        case 46:
                            CharacterManager.instance.CharacterFaceList[8].face[0].SetActive(false);
                            CName.text = "찰스";
                            break;
                        case 48:
                            CName.text = "도로시";
                            break;
                        case 49:
                            CharacterManager.instance.CharacterFaceList[8].face[2].SetActive(true);
                            CName.text = "찰스";
                            break;
                        case 50:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            CharacterManager.instance.CharacterOut(10);//찰스 퇴장
                            break;
                        case 51:
                            CharacterManager.instance.CharacterOut(6);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                VisitorNote.instance.OpenNewSentence(10);
                            }
                            break;
                        case 52:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                    }
                }
                break;

            case 11: //무명이
                if (Dialogue.instance.CharacterDC[11] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 2:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 5:
                            CharacterManager.instance.CharacterFaceList[9].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 9:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[11] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 2:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 3:
                            CharacterManager.instance.CharacterFaceList[9].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 4:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 8:
                            CharacterManager.instance.CharacterFaceList[9].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[9].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 11:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 13:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 14:
                            CharacterManager.instance.CharacterFaceList[9].face[1].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 18:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 19:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 20:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetNeedAction(true);
                                SystemManager.instance.ShowCharNameSettingWindow();
                            }
                            break;
                        case 21:
                            CharacterManager.instance.CharacterFaceList[13].face[4].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 22:
                            SystemManager.instance.SetCanTouch(true);
                            SystemManager.instance.SetNeedAction(false);
                            Dialogue.instance.SetBabyText(false);
                            CName.text = SystemManager.instance.GetNameForNameless();
                            break;
                        case 23:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 24:
                            CharacterManager.instance.CharacterFaceList[9].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;
                                VisitorNote.instance.OpenNewSentence(11);
                            }
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[11] == 2)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 15:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[5].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 16:
                            CharacterManager.instance.CharacterFaceList[9].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 18:
                            CharacterManager.instance.CharacterFaceList[9].face[0].SetActive(false);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 19:
                            CharacterManager.instance.CharacterFaceList[13].face[5].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[7].SetActive(true);
                            break;
                        case 22:
                            CharacterManager.instance.CharacterFaceList[13].face[7].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 25:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 28:
                            CharacterManager.instance.CharacterFaceList[9].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 29:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 30:
                            CharacterManager.instance.CharacterFaceList[9].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[9].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 31:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 32:
                            CharacterManager.instance.CharacterFaceList[9].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[9].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 34:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃 
                            }
                            break;
                        case 35:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 36:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(11);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(21, 0);
                            }
                            break;
                        case 37:
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 38:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 39:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 41:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.MenuFadeOut();//원래 있던 메뉴 페이드아웃
                            }
                            break;
                        case 42:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,0.8f);
                                Menu.instance.SetFriendEventMenu(11);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(31, 0);
                            }
                            break;
                        case 43:
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            break;
                        case 44:
                            CharacterManager.instance.CharacterFaceList[9].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[9].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 45:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 51:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 52:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 53:
                            CharacterManager.instance.CharacterFaceList[9].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[9].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 54:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 55:
                            CharacterManager.instance.CharacterFaceList[9].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 59:
                            CharacterManager.instance.CharacterFaceList[9].face[3].SetActive(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                VisitorNote.instance.OpenNewSentence(12);
                            }
                            break;
                    }
                }
                break;

            case 12: //히로디노
                if (Dialogue.instance.CharacterDC[12] == 0)
                {
                    switch (count)
                    {
                        case 5:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);                          
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "히로";
                            break;
                        case 11:
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 12:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 13:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 14:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "디노";
                            break;
                        case 15:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 16:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "히로";
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[12] == 1)
                {
                    switch (count)
                    {
                        case 1:
                            CharacterManager.instance.CharacterFaceList[10].face[0].SetActive(false);
                            CName.text = "디노";
                            break;
                        case 2:
                            CharacterManager.instance.CharacterFaceList[10].face[1].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 3:
                            CharacterManager.instance.CharacterFaceList[10].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[10].face[2].SetActive(true);
                            CName.text = "디노";
                            break;
                        case 4:
                            CharacterManager.instance.CharacterFaceList[10].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[10].face[3].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 5:
                            CName.text = "디노";
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[10].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[10].face[2].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 7:
                            CName.text = "디노";
                            break;
                        case 8:
                            CharacterManager.instance.CharacterFaceList[10].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[10].face[3].SetActive(true);
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
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,2.3f);
                                getMenu = 1;
                                if(Menu.instance.GetSeatNum() % 2 == 0) // 짝수(마지막 서빙이 히로)면 디노 메뉴 먼저 페이드아웃
                                {
                                    Menu.instance.MenuFadeOut(Menu.instance.GetSeatNum()+1);
                                }
                                else //홀수(마지막 서빙이 디노)면 히로 메뉴 먼저 페이드아웃
                                {
                                    Menu.instance.MenuFadeOut(Menu.instance.GetSeatNum()-1);
                                }
                                Menu.instance.MenuFadeOut(-1, false);   
                            }
                            break;
                        case 24:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,0.6f);
                                Menu.instance.SetFriendEventMenu(12);
                                Menu.instance.SetFriendEventMenu(13);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(22, 0);
                            }
                            break;
                        case 25:
                            SystemManager.instance.SetCanTouch(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.MenuFadeIn();
                                Menu.instance.Invoke("MenuFadeIn", 0.8f);
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 26:
                            CharacterManager.instance.CharacterFaceList[10].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[10].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "히로";
                            break;
                        case 27:
                            CharacterManager.instance.CharacterFaceList[10].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[10].face[4].SetActive(true);
                            CName.text = "디노";
                            break;
                        case 28:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 29:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "디노";
                            break;
                        case 30:
                            CName.text = "히로";
                            break;
                        case 31:
                            CharacterManager.instance.CharacterFaceList[10].face[4].SetActive(false);
                            CName.text = "디노";
                            break;
                        case 33:
                            CharacterManager.instance.CharacterFaceList[10].face[1].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 34:
                            CharacterManager.instance.CharacterFaceList[10].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[10].face[2].SetActive(true);
                            CName.text = "디노";
                            break;
                        case 36:
                            CharacterManager.instance.CharacterFaceList[10].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[10].face[3].SetActive(true);
                            CName.text = "히로";
                            break;
                        case 37:
                            CharacterManager.instance.CharacterFaceList[10].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[10].face[0].SetActive(true);
                            break;
                        case 38:
                            CharacterManager.instance.CharacterFaceList[10].face[0].SetActive(false);
                            CName.text = "디노";
                            break;
                        case 39:
                            CName.text = "히로";                         
                            break;
                        case 42:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                Menu.instance.ReactionFadeIn(Menu.instance.GetSeatNum()-1,0);//히로 리액션 페이드인, 히로 다음 디노에게 마지막으로 서빙함
                                getMenu = 2;
                            }
                            break;
                        case 43:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            CName.text = "디노";
                            break;
                    }
                }
                break;

            case 13: //닥터 펭
                if (Dialogue.instance.CharacterDC[13] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(true);                          
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 9:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 10:
                            CName.text = "닥터 펭";
                            break;
                        case 13:
                            CharacterManager.instance.CharacterFaceList[11].face[0].SetActive(true);
                            break;
                        case 14:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 15:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 16:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 18:
                            CharacterManager.instance.CharacterFaceList[11].face[0].SetActive(false);
                            Dialogue.instance.SetBabyText(false);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[13] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 1:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 3:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 4:
                            CharacterManager.instance.CharacterFaceList[11].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 5:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 9:
                            CharacterManager.instance.CharacterFaceList[11].face[0].SetActive(false);
                            break;
                        case 17:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 18:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 20:
                            CharacterManager.instance.CharacterFaceList[11].face[1].SetActive(true);
                            break;
                        case 23:
                            CharacterManager.instance.CharacterFaceList[11].face[1].SetActive(false);
                            break;
                        case 27:
                            CharacterManager.instance.CharacterFaceList[11].face[0].SetActive(true);
                            break;
                        case 30:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 31:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 33:
                            CharacterManager.instance.CharacterFaceList[11].face[0].SetActive(false);
                            break;
                        case 34:
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃 
                            }
                            break;
                        case 35:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 36:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(14);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(24, 0);
                            }
                            break;
                        case 37:
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();
                                getMenu = 2;
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 38:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 39:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 40:
                            CharacterManager.instance.CharacterFaceList[11].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                    }
                }
                break;
            case 14: //롤렝드
                if (Dialogue.instance.CharacterDC[14] == 0)
                {
                    switch (count)
                    {
                        case 0:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(true);
                            break;
                        case 1:
                            BgmManager.instance.PlayCharacterBGM(14);//캐릭터 테마 재생
                            CharacterManager.instance.CharacterIn(14); //이 캐릭터만 특수하게 여기서 등장
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "???";
                            break;
                        case 2:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 3:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 5:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 6:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 8:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 9:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[13].face[2].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[7].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 11:
                            Dialogue.instance.SetBabyText(false);                           
                            break;
                        case 13:
                            CharacterManager.instance.CharacterFaceList[13].face[7].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 14:
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "롤렝드";
                            break;
                        case 15:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 16:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 17:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(false);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 18:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 19:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            SEManager.instance.PlayBirdSound();
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 20:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 21:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 22:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 24:
                            Dialogue.instance.SetBabyText(false);
                            break;
                    }
                }
                else if (Dialogue.instance.CharacterDC[14] == 1)
                {
                    switch (count)
                    {
                        case 0:
                            BgmManager.instance.StopBgm();
                            BgmManager.instance.PlayCharacterBGM(14);//캐릭터 테마 재생
                            Dialogue.instance.SetBabyText(false);
                            CName.text = "롤렝드";
                            break;
                        case 4:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 5:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 6:
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(false);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 7:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 10:
                            CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 11:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 12:
                            CharacterManager.instance.CharacterFaceList[12].face[1].SetActive(true);
                            break;
                        case 14:
                            CharacterManager.instance.CharacterFaceList[12].face[1].SetActive(false);
                            break;
                        case 22:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(true);
                            break;
                        case 23:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[12].face[1].SetActive(true);
                            break;
                        case 26:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[7].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 27:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 28:
                            CharacterManager.instance.CharacterFaceList[12].face[1].SetActive(false);
                            break;
                        case 29:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 30:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 35:
                            CharacterManager.instance.CharacterFaceList[12].face[1].SetActive(true);
                            break;
                        case 36:
                            CharacterManager.instance.CharacterFaceList[12].face[1].SetActive(false);
                            break;
                        case 38:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(true);
                            break;
                        case 39:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(false);
                            break;
                        case 41:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(true);
                            break;
                        case 44:
                            CharacterManager.instance.CharacterFaceList[13].face[7].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 45:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 46:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(false);
                            break;
                        case 47:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.MenuFadeOut();//원래 있던 메뉴 페이드아웃
                            }
                            break;
                        case 48:
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 1;
                                SmallFade.instance.FadeOut(babyNum, babySeatNum); //주인공 페이드아웃 
                            }
                            break;
                        case 50:
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SystemManager.instance.SetCanTouch(false);
                                SystemManager.instance.SetCanTouch(true,1f);
                                Menu.instance.SetFriendEventMenu(15);
                            }
                            else //다시보기일 때, 특별 메뉴 팝업 설정
                            {
                                Menu.instance.SetTableMenu(25, 0);
                            }
                            break;
                        case 51:
                            CharacterManager.instance.CharacterFaceList[13].face[3].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            SystemManager.instance.SetCanTouch(false);
                            Dialogue.instance.SetBabyText(true);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                SmallFade.instance.SetCharacter(babyNum);
                                Menu.instance.MenuFadeIn();
                            }
                            Popup.instance.OpenPopup();//메뉴 팝업, 팝업 닫으면 다음 대사 넘기기 가능
                            break;
                        case 52:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 53:
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 54:
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 55:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(true);
                            break;
                        case 56:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(false);
                            break;
                        case 57:
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(true);
                            Dialogue.instance.SetBabyText(true);
                            break;
                        case 59:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(true);
                            Dialogue.instance.SetBabyText(false);
                            break;
                        case 60:
                            Dialogue.instance.SetBabyText(true);                           
                            break;
                        case 61:
                            Dialogue.instance.SetBabyText(false);
                            if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                            {
                                getMenu = 2;
                                VisitorNote.instance.OpenNewSentence(15);
                            }
                            CharacterManager.instance.CharacterFaceList[13].face[6].SetActive(false);
                            CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(true);
                            break;
                        case 62:
                            CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(false);
                            break;
                    }
                }
                break;
        }
                return;       
    }
}
