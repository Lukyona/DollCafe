using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class VisitorNote : MonoBehaviour 
{
    public static VisitorNote instance;

    #region 게임 내 오브젝트 변수
    [SerializeField] Animator buttonAnimator;
    [SerializeField] Animator noteAnimator;
    [SerializeField] GameObject[] characterInfos; //캐릭터 정보 배열
    [SerializeField] GameObject[] pages; //페이지 버튼 배열
    [SerializeField] GameObject nextPageButton; //다음 페이지 버튼
    [SerializeField] GameObject previousPageButton; //이전 페이지 버튼
    [SerializeField] GameObject pageText1; // "페이지" 텍스트
    [SerializeField] GameObject pageText2; // "페이지" 텍스트 (회전한 버전)
    [SerializeField] GameObject[] friendshipGauges; //친밀도 게이지 배열
    [SerializeField] GameObject[] favFirstMenu;//좋아하는 첫번째 메뉴 배열, 0도리~~~11히로, 12디노, 13닥터펭, 14롤렝드
    [SerializeField] GameObject[] favSecondMenu; //두번째 좋아하는 메뉴 배열, 0도리, 1 빵빵, 2개나리~~10닥터펭 11롤렝드,  붕붕이/히로디노만 없음
    [SerializeField] GameObject[] hiddenText;//친밀도 이벤트 달성 시 노트에 새롭게 보일 문장, 0도리 1루루 2친구 3찰스 4무명이1 5무명이2 6롤렝드
    [SerializeField] Text nameForNameless; //플레이어가 지어준 무명이 이름
    [SerializeField] Button[] replayButtons;//다시보기 버튼, 0도리
    [SerializeField] GameObject[] replayOptionWindows; //어떤 걸 다시 볼 지 고르는 창, 0도리
    [SerializeField] GameObject specialMenuWindow;//스메셜 메뉴 다시보기 창
    [SerializeField] Image menuImage; // 스페셜 메뉴 이미지
    [SerializeField] Text menuName; // 메뉴 이름
    [SerializeField] Text whichCharacter; //어느 캐릭터의 스페셜 메뉴인지
    [SerializeField] GameObject replayMessageWindow;//첫만남, 이벤트 다시보기 메세지창
    [SerializeField] Text replayMessageText; //누구의 첫만남/이벤트를 회상할까요? 라고 묻는 텍스트
    #endregion

    int pageNum = 1; //1페이지가 첫 페이지
    static int pageGroup = 1; //1-4페이지까지가 1, 5-8까지가 2, 9 - 12가 3, 13 - 14가 4

    int openedPages = 1;//열린 페이지, 도리가 기본으로 있으므로 1, 마지막이 14

    int[] favFirstMenuOpen = new int[15]; //노트의 좋아하는 첫번째 메뉴 정보가 열렸는지 확인하기 위한 배열
    int[] favSecondMenuOpen = new int[12]; //노트의 좋아하는 두번째 메뉴 정보가 열렸는지 확인하기 위한 배열

    int[] hiddenTextStates = new int[8]; //노트의 두번째 하고 싶은 말 정보가 열렸는지 확인

    int[] friendshipInfo = new int[13] { 0, 0, 0, 0, 0, 0,0,0,0,0,0,0,0}; //친밀도 게이지 정보(서빙 횟수) 배열
    
    int firstMeetID = 0; //첫만남 캐릭터별로 숫자 들어감
    int friendEventID = 0; //이벤트 캐릭터별로 숫자 들어감

    bool isreplayOptionWindowOpen = false; //다시보기 버튼 터치되면 true

    int replayState = 0; //다시보기 확정일 때 1, 다시보기 끝나면 2, 끝나고 1초 후 0으로 초기화

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void ShowVisitorNote() //손님 노트 버튼 눌렀을 때
    {
        if(!SystemManager.instance.IsUIOpen())
        {
            SystemManager.instance.SetUIOpen(true);
            SEManager.instance.PlayUITouchSound(); //효과음
            buttonAnimator.SetTrigger("VNButtonOut"); //버튼 위로 올라감       
            noteAnimator.SetTrigger("NoteUp"); //노트 올라옴
        }
    }

    public void CloseVisitorNote() //노트 닫기 버튼 눌렀을 때
    {
        SystemManager.instance.SetUIOpen(false);
        SEManager.instance.PlayUICloseSound(); //효과음
        noteAnimator.SetTrigger("NoteDown"); //노트 내려감
        buttonAnimator.SetTrigger("VNButtonIn"); //버튼 내려옴
        if(friendEventID == 0 && firstMeetID == 0)//다시보기 아닐 때만 
        {
            Invoke(nameof(InitNote), 0.5f);//0.5초 뒤 노트 정보 1페이지로 초기화
        }        
    }

    void InitNote()
    {
        if (isreplayOptionWindowOpen)//다시보기 창이 활성화되어있으면
        {
            ShowReplayOptions();
        }
        characterInfos[pageNum - 1].SetActive(false); // 이전 페이지의 캐릭터 정보를 안 보이게 하고
        pages[pageNum - 1].GetComponent<Button>().interactable = true; //이전 페이지의 버튼을 터치 가능하게 함
        if (pageGroup != 1)
        {
            SetNextPageButtonActive(true); //다음 페이지 버튼 활성화
            previousPageButton.SetActive(false);//이전 버튼 비활성화
            pageText2.SetActive(false);
            pageText1.SetActive(true);

            for (int i = 4; i < 14; i++)//5-14페이지 중 열려있는 페이지 비활성화
            {
                if (pages[i].activeSelf)//페이지가 열려있으면
                {
                    pages[i].SetActive(false);
                }
            }

            for (int i = 0; i < 4; i++) //1-4페이지 활성화
            {
                pages[i].SetActive(true);
            }
            pageGroup = 1;
        }
        pageNum = 1; //현재 페이지 넘버      
        characterInfos[pageNum - 1].SetActive(true); //현재 페이지의 캐릭터 정보를 보이게 함
        pages[pageNum - 1].GetComponent<Button>().interactable = false; //현재 페이지의 버튼 터치 불가능하게 함      
    }

    public void SetCharacterImage(int cNum, Sprite cSprite)
    {
        characterInfos[cNum - 1].GetComponent<Image>().sprite = cSprite;
    }
    
    public void OpenPage(int idx)
    {
        pages[idx].SetActive(true);
    }

    public void UpdateOpenedPages()
    {
        ++openedPages;
    }

    public void SetNextPageButtonActive(bool value)
    {
        nextPageButton.SetActive(value);
    }
    
    public void ActivateReplayButton(int idx)
    {
        replayButtons[idx].gameObject.SetActive(true);
    }

    public void ActivateReplayMessageWindow()
    {
        replayMessageWindow.gameObject.SetActive(true);
    }

    public void SetReplayMessageText(string text)
    {
        replayMessageText.text = text;
    }

    public int GetFriendshipInfo(int idx)
    {
        return friendshipInfo[idx];
    }

    public void SetReplayState(int value)
    {
        replayState = value;
    }

    public int GetReplayState()
    {
        return replayState;
    }

    public int GetFirstMeetID()
    {
        return firstMeetID;
    }

    public void SetFirstMeetID(int value)
    {
        firstMeetID = value;
    }

    public int GetFriendEventID()
    {
        return friendEventID;
    }

    public void SetFriendEventID(int value)
    {
        friendEventID = value;
    }

    public bool IsFriendshipGaugeFull(int idx)
    {
        return friendshipGauges[idx].GetComponent<Image>().fillAmount == 1f? true : false;
    }

    public void IncreaseFrinedshipGauge(int cNum) //친밀도 게이지 증가, n은 캐릭터 넘버
    {
        if(cNum != 0 && cNum != 13) // 디노 패스
        {
            int idx = -1;
            int maxNum = -1;
            switch(cNum)
            {
                case 1: //맥스가 10이므로 0.1씩 증가
                case 2:
                case 3:
                case 4:
                    idx = cNum -1;
                    maxNum = 10;
                    break;
                case 6:
                    idx = cNum -2;
                    maxNum = 10;             
                    break;
                case 7://맥스 15
                case 8:
                case 9:
                case 10:
                case 12://히로디노
                    idx = cNum - 2;
                    maxNum = 15;                     
                    break;
                case 14://닥터펭
                    idx = cNum - 3;
                    maxNum = 15; 
                    break;
                case 11: //맥스 20, 무명이
                    idx = cNum - 2;
                    maxNum = 20; 
                    break;
                case 15://롤렝드
                    idx = cNum - 3;
                    maxNum = 20; 
                    break;
            }
            
            if(idx == -1) return;
            
            if(friendshipGauges[idx].GetComponent<Image>().fillAmount != 1f)//친밀도 맥스가 아닐 경우
            {
                if(maxNum == 10)
                    friendshipGauges[idx].GetComponent<Image>().fillAmount += 0.1f;
                else if(maxNum == 15)
                    friendshipGauges[idx].GetComponent<Image>().fillAmount += 0.067f;
                else if(maxNum == 20)
                    friendshipGauges[idx].GetComponent<Image>().fillAmount += 0.05f;

                ++friendshipInfo[idx];
            }
            
            SaveVisitorNoteInfo();
        }       
    }

    public void TurnToPage(int pNum)
    {              
        if (1 <= pNum && pNum <= 4)
        {
            pageGroup = 1;
        }
        else if(5 <= pNum && pNum <= 8)
        {
            pageGroup = 2;
        }
        else if(9 <= pNum && pNum <= 12)
        {
            pageGroup = 3;
        }
        else if (pNum == 13 || pNum == 14)
        {
            pageGroup = 4;
        }
        if (isreplayOptionWindowOpen)//다시보기 창이 활성화되어있으면
        {
            ShowReplayOptions();
        }

        SEManager.instance.PlayNextPageSound(); //페이지 넘기는 효과음       
        characterInfos[pageNum - 1].SetActive(false); // 이전 페이지의 캐릭터 정보를 안 보이게 하고
        pages[pageNum - 1].GetComponent<Button>().interactable = true; //이전 페이지의 버튼을 터치 가능하게 함      

        pageNum = pNum;

        characterInfos[pageNum - 1].SetActive(true); //현재 페이지의 캐릭터 정보를 보이게 함
        pages[pageNum - 1].GetComponent<Button>().interactable = false; //현재 페이지의 버튼 터치 불가능하게 함
    }

    public void TouchNextPageButton() //다음 페이지 버튼 눌렀을 때
    {
        pageText1.SetActive(false);
        pageText2.SetActive(true);
        previousPageButton.SetActive(true); //이전 페이지 버튼 활성화

        if(pageGroup == 1)
        {
            for (int i = 0; i < 4; i++) //1-4페이지 비활성화
            {
                pages[i].SetActive(false);
            }

            for(int i = 4; i < 8; i++) //5-8페이지 중 열려있는 페이지 활성화
            {
                if(openedPages - i > 0)//페이지가 열려있으면(해당 캐릭터가 방문했었으면)
                {
                    pages[i].SetActive(true);
                }
            }
            
            if(openedPages < 9) //9페이지가 열려있지 않으면 다음 페이지 버튼 비활성화
            {
                SetNextPageButtonActive(false);
            }
            pageGroup = 2;
        }
        else if (pageGroup == 2)
        {
            for (int i = 4; i < 8; i++) //5-8페이지 비활성화
            {
                pages[i].SetActive(false);
            }

            for (int i = 8; i < 12; i++) //9-12페이지 중
            {
                if (openedPages - i > 0)//페이지가 열려있으면
                {
                    pages[i].SetActive(true);
                }
            }

            if (openedPages < 13) //13페이지가 열려있지 않으면
            {
                SetNextPageButtonActive(false);//다음 페이지 버튼 비활성화
            }
            pageGroup = 3;
        }
        else if (pageGroup == 3)
        {
            for (int i = 8; i < 12; i++) 
            {
                pages[i].SetActive(false);
            }
            if(openedPages >= 13) //13페이지 열려있으면 페이지 활성화
            {
                pages[12].SetActive(true);
            }
            if (openedPages == 14)//14페이지 열려있으면 페이지 활성화
            {
                pages[13].SetActive(true);
            }

            pageGroup = 4;
            SetNextPageButtonActive(false);//다음 페이지 버튼은 비활성화, 노트의 마지막 부분
        }
    }
   
    public void TouchPreviousPageButton() //이전 페이지 버튼 터치 
    {
        SetNextPageButtonActive(true); //다음 페이지 버튼 활성화

        if (pageGroup == 2)
        {
            previousPageButton.SetActive(false);//이전 버튼 비활성화
            pageText2.SetActive(false);
            pageText1.SetActive(true);
           
            for (int i = 4; i < 8; i++)//5-8페이지 중 열려있는 페이지 비활성화
            {
                if (openedPages - i != 0)//페이지가 열려있으면
                {
                    pages[i].SetActive(false);
                }
            }

            for (int i = 0; i < 4; i++) //1-4페이지 활성화
            {
                pages[i].SetActive(true);
            }
            
            pageGroup = 1;
        }
        else if (pageGroup == 3)
        {
            for (int i = 8; i < 12; i++)
            {
                if (openedPages - i != 0)//페이지가 열려있으면
                {
                    pages[i].SetActive(false);
                }
            }

            for (int i = 4; i < 8; i++)
            {
                pages[i].SetActive(true);
            }

            pageGroup = 2;
        }
        else if (pageGroup == 4)
        {
            if (openedPages == 13)
            {
                pages[12].SetActive(false);
            }
            if (openedPages == 14)
            {
                pages[13].SetActive(false);
            }

            for (int i = 8; i < 12; i++)
            {
                pages[i].SetActive(true);
            }

            pageGroup = 3;
        }
    }

    public void CheckMenuMatch(int cNum, int menuNum)//캐릭터가 원하는 메뉴를 맞췄을 때, 손님노트의 좋아하는 것 정보 오픈
    {
        switch(cNum)//c는 캐릭터넘버
        {
            case 1://도리
                if(favFirstMenu[0].activeSelf == false || favSecondMenu[0].activeSelf == false)
                {
                    if (menuNum == 1)
                    {
                        favFirstMenu[0].SetActive(true);
                        favFirstMenuOpen[0] = 1;
                    }
                    if (menuNum == 4)
                    {
                        favSecondMenu[0].SetActive(true);
                        favSecondMenuOpen[0] = 1;
                    }
                }              
                break;
            case 2:
                if (favFirstMenu[1].activeSelf == false)
                {
                    if (menuNum == 2)
                    {
                        favFirstMenu[1].SetActive(true);
                        favFirstMenuOpen[1] = 1;
                    }
                }
                break;
            case 3:
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 2].activeSelf == false)
                {
                    if (menuNum == 2)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[2] = 1;
                    }
                    if (menuNum == 6)
                    {
                        favSecondMenu[cNum - 2].SetActive(true);
                        favSecondMenuOpen[1] = 1;
                    }
                }
                break;
            case 4:
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 2].activeSelf == false)
                {
                    if (menuNum == 3)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[3] = 1;
                    }
                    if (menuNum == 4)
                    {
                        favSecondMenu[cNum - 2].SetActive(true);
                        favSecondMenuOpen[2] = 1;
                    }
                }
                break;
            case 5:
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 2].activeSelf == false)
                {
                    if (menuNum == 1)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[4] = 1;
                    }
                    if (menuNum == 6 && favSecondMenu[cNum - 2].activeSelf == false)
                    {
                        SystemManager.instance.BeginDialogue(5);//또롱이 이벤트 시작
                        CharacterManager.instance.SetCurrentEventState(5);
                        favSecondMenu[cNum - 2].SetActive(true);
                        favSecondMenuOpen[3] = 1;
                    }
                }
                break;
            case 6:
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 2].activeSelf == false)
                {
                    if (menuNum == 3)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[5] = 1;
                    }
                    if (menuNum == 8)
                    {
                        favSecondMenu[cNum - 2].SetActive(true);
                        favSecondMenuOpen[4] = 1;
                    }
                }
                break;
            case 7:
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 2].activeSelf == false)
                {
                    if (menuNum == 3)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[6] = 1;
                    }
                    if (menuNum == 7)
                    {
                        favSecondMenu[cNum - 2].SetActive(true);
                        favSecondMenuOpen[5] = 1;
                    }
                }
                break;
            case 8:
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 2].activeSelf == false)
                {
                    if (menuNum == 4)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[7] = 1;
                    }
                    if (menuNum == 7)
                    {
                        favSecondMenu[cNum - 2].SetActive(true);
                        favSecondMenuOpen[6] = 1;
                    }
                }
                break;
            case 9:
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 2].activeSelf == false)
                {
                    if (menuNum == 2)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[8] = 1;
                    }
                    if (menuNum == 8)
                    {
                        favSecondMenu[cNum - 2].SetActive(true);
                        favSecondMenuOpen[7] = 1;
                    }
                }
                break;
            case 10:
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 2].activeSelf == false)
                {
                    if (menuNum == 5)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[9] = 1;
                    }
                    if (menuNum == 6)
                    {
                        favSecondMenu[cNum - 2].SetActive(true);
                        favSecondMenuOpen[8] = 1;
                    }
                }
                break;
            case 11:
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 2].activeSelf == false)
                {
                    if (menuNum == 1)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[10] = 1;
                    }
                    if (menuNum == 5)
                    {
                        favSecondMenu[cNum - 2].SetActive(true);
                        favSecondMenuOpen[9] = 1;
                    }
                }
                break;
            case 12://히로
                if (favFirstMenu[cNum - 1].activeSelf == false)
                {
                    if (menuNum == 5)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[11] = 1;
                    }
                }
                break;
            case 13://디노
                if (favFirstMenu[cNum - 1].activeSelf == false)
                {
                    if (menuNum == 6)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[12] = 1;
                    }
                }
                break;
            case 14://닥터 펭
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 4].activeSelf == false)
                {
                    if (menuNum == 5)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[13] = 1;
                    }
                    if (menuNum == 8)
                    {
                        favSecondMenu[cNum - 4].SetActive(true);
                        favSecondMenuOpen[10] = 1;
                    }
                }
                break;
            case 15://롤렝드
                if (favFirstMenu[cNum - 1].activeSelf == false || favSecondMenu[cNum - 4].activeSelf == false)
                {
                    if (menuNum == 3)
                    {
                        favFirstMenu[cNum - 1].SetActive(true);
                        favFirstMenuOpen[14] = 1;
                    }
                    if (menuNum == 4)
                    {
                        favSecondMenu[cNum - 4].SetActive(true);
                        favSecondMenuOpen[11] = 1;
                    }
                }
                break;
        }
        SaveVisitorNoteInfo();
    }

    public void OpenHiddenText(int cNum)//친밀도 이벤트 완료 후 노트에 추가된 문장 보이기
    {
        switch(cNum) 
        {
            case 1:
                hiddenText[0].SetActive(true);
                hiddenTextStates[0] = 1;
                break;
            case 5:
                hiddenText[1].SetActive(true);
                hiddenTextStates[1] = 1;
                break;
            case 7:
                hiddenText[2].SetActive(true);
                hiddenTextStates[2] = 1;
                break;
            case 9:
                hiddenText[3].SetActive(true);
                hiddenTextStates[3] = 1;
                break;
            case 10:
                hiddenText[4].SetActive(true);
                hiddenTextStates[4] = 1;
                break;
            case 11://무명이1
                hiddenText[5].SetActive(true);
                hiddenTextStates[5] = 1;
                break;
            case 12://무명이2
                hiddenText[6].SetActive(true);
                hiddenTextStates[6] = 1;
                break;
            case 15:
                hiddenText[7].SetActive(true);
                hiddenTextStates[7] = 1;
                break;
        }
        SaveVisitorNoteInfo();
    }

    public void OpenNameForNameless(string name)//무명이 이름 정보 활성화
    {
        nameForNameless.text = name;
        nameForNameless.gameObject.SetActive(true);
        SaveVisitorNoteInfo();
    }

    public void ShowReplayOptions()//다시보기 버튼을 눌렀을 때
    {
        if(!isreplayOptionWindowOpen)//다시보기 버튼을 클릭하지 않은 상태면
        {
            isreplayOptionWindowOpen = true;
            replayOptionWindows[pageNum - 1].SetActive(true);//다시보기 창 활성화
        }
        else if(isreplayOptionWindowOpen)//버튼을 클릭한 상태면
        {
            replayOptionWindows[pageNum - 1].SetActive(false);//다시보기 창 비활성화
            isreplayOptionWindowOpen = false;
        }  
    }

    public void TouchReplayFirstMeeting()//첫만남버튼을 클릭한 경우
    {        
        string text = "";
        switch(pageNum)//현재 페이지 넘버에 따라 텍스트 다르게 하기
        {
            case 1://도리인 경우
                text = "도리와의 ";
                break;
            case 2:
                text = "붕붕과의 ";
                break;
            case 3:
                text = "빵빵과의 ";
                break;
            case 4:
                text = "개나리와의 ";
                break;
            case 5:
                text = "또롱과의 ";
                break;
            case 6:
                text = "도로시와의 ";
                break;
            case 7:
                text = "루루와의 ";
                break;
            case 8:
                text = "샌디와의 ";
                break;
            case 9:
                text = "친구와의 ";
                break;
            case 10:
                text = "찰스와의 ";
                break;
            case 11:
                text = SystemManager.instance.GetNameForNameless() + "와(과)의\n";
                break;
            case 12:
                text = "히로&디노와의\n";
                break;
            case 13:
                text = "닥터 펭과의\n";
                break;
            case 14:
                text = "롤렝드와의 ";
                break;
        }
        firstMeetID = pageNum;
        SetReplayMessageText(text + "첫 만남을 회상할까요?");
        replayMessageWindow.SetActive(true);
    }

    public void AgreeToWatchReplay()//다시보기 메세지창에서 네 를 눌렀을 때
    {
        replayState = 1;
        replayMessageWindow.SetActive(false);//메세지창 비활성화
        CloseVisitorNote();//노트 내리기, 페이지 정보는 그대로
        Invoke(nameof(CheckAndBeginDialogue), 0.2f);//0.2초 후 시나리오 시작, 오디오 페이드아웃 문제 때문
    }  

    void CheckAndBeginDialogue()//누구 시나리오 다시보기인지 구분
    {
        if (firstMeetID != 0)//첫 만남 다시보기일 때
        {
            Dialogue.instance.SetCharacterDC(firstMeetID, 0);//캐릭터DC를 0으로 만들고
            if (firstMeetID != 14)
            {
                SystemManager.instance.BeginDialogue(firstMeetID);
            }
            else if (firstMeetID == 14)
            {
                SystemManager.instance.BeginDialogue(14);
            }
        }
        else if (friendEventID != 0) //이벤트 다시보기
        {
            if (friendEventID <= 10)//10까지
            {
                Dialogue.instance.SetCharacterDC(friendEventID, 1);//캐릭터DC를 1로 만들고
                SystemManager.instance.BeginDialogue(friendEventID);
            }
            else if (friendEventID == 11)//찰스2 이벤트
            {
                Dialogue.instance.SetCharacterDC(10, 2);
                SystemManager.instance.BeginDialogue(10);
            }
            else if (friendEventID == 12)//무명이1 이벤트
            {
                Dialogue.instance.SetCharacterDC(11, 1);
                SystemManager.instance.BeginDialogue(11);
            }
            else if (friendEventID == 13)//무명이2 이벤트
            {
                Dialogue.instance.SetCharacterDC(11, 2);
                SystemManager.instance.BeginDialogue(11);
            }
            else //14부터
            {
                Dialogue.instance.SetCharacterDC(friendEventID - 2, 1);//캐릭터DC를 1로 만들고
                int n = friendEventID - 2;
                if (friendEventID == 16)
                {
                    SystemManager.instance.BeginDialogue(14);
                }
                else
                {
                    SystemManager.instance.BeginDialogue(n);
                }
            }
        }
    }

    public void DisagreeToWatchReplay()//다시보기 메세지창에서 아니요를 눌렀을 때
    {
        replayMessageWindow.SetActive(false);
        if(firstMeetID != 0)
        {
            firstMeetID = 0;
        }
        if(friendEventID != 0)
        {
            friendEventID = 0;
        }
    }

    public void TouchSpecialMenuRePlay() //특별 메뉴 다시보기 버튼 눌렀을 때
    {
        switch (pageNum)//현재 페이지 넘버에 따라 보여주기
        {
            case 1://도리
                menuName.text = "'꽃사탕'";//메뉴 이름 설정
                whichCharacter.text = "도리를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(0).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(0).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(0).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 65); //이미지에 따라 위치 조정
                break;
            case 2:
                menuName.text = "'기름에이드'";
                whichCharacter.text = "붕붕을 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(1).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(1).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(1).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20); //이미지에 따라 위치 조정
                break;
            case 3:
                menuName.text = "'하트잼'";
                whichCharacter.text = "빵빵을 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(2).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(2).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(2).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50); //이미지에 따라 위치 조정
                break;
            case 4:
                menuName.text = "'당근케이크'";
                whichCharacter.text = "개나리를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(3).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(3).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(3).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 60); //이미지에 따라 위치 조정
                break;
            case 6:
                menuName.text = "'장미컵케이크'";
                whichCharacter.text = "도로시를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(4).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(4).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(4).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30); //이미지에 따라 위치 조정
                break;
            case 7:
                menuName.text = "'수제쿠키세트'";
                whichCharacter.text = "루루를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(5).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(5).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(5).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50); //이미지에 따라 위치 조정
                break;
            case 8:
                menuName.text = "'구름솜사탕과 바다에이드'";
                whichCharacter.text = "샌디를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(6).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(6).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(6).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20); //이미지에 따라 위치 조정
                break;
            case 9:
                menuName.text = "'약사탕과 뼈다귀쿠키'";
                whichCharacter.text = "친구를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(7).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(7).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(7).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 60); //이미지에 따라 위치 조정
                break;
            case 11:
                menuName.text = "'밀크초콜릿 볼'";
                whichCharacter.text = SystemManager.instance.GetNameForNameless() + "을(를) 위한 특별 메뉴!";
                Menu.instance.GetSpecialMenu(8).GetComponent<Image>().sprite = Menu.instance.GetNamelessDessertSprite();
                menuImage.sprite = Menu.instance.GetSpecialMenu(8).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(8).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(8).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50); //이미지에 따라 위치 조정
                break;
            case 12:
                menuName.text = "'푸른 별푸딩과 용암 핫초코'";
                whichCharacter.text = "히로와 디노를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(9).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(9).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(9).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50); //이미지에 따라 위치 조정
                break;
            case 13:
                menuName.text = "'태양에이드'";
                whichCharacter.text = "닥터 펭을 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(10).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(10).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(10).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20); //이미지에 따라 위치 조정
                break;
            case 14:
                menuName.text = "'미소의 무지개떡'";
                whichCharacter.text = "롤렝드를 위한 특별 메뉴!";
                menuImage.sprite = Menu.instance.GetSpecialMenu(11).GetComponent<Image>().sprite; //이미지 설정
                menuImage.GetComponent<RectTransform>().sizeDelta = new Vector2(Menu.instance.GetSpecialMenu(11).GetComponent<RectTransform>().rect.width, Menu.instance.GetSpecialMenu(11).GetComponent<RectTransform>().rect.height);
                menuImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 70); //이미지에 따라 위치 조정
                break;
        }
        specialMenuWindow.SetActive(true);
    }

    public void CloseSpecialMenuWindow()//스메셜 메뉴 다시보기 창 닫기 버튼 눌렀을 때
    {
        specialMenuWindow.SetActive(false);
    }

    public void SaveVisitorNoteInfo()
    {
        try
        {
            PlayerPrefs.SetInt("OpenedPages", openedPages); //현재까지 오픈된 노트 페이지 수 저장

            string strArr = ""; // 문자열 생성
            for (int i = 0; i < favFirstMenuOpen.Length; i++) // 첫번째 좋아하는 메뉴 
            {
                strArr = strArr + favFirstMenuOpen[i];
                if (i < favFirstMenuOpen.Length - 1) // 최대 길이의 -1까지만 ,를 저장
                {
                    strArr = strArr + ",";
                }
            }
            PlayerPrefs.SetString("favFirstMenuOpen", strArr); 

            string strArr1 = ""; // 문자열 생성
            for (int i = 0; i < favSecondMenuOpen.Length; i++) // 두번째 좋아하는 메뉴 
            {
                strArr1 = strArr1 + favSecondMenuOpen[i];
                if (i < favSecondMenuOpen.Length - 1) // 최대 길이의 -1까지만 ,를 저장
                {
                    strArr1 = strArr1 + ",";
                }
            }
            PlayerPrefs.SetString("favSecondMenuOpen", strArr1); // PlyerPrefs에 문자열 형태로 저장

            string strArr2 = ""; // 문자열 생성
            for (int i = 0; i < hiddenTextStates.Length; i++) // 두번째 문장
            {
                strArr2 = strArr2 + hiddenTextStates[i];
                if (i < hiddenTextStates.Length - 1) // 최대 길이의 -1까지만 ,를 저장
                {
                    strArr2 = strArr2 + ",";
                }
            }
            PlayerPrefs.SetString("HiddenTextStates", strArr2); // PlyerPrefs에 문자열 형태로 저장

            string strArr3 = ""; // 문자열 생성
            for (int i = 0; i < friendshipInfo.Length; i++)
            {
                strArr3 = strArr3 + friendshipInfo[i];
                if (i < friendshipInfo.Length - 1) // 최대 길이의 -1까지만 ,를 저장
                {
                    strArr3 = strArr3 + ",";
                }
            }
            PlayerPrefs.SetString("FriendshipInfo", strArr3); // PlyerPrefs에 문자열 형태로 저장
            PlayerPrefs.SetInt("ReplayState", replayState);//다시보기 정보 저장, 다시보기 중 앱 종료했을 때를 대비
            PlayerPrefs.SetInt("FirstMeetID", firstMeetID);
            PlayerPrefs.SetInt("FriendEventID", friendEventID);

            PlayerPrefs.Save(); //세이브
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveVisitorNoteInfo Failed (" + e.Message + ")");
        }
    }

    public void LoadVisitorNoteInfo() //게임 데이터 정보 불러옴
    {
        //Debug.Log("LoadDataInfo");
        try
        {
            if (PlayerPrefs.HasKey("OpenedPages"))
            {
                openedPages = PlayerPrefs.GetInt("OpenedPages");
                int max = 0;

                if(openedPages <= 4)//오픈된 페이지가 4 이하면
                {
                    max = openedPages;
                }
                else if(openedPages >= 5)//오픈된 페이지가 5 이상이면
                {
                    max = 4;
                    SetNextPageButtonActive(true); //다음페이지 버튼 활성화
                }

                for (int i = 1; i < max; i++)//오픈된 페이지까지 페이지 버튼 활성화
                {
                    pages[i].SetActive(true);
                }
            }

            if (PlayerPrefs.HasKey("favFirstMenuOpen"))
            {
                string[] dataArr = PlayerPrefs.GetString("favFirstMenuOpen").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

                for (int i = 0; i < dataArr.Length; i++)
                {
                    favFirstMenuOpen[i] = System.Convert.ToInt32(dataArr[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장

                    if (favFirstMenuOpen[i] == 1)
                    {
                        favFirstMenu[i].SetActive(true);
                    }
                }
            }

            if (PlayerPrefs.HasKey("favSecondMenuOpen"))
            {
                string[] dataArr1 = PlayerPrefs.GetString("favSecondMenuOpen").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

                for (int i = 0; i < dataArr1.Length; i++)
                {
                    favSecondMenuOpen[i] = System.Convert.ToInt32(dataArr1[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장

                    if (favSecondMenuOpen[i] == 1)
                    {
                        favSecondMenu[i].SetActive(true);
                    }
                }
            }

            if (PlayerPrefs.HasKey("HiddenTextStates"))
            {
                string[] dataArr2 = PlayerPrefs.GetString("HiddenTextStates").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

                for (int i = 0; i < dataArr2.Length; i++)
                {
                    hiddenTextStates[i] = System.Convert.ToInt32(dataArr2[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장

                    if (hiddenTextStates[i] == 1)
                    {
                        hiddenText[i].SetActive(true);
                    }
                }
            }

            if (PlayerPrefs.HasKey("FriendshipInfo"))
            {
                string[] dataArr3 = PlayerPrefs.GetString("FriendshipInfo").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

                for (int i = 0; i < dataArr3.Length; i++)
                {
                    friendshipInfo[i] = System.Convert.ToInt32(dataArr3[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장
                    if (i <= 4)//도리부터 도로시까지
                    {
                        if(friendshipInfo[i] >= 10)//서빙횟수가 10보다 많거나 같으면
                        {
                            friendshipGauges[i].GetComponent<Image>().fillAmount = 1f;
                        }
                        else
                        {
                            friendshipGauges[i].GetComponent<Image>().fillAmount = (float)(friendshipInfo[i] * 0.1); //0.1을 곱한 값을 친밀도 게이지에 대입
                        }
                    }
                    else if((i >= 5 && i <= 8) || i == 10 || i == 11)//루루부터 찰스까지, 히로디노와 닥터펭도
                    {
                        if (friendshipInfo[i] >= 15)//서빙횟수가 15보다 많거나 같으면
                        {
                            friendshipGauges[i].GetComponent<Image>().fillAmount = 1f;
                        }
                        else
                        {
                            friendshipGauges[i].GetComponent<Image>().fillAmount = (float)(friendshipInfo[i] * 0.067f); //0.067을 곱한 값을 친밀도 게이지에 대입
                        }
                    }
                    else if(i == 9 || i == 12)//무명이거나 롤렝드
                    {
                        if (friendshipInfo[i] >= 20)//서빙횟수가 20보다 많거나 같으면
                        {
                            friendshipGauges[i].GetComponent<Image>().fillAmount = 1f;
                        }
                        else
                        {
                            friendshipGauges[i].GetComponent<Image>().fillAmount = (float)(friendshipInfo[i] * 0.05f); //0.05을 곱한 값을 친밀도 게이지에 대입
                        }
                    }
                }
            }

            if(PlayerPrefs.HasKey("NameForNameless"))
            {
                string name = PlayerPrefs.GetString("NameForNameless");
                OpenNameForNameless(name);
            }

            if (PlayerPrefs.HasKey("ReplayState"))//다시 보기 정보 불러오기
            {
                replayState = PlayerPrefs.GetInt("ReplayState");
                firstMeetID = PlayerPrefs.GetInt("FirstMeetID");
                friendEventID = PlayerPrefs.GetInt("FriendEventID");
            }

            if(replayState != 0)//다시보기 중일 때 종료한 거면 캐릭터DC 초기화
            {
                replayState = 0;
                if (firstMeetID != 0)//첫 만남 다시보기일 때
                {
                    Dialogue.instance.SetCharacterDC(firstMeetID, 3);
                    firstMeetID = 0;
                }
                else if (friendEventID != 0) //이벤트 다시보기일 때
                {
                    if (friendEventID <= 10)//10까지
                    {
                        Dialogue.instance.SetCharacterDC(friendEventID, 3);
                    }
                    else if (friendEventID == 11)//찰스2 이벤트
                    {
                        Dialogue.instance.SetCharacterDC(10, 3);
                    }
                    else if (friendEventID == 12 || friendEventID == 13)//무명이1 이벤트
                    {
                        Dialogue.instance.SetCharacterDC(11, 3);
                    }
                    else //14부터
                    {
                        Dialogue.instance.SetCharacterDC(friendEventID - 2, 3);
                    }
                    friendEventID = 0;
                }
            }
            
            for(int i = 1; i <= 14; i++)
            {
                if (i != 0 && Dialogue.instance.GetCharacterDC(i) == 3)//제제가 아닌 캐릭터의 DC가 3이면, 시나리오가 끝났으면
                {
                    replayButtons[i - 1].gameObject.SetActive(true);//다시보기 버튼 활성화
                    if (i == 1)//도리는 손님노트 이미지를 2번째 표정으로 바꿈
                    {
                        SetCharacterImage(i, CharacterManager.instance.CharacterFaceList[i].face[1].GetComponent<Image>().sprite);
                    }
                    if(i == 11)
                    {
                        SetCharacterImage(i, CharacterManager.instance.CharacterFaceList[i - 2].face[3].GetComponent<Image>().sprite);
                    }
                    if(i == 12 || i == 13)
                    {
                        SetCharacterImage(i, CharacterManager.instance.CharacterFaceList[i - 2].face[0].GetComponent<Image>().sprite);
                    }
                    if(i == 14)
                    {
                        SetCharacterImage(i, CharacterManager.instance.GetBigCharacter(17).GetComponent<Image>().sprite);
                    }
                }         
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadVisitorNoteInfo Failed (" + e.Message + ")");
        }
    }
}
