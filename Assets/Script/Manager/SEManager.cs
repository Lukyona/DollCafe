using UnityEngine;

public class SEManager : MonoBehaviour //효과음 매니저
{
    public static SEManager instance;

    #region 효과음 소스들
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip close;
    [SerializeField] private AudioClip popup;
    [SerializeField] private AudioClip nextPage; //손님 노트 넘기는 효과음
    [SerializeField] private AudioClip click2; //메뉴 힌트,자잘한 버튼들 터치 효과음
    [SerializeField] private AudioClip click3; //메뉴판에서 캐릭터가 원하는 메뉴 골랐을 때
    [SerializeField] private AudioClip bird; //참새 소리   
    [SerializeField] private AudioClip touch;//터치 시 나는 소리
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

    public void PlayTouchSound()//터치 시 효과음 재생
    {
        myAudio.PlayOneShot(touch);
    }

    public void PlayBirdSound()
    {
        myAudio.PlayOneShot(bird);
    }

    public void PlayUITouchSound()
    {
        myAudio.PlayOneShot(click);
    }

    public void PlayUITouchSound2()
    {
        myAudio.PlayOneShot(click2);
    }
    public void PlayUITouchSound3()
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

    public void OffSE() //효과음 끄기
    {
        myAudio.mute = true;
    }

    public void OnSE() //효과음 켜기
    {
        myAudio.mute = false;
    }
}
