using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterVisit : MonoBehaviour //캐릭터 등장에 관한 스크립트
{
    public static CharacterVisit instance;

    public List<string> canVisitCharacters = new List<string> (); //랜덤 방문 가능한 캐릭터들 리스트

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public int visitC = 0; //방문할 캐릭터, 친밀도 이벤트 함수에서 사용
    public void RandomVisit()//캐릭터들의 카페 방문, 등장한 캐릭터 중에 랜덤으로 방문
    {
        if ((VisitorNote.instance.GetFirstMeetID() == 0 && VisitorNote.instance.GetFriendEventID() == 0 && Dialogue.instance.IsTalking()) || PlayerPrefs.GetInt("EndingState") == 1)//캐릭터와 대화 중일 경우 혹은 엔딩이벤트 할 경우 함수 종료
        {
            return;
        }
        if(canVisitCharacters.Count == 0) //현재 방문 가능한 캐릭터가 없을 시 3초 뒤 이 함수 재실행
        {
            if(!IsInvoking("RandomVisit"))
            {
                Invoke("RandomVisit", 3f);
              //  Debug.Log("수동 방문 가능 캐릭터 없음, 3초 뒤");
            }           
            return;
        }
        if (SmallFade.instance.TableEmpty[0] == 1 && SmallFade.instance.TableEmpty[1] == 1 && SmallFade.instance.TableEmpty[2] == 1) //세 테이블에 모두 손님이 있으면
        {
            if (!IsInvoking("RandomVisit"))
            {
                Invoke("RandomVisit", 5f);
              //  Debug.Log("수동 빈 테이블 없음, 5초 뒤");
            }
            return;
        }
        else //최소 한 테이블 이상 비었으면
        {
            //Debug.Log("방문가능캐릭터리스트 크기 " + canVisitCharacters.Count);
            int num = Random.Range(0, canVisitCharacters.Count - 1); //랜덤 캐릭터 넘버
            int cNum = 0;
            switch (canVisitCharacters[num])
            {
                case "small_1Bear":
                    cNum = 1;
                    canVisitCharacters.Remove("small_1Bear"); 
                    break;
                case "small_2Car":
                    cNum = 2;
                    canVisitCharacters.Remove("small_2Car");
                    break;
                case "small_3Bread":
                    cNum = 3;
                    canVisitCharacters.Remove("small_3Bread");
                    break;
                case "small_4Rabbit":
                    cNum = 4;
                    canVisitCharacters.Remove("small_4Rabbit");
                    break;
                case "small_5Ddorong":
                    cNum = 5;
                    canVisitCharacters.Remove("small_5Ddorong");
                    break;
                case "small_6Princess":
                    cNum = 6;
                    canVisitCharacters.Remove("small_6Princess");
                    break;
                case "small_7FamilySeries":
                    cNum = 7;
                    canVisitCharacters.Remove("small_7FamilySeries");
                    break;
                case "small_8SunFlower":
                    cNum = 8;
                    canVisitCharacters.Remove("small_8SunFlower");
                    break;
                case "small_9Dog":
                    cNum = 9;
                    canVisitCharacters.Remove("small_9Dog");
                    break;
                case "small_10Soldier":
                    cNum = 10;
                    canVisitCharacters.Remove("small_10Soldier");
                    break;
                case "small_11Nameless":
                    cNum = 11;
                    canVisitCharacters.Remove("small_11Nameless");
                    break;
                case "small_12Hero&13Dianosoour":
                    cNum = 12;//히로부터 등장
                    canVisitCharacters.Remove("small_12Hero&13Dianosoour");
                    break;
                case "small_14Penguin":
                    cNum = 14;
                    canVisitCharacters.Remove("small_14Penguin");
                    break;
                case "small_15Grandfather":
                    cNum = 15;
                    canVisitCharacters.Remove("small_15Grandfather");
                    break;
                case "small_6Princess&10Soldier":
                    cNum = 6;//도로시부터 등장
                    canVisitCharacters.Remove("small_6Princess&10Soldier");
                    break;
            }
            visitC = cNum;
            CharacterAppear.instance.FriendshipEvent(); //방문할 캐릭터의 친밀도 이벤트가 발생하는지 확인
            //Debug.Log("방문할 캐릭터 넘버 " + cNum);
            if (CharacterAppear.instance.eventOn == 0) //이벤트가 발생하지 않으면 일반 방문 진행
            {
                SmallFade.instance.SetCharacter(cNum); //방문할 캐릭터 세팅
                Invoke("RandomVisit", 7f);
            }
        }
        
    }

    public Queue<int> revisit = new Queue<int>(); //재방문할 수 있는 캐릭터 큐

    public void CanRevisit()
    {
        if(revisit.Count != 0)//큐가 0이 아닐 때만 실행
        {
            int rNum = revisit.Peek();
            switch (rNum)
            {
                case 1:
                    canVisitCharacters.Add("small_1Bear");
                    //Debug.Log("도리 재방문 가능");
                    break;
                case 2:
                    canVisitCharacters.Add("small_2Car");
                    //Debug.Log("붕붕 재방문 가능");
                    break;
                case 3:
                    canVisitCharacters.Add("small_3Bread");
                    // Debug.Log("빵빵 재방문 가능");
                    break;
                case 4:
                    canVisitCharacters.Add("small_4Rabbit");
                    // Debug.Log("개나리 재방문 가능");
                    break;
                case 5:
                    canVisitCharacters.Add("small_5Ddorong");
                    // Debug.Log("또롱 재방문 가능");
                    break;
                case 6:
                    canVisitCharacters.Add("small_6Princess");
                    //Debug.Log("도로시 재방문 가능");
                    break;
                case 7:
                    canVisitCharacters.Add("small_7FamilySeries");
                    //Debug.Log("루루 재방문 가능");
                    break;
                case 8:
                    canVisitCharacters.Add("small_8SunFlower");
                    // Debug.Log("샌디 재방문 가능");
                    break;
                case 9:
                    canVisitCharacters.Add("small_9Dog");
                    // Debug.Log("친구 재방문 가능");
                    break;
                case 10:
                    canVisitCharacters.Add("small_10Soldier");
                    //Debug.Log("찰스 재방문 가능");
                    break;
                case 11:
                    canVisitCharacters.Add("small_11Nameless");
                   // Debug.Log("무명이 재방문 가능");
                    break;
                case 13://히로디노
                    canVisitCharacters.Add("small_12Hero&13Dianosoour");
                   // Debug.Log("히로디노 재방문 가능");
                    break;
                case 14:
                    canVisitCharacters.Add("small_14Penguin");
                   // Debug.Log("닥터펭 재방문 가능");
                    break;
                case 15:
                    canVisitCharacters.Add("small_15Grandfather");
                   // Debug.Log("롤렝드 재방문 가능");
                    break;
                case 17:
                    canVisitCharacters.Add("small_6Princess&10Soldier");
                  // Debug.Log("찰스도로시 재방문 가능");
                    break;
            }
            //Debug.Log("함수 CanRevisit");
            revisit.Dequeue();
        }       
    }
}
