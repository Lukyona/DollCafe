using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHint : MonoBehaviour
{
    public static MenuHint instance;

    [System.Serializable] // 클래스나 구조체를 에디터 인스펙터에 노출
    class TextList
    {
        public string[] MHtext;
    }   
    [SerializeField] TextList[] MHTextList; //문자열 배열을 가지고 있는 배열

    public GameObject[] MH; //메뉴힌트 말풍선

    private GameObject[] HintBubble = new GameObject[6]; // 메뉴 힌트 말풍선
    private Text[] HintText = new Text[6]; // 메뉴 힌트 말풍선 메세지
    

    public GameObject tutorialBubble; //서빙 튜토리얼
    public Text tutorialText;

    public bool tuto = false; // true면 서빙튜토리얼 상황

    public int[] RightMenu = new int[6]; //그 자리의 캐릭터가 원하는 메뉴 넘버 

    Queue<int> mhFade = new Queue<int>();//페이드인 시 사용

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        for(int i = 0; i < MH.Length; i++)
        {
            MH[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
        }
    }

    public void HintFadeOut(int num)//메뉴힌트 페이드아웃
    {
        StartCoroutine(FadeToZero(num)); //페이드아웃 시작
        InActiveMH(num); //페이드아웃된 메뉴 힌트 비활성화
    }

    int seatNum = 0;

    public void InActiveMH(int n) //메뉴 힌트 비활성화
    {
        HintBubble[n].SetActive(false);
    }

    public void ActiveMH() //메뉴 힌트 활성화
    {
        if(tuto)
        {
            HintBubble[seatNum].GetComponent<Button>().interactable = true;
        }
        else
        {
            HintBubble[m].SetActive(true);
        }       
    }

    public void CantClickMHB()//메뉴힌트버블 터치 불가, 캐릭터 이벤트 시 실행
    {
        for(int i = 0; i < 6; i++)
        {
            if (HintBubble[i] != null)
            {
                HintBubble[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    public void CanClickMHB()//메뉴힌트버블 터치 가능, 캐릭터 이벤트 끝나고 실행
    {
        for (int i = 0; i < 6; i++)
        {
            if(HintBubble[i] != null)
            {
                HintBubble[i].GetComponent<Button>().interactable = true;
            }
        }
    }

    public void SetMHB(int num) //어떤 메뉴힌트 말풍선 이미지를 쓸 것인지 설정, 자리에 따라 이미지가 조금 다름
    {       
        if(num == 11) // 튜토리얼 중
        {
            HintBubble[1] = tutorialBubble;
            HintText[1] = tutorialText;
            tuto = true; //튜토리얼 상태
            seatNum = 1;
        }
        else
        {
            HintBubble[num] = MH[num]; //말풍선1로 설정
            HintText[num] = MH[num].GetComponentInChildren<Text>(); //말풍선1의 메세지로 설정
            seatNum = num;  
        }
        
        mhFade.Enqueue(seatNum);
       // Debug.Log("메뉴힌트 큐 추가 " + seatNum);
       // Debug.Log("함수 SetMHB");
    }

    int r; //랜덤 텍스트

    public void SetMHText(int cNum,int num) //메뉴 힌트 메세지 설정
    {       
        switch (cNum) // cNum은 캐릭터 넘버
        {
            case 1: //1은 도리
                if(Menu.instance.menu4Open == true) // 메뉴4가 열렸을 경우
                {
                    r = Random.Range(1, 10); // 메세지 2개 중에 랜덤, 1~5까지가 첫번째, 6~10까지가 두번째
                    
                    if( r >= 5) //두번째 메세지일 경우
                    {
                        RightMenu[num] = 4; //원하는 메뉴는 자몽이므로 4
                        HintText[seatNum].text = MHTextList[0].MHtext[1]; //메뉴 힌트 메세지에 넣기
                    }
                    else if (r <= 4)
                    {
                        RightMenu[num] = 1;
                        HintText[seatNum].text = MHTextList[0].MHtext[0]; //메뉴 힌트 메세지에 넣기
                    }
                }
                else //메뉴4가 열리지 않았으면
                {
                    HintText[seatNum].text = MHTextList[0].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 1;
                }
                break;

            case 2: //2는 붕붕
                HintText[seatNum].text = MHTextList[1].MHtext[0]; //첫번째 메세지로 설정
                RightMenu[num] = 2; //원하는 메뉴는 핫초코
                break;

            case 3: //3은 빵빵
                if (Menu.instance.menu6Open == true) // 메뉴6이 열렸을 경우
                {
                    r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                    if (r >= 5) //두번째 메세지일 경우
                    {
                        RightMenu[num] = 6; //원하는 메뉴는 6
                        HintText[seatNum].text = MHTextList[2].MHtext[1];
                    }
                    else if (r <= 4)
                    {
                        RightMenu[num] = 2;
                        HintText[seatNum].text = MHTextList[2].MHtext[0];
                    }
                }
                else //메뉴6이 열리지 않았으면
                {
                    HintText[seatNum].text = MHTextList[2].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 2;
                }
                break;

            case 4: //4 개나리
                if (Menu.instance.menu4Open == true) 
                {
                    r = Random.Range(1,10); // 메세지 2개 중에 랜덤
                    if (r >= 5) //두번째 메세지일 경우
                    {
                        RightMenu[num] = 4;
                        HintText[seatNum].text = MHTextList[3].MHtext[1];
                    }
                    else if (r <= 4)
                    {
                        RightMenu[num] = 3;
                        HintText[seatNum].text = MHTextList[3].MHtext[0];
                    }
                }
                else 
                {
                    HintText[seatNum].text = MHTextList[3].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 3;
                }
                break;

            case 5: //또롱
                if (Menu.instance.menu6Open == true)
                {
                    r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                    if (r >= 4) //두번째 메세지일 경우, 70%
                    {
                        RightMenu[num] = 6;
                        HintText[seatNum].text = MHTextList[4].MHtext[1];
                    }
                    else if (r <= 3)
                    {
                        RightMenu[num] = 1;
                        HintText[seatNum].text = MHTextList[4].MHtext[0];
                    }
                }
                else
                {
                    HintText[seatNum].text = MHTextList[4].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 1;
                }
                break;

            case 6: //도로시
                if (Menu.instance.menu8Open == true)
                {
                    r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                    if (r >= 5) //두번째 메세지일 경우
                    {
                        RightMenu[num] = 8;
                        HintText[seatNum].text = MHTextList[5].MHtext[1];
                    }
                    else if (r <= 4)
                    {
                        RightMenu[num] = 3;
                        HintText[seatNum].text = MHTextList[5].MHtext[0];
                    }
                }
                else
                {
                    HintText[seatNum].text = MHTextList[5].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 3;
                }
                break;

            case 7: //루루
                if (Menu.instance.menu7Open == true)
                {
                    r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                    if (r >= 6) //두번째 메세지일 경우
                    {
                        RightMenu[num] = 7;
                        HintText[seatNum].text = MHTextList[6].MHtext[1];
                    }
                    else if (r <= 5)
                    {
                        RightMenu[num] = 3;
                        HintText[seatNum].text = MHTextList[6].MHtext[0];
                    }
                }
                else
                {
                    HintText[seatNum].text = MHTextList[6].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 3;
                }
                break;

            case 8: //샌디
                if (Menu.instance.menu7Open == true)
                {
                    r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                    if (r >= 6) //두번째 메세지일 경우
                    {
                        RightMenu[num] = 7;
                        HintText[seatNum].text = MHTextList[7].MHtext[1];
                    }
                    else if (r <= 5)
                    {
                        RightMenu[num] = 4;
                        HintText[seatNum].text = MHTextList[7].MHtext[0];
                    }
                }
                else
                {
                    HintText[seatNum].text = MHTextList[7].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 4;
                }
                break;

            case 9: //친구
                if (Menu.instance.menu8Open == true)
                {
                    r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                    if (r >= 5) //두번째 메세지일 경우
                    {
                        RightMenu[num] = 8;
                        HintText[seatNum].text = MHTextList[8].MHtext[1];
                    }
                    else if (r <= 4)
                    {
                        RightMenu[num] = 2;
                        HintText[seatNum].text = MHTextList[8].MHtext[0];
                        
                    }
                }
                else
                {
                    HintText[seatNum].text = MHTextList[8].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 2;
                }
                break;

            case 10: //찰스
                if (Menu.instance.menu6Open == true)
                {
                    r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                    if (r >= 6) //두번째 메세지일 경우
                    {
                        RightMenu[num] = 6;
                        HintText[seatNum].text = MHTextList[9].MHtext[1];
                    }
                    else if (r <= 5)
                    {
                        RightMenu[num] = 5;
                        HintText[seatNum].text = MHTextList[9].MHtext[0];
                    }
                }
                else
                {
                    HintText[seatNum].text = MHTextList[9].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 5;
                }
                break;

            case 11: //무명이
                r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                if (r >= 6) //두번째 메세지일 경우
                {
                    RightMenu[num] = 5;
                    HintText[seatNum].text = MHTextList[10].MHtext[1];
                }
                else if (r <= 5)
                {
                    RightMenu[num] = 1;
                    HintText[seatNum].text = MHTextList[10].MHtext[0];
                }
                break;

            case 12: //히로
                HintText[seatNum].text = MHTextList[11].MHtext[0]; //첫번째 메세지로 설정
                RightMenu[num] = 5;
                break;

            case 13: //디노
                HintText[seatNum].text = MHTextList[12].MHtext[0]; //첫번째 메세지로 설정
                RightMenu[num] = 6;
                break;

            case 14: //닥터 펭
                if (Menu.instance.menu8Open == true)
                {
                    r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                    if (r >= 6) //두번째 메세지일 경우
                    {
                        RightMenu[num] = 8;
                        HintText[seatNum].text = MHTextList[13].MHtext[1];
                    }
                    else if(r <= 5)
                    {
                        RightMenu[num] = 5;
                        HintText[seatNum].text = MHTextList[13].MHtext[0];
                    }
                }
                else
                {
                    HintText[seatNum].text = MHTextList[13].MHtext[0]; //첫번째 메세지로 설정
                    RightMenu[num] = 5;
                }
                break;

            case 15: //롤렝드
                r = Random.Range(1,10); // 메세지 2개 중에 랜덤

                if (r >= 6) //두번째 메세지일 경우
                {
                    RightMenu[num] = 4;
                    HintText[seatNum].text = MHTextList[14].MHtext[1];
                }
                else if(r <= 5)
                {
                    RightMenu[num] = 3;
                    HintText[seatNum].text = MHTextList[14].MHtext[0];
                } 
                break;
        }
        //Debug.Log("함수 SetMHText");
        MHFadeIn();
    }

    public void MHFadeIn() //메뉴 힌트 페이드인
    {
        if(!mhFadeIn)
        {
            //Debug.Log("함수 MHFadeIn");
            if (tuto) //만약 튜토리얼이면
            {
                UI_Assistant1.instance.panel5.SetActive(false); //패널 5 닫고 
                UI_Assistant1.instance.panel6.SetActive(true); //패널 6이 보이게
            }
            StartCoroutine(FadeToFullAlpha()); //페이드인 시작
        }
    }

    int m;//메뉴힌트버블 활성화 위한 정수
    bool mhFadeIn = false;//리액션,메뉴 페이드 시에 쓰이는 정수와 역할 동일
    public IEnumerator FadeToFullAlpha() // 알파값 0에서 1로 전환
    {
        mhFadeIn = true;
        int n = mhFade.Peek();
        if (tuto) //튜토리얼이면
        {
            GameScript1.instance.TutorialUpBox();
        }
        else
        {
            m = n;
            ActiveMH(); //메뉴힌트 버블 활성화
        }
        HintBubble[n].GetComponent<Image>().color = new Color(HintBubble[n].GetComponent<Image>().color.r, HintBubble[n].GetComponent<Image>().color.g, HintBubble[n].GetComponent<Image>().color.b, 0);
        HintText[n].GetComponent<Text>().color = new Color(HintText[n].GetComponent<Text>().color.r, HintText[n].GetComponent<Text>().color.g, HintText[n].GetComponent<Text>().color.b, 0);
        while (HintBubble[n].GetComponent<Image>().color.a < 1.0f)
        {
            HintBubble[n].GetComponent<Image>().color = new Color(HintBubble[n].GetComponent<Image>().color.r, HintBubble[n].GetComponent<Image>().color.g, HintBubble[n].GetComponent<Image>().color.b, HintBubble[n].GetComponent<Image>().color.a + (Time.deltaTime / 0.7f));
            HintText[n].GetComponent<Text>().color = new Color(HintText[n].GetComponent<Text>().color.r, HintText[n].GetComponent<Text>().color.g, HintText[n].GetComponent<Text>().color.b, HintText[n].GetComponent<Text>().color.a + (Time.deltaTime / 0.7f));
            yield return null;
        }     
        mhFade.Dequeue();
        mhFadeIn = false;
        if (mhFade.Count != 0)
        {
            MHFadeIn();
        }      
    }

    public IEnumerator FadeToZero(int num)  // 알파값 1에서 0으로 전환
    {
        HintBubble[num].GetComponent<Image>().color = new Color(HintBubble[num].GetComponent<Image>().color.r, HintBubble[num].GetComponent<Image>().color.g, HintBubble[num].GetComponent<Image>().color.b, 1);
        HintText[num].GetComponent<Text>().color = new Color(HintText[num].GetComponent<Text>().color.r, HintText[num].GetComponent<Text>().color.g, HintText[num].GetComponent<Text>().color.b, 1);
        while (HintBubble[num].GetComponent<Image>().color.a > 0.0f)
        {
            HintBubble[num].GetComponent<Image>().color = new Color(HintBubble[num].GetComponent<Image>().color.r, HintBubble[num].GetComponent<Image>().color.g, HintBubble[num].GetComponent<Image>().color.b, HintBubble[num].GetComponent<Image>().color.a - (Time.deltaTime / 0.1f));
            HintText[num].GetComponent<Text>().color = new Color(HintText[num].GetComponent<Text>().color.r, HintText[num].GetComponent<Text>().color.g, HintText[num].GetComponent<Text>().color.b, HintText[num].GetComponent<Text>().color.a - (Time.deltaTime / 0.1f));
            yield return null;
        }
    }
}
