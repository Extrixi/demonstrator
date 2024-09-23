using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static QuestManager;

/// <summary>
/// Questboard for Hubs, this displays quests the player can accept.
/// </summary>
public class QuestBoard : MonoBehaviour
{
	public GameObject QuestBoardItem;

	public Transform QuestBoardParent;



	public GameObject QuestBoardUI;

	private bool _isVisible = false;



	private bool _questManagerExsists = false;

	private int Hub = -1;

	// Start is called before the first frame update
	void Start()
	{
		if (QuestManager.current != null)
		{
			//QuestManager.current.onQuestsUpdated += UpdateUI;
			if (!LevelData.GetlevelByName(SceneManager.GetActiveScene().name).HasValue) Hub = -1;
			else Hub = LevelData.GetlevelByName(SceneManager.GetActiveScene().name).Value.Key.FirstID;


			_questManagerExsists = true;

			GenerateUI();
		}
	}



	// Update is called once per frame
	void Update()
	{
		if (QuestBoardUI.activeSelf != _isVisible) QuestBoardUI.SetActive(_isVisible);

		if (_isVisible && Input.GetKeyDown(KeyCode.Escape))
		{
			_isVisible = false;
		}



	}

	public void GenerateUI()
	{
		foreach (QuestInfo info in QuestManager.current.QuestData.Values)
		{
			if (info.Hub == Hub && info.State == QuestState.Avalible)
			{
				GameObject go = Instantiate(QuestBoardItem, QuestBoardParent);

				go.GetComponent<QuestBoardItem>().SetUpItem(info.UID);

			}
		}
	}

	public void ShowUI()
	{
		_isVisible = true;
	}

	public void HideUI()
	{
		_isVisible = false;
	}

	public void ToggleUI()
	{
		_isVisible = !_isVisible;
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|