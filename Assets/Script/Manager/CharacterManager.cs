using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;


public class CharacterManager : MonoBehaviour 
{
    public static CharacterManager instance;

    [SerializeField] GameObject[] bigCharacters; //큰 캐릭터 이미지 배열

    int characterNum; //캐릭터 넘버 

    #region (큰)캐릭터 움직임 관련 변수
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
    int faceNum = -1;//표정 비활성화에 쓰임

    int nextAppear = 0;
    bool checkingTrigger = true;
    public int currentEventState = 0;//0은 친밀도 이벤트 중이 아님을 뜻함, 이벤트 발생하는 캐릭터 번호 들어감, 1도리 2붕붕 3빵빵 4개나리 6도로시 7루루 8샌디 9친구 10찰스1 11찰스2 ,12무명이1, 13무명이2, 14히로디노, 15닥터펭, 16롤렝드

    List<string> availableCharacters = new List<string> (); //랜덤 방문 가능한 캐릭터들 리스트
    Queue<int> revisitCharacters = new Queue<int>(); //재방문할 수 있는 캐릭터 큐

    #region (작은)캐릭터 및 좌석 관련 변수
    [SerializeField] GameObject[] smallCharacters; //작은 캐릭터 이미지 배열
    [SerializeField] GameObject[] sittingCharacter = new GameObject[6];//좌석별 현재 앉아있는 캐릭터, 
    
    [SerializeField] Image soldierRightImage;//도로시와 같이 오는 찰스 이미지(오른쪽)
    [SerializeField] Image namelessSmile;

    [SerializeField] bool[] isTableEmpty = new bool[3]; //테이블이 비었는지 확인

    [SerializeField] int[] characterSeat = new int[15]; // 캐릭터 자리 저장 배열, 히로/디노 따로

    Queue<int> smallFOut = new Queue<int>(); //작은 캐릭터 페이드아웃 시 사용
    Queue<int> smallFadeIn = new Queue<int>(); //페이드인 시 사용

    Vector3[] seatPos = new Vector3[6]; //작은 캐릭터가 앉을 자리 위치 배열

    Queue<int> cleanSeat = new Queue<int>(); //비워질 자리 정보 큐
    #endregion

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

        seatPos[0] = new Vector3(-755,-210,0);
        seatPos[1] = new Vector3(-140,-225,0);
        seatPos[2] = new Vector3(-320,-390,0);
        seatPos[3] = new Vector3(390,-390,0);
        seatPos[4] = new Vector3(210,-200,0);
        seatPos[5] = new Vector3(790,-230,0);

