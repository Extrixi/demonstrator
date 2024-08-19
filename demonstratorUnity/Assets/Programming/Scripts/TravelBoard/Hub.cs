using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hub : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	[SerializeField]
	public LevelData.IDKey key = new LevelData.IDKey(0);


	public GameObject[] Levels;

	public bool IsOnHub = false;

	public bool IsInSubLevel = false;

	public TMP_Text ToolTip;

	private string _hubName;

	private Image _hubImageComp;

	public Sprite DefaultHubIcon;
	public Sprite HoverHubIcon;
	public Sprite SelectedHubIcon;
	public Sprite InLevelHubIcon;

	void Awake()
	{
		_hubImageComp = GetComponent<Image>();
	}

	// Start is called before the first frame update
	void Start()
	{
		if (!LevelData.CheckIfKeyExsists(key))
		{
			Debug.LogError($"Critical Error - You are trying to load a >>hub<< that does not exsist - id {key.ToString()}", this.gameObject);
			return;
		}

		_hubName = LevelData.GetLevelByKey(key).Value.Last;
		ToolTip.text = _hubName;
		ToolTip.gameObject.SetActive(false);

		LevelData.LevelInfo? levelData = LevelData.GetlevelByName(SceneManager.GetActiveScene().name);

		if (!levelData.HasValue) return;

		if (levelData.Value.Key == key)
		{
			IsOnHub = true;
		}
		else if (levelData.Value.Key.FirstID == key.FirstID)
		{
			IsInSubLevel = true;
		}

	}

	void OnEnable()
	{
		if (!LevelData.CheckIfKeyExsists(key))
		{
			Debug.LogError($"Critical Error - You are trying to load a >>hub<< that does not exsist - id {key.ToString()}", this.gameObject);
			return;
		}

		LevelData.LevelInfo? levelData = LevelData.GetlevelByName(SceneManager.GetActiveScene().name);

		if (!levelData.HasValue) return;

		if (levelData.Value.Key == key)
		{
			IsOnHub = true;
		}
		else if (levelData.Value.Key.FirstID == key.FirstID)
		{
			IsInSubLevel = true;
		}
	}

	void OnDisable()
	{
		ShowOrHideLevels(false);
	}

	// Update is called once per frame
	void Update()
	{


		if (IsOnHub && !IsInSubLevel)
		{
			ShowOrHideLevels(IsOnHub);

			_hubImageComp.sprite = SelectedHubIcon;
		}
		else if (IsInSubLevel)
		{
			ShowOrHideLevels(IsInSubLevel);

			_hubImageComp.sprite = InLevelHubIcon;

			// Show Level We are on.
		}
		else
		{
			ShowOrHideLevels(IsOnHub);
		}
	}

	public void ShowOrHideLevels(bool ShowLevels)
	{
		if (Levels.Length <= 0) return;

		foreach (GameObject gameObject in Levels)
		{
			gameObject.SetActive(ShowLevels);
		}
	}

	public void HideHubAndLevels()
	{
		if (Levels.Length <= 0)
		{
			gameObject.SetActive(false);
			return;
		}

		foreach (GameObject gameObject in Levels)
		{
			gameObject.SetActive(false);
		}

		gameObject.SetActive(false);

	}

	// public void OnClicked()
	// {
	// 	if (LevelLoading.Instance != null)
	// 		LevelLoading.Instance.LoadScene(LevelData.GetLevelNameByKey(key));
	// 	else
	// 		SceneManager.LoadSceneAsync(LevelData.GetLevelNameByKey(key));
	// }


	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!IsOnHub && !IsInSubLevel)
		{
			_hubImageComp.sprite = HoverHubIcon;


			ToolTip.gameObject.SetActive(true);
			ToolTip.text = _hubName;
		}


	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (IsOnHub)
		{
			_hubImageComp.sprite = SelectedHubIcon;
		}
		else if (IsInSubLevel)
		{
			_hubImageComp.sprite = InLevelHubIcon;
		}
		else
		{
			_hubImageComp.sprite = DefaultHubIcon;
		}

		ToolTip.gameObject.SetActive(false);

	}

	public void OnPointerClick(PointerEventData eventData)
	{

		if (!IsOnHub)
		{
			IsOnHub = true;

			if (LevelLoading.Instance != null)
			{
				LevelLoading.Instance.LoadScene(LevelData.GetLevelNameByKey(key));
			}
			else
			{
				SceneManager.LoadSceneAsync(LevelData.GetLevelNameByKey(key));
			}
		}
	}
}
