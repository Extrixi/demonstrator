using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static QuestManager;

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

	public void PinQuest()
	{
		if (IsPinned) return;


		IsPinned = QuestManager.current.PinQuest(QuestUID);


		ButtonText.text = "Un-Pin Quest";

	}

	public void UnPinQuest()
	{
		if (!IsPinned) return;

		QuestManager.current.UnPinQuest(QuestUID);

		IsPinned = false;

		ButtonText.text = "Pin Quest";

	}

	// so when quest data changes.
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
}
