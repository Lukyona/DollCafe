using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager instance;

    public AudioSource startBgm;
    public AudioSource cafeBgm;
    public AudioSource princessSound;
    public AudioSource carSound;
    public AudioSource breadSound;
    public AudioSource familySeriesSound;
    public AudioSource soldierSound;
    public AudioSource heroDinosourSound;
    public AudioSource penguinSound;
    public AudioSource bearSound;
    public AudioSource rabbitSound;
    public AudioSource ddorongSound;
    public AudioSource sunFlowerSound;
    public AudioSource dogSound;
    public AudioSource grandFatherSound;
    public AudioSource noNameSound;
    public AudioSource endingSound;

    private AudioSource myAudio;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        myAudio = GetComponent<AudioSource>();
    }


    public void MainBgm() //시작화면 브금
    {
        myAudio = startBgm;
        myAudio.Play();
        int s = PlayerPrefs.GetInt("BgmOnOff");
        if (s == 1)//브금 off 상태면
        {
            BgmOff();
        }
    }

    public void CafeBgm() //카페 브금
    {
        myAudio = cafeBgm;
        myAudio.volume = 1f;
        myAudio.Play();
        int s = PlayerPrefs.GetInt("BgmOnOff");
        if (s == 1)//브금 off 상태면
        {
            BgmOff();
        }
    }

    public void CharacterBGMPlay(int n)
    {        
        switch (n)
        {
            case 0://엔딩의 제제
                myAudio = endingSound;
                myAudio.volume = 1f;
                break;
            case 1:
                myAudio = bearSound;
                myAudio.volume = 1f;
                break;
            case 2:
                myAudio = carSound;
                myAudio.volume = 0.5f;
                break;
            case 3:
                myAudio = breadSound;
                myAudio.volume = 0.7f;
                break;
            case 4:
                myAudio = rabbitSound;
                myAudio.volume = 0.8f;
                break;
            case 5:
                myAudio = ddorongSound;
                myAudio.volume = 1f;
                break;
            case 6://도로시
                myAudio = princessSound;
                myAudio.volume = 0.9f;
                break;
            case 7://루루
                myAudio = familySeriesSound;
                myAudio.volume = 1f;
                break;
            case 8://샌디
                myAudio = sunFlowerSound;
                myAudio.volume = 0.7f;
                break;
            case 9://친구
                myAudio = dogSound;
                myAudio.volume = 0.7f;
                break;
            case 10://찰스
                myAudio = soldierSound;
                myAudio.volume = 1f;
                break;
            case 11://무명이
                myAudio = noNameSound;
                myAudio.volume = 0.6f;
                break;
            case 12://히로디노
                myAudio = heroDinosourSound;
                myAudio.volume = 1f;
                break;
            case 13://닥터 펭
                myAudio = penguinSound;
                myAudio.volume = 1f;
                break;
            case 14://롤렝드
                myAudio = grandFatherSound;
                myAudio.volume = 0.7f;
                break;
        }
        myAudio.Play();
        int s = PlayerPrefs.GetInt("BgmOnOff");
        if (s == 1)//브금 off 상태면
        {
            BgmOff();
        }
    }

    public void BGMFadeOut()
    {
        int s = PlayerPrefs.GetInt("BgmOnOff");
        if (s == 1)//브금 off 상태면
        {
            return;
        }
        FadeAudioSource.instance.StartCoroutine(FadeAudioSource.instance.StartFade(myAudio, 2.5f, 0f));
    }

    public void BgmStop()
    {
        myAudio.Stop();
    }

    public void BgmOff()
    {
        myAudio.mute = true;
    }

    public void BgmOn()
    {
        myAudio.mute = false;
    }
}
