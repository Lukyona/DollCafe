using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    public static Star instance;

    public int starNum = 0; //별 갯수

    GameObject[] Stars;//별 이미지 배열

    public Coroutine starCoroutine;

    public Text StarNumText;//별 개수 표시 텍스트

    List<int> StarList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 }; //나타나게 될 별 리스트

    public Queue<int> starFadeOut = new Queue<int>(); //별 페이드아웃 시 쓰임
    public Queue<int> starRe = new Queue<int>();//페이드아웃된 별을 리스트에 다시 추가할 때 쓰임 

    bool starFade2 = false;//페이드아웃 중일 때 true

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ActivateStarSystem()
    {
        starCoroutine = StartCoroutine(StarAppear());
    }

    public IEnumerator StarAppear()//플레이 타임 30초마다 1개씩 랜덤으로 별이 나타남
    {
        //Debug.Log("스타 나타나기");
       while(StarList.Count != 0)
       {
            int x = Random.Range(0, StarList.Count - 1);//0~6까지
            int n = StarList[x];//랜덤 숫자 인덱스의 값을 n에 대입
            StarList.Remove(n); //그 값을 삭제
            //Debug.Log("스타 삭제된 번호 " + n);
            //Debug.Log("스타 리스트 크기 " + StarList.Count);
            Stars[n - 1].SetActive(true); //해당 별 활성화
            Stars[n -1].GetComponent<Button>().interactable = true;//한번만 클릭 가능
            while (Stars[n - 1].GetComponent<Image>().color.a < 1.0f)//별 페이드인
            {
                Stars[n - 1].GetComponent<Image>().color = new Color(Stars[n - 1].GetComponent<Image>().color.r, Stars[n - 1].GetComponent<Image>().color.g, Stars[n - 1].GetComponent<Image>().color.b, Stars[n - 1].GetComponent<Image>().color.a + (Time.deltaTime / 0.5f));
                yield return null;
            }
            yield return new WaitForSeconds(30f);//30초마다 반복
           // Debug.Log("스타 자체 반복");
       }                        
    }

    public IEnumerator PlusStar()
    {
        if(starFade2)
        {
            Invoke("StarFadeOut", 0.5f);
        }
        else
        {
            starFade2 = true;
            int z = starFadeOut.Peek();
            Stars[z].GetComponent<Button>().interactable = false;//한번만 클릭 가능
            while (Stars[z].GetComponent<Image>().color.a > 0.0f)
            {
                Stars[z].GetComponent<Image>().color = new Color(Stars[z].GetComponent<Image>().color.r, Stars[z].GetComponent<Image>().color.g, Stars[z].GetComponent<Image>().color.b, Stars[z].GetComponent<Image>().color.a - (Time.deltaTime / 0.5f));
                yield return null;
            }
            starFade2 = false;
            starNum++;//별 개수 증가
            StarNumText.text = string.Format("{0}", starNum.ToString());
            Stars[z].SetActive(false); //클릭된 별 비활성화
            starFadeOut.Dequeue();
            starRe.Enqueue(z); //다시 나타날 별 큐에 추가
            //Debug.Log("스타 눌러진 번호 " + (z+1));           
            Invoke("ReAppearStar", 40f);//40초 뒤 다시 나타나기 가능
            PlayerPrefs.SetInt("StarNum", Star.instance.starNum); //별 개수 저장
            PlayerPrefs.Save();
        }        
    }

    void StarFadeOut()//별 추가, 별 페이드아웃 실행하는 함수
    {
        StartCoroutine(PlusStar());
    }

    void ReAppearStar()
    {
        if (StarList.Count == 0)//리스트 크기가 0이면
        {
            Invoke("StarSystem", 25f);//25초 뒤에 별 생성
            //Debug.Log("스타 시스템 25초 뒤 시작");
        }

        int r = starRe.Peek();
        r++;
        StarList.Add(r);
        //Debug.Log("스타 추가된 번호 " + r);
        //Debug.Log("스타 리스트 크기 " + StarList.Count);
        starRe.Dequeue();          
    }

    public void StarCheat()
    {
        starNum += 15;//별 개수 증가
        StarNumText.text = string.Format("{0}", starNum.ToString());
    }
}
