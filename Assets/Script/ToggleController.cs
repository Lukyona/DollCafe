using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour //설정의 브금 효과음 on off토글
{
	private bool isOn = true;

	public Color onColorBg;
	public Color offColorBg;

	public Image toggleBgImage;
	public RectTransform toggle;

	public GameObject handle;
	private RectTransform handleTransform;

	private float handleSize;
	private float onPosX;
	private float offPosX;

	public float handleOffset;

	public GameObject onIcon;
	public GameObject offIcon;


	public float speed;
	static float t = 0.0f;

	private bool switching = false;
	private int state = 0; // 0은 on, 1은 off

	void Awake()
	{
		handleTransform = handle.GetComponent<RectTransform>();
		RectTransform handleRect = handle.GetComponent<RectTransform>();
		handleSize = handleRect.sizeDelta.x;
		float toggleSizeX = toggle.sizeDelta.x;
		onPosX = (toggleSizeX / 2) - (handleSize/2) - handleOffset;
		offPosX = onPosX * -1;

	}

    private void Start()
    {
		OnApplicationFocus(true);//소리 설정 정보 불러오기
    }

    void Update()
	{

		if(switching)
		{
		
			Toggle(isOn);
		}
	}

	public void DoYourStaff()
	{
		Debug.Log(isOn);
	}

	public void Switching()
	{
		switching = true;
	}

    public void Toggle(bool toggleStatus)
	{
		if(!onIcon.activeInHierarchy || !offIcon.activeInHierarchy)
		{
			onIcon.SetActive(true);
			offIcon.SetActive(true);
		}
		
		if(toggleStatus)
		{
			toggleBgImage.color = SmoothColor(onColorBg, offColorBg);
			Transparency (onIcon, 1f, 0f);
			Transparency (offIcon, 0f, 1f);
			handleTransform.localPosition = SmoothMove(handle, onPosX, offPosX);
			if (gameObject.name == "ToggleBgm")
			{
				BgmManager.instance.BgmOff();
			}
			else
			{
				SEManager.instance.SEOff();
			}
			state = 1;
			SaveVolumeSetting();
		}
		else 
		{
			toggleBgImage.color = SmoothColor(offColorBg, onColorBg);
			Transparency (onIcon, 0f, 1f);
			Transparency (offIcon, 1f, 0f);
			handleTransform.localPosition = SmoothMove(handle, offPosX, onPosX);
			if (gameObject.name == "ToggleBgm")
			{
				BgmManager.instance.BgmOn();
			}
			else
			{
				SEManager.instance.SEOn();
			}
			state = 0;
			SaveVolumeSetting();
		}
			
	}


	Vector3 SmoothMove(GameObject toggleHandle, float startPosX, float endPosX)
	{
		
		Vector3 position = new Vector3 (Mathf.Lerp(startPosX, endPosX, t += speed * Time.deltaTime), 0f, 0f);
		StopSwitching();
		return position;
	}

	Color SmoothColor(Color startCol, Color endCol)
	{
		Color resultCol;
		resultCol = Color.Lerp(startCol, endCol, t += speed * Time.deltaTime);
		return resultCol;
	}

	CanvasGroup Transparency (GameObject alphaObj, float startAlpha, float endAlpha)
	{
		CanvasGroup alphaVal;
		alphaVal = alphaObj.gameObject.GetComponent<CanvasGroup>();
		alphaVal.alpha = Mathf.Lerp(startAlpha, endAlpha, t += speed * Time.deltaTime);
		return alphaVal;
	}

	void StopSwitching()
	{
		if(t > 1.0f)
		{
			switching = false;

			t = 0.0f;
			switch(isOn)
			{
			case true:
				isOn = false;
				DoYourStaff();
				break;

			case false:
				isOn = true;
				DoYourStaff();
				break;
			}

		}
	}

    public void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
			LoadVolumeSetting();
			if (gameObject.name == "ToggleBgm")
			{
				if (state == 0) // 0 on
				{
					isOn = true;
					BgmManager.instance.BgmOn();
					toggleBgImage.color = onColorBg;
					handleTransform.localPosition = new Vector3(onPosX, 0f, 0f);
					onIcon.gameObject.SetActive(true);
					offIcon.gameObject.SetActive(false);
					
				}
				else // 1 off
				{
					isOn = false;
					BgmManager.instance.BgmOff();
					toggleBgImage.color = offColorBg;
					handleTransform.localPosition = new Vector3(offPosX, 0f, 0f);
					onIcon.gameObject.SetActive(false);
					offIcon.gameObject.SetActive(true);
				}
			}
			else
			{
				if (state == 0) // 0 on
				{
					isOn = true;
					SEManager.instance.SEOn();
					toggleBgImage.color = onColorBg;
					handleTransform.localPosition = new Vector3(onPosX, 0f, 0f);
					onIcon.gameObject.SetActive(true);
					offIcon.gameObject.SetActive(false);
					
				}
				else // 1 off
				{
					isOn = false;
					SEManager.instance.SEOff();
					toggleBgImage.color = offColorBg;
					handleTransform.localPosition = new Vector3(offPosX, 0f, 0f);
					onIcon.gameObject.SetActive(false);
					offIcon.gameObject.SetActive(true);
					
				}
			}
		}

        else
        {
			SaveVolumeSetting();
        }
    }

	public bool LoadVolumeSetting()
    {
		bool result = false;
		try
		{
			if (PlayerPrefs.HasKey("BgmOnOff") || PlayerPrefs.HasKey("SEOnOff"))
			{				
				if (gameObject.name == "ToggleBgm")
                {
					state = PlayerPrefs.GetInt("BgmOnOff");
					//Debug.Log("PlayerPrefs has key : BgmOnOff");
					//Debug.Log("Bgm : " + state);
				}
                else
                {
					state = PlayerPrefs.GetInt("SEOnOff");
					//Debug.Log("PlayerPrefs has key : SE OnOff");
					//Debug.Log(" SE : " + state);
				}				
			}
			else
			{
				state = 0;
			}
							
			result = true;
		}
		catch (System.Exception e)
		{
			Debug.LogError("LoadSoundInfo Failed (" + e.Message + ")");
		}

		return result;
    }

	public bool SaveVolumeSetting()
    {
		bool result = false;
		try
		{
			if (gameObject.name == "ToggleBgm")
            {
				PlayerPrefs.SetInt("BgmOnOff", state);
				PlayerPrefs.Save();
				//Debug.Log("Saved BgmOnoff : " + state);
			}
			else
            {
				PlayerPrefs.SetInt("SEOnOff", state);
				PlayerPrefs.Save();
				//Debug.Log("Saved SEOnOff : " + state);
			}
						
			result = true;
		}
		catch (System.Exception e)
		{
			Debug.LogError("SaveSoundInfo Failed (" + e.Message + ")");
		}
		return result;
	}
}
