﻿using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public static Setting instance;

    [SerializeField] private Animator settingWindowAnimator;
    [SerializeField] private Button settingButton;
    [SerializeField] private GameObject resetCheck;
    [SerializeField] private Button settingClose;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button yesResetButton;
    [SerializeField] private Button noResetButton;
    [SerializeField] private GameObject creditView;
    [SerializeField] private Scrollbar bar;

    private bool isReset = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        resetButton.GetComponent<Image>().alphaHitTestMinimumThreshold = 1f; // 불투명한 Sprite만 터치가 가능하도록 1로 설정
        creditButton.GetComponent<Image>().alphaHitTestMinimumThreshold = 1f; // 기본값이 0 -> 투명한 부분까지도 터치 가능
    }

    public void ShowSetting() //설정 버튼 터치했을 때
    {
        if (!SystemManager.instance.IsUIOpen)//UI가 올라오지 않았을 때
        {
            SystemManager.instance.IsUIOpen = true;
            SEManager.instance.PlayUITouchSound(); //효과음           

            settingWindowAnimator.SetTrigger("SettingUp"); //설정창 올라옴
            settingButton.GetComponent<Button>().interactable = false; // 설정 버튼 중복 터치 방지
            SystemManager.instance.SetCanTouch(false); // 터치 불가, 대사 못 넘김
        }
    }

    public void CloseSetting() //설정창 닫기 버튼 눌렀을 때
    {
        SEManager.instance.PlayUICloseSound(); //효과음
        settingWindowAnimator.SetTrigger("SettingDown"); //설정창 내려감
        settingButton.GetComponent<Button>().interactable = true; //설정 버튼 터치 가능


        if (!SystemManager.instance.IsNeedAction) //특정한 터치를 해야하는 경우가 아닐 때
        {
            SystemManager.instance.SetCanTouch(true, 1f);
        }

        SystemManager.instance.IsUIOpen = false;
    }

    public void CheckResetGame() //게임 초기화 버튼 눌렀을 때
    {
        SEManager.instance.PlayUITouchSound2();
        settingClose.interactable = false;//설정 닫기 버튼 안 되게 하고
        resetButton.interactable = false;
        creditButton.interactable = false;
        resetCheck.SetActive(true);//경고 메세지 표시
    }

    public void ResetGame() //정말로 게임 초기화
    {
        SEManager.instance.PlayUITouchSound2();
        yesResetButton.interactable = false;
        noResetButton.interactable = false;
        isReset = true;

        DeleteUserInfo();
        SceneChanger.instance.FadeToScene(1); // 시작씬으로 이동
    }

    public void DeleteUserInfo()
    {
        if (PlayerPrefs.GetInt("PurchaseCount") == 0)//결제한 것이 없으면 데이터 모두 삭제
        {
            PlayerPrefs.DeleteAll();
        }
        else //결제를 한번이라도 했다면 결제 정보빼고 모두 삭제
        {
            PlayerPrefs.DeleteKey("AppQuitTime");
            PlayerPrefs.DeleteKey("MainCount");
            PlayerPrefs.DeleteKey("NextAppearNum");
            PlayerPrefs.DeleteKey("Reputation");
            PlayerPrefs.DeleteKey("StarNum");
            PlayerPrefs.DeleteKey("EndingState");
            PlayerPrefs.DeleteKey("CurrentHP");
            PlayerPrefs.DeleteKey("RemainMinTimer");
            PlayerPrefs.DeleteKey("RemainSecTimer");
            PlayerPrefs.DeleteKey("CharacterDC");
            PlayerPrefs.DeleteKey("UnlockedMenuItems");
            PlayerPrefs.DeleteKey("OpenedPages");
            PlayerPrefs.DeleteKey("LikeMenu1Open");
            PlayerPrefs.DeleteKey("LikeMenu2Open");
            PlayerPrefs.DeleteKey("HiddenTextStates");
            PlayerPrefs.DeleteKey("FriendshipInfo");
            PlayerPrefs.DeleteKey("ReplayState");
            PlayerPrefs.DeleteKey("FirstMeetID");
            PlayerPrefs.DeleteKey("FriendEventID");
            PlayerPrefs.DeleteKey("BabyName");
            PlayerPrefs.DeleteKey("NameForNameless");
            PlayerPrefs.DeleteKey("BgmState");
            PlayerPrefs.DeleteKey("SEOnOff");
        }
    }
    public void CancelReset() //초기화 취소버튼 눌렀을 때
    {
        SEManager.instance.PlayUITouchSound2();
        resetCheck.SetActive(false);
        resetButton.interactable = true;
        settingClose.interactable = true;
        creditButton.interactable = true;
    }

    public bool IsReset()
    {
        return isReset;
    }

    public void ShowCredit()//크레딧버튼 눌렀을 때
    {
        SEManager.instance.PlayUITouchSound2();
        resetButton.interactable = false;
        creditButton.interactable = false;
        creditView.SetActive(true);
        bar.value = 1; //스크롤바 핸들러 위치 가장 위로 설정, 위쪽 내용부터 보이도록
    }

    public void CloseCredit()
    {
        SEManager.instance.PlayUITouchSound2();
        creditView.SetActive(false);
        resetButton.interactable = true;
        creditButton.interactable = true;
    }

    public void OpenPrivacyLink()
    {
        Application.OpenURL("https://sites.google.com/view/eocgames-privacy/%ED%99%88");
    }
}
