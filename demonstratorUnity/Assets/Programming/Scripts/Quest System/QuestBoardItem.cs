using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static QuestManager;

public class QuestBoardItem : MonoBehaviour
{
	public int QuestUID;

	public TMP_Text DisplayText;

	// Start is called before the first frame update
	void Start()
	{
		if (QuestManager.current != null)
		{
			QuestManager.current.onQuestsUpdated += OnQuestUpDated;

		}
	}

	void OnDisable()
	{
		if (QuestManager.current != null)
		{
			QuestManager.current.onQuestsUpdated -= OnQuestUpDated;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SetUpItem(int uid)
	{
		QuestUID = uid;

		OnQuestUpDated(uid);
	}

	public void OnQuestUpDated(int uid)
	{
		if (QuestManager.current != null)
		{
			if (!QuestManager.current.GetQuestInfo(QuestUID).HasValue) return;

			QuestInfo info = QuestManager.current.GetQuestInfo(QuestUID).Value;


			if (info.State != QuestState.Avalible) Destroy(this.gameObject);

			DisplayText.text = $"Quest {info.UID} - {info.Name}\nTask - {info.Tasks[info.CurrentProgress]}";

		}
	}

	public void AcceptQuest()
	{
		if (QuestManager.current != null)
		{
			QuestManager.current.AcceptQuest(QuestUID);

			Destroy(this.gameObject);
		}
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
