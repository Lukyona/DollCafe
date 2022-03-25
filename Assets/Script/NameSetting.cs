using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameSetting : MonoBehaviour //무명이, 주인공 아기 이름 설정 스크립트
{
    public static NameSetting instance;

    public string theName; //입력한 이름
    public GameObject inputField;
    public GameObject babyInputField;
    public GameObject nameCheckWindow; //이름 확인창
    public Button noNameCheckButton;//확인버튼
    public Button babyNameCheckButton;
    public Text CheckName; //확인할 이름

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void CheckingName() //이름 확인
    {
        if (GameScript1.instance.mainCount == 0)//아기 이름 설정일 경우
        {
            theName = babyInputField.GetComponent<Text>().text; //입력받은 값 넣음
        }
        else
        {
            theName = inputField.GetComponent<Text>().text; //입력받은 값 넣음
        }
        if (string.IsNullOrWhiteSpace(theName) == false) //입력받은 게 없거나 완전 공백이 아니면
        {
            CheckName.text = theName + "?";//확인할 이름 텍스트에 입력한 이름 넣고
            nameCheckWindow.SetActive(true);//마지막 확인 창 활성화
            if (GameScript1.instance.mainCount == 0)//아기 이름 설정일 경우
            {
                babyNameCheckButton.interactable = false; // 이전 창의 확인 버튼 못 누르게
                n = 1;
            }
            else //무명이일 경우
            {
                noNameCheckButton.interactable = false;
                n = 2;
            }
        }     
    }

    int n;
    public void NameSetComplete()// 이름 확정
    {
        switch(n)
        {
            case 1://아기
                Dialogue1.instance.babyName = theName;//입력한 이름을 아기 이름에 대입
                PlayerPrefs.SetString("BabyName", theName); //아기 이름 저장                             
                UI_Assistant1.instance.BabyNameSetting.SetActive(false);//아기 이름 설정창 비활성화                
                break;
            case 2:
                UI_Assistant1.instance.namedName = theName;//입력한 이름 캐릭터 이름에 대입
                PlayerPrefs.SetString("NamedName", theName);//무명이 이름 저장
                VisitorNote.instance.NameInfoOpen(theName);//손님노트에 이름 정보 활성화
                UI_Assistant1.instance.NameSetting.SetActive(false);//무명이 이름 설정창 비활성화
                break;

        }
        nameCheckWindow.SetActive(false);//이름 확인창 비활성화
        PlayerPrefs.Save();
        UI_Assistant1.instance.Invoke("TouchEnable", 1f);
    }

    public void NameSetNotYet() //다시 이름 고민
    {
        nameCheckWindow.SetActive(false);//이름 확인창 비활성화
        switch(n)
        {
            case 1://아기일 경우
                babyNameCheckButton.interactable = true; //다시 확인 버튼 활성화
                break;
            case 2:
                noNameCheckButton.interactable = true;
                break;
        }
    }
}
