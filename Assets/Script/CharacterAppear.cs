using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAppear : MonoBehaviour
{
    public static CharacterAppear instance;

    int nextAppear = 0;
    int currentEventState = 0;//0은 친밀도 이벤트 중이 아님을 뜻함, 이벤트 발생하는 캐릭터 번호 들어감, 1도리 2붕붕 3빵빵 4개나리 6도로시 7루루 8샌디 9친구 10찰스1 11찰스2 ,12무명이1, 13무명이2, 14히로디노, 15닥터펭, 16롤렝드

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Update() //캐릭터들 첫방문
    {
        if (!SystemManager.instance.IsUIOpen() && !Dialogue.instance.IsTalking() && currentEventState == 0 && VisitorNote.instance.GetReplayState() == 0) // UI가 올라와있지 않고 대화 중이 아닐 때
        {
            if (SmallFade.instance.IsTableEmpty(1) || SmallFade.instance.IsTableEmpty(2) || SmallFade.instance.IsTableEmpty(3)) //테이블이 하나라도 비었다면
            {
                CheckCharacterTrigger();
            }
        }
    }

    public int GetCurrentEventState()
    {
        return currentEventState;
    }

    public void SetCurrentEventState(int value)
    {
        currentEventState = value;
    }

    public void SetNextAppearNum(int num)
    {
        nextAppear = num;
    }

    public int GetNextAppearNum()
    {
        return nextAppear;
    }

    void CheckCharacterTrigger()
    {
        int minReputation = 0; // 필요한 최소 평판
        bool isMenuOpen = true; // 특정 캐릭터는 메뉴 해제 해야함

        switch(nextAppear)
        {
            case 3:
                minReputation = 9;
                break;
            case 4:
                minReputation = 21;
                break;
            case 5:
                minReputation = 36;
                break;
            case 6:
                minReputation = 55;
                break;
            case 7:
                minReputation = 70;
                break;
            case 8:
                minReputation = 85;
                isMenuOpen = Menu.instance.IsMenuOpen(4);
                break;
            case 9:
                minReputation = 95;
                break;
            case 10:
                minReputation = 150;
                isMenuOpen = Menu.instance.IsMenuOpen(5);
                break;
            case 11:
                minReputation = 180;
                break;
            case 12:
                minReputation = 260;
                isMenuOpen = Menu.instance.IsMenuOpen(6);
                break;
            case 13:
                minReputation = 320;
                isMenuOpen = Menu.instance.IsMenuOpen(7);
                break;
            case 14:
                minReputation = 400;
                break;
        }
       
        if(Menu.instance.GetReputation() >= minReputation && isMenuOpen)
        {
            SystemManager.instance.BeginDialogue(nextAppear);
        }
    }
                                                                                                                    
    public void FriendshipEvent(int cNum)//친밀도 이벤트 확인
    {
        if (cNum == 14 || cNum == 15)//닥터펭, 롤렝드의 경우
        {
            if (Dialogue.instance.GetCharacterDC(cNum - 1) != 0 && Dialogue.instance.GetCharacterDC(cNum - 1) != 3)
            {
                if (VisitorNote.instance.IsFriendshipGaugeFull(cNum - 3))//친밀도 게이지가 꽉 찼을 때
                {
                    if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                    {
                        CharacterVisit.instance.CancelInvoke("RandomVisit");
                        Debug.Log("랜덤방문 취소");
                    }
                    MenuHint.instance.CantClickMHB();
                    SystemManager.instance.CantTouchUI();
                    currentEventState = cNum + 1;//친밀도 이벤트 진행 중
                    if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                    {
                        Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                    }
                    else
                    {
                        SmallFade.instance.SetCharacter(cNum);//캐릭터를 설정하고 바로 페이드인    
                    }
                }        
            }
        }
        else
        {
            if (Dialogue.instance.GetCharacterDC(cNum) != 0 && Dialogue.instance.GetCharacterDC(cNum) != 3) //첫만남이 아니고, 시나리오를 다 보지 않았을 때
            {
                switch (cNum)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        if (VisitorNote.instance.IsFriendshipGaugeFull(cNum-1))
                        {//친밀도 게이지가 꽉 찼을 때
                            if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                            {
                                CharacterVisit.instance.CancelInvoke("RandomVisit");
                            }
                            MenuHint.instance.CantClickMHB();
                            SystemManager.instance.CantTouchUI();//UI올라온 상태에서의 시나리오 진행을 막기 위함
                            currentEventState = cNum;//친밀도 이벤트 진행 중
                            if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                            {
                                Debug.Log(cNum + " 친밀도 이벤트 대기");
                                Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                            }
                            else
                            {
                                Debug.Log(cNum + " 친밀도 이벤트 시작");
                                SmallFade.instance.SetCharacter(cNum);//캐릭터를 설정하고 바로 페이드인    
                            }
                        }
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        if (VisitorNote.instance.IsFriendshipGaugeFull(cNum-2))
                        {//친밀도 게이지가 꽉 찼을 때
                            if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                            {
                                CharacterVisit.instance.CancelInvoke("RandomVisit");
                                Debug.Log("랜덤방문 취소");
                            }
                            if (cNum != 9) // 강아지 필통(친구)는 메뉴 주문 후 발생 *****에러 있음, 주문 받고 친밀도 이벤트 시작하고 이전 주문 받은 거로 인해 작은 캐릭터 사라지는 문제
                            {
                                MenuHint.instance.CantClickMHB();
                                SystemManager.instance.CantTouchUI();
                            }
                            currentEventState = cNum;//친밀도 이벤트 진행 중
                            if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                            {
                                Debug.Log(cNum + " 친밀도 이벤트 대기");
                                Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                            }
                            else
                            {
                                Debug.Log(cNum + " 친밀도 이벤트 시작");
                                SmallFade.instance.SetCharacter(cNum);//캐릭터를 설정하고 바로 페이드인    
                            }
                        }
                        break;
                    case 10://찰스1
                        if (Dialogue.instance.GetCharacterDC(10) == 1)//찰스1
                        {
                            if (VisitorNote.instance.GetFriendshipInfo(8) == 10)
                            {//친밀도 10일 때, 서빙횟수가 10번일 때
                                if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                                {
                                    CharacterVisit.instance.CancelInvoke("RandomVisit");
                                    Debug.Log("랜덤방문 취소");
                                }
                                currentEventState = cNum; //두 개 이상의 테이블이 비었고 도로시가 방문가능캐릭터일 때 실행, 도로시까지 필요하기 때문
                                if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                                {
                                    Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                                }
                                else
                                {
                                    if (((SmallFade.instance.IsTableEmpty(1) && SmallFade.instance.IsTableEmpty(2)) ||
                                    (SmallFade.instance.IsTableEmpty(1) && SmallFade.instance.IsTableEmpty(3)) ||
                                    (SmallFade.instance.IsTableEmpty(2) && SmallFade.instance.IsTableEmpty(3)))
                                    && CharacterVisit.instance.canVisitCharacters.Contains("small_6Princess"))
                                    {
                                        SmallFade.instance.SetCharacter(6);//도로시 페이드인
                                        CharacterVisit.instance.canVisitCharacters.Remove("small_6Princess");//도로시는 이 이벤트에서 카페를 방문함
                                    }
                                    else //두 개 이상의 테이블이 비지 않았다면
                                    {
                                        Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                                    }
                                }
                            }
                        }
                        else if (Dialogue.instance.GetCharacterDC(10) == 2)//찰스2
                        {
                            if (VisitorNote.instance.IsFriendshipGaugeFull(cNum-2) && VisitorNote.instance.IsFriendshipGaugeFull(4))
                            {// 찰스, 도로시 친밀도 모두 최대일때
                                if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                                {
                                    CharacterVisit.instance.CancelInvoke("RandomVisit");
                                    Debug.Log("랜덤방문 취소");
                                }
                                currentEventState = 11; //두 개 이상의 테이블이 비었을 때 실행, 도로시까지 필요하기 때문
                                if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                                {
                                    Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                                }
                                else
                                {
                                    if ((SmallFade.instance.IsTableEmpty(1) && SmallFade.instance.IsTableEmpty(2)) ||
                                    (SmallFade.instance.IsTableEmpty(1) && SmallFade.instance.IsTableEmpty(3)) ||
                                    (SmallFade.instance.IsTableEmpty(2) && SmallFade.instance.IsTableEmpty(3)))
                                    {
                                        MenuHint.instance.CantClickMHB();
                                        SystemManager.instance.CantTouchUI();
                                        SmallFade.instance.SetCharacter(6);//도로시 페이드인
                                        CharacterVisit.instance.canVisitCharacters.Remove("small_6Princess");//도로시는 이 이벤트에서 카페를 방문함
                                    }
                                    else //두 개 이상의 테이블이 비지 않았다면
                                    {
                                        Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                                    }
                                }
                            }
                        }
                        break;
                    case 11:
                        if (Dialogue.instance.GetCharacterDC(11) == 1)//무명이1
                        {
                            if (VisitorNote.instance.GetFriendshipInfo(9) == 10)//친밀도 10일때
                            {
                                if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                                {
                                    CharacterVisit.instance.CancelInvoke("RandomVisit");
                                    Debug.Log("랜덤방문 취소");
                                }
                                currentEventState = 12;//친밀도 이벤트 진행 중
                                if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                                {
                                    Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                                }
                                else
                                {
                                    SmallFade.instance.SetCharacter(11);//캐릭터를 설정하고 바로 페이드인    
                                }
                            }
                        }
                        else if (Dialogue.instance.GetCharacterDC(11) == 2)//무명이2
                        {
                            if (VisitorNote.instance.IsFriendshipGaugeFull(cNum-2))//친밀도 20일때
                            {
                                if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                                {
                                    CharacterVisit.instance.CancelInvoke("RandomVisit");
                                    Debug.Log("랜덤방문 취소");
                                }
                                MenuHint.instance.CantClickMHB();
                                SystemManager.instance.CantTouchUI();
                                currentEventState = 13;//친밀도 이벤트 진행 중
                                if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                                {
                                    Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                                }
                                else
                                {
                                    SmallFade.instance.SetCharacter(11);//캐릭터를 설정하고 바로 페이드인    
                                }
                            }
                        }
                        break;
                    case 12://히로디노
                        if (VisitorNote.instance.IsFriendshipGaugeFull(cNum-2))//친밀도 게이지가 꽉 찼을 때
                        {
                            if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                            {
                                CharacterVisit.instance.CancelInvoke("RandomVisit");
                                Debug.Log("랜덤방문 취소");
                            }
                            currentEventState = 14;//친밀도 이벤트 진행 중
                            if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                            {
                                Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                            }
                            else
                            {
                                SmallFade.instance.SetCharacter(cNum);//캐릭터를 설정하고 바로 페이드인    
                            }
                        }
                        break;                    
                }
            }
        }                
    }
}
