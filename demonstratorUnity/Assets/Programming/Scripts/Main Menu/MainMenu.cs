using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	/// <summary>
	/// Starts a new game with a new save, this deletes the old save.
	/// </summary>
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

	/// <summary>
	/// Loads the current save of the game. If there is none, then it will start a new campaign.
	/// </summary>
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

	/// <summary>
	/// Well... it quits, right?
	/// </summary>
	public void Quit()
	{
		Application.Quit();
	}

}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
