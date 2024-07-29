using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TouchableObject : MonoBehaviour //터치된 오브젝트 구분, 터치할 수 있는 오브젝트에 컴포넌트로 들어가있음
{
    public static TouchableObject instance;
   
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    int sNum; //자리 넘버
    int cNum; //캐릭터 넘버
    
    int GetNumber(int idx, string name)
    {
        if(idx+2 == name.Length) // 한 자리 숫자로 문자열이 끝났다면
            return int.Parse(name.Substring(idx+1,1)); // 한 자리 숫자만 추출

        // 숫자가 두 자릿수인지 확인
        int i = 0;
        bool isTwoDigit = int.TryParse(name.Substring(idx+2,1), out i); // int형에 속하는지 확인, 
        //TryParse : 숫자가 아닌 문자를 포함하거나 지정한 형식에 비해 너무 크거나 작은 경우 false 반환, out 매개 변수를 0으로 설정
        //그렇지 않으면 true 반환 out 매개 변수를 문자열의 숫자 값으로 설정
        
        if(isTwoDigit) 
            return int.Parse(name.Substring(idx+1,2)); // 두 자리 숫자 추출
        else
            return int.Parse(name.Substring(idx+1,1)); // 한 자리 숫자만 추출
    }

    public void TouchSmallCharacter() //저장된 캐릭터 자리 구분, (자리에 앉아있는)작은 캐릭터 터치 시 실행
    {
        // 캐릭터 오브젝트 이름 예시 : small_1Bear
        int idx = gameObject.name.IndexOf("_");

        cNum = GetNumber(idx, gameObject.name); // 캐릭터 오브젝트의 숫자 추출

        sNum = SmallFade.instance.CharacterSeat[cNum - 1];

        SmallFade.instance.CantTouchCharacter(cNum); //캐릭터 터치 불가, 중복 터치 방지

        if (SystemManager.instance.GetMaCharacterIn() == 2) //튜토리얼
        {
            MenuHint.instance.SetMHB(11);
            MenuHint.instance.SetMHText(cNum, 1);
        }
        else
        {
            MenuHint.instance.SetMHB(sNum);
            MenuHint.instance.SetMHText(cNum, sNum);
        }
    }

    public void TouchMenuHintBubble() //메뉴힌트말풍선 터치 시 실행
    {
        // 메뉴힌트말풍선 오브젝트 이름 예시 : 2MenuHintRight
        sNum = int.Parse(gameObject.name.Substring(0,1)) - 1; // 숫자만 추출해서 - 1
        Menu.instance.seatNum = sNum; // 0~5

        string charName = SmallFade.instance.SittingCharacter[sNum].name;
        int idx = charName.IndexOf("_");

        Menu.instance.cNum = GetNumber(idx, charName);

        Menu.instance.ClickMenuHint();
    }

    public void TouchMenu()
    {
        // 메뉴 오브젝트 이름 예시 : Menu1_OrangeJuice
        int idx = gameObject.name.IndexOf("_");
        int num = int.Parse(gameObject.name.Substring(idx-1,1)); // 숫자만 추출

        Menu.instance.MenuServingFunction(num);
    }

    public void FadeOutCharacter()
    {
        if (GameScript1.jejeOn) //제제일 경우
        {
            SmallFade.instance.z = 0;
        }
        else
        {
            int n = Menu.instance.seatInfo.Peek();
            string charName = SmallFade.instance.SittingCharacter[n].name;

            // 캐릭터 오브젝트 이름 예시 : small_1Bear
            int idx = charName.IndexOf("_");

            idx = GetNumber(idx, charName);
    
            switch(idx)
            {
                case 6: // princess 도로시
                    SmallFade.instance.smallFOut.Enqueue(6);
                    if (Dialogue.instance.CharacterDC[10] != 3)//찰스2이벤트 이후가 아니면
                    {
                        CharacterVisit.instance.revisit.Enqueue(6);
                    }
                    else if(Dialogue.instance.CharacterDC[10] == 3)//이벤트 후면
                    {
                        if (!SmallFade.instance.x2)
                        {
                            SmallFade.instance.x2 = true;
                        }
                        else if (SmallFade.instance.x2)
                        {
                            SmallFade.instance.x2 = false;
                        }
                    }
                    //Debug.Log("페이드아웃 될 캐릭터 도로시");
                    break;
                case 10: // soldier 찰스
                    SmallFade.instance.smallFOut.Enqueue(10);
                    if (CharacterAppear.instance.eventOn != 11)//찰스2 이벤트 중이 아니면
                    {
                        if (Dialogue.instance.CharacterDC[10] == 3)//찰스2 이벤트 뒤면
                        {
                            if (!SmallFade.instance.x2)
                            {
                                SmallFade.instance.x2 = true;
                            }
                            else if (SmallFade.instance.x2)
                            {
                                SmallFade.instance.x2 = false;
                            }    
                        }
                        else
                        {
                            CharacterVisit.instance.revisit.Enqueue(10);
                        }
                       // Debug.Log("페이드아웃 될 캐릭터 찰스");
                    }
                    break;
                case 12: // hero 히로
                    if(!SmallFade.instance.x1)//히로가 먼저 가는 거면
                    {
                        SmallFade.instance.x1 = true;
                    }
                    else if(SmallFade.instance.x1)//디노가 먼저 갔으면
                    {
                        SmallFade.instance.x1 = false; //값을 0으로 만들고
                    }
                    SmallFade.instance.smallFOut.Enqueue(12);
                   // Debug.Log("페이드아웃 될 캐릭터 히로");
                    break;
                case 13: // dinosour 디노
                    if (!SmallFade.instance.x1)
                    {
                        SmallFade.instance.x1 = true;
                    }
                    else if (SmallFade.instance.x1)
                    {
                        SmallFade.instance.x1 = false;
                    }
                    SmallFade.instance.smallFOut.Enqueue(13); 
                  //  Debug.Log("페이드아웃 될 캐릭터 디노");
                    break;

                default: // 오브젝트 이름에 _이 들어가지 않아 idx가 -1인 경우 or 별다른 처리가 필요없는 캐릭터일 경우
                    if(charName == "sBabyLeft")
                    {
                        SmallFade.instance.smallFOut.Enqueue(17);
                    }
                    else if(charName == "sBabyRight")
                    {
                        SmallFade.instance.smallFOut.Enqueue(16);
                    }
                    else
                    {
                        SmallFade.instance.smallFOut.Enqueue(idx);
                        CharacterVisit.instance.revisit.Enqueue(idx);
                    }
                    break;
            }

            if (charName != "sHero" && charName != "sDinosour")
            { //히로디노가 아니고
                if(Dialogue.instance.CharacterDC[10] != 3 || (Dialogue.instance.CharacterDC[10] == 3 && charName != "sPrincess" && charName != "sSoldier"))
                {//찰스 이벤트가 다 안 끝났거나 끝났어도 찰스,도로시가 아니면
                    SmallFade.instance.cleanSeat.Enqueue(n); //비워질 자리 큐에 정보 추가
                   // Debug.Log(n + "자리 클린시트큐에 추가됨");
                }       
                else if(Dialogue.instance.CharacterDC[10] == 3 && (charName == "sPrincess" || charName == "sSoldier"))
                {//찰스도로시 중 찰스나 도로시일 때
                    if (!SmallFade.instance.x2)//값이 0이어야만 가능
                    {
                        SmallFade.instance.cleanSeat.Enqueue(n); //비워질 자리 큐에 정보 추가
                        CharacterVisit.instance.revisit.Enqueue(17);
                    }
                }
            }
            else
            {
                if (!SmallFade.instance.x1)//값이 0이어야만 가능
                {
                    SmallFade.instance.cleanSeat.Enqueue(n); //비워질 자리 큐에 정보 추가
                  //  Debug.Log(n + "자리 클린시트큐에 추가됨");
                    CharacterVisit.instance.revisit.Enqueue(13);
                }
            }       
            Menu.instance.seatInfo.Dequeue();
            SmallFade.instance.SittingCharacter[n] = null;//버그 대비 페이드아웃 시 null 넣기
        }
        //Debug.Log("함수 FadeOutCharacter");
    } 

    public void TouchStar()// 별 터치 시 실행, 터치한 별을 페이드아웃 큐에 추가
    {
        // 별 오브젝트 이름 예시 : 1Star
        int star = int.Parse(gameObject.name.Substring(0,1)); // 숫자만 추출
        Star.instance.TouchStar(star);
    }

    public void TouchPageButton()//손님노트의 페이지 버튼 터치 시 실행
    {
        // 손님 노트 페이지 버튼 오브젝트 이름 예시 : Page1, Page11
        int idx = gameObject.name.IndexOf("e");

        int pageNum = GetNumber(idx, gameObject.name);

        VisitorNote.instance.pageNum2 = pageNum; //현재 페이지 넘버
        VisitorNote.instance.TurnToPage();
    }

    public void TouchEventReplayButton() //이벤트 다시보기 버튼 터치 시 실행, 어떤 캐릭터의 이벤트 다시보기 버튼을 눌렀는지
    {
        // 오브젝트 이름 예시 : Event1, Event11
        int idx = gameObject.name.IndexOf("t");

        idx = GetNumber(idx, gameObject.name);

        VisitorNote.instance.evRP = idx;

        string name = "";
        switch(idx)
        {
            case 1:
                name = "도리" + "와";
                break;
            case 2:
                name = "붕붕" + "과";
                break;
            case 3:
                name = "빵빵" + "과";
                break;
            case 4:
                name = "개나리" + "와";
                break;
            case 5:
                name = "또롱" + "과";
                break;
            case 6:
                name = "도로시" + "와";
                break;
            case 7:
                name = "루루" + "와";
                break;
            case 8:
                name = "샌디" + "와";
                break;
            case 9:
                name = "친구" + "와";
                break;
            case 10:
            case 11:
                name = "찰스" + "와";
                break;
            case 12:
            case 13:
                name = SystemManager.instance.GetNameForNameless() + "와(과)";
                break;
            case 14:
                name = "히로&디노" + "와";
                break;
            case 15:
                name = "닥터 펭" + "과";
                break;
            case 16:
                name = "롤렝드" + "와";
                break;
        }

        VisitorNote.instance.whichStory.text = name + "의 이벤트를 회상할까요?";

        VisitorNote.instance.rePlayMessage.SetActive(true);
    }
}
