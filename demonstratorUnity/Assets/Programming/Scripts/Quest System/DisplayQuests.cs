using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static QuestManager;

/// <summary>
/// Displays the pinned quest's tasks. Currently up to 3.
/// </summary>
public class DisplayQuests : MonoBehaviour
{
	public TMP_Text QuestTraker;

	private int[] _pinnedQuests = new int[] { -1, -1, -1 };

	#region Start
	// Start is called before the first frame update
	void Start()
	{
		if (QuestManager.current != null)
		{
			QuestManager.current.onPinsUpdated += DisplayQuestsFromData;
			QuestManager.current.onQuestsUpdated += DisplayQuestsFromData;
			DisplayQuestsFromData();
		}

	}
	#endregion

	#region OnDisable
	void OnDisable()
	{
		if (QuestManager.current != null)
		{
			QuestManager.current.onPinsUpdated -= DisplayQuestsFromData;
			QuestManager.current.onQuestsUpdated -= DisplayQuestsFromData;
		}
	}
	#endregion


	#region DisplayQuestsFromData
	/// <summary>
	/// Displays the quest tasks. Needs to be called when a quest is updated.
	/// </summary>
	private void DisplayQuestsFromData()
	{
		List<string> display = new List<string> { $"Quests:\n" };

		foreach (QuestInfo quest in QuestManager.current.QuestData.Values)
		{
			string questCurrentTask = "";

			if (quest.State != QuestState.Accepted) continue;

			if (_pinnedQuests[0] == quest.UID)
			{
				if (quest.Tasks[quest.CurrentProgress] != null) questCurrentTask = quest.Tasks[quest.CurrentProgress];
				else questCurrentTask = "Unknown";

				display.Insert(1, $"[P1] Quest {quest.UID} - {questCurrentTask}\n");

				continue;
			}
			else if (_pinnedQuests[1] == quest.UID)
			{
				if (quest.Tasks[quest.CurrentProgress] != null) questCurrentTask = quest.Tasks[quest.CurrentProgress];
				else questCurrentTask = "Unknown";

				display.Insert(2, $"[P2] Quest {quest.UID} - {questCurrentTask}\n");

				continue;
			}
			else if (_pinnedQuests[2] == quest.UID)
			{
				if (quest.Tasks[quest.CurrentProgress] != null) questCurrentTask = quest.Tasks[quest.CurrentProgress];
				else questCurrentTask = "Unknown";

				display.Insert(3, $"[P3] Quest {quest.UID} - {questCurrentTask}\n");

				continue;
			}

			if (quest.Tasks[quest.CurrentProgress] != null) questCurrentTask = quest.Tasks[quest.CurrentProgress];
			else questCurrentTask = "Unknown";

			display.Add($"Quest {quest.UID} - {questCurrentTask}\n");

		}

		string Comb = "";

		foreach (string DisplayString in display)
		{
			Comb += DisplayString;
		}

		QuestTraker.text = Comb;
	}
	#endregion
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
