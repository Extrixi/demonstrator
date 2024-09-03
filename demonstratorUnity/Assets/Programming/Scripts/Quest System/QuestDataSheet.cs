using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

public class QuestDataSheet : MonoBehaviour
{
	/// <summary>
	/// The key is the UID for the quests. The quest info is the quest itself.
	/// </summary>
	public static Dictionary<int, QuestInfo> QuestDefualt = new Dictionary<int, QuestInfo>()
	{
		{1, new QuestInfo(1, "Q_1H_1", 1, new string[] { "Task 1", "Task 2" })},
		{2, new QuestInfo(2, "Q_2H_1", 1, new string[] { "Task 1", "Task 2" })}
	};


}
