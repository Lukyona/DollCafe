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
    [SerializeField] private TextList[] MHTextList; //문자열 배열을 가지고 있는 배열

    [SerializeField] private GameObject[] MH; //메뉴힌트 말풍선

    private GameObject[] HintBubble = new GameObject[6]; // 메뉴 힌트 말풍선
    private Text[] HintText = new Text[6]; // 메뉴 힌트 말풍선 메세지

    private int[] wantedMenu = new int[6]; //그 자리의 캐릭터가 원하는 메뉴 넘버 

    private Queue<int> mhFade = new Queue<int>();//페이드인 시 사용

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < MH.Length; i++)
        {
            MH[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
        }
    }

    public GameObject GetHintBubble(int num)
    {
        return HintBubble[num];
    }

    public int GetWantedMenuNum(int sNum)
    {
        return wantedMenu[sNum];
    }

    public void InActiveMH(int num) //메뉴 힌트 비활성화
    {
        HintBubble[num].SetActive(false);
    }

    public void CantTouchMHB()//메뉴힌트버블 터치 불가, 캐릭터 이벤트 시 실행
    {
        for (int i = 0; i < 6; i++)
        {
            if (HintBubble[i] != null)
            {
                HintBubble[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    public void CanTouchMHB()//메뉴힌트버블 터치 가능, 캐릭터 이벤트 끝나고 실행
    {
        for (int i = 0; i < 6; i++)
        {
            if (HintBubble[i] != null)
            {
                HintBubble[i].GetComponent<Button>().interactable = true;
            }
        }
    }

    public void SetMHB(int sNum) //어떤 메뉴힌트 말풍선 이미지를 쓸 것인지 설정, 자리에 따라 이미지가 조금 다름
    {
        HintBubble[sNum] = MH[sNum]; //말풍선1로 설정
        HintText[sNum] = MH[sNum].GetComponentInChildren<Text>(); //말풍선1의 메세지로 설정
    }

    public void SetMHText(int cNum, int sNum) //메뉴 힌트 메세지 설정
    {
        int randomNum; //랜덤 텍스트
        switch (cNum) // cNum은 캐릭터 넘버
        {
            case 1: //1은 도리
                if (Menu.instance.IsMenuOpen(4) == true) // 메뉴4가 열렸을 경우
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤, 1~5까지가 첫번째, 6~10까지가 두번째

                    if (randomNum >= 5) //두번째 메세지일 경우
                    {
                        wantedMenu[sNum] = 4; //원하는 메뉴는 자몽이므로 4
                        HintText[sNum].text = MHTextList[0].MHtext[1]; //메뉴 힌트 메세지에 넣기
                    }
                    else if (randomNum <= 4)
                    {
                        wantedMenu[sNum] = 1;
                        HintText[sNum].text = MHTextList[0].MHtext[0]; //메뉴 힌트 메세지에 넣기
                    }
                }
                else //메뉴4가 열리지 않았으면
                {
                    HintText[sNum].text = MHTextList[0].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 1;
                }
                break;

            case 2: //2는 붕붕
                HintText[sNum].text = MHTextList[1].MHtext[0]; //첫번째 메세지로 설정
                wantedMenu[sNum] = 2; //원하는 메뉴는 핫초코
                break;

            case 3: //3은 빵빵
                if (Menu.instance.IsMenuOpen(6) == true) // 메뉴6이 열렸을 경우
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                    if (randomNum >= 5) //두번째 메세지일 경우
                    {
                        wantedMenu[sNum] = 6; //원하는 메뉴는 6
                        HintText[sNum].text = MHTextList[2].MHtext[1];
                    }
                    else if (randomNum <= 4)
                    {
                        wantedMenu[sNum] = 2;
                        HintText[sNum].text = MHTextList[2].MHtext[0];
                    }
                }
                else //메뉴6이 열리지 않았으면
                {
                    HintText[sNum].text = MHTextList[2].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 2;
                }
                break;

            case 4: //4 개나리
                if (Menu.instance.IsMenuOpen(4) == true)
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤
                    if (randomNum >= 5) //두번째 메세지일 경우
                    {
                        wantedMenu[sNum] = 4;
                        HintText[sNum].text = MHTextList[3].MHtext[1];
                    }
                    else if (randomNum <= 4)
                    {
                        wantedMenu[sNum] = 3;
                        HintText[sNum].text = MHTextList[3].MHtext[0];
                    }
                }
                else
                {
                    HintText[sNum].text = MHTextList[3].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 3;
                }
                break;

            case 5: //또롱
                if (Menu.instance.IsMenuOpen(6) == true)
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                    if (randomNum >= 4) //두번째 메세지일 경우, 70%
                    {
                        wantedMenu[sNum] = 6;
                        HintText[sNum].text = MHTextList[4].MHtext[1];
                    }
                    else if (randomNum <= 3)
                    {
                        wantedMenu[sNum] = 1;
                        HintText[sNum].text = MHTextList[4].MHtext[0];
                    }
                }
                else
                {
                    HintText[sNum].text = MHTextList[4].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 1;
                }
                break;

            case 6: //도로시
                if (Menu.instance.IsMenuOpen(8) == true)
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                    if (randomNum >= 5) //두번째 메세지일 경우
                    {
                        wantedMenu[sNum] = 8;
                        HintText[sNum].text = MHTextList[5].MHtext[1];
                    }
                    else if (randomNum <= 4)
                    {
                        wantedMenu[sNum] = 3;
                        HintText[sNum].text = MHTextList[5].MHtext[0];
                    }
                }
                else
                {
                    HintText[sNum].text = MHTextList[5].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 3;
                }
                break;

            case 7: //루루
                if (Menu.instance.IsMenuOpen(7) == true)
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                    if (randomNum >= 6) //두번째 메세지일 경우
                    {
                        wantedMenu[sNum] = 7;
                        HintText[sNum].text = MHTextList[6].MHtext[1];
                    }
                    else if (randomNum <= 5)
                    {
                        wantedMenu[sNum] = 3;
                        HintText[sNum].text = MHTextList[6].MHtext[0];
                    }
                }
                else
                {
                    HintText[sNum].text = MHTextList[6].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 3;
                }
                break;

            case 8: //샌디
                if (Menu.instance.IsMenuOpen(7) == true)
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                    if (randomNum >= 6) //두번째 메세지일 경우
                    {
                        wantedMenu[sNum] = 7;
                        HintText[sNum].text = MHTextList[7].MHtext[1];
                    }
                    else if (randomNum <= 5)
                    {
                        wantedMenu[sNum] = 4;
                        HintText[sNum].text = MHTextList[7].MHtext[0];
                    }
                }
                else
                {
                    HintText[sNum].text = MHTextList[7].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 4;
                }
                break;

            case 9: //친구
                if (Menu.instance.IsMenuOpen(8) == true)
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                    if (randomNum >= 5) //두번째 메세지일 경우
                    {
                        wantedMenu[sNum] = 8;
                        HintText[sNum].text = MHTextList[8].MHtext[1];
                    }
                    else if (randomNum <= 4)
                    {
                        wantedMenu[sNum] = 2;
                        HintText[sNum].text = MHTextList[8].MHtext[0];

                    }
                }
                else
                {
                    HintText[sNum].text = MHTextList[8].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 2;
                }
                break;

            case 10: //찰스
                if (Menu.instance.IsMenuOpen(6) == true)
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                    if (randomNum >= 6) //두번째 메세지일 경우
                    {
                        wantedMenu[sNum] = 6;
                        HintText[sNum].text = MHTextList[9].MHtext[1];
                    }
                    else if (randomNum <= 5)
                    {
                        wantedMenu[sNum] = 5;
                        HintText[sNum].text = MHTextList[9].MHtext[0];
                    }
                }
                else
                {
                    HintText[sNum].text = MHTextList[9].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 5;
                }
                break;

            case 11: //무명이
                randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                if (randomNum >= 6) //두번째 메세지일 경우
                {
                    wantedMenu[sNum] = 5;
                    HintText[sNum].text = MHTextList[10].MHtext[1];
                }
                else if (randomNum <= 5)
                {
                    wantedMenu[sNum] = 1;
                    HintText[sNum].text = MHTextList[10].MHtext[0];
                }
                break;

            case 12: //히로
                HintText[sNum].text = MHTextList[11].MHtext[0]; //첫번째 메세지로 설정
                wantedMenu[sNum] = 5;
                break;

            case 13: //디노
                HintText[sNum].text = MHTextList[12].MHtext[0]; //첫번째 메세지로 설정
                wantedMenu[sNum] = 6;
                break;

            case 14: //닥터 펭
                if (Menu.instance.IsMenuOpen(8) == true)
                {
                    randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                    if (randomNum >= 6) //두번째 메세지일 경우
                    {
                        wantedMenu[sNum] = 8;
                        HintText[sNum].text = MHTextList[13].MHtext[1];
                    }
                    else if (randomNum <= 5)
                    {
                        wantedMenu[sNum] = 5;
                        HintText[sNum].text = MHTextList[13].MHtext[0];
                    }
                }
                else
                {
                    HintText[sNum].text = MHTextList[13].MHtext[0]; //첫번째 메세지로 설정
                    wantedMenu[sNum] = 5;
                }
                break;

            case 15: //롤렝드
                randomNum = Random.Range(1, 10); // 메세지 2개 중에 랜덤

                if (randomNum >= 6) //두번째 메세지일 경우
                {
                    wantedMenu[sNum] = 4;
                    HintText[sNum].text = MHTextList[14].MHtext[1];
                }
                else if (randomNum <= 5)
                {
                    wantedMenu[sNum] = 3;
                    HintText[sNum].text = MHTextList[14].MHtext[0];
                }
                break;
        }
        mhFade.Enqueue(sNum);
        MHFadeIn();
    }

    public void MHFadeIn() //메뉴 힌트 페이드인
    {
        if (!mhFadeIn)
        {
            if (SystemManager.instance.MainCount == 2) //만약 서빙 튜토리얼이면
            {
                Dialogue.instance.SetPanelActive(5, false); //패널 5 닫고 
                Dialogue.instance.SetPanelActive(6, true); //패널 6이 보이게
            }
            StartCoroutine(FadeToFullAlpha()); //페이드인 시작
        }
    }

    private bool mhFadeIn = false;//리액션,메뉴 페이드 시에 쓰이는 정수와 역할 동일
    private IEnumerator FadeToFullAlpha() // 알파값 0에서 1로 전환
    {
        mhFadeIn = true;
        int idx = mhFade.Peek();

        if (SystemManager.instance.MainCount == 2) //튜토리얼이면
        {
            SystemManager.instance.UpTextBox();
            CantTouchMHB();
            HintBubble[idx].transform.parent.GetComponent<Canvas>().sortingOrder = 7;
        }

        HintBubble[idx].SetActive(true);

        HintBubble[idx].GetComponent<Image>().color = new Color(HintBubble[idx].GetComponent<Image>().color.r, HintBubble[idx].GetComponent<Image>().color.g, HintBubble[idx].GetComponent<Image>().color.b, 0);
        HintText[idx].GetComponent<Text>().color = new Color(HintText[idx].GetComponent<Text>().color.r, HintText[idx].GetComponent<Text>().color.g, HintText[idx].GetComponent<Text>().color.b, 0);
        while (HintBubble[idx].GetComponent<Image>().color.a < 1.0f)
        {
            HintBubble[idx].GetComponent<Image>().color = new Color(HintBubble[idx].GetComponent<Image>().color.r, HintBubble[idx].GetComponent<Image>().color.g, HintBubble[idx].GetComponent<Image>().color.b, HintBubble[idx].GetComponent<Image>().color.a + (Time.deltaTime / 0.7f));
            HintText[idx].GetComponent<Text>().color = new Color(HintText[idx].GetComponent<Text>().color.r, HintText[idx].GetComponent<Text>().color.g, HintText[idx].GetComponent<Text>().color.b, HintText[idx].GetComponent<Text>().color.a + (Time.deltaTime / 0.7f));
            yield return null;
        }
        mhFade.Dequeue();
        mhFadeIn = false;
        if (mhFade.Count != 0)
        {
            MHFadeIn();
        }
    }

    public void MHFadeOut(int num)//메뉴힌트 페이드아웃
    {
        StartCoroutine(FadeToZero(num)); //페이드아웃 시작
        InActiveMH(num); //페이드아웃된 메뉴 힌트 비활성화
    }

    private IEnumerator FadeToZero(int num)  // 알파값 1에서 0으로 전환
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
