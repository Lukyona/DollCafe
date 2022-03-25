using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeAudioSource : MonoBehaviour {

    public static FadeAudioSource instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)//오디오소스 볼륨 페이드아웃
    {
        float currentTime = 0;
        float start = audioSource.volume;        
        while (currentTime < duration)
        {
            if(SceneManager.GetActiveScene().name == "GameScene" && VisitorNote.instance.replayOn == 1)//다시보기 시작되면 페이드아웃 바로 종료
            {
                Debug.Log("오디오페이드아웃 종료");
                break;
            }
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        audioSource.Stop();
        yield break;        
    }
}