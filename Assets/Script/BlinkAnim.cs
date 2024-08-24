﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkAnim : MonoBehaviour //터치투 스타트 메세지를 깜빡거리게
{
    [SerializeField] Image image;

    float time;

    void Start()
    {
        BgmManager.instance.PlayMainBgm(); //시작화면 브금 재생
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //터치되면
        {
            BgmManager.instance.BGMFadeOut();
            SceneChanger.instance.FadeToScene(2);//로딩씬으로 이동
        }

        if (time < 0.5f) // 0.5미만이면 서서히 투명해지기 
        {
            image.color = new Color(1, 1, 1, 1 - time);
        }
        else 
        {
            image.color = new Color(1, 1, 1, time); // 서서히 선명해지기
            if(time > 1f)
            {
                time = 0; // 초기화
            }
        }

        time += Time.deltaTime;
    }
}
