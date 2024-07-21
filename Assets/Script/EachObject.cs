using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EachObject : MonoBehaviour //클릭된 오브젝트(캐릭터나 메뉴힌트버블) 구분
{
    public static EachObject instance;
   
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    int num; //자리넘버
    int cNum; //캐릭터 넘버
    
    public void CharacterSeatInfo() //저장된 캐릭터 자리 구분, 작은 캐릭터 이미지 터치 시 실행
    {
        switch (gameObject.name)
        {
            case "sBear":
                num = SmallFade.instance.CharacterSeat[0];
                cNum = 1;
                break;
            case "sCar":
                num = SmallFade.instance.CharacterSeat[1];
                cNum = 2;
                break;
            case "sBread":
                num = SmallFade.instance.CharacterSeat[2];
                cNum = 3;
                break;
            case "sRabbit":
                num = SmallFade.instance.CharacterSeat[3];
                cNum = 4;
                break;
            case "sDdorong":
                num = SmallFade.instance.CharacterSeat[4];
                cNum = 5;
                break;
            case "sPrincess":
                num = SmallFade.instance.CharacterSeat[5];
                cNum = 6;
                break;
            case "sFamilySeries":
                num = SmallFade.instance.CharacterSeat[6];
                cNum = 7;
                break;
            case "sSunflower":
                num = SmallFade.instance.CharacterSeat[7];
                cNum = 8;
                break;
            case "sDog":
                num = SmallFade.instance.CharacterSeat[8];
                cNum = 9;
                break;
            case "sSoldier":
                num = SmallFade.instance.CharacterSeat[9];
                cNum = 10;
                break;
            case "sNoName":
                num = SmallFade.instance.CharacterSeat[10];
                cNum = 11;
                break;
            case "sHero":
                num = SmallFade.instance.CharacterSeat[11];
                cNum = 12;
                break;
            case "sDinosour":
                num = SmallFade.instance.CharacterSeat[12];
                cNum = 13;
                break;
            case "sPenguin":
                num = SmallFade.instance.CharacterSeat[13];
                cNum = 14;
                break;
            case "sGrandfather":
                num = SmallFade.instance.CharacterSeat[14];
                cNum = 15;
                break;
        }
        SmallFade.instance.CantClickCharacter(cNum); //캐릭터 터치 불가, 중복 터치 방지

        if (GameScript1.instance.mainCount == 2) //튜토리얼
        {
            num = 11;
            MenuHint.instance.SetMHB(num);
            num = 1;
            MenuHint.instance.SetMHText(cNum, num);
        }
        else
        {
            MenuHint.instance.SetMHB(num);
            MenuHint.instance.SetMHText(cNum, num);
        }
    }

    public void NowHintBubble()//현재 클릭된 메뉴힌트버블이
    {
        if (gameObject.name.Contains("1"))//첫번째 테이블 왼쪽자리 버블이면
        {
            Menu.instance.seatNum = 0; 
            switch(SmallFade.instance.SittingCharacter[0].name)
            {
                case "sCar":
                    Menu.instance.cNum = 2;
                    break;
                case "sRabbit":
                    Menu.instance.cNum = 4;
                    break;
                case "sPrincess":
                    Menu.instance.cNum = 6;
                    break;
                case "sSunflower":
                    Menu.instance.cNum = 8;
                    break;
                case "sSoldier":
                    Menu.instance.cNum = 10;
                    break;
                case "sHero":
                    Menu.instance.cNum = 12;
                    break;
                case "sPenguin":
                    Menu.instance.cNum = 14;
                    break;
            }            
        }

        if (gameObject.name.Contains("3")) // 두번째 테이블 왼쪽
        {
            Menu.instance.seatNum = 2; 
            switch (SmallFade.instance.SittingCharacter[2].name)
            {
                case "sCar":
                    Menu.instance.cNum = 2;
                    break;
                case "sRabbit":
                    Menu.instance.cNum = 4;
                    break;
                case "sPrincess":
                    Menu.instance.cNum = 6;
                    break;
                case "sSunflower":
                    Menu.instance.cNum = 8;
                    break;
                case "sSoldier":
                    Menu.instance.cNum = 10;
                    break;
                case "sHero":
                    Menu.instance.cNum = 12;
                    break;
                case "sPenguin":
                    Menu.instance.cNum = 14;
                    break;
            }
        }

        if (gameObject.name.Contains("5")) //세번재 테이블 왼쪽
        {
            Menu.instance.seatNum = 4; 
            switch (SmallFade.instance.SittingCharacter[4].name)
            {
                case "sCar":
                    Menu.instance.cNum = 2;
                    break;
                case "sRabbit":
                    Menu.instance.cNum = 4;
                    break;
                case "sPrincess":
                    Menu.instance.cNum = 6;
                    break;
                case "sSunflower":
                    Menu.instance.cNum = 8;
                    break;
                case "sSoldier":
                    Menu.instance.cNum = 10;
                    break;
                case "sHero":
                    Menu.instance.cNum = 12;
                    break;
                case "sPenguin":
                    Menu.instance.cNum = 14;
                    break;
            }
        }

        if (gameObject.name.Contains("2")) //첫번째 테이블 오른쪽
        {
            Menu.instance.seatNum = 1;
            switch (SmallFade.instance.SittingCharacter[1].name)
            {
                case "sBear":
                    Menu.instance.cNum = 1;
                    break;
                case "sBread":
                    Menu.instance.cNum = 3;
                    break;
                case "sDdorong":
                    Menu.instance.cNum = 5;
                    break;
                case "sFamilySeries":
                    Menu.instance.cNum = 7;
                    break;
                case "sDog":
                    Menu.instance.cNum = 9;
                    break;
                case "sNoName":
                    Menu.instance.cNum = 11;
                    break;
                case "sDinosour":
                    Menu.instance.cNum = 13;
                    break;
                case "sGrandfather":
                    Menu.instance.cNum = 15;
                    break;
                case "sSoldier":
                    Menu.instance.cNum = 10;
                    break;
            }
        }

        if (gameObject.name.Contains("4"))
        {
            Menu.instance.seatNum = 3;
            switch (SmallFade.instance.SittingCharacter[3].name)
            {
                case "sBear":
                    Menu.instance.cNum = 1;
                    break;
                case "sBread":
                    Menu.instance.cNum = 3;
                    break;
                case "sDdorong":
                    Menu.instance.cNum = 5;
                    break;
                case "sFamilySeries":
                    Menu.instance.cNum = 7;
                    break;
                case "sDog":
                    Menu.instance.cNum = 9;
                    break;
                case "sNoName":
                    Menu.instance.cNum = 11;
                    break;
                case "sDinosour":
                    Menu.instance.cNum = 13;
                    break;
                case "sGrandfather":
                    Menu.instance.cNum = 15;
                    break;
                case "sSoldier":
                    Menu.instance.cNum = 10;
                    break;
            }
        }

        if (gameObject.name.Contains("6"))
        {
            Menu.instance.seatNum = 5;
            switch (SmallFade.instance.SittingCharacter[5].name)
            {
                case "sBear":
                    Menu.instance.cNum = 1;
                    break;
                case "sBread":
                    Menu.instance.cNum = 3;
                    break;
                case "sDdorong":
                    Menu.instance.cNum = 5;
                    break;
                case "sFamilySeries":
                    Menu.instance.cNum = 7;
                    break;
                case "sDog":
                    Menu.instance.cNum = 9;
                    break;
                case "sNoName":
                    Menu.instance.cNum = 11;
                    break;
                case "sDinosour":
                    Menu.instance.cNum = 13;
                    break;
                case "sGrandfather":
                    Menu.instance.cNum = 15;
                    break;
                case "sSoldier":
                    Menu.instance.cNum = 10;
                    break;
            }
        }
        Menu.instance.ClickMenuHint();
       // Debug.Log("함수 NowHintBubble");
    }

    public void ClickMenu()
    {
        switch(gameObject.name)
        {
            case "OrangeJuice":
                Menu.instance.MenuServingFunction(1);
                //Debug.Log("함수 ClickMenu1");
                break;
            case "HotChoco":
                Menu.instance.MenuServingFunction(2);
                //Debug.Log("함수 ClickMenu2");
                break;
            case "GreenTeaSmoothie":
                Menu.instance.MenuServingFunction(3);
                //Debug.Log("함수 ClickMenu3");
                break;
            case "FruitsAde":
                Menu.instance.MenuServingFunction(4);
                //Debug.Log("함수 ClickMenu4");
                break;
            case "StrawberrySmoothie":
                Menu.instance.MenuServingFunction(5);
                //Debug.Log("함수 ClickMenu5");
                break;
            case "BlueberryYogurt":
                Menu.instance.MenuServingFunction(6);
               // Debug.Log("함수 ClickMenu6");
                break;
            case "Pancake":
                Menu.instance.MenuServingFunction(7);
               // Debug.Log("함수 ClickMenu7");
                break;
            case "CheeseCake":
                Menu.instance.MenuServingFunction(8);
               // Debug.Log("함수 ClickMenu8");
                break;
        }
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
           // Debug.Log(n + "자리 앉은 캐릭터");
            switch(SmallFade.instance.SittingCharacter[n].name)
            {
                case "sBear":
                    SmallFade.instance.smallFOut.Enqueue(1);
                    CharacterVisit.instance.revisit.Enqueue(1);
                   // Debug.Log("페이드아웃 될 캐릭터 도리");
                    break;
                case "sCar":
                    SmallFade.instance.smallFOut.Enqueue(2);
                    CharacterVisit.instance.revisit.Enqueue(2);
                    //Debug.Log("페이드아웃 될 캐릭터 붕붕");
                    break;
                case "sBread":
                    SmallFade.instance.smallFOut.Enqueue(3);
                    CharacterVisit.instance.revisit.Enqueue(3);
                   // Debug.Log("페이드아웃 될 캐릭터 빵빵");
                    break;
                case "sRabbit":
                    SmallFade.instance.smallFOut.Enqueue(4);
                    CharacterVisit.instance.revisit.Enqueue(4);
                   // Debug.Log("페이드아웃 될 캐릭터 개나리");
                    break;
                case "sDdorong":
                    SmallFade.instance.smallFOut.Enqueue(5);
                    CharacterVisit.instance.revisit.Enqueue(5);
                   // Debug.Log("페이드아웃 될 캐릭터 또롱");
                    break;
                case "sPrincess":
                    SmallFade.instance.smallFOut.Enqueue(6);
                    if (Dialogue1.instance.CharacterDC[10] != 3)//찰스2이벤트 이후가 아니면
                    {
                        CharacterVisit.instance.revisit.Enqueue(6);
                    }
                    else if(Dialogue1.instance.CharacterDC[10] == 3)//이벤트 후면
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
                case "sFamilySeries":
                    SmallFade.instance.smallFOut.Enqueue(7);
                    CharacterVisit.instance.revisit.Enqueue(7);
                   // Debug.Log("페이드아웃 될 캐릭터 루루");
                    break;
                case "sSunflower":
                    SmallFade.instance.smallFOut.Enqueue(8);
                    CharacterVisit.instance.revisit.Enqueue(8);
                    //Debug.Log("페이드아웃 될 캐릭터 샌디");
                    break;
                case "sDog":
                    SmallFade.instance.smallFOut.Enqueue(9);
                    CharacterVisit.instance.revisit.Enqueue(9);
                   // Debug.Log("페이드아웃 될 캐릭터 친구");
                    break;
                case "sSoldier":
                    SmallFade.instance.smallFOut.Enqueue(10);
                    if (CharacterAppear.instance.eventOn != 11)//찰스2 이벤트 중이 아니면
                    {
                        if (Dialogue1.instance.CharacterDC[10] == 3)//찰스2 이벤트 뒤면
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
                case "sNoName":
                    SmallFade.instance.smallFOut.Enqueue(11);                  
                    CharacterVisit.instance.revisit.Enqueue(11);
                   // Debug.Log("페이드아웃 될 캐릭터 무명이");
                    break;
                case "sHero":
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
                case "sDinosour":
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
                case "sPenguin":
                    SmallFade.instance.smallFOut.Enqueue(14);
                    CharacterVisit.instance.revisit.Enqueue(14);
                   // Debug.Log("페이드아웃 될 캐릭터 닥터펭");
                    break;
                case "sGrandfather":
                    SmallFade.instance.smallFOut.Enqueue(15);
                    CharacterVisit.instance.revisit.Enqueue(15);
                   // Debug.Log("페이드아웃 될 캐릭터 롤렝드");
                    break;
                case "sBabyLeft":
                    SmallFade.instance.smallFOut.Enqueue(17);
                   // Debug.Log("페이드아웃 될 캐릭터 주인공 아기");
                    break;
                case "sBabyRight":
                    SmallFade.instance.smallFOut.Enqueue(16);
                  //  Debug.Log("페이드아웃 될 캐릭터 주인공 아기");
                    break;
            }

            if (SmallFade.instance.SittingCharacter[n].name != "sHero" && SmallFade.instance.SittingCharacter[n].name != "sDinosour")
            {//히로디노가 아니고
                if(Dialogue1.instance.CharacterDC[10] != 3 || (Dialogue1.instance.CharacterDC[10] == 3 && SmallFade.instance.SittingCharacter[n].name != "sPrincess" && SmallFade.instance.SittingCharacter[n].name != "sSoldier"))
                {//찰스 이벤트가 다 안 끝났거나 끝났어도 찰스,도로시가 아니면
                    SmallFade.instance.cleanSeat.Enqueue(n); //비워질 자리 큐에 정보 추가
                   // Debug.Log(n + "자리 클린시트큐에 추가됨");
                }       
                else if(Dialogue1.instance.CharacterDC[10] == 3 && (SmallFade.instance.SittingCharacter[n].name == "sPrincess" || SmallFade.instance.SittingCharacter[n].name == "sSoldier"))
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

    public void WhatStar()//클릭된 별이 어떤 별인지 구분하여 별 페이드아웃 큐에 추가
    {
        if (gameObject.name.Contains("(1)"))
        {
            Star.instance.starFadeOut.Enqueue(0);
        }

        if (gameObject.name.Contains("(2)"))
        {
            Star.instance.starFadeOut.Enqueue(1);
        }

        if (gameObject.name.Contains("(3)"))
        {
            Star.instance.starFadeOut.Enqueue(2);
        }

        if (gameObject.name.Contains("(4)"))
        {
            Star.instance.starFadeOut.Enqueue(3);
        }

        if (gameObject.name.Contains("(5)"))
        {
            Star.instance.starFadeOut.Enqueue(4);
        }

        if (gameObject.name.Contains("(6)"))
        {
            Star.instance.starFadeOut.Enqueue(5);
        }

        if (gameObject.name.Contains("(7)"))
        {
            Star.instance.starFadeOut.Enqueue(6);
        }

        Star.instance.StartCoroutine(Star.instance.PlusStar());
    }

    public void ClickPage()//손님노트의 페이지 클릭했을 때
    {
        switch (gameObject.name)
        {
            case "Page1":
                VisitorNote.instance.pageNum2 = 1; //현재 페이지 넘버
                break;
            case "Page2":
                VisitorNote.instance.pageNum2 = 2; //현재 페이지 넘버
                break;
            case "Page3":
                VisitorNote.instance.pageNum2 = 3; //현재 페이지 넘버
                break;
            case "Page4":
                VisitorNote.instance.pageNum2 = 4; //현재 페이지 넘버
                break;
            case "Page5":
                VisitorNote.instance.pageNum2 = 5; //현재 페이지 넘버
                break;
            case "Page6":
                VisitorNote.instance.pageNum2 = 6; //현재 페이지 넘버
                break;
            case "Page7":
                VisitorNote.instance.pageNum2 = 7; //현재 페이지 넘버
                break;
            case "Page8":
                VisitorNote.instance.pageNum2 = 8; //현재 페이지 넘버
                break;
            case "Page9":
                VisitorNote.instance.pageNum2 = 9; //현재 페이지 넘버
                break;
            case "Page10":
                VisitorNote.instance.pageNum2 = 10; //현재 페이지 넘버
                break;
            case "Page11":
                VisitorNote.instance.pageNum2 = 11; //현재 페이지 넘버
                break;
            case "Page12":
                VisitorNote.instance.pageNum2 = 12; //현재 페이지 넘버
                break;
            case "Page13":
                VisitorNote.instance.pageNum2 = 13; //현재 페이지 넘버
                break;
            case "Page14":
                VisitorNote.instance.pageNum2 = 14; //현재 페이지 넘버
                break;
        }
        VisitorNote.instance.TurnToPage();
    }

    public void WhichEventReplay()//어떤 캐릭터의 이벤트 다시보기 버튼을 눌렀는지
    {

        if (gameObject.name.Contains("1"))//도리이면
        {
            VisitorNote.instance.whichStory.text = "도리와의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 1;
        }
        if (gameObject.name.Contains("2"))
        {
            VisitorNote.instance.whichStory.text = "붕붕과의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 2;
        }
        if (gameObject.name.Contains("3"))
        {
            VisitorNote.instance.whichStory.text = "빵빵과의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 3;
        }
        if (gameObject.name.Contains("4"))
        {
            VisitorNote.instance.whichStory.text = "개나리와의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 4;
        }
        if (gameObject.name.Contains("5"))
        {
            VisitorNote.instance.whichStory.text = "또롱과의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 5;
        }
        if (gameObject.name.Contains("6"))
        {
            VisitorNote.instance.whichStory.text = "도로시와의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 6;
        }
        if (gameObject.name.Contains("7"))
        {
            VisitorNote.instance.whichStory.text = "루루와의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 7;
        }
        if (gameObject.name.Contains("8"))
        {
            VisitorNote.instance.whichStory.text = "샌디와의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 8;
        }
        if (gameObject.name.Contains("9"))
        {
            VisitorNote.instance.whichStory.text = "친구와의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 9;
        }
        if (gameObject.name.Contains("10"))
        {
            VisitorNote.instance.whichStory.text = "찰스와의 이벤트1을 회상할까요?";
            VisitorNote.instance.evRP = 10;
        }
        if (gameObject.name.Contains("11"))
        {
            VisitorNote.instance.whichStory.text = "찰스와의 이벤트2를 회상할까요?";
            VisitorNote.instance.evRP = 11;
        }
        if (gameObject.name.Contains("12"))
        {
            VisitorNote.instance.whichStory.text = UI_Assistant1.instance.namedName + "와(과)의 이벤트1을 회상할까요?";
            VisitorNote.instance.evRP = 12;
        }
        if (gameObject.name.Contains("13"))
        {
            VisitorNote.instance.whichStory.text = UI_Assistant1.instance.namedName + "와(과)의 이벤트2를 회상할까요?";
            VisitorNote.instance.evRP = 13;
        }
        if (gameObject.name.Contains("14"))
        {
            VisitorNote.instance.whichStory.text = "히로&디노와의\n이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 14;
        }
        if (gameObject.name.Contains("15"))
        {
            VisitorNote.instance.whichStory.text = "닥터 펭과의\n이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 15;
        }
        if (gameObject.name.Contains("16"))
        {
            VisitorNote.instance.whichStory.text = "롤렝드와의 이벤트를 회상할까요?";
            VisitorNote.instance.evRP = 16;
        }
        VisitorNote.instance.rePlayMessage.SetActive(true);
    }
}
