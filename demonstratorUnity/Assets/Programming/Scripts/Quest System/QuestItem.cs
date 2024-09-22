using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static QuestManager;

/// <summary>
/// Quest item for use in the quest manager UI.
/// </summary>
public class QuestItem : MonoBehaviour
{
	public TMP_Text Text;
	public TMP_Text ButtonText;


	public int QuestUID;


	public bool IsPinned = false;

	// Start is called before the first frame update
	void Start()
	{
		// if (QuestManager.current != null)
		// {
		// 	IsPinned = QuestManager.current.CheckQuestIsPinned(QuestUID);
		// }

		if (SaveManager.current != null)
		{
			QuestManager.current.onPinsUpdated += OnSaveDataChanged;
			QuestManager.current.onQuestsUpdated += OnSaveDataChanged;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnDisable()
	{
		if (SaveManager.current != null)
		{
			QuestManager.current.onPinsUpdated += OnSaveDataChanged;
			QuestManager.current.onQuestsUpdated += OnSaveDataChanged;
		}
	}

	#region SetUpQuestItem
	/// <summary>
	/// Initilizes this quest item using the given quest UID.
	/// </summary>
	/// <param name="questUID">The UID of the quest this is ment to represent.</param>
	public void SetUpQuestItem(int questUID)
	{
		QuestUID = questUID;

		// if (QuestManager.current != null)
		// {
		// 	IsPinned = QuestManager.current.CheckQuestIsPinned(QuestUID);

		// 	Text.text = $"Quest {QuestUID} - {QuestManager.current.GetQuestInfo(QuestUID).Value.Name}\nTask - {QuestManager.current.GetQuestInfo(QuestUID).Value.Tasks[QuestManager.current.GetQuestInfo(QuestUID).Value.CurrentProgress]}";
		// }

		// ButtonText.text = IsPinned ? "Un-Pin Quest" : "Pin Quest";

		OnSaveDataChanged();

	}
	#endregion


	#region TogglePin
	/// <summary>
	/// Toggles the quest being pinned.
	/// </summary>
	public void TogglePin()
	{
		if (IsPinned)
		{
			UnPinQuest();
		}
		else
		{
			PinQuest();
		}
	}
	#endregion


	#region PinQuest // i will not summerise. it is 3am.
	public void PinQuest()
	{
		if (IsPinned) return;


		IsPinned = QuestManager.current.PinQuest(QuestUID);


		ButtonText.text = "Un-Pin Quest";

	}
	#endregion


	#region UnPinQuest
	public void UnPinQuest()
	{
		if (!IsPinned) return;

		QuestManager.current.UnPinQuest(QuestUID);

		IsPinned = false;

		ButtonText.text = "Pin Quest";

	}
	#endregion


	#region OnSaveDataChanged
	// so when quest data changes.
	// * this was done before the new Quest system events were made.
	/// <summary>
	/// called when save data is changed. Updates the quest.
	/// </summary>
	private void OnSaveDataChanged()
	{
		if (QuestManager.current != null)
		{
			if (!QuestManager.current.GetQuestInfo(QuestUID).HasValue) return;

			IsPinned = QuestManager.current.CheckQuestIsPinned(QuestUID);

			// TODO needs to be updated with quests - Yes, it is now.

			QuestInfo info = QuestManager.current.GetQuestInfo(QuestUID).Value;

			string taskDisplay = "Failure to get task - Out of bounds!";


			if (info.State == QuestState.Completed) taskDisplay = "Completed Quest";
			else if (info.CurrentProgress < info.Tasks.Length) taskDisplay = info.Tasks[info.CurrentProgress];

			Text.text = $"Quest {QuestUID} - {info.Name}\nTask - {taskDisplay}";

			if (IsPinned)
			{

				if (QuestManager.current.PinnedQuests[0] == QuestUID)
				{
					transform.SetSiblingIndex(0);
				}
				else if (QuestManager.current.PinnedQuests[1] == QuestUID)
				{
					transform.SetSiblingIndex(1);

				}
				else if (QuestManager.current.PinnedQuests[2] == QuestUID)
				{
					transform.SetSiblingIndex(2);

				}

			}
			else
			{
				transform.SetAsLastSibling();
			}


		}

		ButtonText.text = IsPinned ? "Un-Pin Quest" : "Pin Quest";
	}
	#endregion
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
