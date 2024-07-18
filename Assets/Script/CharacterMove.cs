
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CharacterMove : MonoBehaviour //대화할 때 큰 캐릭터 이미지 이동
{
    public static CharacterMove instance;
    private GameObject character;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

    }

    public Transform inPos; //대화 시작할 때 캐릭터가 들어오는 위치
    public Transform outPos; //캐릭터가 나가는 위치
    public float moveSpeed; //이동 속도

    private int characterOut = 0; // 1이면 캐릭터가 나가야함, 3은 군인이벤트

    public Transform inPos2; //군인 대화 이벤트, 공주 위치
    public int soldierEvent = 0; //군인 대화 이벤트일 경우 1

    public void Update()
    {

        if (characterOut == 1) // 1이면 나가고
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, outPos.position, moveSpeed * Time.deltaTime);
            if (soldierEvent == 1) //군인 이벤트 중일 경우
            {
                if (character.transform.position == outPos.position) //캐릭터가 완전히 나갔을 때 대사 넘기기 가능
                {
                    //UI_Assistant1.instance.stop = 0;
                    UserInputManager.instance.SetCanTouch(true);
                }
                else //완전히 나가지 않았으면 대사 못 넘김
                {
                    //UI_Assistant1.instance.stop = 1; 
                    UserInputManager.instance.SetCanTouch(false); // 터치로 대사 못 넘기게 함
                }

            }
        }
        else if (characterOut == 0) //0이면 들어옴
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, inPos.position, moveSpeed * Time.deltaTime);

        }
        else if (characterOut == 3)//군인 대화 이벤트, 공주 이동
        {
            character.transform.position = Vector3.MoveTowards(character.transform.position, inPos2.position, moveSpeed * Time.deltaTime);
            if(character.transform.position == inPos2.position)//공주 이동 끝나면 바로 찰스 등장
            {
                SetCharacter(GameScript1.instance.BigCharacter[10]);
                InCount();
            }
        }
        
    }
    public void OutCount() //캐릭터가 나가야할 때
    {
        characterOut = 1;
    }

    public void InCount() //캐릭터 들어와야 할 때
    {
         characterOut = 0;

    }
    public void SetCharacter(GameObject obj) // 어떤 캐릭터가 들어오고 나갈지 설정
    {
        character = obj;
    }

    public void PrincessMove()
    {
        characterOut = 3;
    } 
}
