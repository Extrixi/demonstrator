using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to allow the player to leave a level as there are no travel boards.
/// </summary>
public class LevelExit : MonoBehaviour
{
	public GameObject ExitInteractionObject;


	private LevelData.IDKey? LevelKey = null;

	private LevelData.IDKey? HubKey = null;

	private bool OnValidLevel = false;

	// Start is called before the first frame update
	void Start()
	{
		ExitInteractionObject.SetActive(false);

		LevelData.LevelInfo? levelData = LevelData.GetlevelByName(SceneManager.GetActiveScene().name);


		if (levelData != null)
		{
			LevelKey = levelData.Value.Key;
		}
		else
		{
			throw new NullReferenceException($"Cannot identify this level. Check if this level is valid in LevelData, or if you have changed the name of the level.");
		}


		if (LevelKey.Value.LastID == -1)
		{
			Debug.LogError("CANNOT USE IN A HUB! This object returns the player back to the hub. DO NOT use this on a hub!");
			throw new NullReferenceException("CANNOT USE IN A HUB! This object returns the player back to the hub. DO NOT use this on a hub!");
			// did this because you cannot use this on a hub. Dont use this on a hub.
		}
		else if (LevelKey != null)
		{
			OnValidLevel = true;
			HubKey = new LevelData.IDKey(LevelKey.Value.FirstID);
		}

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnTriggerEnter(Collider other)
	{
		if (!OnValidLevel) return;

		if (other.transform.tag == "Player")
		{
			ExitInteractionObject.SetActive(true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!OnValidLevel) return;


		if (other.transform.tag == "Player")
		{
			ExitInteractionObject.SetActive(false);

		}
	}

	public void OnWantingToLeave()
	{
		if (!OnValidLevel) return;

		if (LevelLoading.Instance != null)
		{
			LevelLoading.Instance.LoadScene(LevelData.GetLevelByKey(HubKey.Value).Value.Name);
		}
		else
		{
			SceneManager.LoadSceneAsync(LevelData.GetLevelByKey(HubKey.Value).Value.Name);
		}
	}


}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
