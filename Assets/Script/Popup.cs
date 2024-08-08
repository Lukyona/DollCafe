using System;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour // 새로운 손님 팝업
{
    public static Popup instance;

    public Animator popupAnimator;
    public GameObject newVisitorPopup; //팝업창
    public GameObject character; //팝업 알림에 보여질 캐릭터 이미지
    public GameObject specialMenuPopup;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void OpenPopup() //팝업 열림
    {
        Menu.instance.UIOn = true;
        SEManager.instance.PlayPopupSound(); //효과음

        if (CharacterAppear.instance.eventOn != 0 || VisitorNote.instance.evRP != 0) // 스페셜 메뉴 팝업인 경우
        {
            newVisitorPopup.SetActive(false);
            specialMenuPopup.SetActive(true);
            //Debug.Log("팝업 이벤트 중");  
        }
        else // 새로운 손님 팝업
        {
            specialMenuPopup.SetActive(false);
            newVisitorPopup.SetActive(true); //팝업 보이게
            //Debug.Log("팝업 이벤트 중 아님");
        }       

        popupAnimator.SetTrigger("PopupOpen"); //팝업 오픈 애니메이션 작동
    }

    public void ClosePopup() //팝업 닫기 버튼 눌렀을 때
    {
        Menu.instance.UIOn = false;
        SEManager.instance.PlayUICloseSound(); //효과음
        popupAnimator.SetTrigger("PopupClose"); //닫히는 애니메이션 작동
        Invoke("PopupNotActive", 0.3f); //팝업 비활성화

        if(SystemManager.instance.GetMainCount() == 2) //도리 방문, 첫 손님
        {
            //서빙 튜토리얼 실행
            SmallFade.instance.FadeOutJeje();
            SystemManager.instance.BeginDialogue(0, 1f);
        }
    }

    void PopupNotActive()
    {
        newVisitorPopup.SetActive(false);

        if (CharacterAppear.instance.eventOn != 0 || VisitorNote.instance.evRP != 0)//친밀도 이벤트 도중이거나 다시보기 도중이면
        {
            specialMenuPopup.SetActive(false);
            SystemManager.instance.SetCanTouch(true);
        }

    }

    public void SetPopupCharacter(in GameObject ch) //팝업창에 나올 캐릭터 설정, 매개변수는 읽기 전용 in키워드 사용
    {
        character.GetComponent<RectTransform>().sizeDelta = new Vector2(ch.GetComponent<RectTransform>().rect.width, ch.GetComponent<RectTransform>().rect.height); //사이즈 조정
        character.GetComponent<Image>().sprite = ch.GetComponent<Image>().sprite;
        if (SystemManager.instance.GetMainCount() == 13) //히로/디노의 경우 이미지 위치 조정
        {
            character.GetComponent<RectTransform >().anchoredPosition = new Vector2(25, 80);
        }
    }

    public void SetPopupMenu(GameObject menu, String name, float anchorY)
    {
        //팝업창에 나타날 이미지 설정
        specialMenuPopup.transform.GetChild(0).GetComponentInChildren<Image>().sprite = menu.GetComponent<Image>().sprite;
        // 이미지 사이즈 조정
        Vector2 size = new Vector2(menu.GetComponent<RectTransform>().rect.width, menu.GetComponent<RectTransform>().rect.height); 
        specialMenuPopup.transform.GetChild(0).GetComponentInChildren<Image>().GetComponent<RectTransform>().sizeDelta = size;
        specialMenuPopup.transform.GetChild(0).GetComponentInChildren<Image>().GetComponent<RectTransform>().anchoredPosition = new Vector2(0, anchorY); //이미지에 따라 앵커 Y 위치 조정
        // 나타날 텍스트 설정
        specialMenuPopup.transform.GetChild(1).GetComponentInChildren<Text>().text = name; 
        specialMenuPopup.transform.GetChild(1).GetComponentInChildren<Text>().text += "를 위한 특별메뉴!";
    }
}
