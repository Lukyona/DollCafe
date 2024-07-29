﻿using UnityEngine;



public class CharacterManager : MonoBehaviour 
{
    public static CharacterManager instance;

    #region 캐릭터 움직임 관련 변수
    public GameObject[] BigCharacter; //큰 캐릭터 이미지 배열

    int charNum; //캐릭터 넘버

    Vector3 charInPos; //대화 시작할 때 캐릭터가 들어오는 위치
    Vector3 charOutPos; //캐릭터가 나가는 위치
    float moveSpeed = 1000f; //이동 속도

    int characteInOutState = 0; // 1이면 캐릭터가 들어오고, 2면 나가고, 3은 군인이벤트

    bool isSoldierEvent = false; //군인 대화 이벤트일 경우 1
    #endregion
    
    [System.Serializable] // 클래스나 구조체를 에디터 인스펙터에 노출
    public class FaceList
    {
        public GameObject[] face;
    }

    public FaceList[] CharacterFaceList; //오브젝트 배열을 가지고 있는 배열

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        charInPos = new Vector3(500,-50,0);
        charOutPos = new Vector3(1400,-50,0);
    }

    public void Update()
    {
        if (characteInOutState == 1) //들어오기
        {
            BigCharacter[charNum].transform.position = Vector3.MoveTowards(BigCharacter[charNum].transform.position, charInPos, moveSpeed * Time.deltaTime);
        }
        else if (characteInOutState == 2) // 나가기
        {
            BigCharacter[charNum].transform.position = Vector3.MoveTowards(BigCharacter[charNum].transform.position, charOutPos, moveSpeed * Time.deltaTime);
            if (isSoldierEvent) //군인 이벤트 중일 경우
            {
                if (BigCharacter[charNum].transform.position == charOutPos) //캐릭터가 완전히 나갔을 때 대사 넘기기 가능
                {
                    BigCharacter[charNum].SetActive(false);
                    SystemManager.instance.SetCanTouch(true);
                }
                else //완전히 나가지 않았으면 대사 못 넘김
                {
                    SystemManager.instance.SetCanTouch(false); // 터치로 대사 못 넘기게 함
                }
            }
        }
        else if (characteInOutState == 3)//군인 대화 이벤트, 공주 이동
        {
            Vector3 princessInPos = new Vector3(150,-50,0); //군인 대화 이벤트, 공주 위치

            BigCharacter[charNum].transform.position = Vector3.MoveTowards(BigCharacter[charNum].transform.position, princessInPos, moveSpeed * Time.deltaTime);
            if(BigCharacter[charNum].transform.position == princessInPos)//공주 이동 끝나면 바로 찰스 등장
            {
                CharacterIn(10);
            }
        }
    }

    #region 캐릭터 움직임 관련 함수
    public void CharacterIn(int cNum) //캐릭터 들어와야 할 때
    {
        charNum = cNum;
        BigCharacter[charNum].SetActive(true);
        characteInOutState = 1;
    }

    public void CharacterOut(int cNum = -1) //캐릭터가 나가야할 때
    {
        if(cNum != -1)
            charNum = cNum;
            
        characteInOutState = 2;
    }

    public void DeactivateCharacterMoving() // 캐릭터 움직이기 비활성화
    {
        characteInOutState = 0;
    }

    public void PrincessIn()
    {
        characteInOutState = 3;
    } 

    public void SetSoldierEvent(bool value)
    {
        isSoldierEvent = value;
    }
    #endregion



}