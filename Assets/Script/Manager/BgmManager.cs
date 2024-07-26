using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager : MonoBehaviour
{
    public static BgmManager instance;

    #region BGMList 
    [SerializeField] AudioSource startBgm;
    [SerializeField] AudioSource cafeBgm;
    [SerializeField] AudioSource princessSound;
    [SerializeField] AudioSource carSound;
    [SerializeField] AudioSource breadSound;
    [SerializeField] AudioSource familySeriesSound;
    [SerializeField] AudioSource soldierSound;
    [SerializeField] AudioSource heroDinosourSound;
    [SerializeField] AudioSource penguinSound;
    [SerializeField] AudioSource bearSound;
    [SerializeField] AudioSource rabbitSound;
    [SerializeField] AudioSource ddorongSound;
    [SerializeField] AudioSource sunFlowerSound;
    [SerializeField] AudioSource dogSound;
    [SerializeField] AudioSource grandFatherSound;
    [SerializeField] AudioSource noNameSound;
    [SerializeField] AudioSource endingSound;
    #endregion

    private AudioSource myAudio;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        myAudio = GetComponent<AudioSource>();
    }

    public void PlayMainBgm() //시작화면 브금
    {
        int s = PlayerPrefs.GetInt("BgmOnOff");
        if (s == 1)//브금 off 상태면
            return;

        myAudio = startBgm;
        myAudio.Play();
    }

    public void PlayCafeBgm() //카페 브금
    {
        myAudio = cafeBgm;
        int s = PlayerPrefs.GetInt("BgmOnOff");
        if (s == 1)//브금 off 상태면
        {
            OffBgm();
        }

        myAudio.volume = 1f;
        myAudio.Play();
    }

    public void PlayCharacterBGM(int n)
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
            OffBgm();
        }
    }

    public void BGMFadeOut()
    {
        int s = PlayerPrefs.GetInt("BgmOnOff");
        if (s == 1)//브금 off 상태면
        {
            return;
        }
        StartCoroutine(StartFade(myAudio, 2.5f));
    }

    public void StopBgm()
    {
        myAudio.Stop();
    }

    public void OffBgm()
    {
        myAudio.mute = true;
    }

    public void OnBGM()
    {
        myAudio.mute = false;
    }

    public IEnumerator StartFade(AudioSource audioSource, float duration)//오디오소스 볼륨 페이드아웃
    {
        float currentTime = 0f;
        float targetVolume = 0f;
        float start = audioSource.volume;        
        while (currentTime < duration) // 2.5초동안 페이드아웃
        {
            if(SceneManager.GetActiveScene().name == "GameScene" && VisitorNote.instance.replayOn == 1)//다시보기 시작되면 페이드아웃 바로 종료
            {
                Debug.Log("오디오페이드아웃 종료");
                break;
            }
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration); // 오디오소스의 볼륨을 0으로 서서히 낮추기
            yield return null;
        }
        audioSource.Stop(); 
        yield break; // 코루틴 종료
    }
}
