using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour 
{
    public static Setting instance;

    public Animator settingWindowAnimator;
    public Button settingButton;
    public GameObject resetCheck;
    public Button settingClose;
    public Button resetButton;
    public Button creditButton;
    public Button yesResetButton;
    public Button noResetButton;
    public GameObject creditView;
    public Scrollbar bar;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        resetButton.GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
        creditButton.GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
    }

    public void ClickSettingButton() //설정 버튼 터치했을 때
    {
        if (!Menu.instance.UIOn)//UI가 올라오지 않았을 때
        {
            Menu.instance.UIOn = true;
            SEManager.instance.UIButtonClick(); //효과음           

            settingWindowAnimator.SetTrigger("SettingUp"); //설정창 올라옴
            settingButton.GetComponent<Button>().interactable = false; // 설정 버튼 클릭 안 되게
            GameScript1.instance.gameObject.GetComponent<Dialogue2>().enabled = false;//대사 못 넘김
        }
    }

    public void ClickSettingCloseButton() //설정창 닫기 버튼 눌렀을 때
    {        
        SEManager.instance.ClickClose(); //효과음
        settingWindowAnimator.SetTrigger("SettingDown"); //설정창 내려감
        settingButton.GetComponent<Button>().interactable = true; //설정 버튼 터치 가능

        GameScript1.instance.gameObject.GetComponent<Dialogue2>().enabled = true;//대사 넘기기 가능

        if (UI_Assistant1.instance.talking && UI_Assistant1.instance.stop == 0)//대화 중이고, 특정한 터치를 해야하는 경우가 아닐 때
        {
            UI_Assistant1.instance.stop = 1;
            UI_Assistant1.instance.Invoke("TouchEnable", 1f);
        }

        Menu.instance.UIOn = false;
    }

    public void ClickGameResetButton() //게임 초기화 버튼 눌렀을 때
    {
        SEManager.instance.UIClick2();
        settingClose.interactable = false;//설정 닫기 버튼 안 되게 하고
        resetButton.interactable = false;
        creditButton.interactable = false;
        resetCheck.SetActive(true);//경고 메세지 표시
    }

    public void GameReset()//정말로 게임 초기화
    {
        SEManager.instance.UIClick2();
        yesResetButton.interactable = false;
        noResetButton.interactable = false;        
        GameScript1.instance.delete = true;
        if(GameScript1.instance.pNum == 0)//결제한 것이 없으면 데이터 모두 삭제
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("결제 정보 없음");
        }
        else //결제를 한번이라도 했다면 결제 정보빼고 모두 삭제
        {
            Debug.Log("결제 정보 있음");
            PlayerPrefs.DeleteKey("AppQuitTime");
            PlayerPrefs.DeleteKey("MainCount");
            PlayerPrefs.DeleteKey("NextAppear");
            PlayerPrefs.DeleteKey("Reputation");
            PlayerPrefs.DeleteKey("StarNum");
            PlayerPrefs.DeleteKey("end");
            PlayerPrefs.DeleteKey("HPAmount");
            PlayerPrefs.DeleteKey("RemainMinTimer");
            PlayerPrefs.DeleteKey("RemainSecTimer");
            PlayerPrefs.DeleteKey("CharacterDC");
            PlayerPrefs.DeleteKey("OpenMenu");
            PlayerPrefs.DeleteKey("OpenPage");
            PlayerPrefs.DeleteKey("LikeMenu1Open");
            PlayerPrefs.DeleteKey("LikeMenu2Open");
            PlayerPrefs.DeleteKey("SecondSentence");
            PlayerPrefs.DeleteKey("FriendshipInfo");
            PlayerPrefs.DeleteKey("RePlayOn");
            PlayerPrefs.DeleteKey("FMRP");
            PlayerPrefs.DeleteKey("EVRP");
            PlayerPrefs.DeleteKey("GameOff");
            PlayerPrefs.DeleteKey("BabyName");
            PlayerPrefs.DeleteKey("NamedName");
            PlayerPrefs.DeleteKey("BgmOnOff");
            PlayerPrefs.DeleteKey("SEOnOff");
        }
        SceneChanger.sc.FadeToScene(1);
    }

    public void ClickCancelReset()//초기화 취소버튼 눌렀을 때
    {
        SEManager.instance.UIClick2();
        resetCheck.SetActive(false);
        resetButton.interactable = true;
        settingClose.interactable = true;
        creditButton.interactable = true;
    }

    public void ClickCreditButton()//크레딧버튼 눌렀을 때
    {
        SEManager.instance.UIClick2();
        resetButton.interactable = false;
        creditButton.interactable = false;
        creditView.SetActive(true);
        bar.value = 1;
    }

    public void CloseCredit()
    {
        SEManager.instance.UIClick2();
        creditView.SetActive(false);
        resetButton.interactable = true;
        creditButton.interactable = true;       
    }

    public void PrivacyLink()
    {
        Application.OpenURL("https://eocgames.blogspot.com/2022/05/blog-post.html");
    }
}
