﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEManager : MonoBehaviour //효과음 매니저
{
    public static SEManager instance;

    [SerializeField] AudioClip click; 
    [SerializeField] AudioClip close; 
    [SerializeField] AudioClip popup;
    [SerializeField] AudioClip nextPage; //손님 노트 넘기는 효과음
    [SerializeField] AudioClip click2; //메뉴 힌트,자잘한 버튼들 터치 효과음
    [SerializeField] AudioClip click3; //메뉴판에서 캐릭터가 원하는 메뉴 골랐을 때
    [SerializeField] AudioClip bird; //참새 소리   
    [SerializeField] AudioClip touch;//터치 시 나는 소리

    private AudioSource myAudio;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        myAudio = GetComponent<AudioSource>();
    }

    public void PlayTouchSound()//터치 시 효과음 재생
    {
       myAudio.PlayOneShot(touch);
    }

    public void PlayBirdSound()
    {
        myAudio.PlayOneShot(bird);
    }  

    public void PlayUIClickSound()
    {
        myAudio.PlayOneShot(click);
    } 

    public void PlayUIClickSound2()
    {
        myAudio.PlayOneShot(click2);
    }
    public void PlayUIClickSound3()
    {
        myAudio.PlayOneShot(click3);
    }

    public void PlayUICloseSound()
    {
        myAudio.PlayOneShot(close);
    }

    public void PlayPopupSound()
    {
        myAudio.PlayOneShot(popup);
    }

    public void PlayNextPageSound()
    {
        myAudio.PlayOneShot(nextPage);
    }

    public void SEOff() //효과음 끄기
    {
        myAudio.mute = true;
    }

    public void SEOn() //효과음 켜기
    {
        myAudio.mute = false;
    }

}
