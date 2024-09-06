using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void StartNewGame()
	{
		if (SaveManager.current != null)
		{
			SaveManager.current.DeleteSave();

			SaveData.current.CurrentLevelKey = new LevelData.IDKey(0);
			SaveManager.current.ForceSave();
		}

		// for testing perposes.
		if (LevelLoading.Instance != null)
		{
			LevelLoading.Instance.LoadScene(LevelData.GetLevelByKey(0, 0).Value.Name);
		}
		else
		{
			SceneManager.LoadScene(LevelData.GetLevelByKey(0, 0).Value.Name);
		}
	}

	public void LoadGame()
	{
		if (SaveManager.current == null) return;

		if (!LevelData.GetLevelByKey(SaveData.current.CurrentLevelKey).HasValue || SaveData.current.CurrentLevelKey.FirstID == -1)
		{
			StartNewGame();
			return;
		}

		if (LevelLoading.Instance != null)
		{
			LevelLoading.Instance.LoadScene(LevelData.GetLevelByKey(SaveData.current.CurrentLevelKey).Value.Name);
		}
		else
		{
			SceneManager.LoadScene(LevelData.GetLevelByKey(SaveData.current.CurrentLevelKey).Value.Name);
		}

	}

	public void Quit()
	{
		Application.Quit();
	}

}