        isTableEmpty[0] = true;
        isTableEmpty[1] = true;
        isTableEmpty[2] = true;
    }

    public void Update()
    {
        #region 큰 캐릭터 이동 관련
        if (characteInOutState == 1) //들어오기
        {
            bigCharacters[characterNum].transform.position = Vector3.MoveTowards(bigCharacters[characterNum].transform.position, charInPos, moveSpeed * Time.deltaTime);
            if(isSoldierEvent)
            {
                if(bigCharacters[characterNum].transform.position == charInPos && characterNum == 6)
                {
                    characterNum = 10; // 찰스 다이얼로그 이어가기 위함
                    SystemManager.instance.SetCanTouch(true, 1f); //도로시 등장 완료하면 터치해서 대사 넘기기 가능
                    DeactivateCharacterMoving();
                }
            }
        }
        else if (characteInOutState == 2) // 나가기
        {
            bigCharacters[characterNum].transform.position = Vector3.MoveTowards(bigCharacters[characterNum].transform.position, charOutPos, moveSpeed * Time.deltaTime);
            if (isSoldierEvent) //군인 이벤트 중일 경우
            {
                if (bigCharacters[characterNum].transform.position == charOutPos) //캐릭터가 완전히 나갔을 때 대사 넘기기 가능
                {
                    SystemManager.instance.SetCanTouch(true);
                    if(characterNum == 6) // 도로시까지 완전히 나갔으면
                    {
                        characterNum = 10;
                        SetSoldierEvent(false);
                    }
                }
                else //완전히 나가지 않았으면 대사 못 넘김
                {
                    SystemManager.instance.SetCanTouch(false); // 터치로 대사 못 넘기게 함
                }
            }
            else
            {
                if (bigCharacters[characterNum].transform.position == charOutPos) // 캐릭터가 완전히 화면 밖으로 나갔다면 비활성화
                    DeactivatebigCharacters();
            }
        }
        else if (characteInOutState == 3)//군인 대화 이벤트, 공주 이동
        {
            Vector3 princessInPos = new Vector3(150,-50,0); //군인 대화 이벤트, 공주 위치

            bigCharacters[characterNum].transform.position = Vector3.MoveTowards(bigCharacters[characterNum].transform.position, princessInPos, moveSpeed * Time.deltaTime);
            if(bigCharacters[characterNum].transform.position == princessInPos)//공주 이동 끝나면 바로 찰스 등장
            {
                CharacterIn(10);
            }
        }
        #endregion

        if (checkingTrigger && nextAppear <= 14 && !SystemManager.instance.IsUIOpen() && !Dialogue.instance.IsTalking() && currentEventState == 0
            && VisitorNote.instance.GetReplayState() == 0 && SystemManager.instance.GetMainCount() > 3)
        {// UI가 올라와있지 않고 대화 중이 아닐 때
            if (IsTableEmpty(1) || IsTableEmpty(2) || IsTableEmpty(3)) //테이블이 하나라도 비었다면
            {
                CheckCharacterTrigger();
            }
        }
    }

    #region 캐릭터 움직임 관련 함수
    public void CharacterIn(int cNum) //캐릭터 들어와야 할 때
    {
        characterNum = cNum;
        bigCharacters[characterNum].SetActive(true);
        characteInOutState = 1;
    }

    public void CharacterOut(int cNum = -1) //캐릭터가 나가야할 때
    {
        if(cNum != -1)
            characterNum = cNum;
            
        characteInOutState = 2;
    }

    public void DeactivateCharacterMoving() // 캐릭터 움직이기 비활성화
    {
        characteInOutState = 0;
    }

    public void MovePrincess() 
    {
        characteInOutState = 3;
        characterNum = 6;
    } 

    public void SetSoldierEvent(bool value)
    {
        isSoldierEvent = value;
    }
    #endregion

    public GameObject GetBigCharacter(int cNum)
    {
        return bigCharacters[cNum];
    }

    void DeactivatebigCharacters()//큰 캐릭터 비활성화하는 함수
    {
        bigCharacters[characterNum].SetActive(false);
        DeactivateCharacterMoving();
        if(faceNum != -1)//표정 비활성화해야할 것이 있으면
        {
            DeactivateCharacterFace();
        }
    }

    public int GetCharacterNum()
    {
        return characterNum;
    }

    public void SetCharacterNum(int value)
    {
        characterNum = value;
    }

    public void SetFaceNum(int value)
    {
        faceNum = value;
    }

    void DeactivateCharacterFace()
    {
        if (faceNum == 1)//도리
        {
            CharacterFaceList[1].face[1].SetActive(false);//표정 비활성화
        }
        else if (faceNum == 10)//찰스1,2
        {
            CharacterFaceList[8].face[2].SetActive(false);
        }
        else if (faceNum == 11 || faceNum == 7)//무명1, 루루
        {
            CharacterFaceList[faceNum - 2].face[1].SetActive(false);//표정 비활성화
        }
        else if (faceNum == 12)//무명2
        {
            CharacterFaceList[9].face[3].SetActive(false);
        }
        else if (faceNum == 13 || faceNum == 9)//닥터펭, 친구 이벤트의 경우
        {
            CharacterFaceList[faceNum - 2].face[0].SetActive(false);
        }
    }


    #region 캐릭터 첫 등장 및 이벤트 관련 함수
    public int GetCurrentEventState()
    {
        return currentEventState;
    }

    public void SetCurrentEventState(int value)
    {
        currentEventState = value;
    }

    public void SetNextAppearNum(int num)
    {
        nextAppear = num;
    }

    public int GetNextAppearNum()
    {
        return nextAppear;
    }

    public void CanCheckTrigger()
    {
        checkingTrigger = true;
    }

    void CheckCharacterTrigger()
    {
        int minReputation = 0; // 필요한 최소 평판
        bool isMenuOpen = true; // 특정 캐릭터는 메뉴 해제 해야함

        switch(nextAppear)
        {
            case 3:
                minReputation = 9;
                break;
            case 4:
                minReputation = 21;
                break;
            case 5:
                minReputation = 36;
                break;
            case 6:
                minReputation = 55;
                break;
            case 7:
                minReputation = 70;
                break;
            case 8:
                minReputation = 85;
                isMenuOpen = Menu.instance.IsMenuOpen(4);
                break;
            case 9:
                minReputation = 95;
                break;
            case 10:
                minReputation = 150;
                isMenuOpen = Menu.instance.IsMenuOpen(5);
                break;
            case 11:
                minReputation = 180;
                break;
            case 12:
                minReputation = 260;
                isMenuOpen = Menu.instance.IsMenuOpen(6);
                break;
            case 13:
                minReputation = 320;
                isMenuOpen = Menu.instance.IsMenuOpen(7);
                break;
            case 14:
                minReputation = 400;
                break;
        }
       
        if(Menu.instance.GetReputation() >= minReputation && isMenuOpen)
        {
            checkingTrigger = false;
            SystemManager.instance.BeginDialogue(nextAppear);
        }
    }

    IEnumerator FriendshipEvent(int cNum, float time = 0f)//친밀도 이벤트 확인
    {
        yield return new WaitForSeconds(time);

        int idx1 = -1;
        int idx2 = -1;
        if (cNum == 14 || cNum == 15)//닥터펭, 롤렝드의 경우
        {
            idx1 = cNum - 1;
            idx2 = cNum - 3;         
        }
        else
        {
            idx1 = cNum;
            switch (cNum)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    idx2 = cNum - 1;
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 12:
                    idx2 = cNum - 2;
                    break;
                case 10://찰스
                    if (Dialogue.instance.GetCharacterDC(10) == 1)//찰스1
                    {
                        if (VisitorNote.instance.GetFriendshipInfo(8) == 10)
                        {//친밀도 10일 때, 서빙횟수가 10번일 때
                            if (IsInvoking(nameof(RandomVisit)))
                            {
                                CancelInvoke(nameof(RandomVisit));
                                Debug.Log("랜덤방문 취소");
                            }
                            currentEventState = cNum; //두 개 이상의 테이블이 비었고 도로시가 방문가능캐릭터일 때 실행, 도로시까지 필요하기 때문
                            if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                            {
                                StartCoroutine(FriendshipEvent(cNum, 1f));
                            }
                            else
                            {
                                if (((IsTableEmpty(1) && IsTableEmpty(2)) ||
                                (IsTableEmpty(1) && IsTableEmpty(3)) ||
                                (IsTableEmpty(2) && IsTableEmpty(3)))
                                && availableCharacters.Contains("small_6Princess")) //2개 이상의 테이블이 비었을 때 시작 가능
                                {
                                    checkingTrigger = false;
                                    SetCharacter(6);//도로시 페이드인
                                    availableCharacters.Remove("small_6Princess");//도로시는 이 이벤트에서 카페를 방문함
                                }
                                else //두 개 이상의 테이블이 비지 않았다면
                                {
                                    StartCoroutine(FriendshipEvent(cNum, 1f));
                                }
                            }
                            yield break;
                        }
                    }
                    else if (Dialogue.instance.GetCharacterDC(10) == 2)//찰스2
                    {
                        if (VisitorNote.instance.IsFriendshipGaugeFull(cNum-2) && VisitorNote.instance.IsFriendshipGaugeFull(4))
                        {// 찰스, 도로시 친밀도 모두 최대일때
                            if (IsInvoking(nameof(RandomVisit)))
                            {
                                CancelInvoke(nameof(RandomVisit));
                                Debug.Log("랜덤방문 취소");
                            }
                            currentEventState = 11; 
                            if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                            {
                                StartCoroutine(FriendshipEvent(cNum, 1f));
                            }
                            else
                            {
                                if ((IsTableEmpty(1) && IsTableEmpty(2)) ||
                                (IsTableEmpty(1) && IsTableEmpty(3)) ||
                                (IsTableEmpty(2) && IsTableEmpty(3)))
                                {//두 개 이상의 테이블이 비었을 때 실행, 도로시까지 필요하기 때문
                                    MenuHint.instance.CantTouchMHB();
                                    SystemManager.instance.CantTouchUI();
                                    checkingTrigger = false;
                                    SetCharacter(6);//도로시 페이드인
                                    availableCharacters.Remove("small_6Princess");//도로시는 이 이벤트에서 카페를 방문함
                                }
                                else //두 개 이상의 테이블이 비지 않았다면
                                {
                                    StartCoroutine(FriendshipEvent(cNum, 1f));
                                }
                            }
                            yield break;
                        }
                    }
                    break;
                case 11:
                    if (Dialogue.instance.GetCharacterDC(11) == 1)//무명이1
                    {
                        if (VisitorNote.instance.GetFriendshipInfo(9) == 10)//친밀도 10일때
                        {
                            if (IsInvoking(nameof(RandomVisit)))
                            {
                                CancelInvoke(nameof(RandomVisit));
                                Debug.Log("랜덤방문 취소");
                            }
                            currentEventState = 12;//친밀도 이벤트 진행 중
                            if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                            {
                                StartCoroutine(FriendshipEvent(cNum, 1f));
                            }
                            else
                            {
                                checkingTrigger = false;
                                SetCharacter(11);//캐릭터를 설정하고 바로 페이드인    
                            }
                            yield break;
                        }
                    }
                    else if (Dialogue.instance.GetCharacterDC(11) == 2)//무명이2
                    {
                        if (VisitorNote.instance.IsFriendshipGaugeFull(cNum-2))//친밀도 20일때
                        {
                            if (IsInvoking(nameof(RandomVisit)))
                            {
                                CancelInvoke(nameof(RandomVisit));
                                Debug.Log("랜덤방문 취소");
                            }
                            MenuHint.instance.CantTouchMHB();
                            SystemManager.instance.CantTouchUI();
                            currentEventState = 13;//친밀도 이벤트 진행 중
                            if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                            {
                                StartCoroutine(FriendshipEvent(cNum, 1f));
                            }
                            else
                            {
                                checkingTrigger = false;
                                SetCharacter(11);//캐릭터를 설정하고 바로 페이드인    
                            }
                            yield break;
                        }
                    }
                break;
            }
        }     

        if (cNum != 5 && cNum != 10 && cNum != 11 && Dialogue.instance.GetCharacterDC(idx1) != 0 && Dialogue.instance.GetCharacterDC(idx1) != 3)
        {//또롱, 찰스, 무명이 제외
            if (VisitorNote.instance.IsFriendshipGaugeFull(idx2))//친밀도 게이지가 꽉 찼을 때
            {
                if (IsInvoking(nameof(RandomVisit)))
                {
                    CancelInvoke(nameof(RandomVisit));
                }

                if (cNum != 9 && cNum != 15) 
                {
                    MenuHint.instance.CantTouchMHB();
                    SystemManager.instance.CantTouchUI();
                }

                if(cNum == 12)
                    currentEventState = cNum + 2;
                else if(cNum == 14 || cNum == 15)
                    currentEventState = cNum + 1;
                else
                    currentEventState = cNum;

                if (SystemManager.instance.IsUIOpen() || Dialogue.instance.IsTalking() || VisitorNote.instance.GetReplayState() != 0)//만약 UI가 올라와있다면
                {
                    StartCoroutine(FriendshipEvent(cNum, 1f));
                }
                else
                {
                    checkingTrigger = false;
                    SetCharacter(cNum);//캐릭터를 설정하고 바로 페이드인    
                }
            }        
        }
        
        if (GetCurrentEventState() == 0) //이벤트가 발생하지 않으면 일반 방문 진행
        {
            SetCharacter(cNum); //방문할 캐릭터 세팅
            Invoke(nameof(RandomVisit), 7f);
            //Debug.Log("랜덤방문 7초뒤");
        }
        yield break;
    }
    #endregion

    #region 캐릭터 랜덤 방문 및 재방문 관련 함수
    public void RandomVisit()//캐릭터들의 카페 방문, 등장한 캐릭터 중에 랜덤으로 방문
    {
        if ((VisitorNote.instance.GetFirstMeetID() == 0 && VisitorNote.instance.GetFriendEventID() == 0 && Dialogue.instance.IsTalking()) || PlayerPrefs.GetInt("EndingState") == 1)//캐릭터와 대화 중일 경우 혹은 엔딩이벤트 할 경우 함수 종료
        {
            if(!IsInvoking(nameof(RandomVisit)))
            {
                Invoke(nameof(RandomVisit), 3f);
                //Debug.Log("랜덤방문 3초뒤");

            }      
            return;
        }
        if(availableCharacters.Count == 0) //현재 방문 가능한 캐릭터가 없을 시 3초 뒤 이 함수 재실행
        {
            if(!IsInvoking(nameof(RandomVisit)))
            {
                Invoke(nameof(RandomVisit), 3f);
                //Debug.Log("수동 방문 가능 캐릭터 없음, 3초 뒤");
            }           
            return;
        }
        if (!IsTableEmpty(1) && !IsTableEmpty(2) && !IsTableEmpty(3)) //세 테이블에 모두 손님이 있으면
        {
            if (!IsInvoking(nameof(RandomVisit)))
            {
                Invoke(nameof(RandomVisit), 5f);
                //Debug.Log("수동 빈 테이블 없음, 5초 뒤");
            }
            return;
        }
        else //최소 한 테이블 이상 비었으면
        {
            //Debug.Log("방문가능캐릭터리스트 크기 " + availableCharacters.Count);
            int num = Random.Range(0, availableCharacters.Count - 1); //랜덤 캐릭터 넘버
            int cNum = 0;
            switch (availableCharacters[num])
            {
                case "small_1Bear":
                    cNum = 1;
                    availableCharacters.Remove("small_1Bear"); 
                    break;
                case "small_2Car":
                    cNum = 2;
                    availableCharacters.Remove("small_2Car");
                    break;
                case "small_3Bread":
                    cNum = 3;
                    availableCharacters.Remove("small_3Bread");
                    break;
                case "small_4Rabbit":
                    cNum = 4;
                    availableCharacters.Remove("small_4Rabbit");
                    break;
                case "small_5Ddorong":
                    cNum = 5;
                    availableCharacters.Remove("small_5Ddorong");
                    break;
                case "small_6Princess":
                    cNum = 6;
                    availableCharacters.Remove("small_6Princess");
                    break;
                case "small_7FamilySeries":
                    cNum = 7;
                    availableCharacters.Remove("small_7FamilySeries");
                    break;
                case "small_8SunFlower":
                    cNum = 8;
                    availableCharacters.Remove("small_8SunFlower");
                    break;
                case "small_9Dog":
                    cNum = 9;
                    availableCharacters.Remove("small_9Dog");
                    break;
                case "small_10Soldier":
                    cNum = 10;
                    availableCharacters.Remove("small_10Soldier");
                    break;
                case "small_11Nameless":
                    cNum = 11;
                    availableCharacters.Remove("small_11Nameless");
                    break;
                case "small_12Hero&13Dianosoour":
                    cNum = 12;//히로부터 등장
                    availableCharacters.Remove("small_12Hero&13Dianosoour");
                    break;
                case "small_14Penguin":
                    cNum = 14;
                    availableCharacters.Remove("small_14Penguin");
                    break;
                case "small_15Grandfather":
                    cNum = 15;
                    availableCharacters.Remove("small_15Grandfather");
                    break;
                case "small_6Princess&10Soldier":
                    cNum = 6;//도로시부터 등장
                    availableCharacters.Remove("small_6Princess&10Soldier");
                    break;
            }
            
            StartCoroutine(FriendshipEvent(cNum)); //방문할 캐릭터의 친밀도 이벤트가 발생하는지 확인
        }
        
    }

    public void AddToRevisit(int cNum)
    {
        revisitCharacters.Enqueue(cNum);
    }

    public void EnableRevisit()
    {
        if(revisitCharacters.Count != 0)//큐가 0이 아닐 때만 실행
        {
            switch (revisitCharacters.Peek())
            {
                case 1:
                    availableCharacters.Add("small_1Bear");
                    //Debug.Log("도리 재방문 가능");
                    break;
                case 2:
                    availableCharacters.Add("small_2Car");
                    //Debug.Log("붕붕 재방문 가능");
                    break;
                case 3:
                    availableCharacters.Add("small_3Bread");
                    // Debug.Log("빵빵 재방문 가능");
                    break;
                case 4:
                    availableCharacters.Add("small_4Rabbit");
                    // Debug.Log("개나리 재방문 가능");
                    break;
                case 5:
                    availableCharacters.Add("small_5Ddorong");
                    // Debug.Log("또롱 재방문 가능");
                    break;
                case 6:
                    availableCharacters.Add("small_6Princess");
                    //Debug.Log("도로시 재방문 가능");
                    break;
                case 7:
                    availableCharacters.Add("small_7FamilySeries");
                    //Debug.Log("루루 재방문 가능");
                    break;
                case 8:
                    availableCharacters.Add("small_8SunFlower");
                    // Debug.Log("샌디 재방문 가능");
                    break;
                case 9:
                    availableCharacters.Add("small_9Dog");
                    // Debug.Log("친구 재방문 가능");
                    break;
                case 10:
                    availableCharacters.Add("small_10Soldier");
                    //Debug.Log("찰스 재방문 가능");
                    break;
                case 11:
                    availableCharacters.Add("small_11Nameless");
                   // Debug.Log("무명이 재방문 가능");
                    break;
                case 13://히로디노
                    availableCharacters.Add("small_12Hero&13Dianosoour");
                   // Debug.Log("히로디노 재방문 가능");
                    break;
                case 14:
                    availableCharacters.Add("small_14Penguin");
                   // Debug.Log("닥터펭 재방문 가능");
                    break;
                case 15:
                    availableCharacters.Add("small_15Grandfather");
                   // Debug.Log("롤렝드 재방문 가능");
                    break;
                case 17:
                    availableCharacters.Add("small_6Princess&10Soldier");
                  // Debug.Log("찰스도로시 재방문 가능");
                    break;
            }
            //Debug.Log("함수 EnableRevisit");
            revisitCharacters.Dequeue();
        }       
    }
    #endregion

    #region (작은)캐릭터 및 좌석 관련 함수
    public int GetCharacterSeatNum(int cNum)
    {
        return characterSeat[cNum-1];
    }

    public GameObject GetSmallCharacter(int cNum)
    {
        return smallCharacters[cNum];
    }

    public GameObject GetSittingCharacter(int seatNum)
    {
        return sittingCharacter[seatNum];
    }

    public void CantTouchCharacter(int n) //캐릭터 클릭 못하게
    {
        smallCharacters[n].GetComponent<Button>().interactable = false;
    }

    public void CanTouchCharacter(int n) //캐릭터 클릭 가능하게, 페이드인하고 가능
    {
        smallCharacters[n].GetComponent<Button>().interactable = true;
    }

    public void SetCharacter(int cNum) //작은 캐릭터 설정, cNum은 캐릭터 넘버
    {
        smallFadeIn.Enqueue(cNum);
        //Debug.Log("셋캐릭터 " + cNum);

        if (cNum == 0 || cNum >= 16) // 제제/주인공은 바로 페이드인
        {
            Invoke(nameof(FadeIn),1f);
            return;
        }
        
        SetSeatPosition(cNum);

        if (SystemManager.instance.GetMainCount() == 2)//서빙 튜토리얼일 경우
        {
            //회색 패널보다 레이어가 뒤에 위치해있어서 터치가 안되므로 일시적으로 순서를 패널보다 앞쪽으로 배치한다
            smallCharacters[1].transform.parent.GetComponent<Canvas>().sortingOrder = 7;
        }
        //Debug.Log("함수 SetCharacter");
    }

    public void SetSeatPosition(int cNum) //앉을 자리 설정
    {
        int seatNum = 0; //자리 넘버

        if (SystemManager.instance.GetMainCount() == 2)//서빙 튜토리얼일 경우
        {
            seatNum= 1; //2번째 자리, 첫번째 짝수자리, 첫번째 테이블 오른쪽 자리
        }
        else//튜토리얼 아닐 시 자리 배정
        {
            if(cNum == 13)//캐릭터가 디노면, 히로 자리에 따라서 옆에 배정
            {
                seatNum = characterSeat[11] + 1;
            }
            else if(cNum == 10 && (Dialogue.instance.GetCharacterDC(10) == 3) || (GetCurrentEventState() == 11 && Dialogue.instance.GetSpecialMenuState() == 1))
            {//도로시와 같이 오는 찰스, 찰스2이벤트 중 찰스 중간 페이드인
                seatNum = characterSeat[5] + 1;
                smallCharacters[10].GetComponent<Image>().sprite = soldierRightImage.sprite;//이미지 변경
            }          
            else // 혼자인 캐릭터 자리 설정
            {
                if (cNum == 11 && Dialogue.instance.GetCharacterDC(11) == 3)//무명이, 시나리오 다 봤을 때
                {
                    smallCharacters[11].GetComponent<Image>().sprite = namelessSmile.sprite;//이미지 변경
                }

                if (IsTableEmpty(1) && IsTableEmpty(2) && IsTableEmpty(3)) //모든 테이블이 빈 상태면
                {
                    //Debug.Log("테이블이 모두 비었음");
                    seatNum = Random.Range(0, 5); // 0~5번 자리 모두 착석 가능
                    if (cNum % 2 == 1) //홀수 번호 캐릭터면 오른쪽(1,3,5)에 앉음
                    {
                        if(seatNum % 2 == 0) seatNum += 1; // 짝수가 나오면 1 더해서 홀수 만들기
                    }
                    else if (cNum % 2 == 0) //짝수 번호 캐릭터는 왼쪽(0,2,4)에 앉음
                    {
                        if(seatNum % 2 == 1) seatNum -= 1; // 홀수가 나오면 1 빼서 홀수 만들기
                    }
                }
                else if (IsTableEmpty(1) && IsTableEmpty(2)) //1,2번 테이블만 빈 상태면
                {
                    //Debug.Log("1,2테이블이 비었음");
                    seatNum = Random.Range(0, 3); //0~3번 자리 착석 가능
                    if (cNum % 2 == 1) 
                    {
                        if(seatNum % 2 == 0) seatNum += 1;
                    }
                    if (cNum % 2 == 0) 
                    {
                        if(seatNum % 2 == 1) seatNum -= 1;
                    }
                }
                else if (IsTableEmpty(1) && IsTableEmpty(3))//1,3번 테이블만 빈 상태면
                {
                    //Debug.Log("1,3테이블이 비었음");
                    seatNum = Random.Range(0, 5); // 2,3번은 착석 불가
                    if (cNum % 2 == 1) 
                    {
                        if(seatNum == 2) seatNum -= 1;
                        else if(seatNum == 3) seatNum += 2;
                        else if(seatNum % 2 == 0) seatNum += 1;
                    }
                    if (cNum % 2 == 0) 
                    {
                        if(seatNum == 2) seatNum -= 2;
                        else if(seatNum == 3) seatNum += 1;
                        else if(seatNum % 2 == 1) seatNum -= 1;
                    }
                }
                else if (IsTableEmpty(2) && IsTableEmpty(3)) //2,3번 테이블만 빈 상태면
                {
                  //  Debug.Log("2,3테이블이 비었음");
                    seatNum = Random.Range(2, 5);

                    if (cNum % 2 == 1) 
                    {
                        if(seatNum % 2 == 0) seatNum += 1;
                    }

                    if (cNum % 2 == 0) 
                    {
                        if(seatNum % 2 == 1) seatNum -= 1;
                    }
                }
                else if (IsTableEmpty(1)) //1번 테이블만 빈 상태
                {
                  //  Debug.Log("1테이블만 비었음");
                    if (cNum % 2 == 1) 
                    {
                        seatNum = 1;
                    }

                    if (cNum % 2 == 0) 
                    {
                        seatNum = 0;
                    }
                }
                else if (IsTableEmpty(2)) //2번 테이블만 빈 상태
                {
                   // Debug.Log("2테이블만 비었음");
                    if (cNum % 2 == 1) 
                    {
                        seatNum = 3;
                    }

                    if (cNum % 2 == 0) 
                    {
                        seatNum = 2;
                    }
                }
                else if (IsTableEmpty(3)) //3번 테이블만 비었을 때
                {
                    //Debug.Log("3테이블만 비었음");
                    if (cNum % 2 == 1) 
                    {
                        seatNum = 5;
                    }

                    if (cNum % 2 == 0) 
                    {
                        seatNum = 4;
                    }
                }
            }              
        }
        
        characterSeat[cNum - 1] = seatNum ; //캐릭터 자리 정보 저장, 
        //Debug.Log("캐릭넘버 " + cNum + " 자리: " + seatNum);

        // 앉을 자리로 옮기기
        smallCharacters[cNum].transform.position = seatPos[seatNum];
        sittingCharacter[seatNum] = smallCharacters[cNum];
        switch (seatNum) // 테이블 착석 여부 갱신, false 착석, true 비었음
        {
            case 0: 
            case 1:
                isTableEmpty[0] = false;
                break;
            case 2:
            case 3:
                isTableEmpty[1] = false;
                break;
            case 4:
            case 5:
                isTableEmpty[2] = false;
                break;
        }
        
        int n;
        if(cNum > 12)//디노부터~
        {
            n = cNum - 1;
        }
        else
        {
            n = cNum;
        }

        if(SystemManager.instance.GetMainCount() > 2 && Dialogue.instance.GetCharacterDC(n) != 0 && GetCurrentEventState() != 2)
        {//붕붕이 친밀도 이벤트, 캐릭터 첫 등장 제외하고 메인카운트 3이상이면 바로 캐릭터 페이드인
            if(IsInvoking(nameof(FadeIn)))//다른 캐릭터 페이드인이 인보크 중이면
            {
                Invoke(nameof(FadeIn), 0.7f); //0.7초 뒤 실행
            }
            else
            {
                FadeIn();        
            }          
        }

        if (GetCurrentEventState() == 2)//특정 캐릭터들은 바로 시나리오 이벤트 시작
        {
            SystemManager.instance.BeginDialogue(GetCurrentEventState());
        }
        if (GetCurrentEventState() != 0 && GetCurrentEventState() != 14)//히로디노이벤트가 아니면 주인공 아기 캐릭터 자리 설정
        {
            if(GetCurrentEventState() == 11 && (cNum == 6 || Dialogue.instance.GetSpecialMenuState() == 1)
            || GetCurrentEventState() == 10 && (cNum == 6)) return; // 찰스1이벤트, 찰스2이벤트의 도로시일 때는 아기 자리 세팅X
            SetBabySeat(seatNum);          
        }
        //Debug.Log("함수 SetSeatPosition");
    }

    public void SetBabySeat(int sNum)
    {
        if(sNum % 2 == 0) // 캐릭터 자리가 왼쪽(짝수)면
        {
            smallCharacters[16].transform.position = seatPos[sNum+1]; //주인공 아기를 오른쪽 자리로 옮김
            sittingCharacter[sNum+1] = smallCharacters[16];
        }
        else
        {
            smallCharacters[17].transform.position = seatPos[sNum-1];
            sittingCharacter[sNum-1] = smallCharacters[17];
        }
    }

    public bool IsTableEmpty(int tableNum)
    {
        return isTableEmpty[tableNum - 1];
    }

    public void CleanTable() // 손님이 가고 테이블을 빈 걸로 표시
    {
        switch (cleanSeat.Peek())
        {
            case 0:
            case 1:
                isTableEmpty[0] = true;
                break;
            case 2:
            case 3:
                isTableEmpty[1] = true;
                break;
            case 4:
            case 5:
                isTableEmpty[2] = true;
                break;
        }
        cleanSeat.Dequeue();

        //Debug.Log(fNum + "자리 비워짐");
        if(SystemManager.instance.GetMainCount() > 7)//메인카운트가 7이상이면
        {
            Invoke(nameof(EnableRevisit), 8f);//8초 뒤 재방문 가능
        }
        else
        {
            Invoke(nameof(EnableRevisit), 3f);
        }
        //Debug.Log("함수 CleanTable");       
    }

    public void FadeIn() //작은 캐릭터 페이드인
    {
        StartCoroutine(CharFadeToFullAlpha()); //페이드인 시작
        //Debug.Log("함수 SmallFadeIn");
    }

   public IEnumerator CharFadeToFullAlpha() // 알파값 0에서 1로 전환
    {
        if(smallFadeIn.Count == 0) yield break;

        int v = smallFadeIn.Peek();
       
       // Debug.Log("페이드인 될 캐릭터" + v);

        smallCharacters[v].SetActive(true); //작은 캐릭터 활성화      

        smallCharacters[v].GetComponent<Image>().color = new Color(smallCharacters[v].GetComponent<Image>().color.r,smallCharacters[v].GetComponent<Image>().color.g,smallCharacters[v].GetComponent<Image>().color.b, 0);
        while (smallCharacters[v].GetComponent<Image>().color.a < 1.0f)
        {
           smallCharacters[v].GetComponent<Image>().color = new Color(smallCharacters[v].GetComponent<Image>().color.r,smallCharacters[v].GetComponent<Image>().color.g,smallCharacters[v].GetComponent<Image>().color.b,smallCharacters[v].GetComponent<Image>().color.a + (Time.deltaTime / 0.8f));
            yield return null;
        }
        
        smallFadeIn.Dequeue();
        //Debug.Log("페이드인 됨" + v);


        if ((GetCurrentEventState() == 0 || GetCurrentEventState() == 9 || GetCurrentEventState() == 12 || GetCurrentEventState() == 16) 
            && v != 0 && v != 17 && v != 16)
        {//친밀도 이벤트가 아니고 제제가 아니고, 주인공 아기가 아닐 때만 캐릭터 클릭 가능, 친구,무명이1,롤렝드 친밀도 이벤트시에는 가능,
            CanTouchCharacter(v);
        }
        
        if(GetCurrentEventState() == 10 || GetCurrentEventState() == 11)//찰스1,2 이벤트일 경우
        {
            if(v == 6)//도로시 페이드인했으면
            {
                SetCharacter(10); //찰스 페이드인
            }
            if(GetCurrentEventState() == 10 && v == 10)//찰스만 클릭가능
            {
                CanTouchCharacter(10);
            }
        }

        if(GetCurrentEventState() != 0 && GetCurrentEventState() != 10 && GetCurrentEventState() != 11 && v != 16 && v != 17 && v != 2 && v != 9 
            || (GetCurrentEventState() == 11 && v == 10))
        {//친밀도 이벤트가 진행 중이고, 주인공 아기가 아니고, 특정 캐릭터(이벤트)가 아닐때
            if (GetCurrentEventState() <= 10)
            {
                SystemManager.instance.BeginDialogue(GetCurrentEventState()); //해당 캐릭터 시나리오 등장
            }
            else if (GetCurrentEventState() >= 11 && GetCurrentEventState() <= 13)//이벤트 넘버 11~13
            {
                if (Dialogue.instance.GetSpecialMenuState() == 0 && GetCurrentEventState() != 12)//첫번째 조건은 찰스2 이벤트 때문, 두번째 조건은 무명이1 이벤트때문
                {
                    SystemManager.instance.BeginDialogue(v);
                }           
            }
            else if (GetCurrentEventState() == 15)//닥터 펭 친밀도 이벤트의 경우
            {
                int v2 = v - 1;
                SystemManager.instance.BeginDialogue(v2);              
            }
        }

        if(v == 12)//히로 페이드인 끝나면 디노 빨리 나타나게끔
        {
            if (GetCurrentEventState() == 14)//히로디노 친밀도 이벤트 시
            {
                CanTouchCharacter(v);//히로 먼저 클릭 가능
            }
            SetCharacter(13);           
        }
        if(Dialogue.instance.GetCharacterDC(10) == 3 && v == 6)//찰스와 같이 방문한 도로시 페이드인 끝나면 찰스 나타나게끔
        {
            SetCharacter(10);
        }
        yield break;
    }

    public void FadeOutJeje()
    {
        smallFOut.Enqueue(0);
        StartCoroutine(CharFadeToZero());//페이드아웃 시작         
    }

    public void FadeOut(int cNum, int sNum = -1) //작은 캐릭터 페이드아웃
    {   
        //Debug.Log("캐릭터 페이드아웃" + cNum);
        smallFOut.Enqueue(cNum);

        if(cNum != 12 && cNum  < 16) // 히로, 주인공은 패스
        {
            //  도로시/찰스 제외 캐릭터거나 찰스2이벤트 전이거나 찰스2이벤트 완료+찰스일 때
            if((cNum  != 6 && cNum  != 10) || Dialogue.instance.GetCharacterDC(10) != 3 || (Dialogue.instance.GetCharacterDC(10) == 3 && cNum  == 10))
                cleanSeat.Enqueue(characterSeat[cNum -1]); //비워질 자리 큐에 정보 추가
        }
    
        StartCoroutine(CharFadeToZero());//페이드아웃 시작    
        if(sNum == -1)
            sittingCharacter[characterSeat[cNum -1]] = null;//버그 대비 페이드아웃 시 null 넣기
        else
            sittingCharacter[sNum] = null;

        //Debug.Log("함수 SmallFadeOut");
    }
    
    public IEnumerator CharFadeToZero()  // 알파값 1에서 0으로 전환
    {
        if(smallFOut.Count == 0) yield break;

        int cNum = smallFOut.Peek();
        smallCharacters[cNum].GetComponent<Image>().color = new Color(smallCharacters[cNum].GetComponent<Image>().color.r, smallCharacters[cNum].GetComponent<Image>().color.g, smallCharacters[cNum].GetComponent<Image>().color.b, 1);
        while (smallCharacters[cNum].GetComponent<Image>().color.a > 0.0f)
        {
            smallCharacters[cNum].GetComponent<Image>().color = new Color(smallCharacters[cNum].GetComponent<Image>().color.r, smallCharacters[cNum].GetComponent<Image>().color.g, smallCharacters[cNum].GetComponent<Image>().color.b, smallCharacters[cNum].GetComponent<Image>().color.a - (Time.deltaTime / 0.8f));
            yield return null;
        }
//        Debug.Log("페이드아웃 됨" + cNum);
        smallFOut.Dequeue();

        if(GetCurrentEventState() == 0 && Dialogue.instance.GetSpecialMenuState() == 2)//친밀도 이벤트가 끝났을 때
        {
            if(cNum != 12)//히로만 아니면
            {
                Dialogue.instance.SetSpecialMenuState(0);
            }   

            int sNum;
            if(cNum % 2 == 1 && cNum != 13) // 홀수 캐릭터(디노 제외)
            {
                sNum = characterSeat[cNum-1]-1;
                FadeOut(17, sNum); //주인공(왼쪽) 페이드아웃
            }

            if(cNum % 2 == 0 && cNum != 12) // 짝수 캐릭터(히로 제외)
            {
                sNum = characterSeat[cNum-1]+1;
                FadeOut(16, sNum);
            }
        }

        if (cNum != 0) //제제가 아닐 경우
        {
            smallCharacters[cNum].SetActive(false);//캐릭터 비활성화

            if(cNum != 16 && cNum != 17)//아기 제외
            {
                characterSeat[cNum - 1] = 0;//자리 0으로 초기화
                
                if(cNum == 12) yield break; // 히로는 패스, 디노일 때 실행

                if (cNum == 6 || cNum == 10)//도로시나 찰스일 경우
                {
                    if (Dialogue.instance.GetCharacterDC(10) == 3 && cNum == 10)
                    {//찰스2이벤트 이후면
                        Invoke(nameof(CleanTable), 2f);
                        AddToRevisit(17);
                    }
                    else if(Dialogue.instance.GetCharacterDC(10) != 3)//찰스2 이벤트 전이면 다른 캐릭터와 똑같이 실행
                    {
                        CleanTable();
                        if(GetCurrentEventState() != 11) // 찰스2이벤트 중에는 실행X
                            AddToRevisit(cNum);
                    }
                }
                else if(cNum == 13) // 디노
                {
                    Invoke(nameof(CleanTable), 2f);
                    AddToRevisit(13);
                }
                else //혼자인 캐릭터
                {
                    CleanTable();
                    AddToRevisit(cNum);
                }        
            }
        }      
    }
    #endregion
}