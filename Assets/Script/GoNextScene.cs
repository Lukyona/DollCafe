using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoNextScene : MonoBehaviour //다음 씬으로 넘어가기
{
    public static GoNextScene instance;
    public Animator animator;
    public Text creditText;//엔딩 크레딧 텍스트

    [SerializeField]
    Image lodingBar;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "GameScene" && Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))//뒤로가기 버튼 두 번으로 앱 종료
            {
                Application.Quit();
            }

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "LogoScene") //현재 씬이 로고 씬이면
        {           
            PlayerPrefs.SetInt("GameOff", 1); //중간 이탈이 아니라 게임을 끄고 들어온 것
            int n = PlayerPrefs.GetInt("end");
            if (n == 2)//엔딩을 보고난 후에 게임을 킨 거면
            {               
                PlayerPrefs.SetInt("end", 3);
            }
            PlayerPrefs.Save();
            Invoke("GoStartScreen", 4f); //스타트씬으로 넘어감
        }

        if (SceneManager.GetActiveScene().name == "GoodNightScene") //현재 씬이 굿나잇씬이면
        {
            lodingBar.fillAmount = 0;
            Invoke("LoadCafeScene", 1.3f);
        }

        if (SceneManager.GetActiveScene().name == "EndingCreditScene") //현재 씬이 영업준비 씬이면
        {
            string name = PlayerPrefs.GetString("NamedName");
            string bName = PlayerPrefs.GetString("BabyName");
            creditText.text = "\n<Art - Background>\n\n김지수\n\nMomo\n\n< Art - Character >\n\nMomo\n\nNuba\n\nVint"
                + "\n\n<Art - Menu>\n\n김지수\n\nMomo\n\nNuba\n\nVint\n\n< Art - UI >\n\nMomo\n\nLukyona\n\n< Character Scenario >\n\nMomo\n\nNuba\n\nLukyona"
                + "\n\n<Programming>\n\nLukyona\n\n\n\n<Friends>\n\n도리\n\n\n붕붕\n\n\n빵빵\n\n\n개나리\n\n\n또롱\n\n\n도로시\n\n\n루루\n\n\n샌디\n\n\n친구\n\n\n찰스\n\n\n"
                + name//무명이
                + "\n\n\n히로\n\n\n디노\n\n\n닥터 펭\n\n\n롤렝드\n\n\n\n제제\n\n\n\n\n<Special Thanks>\n\n"
                + bName + "\n\n\nand You"
                + "\n\n\n\n\n\n\n\n\n지금까지 카페 코스모스를 방문해주셔서 감사합니다.";


            Invoke("GoStartScreen", 75f); //75초 후 스타트씬으로 넘어감
        }

        if (SceneManager.GetActiveScene().name == "PrepareScene") //현재 씬이 영업준비 씬이면
        {
            Invoke("GoStartScreen", 4f); //4초 후 스타트씬으로 넘어감
        }

    }

    public void GoStartScreen()//스타트 씬으로 이동
    {
        SceneChanger.sc.FadeToScene(1);
    }

    public void GoEndingCreditScreen()//엔딩크레딧씬으로 이동
    {
        SceneChanger.sc.FadeToScene(4);
    }

    void LoadCafeScene()
    {
        int n = PlayerPrefs.GetInt("end");
        if (n == 2)//엔딩 이벤트 보고 종료하지 않은 채 다시 들어온 경우
        {
            StartCoroutine(LoadAsyncScene1());
        }
        else
        {
            StartCoroutine(LoadAsyncScene());
        }
    }

    IEnumerator LoadAsyncScene()
    {
        yield return null;
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync("GameScene");

        asyncScene.allowSceneActivation = false;
        float timeC = 0;
        while(!asyncScene.isDone)
        {
            yield return null;
            timeC += Time.deltaTime;
            if(asyncScene.progress >= 0.9f)
            {
                lodingBar.fillAmount = Mathf.Lerp(lodingBar.fillAmount, 1, timeC);
                if(lodingBar.fillAmount == 1.0f)
                {
                    asyncScene.allowSceneActivation = true;
                }
            }
            else
            {
                lodingBar.fillAmount = Mathf.Lerp(lodingBar.fillAmount, asyncScene.progress, timeC);
                if(lodingBar.fillAmount >= asyncScene.progress)
                {
                    timeC = 0;
                }
            }
        }
    }

    IEnumerator LoadAsyncScene1()
    {
        yield return null;
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync("PrepareScene");

        asyncScene.allowSceneActivation = false;
        float timeC = 0;
        while (!asyncScene.isDone)
        {
            yield return null;
            timeC += Time.deltaTime;
            if (asyncScene.progress >= 0.9f)
            {
                lodingBar.fillAmount = Mathf.Lerp(lodingBar.fillAmount, 1, timeC);
                if (lodingBar.fillAmount == 1.0f)
                {
                    asyncScene.allowSceneActivation = true;
                }
            }
            else
            {
                lodingBar.fillAmount = Mathf.Lerp(lodingBar.fillAmount, asyncScene.progress, timeC);
                if (lodingBar.fillAmount >= asyncScene.progress)
                {
                    timeC = 0;
                }
            }
        }
    }
}
