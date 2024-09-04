using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SetUpQuestItem(int questUID)
	{
		QuestUID = questUID;

		if (QuestManager.current != null)
		{
			IsPinned = QuestManager.current.CheckQuestIsPinned(QuestUID);

			// TODO needs to be updated with quests
			Text.text = $"Quest {QuestUID} - {QuestManager.current.GetQuestInfo(QuestUID).Value.Name}\nTask - {QuestManager.current.GetQuestInfo(QuestUID).Value.Tasks[QuestManager.current.GetQuestInfo(QuestUID).Value.CurrentProgress]}";
		}

		ButtonText.text = IsPinned ? "Un-Pin Quest" : "Pin Quest";

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
}
