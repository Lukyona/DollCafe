using UnityEngine;
using UnityEngine.UI;
using System;

[SerializeField]
class ToggleController : MonoBehaviour //설정의 브금 효과음 on off토글
{
	[SerializeField] private Color onColor;
	[SerializeField] private Color offColor;

	[SerializeField] private GameObject onText;
	[SerializeField] private GameObject offText;

	[SerializeField] private Image toggleBgImage;
	[SerializeField] private RectTransform toggle; // 토글 오브젝트

	[SerializeField] private GameObject handle; // 하얀색 타원 도형, 토글할 때 좌우로 이동
	private RectTransform handleTransform;

	private float handleSize;
	private float onPosX;
	private float offPosX;

	private float handleOffset = 4f; // 약간의 보정치

	private float speed = 1f;
	private static float time = 0f;

	private bool switching = false; // true면 토글 작동

	private int state = 0; // 0은 on, 1은 off

	private void Awake()
	{
		handleTransform = handle.GetComponent<RectTransform>();
		handleSize = handleTransform.sizeDelta.x;

		float toggleSizeX = toggle.sizeDelta.x;
		onPosX = (toggleSizeX / 2) - (handleSize / 2) - handleOffset; // On상태일 때 핸들 X위치
		offPosX = onPosX * -1;                       // Off상태일 때 핸들 X위치
	}

	private void Start()
	{
		LoadVolumeSetting();
	}

	private void Update()
	{
		if (switching)
		{
			Toggle(state);
		}
	}

	[SerializeField]
	private void Switching() // 토글 터치했을 때, 토글 작동 끝났을 때 실행
	{
		if (switching == true)
		{
			if (time > 1f) // 토글 완료
			{
				switching = false;
				time = 0f;
				state = state == 0 ? 1 : 0; // 설정 상태 변경

				SaveVolumeSetting();
				gameObject.GetComponent<Button>().interactable = true;
			}
		}
		else
		{
			gameObject.GetComponent<Button>().interactable = false;
			switching = true;
		}
	}

	[SerializeField]
	private void Toggle(int toggleStatus)
	{
		if (toggleStatus == 0) // On -> Off
		{
			toggleBgImage.color = SmoothColor(onColor, offColor); // 초록색에서 빨간색으로 색 변경
			Transparency(onText, 1f, 0f); //  OnText 서서히 투명하게
			Transparency(offText, 0f, 1f);
			handleTransform.localPosition = SmoothMove(onPosX, offPosX); // 핸들 위치 변경

			if (gameObject.name == "ToggleBgm")
			{
				BgmManager.instance.OffBgm();
			}
			else
			{
				SEManager.instance.OffSE();
			}
		}
		else // Off -> On
		{
			toggleBgImage.color = SmoothColor(offColor, onColor);
			Transparency(onText, 0f, 1f);
			Transparency(offText, 1f, 0f);
			handleTransform.localPosition = SmoothMove(offPosX, onPosX);

			if (gameObject.name == "ToggleBgm")
			{
				BgmManager.instance.OnBgm();
			}
			else
			{
				SEManager.instance.OnSE();
			}
		}
		Switching(); // 토글 작동 해제
	}

	private Vector3 SmoothMove(float startPosX, float endPosX)
	{
		Vector3 position = new Vector3(Mathf.Lerp(startPosX, endPosX, time += speed * Time.deltaTime), 0f, 0f);
		return position;
	}

	private Color SmoothColor(Color startColor, Color endColor)
	{
		Color resultCol;
		resultCol = Color.Lerp(startColor, endColor, time += speed * Time.deltaTime);
		return resultCol;
	}

	private void Transparency(GameObject alphaObj, float startAlpha, float endAlpha)
	{
		CanvasGroup alphaVal;
		alphaVal = alphaObj.gameObject.GetComponent<CanvasGroup>();
		alphaVal.alpha = Mathf.Lerp(startAlpha, endAlpha, time += speed * Time.deltaTime);
	}

	[SerializeField]
	private void LoadVolumeSetting()
	{
		try
		{
			if (PlayerPrefs.HasKey("BgmState") || PlayerPrefs.HasKey("SEOnOff"))
			{
				if (gameObject.name == "ToggleBgm")
				{
					state = Convert.ToInt32(BgmManager.instance.IsBgmOff());
					if (state == 1) // 설정이 Off로 되어있다면
					{
						BgmManager.instance.OffBgm();
						toggleBgImage.color = offColor;
						handleTransform.localPosition = new Vector3(offPosX, 0f, 0f);
						Transparency(onText, 0f, 0f);
						Transparency(offText, 1f, 1f);
					}
				}
				else
				{
					state = PlayerPrefs.GetInt("SEOnOff");
					if (state == 1)
					{
						SEManager.instance.OffSE();
						toggleBgImage.color = offColor;
						handleTransform.localPosition = new Vector3(offPosX, 0f, 0f);
						Transparency(onText, 0f, 0f);
						Transparency(offText, 1f, 1f);
					}
				}
			}
			else
			{
				state = 0;
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError("LoadSoundInfo Failed (" + e.Message + ")");
		}
	}

	[SerializeField]
	private void SaveVolumeSetting()
	{
		try
		{
			if (gameObject.name == "ToggleBgm")
			{
				PlayerPrefs.SetInt("BgmState", state);
			}
			else
			{
				PlayerPrefs.SetInt("SEOnOff", state);
			}
			PlayerPrefs.Save();
		}
		catch (System.Exception e)
		{
			Debug.LogError("SaveSoundInfo Failed (" + e.Message + ")");
		}
	}
}
