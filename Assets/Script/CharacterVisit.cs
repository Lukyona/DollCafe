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
        if ((VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0 && UI_Assistant1.instance.talking) || GameScript1.instance.endStory == 1)//캐릭터와 대화 중일 경우 혹은 엔딩이벤트 할 경우 함수 종료
        {
            return;
        }
        if(canVisitCharacters.Count == 0) //현재 방문 가능한 캐릭터가 없을 시 3초 뒤 이 함수 재실행
        {
            if(!CharacterVisit.instance.IsInvoking("RandomVisit"))
            {
                Invoke("RandomVisit", 3f);
              //  Debug.Log("수동 방문 가능 캐릭터 없음, 3초 뒤");
            }           
            return;
        }
        if (SmallFade.instance.TableEmpty[0] == 1 && SmallFade.instance.TableEmpty[1] == 1 && SmallFade.instance.TableEmpty[2] == 1) //세 테이블에 모두 손님이 있으면
        {
            if (!CharacterVisit.instance.IsInvoking("RandomVisit"))
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
                case "sBear":
                    cNum = 1;
                    canVisitCharacters.Remove("sBear"); 
                    break;
                case "sCar":
                    cNum = 2;
                    canVisitCharacters.Remove("sCar");
                    break;
                case "sBread":
                    cNum = 3;
                    canVisitCharacters.Remove("sBread");
                    break;
                case "sRabbit":
                    cNum = 4;
                    canVisitCharacters.Remove("sRabbit");
                    break;
                case "sDdorong":
                    cNum = 5;
                    canVisitCharacters.Remove("sDdorong");
                    break;
                case "sPrincess":
                    cNum = 6;
                    canVisitCharacters.Remove("sPrincess");
                    break;
                case "sFamilySeries":
                    cNum = 7;
                    canVisitCharacters.Remove("sFamilySeries");
                    break;
                case "sSunflower":
                    cNum = 8;
                    canVisitCharacters.Remove("sSunflower");
                    break;
                case "sDog":
                    cNum = 9;
                    canVisitCharacters.Remove("sDog");
                    break;
                case "sSoldier":
                    cNum = 10;
                    canVisitCharacters.Remove("sSoldier");
                    break;
                case "sNoName":
                    cNum = 11;
                    canVisitCharacters.Remove("sNoName");
                    break;
                case "sHero&sDinosour":
                    cNum = 12;//히로부터 등장
                    canVisitCharacters.Remove("sHero&sDinosour");
                    break;
                case "sPenguin":
                    cNum = 14;
                    canVisitCharacters.Remove("sPenguin");
                    break;
                case "sGrandfather":
                    cNum = 15;
                    canVisitCharacters.Remove("sGrandfather");
                    break;
                case "sPrincess&sSoldier":
                    cNum = 6;//도로시부터 등장
                    canVisitCharacters.Remove("sPrincess&sSoldier");
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
                    canVisitCharacters.Add("sBear");
                    //Debug.Log("도리 재방문 가능");
                    break;
                case 2:
                    canVisitCharacters.Add("sCar");
                    //Debug.Log("붕붕 재방문 가능");
                    break;
                case 3:
                    canVisitCharacters.Add("sBread");
                    // Debug.Log("빵빵 재방문 가능");
                    break;
                case 4:
                    canVisitCharacters.Add("sRabbit");
                    // Debug.Log("개나리 재방문 가능");
                    break;
                case 5:
                    canVisitCharacters.Add("sDdorong");
                    // Debug.Log("또롱 재방문 가능");
                    break;
                case 6:
                    canVisitCharacters.Add("sPrincess");
                    //Debug.Log("도로시 재방문 가능");
                    break;
                case 7:
                    canVisitCharacters.Add("sFamilySeries");
                    //Debug.Log("루루 재방문 가능");
                    break;
                case 8:
                    canVisitCharacters.Add("sSunflower");
                    // Debug.Log("샌디 재방문 가능");
                    break;
                case 9:
                    canVisitCharacters.Add("sDog");
                    // Debug.Log("친구 재방문 가능");
                    break;
                case 10:
                    canVisitCharacters.Add("sSoldier");
                    //Debug.Log("찰스 재방문 가능");
                    break;
                case 11:
                    canVisitCharacters.Add("sNoName");
                   // Debug.Log("무명이 재방문 가능");
                    break;
                case 13://히로디노
                    canVisitCharacters.Add("sHero&sDinosour");
                   // Debug.Log("히로디노 재방문 가능");
                    break;
                case 14:
                    canVisitCharacters.Add("sPenguin");
                   // Debug.Log("닥터펭 재방문 가능");
                    break;
                case 15:
                    canVisitCharacters.Add("sGrandfather");
                   // Debug.Log("롤렝드 재방문 가능");
                    break;
                case 17:
                    canVisitCharacters.Add("sPrincess&sSoldier");
                  // Debug.Log("찰스도로시 재방문 가능");
                    break;
            }
            //Debug.Log("함수 CanRevisit");
            revisit.Dequeue();
        }       
    }
}
