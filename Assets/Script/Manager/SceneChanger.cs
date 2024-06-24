using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour //화면 페이드 인아웃
{
    public static SceneChanger sc;
    public Animator animator;
    private int SceneToLoad;

    void Awake()
    {
        if(sc == null)
        {
            sc = this;
        }

    }

    public void FadeToScene(int sIndex) 
    {
        SceneToLoad = sIndex;
        animator.SetTrigger("FadeOut");
        Invoke("OnFadeComplete", 2.8f);
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(SceneToLoad);//잘자렴아가씬 로드
    }
}

