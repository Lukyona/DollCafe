using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallFade : MonoBehaviour //작은 캐릭터 스크립트
{
    public static SmallFade instance;
    public GameObject[] SmallCharacter; //작은 캐릭터 이미지 배열
    public GameObject[] SittingCharacter = new GameObject[6];//좌석별 현재 앉아있는 캐릭터, 
  
    public int[] TableEmpty = new int[3]; //테이블이 비었는지 확인

    public int[] CharacterSeat = new int[15]; // 캐릭터 자리 저장 배열

    public Queue<int> smallFOut = new Queue<int>(); //작은 캐릭터 페이드아웃 시 사용
    public Queue<int> smallFadeIn = new Queue<int>(); //페이드인 시 사용

    public bool x1 = false;//히로, 디노 중 먼저 가는 캐릭터가 true
    public bool x2 = false; //찰스 도로시용

    public Image soldier2;//도로시와 같이 오는 찰스 이미지(오른쪽)
    public Image nameless2;

    Vector3[] Seat = new Vector3[6]; //작은 캐릭터가 앉을 자리 배열

    void Awake()
    {        
        if(instance == null)
        {
            instance = this;
        }        
    }

    void Start()
    {
        Seat[0] = new Vector3(-755,-210,0);
        Seat[1] = new Vector3(-140,-225,0);
        Seat[2] = new Vector3(-320,-390,0);
        Seat[3] = new Vector3(390,-390,0);
        Seat[4] = new Vector3(210,-200,0);
        Seat[5] = new Vector3(790,-230,0);
    }

    public GameObject GetSmallCharacter(int cNum)
    {
        return SmallCharacter[cNum];
    }

    public void CantTouchCharacter(int n) //캐릭터 클릭 못하게
    {
        SmallCharacter[n].GetComponent<Button>().interactable = false;
    }

    public void CanClickCharacter(int n) //캐릭터 클릭 가능하게, 페이드인하고 가능
    {
        SmallCharacter[n].GetComponent<Button>().interactable = true;
    }

    public void SetCharacter(int cNum) //작은 캐릭터 설정, cNum은 캐릭터 넘버
    {
        smallFadeIn.Enqueue(cNum);

        if (cNum == 0) // 제제는 좌석에 앉지 않으므로 페이드인
        {
            Invoke(nameof(FadeIn),1f);
            return;
        }

        Debug.Log("셋캐릭터 " + cNum);
        
        SetSeatPosition(cNum);

        if (SystemManager.instance.GetMainCount() == 2)//서빙 튜토리얼일 경우
        {
            //회색 패널보다 레이어가 뒤에 위치해있어서 터치가 안되므로 일시적으로 순서를 패널보다 앞쪽으로 배치한다
            SmallCharacter[1].transform.parent.GetComponent<Canvas>().sortingOrder = 7;
        }

        //Debug.Log("함수 SetCharacter");
    }


    int sn = 0; //자리 넘버
    public void SetSeatPosition(int cNum) //앉을 자리 설정
    {
        if (SystemManager.instance.GetMainCount() == 2)//서빙 튜토리얼일 경우
        {
            sn = 1; //2번째 자리, 첫번째 짝수자리, 첫번째 테이블 오른쪽 자리
        }
        else//튜토리얼 아닐 시 자리 배정
        {
            if(cNum == 13)//캐릭터가 디노면, 히로 자리에 따라서 옆에 배정
            {
                sn = CharacterSeat[11] + 1;
            }
            else if(cNum == 10 && (Dialogue.instance.CharacterDC[10] == 3) || (CharacterAppear.instance.eventOn == 11 && UI_Assistant1.instance.getMenu == 1))//찰스2이벤트 중 찰스 중간 페이드인
            {//도로시와 같이 오는 찰스
                sn = CharacterSeat[5] + 1;
                SmallCharacter[10].GetComponent<Image>().sprite = soldier2.sprite;//이미지 변경
            }          
            else // 혼자인 캐릭터 자리 설정
            {
                if (cNum == 11 && Dialogue.instance.CharacterDC[11] == 3)//무명이, 시나리오 다 봤을 때
                {
                    SmallCharacter[11].GetComponent<Image>().sprite = nameless2.sprite;//이미지 변경
                }

                if (TableEmpty[0] == 0 && TableEmpty[1] == 0 && TableEmpty[2] == 0) //모든 테이블이 빈 상태면
                {
                    //Debug.Log("테이블이 모두 비었음");
                    sn = Random.Range(0, 5); // 0~5번 자리 모두 착석 가능
                    if (cNum % 2 == 1) //홀수 번호 캐릭터면 오른쪽(1,3,5)에 앉음
                    {
                        if(sn % 2 == 0) sn += 1; // 짝수가 나오면 1 더해서 홀수 만들기
                    }
                    else if (cNum % 2 == 0) //짝수 번호 캐릭터는 왼쪽(0,2,4)에 앉음
                    {
                        if(sn % 2 == 1) sn -= 1; // 홀수가 나오면 1 빼서 홀수 만들기
                    }
                }
                else if (TableEmpty[0] == 0 && TableEmpty[1] == 0) //1,2번 테이블만 빈 상태면
                {
                    //Debug.Log("1,2테이블이 비었음");
                    sn = Random.Range(0, 3); //0~3번 자리 착석 가능
                    if (cNum % 2 == 1) 
                    {
                        if(sn % 2 == 0) sn += 1;
                    }
                    if (cNum % 2 == 0) 
                    {
                        if(sn % 2 == 1) sn -= 1;
                    }
                }
                else if (TableEmpty[0] == 0 && TableEmpty[2] == 0)//1,3번 테이블만 빈 상태면
                {
                    //Debug.Log("1,3테이블이 비었음");
                    sn = Random.Range(0, 5); // 2,3번은 착석 불가
                    if (cNum % 2 == 1) 
                    {
                        if(sn == 2) sn -= 1;
                        else if(sn == 3) sn += 2;
                        else if(sn % 2 == 0) sn += 1;
                    }
                    if (cNum % 2 == 0) 
                    {
                        if(sn == 2) sn -= 2;
                        else if(sn == 3) sn += 1;
                        else if(sn % 2 == 1) sn -= 1;
                    }
                }
                else if (TableEmpty[1] == 0 && TableEmpty[2] == 0) //2,3번 테이블만 빈 상태면
                {
                  //  Debug.Log("2,3테이블이 비었음");
                    sn = Random.Range(2, 5);

                    if (cNum % 2 == 1) 
                    {
                        if(sn % 2 == 0) sn += 1;
                    }

                    if (cNum % 2 == 0) 
                    {
                        if(sn % 2 == 1) sn -= 1;
                    }
                }
                else if (TableEmpty[0] == 0) //1번 테이블만 빈 상태
                {
                  //  Debug.Log("1테이블만 비었음");
                    if (cNum % 2 == 1) 
                    {
                        sn = 1;
                    }

                    if (cNum % 2 == 0) 
                    {
                        sn = 0;
                    }
                }
                else if (TableEmpty[1] == 0) //2번 테이블만 빈 상태
                {
                   // Debug.Log("2테이블만 비었음");
                    if (cNum % 2 == 1) 
                    {
                        sn = 3;
                    }

                    if (cNum % 2 == 0) 
                    {
                        sn = 2;
                    }
                }
                else if (TableEmpty[2] == 0) //3번 테이블만 비었을 때
                {
                    //Debug.Log("3테이블만 비었음");
                    if (cNum % 2 == 1) 
                    {
                        sn = 5;
                    }

                    if (cNum % 2 == 0) 
                    {
                        sn = 4;
                    }
                }
            }              
        }
        
        CharacterSeat[cNum - 1] = sn; //캐릭터 자리 정보 저장, 
        //Debug.Log("캐릭넘버 " + cNum + " 자리: " + sn);

        // 앉을 자리로 옮기기
        SmallCharacter[cNum].transform.position = Seat[sn];
        SittingCharacter[sn] = SmallCharacter[cNum];
        switch (sn) // 테이블 착석 여부 갱신, 1은 착석, 0은 비었음
        {
            case 0: 
            case 1:
                TableEmpty[0] = 1;
                break;
            case 2:
            case 3:
                TableEmpty[1] = 1;
                break;
            case 4:
            case 5:
                TableEmpty[2] = 1;
                break;
        }
        
        int n;
        if(cNum > 12)//디노부터~
        {
            n = cNum - 1;
        }
        else
        {
            n = cNum;
        }

        if(SystemManager.instance.GetMainCount() > 2 && Dialogue.instance.CharacterDC[n] != 0 && CharacterAppear.instance.eventOn != 2)
        {//붕붕이 친밀도 이벤트, 캐릭터 첫 등장 제외하고 메인카운트 3이상이면 바로 캐릭터 페이드인
            if(IsInvoking("FadeIn"))//다른 캐릭터 페이드인이 인보크 중이면
            {
                Invoke("FadeIn", 0.7f); //0.7초 뒤 실행
            }
            else
            {
                FadeIn();        
            }          
        }

        if (CharacterAppear.instance.eventOn == 2)//특정 캐릭터들은 바로 시나리오 이벤트 시작
        {
            SystemManager.instance.BeginDialogue(CharacterAppear.instance.eventOn);
        }
        if (CharacterAppear.instance.eventOn != 0 && CharacterAppear.instance.eventOn != 14 && UI_Assistant1.instance.getMenu != 1)//히로디노이벤트가 아니면 주인공 아기 캐릭터 자리 설정, 두번째 조건은 찰스2 이벤트 때문
        {
            SetBabySeat(sn);          
        }
        //Debug.Log("함수 SetSeatPosition");
    }

    public void SetBabySeat(int s)
    {
        if(s % 2 == 0) // 캐릭터 자리가 왼쪽(짝수)면
        {
            SmallCharacter[16].transform.position = Seat[s+1]; //주인공 아기를 오른쪽 자리로 옮김
            SittingCharacter[s+1] = SmallCharacter[16];
        }
        else
        {
            SmallCharacter[17].transform.position = Seat[s-1];
            SittingCharacter[s-1] = SmallCharacter[17];
        }
    }

    public Queue<int> cleanSeat = new Queue<int>(); //비워질 자리 정보 큐
    public void CleanTable() // 손님이 가고 테이블을 빈 걸로 표시
    {
        int fNum = cleanSeat.Peek();
        switch (fNum)
        {
            case 0:
            case 1:
                TableEmpty[0] = 0;
                break;
            case 2:
            case 3:
                TableEmpty[1] = 0;
                break;
            case 4:
            case 5:
                TableEmpty[2] = 0;
                break;
        }
        //Debug.Log(fNum + "자리 비워짐");
        if(SystemManager.instance.GetMainCount() > 7)//메인카운트가 7이상이면
        {
            CharacterVisit.instance.Invoke("CanRevisit", 8f);//6초 뒤 재방문 가능
        }
        else
        {
            CharacterVisit.instance.Invoke("CanRevisit", 3f);
        }
        //Debug.Log("함수 CleanTable");       
        cleanSeat.Dequeue();
    }

    bool sFade = false;//페이드인 진행 상태 구분에 사용
    public void FadeIn() //작은 캐릭터 페이드인
    {
        if(sFade)
        {
            Invoke("FadeIn", 0.3f);
        }
        else
        {
            StartCoroutine(FadeToFullAlpha()); //페이드인 시작
            //Debug.Log("함수 SmallFadeIn");
        }     
    }

    public void FadeOutJeje()
    {
        smallFOut.Enqueue(0);
        StartCoroutine(FadeToZero());//페이드아웃 시작         
    }

    public void FadeOut() //작은 캐릭터 페이드아웃
    {   
        int n = Menu.instance.seatInfo.Peek();
        string charName = instance.SittingCharacter[n].name;

        // 캐릭터 오브젝트 이름 예시 : small_1Bear
        int idx = charName.IndexOf("_");

        idx = TouchableObject.instance.GetNumber(idx, charName);

        switch(idx)
        {
            case 6: // princess 도로시
                smallFOut.Enqueue(6);
                if (Dialogue.instance.CharacterDC[10] != 3)//찰스2이벤트 이후가 아니면
                {
                    CharacterVisit.instance.revisit.Enqueue(6);
                }
                else if(Dialogue.instance.CharacterDC[10] == 3)//이벤트 후면
                {
                    if (!x2)
                    {
                        x2 = true;
                    }
                    else if (x2)
                    {
                        x2 = false;
                    }
                }
                //Debug.Log("페이드아웃 될 캐릭터 도로시");
                break;
            case 10: // soldier 찰스
                smallFOut.Enqueue(10);
                if (CharacterAppear.instance.eventOn != 11)//찰스2 이벤트 중이 아니면
                {
                    if (Dialogue.instance.CharacterDC[10] == 3)//찰스2 이벤트 뒤면
                    {
                        if (!x2)
                        {
                            x2 = true;
                        }
                        else if (x2)
                        {
                            x2 = false;
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
                if(!x1)//히로가 먼저 가는 거면
                {
                    x1 = true;
                }
                else if(x1)//디노가 먼저 갔으면
                {
                    x1 = false; //값을 0으로 만들고
                }
                smallFOut.Enqueue(12);
                // Debug.Log("페이드아웃 될 캐릭터 히로");
                break;
            case 13: // dinosour 디노
                if (!x1)
                {
                    x1 = true;
                }
                else if (x1)
                {
                    x1 = false;
                }
                smallFOut.Enqueue(13); 
                //  Debug.Log("페이드아웃 될 캐릭터 디노");
                break;

            default: // 오브젝트 이름에 _이 들어가지 않아 idx가 -1인 경우 or 별다른 처리가 필요없는 캐릭터일 경우
                if(charName == "sBabyLeft")
                {
                    smallFOut.Enqueue(17);
                }
                else if(charName == "sBabyRight")
                {
                    smallFOut.Enqueue(16);
                }
                else
                {
                    smallFOut.Enqueue(idx);
                    CharacterVisit.instance.revisit.Enqueue(idx);
                }
                break;
        }

        if (charName != "small_12Hero" && charName != "small_13Dinosour")
        { //히로디노가 아니고
            if(Dialogue.instance.CharacterDC[10] != 3 || (Dialogue.instance.CharacterDC[10] == 3 && charName != "small_6Princess" && charName != "small_10Soldier"))
            {//찰스 이벤트가 다 안 끝났거나 끝났어도 찰스,도로시가 아니면
                cleanSeat.Enqueue(n); //비워질 자리 큐에 정보 추가
                 //Debug.Log(n + "자리 클린시트큐에 추가됨");
            }       
            else if(Dialogue.instance.CharacterDC[10] == 3 && (charName == "small_6Princess" || charName == "small_10Soldier"))
            {//찰스도로시 중 찰스나 도로시일 때
                if (!x2)//값이 0이어야만 가능
                {
                    cleanSeat.Enqueue(n); //비워질 자리 큐에 정보 추가
                    CharacterVisit.instance.revisit.Enqueue(17);
                }
            }
        }
        else // 히로 혹은 디노일 경우
        {
            if (!x1)//값이 0이어야만 가능
            {
                cleanSeat.Enqueue(n); //비워질 자리 큐에 정보 추가
                //  Debug.Log(n + "자리 클린시트큐에 추가됨");
                CharacterVisit.instance.revisit.Enqueue(13);
            }
        }       
        Menu.instance.seatInfo.Dequeue();
        SittingCharacter[n] = null;//버그 대비 페이드아웃 시 null 넣기
    
        StartCoroutine(FadeToZero());//페이드아웃 시작         
        //Debug.Log("함수 SmallFadeOut");
    }

    public IEnumerator FadeToFullAlpha() // 알파값 0에서 1로 전환
    {
        sFade = true;
        int v = smallFadeIn.Peek();
       
         Debug.Log("페이드인 될 캐릭터" + v);

        SmallCharacter[v].SetActive(true); //작은 캐릭터 활성화      

        SmallCharacter[v].GetComponent<Image>().color = new Color(SmallCharacter[v].GetComponent<Image>().color.r,SmallCharacter[v].GetComponent<Image>().color.g,SmallCharacter[v].GetComponent<Image>().color.b, 0);
        while (SmallCharacter[v].GetComponent<Image>().color.a < 1.0f)
        {
           SmallCharacter[v].GetComponent<Image>().color = new Color(SmallCharacter[v].GetComponent<Image>().color.r,SmallCharacter[v].GetComponent<Image>().color.g,SmallCharacter[v].GetComponent<Image>().color.b,SmallCharacter[v].GetComponent<Image>().color.a + (Time.deltaTime / 0.8f));
            yield return null;
        }
        Debug.Log("페이드인 됨" + v);
        
        smallFadeIn.Dequeue();

        if ((CharacterAppear.instance.eventOn == 0 || CharacterAppear.instance.eventOn == 9 || CharacterAppear.instance.eventOn == 12 || CharacterAppear.instance.eventOn == 16) 
            && v != 0 && v != 17 && v != 16)
        {//친밀도 이벤트가 아니고 제제가 아니고, 주인공 아기가 아닐 때만 캐릭터 클릭 가능, 친구,무명이1,롤렝드 친밀도 이벤트시에는 가능,
            CanClickCharacter(v);
        }
        
        if(CharacterAppear.instance.eventOn == 10 || CharacterAppear.instance.eventOn == 11)//찰스1,2 이벤트일 경우
        {
            if(v == 6)//도로시 페이드인했으면
            {
                SetCharacter(10); //찰스 페이드인
            }
            if(CharacterAppear.instance.eventOn == 10 && v == 10)//찰스만 클릭가능
            {
                CanClickCharacter(10);
            }
        }

        if(v == 12)//히로 페이드인 끝나면 디노 빨리 나타나게끔
        {
            if (CharacterAppear.instance.eventOn == 14)//히로디노 친밀도 이벤트 시
            {
                CanClickCharacter(v);//히로 먼저 클릭 가능
            }
            SetCharacter(13);           
        }
        if(Dialogue.instance.CharacterDC[10] == 3 && v == 6)//찰스와 같이 방문한 도로시 페이드인 끝나면 찰스 나타나게끔
        {
            SetCharacter(10);
        }

        if(CharacterAppear.instance.eventOn != 0 && CharacterAppear.instance.eventOn != 10 && CharacterAppear.instance.eventOn != 11 && v != 16 && v != 17 && v != 2 && v != 9 
            || (CharacterAppear.instance.eventOn == 11 && v == 10))
        {//친밀도 이벤트가 진행 중이고, 주인공 아기가 아니고, 특정 캐릭터(이벤트)가 아닐때
            if (CharacterAppear.instance.eventOn <= 10)
            {
                SystemManager.instance.BeginDialogue(CharacterAppear.instance.eventOn); //해당 캐릭터 시나리오 등장
            }
            else if (CharacterAppear.instance.eventOn >= 11 && CharacterAppear.instance.eventOn <= 13)//이벤트 넘버 11~13
            {
                if (UI_Assistant1.instance.getMenu == 0 && CharacterAppear.instance.eventOn != 12)//첫번째 조건은 찰스2 이벤트 때문, 두번째 조건은 무명이1 이벤트때문
                {
                    SystemManager.instance.BeginDialogue(v);
                }           
            }
            else if (CharacterAppear.instance.eventOn == 15)//닥터 펭 친밀도 이벤트의 경우
            {
                int v2 = v - 1;
                SystemManager.instance.BeginDialogue(v2);              
            }
        }
        sFade = false;
    }

    public IEnumerator FadeToZero()  // 알파값 1에서 0으로 전환
    {
        int cNum = smallFOut.Peek();
        SmallCharacter[cNum].GetComponent<Image>().color = new Color(SmallCharacter[cNum].GetComponent<Image>().color.r, SmallCharacter[cNum].GetComponent<Image>().color.g, SmallCharacter[cNum].GetComponent<Image>().color.b, 1);
        while (SmallCharacter[cNum].GetComponent<Image>().color.a > 0.0f)
        {
            SmallCharacter[cNum].GetComponent<Image>().color = new Color(SmallCharacter[cNum].GetComponent<Image>().color.r, SmallCharacter[cNum].GetComponent<Image>().color.g, SmallCharacter[cNum].GetComponent<Image>().color.b, SmallCharacter[cNum].GetComponent<Image>().color.a - (Time.deltaTime / 0.8f));
            yield return null;
        }
        //Debug.Log("페이드아웃 됨" + cNum);
        smallFOut.Dequeue();

        if (cNum != 0) //제제가 아닐 경우
        {
            SmallCharacter[cNum].SetActive(false);//캐릭터 비활성화
            if(cNum != 16 && cNum != 17)//아기 제외
            {
                CharacterSeat[cNum - 1] = 0;//자리 0으로 초기화

                if (cNum == 6 || cNum == 10)//도로시나 찰스일 경우
                {
                    if ((Dialogue.instance.CharacterDC[10] == 3 && !x2) || CharacterAppear.instance.eventOn == 11)//찰스 때만 실행, 도로시 땐 실행 안 함
                    {//찰스2이벤트 이후면 둘 중에 마지막에 나오는 캐릭터 때 실행 혹은 찰스2이벤트 중간의 찰스일 때
                        CleanTable();
                    }
                    else if (Dialogue.instance.CharacterDC[10] != 3)//찰스2 이벤트 전이면 다른 캐릭터와 똑같이 실행
                    {
                        CleanTable();
                    }
                }
                else if(cNum == 12 || cNum == 13)
                {
                    if (!x1)//히로디노는 마지막에 나오는 캐릭터 때 실행
                    {
                        CleanTable();
                    }
                }
                else //혼자인 캐릭터
                {
                    CleanTable();
                }           
            }
            if (smallFOut.Count != 0)
            {
                StartCoroutine(FadeToZero());
            }
        }
        
        if(CharacterAppear.instance.eventOn == 0 && UI_Assistant1.instance.getMenu == 2)//친밀도 이벤트가 끝났을 때
        {
            switch(cNum)//해당 캐릭터에 따라서 주인공 아기 이미지 페이드아웃
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 9:
                case 11:
                case 15:
                    smallFOut.Enqueue(17);
                    break;
                case 2:
                case 4:
                case 6:
                case 8:
                case 10:
                case 14:
                    smallFOut.Enqueue(16);
                    break;
                case 13://디노 페이드아웃하고 서빙 넘버 초기화
                    Menu.instance.dinoOk = false;
                    Menu.instance.heroOk = false;
                    break;
            }
            if(cNum != 12)//히로만 아니면
            {
                UI_Assistant1.instance.getMenu = 0;
            }           
            if(cNum != 12 && cNum != 13)//히로디노만 아니면
            {
                StartCoroutine(FadeToZero());//페이드아웃 시작
            }            
        }
    }
}
