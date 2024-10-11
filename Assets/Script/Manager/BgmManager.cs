using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager : MonoBehaviour
{
    public static BgmManager instance;

    #region BGMList 
    [SerializeField] private AudioClip startBgm;
    [SerializeField] private AudioClip cafeBgm;

    [SerializeField] private AudioClip[] characterBgmList;
    [SerializeField] private float[] characterVolumeList;
    #endregion

    private AudioSource myAudio;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        myAudio = GetComponent<AudioSource>();
    }

    private void PlayBgm(AudioClip clip)
    {
        if (IsBgmOff())
        {
            OffBgm();
            return;
        }
        myAudio.clip = clip;
        myAudio.volume = 1f;
        myAudio.loop = true;
        myAudio.Play();
    }

    public void PlayStartBgm()
    {
        PlayBgm(startBgm);
    }

    public void PlayCafeBgm()
    {
        PlayBgm(cafeBgm);
    }

    public void PlayCharacterBGM(int cNum)
    {
        if (cNum < 0 || cNum >= characterBgmList.Length) return; // 유효 범위 체크

        myAudio.clip = characterBgmList[cNum];
        myAudio.volume = characterVolumeList[cNum];
        myAudio.Play();

        if (IsBgmOff())//브금 off 상태면
        {
            OffBgm();
        }
    }

    public bool IsBgmOff()
    {
        return PlayerPrefs.GetInt("BgmState") == 1;
    }

    public void StopBgm()
    {
        myAudio.Stop();
    }

    public void OffBgm()
    {
        myAudio.mute = true;
    }

    public void OnBgm()
    {
        myAudio.mute = false;
    }

    public void BgmFadeOut()
    {
        if (IsBgmOff())
        {
            return;
        }
        StartCoroutine(StartFade(myAudio, 2.5f));
    }

    private IEnumerator StartFade(AudioSource audioSource, float duration)//오디오소스 볼륨 페이드아웃
    {
        float currentTime = 0f;
        float targetVolume = 0f;
        float start = audioSource.volume;
        while (currentTime < duration) // 2.5초동안 페이드아웃
        {
            if (SceneManager.GetActiveScene().name == "GameScene" && VisitorNote.instance.GetReplayState() == 1)//다시보기 시작되면 페이드아웃 바로 종료
            {
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
