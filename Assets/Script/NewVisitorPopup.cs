using UnityEngine;
using UnityEngine.UI;

public class NewVisitorPopup : MonoBehaviour // 새로운 손님 팝업
{
    public static NewVisitorPopup instance;

    public Animator popupAnimator;
    public GameObject popup; //팝업창
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
        if (CharacterAppear.instance.eventOn != 0 || VisitorNote.instance.evRP != 0)
        {
            popup.SetActive(false);
            specialMenuPopup.SetActive(true);
            //Debug.Log("팝업 이벤트 중");  
        }
        else
        {
            specialMenuPopup.SetActive(false);
            popup.SetActive(true); //팝업 보이게
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

        if(Dialogue1.instance.CharacterDC[0] == 1) //만약 제제 다이얼로그 카운트가 1이면
        {
            GameScript1.instance.Invoke("ServingTutorial",0.7f); //서빙 튜토리얼 실행
        }
    }

    void PopupNotActive()
    {
        if (CharacterAppear.instance.eventOn != 0 || VisitorNote.instance.evRP != 0)//친밀도 이벤트 도중이거나 다시보기 도중이면
        {
            specialMenuPopup.SetActive(false);
            //UI_Assistant1.instance.TouchEnable();
            UserInputManager.instance.SetCanTouch(true);
        }
        else
        {
            popup.SetActive(false);
        }     
    }

    public void SetPopupCharacter(GameObject ch) //팝업창에 나올 캐릭터 설정
    {
        character.GetComponent<RectTransform>().sizeDelta = new Vector2(ch.GetComponent<RectTransform>().rect.width, ch.GetComponent<RectTransform>().rect.height); //사이즈 조정
        character.GetComponent<Image>().sprite = ch.GetComponent<Image>().sprite;
        if (GameScript1.instance.mainCount == 13) //히로 디노의 경우 이미지 위치 조정
        {
            character.GetComponent<RectTransform >().anchoredPosition = new Vector2(25, 80);
        }
    }
}
