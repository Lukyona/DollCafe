using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAppear : MonoBehaviour
{
    public static CharacterAppear instance;

    int nextAppear = 0;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Update() //캐릭터들 첫방문
    {
        if (!SystemManager.instance.IsUIOpen() && !UI_Assistant1.instance.talking && eventOn == 0 && VisitorNote.instance.replayOn == 0) // UI가 올라와있지 않고 대화 중이 아닐 때
        {
            if (SmallFade.instance.TableEmpty[0] == 0 || SmallFade.instance.TableEmpty[1] == 0 || SmallFade.instance.TableEmpty[2] == 0) //테이블이 하나라도 비었다면
            {
                WhichCharacterAppear();
            }
        }
    }

    public void SetNextAppearNum(int num)
    {
        nextAppear = num;
    }

    public int GetNextAppearNum()
    {
        return nextAppear;
    }

    void WhichCharacterAppear()
    {
        if (Menu.instance.GetReputation() >= 9 && nextAppear == 3)
        {//평판이 9 이상이며 다음 등장 캐릭터 번호가 3일 때
            SystemManager.instance.BeginDialogue(3); //빵빵 등장
        }

        if (Menu.instance.GetReputation() >= 21 && nextAppear == 4)
        {
            SystemManager.instance.BeginDialogue(4);//개나리 등장
        }

        if (Menu.instance.GetReputation() >= 36 && nextAppear == 5)
        {
            SystemManager.instance.BeginDialogue(5);//또롱 등장
        }

        if (Menu.instance.GetReputation() >= 55 && nextAppear == 6)
        {
            SystemManager.instance.BeginDialogue(6);
        }

        if (Menu.instance.GetReputation() >= 70 && nextAppear == 7)
        {
            SystemManager.instance.BeginDialogue(7);
        }

        if (Menu.instance.GetReputation() >= 85 && nextAppear == 8 && Menu.instance.IsMenuOpen(4))
        {
            SystemManager.instance.BeginDialogue(8);
        }

        if (Menu.instance.GetReputation() >= 95 && nextAppear == 9)
        {
            SystemManager.instance.BeginDialogue(9);
        }

        if (Menu.instance.GetReputation() >= 150 && nextAppear == 10 && Menu.instance.IsMenuOpen(5))
        {
            SystemManager.instance.BeginDialogue(10);
        }

        if (Menu.instance.GetReputation() >= 180 && nextAppear == 11)
        {
            SystemManager.instance.BeginDialogue(11);
        }

        if (Menu.instance.GetReputation() >= 260 && nextAppear == 12 && Menu.instance.IsMenuOpen(6))
        {
            SystemManager.instance.BeginDialogue(12);
        }

        if (Menu.instance.GetReputation() >= 320 && nextAppear == 13 && Menu.instance.IsMenuOpen(7))
        {
            SystemManager.instance.BeginDialogue(13);
        }

        if (Menu.instance.GetReputation() >= 400 && nextAppear == 14)
        {
            SystemManager.instance.BeginDialogue(14);
        }
    }
                                                                                                                            //5또롱이도 있음
    public int eventOn = 0;//0은 친밀도 이벤트 중이 아님을 뜻함, 이벤트 발생하는 캐릭터 번호 들어감, 1도리 2붕붕 3빵빵 4개나리 6도로시 7루루 8샌디 9친구 10찰스1 11찰스2 ,12무명이1, 13무명이2, 14히로디노, 15닥터펭, 16롤렝드
    public void FriendshipEvent()//친밀도 이벤트
    {
        int c = CharacterVisit.instance.visitC;
        if (c == 14 || c == 15)//닥터펭, 롤렝드의 경우
        {
            if (Dialogue.instance.CharacterDC[c - 1] != 0 && Dialogue.instance.CharacterDC[c - 1] != 3)
            {
                if (VisitorNote.instance.friendshipGauge[c - 3].GetComponent<Image>().fillAmount == 1)//친밀도 게이지가 꽉 찼을 때
                {
                    if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                    {
                        CharacterVisit.instance.CancelInvoke("RandomVisit");
                        Debug.Log("랜덤방문 취소");
                    }
                    MenuHint.instance.CantClickMHB();
                    SystemManager.instance.CantTouchUI();
                    eventOn = c+1;//친밀도 이벤트 진행 중
                    if (SystemManager.instance.IsUIOpen() || UI_Assistant1.instance.talking || VisitorNote.instance.replayOn != 0)//만약 UI가 올라와있다면
                    {
                        Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                    }
                    else
                    {
                        SmallFade.instance.SetCharacter(c);//캐릭터를 설정하고 바로 페이드인    
                    }
                }        
            }
        }
        else
        {
            if (Dialogue.instance.CharacterDC[c] != 0 && Dialogue.instance.CharacterDC[c] != 3) //첫만남이 아니고, 시나리오를 다 보지 않았을 때
            {
                switch (c)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        if (VisitorNote.instance.friendshipGauge[c - 1].GetComponent<Image>().fillAmount == 1)
                        {//친밀도 게이지가 꽉 찼을 때
                            if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                            {
                                CharacterVisit.instance.CancelInvoke("RandomVisit");
                            }
                            MenuHint.instance.CantClickMHB();
                            SystemManager.instance.CantTouchUI();//UI올라온 상태에서의 시나리오 진행을 막기 위함
                            eventOn = c;//친밀도 이벤트 진행 중
                            if (SystemManager.instance.IsUIOpen() || UI_Assistant1.instance.talking || VisitorNote.instance.replayOn != 0)//만약 UI가 올라와있다면
                            {
                                Debug.Log(c + " 친밀도 이벤트 대기");
                                Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                            }
                            else
                            {
                                Debug.Log(c + " 친밀도 이벤트 시작");
                                SmallFade.instance.SetCharacter(c);//캐릭터를 설정하고 바로 페이드인    
                            }
                        }
                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        if (VisitorNote.instance.friendshipGauge[c - 2].GetComponent<Image>().fillAmount == 1)
                        {//친밀도 게이지가 꽉 찼을 때
                            if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                            {
                                CharacterVisit.instance.CancelInvoke("RandomVisit");
                                Debug.Log("랜덤방문 취소");
                            }
                            if (c != 9) // 강아지 필통(친구)는 메뉴 주문 후 발생 *****에러 있음, 주문 받고 친밀도 이벤트 시작하고 이전 주문 받은 거로 인해 작은 캐릭터 사라지는 문제
                            {
                                MenuHint.instance.CantClickMHB();
                                SystemManager.instance.CantTouchUI();
                            }
                            eventOn = c;//친밀도 이벤트 진행 중
                            if (SystemManager.instance.IsUIOpen() || UI_Assistant1.instance.talking || VisitorNote.instance.replayOn != 0)//만약 UI가 올라와있다면
                            {
                                Debug.Log(c + " 친밀도 이벤트 대기");
                                Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                            }
                            else
                            {
                                Debug.Log(c + " 친밀도 이벤트 시작");
                                SmallFade.instance.SetCharacter(c);//캐릭터를 설정하고 바로 페이드인    
                            }
                        }
                        break;
                    case 10://찰스1
                        if (Dialogue.instance.CharacterDC[10] == 1)//찰스1
                        {
                            if (VisitorNote.instance.FriendshipInfo[8] == 10)
                            {//친밀도 10일 때, 서빙횟수가 10번일 때
                                if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                                {
                                    CharacterVisit.instance.CancelInvoke("RandomVisit");
                                    Debug.Log("랜덤방문 취소");
                                }
                                eventOn = c; //두 개 이상의 테이블이 비었고 도로시가 방문가능캐릭터일 때 실행, 도로시까지 필요하기 때문
                                if (SystemManager.instance.IsUIOpen() || UI_Assistant1.instance.talking || VisitorNote.instance.replayOn != 0)//만약 UI가 올라와있다면
                                {
                                    Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                                }
                                else
                                {
                                    if (((SmallFade.instance.TableEmpty[0] == 0 && SmallFade.instance.TableEmpty[1] == 0) ||
                                    (SmallFade.instance.TableEmpty[0] == 0 && SmallFade.instance.TableEmpty[2] == 0) ||
                                    (SmallFade.instance.TableEmpty[1] == 0 && SmallFade.instance.TableEmpty[2] == 0))
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
                        else if (Dialogue.instance.CharacterDC[10] == 2)//찰스2
                        {
                            if (VisitorNote.instance.friendshipGauge[c - 2].GetComponent<Image>().fillAmount == 1f && VisitorNote.instance.friendshipGauge[4].GetComponent<Image>().fillAmount == 1f)
                            {// 찰스, 도로시 친밀도 모두 최대일때
                                if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                                {
                                    CharacterVisit.instance.CancelInvoke("RandomVisit");
                                    Debug.Log("랜덤방문 취소");
                                }
                                eventOn = 11; //두 개 이상의 테이블이 비었을 때 실행, 도로시까지 필요하기 때문
                                if (SystemManager.instance.IsUIOpen() || UI_Assistant1.instance.talking || VisitorNote.instance.replayOn != 0)//만약 UI가 올라와있다면
                                {
                                    Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                                }
                                else
                                {
                                    if ((SmallFade.instance.TableEmpty[0] == 0 && SmallFade.instance.TableEmpty[1] == 0) ||
                                    (SmallFade.instance.TableEmpty[0] == 0 && SmallFade.instance.TableEmpty[2] == 0) ||
                                    (SmallFade.instance.TableEmpty[1] == 0 && SmallFade.instance.TableEmpty[2] == 0))
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
                        if (Dialogue.instance.CharacterDC[11] == 1)//무명이1
                        {
                            if (VisitorNote.instance.FriendshipInfo[9] == 10)//친밀도 10일때
                            {
                                if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                                {
                                    CharacterVisit.instance.CancelInvoke("RandomVisit");
                                    Debug.Log("랜덤방문 취소");
                                }
                                eventOn = 12;//친밀도 이벤트 진행 중
                                if (SystemManager.instance.IsUIOpen() || UI_Assistant1.instance.talking || VisitorNote.instance.replayOn != 0)//만약 UI가 올라와있다면
                                {
                                    Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                                }
                                else
                                {
                                    SmallFade.instance.SetCharacter(11);//캐릭터를 설정하고 바로 페이드인    
                                }
                            }
                        }
                        else if (Dialogue.instance.CharacterDC[11] == 2)//무명이2
                        {
                            if (VisitorNote.instance.friendshipGauge[c - 2].GetComponent<Image>().fillAmount == 1f)//친밀도 20일때
                            {
                                if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                                {
                                    CharacterVisit.instance.CancelInvoke("RandomVisit");
                                    Debug.Log("랜덤방문 취소");
                                }
                                MenuHint.instance.CantClickMHB();
                                SystemManager.instance.CantTouchUI();
                                eventOn = 13;//친밀도 이벤트 진행 중
                                if (SystemManager.instance.IsUIOpen() || UI_Assistant1.instance.talking || VisitorNote.instance.replayOn != 0)//만약 UI가 올라와있다면
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
                        if (VisitorNote.instance.friendshipGauge[c - 2].GetComponent<Image>().fillAmount == 1)//친밀도 게이지가 꽉 찼을 때
                        {
                            if (CharacterVisit.instance.IsInvoking("RandomVisit"))
                            {
                                CharacterVisit.instance.CancelInvoke("RandomVisit");
                                Debug.Log("랜덤방문 취소");
                            }
                            eventOn = 14;//친밀도 이벤트 진행 중
                            if (SystemManager.instance.IsUIOpen() || UI_Assistant1.instance.talking || VisitorNote.instance.replayOn != 0)//만약 UI가 올라와있다면
                            {
                                Invoke("FriendshipEvent", 1f);// 1초마다 이 함수 재실행
                            }
                            else
                            {
                                SmallFade.instance.SetCharacter(c);//캐릭터를 설정하고 바로 페이드인    
                            }
                        }
                        break;                    
                }
            }
        }                
    }
}
