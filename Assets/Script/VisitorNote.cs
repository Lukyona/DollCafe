using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisitorNote : MonoBehaviour 
{
    public static VisitorNote instance;

    public Animator VNButtonAnimator;
    public Animator noteAnimator;
    public int pageNum = 1; //1페이지가 첫 페이지
    public int pageNum2 = 0;
    public GameObject[] characterInfo; //캐릭터 정보 배열
    public GameObject[] page; //페이지 배열
    public GameObject[] friendshipGauge; //친밀도 게이지 배열
    public GameObject nextPageButton; //다음 페이지 버튼
    public GameObject previousPageButton; //이전 페이지 버튼
    static int bigPageNum = 1; //1-4페이지까지가 1, 5-8까지가 2, 9 - 12가 3, 13 - 14가 4

    public GameObject pageText1; //페이지 텍스트
    public GameObject pageText2;

    public GameObject[] LikeMenu1;//좋아하는 첫번째 메뉴 배열, 0도리~~~11히로, 12디노, 13닥터펭, 14롤렝드
    public GameObject[] LikeMenu2; //두번째 좋아하는 메뉴 배열, 0도리, 1 빵빵, 2개나리~~10닥터펭 11롤렝드,  붕붕이/히로디노만 없음

    public GameObject[] WantToSay;//친밀도 이벤트 달성 시 노트에 새롭게 보일 문장, 0도리 1루루 2친구 3찰스 4무명이1 5무명이2 6롤렝드

    public Text Name; //플레이어가 지어준 무명이 이름

    public int[] NextPageOpen = new int[10]; // 인덱스 0은  5페이지, 값이 0이면 페이지가 안 열린 거

    public int openPage = 1;//열린 페이지, 도리가 기본으로 있으므로 1, 마지막이 14

    int[] LikeMenu1Open = new int[15]; //노트의 좋아하는 첫번째 메뉴 정보가 열렸는지 확인하기 위한 배열
    int[] LikeMenu2Open = new int[12]; //노트의 좋아하는 두번째 메뉴 정보가 열렸는지 확인하기 위한 배열

    int[] SecondSentence = new int[8]; //노트의 두번째 하고 싶은 말 정보가 열렸는지 확인

    public int[] FriendshipInfo = new int[13] { 0, 0, 0, 0, 0, 0,0,0,0,0,0,0,0}; //친밀도 게이지 정보(서빙 횟수) 배열

    public Button[] RePlayButton;//다시보기 버튼, 0도리
    public GameObject[] RePlayView; //어떤 걸 다시 볼 지 고르는 창, 0도리

    public GameObject showSpecialMenu;//스메셜 메뉴 다시보기 창
    public Image menuImage; // 스페셜 메뉴 이미지
    public Text menuName; // 메뉴 이름
    public Text whichCharacter; //어느 캐릭터의 스페셜 메뉴인지

    public GameObject rePlayMessage;//첫만남, 이벤트 다시보기 메세지창
    public Text whichStory; //누구의 첫만남/이벤트를 회상할까요? 라고 묻는 텍스트

    public int fmRP = 0; //첫만남 캐릭터별로 숫자 들어감
    public int evRP = 0; //이벤트 캐릭터별로 숫자 들어감

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void ClickVNButton() //손님 노트 버튼 눌렀을 때
    {
        if(!Menu.instance.UIOn)
        {
            Menu.instance.UIOn = true;
            SEManager.instance.PlayUIClickSound(); //효과음
            VNButtonAnimator.SetTrigger("VNButtonOut"); //버튼 위로 올라감       
            noteAnimator.SetTrigger("NoteUp"); //노트 올라옴
        }
    }

    public void ClickNoteCloseButton() //노트 닫기 버튼 눌렀을 때
    {
        Menu.instance.UIOn = false;
        SEManager.instance.PlayUICloseSound(); //효과음
        noteAnimator.SetTrigger("NoteDown"); //노트 내려감
        VNButtonAnimator.SetTrigger("VNButtonIn"); //버튼 내려옴
        if(evRP == 0 && fmRP == 0)//다시보기 아닐 때만 
        {
            Invoke("NoteInit", 0.5f);//0.5초 뒤 노트 정보 1페이지로 초기화
        }
        
    }

    void NoteInit()
    {
        if (rpbClick == 1)//다시보기 창이 활성화되어있으면
        {
            ShowRePlayView();
        }
        characterInfo[pageNum - 1].SetActive(false); // 이전 페이지의 캐릭터 정보를 안 보이게 하고
        page[pageNum - 1].GetComponent<Button>().interactable = true; //이전 페이지의 버튼을 터치 가능하게 함
        if (bigPageNum != 1)
        {
            nextPageButton.SetActive(true); //다음 페이지 버튼 활성화
            previousPageButton.SetActive(false);//이전 버튼 비활성화
            pageText2.SetActive(false);
            pageText1.SetActive(true);

            for (int i = 4; i < 14; i++)//5-14페이지 중 열려있는 페이지 비활성화
            {
                if (NextPageOpen[i - 4] == 1)//페이지가 열려있으면
                {
                    page[i].SetActive(false);
                }
            }

            for (int i = 0; i < 4; i++) //1-4페이지 활성화
            {
                page[i].SetActive(true);
            }
            bigPageNum = 1;
        }
        pageNum = 1; //현재 페이지 넘버      
        characterInfo[pageNum - 1].SetActive(true); //현재 페이지의 캐릭터 정보를 보이게 함
        page[pageNum - 1].GetComponent<Button>().interactable = false; //현재 페이지의 버튼 터치 불가능하게 함      
    }

    public void IncreaseFrinedshipGauge(int n) //친밀도 게이지 증가, n은 캐릭터 넘버
    {
        if(n != 0)
        {
            switch(n)
            {
                case 1: //맥스가 10이므로 0.1씩 증가
                case 2:
                case 3:
                case 4:
                    if(friendshipGauge[n - 1].GetComponent<Image>().fillAmount != 1f)//친밀도 맥스가 아닐 경우
                    {
                        friendshipGauge[n - 1].GetComponent<Image>().fillAmount += 0.1f;
                        FriendshipInfo[n - 1]++;
                    }
                    break;
                case 6:
                    if (friendshipGauge[n - 2].GetComponent<Image>().fillAmount != 1f)
                    {
                        friendshipGauge[n - 2].GetComponent<Image>().fillAmount += 0.1f;
                        FriendshipInfo[n - 2]++;
                    }                  
                    break;
                case 7://맥스 15
                case 8:
                case 9:
                case 10:
                case 12://히로디노
                    if (friendshipGauge[n - 2].GetComponent<Image>().fillAmount != 1f)
                    {
                        friendshipGauge[n - 2].GetComponent<Image>().fillAmount += 0.067f;
                        if(friendshipGauge[n - 2].GetComponent<Image>().fillAmount == 1f)
                        {
                            FriendshipInfo[n - 2] = 15;
                        }
                        else
                        {
                            FriendshipInfo[n - 2]++;
                        }
                    }                       
                    break;
                case 13://히로디노 한번은 패스, 두 캐릭터 중 한번만 친밀도 증가
                    break;
                case 14://닥터펭
                    if (friendshipGauge[n - 3].GetComponent<Image>().fillAmount != 1f)
                    {
                        friendshipGauge[n - 3].GetComponent<Image>().fillAmount += 0.067f;
                        if (friendshipGauge[n - 3].GetComponent<Image>().fillAmount == 1f)
                        {
                            FriendshipInfo[n - 3] = 15;
                        }
                        else
                        {
                            FriendshipInfo[n - 3]++;
                        }
                    }
                    break;
                case 11: //맥스 20, 무명이
                    if (friendshipGauge[n - 2].GetComponent<Image>().fillAmount != 1f)
                    {
                        friendshipGauge[n - 2].GetComponent<Image>().fillAmount += 0.05f;
                        FriendshipInfo[n - 2]++; 
                    }
                    break;
                case 15://롤렝드
                    if (friendshipGauge[n - 3].GetComponent<Image>().fillAmount != 1f)
                    {
                        friendshipGauge[n - 3].GetComponent<Image>().fillAmount += 0.05f;
                        FriendshipInfo[n - 3]++; 
                    }
                    break;
            }
            SaveVisitorNoteInfo();
        }       
    }

    void PageFunction1()
    {
        SEManager.instance.PlayNextPageSound(); //페이지 넘기는 효과음       
        characterInfo[pageNum - 1].SetActive(false); // 이전 페이지의 캐릭터 정보를 안 보이게 하고
        page[pageNum - 1].GetComponent<Button>().interactable = true; //이전 페이지의 버튼을 터치 가능하게 함        
        PageFunction2();
    }

    void PageFunction2()
    {
        characterInfo[pageNum2 - 1].SetActive(true); //현재 페이지의 캐릭터 정보를 보이게 함
        page[pageNum2 - 1].GetComponent<Button>().interactable = false; //현재 페이지의 버튼 터치 불가능하게 함
        pageNum = pageNum2;
    }

    public void TurnToPage()
    {              
        if (1 <= pageNum2 && pageNum2 <= 4)
        {
            bigPageNum = 1;
        }
        else if(5 <= pageNum2 && pageNum2 <= 8)
        {
            bigPageNum = 2;
        }
        else if(9 <= pageNum2 && pageNum2 <= 12)
        {
            bigPageNum = 3;
        }
        else if (pageNum2 == 13 || pageNum2 == 14)
        {
            bigPageNum = 4;
        }
        if (rpbClick == 1)//다시보기 창이 활성화되어있으면
        {
            ShowRePlayView();
        }
        PageFunction1();
    }

    public void ClickNextPageButton() //다음 페이지 버튼 눌렀을 때
    {
        pageText1.SetActive(false);
        pageText2.SetActive(true);
        previousPageButton.SetActive(true); //이전 페이지 버튼 활성화

        if(bigPageNum == 1)
        {
            for (int i = 0; i < 4; i++) //1-4페이지 비활성화
            {
                page[i].SetActive(false);
            }

            for(int i = 4; i < 8; i++) //5-8페이지 중 열려있는 페이지 활성화
            {
                if(NextPageOpen[i - 4] == 1)//페이지가 열려있으면
                {
                    page[i].SetActive(true);
                }
            }
            
            if(NextPageOpen[4] == 0) //9페이지가 열려있지 않으면 다음 페이지 버튼 비활성화
            {
                nextPageButton.SetActive(false);
            }
            bigPageNum = 2;
        }

        else if (bigPageNum == 2)
        {
            for (int i = 4; i < 8; i++) //5-8페이지 비활성화
            {
                page[i].SetActive(false);
            }

            for (int i = 8; i < 12; i++) //9-12페이지 중
            {
                if (NextPageOpen[i - 4] == 1)//페이지가 열려있으면
                {
                    page[i].SetActive(true);
                }
            }

            if (NextPageOpen[8] == 0) //13페이지가 열려있지 않으면
            {
                nextPageButton.SetActive(false);//다음 페이지 버튼 비활성화
            }
            bigPageNum = 3;
        }

        else if (bigPageNum == 3)
        {
            for (int i = 8; i < 12; i++) 
            {
                page[i].SetActive(false);
            }
            if(NextPageOpen[8] == 1) //13페이지 열려있으면 페이지 활성화
            {
                page[12].SetActive(true);
            }
            if (NextPageOpen[9] == 1)//14페이지 열려있으면 페이지 활성화
            {
                page[13].SetActive(true);
            }

            bigPageNum = 4;
            nextPageButton.SetActive(false);//다음 페이지 버튼은 비활성화, 노트의 마지막 부분
        }
    }
   
    public void ClickPreviousPageButton() //이전 페이지 버튼 터치 
    {
        nextPageButton.SetActive(true); //다음 페이지 버튼 활성화

        if (bigPageNum == 2)
        {
            previousPageButton.SetActive(false);//이전 버튼 비활성화
            pageText2.SetActive(false);
            pageText1.SetActive(true);
           
            for (int i = 4; i < 8; i++)//5-8페이지 중 열려있는 페이지 비활성화
            {
                if (NextPageOpen[i - 4] == 1)//페이지가 열려있으면
                {
                    page[i].SetActive(false);
                }
            }

            for (int i = 0; i < 4; i++) //1-4페이지 활성화
            {
                page[i].SetActive(true);
            }
            
            bigPageNum = 1;
        }

        else if (bigPageNum == 3)
        {
            for (int i = 8; i < 12; i++)
            {
                if (NextPageOpen[i - 4] == 1)//페이지가 열려있으면
                {
                    page[i].SetActive(false);
                }
            }

            for (int i = 4; i < 8; i++)
            {
                page[i].SetActive(true);
            }

            bigPageNum = 2;
        }

        else if (bigPageNum == 4)
        {
            if (NextPageOpen[8] == 1)
            {
                page[12].SetActive(false);
            }
            if (NextPageOpen[9] == 1)
            {
                page[13].SetActive(false);
            }

            for (int i = 8; i < 12; i++)
            {
                page[i].SetActive(true);
            }

            bigPageNum = 3;
        }
    }

    public void GuessMenuRight(int c, int n)//캐릭터가 원하는 메뉴를 맞췄을 때, 손님노트의 좋아하는 것 정보 오픈
    {
        switch(c)//c는 캐릭터넘버
        {
            case 1://도리
                if(LikeMenu1[0].activeSelf == false || LikeMenu2[0].activeSelf == false)
                {
                    if (n == 1)
                    {
                        LikeMenu1[0].SetActive(true);
                        LikeMenu1Open[0] = 1;
                    }
                    if (n == 4)
                    {
                        LikeMenu2[0].SetActive(true);
                        LikeMenu2Open[0] = 1;
                    }
                }              
                break;
            case 2:
                if (LikeMenu1[1].activeSelf == false)
                {
                    if (n == 2)
                    {
                        LikeMenu1[1].SetActive(true);
                        LikeMenu1Open[1] = 1;
                    }
                }
                break;
            case 3:
                if (LikeMenu1[c-1].activeSelf == false || LikeMenu2[c-2].activeSelf == false)
                {
                    if (n == 2)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[2] = 1;
                    }
                    if (n == 6)
                    {
                        LikeMenu2[c - 2].SetActive(true);
                        LikeMenu2Open[1] = 1;
                    }
                }
                break;
            case 4:
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 2].activeSelf == false)
                {
                    if (n == 3)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[3] = 1;
                    }
                    if (n == 4)
                    {
                        LikeMenu2[c - 2].SetActive(true);
                        LikeMenu2Open[2] = 1;
                    }
                }
                break;
            case 5:
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 2].activeSelf == false)
                {
                    if (n == 1)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[4] = 1;
                    }
                    if (n == 6 && LikeMenu2[c - 2].activeSelf == false)
                    {
                        GameScript1.instance.CharacterStart(5);//또롱이 이벤트 시작
                        CharacterAppear.instance.eventOn = 5;
                        LikeMenu2[c - 2].SetActive(true);
                        LikeMenu2Open[3] = 1;
                    }
                }
                break;
            case 6:
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 2].activeSelf == false)
                {
                    if (n == 3)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[5] = 1;
                    }
                    if (n == 8)
                    {
                        LikeMenu2[c - 2].SetActive(true);
                        LikeMenu2Open[4] = 1;
                    }
                }
                break;
            case 7:
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 2].activeSelf == false)
                {
                    if (n == 3)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[6] = 1;
                    }
                    if (n == 7)
                    {
                        LikeMenu2[c - 2].SetActive(true);
                        LikeMenu2Open[5] = 1;
                    }
                }
                break;
            case 8:
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 2].activeSelf == false)
                {
                    if (n == 4)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[7] = 1;
                    }
                    if (n == 7)
                    {
                        LikeMenu2[c - 2].SetActive(true);
                        LikeMenu2Open[6] = 1;
                    }
                }
                break;
            case 9:
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 2].activeSelf == false)
                {
                    if (n == 2)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[8] = 1;
                    }
                    if (n == 8)
                    {
                        LikeMenu2[c - 2].SetActive(true);
                        LikeMenu2Open[7] = 1;
                    }
                }
                break;
            case 10:
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 2].activeSelf == false)
                {
                    if (n == 5)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[9] = 1;
                    }
                    if (n == 6)
                    {
                        LikeMenu2[c - 2].SetActive(true);
                        LikeMenu2Open[8] = 1;
                    }
                }
                break;
            case 11:
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 2].activeSelf == false)
                {
                    if (n == 1)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[10] = 1;
                    }
                    if (n == 5)
                    {
                        LikeMenu2[c - 2].SetActive(true);
                        LikeMenu2Open[9] = 1;
                    }
                }
                break;
            case 12://히로
                if (LikeMenu1[c - 1].activeSelf == false)
                {
                    if (n == 5)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[11] = 1;
                    }
                }
                break;
            case 13://디노
                if (LikeMenu1[c - 1].activeSelf == false)
                {
                    if (n == 6)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[12] = 1;
                    }
                }
                break;
            case 14://닥터 펭
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 4].activeSelf == false)
                {
                    if (n == 5)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[13] = 1;
                    }
                    if (n == 8)
                    {
                        LikeMenu2[c - 4].SetActive(true);
                        LikeMenu2Open[10] = 1;
                    }
                }
                break;
            case 15://롤렝드
                if (LikeMenu1[c - 1].activeSelf == false || LikeMenu2[c - 4].activeSelf == false)
                {
                    if (n == 3)
                    {
                        LikeMenu1[c - 1].SetActive(true);
                        LikeMenu1Open[14] = 1;
                    }
                    if (n == 4)
                    {
                        LikeMenu2[c - 4].SetActive(true);
                        LikeMenu2Open[11] = 1;
                    }
                }
                break;
        }
        SaveVisitorNoteInfo();
    }

    public void OpenNewSentence(int c)//친밀도 이벤트 완료 후 노트에 추가된 문장 보이기
    {
        switch(c) //c는 캐릭터넘버
        {
            case 1:
                WantToSay[0].SetActive(true);
                SecondSentence[0] = 1;
                break;
            case 5:
                WantToSay[1].SetActive(true);
                SecondSentence[1] = 1;
                break;
            case 7:
                WantToSay[2].SetActive(true);
                SecondSentence[2] = 1;
                break;
            case 9:
                WantToSay[3].SetActive(true);
                SecondSentence[3] = 1;
                break;
            case 10:
                WantToSay[4].SetActive(true);
                SecondSentence[4] = 1;
                break;
            case 11://무명이1
                WantToSay[5].SetActive(true);
                SecondSentence[5] = 1;
                break;
            case 12://무명이2
                WantToSay[6].SetActive(true);
                SecondSentence[6] = 1;
                break;
            case 15:
                WantToSay[7].SetActive(true);
                SecondSentence[7] = 1;
                break;
        }
        SaveVisitorNoteInfo();
    }

    public void NameInfoOpen(string name)//무명이 이름 정보 활성화
    {
        Name.text = name;
        Name.gameObject.SetActive(true);
        SaveVisitorNoteInfo();
    }

    int rpbClick = 0; //다시보기 버튼 클릭되면 1

    public void ShowRePlayView()//다시보기 버튼을 눌렀을 때
    {
        if(rpbClick == 0)//다시보기 버튼을 클릭하지 않은 상태면
        {
            rpbClick = 1;
            RePlayView[pageNum - 1].SetActive(true);//다시보기 창 활성화
        }
        else if(rpbClick == 1)//버튼을 클릭한 상태면
        {
            RePlayView[pageNum - 1].SetActive(false);//다시보기 창 비활성화
            rpbClick = 0;
        }
        
    }

    public void ClickReplayFirstMeeting()//첫만남버튼을 클릭한 경우
    {        
        switch(pageNum)//현재 페이지 넘버에 따라 텍스트 다르게 하기
        {
            case 1://도리인 경우
                whichStory.text = "도리와의 첫 만남을 회상할까요?";
                break;
            case 2:
                whichStory.text = "붕붕과의 첫 만남을 회상할까요?";
                break;
            case 3:
                whichStory.text = "빵빵과의 첫 만남을 회상할까요?";
                break;
            case 4:
                whichStory.text = "개나리와의 첫 만남을 회상할까요?";
                break;
            case 5:
                whichStory.text = "또롱과의 첫 만남을 회상할까요?";
                break;
            case 6:
                whichStory.text = "도로시와의 첫 만남을 회상할까요?";
                break;
            case 7:
                whichStory.text = "루루와의 첫 만남을 회상할까요?";
                break;
            case 8:
                whichStory.text = "샌디와의 첫 만남을 회상할까요?";
                break;
            case 9:
                whichStory.text = "친구와의 첫 만남을 회상할까요?";
                break;
            case 10:
                whichStory.text = "찰스와의 첫 만남을 회상할까요?";
                break;
            case 11:
                whichStory.text = SystemManager.instance.GetNameForNameless() + "와(과)의\n첫 만남을 회상할까요?";
                break;
            case 12:
                whichStory.text = "히로&디노와의\n첫 만남을 회상할까요?";
                break;
            case 13:
                whichStory.text = "닥터 펭과의\n첫 만남을 회상할까요?";
                break;
            case 14:
                whichStory.text = "롤렝드와의 첫 만남을 회상할까요?";
                break;
        }
        fmRP = pageNum;
        rePlayMessage.SetActive(true);
    }

    public int replayOn = 0; //다시보기 확정일 때 1, 다시보기 끝나면 2, 끝나고 1초 후 0으로 초기화
    public void RePlaySenario()//다시보기 메세지창에서 네 를 눌렀을 때
    {
        replayOn = 1;
        rePlayMessage.SetActive(false);//메세지창 비활성화
        ClickNoteCloseButton();//노트 내리기, 페이지 정보는 그대로
        Invoke("WhichSenario", 0.2f);//0.2초 후 시나리오 시작, 오디오 페이드아웃 문제 때문
    }  


    void WhichSenario()//누구 시나리오 다시보기인지 구분
    {
        if (fmRP != 0)//첫 만남 다시보기일 때
        {
            Dialogue.instance.CharacterDC[fmRP] = 0;//캐릭터DC를 0으로 만들고
            if (fmRP != 14)
            {
                GameScript1.instance.CharacterStart(fmRP);
            }
            else if (fmRP == 14)
            {
                GameScript1.instance.GrandfatherStart();
            }
        }
        else if (evRP != 0) //이벤트 다시보기
        {
            if (evRP <= 10)//10까지
            {
                Dialogue.instance.CharacterDC[evRP] = 1;//캐릭터DC를 1로 만들고
                GameScript1.instance.CharacterStart(evRP);
            }
            else if (evRP == 11)//찰스2 이벤트
            {
                Dialogue.instance.CharacterDC[10] = 2;
                GameScript1.instance.CharacterStart(10);
            }
            else if (evRP == 12)//무명이1 이벤트
            {
                Dialogue.instance.CharacterDC[11] = 1;
                GameScript1.instance.CharacterStart(11);
            }
            else if (evRP == 13)//무명이2 이벤트
            {
                Dialogue.instance.CharacterDC[11] = 2;
                GameScript1.instance.CharacterStart(11);
            }
            else //14부터
            {
                Dialogue.instance.CharacterDC[evRP - 2] = 1;//캐릭터DC를 1로 만들고
                int n = evRP - 2;
                if (evRP == 16)
                {
                    GameScript1.instance.GrandfatherStart();
                }
                else
                {
                    GameScript1.instance.CharacterStart(n);
                }
            }
        }
    }

    public void ClickSpecialMenuRePlay() //특별 메뉴 다시보기 버튼 눌렀을 때
    {
        switch (pageNum)//현재 페이지 넘버에 따라 보여주기
        {
            case 1://도리
                menuName.text = "'꽃사탕'";//메뉴 이름 설정
                whichCharacter.text = "도리를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[0].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[0].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[0].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 65); //이미지에 따라 위치 조정
                break;
            case 2:
                menuName.text = "'기름에이드'";
                whichCharacter.text = "붕붕을 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[1].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[1].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[1].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20); //이미지에 따라 위치 조정
                break;
            case 3:
                menuName.text = "'하트잼'";
                whichCharacter.text = "빵빵을 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[2].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[2].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[2].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50); //이미지에 따라 위치 조정
                break;
            case 4:
                menuName.text = "'당근케이크'";
                whichCharacter.text = "개나리를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[3].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[3].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[3].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 60); //이미지에 따라 위치 조정
                break;
            case 6:
                menuName.text = "'장미컵케이크'";
                whichCharacter.text = "도로시를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[4].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[4].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[4].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30); //이미지에 따라 위치 조정
                break;
            case 7:
                menuName.text = "'수제쿠키세트'";
                whichCharacter.text = "루루를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[5].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[5].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[5].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50); //이미지에 따라 위치 조정
                break;
            case 8:
                menuName.text = "'구름솜사탕과 바다에이드'";
                whichCharacter.text = "샌디를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[6].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[6].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[6].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20); //이미지에 따라 위치 조정
                break;
            case 9:
                menuName.text = "'약사탕과 뼈다귀쿠키'";
                whichCharacter.text = "친구를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[7].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[7].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[7].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 60); //이미지에 따라 위치 조정
                break;
            case 11:
                menuName.text = "'밀크초콜릿 볼'";
                whichCharacter.text = SystemManager.instance.GetNameForNameless() + "을(를) 위한 특별 메뉴!";
                Menu.instance.SpecialMenu[8].GetComponent<Image>().sprite = Menu.instance.NonameDessert.sprite;
                menuImage.sprite = Menu.instance.SpecialMenu[8].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[8].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[8].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50); //이미지에 따라 위치 조정
                break;
            case 12:
                menuName.text = "'푸른 별푸딩과 용암 핫초코'";
                whichCharacter.text = "히로와 디노를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[9].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[9].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[9].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50); //이미지에 따라 위치 조정
                break;
            case 13:
                menuName.text = "'태양에이드'";
                whichCharacter.text = "닥터 펭을 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[10].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[10].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[10].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20); //이미지에 따라 위치 조정
                break;
            case 14:
                menuName.text = "'미소의 무지개떡'";
                whichCharacter.text = "롤렝드를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.SpecialMenu[11].GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.SpecialMenu[11].GetComponent<RectTransform>().rect.width, Menu.instance.SpecialMenu[11].GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 70); //이미지에 따라 위치 조정
                break;
        }
        showSpecialMenu.SetActive(true);
    }
    
    public void ClickNoRePlay()//다시보기 메세지창에서 아니요를 눌렀을 때
    {
        rePlayMessage.SetActive(false);
        if(fmRP != 0)
        {
            fmRP = 0;
        }
        if(evRP != 0)
        {
            evRP = 0;
        }
    }

    public void ClickMenuRePlayClose()//스메셜 메뉴 다시보기 창 닫기 버튼 눌렀을 때
    {
        showSpecialMenu.SetActive(false);
    }

    public bool SaveVisitorNoteInfo()
    {
        bool result = false;
        try
        {
            PlayerPrefs.SetInt("OpenPage", openPage); //현재까지 오픈된 노트 페이지 수 저장

            string strArr = ""; // 문자열 생성
            for (int i = 0; i < LikeMenu1Open.Length; i++) // 첫번째 좋아하는 메뉴 
            {
                strArr = strArr + LikeMenu1Open[i];
                if (i < LikeMenu1Open.Length - 1) // 최대 길이의 -1까지만 ,를 저장
                {
                    strArr = strArr + ",";
                }
            }
            PlayerPrefs.SetString("LikeMenu1Open", strArr); 


            string strArr1 = ""; // 문자열 생성
            for (int i = 0; i < LikeMenu2Open.Length; i++) // 두번째 좋아하는 메뉴 
            {
                strArr1 = strArr1 + LikeMenu2Open[i];
                if (i < LikeMenu2Open.Length - 1) // 최대 길이의 -1까지만 ,를 저장
                {
                    strArr1 = strArr1 + ",";
                }
            }
            PlayerPrefs.SetString("LikeMenu2Open", strArr1); // PlyerPrefs에 문자열 형태로 저장


            string strArr2 = ""; // 문자열 생성
            for (int i = 0; i < SecondSentence.Length; i++) // 두번째 문ㅈ
            {
                strArr2 = strArr2 + SecondSentence[i];
                if (i < SecondSentence.Length - 1) // 최대 길이의 -1까지만 ,를 저장
                {
                    strArr2 = strArr2 + ",";
                }
            }
            PlayerPrefs.SetString("SecondSentence", strArr2); // PlyerPrefs에 문자열 형태로 저장


            string strArr3 = ""; // 문자열 생성
            for (int i = 0; i < FriendshipInfo.Length; i++)
            {
                strArr3 = strArr3 + FriendshipInfo[i];
                if (i < FriendshipInfo.Length - 1) // 최대 길이의 -1까지만 ,를 저장
                {
                    strArr3 = strArr3 + ",";
                }
            }
            PlayerPrefs.SetString("FriendshipInfo", strArr3); // PlyerPrefs에 문자열 형태로 저장


            PlayerPrefs.SetInt("RePlayOn", replayOn);//다시보기 정보 저장, 다시보기 중 앱 종료했을 때를 대비
            PlayerPrefs.SetInt("FMRP", fmRP);
            PlayerPrefs.SetInt("EVRP", evRP);

            PlayerPrefs.Save(); //세이브
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveVisitorNoteInfo Failed (" + e.Message + ")");
        }
        return result;
    }

    public bool LoadVisitorNoteInfo() //게임 데이터 정보 불러옴
    {
        //Debug.Log("LoadDataInfo");
        bool result = false;
        
        try
        {
            if (PlayerPrefs.HasKey("OpenPage"))
            {
                openPage = PlayerPrefs.GetInt("OpenPage");
                if(openPage <= 4)//오픈된 페이지가 4 이하면
                {
                    for (int i = 1; i < openPage; i++)//오픈된 페이지까지 페이지 버튼 활성화
                    {
                        page[i].SetActive(true);
                    }
                }
                else if(openPage >= 5)//오픈된 페이지가 5 이상이면
                {
                    for (int i = 1; i < 4; i++)//4페이지까지 활성화하고
                    {
                        page[i].SetActive(true);
                    }
                    nextPageButton.SetActive(true); //다음페이지 버튼 활성화

                    for(int z = 0; z < openPage - 4; z++)//5페이지부터 열린 페이지 확인
                    {
                        NextPageOpen[z] = 1;
                    }
                }
            }

            if (PlayerPrefs.HasKey("LikeMenu1Open"))
            {
                string[] dataArr = PlayerPrefs.GetString("LikeMenu1Open").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

                for (int i = 0; i < dataArr.Length; i++)
                {
                    LikeMenu1Open[i] = System.Convert.ToInt32(dataArr[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장

                    if (LikeMenu1Open[i] == 1)
                    {
                        LikeMenu1[i].SetActive(true);
                    }
                }
            }

            if (PlayerPrefs.HasKey("LikeMenu2Open"))
            {
                string[] dataArr1 = PlayerPrefs.GetString("LikeMenu2Open").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

                for (int i = 0; i < dataArr1.Length; i++)
                {
                    LikeMenu2Open[i] = System.Convert.ToInt32(dataArr1[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장

                    if (LikeMenu2Open[i] == 1)
                    {
                        LikeMenu2[i].SetActive(true);
                    }
                }
            }

            if (PlayerPrefs.HasKey("SecondSentence"))
            {
                string[] dataArr2 = PlayerPrefs.GetString("SecondSentence").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

                for (int i = 0; i < dataArr2.Length; i++)
                {
                    SecondSentence[i] = System.Convert.ToInt32(dataArr2[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장

                    if (SecondSentence[i] == 1)
                    {
                        WantToSay[i].SetActive(true);
                    }
                }
            }

            if (PlayerPrefs.HasKey("FriendshipInfo"))
            {
                string[] dataArr3 = PlayerPrefs.GetString("FriendshipInfo").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

                for (int i = 0; i < dataArr3.Length; i++)
                {
                    FriendshipInfo[i] = System.Convert.ToInt32(dataArr3[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장
                    if (i <= 4)//도리부터 도로시까지
                    {
                        if(FriendshipInfo[i] >= 10)//서빙횟수가 10보다 많거나 같으면
                        {
                            friendshipGauge[i].GetComponent<Image>().fillAmount = 1f;
                        }
                        else
                        {
                            friendshipGauge[i].GetComponent<Image>().fillAmount = (float)(FriendshipInfo[i] * 0.1); //0.1을 곱한 값을 친밀도 게이지에 대입
                        }
                    }
                    else if((i >= 5 && i <= 8) || i == 10 || i == 11)//루루부터 찰스까지, 히로디노와 닥터펭도
                    {
                        if (FriendshipInfo[i] >= 15)//서빙횟수가 15보다 많거나 같으면
                        {
                            friendshipGauge[i].GetComponent<Image>().fillAmount = 1f;
                        }
                        else
                        {
                            friendshipGauge[i].GetComponent<Image>().fillAmount = (float)(FriendshipInfo[i] * 0.067f); //0.067을 곱한 값을 친밀도 게이지에 대입
                        }
                    }
                    else if(i == 9 || i == 12)//무명이거나 롤렝드
                    {
                        if (FriendshipInfo[i] >= 20)//서빙횟수가 20보다 많거나 같으면
                        {
                            friendshipGauge[i].GetComponent<Image>().fillAmount = 1f;
                        }
                        else
                        {
                            friendshipGauge[i].GetComponent<Image>().fillAmount = (float)(FriendshipInfo[i] * 0.05f); //0.05을 곱한 값을 친밀도 게이지에 대입
                        }
                    }
                }
            }

            if(PlayerPrefs.HasKey("NameForNameless"))
            {
                string n = PlayerPrefs.GetString("NameForNameless");
                NameInfoOpen(n);
            }

            if (PlayerPrefs.HasKey("RePlayOn"))//다시 보기 정보 불러오기
            {
                replayOn = PlayerPrefs.GetInt("RePlayOn");
                fmRP = PlayerPrefs.GetInt("FMRP");
                evRP = PlayerPrefs.GetInt("EVRP");
            }

            if(replayOn != 0)//다시보기 중일 때 종료한 거면 캐릭터DC 초기화
            {
                replayOn = 0;
                if (fmRP != 0)//첫 만남 다시보기일 때
                {
                    Dialogue.instance.CharacterDC[fmRP] = 3;
                    fmRP = 0;
                }
                else if (evRP != 0) //이벤트 다시보기일 때
                {
                    if (evRP <= 10)//10까지
                    {
                        Dialogue.instance.CharacterDC[evRP] = 3;
                    }
                    else if (evRP == 11)//찰스2 이벤트
                    {
                        Dialogue.instance.CharacterDC[10] = 3;
                    }
                    else if (evRP == 12)//무명이1 이벤트
                    {
                        Dialogue.instance.CharacterDC[11] = 3;
                    }
                    else if (evRP == 13)//무명이2 이벤트
                    {
                        Dialogue.instance.CharacterDC[11] = 3;
                    }
                    else //14부터
                    {
                        Dialogue.instance.CharacterDC[evRP - 2] = 3;
                    }
                    evRP = 0;
                }
            }
            for(int i = 1; i <= 14; i++)
            {
                if (i != 0 && Dialogue.instance.CharacterDC[i] == 3)//제제가 아닌 캐릭터의 DC가 3이면, 시나리오가 끝났으면
                {
                    VisitorNote.instance.RePlayButton[i - 1].gameObject.SetActive(true);//다시보기 버튼 활성화
                    if (i == 1)//도리는 손님노트 이미지를 2번째 표정으로 바꿈
                    {
                        VisitorNote.instance.characterInfo[i - 1].GetComponent<Image>().sprite = CharacterList.instance.CharacterFaceList[i].face[1].GetComponent<Image>().sprite;
                    }
                    if(i == 11)
                    {
                        VisitorNote.instance.characterInfo[i - 1].GetComponent<Image>().sprite = CharacterList.instance.CharacterFaceList[i - 2].face[3].GetComponent<Image>().sprite;
                    }
                    if(i == 12 || i == 13)
                    {
                        VisitorNote.instance.characterInfo[i - 1].GetComponent<Image>().sprite = CharacterList.instance.CharacterFaceList[i - 2].face[0].GetComponent<Image>().sprite;
                    }
                    if(i == 14)
                    {
                        VisitorNote.instance.characterInfo[i - 1].GetComponent<Image>().sprite = GameScript1.instance.BigCharacter[17].GetComponent<Image>().sprite;
                    }
                }         
            }
           
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadVisitorNoteInfo Failed (" + e.Message + ")");
        }
        return result;
    }
}
