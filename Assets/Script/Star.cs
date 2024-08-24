using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    public static Star instance;

    [SerializeField] Text StarNumText;//별 개수 표시 텍스트

    int starNum = 0; // 현재 보유한 별 개수

    [SerializeField] GameObject[] Stars; //별 이미지 배열

    List<int> StarList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 }; //나타나게 될 별 리스트

    Queue<int> readyToFadeOut = new Queue<int>(); //별 페이드아웃 시 쓰임
    Queue<int> waitingLine = new Queue<int>(); //페이드 아웃된 별을 리스트에 다시 추가하기 전까지 보관 

    bool isFadingOut = false; //페이드아웃 중일 때 true

    Coroutine starCoroutine;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ActivateStarSystem()
    {
        starCoroutine = StartCoroutine(ShowStar());
    }

    public void DeactivateStarSystem()
    {
        if(starCoroutine == null) return;

        StopCoroutine(starCoroutine);
        starCoroutine = null;
    }

    public void ActivateStarFadeOut() //별 추가 및 페이드아웃 함수
    {
        StartCoroutine(GetStar());
    }

    public bool IsStarSystemRunning()
    {
        return starCoroutine == null? false : true;
    }

    public IEnumerator ShowStar() //플레이 타임 25~30초마다 1개씩 랜덤으로 별이 나타남
    {
       while(StarList.Count != 0) // 리스트 크기가 0이 아니라면
       {
            int idx = Random.Range(0, StarList.Count); //0~6까지
            StarList.Remove(StarList[idx]); //그 값을 삭제
            //Debug.Log("스타 삭제된 번호 " + n);
            //Debug.Log("스타 리스트 크기 " + StarList.Count);
            Stars[idx].SetActive(true); //해당 별 활성화
            Stars[idx].GetComponent<Button>().interactable = true; //한번만 클릭 가능

            while (Stars[idx].GetComponent<Image>().color.a < 1.0f) //별 페이드인
            {
                Stars[idx].GetComponent<Image>().color = new Color(Stars[idx].GetComponent<Image>().color.r, Stars[idx].GetComponent<Image>().color.g, Stars[idx].GetComponent<Image>().color.b, Stars[idx].GetComponent<Image>().color.a + (Time.deltaTime / 0.5f));
                yield return null;
            }

            float second = Random.Range(25,31);
            yield return new WaitForSeconds(second);//25~30초마다 반복
       }                        
    }

    public IEnumerator GetStar()
    {
        if(isFadingOut) // 페이드 아웃 중이면
        {
            Invoke(nameof(ActivateStarFadeOut), 0.5f);
        }
        else
        {
            isFadingOut = true;
            int idx = readyToFadeOut.Peek() - 1; // 맨 앞 요소 꺼내기, 요소 값 - 1 = 스타 배열 인덱스값
            Stars[idx].GetComponent<Button>().interactable = false; // 클릭 불가능
            while (Stars[idx].GetComponent<Image>().color.a > 0.0f)
            {
                Stars[idx].GetComponent<Image>().color = new Color(Stars[idx].GetComponent<Image>().color.r, Stars[idx].GetComponent<Image>().color.g, Stars[idx].GetComponent<Image>().color.b, Stars[idx].GetComponent<Image>().color.a - (Time.deltaTime / 0.5f));
                yield return null;
            }
            isFadingOut = false;

            Stars[idx].SetActive(false); //클릭된 별 비활성화

            starNum++;//별 개수 증가
            StarNumText.text = string.Format("{0}", starNum.ToString());

            readyToFadeOut.Dequeue();
            waitingLine.Enqueue(idx+1); // 대기 별 큐에 추가

            Invoke(nameof(ReAppearStar), 40f);//40초 뒤 다시 나타나기 가능
            
            PlayerPrefs.SetInt("StarNum", starNum); //별 개수 저장
            PlayerPrefs.Save();

            if(!SystemManager.instance.IsInvoking("CheckTipState") && starNum >= 3) 
            {
                SystemManager.instance.CheckTipState();
            }
        }        
    }

    void ReAppearStar()
    {
        if (StarList.Count == 0)//리스트 크기가 0이면
        {
            Invoke(nameof(ActivateStarSystem), 25f);//25초 뒤에 별 생성
            //Debug.Log("스타 시스템 25초 뒤 시작");
        }

        int star = waitingLine.Peek();
        StarList.Add(star);
        //Debug.Log("스타 추가된 번호 " + r);
        //Debug.Log("스타 리스트 크기 " + StarList.Count);
        waitingLine.Dequeue();          
    }

    public void TouchStar(int starValue)
    {
        readyToFadeOut.Enqueue(starValue);
        ActivateStarFadeOut();
    }

    public void UseStar(int value)
    {
        starNum -= value;
        StarNumText.text = string.Format("{0}", GetCurrentStarNum().ToString());
        PlayerPrefs.SetInt("StarNum", starNum); //별 개수 저장
        PlayerPrefs.Save();
    }

    public void SetStarNum(int value)
    {
        starNum = value;
        StarNumText.text = string.Format("{0}", GetCurrentStarNum().ToString());

        PlayerPrefs.SetInt("StarNum", starNum); //별 개수 저장
        PlayerPrefs.Save();
    }
    
    public int GetCurrentStarNum()
    {
        return starNum;
    }

    public void StarCheat()
    {
        starNum += 15;//별 개수 증가
        StarNumText.text = string.Format("{0}", starNum.ToString());
    }
}
