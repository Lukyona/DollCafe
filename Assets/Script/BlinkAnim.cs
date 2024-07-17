using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkAnim : MonoBehaviour //터치투 스타트 메세지를 깜빡거리게
{
    void Start()
    {
        BgmManager.instance.PlayMainBgm(); //시작화면 브금 재생
    }
    public Image image;

    float time;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) //터치하면 다음 씬으로
        {
            SceneChanger.sc.FadeToScene(2);//잘자렴아가씬으로 이동
            BgmManager.instance.BGMFadeOut();

        }

        if (time < 0.5f)
        {
            image.color = new Color(1, 1, 1, 1 - time);
        }
        else
        {
            image.color = new Color(1, 1, 1, time) ;
            if(time > 1f)
            {
                time = 0;
            }
        }

        time += Time.deltaTime;

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))//뒤로가기 버튼 두 번으로 앱 종료
            {
                Application.Quit();
            }

        }
    }

}
