using System.Collections;
using System.Collections.Generic;
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

	private Image HubImageComp;

	public Sprite DefaultHubIcon;
	public Sprite HoverHubIcon;
	public Sprite SelectedHubIcon;
	public Sprite InLevelHubIcon;

	void Awake()
	{
		HubImageComp = GetComponent<Image>();
	}

	// Start is called before the first frame update
	void Start()
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

			HubImageComp.sprite = SelectedHubIcon;
		}
		else if (IsInSubLevel)
		{
			ShowOrHideLevels(IsInSubLevel);

			HubImageComp.sprite = InLevelHubIcon;

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
			HubImageComp.sprite = HoverHubIcon;
		}


	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (IsOnHub)
		{
			HubImageComp.sprite = SelectedHubIcon;
		}
		else if (IsInSubLevel)
		{
			HubImageComp.sprite = InLevelHubIcon;
		}
		else
		{
			HubImageComp.sprite = DefaultHubIcon;
		}
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
