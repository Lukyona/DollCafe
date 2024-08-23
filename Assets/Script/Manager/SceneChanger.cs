using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneChanger : MonoBehaviour //화면 페이드 인아웃
{
    public static SceneChanger instance;
    public Animator animator;

    [SerializeField] //private 직렬화
    Image lodingBar;

    public Text creditText;//엔딩 크레딧 텍스트

    private int sceneToLoad;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

 void Start() // 다른 스크립트들의 Awake가 종료되면 실행
    {
        switch (SceneManager.GetActiveScene().name) // 현재 씬이 무엇인지 파악하기
        {
            case "LogoScene": //로고 씬이라면
                // PlayerPrefs를 이용하면 로컬에 데이터를 저장할 수 있음. (문자열 키값, 데이터)
                if (PlayerPrefs.GetInt("EndingState") == 2) //엔딩을 보고난 후에 게임을 킨 거면
                {               
                    PlayerPrefs.SetInt("EndingState", 3);
                }
                PlayerPrefs.Save();
                Invoke(nameof(GoStartScene), 4f); //스타트씬으로 넘어감
                break;
            case "GoodNightScene": //로딩 씬이라면
                lodingBar.fillAmount = 0;
                Invoke(nameof(LoadCafeScene), 1.3f);
                break;
            case "EndingCreditScene": //엔딩 크레딧 씬이라면
                string nName = PlayerPrefs.GetString("NameForNameless"); //플레이어가 설정한 이름들 가져오기
                string bName = PlayerPrefs.GetString("BabyName");
                { // 엔딩 크레딧 내용
                    creditText.text = "<Art - Background>\n\n김지수\n\nMomo\n\n< Art - Character >\n\nMomo\n\nNuba\n\nVint"
                    + "\n\n<Art - Menu>\n\n김지수\n\nMomo\n\nNuba\n\nVint\n\n< Art - UI >\n\nMomo\n\nLukyona\n\n< Character Scenario >\n\nMomo\n\nNuba\n\nLukyona"
                    + "\n\n<Programming>\n\nLukyona\n\n\n<Friends>\n\n도리\n\n\n붕붕\n\n\n빵빵\n\n\n개나리\n\n\n또롱\n\n\n도로시\n\n\n루루\n\n\n샌디\n\n\n친구\n\n\n찰스\n\n\n"
                    + nName //무명이
                    + "\n\n\n히로\n\n\n디노\n\n\n닥터 펭\n\n\n롤렝드\n\n\n\n제제\n\n\n\n\n<Special Thanks>\n\n"
                    + bName + "\n\n\nand You"
                    + "\n\n\n\n\n\n\n\n\n지금까지 카페 코스모스를 방문해주셔서 감사합니다.";
                }
                Invoke(nameof(GoStartScene), 75f); //75초 후 스타트씬으로 넘어감
                break;
            case "PrepareScene": //영업 준비 씬이라면
                Invoke(nameof(GoStartScene), 4f); //4초 후 스타트씬으로 넘어감
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "GameScene" && Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))//뒤로가기 버튼 두 번이면 바로 앱 종료(인게임씬은 제외)
            {
                Application.Quit();
            }
        }
    }

    public void FadeToScene(int sceneIndex) 
    {
        sceneToLoad = sceneIndex;
        animator.SetTrigger("FadeOut");
        Invoke(nameof(OnFadeComplete), 2.8f);
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad); //씬 로드
    }

     public void GoStartScene()//스타트 씬으로 이동
    {
        FadeToScene(1);
    }

    public void GoEndingCreditScene()//엔딩크레딧씬으로 이동
    {
        FadeToScene(4);
    }

    void LoadCafeScene()
    {
        string sceneName = "";
        if (PlayerPrefs.GetInt("EndingState") == 2)//엔딩 이벤트 보고 종료하지 않은 채 다시 들어온 경우
        {
            sceneName = "PrepareScene";
        }
        else 
        {
            sceneName = "GameScene";
        }
        StartCoroutine(LoadAsyncScene(sceneName));
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        yield return null; // 다음 프레임에 실행, ->업데이트 함수가 실행되었다가 돌아옴
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(sceneName); // LoadSceneAsync : 비동기 방식, 일시 중지가 발생하지 않음, 진행 정도를 반환

        asyncScene.allowSceneActivation = false; //장면이 준비된 즉시 활성화되는 것을 비허용, true는 허용
        float time = 0;
        while(!asyncScene.isDone)
        {
            yield return null; 
            time += Time.deltaTime;
            if(asyncScene.progress >= 0.9f) // 로딩이 거의 다 끝났으면
            {
                lodingBar.fillAmount = Mathf.Lerp(lodingBar.fillAmount, 1, time); // Lerp(시작값, 끝값, 보간값)
                if(lodingBar.fillAmount == 1.0f) // 로딩바가 다 채워지면   
                {
                    asyncScene.allowSceneActivation = true;
                }
            }
            else
            {
                lodingBar.fillAmount = Mathf.Lerp(lodingBar.fillAmount, asyncScene.progress, time); //진행된 정도까지만 보간
            }
        }
    }
}

