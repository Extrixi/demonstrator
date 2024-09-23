using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

/// <summary>
/// The core data of quests.
/// </summary>
public class QuestDataSheet : MonoBehaviour
{
	// TODO make this a object on the persistant object so designers can make more items if needed.

	/// <summary>
	/// The key is the UID for the quests. The quest info is the quest itself.
	/// </summary>
	public static Dictionary<int, QuestInfo> QuestDefualt = new Dictionary<int, QuestInfo>()
	{
		//! DO NOT USE 0 (ZERO) FOR UID, IT IS USED AS A NULL FOR OTHER SCRIPTS!
		// Starting Quest
		{1, new QuestInfo(1, "Q_1H_1", 0, new string[] { "Task 1", "Task 2" }, QuestState.Accepted)},
		{2, new QuestInfo(2, "Q_2H_1", 0, new string[] { "Task 1", "Task 2" })},
		{3, new QuestInfo(3, "Q_2H_1", 0, new string[] { "Task 1", "Task 2" })},
		{4, new QuestInfo(4, "Q_2H_1", 0, new string[] { "Task 1", "Task 2" })},
		{5, new QuestInfo(5, "Q_2H_1", 0, new string[] { "Task 1", "Task 2" })}
	};

}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|