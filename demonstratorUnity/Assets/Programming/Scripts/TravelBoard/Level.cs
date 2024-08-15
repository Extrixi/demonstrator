using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

	[SerializeField]
	public LevelData.IDKey key = new LevelData.IDKey(0, 0);

	public bool IsOnLevel = false;

	public TMP_Text ToolTip;

	private string _levelName;

	private Image _levelImageComp;

	public Sprite DefaultLevelIcon;
	public Sprite HoverLevelIcon;
	public Sprite SelectedLevelIcon;

	// Start is called before the first frame update
	void Start()
	{
		if (!LevelData.CheckIfKeyExsists(key))
		{
			Debug.LogError($"Critical Error - You are trying to load a >>level<< that does not exsist - id {key.ToString()}", this.gameObject);
		}

		_levelName = LevelData.GetLevelByKey(key).Value.Last;
		ToolTip.text = _levelName;
		ToolTip.gameObject.SetActive(false);

		LevelData.LevelInfo? levelData = LevelData.GetlevelByName(SceneManager.GetActiveScene().name);

		if (!levelData.HasValue) return;

		if (levelData.Value.Key == key)
		{
			IsOnLevel = true;
		}
	}

	void OnEnable()
	{
		if (!LevelData.CheckIfKeyExsists(key))
		{
			Debug.LogError($"Critical Error - You are trying to load a >>level<< that does not exsist - id {key.ToString()}", this.gameObject);
		}

		LevelData.LevelInfo? levelData = LevelData.GetlevelByName(SceneManager.GetActiveScene().name);

		if (!levelData.HasValue) return;

		if (levelData.Value.Key == key)
		{
			IsOnLevel = true;
		}
	}

	void OnDisable()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (IsOnLevel)
		{

			_levelImageComp.sprite = SelectedLevelIcon;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!IsOnLevel)
		{
			_levelImageComp.sprite = HoverLevelIcon;



		}

		ToolTip.gameObject.SetActive(true);
		ToolTip.text = _levelName;


	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (IsOnLevel)
		{
			_levelImageComp.sprite = SelectedLevelIcon;
		}
		else
		{
			_levelImageComp.sprite = DefaultLevelIcon;
		}

		ToolTip.gameObject.SetActive(false);
	}

	public void OnPointerClick(PointerEventData eventData)
	{

		if (!IsOnLevel)
		{
			IsOnLevel = true;

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
