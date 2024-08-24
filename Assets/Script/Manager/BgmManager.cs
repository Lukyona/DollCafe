using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager : MonoBehaviour
{
    public static BgmManager instance;

    #region BGMList 
    [SerializeField] AudioClip startBgm;
    [SerializeField] AudioClip cafeBgm;
    [SerializeField] AudioClip bearSound;
    [SerializeField] AudioClip carSound;
    [SerializeField] AudioClip breadSound;
    [SerializeField] AudioClip rabbitSound;
    [SerializeField] AudioClip ddorongSound;
    [SerializeField] AudioClip princessSound;
    [SerializeField] AudioClip familySeriesSound;
    [SerializeField] AudioClip sunFlowerSound;
    [SerializeField] AudioClip dogSound;
    [SerializeField] AudioClip soldierSound;
    [SerializeField] AudioClip namelessSound;
    [SerializeField] AudioClip heroDinosourSound;
    [SerializeField] AudioClip penguinSound;
    [SerializeField] AudioClip grandFatherSound;
    [SerializeField] AudioClip endingSound;
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
        int stop = PlayerPrefs.GetInt("BgmOnOff");
        if (stop == 1)//브금 off 상태면
            return;

        myAudio.clip = startBgm;
        myAudio.loop = true;
        myAudio.Play();
    }

    public void PlayCafeBgm() //카페 브금
    {
        int stop = PlayerPrefs.GetInt("BgmOnOff");
        if (stop == 1)//브금 off 상태면
        {
            OffBgm();
        }
        myAudio.clip = cafeBgm;
        myAudio.volume = 1f;
        myAudio.loop = true;
        myAudio.Play();
    }

    public void PlayCharacterBGM(int cNum)
    {        
        switch (cNum)
        {
            case 0://엔딩의 제제
                if(SystemManager.instance.GetMainCount() < 3) return;
                myAudio.clip = endingSound;
                myAudio.volume = 1f;
                break;
            case 1:
                myAudio.clip = bearSound;
                myAudio.volume = 1f;
                break;
            case 2:
                myAudio.clip = carSound;
                myAudio.volume = 0.5f;
                break;
            case 3:
                myAudio.clip = breadSound;
                myAudio.volume = 0.7f;
                break;
            case 4:
                myAudio.clip = rabbitSound;
                myAudio.volume = 0.8f;
                break;
            case 5:
                myAudio.clip = ddorongSound;
                myAudio.volume = 1f;
                break;
            case 6://도로시
                myAudio.clip = princessSound;
                myAudio.volume = 0.9f;
                break;
            case 7://루루
                myAudio.clip = familySeriesSound;
                myAudio.volume = 1f;
                break;
            case 8://샌디
                myAudio.clip = sunFlowerSound;
                myAudio.volume = 0.7f;
                break;
            case 9://친구
                myAudio.clip = dogSound;
                myAudio.volume = 0.7f;
                break;
            case 10://찰스
                myAudio.clip = soldierSound;
                myAudio.volume = 1f;
                break;
            case 11://무명이
                myAudio.clip = namelessSound;
                myAudio.volume = 0.6f;
                break;
            case 12://히로디노
                myAudio.clip = heroDinosourSound;
                myAudio.volume = 1f;
                break;
            case 13://닥터 펭
                myAudio.clip = penguinSound;
                myAudio.volume = 1f;
                break;
            case 14://롤렝드
                myAudio.clip = grandFatherSound;
                myAudio.volume = 0.7f;
                break;
        }
        myAudio.loop = true;
        myAudio.Play();
        
        int stop = PlayerPrefs.GetInt("BgmOnOff");
        if (stop == 1)//브금 off 상태면
        {
            OffBgm();
        }
    }

    public void BGMFadeOut()
    {
        int stop = PlayerPrefs.GetInt("BgmOnOff");
        if (stop == 1)//브금 off 상태면
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
            if(SceneManager.GetActiveScene().name == "GameScene" && VisitorNote.instance.GetReplayState() == 1)//다시보기 시작되면 페이드아웃 바로 종료
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
