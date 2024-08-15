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
		// for testing perposes.
		if (LevelLoading.Instance != null)
		{
			LevelLoading.Instance.LoadScene(LevelData.GetLevelByKey(0).Value.Name);
		}
		else
		{
			SceneManager.LoadScene(0);
		}
	}

	public void Quit()
	{
		Application.Quit();
	}

}
