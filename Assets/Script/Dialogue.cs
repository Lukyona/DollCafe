using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour //캐릭터들 대화
{
    public static Dialogue instance;
    int characterNum = 0; //캐릭터 번호, 0은 제제, 1은 도리, 2는 붕붕, …
    private TextWriter.TextWriterSingle textWriterSingle;
    public Text CName; //캐릭터 이름
    public string[] messegeArray = null;
    public int[] CharacterDC = new int[15]; //캐릭터들 다이얼로그 카운트, 인덱스 0 제제, 1 도리, BackToCafe에서 ++

    string babyName; //아기 이름

    bool isBabyText = false;


    private void Awake()
    {     
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update() //화면의 아무 곳이나 터치하면 다음 대사 나타남
    {
        if (UI_Assistant1.instance.talking && SystemManager.instance.CanTouch() && Input.GetMouseButtonDown(0))
        {
            UI_Assistant1.instance.OpenDialogue2();
        }
    }

    public void SetCharacterNum(int num)
    {
        characterNum = num;
    }

    public void SetBabyName(string name)
    {
        babyName = name;
    }

    public void SetIsBabyText(bool value)
    {
        isBabyText = value;
    }

    public bool IsBabayText()
    {
        return isBabyText;
    }

    public void UpdateCharacterDC(int cNum = -1)
    {
        if(cNum != -1)
            characterNum = cNum;

        ++CharacterDC[characterNum];
    }

    public bool SaveCharacterDCInfo()//캐릭터 대화 진행 정보 저장
    {
        bool result = false;
        try
        {
            string strArr = ""; // 문자열 생성

            for (int i = 0; i < CharacterDC.Length; i++) // 배열과 ','를 번갈아가며 tempStr에 저장
            {
                strArr = strArr + CharacterDC[i];
                if (i < CharacterDC.Length - 1) // 최대 길이의 -1까지만 ,를 저장
                {
                    strArr = strArr + ",";
                }
            }

            PlayerPrefs.SetString("CharacterDC", strArr); // PlyerPrefs에 문자열 형태로 저장
            PlayerPrefs.Save(); //세이브
            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveCharacterDCInfo Failed (" + e.Message + ")");
        }
        return result;
    }

    public bool LoadCharacterDCInfo() //캐릭터 대화 진행 정보 불러오기
    {
        bool result = false;
        try
        {
            if (PlayerPrefs.HasKey("CharacterDC"))
            {
                string[] dataArr = PlayerPrefs.GetString("CharacterDC").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의 ,로 구분하여 배열에 저장

                for (int i = 0; i < dataArr.Length; i++)
                {
                    CharacterDC[i] = System.Convert.ToInt32(dataArr[i]); // 문자열 형태로 저장된 값을 정수형으로 변환후 저장                  
                }
            }

            result = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadCharacterDCInfo Failed (" + e.Message + ")");
        }
        return result;
    }

    public void SelectedFirstDialogue() //대화창이 등장할 때의 첫 문장
    {
        switch (characterNum)
        {
            case 0:
                HelperStartDialogue();
                break;
            case 1:
                BearStartDialogue();
                break;
            case 2:
                CarFirstDialogue();
                break;
            case 3:
                BreadFirstDialogue();
                break;
            case 4:
                RabbitFirstDialogue();
                break;
            case 5:
                DdorongFirstDialogue();
                break;
            case 6:
                PrincessFirstDialogue();
                break;
            case 7:
                FamilySeriesFirstDialogue();
                break;
            case 8:
                SunflowerFirstDialogue();
                break;
            case 9:
                DogFirstDialogue();
                break;
            case 10:
                SoldierFirstDialogue();
                break;
            case 11:
                NoNameFirstDialogue();
                break;
            case 12:
                HeroDinosourFirstDialogue();
                break;
            case 13:
                PenguinFirstDialogue();
                break;
            case 14:
                GrandfatherFirstDialogue();
                break;

        }
    }

    public void SelectedNextDialogue() //첫 문장 후의 대사들
    {
        switch (characterNum)
        {
            case 0:
                HelperNextDialogue();
                break;
            case 1:
                BearNextDialogue();
                break;
            case 2:
                CarNextDialogue();
                break;
            case 3:
                BreadNextDialogue();
                break;
            case 4:
                RabbitNextDialogue();
                break;
            case 5:
                DdorongNextDialogue();
                break;
            case 6:
                PrincessNextDialogue();
                break;
            case 7:
                FamilySeriesNextDialogue();
                break;
            case 8:
                SunflowerNextDialogue();
                break;
            case 9:
                DogNextDialogue();
                break;
            case 10:
                SoldierNextDialogue();
                break;
            case 11:
                NoNameNextDialogue();
                break;
            case 12:
                HeroDinosourNextDialogue();
                break;
            case 13:
                PenguinNextDialogue();
                break;
            case 14:
                GrandfatherNextDialogue();
                break;
        }
    }


    private void HelperStartDialogue() //제제 대사 첫 문장
    {
        switch (CharacterDC[0])
        {
            case 0://게임 처음 시작
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "드디어 왔구나!");
                CName.text = "??";
                break;
            case 1://서빙 튜토리얼
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "자, 이젠 실전이야!");
                CName.text = "제제";
                break;
            case 2://엔딩 이벤트
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "우리 카페가 이렇게나 성장하다니!\n정말 놀라운걸?");
                CName.text = "제제";
                break;

        }
    }

    private void HelperNextDialogue() //제제 첫 문장 제외한 대사 
    {
        switch (CharacterDC[0])
        {
            case 0: //게임 처음 시작, 튜토리얼
                messegeArray = new string[] {
                "이제 네가 왔으니 다시 카페를 열 수 있겠다!",
                "으응..? 카페...?", //baby 1
                "응? 아아, 기억이 안 난다구?",
                "음.. 그럴 수 있어!",
                ".. 설마 네 이름까지 까먹어버린 건 아니겠지?",//4
                "당연하지!\n내 이름은 " + babyName + "인 걸!",
                "좋아, 그러면 내 이름이\n제제인 것도 기억하려나?",
                "제제? 제제는.. 음..\n맨날 나랑 같이 자는..", //baby 7
                "..크흠!!\n바쁘니까 얼른 설명 시작할게!",
                "우선 오른쪽 위에 있는 메뉴판!",//9
                "여기에 있는 음료나 디저트를\n손님에게 대접할 수 있어.", //\n으로 줄바꿈
                "어떤 메뉴들이 있는 지는 나중에 확인해봐!",
                "다음은 메뉴판 옆에 보이는 손님노트야.",//12
                "카페를 다녀간 손님들이 직접 쓰거나\n우리가 쓰기도 해.",
                "음.. 말하자면 단골 목록같은 거지!",
                "그리고 왼쪽 위에 있는 하트는 체력이야.",//15
                "손님에게 서빙할 때마다 1씩 줄어들지.",
                "체력이 없으면 서빙을 하지 못하니까 주의해야 해.",
                "만약 서빙을 하고 싶은데 체력이 없다면..\n옆에 초록색 +버튼을 눌러봐.",
                "체력을 얻을 수 있는 방법이 나올 지도 몰라!",
                "근데... 서빙이 뭐야?", //baby 20
                "아, 서빙은.. 손님에게 음료나 디저트를\n가져다주는 거야!",
                "아하!", //baby 22
                "이번엔 하트 아래에 있는 별에 관한 거야.",
                "카페 곳곳에 별들이 나타날 때가 있는데,\n그 별들을 모으면 새로운 메뉴를 오픈할 수 있어.",
                "마지막으로 저 위에 엄지를 추켜올린\n손 그림 보이지?",//25
                "저건 우리 카페의 평판이야.",
                "우리가 열심히 일할 수록 평판은 올라갈 거야.",
                "평판이 올라가면 새로운 손님이\n방문할 가능성도 높아지지.",
                "오오... 잘은 모르겠지만 열심히 해볼게!", // baby 29
                "좋아! 그럼 일단 여기까지만 하고\n나머지는 나중에 또 알려줄게.",
                "참, 우리 카페 이름은 코스모스야.\n잊어버리지 말라구!",
                "자, 이제 카페를 오픈해볼까?",
                };
                break;

            case 1://서빙 튜토리얼
                messegeArray = new string[] {
                    "서빙은 어떻게 하는 건지 알려줄게!",
                    "먼저 손님을 터치하면 손님이 원하는 메뉴에 대한 힌트를 얻을 수 있어.",
                    "손님을 터치해보자!",
                    "이 손님은 오렌지가 들어간 음료를\n마시고 싶어하는 거 같은데?",
                    "말풍선을 터치하면 어떤 것을 서빙할 지\n고를 수 있도록 메뉴판이 등장해.",
                    "말풍선을 터치해보자!",
                    "그리고 메뉴판에서 서빙할 메뉴를 골라서 터치하면..",//6
                    "짠~ 서빙 완료!",
                    "어때, 간단하지?",
                    "손님이 원하는 메뉴를 맞추면\n그만큼 카페의 평판도 더 올라가게 돼.",
                    "따라서 손님이 원하는 메뉴가 무엇인지\n파악하는 것도 중요한 일 중에 하나지.",
                    "너라면 잘 할 수 있을 거야.",
                    "자, 다시 일해볼까?",
                };
                break;
            case 2://엔딩 이벤트
                messegeArray = new string[] {
                   "모두 네 덕분이야.",
                   "네가 손님들에게 잘 대해줘서\n단골손님도 많아졌고 말이야.",
                   "너는 이 카페에 있는 동안 어땠니?",
                   "이 곳이 네게 편안하고 따뜻한 기분을\n느끼게 해주는 장소였길 바라.",
                   "밖에서는 때때로 힘든 일도 겪기 마련이지.",
                   "하지만 이 카페에서만큼은\n네가 행복하기만 했으면 좋겠어.",
                   "너의 행복이 곧 나의 행복이나\n마찬가지니까 말이야.",
                   "아무튼! 오늘 카페 영업은 여기까지야!",
                   "혹시라도 이 카페가 그리워지면..",
                   "언제든지 와도 좋아!",
                   "언제나 널 기다리고 있을게.",
                   "그럼 또 만나자!",
                };
                break;
        }

    }

    void BearStartDialogue() //도리 대사 첫 문장
    {
        switch (CharacterDC[1])
        {          
            case 0: //카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "어라? 여기는 뭐하는 곳인가요?");
                CName.text = "??";
                break;
            case 1: // 친밀도 10
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "흐음...");
                CName.text = "도리";
                break;

        }
    }

    void BearNextDialogue() // 도리 첫 문장 제외한 대사
    {
        switch (CharacterDC[1])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "앗, 어서와!\n여기는 코스모스 카페야! ",
                    "카페..??",
                    "그게 뭔가요? 재밌는 건가요?",
                    "우리 주인님이랑 노는 것보다 재밌는 건 없는데..",
                    "음, 그게..\n카페는.. 맛있는 걸 먹을 수 있어!",
                    "맛있는 거요? 저도 먹을 수 있어요?",
                    "당연하지!\n코스모스 카페에 온 걸 환영해!",
                    "감사해요. 앗, 그러고보니\n이름도 말하지 않았네요.",
                    "제 이름은 도리예요.",
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "도리야~",
                    "으으음...",
                    "도리야!",
                    "앗, 네?",//3
                    "무슨 고민 있어? 엄청 심각해보여..",
                    "아.. 그게..",
                    "사실 저희 주인님이 곧 결혼하시거든요.",//6
                    "그래서 주인님께 선물을 드리고 싶은데\n뭘 드려야 할지 모르겠어요..",
                    "사람들은 엄청 커다란 기계 같은 걸\n선물로 주기도 하더라고요.",
                    "안타깝게도 저는 그런 걸 해드릴 능력은 안돼서요..",
                    "음~ 나는 초콜릿이나 사탕을\n선물로 받으면 좋던데!", // 10
                    "먹는 거는 제가 만들기도 어렵고\n어디서 훔쳐올 수도 없어서..",
                    "으음~ 그러면...\n색연필이나 크레파스는?",
                    "그것도 제 능력으론 구하기\n어려울 것 같아요..",
                    "으으으음~ 또.. 또 뭐가 있을까..",
                    "...아! 꽃은 어때?\n예쁜 꽃을 보면 기분도 좋아지잖아!",
                    "꽃도 바깥에 있는 거라...", //16
                    "앗! 여기에 있는 꽃이라면.. \n가져갈 수 있을 지도 몰라요!",
                    "헉, 우리 카페에 있는 꽃을 가져가려구?!",
                    "아뇨, 카페 바깥에 있는 꽃이요!",//19
                    "마법을 쓰면 한 송이 정도는 가능할지도 몰라요!",
                    "마법? 도리 너 마법도 쓸 줄 알아?!", //21
                    "우와, 나도 알려줘! 나도 마법 쓸래!",
                    "저희 인형들이나 장난감들은\n모두 마법을 쓸 수 있어요.",
                    "물론 정말 필요할 때만 쓰지만요.",
                    "오늘 같이 선물 고민도 해주시고\n정말 감사했어요.",//25
                    "그럼 전 얼른 주인님 맘에 쏙 들만한\n예쁜 꽃을 찾으러 가볼게요!",
                    "아앗, 그럼 잠시만 기다려봐!",//27
                    "네?",
                    "...자, 이거!",
                    "이게.. 뭐죠?",//30
                    "도리 너 카페와서 아무것도 안 먹었잖아~\n꽃 찾을 때 배고플까 봐!",
                    "..감사해요. " + babyName + "님은 정말 따뜻한 분이에요.", //32
                    "헤헤, 그러면 마법 어떻게 쓰는 지만\n알려주면 안 돼?", 
                    "후훗, " + babyName + "님은 이미 쓰고 계신 걸요.",
                    "..엥? 내가?",
                    "그럼 이만.. 사탕 잘 먹을게요.\n나중에 또 봬요.",
                };
                break;

        }
    }

    void CarFirstDialogue() //붕붕 첫 문장
    {
        switch(CharacterDC[2])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "어? 여기에 이런 곳이 있었나? 드릉드릉");
                CName.text = "??";
                break;
            case 1: //친밀도 10
                CharacterManager.instance.CharacterFaceList[2].face[0].SetActive(true);
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "드릉!!! 드르응!!!! 드릉드릉드르릉!!!!!!!");
                CName.text = "붕붕";
                break;
        }      
    }

    void CarNextDialogue() //붕붕 첫 문장 제외한 대사
    {
        switch (CharacterDC[2])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "응? 이게 무슨 소리지..?", //baby 
                    "우와아!! 완전 멋진 자동차잖아?!", //baby
                    "흠흠.. 보는 눈이 있군 그래.",
                    "이 몸으로 말할 것 같으면 세계 자동차 레이싱 대회에서 1등을 한 몸이라구. 드릉드릉",
                    "우와, 대단하다....", //baby
                    "에헴! 그렇다고 할 수 있지.",
                    "아무튼 평소처럼 멋지게 이 주변을 달리고 있었는데 못 보던 곳이 보여서 말이야. 드릉드릉",
                    "여기는 코스모스 카페야.\n들어오면 맛있는 걸 먹을 수 있어!", //baby 
                    "맛있는 거? 흐음, 엄청난 기름이라도\n있는 건가.. 드릉드릉",
                    "좋아! 그게 뭐든 도전해 볼 가치는 있겠지!",
                    "내 이름은 붕붕이다. 잘 부탁하지. 드릉드릉",
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "으아, 깜짝이야!! 귀 떨어질 뻔 했네.. 붕붕이니?",
                    "그래, 나다. 드릉드릉",
                    "혹시 뭐 화난 거 있어..?",
                    "그래, 있어!\n아주 아주 아주 화나는 일이 말이야!",
                    "얼마 전에 새로운 신입 자동차가 들어왔어.\n난 별로 신경을 안 썼지.",
                    "왜냐고?\n어차피 내가 제일 빠른 자동차니까.",
                    "그래서 나는 평소처럼 주인과 같이\n레이싱 대회를 즐기고 있었어.",
                    "그.런.데!",
                    "그..그런데?", // 8
                    "그 신입이 1등을 해버린 거야.\n날 제치고!",
                    "주인도 그 신입 자동차를 더 좋아하는 것만 같고..",
                    "이게 바로 굴러들어온 돌이\n박힌 돌을 빼낸다는 건가..?",
                    "그래서 오늘 기분이 아주.. \n화가 나면서도 씁쓸해.",
                    "왜 그런 말 있잖아.\n1등만 기억하는 더러운 세상!!",//13
                    "이제 난 1등이 아니니..",
                    "주인에게도, 다른 장난감들에게도\n잊혀지게 되는 건가..",
                    "이렇게 슬픈 표정의 붕붕이라니.. ", //16
                    "붕붕아, 우리 처음 만났을 때 기억나?",
                    "그 때의 넌 엄청 위풍당당하고\n멋있는 애였어.",
                    "그리고 내가 본 자동차 중에서\n세상 최고로 멋진 자동차였고!",
                    "맞아..\n난 모든 자동차 장난감들의 우상이었어..", //20
                    "그래, 그게 바로 너야!",
                    "내가 아는 붕붕이라면\n절대 2등이라는 결과에 무너지지 않고",
                    "어떻게든 1등을 다시 되찾으려고\n노력할 친구라구.",
                    "그리고 반드시 1등을 하게 될 거라고 난 확신해.",
                    "그래.. 맞아! 이제 알았다.\n지금 이러고 있을 때가 아니라는 걸!", //25
                    "좋아, 그러면 내가 최상의 컨디션이 될 수 있도록\n오늘은 강력한 걸로 부탁한다! 드릉드릉",
                    "알았어! 기다려봐~!",
                    "...",
                    ".....",
                    "자! 너만을 위한 강력한 음료야!",//30
                    "오오!! 크으~~ 진한 기름향이 마음에 드는군!",
                    "그럼 난 이제 하드한 트레이닝을 하러 가보겠어.\n고맙다, 친구! 드릉드릉!",
                };
                break;
        }
    }

    void BreadFirstDialogue() //빵빵 첫 문장
    {
        switch (CharacterDC[3])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "오잉? 모야모야? 여기 모야?");
                CName.text = "??";
                break;
            case 1: //친밀도 10
                CharacterManager.instance.CharacterFaceList[3].face[0].SetActive(true);
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "모야모야, 진짜 짜증나!!");
                CName.text = "빵빵";
                break;
        }
    }

    void BreadNextDialogue() //뻥빵 첫 문장 제외한 대사
    {
        switch (CharacterDC[3])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "앗, 새로운 손님이다! 어서와!",
                    "귀여워!! (빵긋)",
                    "에..?",
                    "너도 귀엽고, 이 아기자기한 공간도\n너무 귀엽다아~! (빵긋)",
                    "아 참, 내 정신 좀 봐.",
                    "내 이름은 빵빵이야.\n근데 여기 모하는 데야? (빵긋)",
                    "여긴 코스모스 카페야.\n맛있는 걸 먹을 수 있지! 너도 먹을래?", //6
                    "모야모야, 나도 먹을 수 있어?\n그럼 귀여운 걸로 부탁할게~! (빵긋)",
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "빵빵아, 무슨 일이야?",
                    "아니, 내 얘기 좀 들어봐.\n우리 주인이 이번에 고등학교라는 곳을 갔거든?",
                    "근데 내가 아닌 다른 필통을 들고 간거야!!",
                    "이게 말이 돼?!\n아니야, 이건 말도 안돼!!",
                    "하하.. 빵빵아, 일단 진정하구..", //4
                    "후.. 알았어.",
                    "우리 주인의 첫 필통은 나였어.\n초등학교에서부터 중학교라는 곳까지 쭈욱.",
                    "나의 귀여운 생김새 덕분에\n난 어딜가든 인기쟁이였어.",
                    "엄청나게 많은 사람들이 날 보고 귀엽다고 해줬지.",
                    "난 내가 주인의 처음이자\n마지막 필통이 될 거라고 생각했어.",
                    "왜냐구? 난 너무 귀여우니까!",
                    "그래서 그 고등학교라는 곳에도\n당연히 나를 데려갈 줄 알았는데..",
                    "아무 무늬도 없고 고작 알파벳 몇 개 써진 게 전부인\n그런 재미없는 필통을 들고 간 거야!!",//12
                    "게다가 전혀 귀엽지도 않다구!",
                    "나는 주인의 가방이 아닌 책상에 남겨지고..\n이건 말도 안돼!",
                    "음.. 빵빵아. 너는 그 필통이 미운 거니,\n아니면 너를 데려가지 않은 주인이 미운 거니?", //15
                    "아니야! 나.. 난 그 필통을 미워하진 않아..",
                    "그리고 난 우리 주인이 좋아!\n절대 미워하는 일따윈 없다구..",
                    "난 그저.. 우리 주인이 나로 인해서\n계속 기뻐하고 행복했으면 좋겠어.",
                    "주변 친구들이 나를 귀엽다고 말해줄 때마다\n우리 주인은 엄청 밝고 환하게 웃어줬단 말이야..",
                    "난 우리 주인이 웃는 게 좋아.\n그 아이가 웃으면 나까지 행복해지거든. (빵긋)",
                    "빵빵이 넌 주인을 정말 좋아하는구나?", //21
                    "그러엄~ 우리 주인이 최고지!\n그 아인 당당하고, 멋지고, 그리고 귀여워! (빵긋)",
                    "음, 내가 봤을 때는 빵빵이 네가\n그렇게 걱정할 필요가 없을 것 같아.",
                    "네 주인에게 있어서 너는 오랜 시간 함께한\n절친이자 추억이 가득한 필통이니까.",
                    "굳이 고등학교에 따라가지 않아도 네가 있는\n그 책상에서 주인을 바라봐주기만 한다면",
                    "주인에게 힘이 될 거라고 생각해.",
                    "왜냐면~ 넌 귀여우니까!",
                    "맞아! 난 귀엽지.\n그리고.. 우리 주인도 날 좋아하고.", //28,
                    "그러면 난 앞으로 주인의 책상에서\n최대한 귀엽게 주인을 바라봐 줄 거야! (빵긋)",
                    "좋은 생각이야!",
                    "그래서 말인데! 오늘은 뭔가 평소보다\n조금 더 귀여운 그런 디저트 없을까?",//31
                    "으음.. 그러면 조금만 기다려봐!",
                    "알았어!",
                    "...",
                    ".....",
                    "짜잔! 어때, 마음에 들어?",//36
                    "모야모야아!! 너어어어무 귀여워!\n마치 나처럼!",
                    "맛도 달콤하고!\n정말 고마워, " + babyName +"!",
                    "좋아, 그럼 어떻게 더 귀여워질지\n생각하러 가야겠어.",//39
                    "내 얘기 들어줘서 고마워. 또 보자! (빵긋)",
                };
                break;
        }
    }

    void RabbitFirstDialogue() //개나리 첫 문장
    {
        switch (CharacterDC[4])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "꺄르르~ 안녕하세요~");
                CName.text = "???";
                break;
            case 1: //친밀도 10
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "있잖아요~ 원래 나는요..");
                CName.text = "개나리";
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.smallFadeIn.Enqueue(16);
                    SmallFade.instance.FadeIn();
                }
                break;
        }
    }

    void RabbitNextDialogue() //개나리 첫 문장 제외한 대사
    {
        switch (CharacterDC[4])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "와아~ 안녕! 처음 보는 친구네?",
                    "저는 1학년 1반 11번 양말토끼 개나리입니다아~!",
                    "우와, 꽃 이름이네? 예쁘다아~",
                    "고마워요! 지그음~ 우리 할아부지가\n코오 주무시구 계셔서 놀러 왔어요~!",
                    "울 할아부지한테 허락두 안 받고 나왔는데..\n혼나면 어떡하지?",
                    "근데 울 할부지는 엄~~청 상냥해서\n안 혼내실 거 같아요. 그죠!?",
                    "음~ 조금만 놀다가는 건 괜찮을 거야!", //6
                    "근데 양말 토끼라구 했지?\n진짜 양말로 만들어진 거야?",
                    "응, 응! 나, 양말루 만들어졌어요!\n이쁜 노란색 양말..",
                    "이거 이거 목폴라두 울 할부지가 해줬어요~\n예쁘죠! 꺄르르~",
                    "그리고~ 여기 맛있는 거 있다고 들었어요!\n나두 먹구 싶어요~!",
                    "언제든지 환영이야!\n그럼 이제 앉아서 기다려줄래?"
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "으음...",
                    "..울 할아부지한테는 귀여운 손녀딸이 있대요!",
                    "근데요, 그 애가 생일이었는데요~",
                    "울 할부지가 그 애 생일선물로\n나를 만들어주셨어요~!",
                    "양말루 말이에요!",
                    "근데요, 으음..\n그 애는 내 색깔보단 초록색을 더 좋아해서요.",
                    "나는 그 애 집에 안 가구\n울 할아부지랑 평생 살 수 있게 됐어요!",
                    "그랬구나.. 근데 그 애와\n놀지 못해서 슬프지는 않아?", //7
                    "하나두 안 슬퍼요!\n울 할부지가 맨날 나 이뻐해줘요~",
                    "나두 울 할아부지가 제~~~~일 좋아!\n꺄르르~",
                    "나리는 할아버지를 정말 좋아하는구나?",
                    "흐음.. 잠시만 기다려봐!",//11
                    "으응~?",
                    "짜잔! 오늘은 특별한 디저트를 준비해봤어~",
                    "우와앙~~! 너무 고마워요!", //14
                    "게다가 진짜진짜루 맛있어요~~!",
                    "울 할부지 다음으로 이 카페가 제~~~일 좋아요!\n" + babyName + "님, 고마워요!",
                };
                break;
        }
    }

    void DdorongFirstDialogue() //또롱이 첫 문장
    {
        switch (CharacterDC[5])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "안냐!!");
                CName.text = "??";
                break;
            case 1:
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "이거!! ..콩!!");
                CName.text = "또롱";
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.SetBabySeat(SmallFade.instance.CharacterSeat[4]);//또롱이 건너편으로  자리 잡기
                    SmallFade.instance.smallFadeIn.Enqueue(17);
                    SmallFade.instance.FadeIn();
                }
                break;
        }
    }

    void DdorongNextDialogue() //또롱이 첫 문장 제외한 대사
    {
        switch (CharacterDC[5])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "앗, 새로운 친구네? 어서와~",
                    "여기 마씻는 거! 이써!",
                    "아아, 맛있는 거?\n맞아, 잘 찾아왔어!",
                    "아싸!",
                    "넌 이름이 뭐니?",
                    "또론!",
                    "또론?",
                    "우움, 론이 아니구~ 또..로옹..!",
                    "아하, 또롱이구나!\n귀여운 이름이네~", //8
                    "자, 그럼 앉아있을래?\n맛있는 거 가져다 줄게.",
                    "아라써!",
                };
                break;
            case 1:
                messegeArray = new string[]
                {
                    "엥? 블루베리를 말하는 거야?",
                    "에!! 이거 콩!!",
                    "또롱아, 이건 콩이 아니라\n블루베리라고 하는 거야~",
                    "콩이 아니야?",//3
                    "브루배리?",
                    "응, 맞아! 이건 블루베리라는 거야~",//5
                    "브루배리... 콩이 아니야..",
                    "우리 주이니 콩 시러해.",
                    "맨날 엄마가 콩 머그라그래써.",
                    "근데 난 콩 조아.",
                    "동글동글 재미써!",
                    "브루배리... 콩 아니지만 조아!",
                    "헤헤, 나도 블루베리 좋아!",//12
                    "담에 또 머거도 대??",
                    "그러엄~ 다음에 또 먹고 싶으면 말만 해!",
                    "아싸! 콩.. 아니, 브루배리 또 머거야지!",
                };
                break;
        }
    }

    void PrincessFirstDialogue() //도로시 첫 문장
    {
        switch (CharacterDC[6])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "정말이지 허름한 곳이로군. 큼큼.");
                CName.text = "???";
                break;
            case 1: //친밀도 10
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "여봐라, 귀한 분이 오셨는데\n어서 모시지 않고 뭐하느냐~");
                CName.text = "도로시";
                break;
        }
    }

    void PrincessNextDialogue() //도로시 첫 문장 제외한 대사
    {
        switch (CharacterDC[6])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "이런 누추한 곳에 귀한 손님이 납셨는데\n어서 받들지 않고 무얼 하는 게지?",
                    "앗, 미안! 못 봤네.\n코스모스 카페에 어서와!",
                    "행복과 미소.. 그리고 꿈의 냄새가\n나는 것만큼은 괜찮구나.",
                    "가만 보니 인테리어도 그닥 나쁘진 않은 것 같군.",
                    "그치만 나처럼 위대하고 고귀한 몸이\n오래 있을 곳은 못 돼.",
                    "음.. 오늘 처음 온 거 같은데,\n난 " + babyName + ". 넌 이름이 뭐야?",//5
                    "내가 누구냐고?",
                    "세상에, 보면 몰라?\n어딜 봐도 고귀한 자태를 뿜어내고 있지 않느냐?",
                    "내가 바로 도로시 공주다.",
                    "와아.. 공주구나~\n어쩐지 되게 예쁘더라!", //9
                    "흥, 당연한 것을 말해 뭐하느냐.",
                    "슬슬 단 것이 땡기는 구나.\n어서 단 것을 내오도록 하거라.",
                    "아, 알겠어! 조금만 기다려줘~",
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "어서와, 도로시!",
                    "내 오늘은 친히 부탁을 하나 하고 싶은데..",
                    "응? 부탁?",
                    "오늘은 뭔가.. 특별한 것이 먹고 싶구나.",
                    "나를 위해 메뉴판에 없는\n특별한 요리를 해줄 수 있겠느냐?",
                    "으음~ 좋아! 한 번 해볼게!", //5
                    "좋다, 기다리도록 하지.",
                    "...",
                    ".....",
                    ".......",
                    "됐다!", //10
                    "날 이 정도나 기다리게 했으면\n분명 굉장한 요리겠지?",
                    "이, 이건...!",
                    "이건.. 너무나 마음에 쏙 드는구나.",
                    "외관도 아름다울 뿐더러\n맛도 이리 훌륭할 수가..",//14
                    "궁전에서도 매일 이런 것을 먹을 수 있다면\n얼마나 좋을까..",
                    "너, 내 궁전으로 와서\n나만의 요리사가 될 생각은 없느냐?",
                    "아하하... 그건 좀 힘들 것 같은데..",//17
                    "그래..? 아쉽군..",
                    "아무튼, 오랜만에 미식을 하니 기분이 좋군 그래.",
                    "고맙구나.",
                };
                break;
        }
    }

    void FamilySeriesFirstDialogue() //루루 첫 문장
    {
        switch (CharacterDC[7])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "안녕하세요..!");
                CName.text = "??";
                break;
            case 1: //친밀도 15
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "오늘도 코스모스 카페는 참 따뜻하네요.");
                CName.text = "루루";
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.smallFadeIn.Enqueue(17);
                    SmallFade.instance.FadeIn();
                }
                break;
        }
    }

    void FamilySeriesNextDialogue() //샌디 첫 문장 제외한 대사
    {
        switch (CharacterDC[7])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "어서와!",
                    "여기서 맛있는 음료수와 디저트를 판다는\n이야기를 들었는데, 저도 먹을 수 있을까요?",
                    "물론이지! 잘 찾아왔어!",
                    "다행이다! 설마 잘못 찾아온 거면\n어쩌지 고민했는데!",
                    "잘 찾아왔으니까 더 이상 걱정하지 않아도 돼!\n난 " + babyName + "인데, 네 이름은 뭐니?",//4
                    "저는 루루예요! 제 친구들이 이 곳의 음료수와 디저트에 대해 관심이 많아요.",
                    "헉, 정말?",
                    "네, 관심이 엄청 많아요!\n모두들 티타임을 굉장히 좋아하니까요!", //7
                    "항상 티타임을 가질 때면 그런 이야기를 해요.",
                    "이 곳의 음료수와 디저트가 최고라는 소문을 들어서\n모두가 항상 궁금하다고 말하거든요!",
                    "좋아, 뭐든 말만 해! 실망하지 않을 거야!", //10
                    "친구들에게 좋은 이야기를 할 수 있도록\n최선을 다해볼게!",
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "그렇게 말해줘서 고마워~",
                    "제 친구들도 코스모스 카페에 올 수 있다면\n참 좋을텐데...",
                    "친구들은 여기 못 와?",
                    "네, 아마도요..",//3
                    "어째선지 이 곳에 올 수 있는 건 저 뿐이라\n제가 친구들을 위해 이 곳을 들리고 있어요.",
                    "그렇다면 루루가 친구들 사이에서\n큰 역할을 맡고 있는 거네?",
                    "맞아요. 하지만 친구들이 직접\n오지 못하는게 정말 슬퍼요.",
                    "음.. 그렇다면 오늘은 루루를 위해서\n특별한 메뉴를 준비해줄게!",
                    "정말요? 저를 위해서?",
                    "응! 대신에 루루의 친구들에 관한\n이야기가 필요해.", //9
                    "친구들에 대해 얘기해줄 수 있니?",
                    "당연하죠! 얼마든지요!",//11
                    "그렇다면... 에헴!",
                    "저는 항상 친구들과 티타임을 가져요.\n친구들이 티타임을 좋아하거든요.",
                    "티타임을 가장 좋아하고\n항상 티타임을 먼저 준비하는 친구는 바리에요.",
                    "바리는 커다란 귀를 가지고 있는 박쥐인데,\n친구들을 가장 사랑하는 친구이기도 해요.",
                    "바리는 원하는 곳으로 갈 수 있는\n멋진 힘을 가지고 있어요.",
                    "그래서 그 곳에서 항상 새로운 친구를 만난댔어요.",
                    "티타임을 할 때, 서로 마주보고 앉아\n바리 친구들에 대해 이야기를 듣곤 해요.",
                    "그 이야기를 들어보면, 바리와 그 친구들이 서로를 많이 아끼고 사랑한다는 걸 알 수 있어요.",
                    "굉장히 정이 많은 친군가 보구나?", // 20
                    "맞아요. 그래서 바리가 들려주는 친구들의\n이야기는 언제나 절 신나게 해요!",
                    "그리고.. 바리와 함께\n티타임을 준비하는 친구가 있어요.",
                    "그 친구의 이름은 루미에요.\n루미는 커다란 여우귀를 가지고 있어요.",
                    "항상 시끌벅적한 바리와는 다르게\n루미는 차분하고 다정해요.",
                    "언제나 다른 친구들이 힘들 때면 다가와 위로해주고,\n맛있는 음료수와 디저트를 만들어주기도 해요.",
                    "그리고 곤란한 일이 생길 때는 루미가 도와줘요.\n루미는 우리의 든든한 해결사에요.",
                    "그래서 루미와 함께 있으면 무엇도 곤란하지 않아요!",
                    "정말 멋있는 친구다!", //28
                    "헤헤, 그리고 저랑 비슷한 친구도\n티타임을 함께 해요.",
                    "그 친구의 이름은 하피인데,\n몸에 커다란 날개가 있어서",
                    "언제든지 원하는 곳으로 날아갈 수 있어요.",
                    "하피는 조용하고.. 신중해요. 그리고 겁이 많아요.\n하피는 바리와 다르게 친구가 많지는 않아요.",
                    "하지만 하피도 자기와 함께하는\n친구들을 사랑하고 있어요.",
                    "그래서 요즘에는 자신의 감정을\n당당히 표현하는 연습을 해요.",
                    "어제도 저와 함께 티타임 테이블을\n정리하면서 이야기를 나누는데,",
                    "제가 하피의 친구여서 기쁘다고,\n사랑한다고 말해줬어요.",
                    ".. 그래서일까요?",
                    "음? 뭐가?",//38
                    "하피가 친구들에게 하루에 한 번씩\n사랑한다고 말해주거든요.",
                    "그래서 모두가 하피를 사랑하는게 아닐까요?",
                    "그러게. 그런 말을 해주는 친구라면\n나도 그 친구를 정말 좋아할 것 같아!",
                    "히히, 그렇죠?",
                    "아, 티타임에 참여하진 않지만,\n밤에 종종 만나는 친구도 있어요.",
                    "별님 달님이 반짝이는 날에\n숲 안 쪽 연못으로 가면 만날 수 있어요.",
                    "이름은 별이래요.\n별님처럼 반짝이는게 딱 어울리는 이름이에요!",
                    "별은 제가 의기소침해있을 때면\n제 어깨를 다독여주며",
                    "\"넌 특별한 아이야.\" 라고 말해줘요.",
                    "그 말을 들을 때면 제가 정말\n특별하다는 믿음이 생겨요.",
                    "그리고 힘이 들 땐 별이 선물해준\n별브로치 리본을 만져요.",
                    "그러면 별이 제 옆에 있다는걸 느낄 수 있거든요.",
                    "저와 제 친구들은 서로 모습도 다르고\n같은 부모님이 있는건 아니에요.",
                    "하지만 제게는 더 없이 소중하고 든든한 지원군이자\n저의 또 다른 가족이랍니다.",
                    "헤헤, 루루의 소중한\n친구들 이야기 잘 들었어!", //53
                    "이제 조금만 기다려줄래?\n얼른 특별한 메뉴를 만들어올게!",
                    "네! 기다릴게요!",
                    "...",
                    ".....",
                    "........",
                    "많이 기다렸지? 미안!",//59
                    "아니에요! 저를 위해\n만들어주시는 것만으로도 감사해요.",
                    "우와아~ 너무 예뻐요..\n그리고 정말로 맛있어요!",
                    "이 카페에 오길\n정말 잘했다는 생각이 들어요.",
                    babyName + "님, 감사해요.",

                };
                break;
        }
    }

    void SunflowerFirstDialogue() //샌디 첫 문장
    {
        switch (CharacterDC[8])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "안녕하세요~\n혹시 여기가 코스모스 카페 맞나요?");
                CName.text = "??";
                break;
            case 1: //친밀도 15
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "오늘은 당신에게 특별한 이야기를 해주고 싶어요.");
                CName.text = "샌디";
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.smallFadeIn.Enqueue(16);
                    SmallFade.instance.FadeIn();
                }
                break;
        }
    }

    void SunflowerNextDialogue() //샌디 첫 문장 제외한 대사
    {
        switch (CharacterDC[8])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "응, 맞아! 여기가 코스모스 카페야.",
                    "제가 잘 찾아왔군요!\n전 샌디라고 해요.",
                    "코스모스 카페에 대한 이야기를 듣고\n한 번쯤 찾아오고 싶었어요!",
                    "코스모스 카페에 대한 이야기?",
                    "네, 장난감들 사이에서 코스모스 카페에\n대한 소문이 자자해요!", //4
                    "카페에서 맛있는 것도 먹고,\n다양한 이야기도 서로 나누면서",
                    "또 다른 즐거움을 느낄 수 있는 곳이라고 말이죠.",
                    "우와! 그게 정말이야?", //7
                    "정말이에요!",
                    "카페 다녀간 인형들은 모두\n이 카페를 좋아하고 있어요!",
                    "좋아, 그렇다면 모두를 위해\n오늘도 열심히 해야겠네!", //10
                    "얼른 앉아, 샌디!\n금방 맛있는 걸 가져다줄게!",
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                   "응? 어떤 이야기인데?",
                   "당신은 여행 다니기를 좋아하나요?",
                   "응! 여행 좋아!",
                   "히히, 저도 여행을 정말 좋아해요.", //3
                   "제가 해드리고 싶은 이야기는\n여행과 관련되어있어요.",
                   "제 주인은 여행 다니기를 정말 좋아해요.",
                   "차를 타고 여행을 갈 때도 있고,\n비행기를 타고 여행을 갈 때도 있어요.",
                   "처음 저를 만난 주인은 저를 손에\n꼬옥 쥐고서 이렇게 말했어요.",
                   "\"너랑 같이 여행을 다니면 분명 즐겁겠다!\"",
                   "그 때의 저는 여행을 한 번도 다녀본 적이 없었어요.",
                   "그래서 여행이 재밌을까?\n라는 생각이 들었죠.",
                   "당신도 그런 생각 해본 적 있나요?\n여행이 재밌을까, 하는 생각이요!",
                   "음, 글쎄에.. 안 해본 것 같은데?", //12
                   "후후, 그런가요?",
                   "지금부터 제가 보고 느꼈던 것들을\n모두 이야기해줄게요.",
                   "그러니까, 주인을 만나서\n처음으로 여행을 갔을 때에요.",
                   "첫 여행을 떠났을 때\n우리는 차를 타고 비행기를 탔었답니다.",
                   "주인은 제게 '밤이니 잠을 자겠다'며\n작게 속삭였고",
                   "저는 주인에게 잘자라는 인사를 해주고 싶었죠.",
                   "곤히 잠든지 얼마 지나지 않아,\n주인은 저를 두 손으로 꼬옥 쥐었어요.",
                   "그리고 조심스럽게 비행기의 창문 쪽으로\n저를 옮겨주었어요.",
                   "저는 조용히 창 밖의 풍경을 보았답니다.",
                   "비행기의 창문으로 본 그 풍경은\n정말로 아름다웠어요.",
                   "서서히 떠오르는 태양의 위를\n살폿 가리는 푹신한 구름,",
                   "푹신한 구름의 사이 사이로 뻗어나오는\n따사로운 아침햇살,",
                   "새하얀 구름의 색과 다홍빛의 햇살이 만나\n섞인 태양의 빛은..",
                   "새로운 하루를 맞이하는\n분홍빛 꽃의 꽃망울처럼 포근했어요.",
                   "색색깔로 옅게 퍼지는 빛과 구름은\n정말로 사랑스러웠어요.",
                   "주인은 그 때 제게 말했어요.\n\"너에게도 이걸 보여주고 싶었어!\" 라고 말이에요.",
                   "저는 그 때 처음 알았어요.",
                   "여행을 다니면 이런 아름다운 풍경을\n자주 본다는 걸!",
                   "그 때부터 즐거운 마음으로 여행을 다녔어요.",
                   "산에도 가보고, 들판 위로 가보고,\n낯선 도시로도 가봤어요.",
                   "낮에는 이 곳 저 곳을 돌아다니며 추억을 쌓고\n밤에는 화려하게 빛나는 네온사인을 바라보고",
                   "아, 어쩌면 빛나는 별과 달을 보기도 해요!",
                   "네온사인과 멀어진 곳은\n별과 달이 더 잘 보이니까요!",
                   "그 중에서 제가 가장 좋아하는 곳은 바다에요.\n저는 주인 덕분에 처음 바다를 봤어요.",
                   "저는 처음 보았던 그 바다를 잊을 수 없어요.\n비록 저는 물에 들어갈 수 없었지만..",
                   "주인이 저를 돗자리에 두고 간 덕에\n바다를 원없이 구경할 수 있었어요.",
                   "시원하게 흔들리는 바람,\n커다랗게 밀려와 부서지는 파도,",
                   "그 곳은 제가 좋아하는 태양이 더욱 강렬해요.",
                   "이 모든 것들이 조화로웠어요.\n그 풍경이 얼마나 아름답던지!",
                   "저는 그 자리에서 계속 바다를 바라봤어요.\n며칠동안 바라볼 수 있을 것 같았어요!",
                   "저는 여름이 정말 좋아요.\n얼른 주인과 또 바다를 보러 가고싶어요!",
                   "와아.. 마치 바다가 내 앞에\n펼쳐져 있는 것만 같은 걸?",//44
                   "정말 생생하고 즐거운 이야기였어.\n고마워, 샌디!",
                   "너를 위한 메뉴가 떠올랐어!\n금방 올게!",
                   "앗, 네!",
                   "...",
                   "자! 빨리 왔지?",//49
                   "네, 정말 빨리 오셨네요!",
                   "....",
                   "왜? 맛이 없어..?",//52
                   "죄송해요, 너무 맛있어서 그만..",
                   "제가 보았던 그 바다를\n다시 마주하는 기분이랄까요..?",
                   "뭐랄까, 정말 여행이라는 즐거움을\n맛으로 표현한다면 이런 맛이 아닐까 싶어요.",
                   "제게 또 하나의 추억을 만들어주셨네요.\n감사해요.",
                };
                break;
        }
    }

    void DogFirstDialogue() //친구 첫 문장
    {
        switch (CharacterDC[9])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "여기가 그 카페인가? 왕!");
                CName.text = "??";
                break;
            case 1: //친밀도 10, 서빙 후
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "이걸 뽀삐도 같이 먹을 수 있다면 좋을 텐데..");
                CName.text = "친구";
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.smallFadeIn.Enqueue(17);
                    SmallFade.instance.FadeIn();
                }
                break;
        }
    }

    void DogNextDialogue() //친구 첫 문장 제외한 대사
    {
        switch (CharacterDC[9])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "아이, 잘했어!\n잘 찾아온 게 맞네. 왕!",
                    "응? 우와, 스스로 머리 쓰다듬는\n강아지다~ 귀여워~",
                    "귀여운 강아지야~ 넌 누구니?",
                    "나? 내 이름은 '친구'라고 해.",//3
                    "나는 보다시피 필통이야.\n동물 모양 필통.",
                    "하지만 내 안에 연필은 들어있지 않아.",
                    "그러면 뭐가 있는데에?",
                    "뭐가 있냐구? 으음.. 놀라지 마.",
                    "내 안에는 약이 들어있어!\n내가 아픈 건 아니구.. 왕!",
                    "헉, 그럼 누가 아픈데?",//9
                    "그건 나중에 시간나면 말해줄게. 왕!",
                    "그보다 나 조금 배고픈데..\n혹시 먹을 거 있을까?",
                    "그럼~! 어서 들어와!", //12
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "응? 뽀삐가 너의 주인이니?",
                    "음... 뽀삐는 진짜 강아지야.",
                    "내 안에 들어있는 게 바로 뽀삐의 약이지.",
                    "우리 뽀삐는 조금 아파서..\n약을 먹어야 하거든.",
                    "뽀삐는 약이 맛이 없는지 잘 먹지를 않아서\n내가 먹는 척을 하면서 안심을 줘.",
                    "이건 맛있는 거야~ 아이 착해~ 하면서!",
                    "착한 뽀삐는 그럴 때마다 약을 먹어준다구~",//6
                    "그거 알아?",
                    "나 덕분에 뽀삐는 약을 잘 먹어서\n점점 예전처럼 건강한 강아지가 되고 있어, 왕!",
                    "정말? 그거 참 다행이네!",//9
                    "최근엔 우리 뽀삐가 주인님과 산책을 갔다가\n흙탕물에 몸을 뒹굴었지 뭐야!",
                    "덕분에 샤워를 했지만 뽀삐가 점점 활발해지는 건\n건강해지고 있다는 뜻이 아닐까?",
                    "아이, 기뻐라~",
                    "내 몸 안에 든 약들이 줄어들고 있다는 것은\n정말 기쁜 일이야. 왕 왕! (꼬리를 흔든다)",
                    "그러게, 나도 뽀삐가 얼른\n건강해지길 바랄게!", //14
                    "그러면~ 오늘은 뽀삐가 빨리 건강해지길\n바라는 마음을 담아 특별한 메뉴를 만들어줄게!",
                    "잠깐만 기다려!",
                    "알았어, 왕!",//17
                    "...",
                    ".....",
                    "짠~! 어서 먹어봐!",
                    "왕! 이거 뭐야!\n내 몸에 있는 거랑 똑같이 생겼다!",
                    "근데 진짜 맛있어, 왕!",
                    "헤헤, 진짜 약은 아니니까 걱정 마.\n맛있다니 다행이다!", //23
                    "정말 고맙다, 왕!",
                    "내 안에 약이 더 이상 채워지지 않는\n그 날까지 힘내보겠다, 왕!",
                };
                break;
        }
    }

    void SoldierFirstDialogue() //찰스 첫 문장
    {
        switch (CharacterDC[10])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "실례합니다! 아무도 없으십니까!");
                CName.text = "??";
                break;
            case 1: //친밀도 10이상, 서빙 후, 카페에 공주 있을 경우
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "저... 그런데 말입니다.");
                CName.text = "찰스";
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.smallFadeIn.Enqueue(16);
                    SmallFade.instance.FadeIn();
                }
                break;
            case 2: //친밀도 15이상, 서빙 전, 공주 친밀도 10이상, 카페에 공주 있을 경우
                CharacterManager.instance.CharacterFaceList[8].face[1].SetActive(true);
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, babyName + "님.. 저 좀 도와주십쇼..");
                CName.text = "찰스";
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.smallFadeIn.Enqueue(16);
                    SmallFade.instance.FadeIn();
                }
                break;
        }
    }

    void SoldierNextDialogue() //찰스 첫 문장 제외한 대사
    {
        switch (CharacterDC[10])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "앗, 여기 사람 있어! 어서와~",
                    "충성! 안녕하십니까!\n저는 찰스라고 합니다.",
                    "휴전 중 잠시 쉬기에 괜찮은 곳이\n보여서 오게 됐습니다.",
                    "그렇구나~ 근데 휴전이 뭐야?",//3
                    "휴전이란 전쟁을 잠깐 멈추는 걸 뜻합니다.",
                    "내일 총사령관님이 깨어나시면\n다시 전쟁이 시작될 겁니다.",
                    "전쟁이라니 힘들겠다..\n그럼 얼른 앉아봐!",//6
                    "잘 쉴 수 있도록\n내가 맛있는 거 가져다줄게~!",
                    "감사합니다! 충성!"
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "응? 왜 그래?",
                    "혹시 저 분은.. 누구십니까?",
                    "저기 계시는.. 어여쁜 분홍색 드레스의\n숙녀분.. 말입니다.",
                    "아 도로시?\n도로시는 공주야!", //3
                    "아앗..! 조, 조용히 말해주십쇼..!",
                    "어쩐지.. 풍기는 분위기가 남다르다 했습니다.",
                    "고상하시고 기품이 있으시며\n아름다우신데 신분도 높으시다니..",
                    "아마도 전 저 분의 눈길조차 받기 힘들겠죠..",
                    "찰스 너.. 도로시 좋아해?",//8
                    "앗!, 아아.. 아닙니..!\n아니, 아닌 게 아니라..!",
                    "으으.. 이만 가보겠습니다!\n안녕히 계십쇼!!"
                };
                break;

            case 2:
                messegeArray = new string[]
                {
                    "뭐야~ 무슨 일인데?",
                    "아무래도.. 도로시 공주님께 제 마음을\n표현하지 않고서는 답답해서 못 살겠습니다..",
                    "보기 좋게 걷어차일지라도 제 마음을 표현하는 편이\n속이 훨씬 후련해질 것 같습니다.",
                    "당연히 가망이 없을 걸 알지만서도\n자꾸 실낱같은 희망을 품게 됩니다.",
                    "한 번 표현하고 시원하게 거절당하고 나면\n마음 정리하는 데에도 도움이 되지 않겠습니까..?",
                    "으음..",//5
                    "정말 한 번만 도와주십쇼..!",
                    "주변에 공주님과 친분이 있는 분이 " + babyName + "님뿐입니다..",
                    "흠, 어려울 거 같긴 하지만..",
                    "알겠어, 도와줄게! 어떻게 하면 되는데?",
                    "일단 제가 공주님께 말을 걸 수 있는\n상황이 필요합니다.",//10
                    "음.. 이건 어떻습니까?",
                    babyName + "님은 일부러 공주님이 원하는\n메뉴가 아닌 다른 메뉴를 서빙하고,",
                    "제가 다가가서 제 메뉴가 실수로\n공주님께 서빙된 것 같다고 말을 거는 겁니다!",
                    "흐음, 그럼 난 도로시한테\n다른 메뉴만 서빙하면 된다는 거지?",
                    "바로 그겁니다!",//15
                    "좋아, 어렵지 않네!",
                    "그럼 지금 간다?",
                    "앗, 저 잠시 마, 마음의 준비를..!",//18
                    "아, 아닙니다. 다녀오십쇼..", //군인 퇴장
                    ".. 흠, 흠!",
                    "주문한 메뉴 나왔어~",
                    ".. 무엇이냐?", //도로시 등장
                    "어라, 이상하다..\n이거 주문하지 않았어?", //23
                    "나는 아직 주문을..", //도로시
                    "(빨리 와, 찰스!)",
                    "제.. 제 메뉴인 것 같습니다!",//26
                    "? 누구..",//도로시
                    "충성! 아, 안녕하십니까!!\n저는 찰스라고 합니다!",
                    "아무래도 제 메뉴가 실수로 공주님께\n서빙된 것 가, 같습니다!",
                    "하하.. 내가 헷갈렸나 봐, 미안!",//30
                    "아아.. 그래.\n그럼 이건 알아서 처리하도록 하고.",//도로시
                    "나는 조금 이따가 주문하도록 하지.",
                    "알겠어~", //아기
                    "저.. 음.. 저기..",//34
                    "그런데 그쪽은 왜 안 가고 있는지?",//도로시
                    "앗.. 저.. 그게..\n이런 말씀 드리기 죄송합니다만..",
                    "사, 사실 공주님을 처음 본 날 그 순간부터..",
                    "제가.. 공주님을..",
                    "좋아하기라도?",
                    "좋..! 예? 아니.. 그.. 맞.. 맞습..", //40
                    "됐다. 이런 일이 한 두 번도 아니라서 말이지.\n대충 표정만 봐도 아느니라.",
                    "흠.. 자네에게 솔깃한 제안을 하나 하지.\n내 경호원을 해볼 생각은 없나?",
                    "예? 경호원.. 말입니까?",//43
                    "그래. 원래 지내는 곳에는 경호원들이 많이 있지만\n여기 올 때는 경호원 없이 나 혼자 오거든.",
                    "내 경호원 일을 잘 수행해준다면 자네를 대한\n나의 생각을 말해줄 수도 있는데.. 어떤가?",
                    "하.. 하겠습니다!!",
                    "제 본업은 군인!\n누군가를 지키는 일이라면 제 전문입니다!",
                    "좋아. 마음에 드는군.\n앞으로 잘 부탁하네, 경호원.",
                    "저야말로 도로시 공주님을 모시게 되어서..\n정말 영광입니다!!", //49
                    "흐음.. ",//아기
                    "(이건 전혀 예상하지 못했는데..)",
                    "(뭐, 잘 된 거겠지?)", //이후 공주 군인 같이 등장
                };
                break;
        }
    }

    void NoNameFirstDialogue() //무명이 첫 문장
    {
        switch (CharacterDC[11])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "....");
                CName.text = "??";
                break;
            case 1: //친밀도 10이상
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "여, 여긴 참 따뜻한 곳이야..\n난 이 곳이 정말 조, 좋아.");
                CName.text = "??";
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.smallFadeIn.Enqueue(17);
                    SmallFade.instance.FadeIn();
                }
                break;
            case 2: //친밀도 20이상
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "저, 저기... 있잖아..");
                CName.text = SystemManager.instance.GetNameForNameless();
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.smallFadeIn.Enqueue(17);
                    SmallFade.instance.FadeIn();
                }
                break;
        }
    }

    void NoNameNextDialogue() //무명이 첫 문장 제외한 대사
    {
        switch (CharacterDC[11])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "음? 뭐지..\n누군가 쳐다보는 것 같은 느낌이...",
                    ".....",
                    "..저 친구인가?\n근데 왜 안 들어오고 저러고 있지?",
                    "음.. 말이라도 걸어볼까.",
                    "안녕? 혹시 들어오고 싶은 거면..",
                    "헉!! 미..미,미안!!\n절대 방해하려던 건 아,아니었어...",//5
                    "아, 그건 괜찮아!\n전혀 방해되지 않았어.",
                    "있잖아, 안으로 들어오지 않을래?",
                    "나, 나?!\n내가 그래도 되,되는 걸까...?",//8
                    "그럼 당연하지!\n코스모스 카페는 어떤 친구든 환영해!",
                    "고, 고마워...",
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "고마워. 나도 여기가 너~무 좋아.",
                    "친구들도 전부 좋고,\n같이 얘기하는 것두 재밌고 말이야.",
                    "음, 근데 있잖아..\n나 너한테 궁금한 게 있어!",
                    "나, 나한테?!",//3
                    "응! 네 이름이 뭔지 궁금해!",
                    "우린 친구인데 한 번도 네 이름을\n들어본 적이 없더라구.",
                    "친구?!\n우리.. 치,친구인 거야?",
                    "응, 당연하지! 넌 내 친구인데?",
                    "친구.. 친구라니.\n나 태어나서 치, 친구 처음 해봐..",//8
                    "그래서 지금.. 너무 기, 기뻐..",
                    "앗, 진짜??\n그럼 내가 첫 친구인 거네?",
                    "이예~!! 내가 첫 친구다!!",
                    "나에게 너의 첫 친구가 될 기회를\n줘서 정말 고마워!",
                    "좋아, 그러면 이름을 알려줄래?\n앞으로는 널 이름으로 부르고 싶거든.",
                    "그, 그게..... 사실....",//14
                    ".. 난 이름이 없어.",
                    "아무도 나에게 이름을 지어주지 않았거든..",
                    "그래서 말인데..\n네가 나에게 이름을 지어주지 않을래?",
                    "앗, 내가..?",
                    "응, 너는 나의 처, 첫 친구니까..\n네가 나에게 이, 이름을 지어 줬으면 해.",
                    "으음... 너의 이름은..",//20
                    SystemManager.instance.GetNameForNameless() + " 어때?",
                    SystemManager.instance.GetNameForNameless() + "...",
                    "어때? 마음에 드니?",
                    "물론이지!\n드디어 나에게도 이,이름이 생겼어..!",//24
                    "행복해.. 고, 고마워.",

                };
                break;

            case 2:
                messegeArray = new string[]
                {
                    "응? 무슨 일이야, " + SystemManager.instance.GetNameForNameless() + "?",
                    "가, 갑작스러울 수도 있겠지만..\n너한테 꼭 하, 하고 싶은 얘기가 있어!",
                    "나.. 너랑 친구를 하면서\n새로운 가, 감정들을 많이 배웠어.",
                    "즐거움, 기쁨, 행복..\n그리고 친구가 있다는 건 이렇게 좋은 것이라는 걸..",
                    "그, 그래서 말인데..",
                    "나, 나한테 친구 만드는 법을 알려주지 않을래?!",
                    "친구.. 만드는 법?",//6
                    "으, 응! 내가 지내는 곳에서도\n친구를 만들고 싶어..!",
                    "나는.. 우리 주인님이 엄마라고 부르는\n사람이 데려온 이, 인형이야.",
                    "하, 하지만 우리 주인님은\n내가 마음에 안 드셨나봐.. ",
                    "다른 장난감 친구들과는 자주 노는데,\n나는 하, 항상 선택받지 못했어.",
                    "언제나 내 자리에 그대로 있었지..",
                    "그, 그래서 난 다른 친구들과 놀아본 적도 없고,\n그러다보니 말도 해본 적이 없어..",
                    "도저히 말을 걸 자신이 없어.. 너무 무서워.\n주, 주인님처럼 걔네도 날 싫어하면 어떡하지?",
                    "나, 난 너무 못생겼고..\n마, 말도 더듬고.. 재미도 없고.. 또.. ",
                    "으음, 그만! 그만해, 이 바보야!",//15
                    "바, 바보? 너도 날 싫어하는 거니..?",
                    "안돼, 그러지마..\n내 유일한 친구를 잃고 싶지 않아..",
                    "아니, 그게 아니야!\n우린 여전히 친구고, 나는 널 무지무지 좋아해.",
                    "내가 바보라고 한 건 네가 얼마나 멋진 애인지\n너 스스로가 몰라서 그래.",
                    "누가 널 싫어한다면\n그 애랑은 굳이 친하게 지내지 않아도 괜찮아.",
                    "걔는 네가 얼마나 멋진 인형인지\n몰라보는 거야.",
                    "꼭 모두와 친구가 될 필요는 없어.",
                    "너의 가치를 알아보는 친구가\n한 명이라도 있다면 그걸로 충분한 거라구.",
                    "만약 너의 가치를 아무도 몰라준다고 해도,\n너무 슬퍼하지 마.",
                    "내가 있잖아!",
                    "넌 충분히 멋지고 빛나는 인형이야.",
                    "그러니 좀 더 당당해져도 돼.\n넌 내 친구니까!",
                    "정말..? 내가 멋지고 빛난다고...? ",//28
                    "응!! 난 거짓말 안 해!",
                    "고,고마워.. 나 이런 말 처음 들어봐.. ",
                    "너는 너 자체로 소중해.\n그리고 넌 더 사랑받을 자격이 있어.",
                    "지금까지 아, 아무도 내게 이런 말을\n해준 적이 없었어..",//32
                    "..어라? 슬프지 않은데 눈물이 나와..",
                    "앗, 울리려던 건 아닌데..!\n자, 잠깐만 기다려!",
                    "...",
                    ".....",
                    SystemManager.instance.GetNameForNameless() + ", 진정하고 이것 좀 봐봐~",//37
                    "이, 이게 뭐야..?",
                    "뭐긴~\n울음도 뚝 그치게 만드는 마법의 디저트지!",//39
                    "자, 봐봐~",
                    "겉으로 보기엔 평범하게 생긴\n화이트초콜릿 볼이지만..",
                    "따뜻한 초콜릿을 부어주면..",
                    "짜잔~ 이렇게 안에 있던\n아이스크림이 나타난다구!",
                    "우, 우와아... 굉장하다..",//44
                    "그치?",
                    "비록 겉모습은 평범하거나\n보잘것없을지 몰라도..",
                    "그 속에는 작지만\n예쁘고 사랑스러운 것이 있지.",//47
                    "우리도 마찬가지야.",
                    "제일 중요한 건 우리 스스로가\n그 사실을 알고",
                    "자신를 아껴주며\n소중하게 생각해야 한다는 거야.",
                    "그렇구나.. 나를 소중하게..",//51
                    "그럼 더 녹기 전에 얼른 먹어봐!",
                    "마, 맛있다..\n지금까지 먹어본 것 중에 제일 맛있는 것 같아..",
                    "다행이다. 울음도 뚝했구!", //54
                    "그리고 내가 친구가 없었던 건..",
                    "어,어쩌면 내가 먼저 다가가지 않았기\n때문일지도 몰라.",
                    "나.. 집에 돌아가서 다른 장난감들에게\n먼저 말을 걸어봐야겠어.",
                    "그리고 또..",
                    "이 카페에 와서 너와 말을 나눈 건\n내가 지금껏 한 일 중에 최고로 잘한 일인 것 같아.",//59
                    "정말 고마워.. 잊지 않을게.",
                };
                break;
        }
    }

    void HeroDinosourFirstDialogue() //영웅공룡 첫 문장
    {
        switch (CharacterDC[12])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "이 곳이야.");
                CName.text = "??";
                break;
            case 1: //친밀도 15
                CharacterManager.instance.CharacterFaceList[12].face[0].SetActive(true);
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "그러고보니 디노 너..");
                CName.text = "히로";
                break;
        }
    }

    void HeroDinosourNextDialogue() //영웅공룡 첫 문장 제외한 대사
    {
        switch (CharacterDC[12])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "멍청한 녀석!\n갑자기 나를 이런 곳에 데려오다니 너무 수상하군.", //디노
                    "설마 함정은 아니겠지?",
                    "무슨 섭한 소릴!\n난 너와 화해를 하고 싶어서 여기로 온 거야.", //히로
                    "뭐.. 뭣이?!",
                    "그, 그럴 수가..!",
                    "(헉, 공룡이랑 눈 마주쳤다!)", //5
                    "네 녀석은 누구냐!",//디노
                    "아.. 안..녕?\n난 " + babyName + "..",
                    "안녕, 꼬마친구?\n내 이름은 캡틴 히로!", //히로
                    "평화를 위해 싸우는 정의의 히어로야!",
                    "만나서 반가워!\n히로라고 부르면 돼.",
                    "와아~ 멋지다!\n나도 반가워, 히로!", //11
                    "그리고 이 녀석은 내 오랜 숙적이자\n라이벌인 디노라고 해.", //히로                  
                    "바.. 반가워~ 디노!",
                    "흥.. 친한 척하지 마!", //디노
                    "(진짜 까칠하다..)",//15
                    "하하, 아무튼 요즘 휴식을 취하고 싶기도 하고\n디노와 이야기를 좀 나누고 싶어서 오게 됐어.",//히로
                    "잘 부탁해, 꼬마친구!",
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "매번 내 주먹에 맞아 날아가면서\n아프지 않았어?",
                    "그걸 말이라고.",//디노
                    "하하, 역시 그랬었군..",//히로
                    "그치만 넌 그러면서 도시를 지킨 거잖아?", //디노
                    "디노..",//히로
                    "어쨌거나 나는 악당이니까\n네녀석한테 언젠가 복수를 할 거라고.", //디노
                    "이런, 그건 좀 곤란한데..",//히로
                    "농담이다, 멍청이.",//디노, 6
                    "나도 나이를 먹어서 그런지\n도시를 부수는 것도 귀찮고",
                    "특히나 네녀석과 싸우는 건\n이제 지긋지긋하다고.",
                    "슬슬 악당노릇도 그만할 생각이야.",
                    "..무엇보다 우리 주인,\n제이크와 놀아본 지도 오래됐고 말이야.",
                    "....",//히로
                    ".. 참, 제이크에게 동생이 생겼다는 소식 들었나?", //디노
                    "네가 그 녀석을 위해\n또다시 나를 무찌르러 올 지 궁금한데.",
                    "나는...",//히로, 14
                    "히어로를 은퇴할 거야.",
                    "나하고 싸워왔던 오랜 친구가..\n방금 은퇴 선언을 했거든.",
                    "그리고 제이크의 동생한테는\n새로운 영웅과 악당이 생기겠지.",
                    "나는 제이크의 장난감으로 남고 싶어.",
                    ".. 신기하군.\n아니, 어쩌면 당연한 건가.",//디노
                    "..?",//히로
                    "나도 제이크의 장난감으로 남고 싶다고 생각했거든.",//디노
                    "저기.. 얘들아!\n이거 한 번 먹어볼래?",//23
                    "뭔가 오늘은 평소랑 다른 분위기에다가..\n둘이 진짜 친구가 된 것 같기도 하고?",
                    "아무튼 너희가 좀 더 친해졌으면\n하는 마음에서 만들어봤어!",
                    "이야~ 이거 아주 맛이 끝내주는데?",//히로
                    ".. 뭐야, 생각보다 맛있잖아.\n쳇, 마음에 드는군.",//디노
                    "와~ 까칠한 디노가 칭찬해줬다~!",//28
                    "따, 딱히 너 들으라고 한 건 아니거든!", //디노
                    "하하! 그리고 디노랑 나는\n오늘부터 진정한 친구가 되기로 했어.", //히로
                    "이봐, 누구 맘대로 친구야?!",//디노
                    "우린 앞으로도 쭉 라이벌이야, 알겠어?",
                    "아니, 그치만..", //히로
                    "잊었나?\n제이크의 장난감으로 남고 싶다며.",//디노, 34
                    "누가 마지막까지 제이크의\n장난감으로 남는지 승부다.",
                    "..그렇군.",//히로
                    "좋아, 그러면 라이벌 겸 친구인 거다!",
                    "아, 아니 그러니까 누구 맘대로..!",//디노
                    "고마워, 꼬마친구!",//히로, 39
                    "디노와 진심이 담긴 이야기를 나눌 수 있었던 건\n다 이 카페가 있었기 때문이야.",
                    "우릴 위해 만들어준 디저트도 정말 고마웠어!",
                    "그럼 다음에 또 보자! 안녕!",
                    "이봐, 난 친구 안 한다니까!!\n너 거기 안 서?!",//디노
                };
                break;
        }
    }

    void PenguinFirstDialogue() //닥터 펭 첫 문장
    {
        switch (CharacterDC[13])
        {
            case 0://카페 첫 방문
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "흠흠.. 혹시 지금 영업하고 있소?");
                CName.text = "???";
                break;
            case 1: //친밀도 15
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.characterText, "반갑네, 오늘도 이 카페는 따뜻하구먼.");
                CName.text = "닥터 펭";
                if (VisitorNote.instance.fmRP == 0 && VisitorNote.instance.evRP == 0)//다시보기가 아닐 때
                {
                    SmallFade.instance.smallFadeIn.Enqueue(16);
                    SmallFade.instance.FadeIn();
                }
                break;
        }
    }

    void PenguinNextDialogue() //닥터 펭 첫 문장 제외한 대사
    {
        switch (CharacterDC[13])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    "아! 어서와!\n편하게 들어오면 돼!",
                    "마침 잘 됐군.\n말을 많이해서 목이 말랐던 참이었으니.",
                    "그래.. 여기서 어떤게 제일 시원하고\n달콤한지 알려주겠소?",
                    "물론이지! 자, 여기 메뉴판이야.\n한 번 봐봐!",//3
                    "우리 카페는 다양한 걸 만들거든.\n분명 마음에 드는게 있을거야.",
                    "좋네, 어디 한 번 보도록 하지.",
                    "그런데, 한가지 물어봐도 될까?",//6
                    "음? 좋네.\n무엇이 궁금한가?",
                    "아까 말을 많이 했다고 했는데..\n혹시 뭘하다 왔는지 알려줄 수 있어?",
                    "아, 그게 궁금했던 모양이로군.",//9
                    "이거 참, 내 소개가 늦었소.\n나는 닥터 펭이요.",
                    "다른 장난감들에게 내가 그동안 보고\n들은 것들을 이야기해주고 오는 길이오.",
                    "하도 오랫동안 보고 듣다보니,\n말해줄게 많더군.",
                    "그리고 원래 내가 이야기하는걸\n좋아하는 편이기도 하고. 허허!",
                    "오, 그럼 나도 그 이야기 들을 수 있을까?\n궁금한데!",//14
                    "허허, 이야기가 그렇게 듣고 싶은게요?",
                    "물론이지! 나 이야기 듣는거 굉장히 좋아해!",
                    "그래서 카페에 온 친구들이랑\n대화도 자주 하거든.",
                    "그게 정말이오?\n이런 평화로운 곳에서 이야기를 나눌 수 있다니.",//18
                    "이건 여지껏 내가 들어본 적 없는 소식이군.\n그럼 나도 그대에게 이야기를 하지 않을 수 없지.",
                    "나중에 시간나면 꼭 이야기를 해줄테니 기다리게나.",
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "어서와, 닥터 펭!\n오늘도 목마를 때까지 이야기하다 온 거야?",
                    "허허, 그렇기도 하지.\n하지만 오늘은 그것이 다가 아니네.",
                    "오늘은 그대에게도 이야기를 해주러 왔네.",
                    "와, 드디어!\n언제쯤 들려줄지 엄청 궁금했었는데!",
                    "허허허. 오래 기다렸구만.\n고생 많았네, 그대.",//4
                    "그렇다면 오늘은 닥터 펭이 들려주는\n이야기를 듣고 특별한 음료를 만들어줄게!",
                    "그게 정말인가?\n이렇게 고마울 수가!",
                    "고맙긴! 닥터 펭이 코스모스 카페를\n꾸준히 와주는 게 더 고맙지!",
                    "허허허, 아무튼 기다리게 해서 미안하다네.\n그렇다면 더 이상 뜸들일 수는 없겠군.",//8
                    "나는 한 커다란 쇼핑몰 진열박스 안에\n진열된 도자기인형이네.",
                    "진열박스에 진열되어 있으니\n그 앞을 지나가는 이들을 관찰하는게 수월하지.",
                    "오늘 해줄 이야기는,\n내가 가장 오래 관찰한 사람의 이야기네.",
                    "그는 내가 있는 진열박스와\n장난감 코너를 도맡은 사람이지.",
                    "처음 나를 보았던 그의 눈은 반짝거렸네.\n마치 내가 빛을 받아 반짝이는 것처럼 말이네.",
                    "어느 마감 시간이었지.\n그는 어김없이 진열박스 앞을 지나가던 중이었네.",
                    "평소 반짝이던 그의 눈은\n불 꺼진 마트보다 캄캄했네.",
                    "무슨 일인가 싶어 그를 응시하던 중에\n어머니께서 계속 일하셨더라면, 이라고 중얼이더군.",
                    "응? 대체 무슨 일이길래...",//17
                    "그는 진열박스에 손을 대고 작게 한숨을 쉬었지.\n그리고 천천히 말하기 시작했네.",
                    "그의 어머니 또한 나같은 도자기인형을\n만드는 사람이라고,",
                    "사고 때문에 더는 도자기인형을\n만들 수 없다고 말이지.",//20
                    "그의 어머니는 자신의 일을\n굉장히 사랑했던 사람이라 했네.",
                    "그런데 더 이상 일을 하지 못한다니.\n그는 굉장히 침울해 했지. 물론 나도 안타까웠네.",
                    "그런데 며칠 후, 그가 다시 날 보러 왔네.",//23
                    "그는 더 이상 침울한 표정을 짓지 않았어.\n오히려 해맑은 표정이었지!",
                    "그는 내게 이렇게 말했네.",
                    "\"어머니를 위해서 도자기인형을 만들거야.\"\n\"은퇴선물로 내가 만든 도자기인형을 드리는 거지!\"",
                    "어찌나 밝게 웃던지, 마치 태양을 보는 것 같았네.",//27
                    "사랑하는 어머니를 위해\n어머니가 사랑하는걸 선물해준다니.",
                    "이 얼마나 가슴 훈훈한 이야기인가? 하하하!",
                    "그렇네, 정말 잘됐다!\n그런 선물이라면 누구라도 감동할 것 같아!",//30
                    "그렇고 말고.\n이 이야기는 내가 가장 좋아하는 이야기네.",
                    "누군가의 행복이 나의 행복이 되니 말일세.\n앞으로 그 두 사람이 행복했으면 좋겠군.",
                    "나중에 또 그 두 사람의 이야기를 듣게 된다면,\n그대에게도 알려주러 오도록 하겠네.",
                    "좋아! 그럼 이제 잠시만 기다려봐!",//34
                    "...",
                    ".....",
                    "짜자잔~",
                    "오, 이것이 나를 위한 특별한 음료?",
                    "맞아! 열심히 얘기해 준 닥터 펭을 위해\n나도 열심히 만들어봤어!",
                    "으흐음, 맛도 기가 막히는 구먼!\n정말 고맙네.",//40
                    "오늘도 이 카페에서 좋은 기억을\n가지고 가는구먼.",
                    "나중에 또 들리겠네.\n그때까지 잘 있게!",
                };
                break;
        }
    }

    void GrandfatherFirstDialogue() //할아버지 첫 문장
    {
        switch (CharacterDC[14])
        {
            case 0://카페 첫 방문
                CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.babyText, "어라? 참새가 왜 바닥에 누워있지?");
                break;
            case 1: //친밀도 20이상
                CharacterManager.instance.CharacterFaceList[13].face[0].SetActive(false);
                CharacterManager.instance.CharacterFaceList[13].face[1].SetActive(true);
                textWriterSingle = TextWriter.AddWriter_Static(UI_Assistant1.instance.babyText, "로랑드 할아버지! 할아버지 몸에\n달린 그 줄들은 뭐에요?");
                CharacterManager.instance.CharacterIn(14);//큰 캐릭터 일러스트는 바로 들어오기
                break;
        }
    }

    void GrandfatherNextDialogue() //할아버지 첫 문장 제외한 대사
    {
        switch (CharacterDC[14])
        {
            case 0://카페 첫 방문
                messegeArray = new string[]
                {
                    ".. 헉! 참새야! 참새야!\n정신차려봐!",
                    "얘야, 무슨 일이니?",
                    "참새가.. 참새가 누워서 움직이질 않아요..",
                    "으음, 아마도 이 아이는..\n오랫동안 물을 못 마신 것 같구나.",
                    "꼬마야, 혹시 물이 있니?",
                    "물이요? 잠시만요!!\n참새 좀 봐주세요!",
                    "최대한 서두르렴.",//6
                    "....",
                    "..여기 물 가져왔어요!!",
                    "자.. 옳지, 옳지.\n너무 급하게 들이키진 말고.",
                    ".. 참새 이제 괜찮은 거에요?",
                    "그래, 다행히 위험한 순간은 넘겼단다.",//11
                    "곧 있으면 기운을 차려서\n다시 날아다닐 수 있을 게야.",
                    "다행이다...",
                    "인사가 늦었구나.\n내 이름은 롤렝드란다.",
                    "도와주셔서 고맙습니다!\n로.. 로랑드 할아버지.",
                    "허허, 내 이름이 어렵나 보구나.\n편한 대로 부르렴.",//16
                    "앗, 참새가 움직여요!",
                    "오, 녀석이 드디어 기운을 차린 모양이군.",
                    "참새야, 잘 가~!\n다신 아프지 마~!",
                    "마치 너에게 고맙다는 인사를 하는 것 같구나.",//20
                    "헤헤, 전 물을 가져다준 것 밖에 없는 걸요.",
                    "로랑드 할아버지가 아니었다면\n참새를 도와주지 못했을 거에요!",
                    "음.. 감사의 의미로 카페에\n초대하고 싶은데.. 와주실래요?",
                    "오, 그럼 물론이지.\n나야말로 초대해줘서 영광이구나.",//24
                };
                break;

            case 1:
                messegeArray = new string[]
                {
                    "아, 이거 말이냐?",
                    "이건 내가 혼자 움직이는 것처럼\n보이게 도와주는 것이란다.",
                    "실제로는 사람이 나를 움직여주는 거지만 말이야.",
                    "너와 같은 어린 아이들의 앞에서\n공연을 하는 게 바로 내 일이거든.",
                    "우와! 저도 할아버지 공연 볼래요!",//4
                    "그래그래, 꼭 보러 오려무나.\n나도 우리 꼬마친구가 와준다면 참 기쁠 것 같거든.",
                    "그러면 로랑드 할아버지를\n움직여주는 사람은 누구예요??",
                    "나의 주인이면서 인형사라고도 불리는 사람이지.",//7
                    "그도 나처럼 나이가 많고,\n새하얀 머리를 가진 누군가의 할아버지란다.",
                    "흠.. 그의 얘기를 좀 더 해도 되겠니?",
                    "네, 좋아요!",//10
                    "그에게는 어린 손자가 있었단다.\n정말 귀여운 아이였어.",
                    "하지만 안타깝게도 그 손자는\n태어날 때부터 몸이 약했단다.",//12
                    "그래서 항상 병실에서만 있어야 했지.",
                    "그는 손자를 매우 사랑했단다.",//14
                    "어떻게 하면 손자를 더 즐겁게\n해줄 수 있을지 오래도록 고민했지.",
                    "그러다 문득, 인형으로 연극을 해주면\n어떨까라는 생각을 하게 되었단다.",
                    "그의 손자는 평생 병원에서만 지냈기 때문에..",
                    "한 번도 영화나 연극 같은 공연을\n보러 간 적이 없었으니까 말이야.",
                    "그렇게 처음 만들어진 게 바로 나란다.\n나는 그와 정말 많이 닮았어.",
                    "외적으로든 내적으로든 말이야.",
                    "..아무튼, 그는 나말고도 여러 다른 인형들을\n만들어서 손자 앞에서 공연을 했단다.",
                    "손자는 인형극을 정말 좋아했어.\n내가 다 뿌듯하더구나.",//22
                    "하지만 그 모습을 본 하늘의 천사들이\n그 아이에게 질투를 느꼈는지..",
                    "며칠 뒤에 그 아이를 먼 곳으로 데려가버렸단다.",
                    "나의 주인은 크나큰 슬픔에 빠졌지.\n그 후로 오랫동안 인형극을 하지 않았어.",
                    ".. 그 아이가 다시 돌아오지는 않았나요?",//26
                    "그 아이는 다시는 돌아올 수 없는 곳으로 갔단다.",
                    ".. 그렇지만 아직 이야기는 끝나지 않았는데,\n더 들어보겠니?",
                    "네.. 들어볼래요.",//29
                    "그가 인형극을 그만두고, 꽤 많은 시간이 지났단다.",
                    "빈 집이었던 옆 집에\n새로운 이웃이 이사를 오게 되었고,",
                    "아주 귀여운 작은 꼬마가 인사를 왔단다.",
                    "그 꼬마는 그의 손자와 많이 닮았단다.",
                    "그래서 그런지, 그는 그 꼬마에게\n상냥하게 대해주지 않았어.",
                    "그 꼬마를 볼 때마다 마음 한켠이 저려왔을 거야.\n내가 그랬듯이 말이야.",//35
                    "하지만 그 꼬마는 하루도 빠짐없이 그를 찾아왔단다.",
                    "어떤 날을 자기가 만들었다며 쿠키를 주기도 했고,\n또 어떤 날은 예쁜 꽃을 주기도 했지.",
                    "그 모습이 어찌나 사랑스럽던지..",//38
                    "그 꼬마를 차갑게 대할 수 있는 사람은\n아마 한 명도 없을 게다.",
                    "그리고 그건 내 주인도 마찬가지였지.\n그는 서서히 마음을 열기 시작했단다.",
                    "나를 데려가서 인형극도 해주었어.\n정말 오랜만에 보는 행복한 얼굴이었단다.",//41
                    "그 꼬마는 인형극이 아주 마음에 들었는지\n온 마을 아이들에게 소문을 내서",
                    "이제 그와 나는 마을 공원에서\n인형극을 한단다.",
                    "헤헤, 저도 그 친구랑 친구가 되고 싶네요.",//44
                    "아마 좋은 친구가 될 수 있을 게다.\n너랑도 비슷한 점이 많거든.",
                    "어이쿠, 슬슬 가봐야 할 시간이네.\n긴 얘기 들어줘서 고맙구나.",
                    "아앗, 잠시만요!\n로랑드 할아버지, 잠깐만 기다리세요!",
                    "으응?\n왜 그러냐?",//48
                    ".. 가버렸구먼.",
                    ".....",
                    "로랑드 할아버지! 이거 드셔보세요!",//51
                    "어엉? 이게 뭐냐?\n난 주문한 적이 없는데..",
                    "제가 그냥 드리는 거에요!",
                    "그래? 그럼 고맙게 받으마.",
                    "오오, 네가 직접 만든 거냐?\n근래에 먹어본 것 중에 제일 맛있구나.",//55
                    "근데 여기 꽂힌 이 조그마한 건 뭐냐?",
                    "헤헤, 그건 제 웃는 얼굴이에요!\n할아버지 가지세요!",//57
                    "그리고 로랑드 할아버지,\n저도 다음에 공연 꼭 보여주셔야 돼요!",
                    "허허, 녀석 참.\n내가 시간을 더 내서라도 공연해주마.",
                    "와아~! 고마워요, 로랑드 할아버지!",//60
                    "그래그래. 그럼 이만 일어나야겠구나.\n오늘도 좋은 시간 보내게 해줘서 고맙다.",
                    "나중에 또 보자구나.",
                };
                break;            
        }
    }
}
