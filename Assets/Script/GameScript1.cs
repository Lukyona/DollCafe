using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class GameScript1 : MonoBehaviour //전체적인 게임스트립트
{
    public static GameScript1 instance;
    public GameObject panel; //대화할 때 쓰는 회색 반투명 패널
    public GameObject[] BigCharacter; //큰 캐릭터 이미지 배열
    public int textnum = 0; // 0 = character, 1 = baby
    public GameObject babyTextBox;
    public GameObject characterTextBox;



    public GameObject[] Seat; //작은 캐릭터가 앉을 자리 배열
    public GameObject servingTutorial;//서빙 튜토 때만 쓰이는 도리와 메뉴힌트버블

    public static bool smallFadeOut = false; //현재 캐릭터가 제제일 경우 true

    public GameObject bear;

    public int endStory = 0;//1이면 엔딩 이벤트 실행된 것

    public bool delete = false;


    public GameObject PurchasingWindow;//붕어빵 버튼 눌렀을 때 나오는 창
    public Text PurchasingText; //창 메세지
    public Text FishBreadText; //붕어빵 버튼 텍스트

    public GameObject CompletePurchasingWindow;//결제 후 메세지 창
    public Text CompletePurchasingText;//메세지 내용

    public GameObject pButton1;//체력 증가 버튼
    public GameObject pButton2;//회복 속도 증가 버튼

    public Text littleFishBreadText;

    public Button FishBreadButton;//붕어빵 버튼
    public Image FishBread2;//두 번 결제 후 붕어빵 버튼 이미지

    public Image JejeBubble;
    public Text JejeText;

    public Animator jejeBubbleAnimator;

    public Image Tip;
    public Image TipMessage;

    public Image StarLackMessage;
    public Animator starLackAnimator;
    public Button TipNoteButton;
    public Animator TNBAnimator;
    public Image TipNote;
    public Animator TipNoteAnimator;

    public Text Tip2;
    public Text Tip3;

    public int mainCount = 0; 


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

   


   
   

   

    int c = 0;


    

    int f = 0;//표정 비활성화에 쓰임
  





   



   

  

    int price = 1000;



    public GameObject exchangeMessageWindow;
}

 

