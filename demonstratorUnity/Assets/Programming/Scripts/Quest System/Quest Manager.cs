using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages the quests and save data of the quests.
/// </summary>
public class QuestManager : MonoBehaviour
{
	/// <summary>
	/// Singleton
	/// </summary>
	public static QuestManager current;

	/// <summary>
	/// The state of the quest. [ 0 - Hidden | 1 - Avalible | 2 - Accepted | 3 - Completed ]
	/// </summary>
	[Serializable]
	public enum QuestState
	{
		Hidden = 0,
		Avalible = 1,
		Accepted = 2,
		Completed = 3,
	}

	/// <summary>
	/// Data of the quest, this is used to store quest data.
	/// </summary>
	[Serializable]
	public struct QuestInfo
	{
		public int UID;
		public string Name;
		public int Hub;
		public string[] Tasks;
		public int CurrentProgress;
		public QuestState State;

		public QuestInfo(int uid, string name, int hub, string[] messages, QuestState currentState = QuestState.Avalible, int currentProgress = 0)
		{
			UID = uid;
			Name = name;
			Hub = hub;
			Tasks = messages;
			State = currentState;
			CurrentProgress = currentProgress;
		}

		public override string ToString()
		{
			return $"[ {{ID = {UID} }}, {{ Name = {Name} }}, {{ Hub = {Hub} }}, {{ Messages = {Tasks.ToString()}}}, {{ Progress = {State.ToString()} }}, {{ CurrentState = {CurrentProgress} }} ]";
		}

		public QuestInfo Clone()
		{
			return new QuestInfo(UID, Name, Hub, Tasks, State, CurrentProgress);
		}
	}

	/// <summary>
	/// Current stored data of ALL quests.
	/// </summary>
	public Dictionary<int, QuestInfo> QuestData;

	public int[] PinnedQuests = new int[] { -1, -1, -1 };

	// Event for quest pinning.
	public event Action onPinsUpdated;
	public void OnPinsUpdatedInvoke()
	{
		if (onPinsUpdated != null)
		{
			onPinsUpdated.Invoke();
		}
	}

	// Event for when quests update.
	public event Action<int> onQuestsUpdated;
	void OnUpdateQuests(int UID = -1)
	{
		//SaveData.current.Quests = QuestData;
		CopyQuests(QuestData, ref SaveData.current.Quests);

		SaveManager.current.ForceSave();

		if (onQuestsUpdated != null)
		{
			onQuestsUpdated.Invoke(UID);
		}
	}


	#region Awake
	void Awake()
	{
		if (current != null && current != this)
		{
			Destroy(this);
		}
		else
		{
			current = this;
		}
	}
	#endregion


	#region Start
	// Start is called before the first frame update
	void Start()
	{
		if (SaveManager.current != null)
		{
			QuestData = SaveData.current.Quests;
			//CopyQuests(SaveData.current.Quests, ref QuestData);

			PinnedQuests = SaveData.current.PinnedQuests;

			if (PinnedQuests == null)
			{
				PinnedQuests = new int[] { -1, -1, -1 };
				SaveData.current.PinnedQuests = PinnedQuests;
				SaveManager.current.ForceSave();
			}
			else if (PinnedQuests.Length != 3)
			{
				PinnedQuests = new int[] { -1, -1, -1 };
				SaveData.current.PinnedQuests = PinnedQuests;
				SaveManager.current.ForceSave();
			}

			SaveManager.current.onLoad += OnSaveDataLoad;
			// SaveManager.current.onSave += OnSaveDataSave;

			//CopyQuests(QuestDataSheet.QuestDefualt, ref QuestData);
		}
		else
		{
			// Fail safe. just in case.
			Debug.LogError("CATASTROPHIC ERROR - Quest sysyem cannot access save data!", this.gameObject);
			QuestData = QuestDataSheet.QuestDefualt;
			PinnedQuests = new int[] { -1, -1, -1 };
		}
	}
	#endregion


	#region OnDisable
	void OnDisable()
	{
		SaveManager.current.onLoad -= OnSaveDataLoad;
		// SaveManager.current.onSave -= OnSaveDataSave;

	}
	#endregion


	#region OnSaveDataLoad
	private void OnSaveDataLoad()
	{
		QuestData = SaveData.current.Quests;
	}
	#endregion


	#region SaveQuets
	/// <summary>
	/// Saves the quests to file.
	/// </summary>
	private void SaveQuets() // ? not sure why this is not used? might have a reason?
	{
		SaveData.current.Quests = QuestData;
		SaveManager.current.ForceSave();
	}
	#endregion


	#region UpdateQuest (int uid, QuestInfo questInfo)
	/// <summary>
	/// Updates a quest using its UID (int) with a new QuestInfo.
	/// </summary>
	/// <param name="uid">The UID of the quest.</param>
	/// <param name="questInfo">The new data to replace the current data of the quest.</param>
	/// <exception cref="NullReferenceException">Thrown when there is no quests matching that UID.</exception>
	[Obsolete("Better alternative UpdateQuest(QuestInfo questInfo)", false)]
	public void UpdateQuest(int uid, QuestInfo questInfo)
	{
		if (!QuestData.ContainsKey(uid)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestData[uid] = questInfo;

		OnUpdateQuests(uid);
	}
	#endregion


	#region UpdateQuest (QuestInfo questInfo)
	/// <summary>
	/// Updates the quest using just the QuestInfo.
	/// </summary>
	/// <param name="questInfo">The quest and data you want to replace.</param>
	/// <exception cref="NullReferenceException">Thrown when there is no quests matching that UID.</exception>
	public void UpdateQuest(QuestInfo questInfo)
	{
		if (!QuestData.ContainsKey(questInfo.UID)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestData[questInfo.UID] = questInfo;

		OnUpdateQuests(questInfo.UID);
	}
	#endregion


	#region UpdateQuest (int uid, QuestState progress)
	/// <summary>
	/// Updates a quest's progress using its UID (int).
	/// </summary>
	/// <param name="uid">The UID of the quest.</param>
	/// <param name="state">The new progress of the quest.</param>
	/// <exception cref="NullReferenceException">Thrown when there is no quests matching that UID.</exception>
	public void UpdateQuest(int uid, QuestState state)
	{
		if (!QuestData.ContainsKey(uid)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = QuestData[uid];

		questInfo.State = state;

		QuestData[uid] = questInfo;

		OnUpdateQuests(uid);
	}
	#endregion


	#region UpdateQuest (int id, int currentState)
	/// <summary>
	/// Updates a quest's currentState using its UID (int).
	/// </summary>
	/// <param name="uid">The UID of the quest.</param>
	/// <param name="currentProgress">The new state of the quest.</param>
	/// <exception cref="NullReferenceException">Thrown when there is no quests matching that UID.</exception>
	public void UpdateQuest(int uid, int currentProgress)
	{
		if (!QuestData.ContainsKey(uid)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = QuestData[uid];

		questInfo.CurrentProgress = currentProgress;

		QuestData[uid] = questInfo;

		OnUpdateQuests(uid);
	}
	#endregion


	#region AcceptQuest
	/// <summary>
	/// Accepts the quest.
	/// </summary>
	/// <param name="uid">The UID of the quest to accept.</param>
	/// <exception cref="NullReferenceException">Thrown when there is no quests matching that UID.</exception>
	public void AcceptQuest(int uid)
	{
		if (!QuestData.ContainsKey(uid)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = QuestData[uid];

		questInfo.State = QuestState.Accepted;
		// questInfo.CurrentProgress = 1;

		QuestData[uid] = questInfo;

		OnUpdateQuests(uid);
	}
	#endregion


	#region 
#nullable enable
	/// <summary>
	/// Gets the QuestInfo using the UID.
	/// </summary>
	/// <param name="uid">The unique id of the quest to find.</param>
	/// <returns>The QuestInfo of the quest, this CAN be null.</returns>
	public QuestInfo? GetQuestInfo(int uid)
	{
		if (QuestData.ContainsKey(uid))
			return QuestData[uid];
		else
			return null;
	}
#nullable disable
	#endregion


	#region CopyQuests
	/// <summary>
	/// Copts a Dictionary<int, QuestInfo> to another Dictionary.
	/// </summary>
	/// <param name="dataToCopy">The dictionary you wish to copy.</param>
	/// <param name="Target">The dictionary to copy to.</param>
	public static void CopyQuests(Dictionary<int, QuestInfo> dataToCopy, ref Dictionary<int, QuestInfo> Target)
	{
		var originalDictionary = dataToCopy;
		Target = originalDictionary.ToDictionary(
			x => x.Key, // Typically no cloning necessary (immuable)
			x => (QuestInfo)x.Value.Clone()  // Do the copy how you want
		);
	}
	#endregion

	#region 
#nullable enable
	/// <summary>
	/// Gets the task display from the quest info.
	/// </summary>
	/// <param name="quest">The quest info to get the task from.</param>
	/// <returns>A text for display.</returns>
	public static string? GetTask(QuestInfo quest)
	{
		if (quest.CurrentProgress >= quest.Tasks.Length)
		{
			return null;
		}

		return quest.Tasks[quest.CurrentProgress];
	}
#nullable disable
	#endregion


	#region CompleateQuestTask
	/// <summary>
	/// Completes the task with the UID.
	/// </summary>
	/// <param name="questUID">The UID of the quest to complete.</param>
	/// <exception cref="NullReferenceException">Thrown when there is no quests matching that UID.</exception>
	public void CompleateQuestTask(int questUID)
	{
		if (!QuestData.ContainsKey(questUID)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = GetQuestInfo(questUID).Value;

		questInfo.CurrentProgress++;

		if (questInfo.CurrentProgress >= questInfo.Tasks.Length)
		{
			questInfo.State = QuestState.Completed;
		}


		UpdateQuest(questInfo);
	}
	#endregion


	#region PinQuest
	/// <summary>
	/// Pins the quest with the UID.
	/// </summary>
	/// <param name="questUID">The UID of the quest you want to pin.</param>
	/// <returns>True if the operation was successful.</returns>
	public bool PinQuest(int questUID)
	{
		if (CheckQuestIsPinned(questUID)) return false;

		if (PinnedQuests.Contains(-1))
		{
			if (PinnedQuests[0] == -1)
			{
				PinnedQuests[0] = questUID;

				OnPinsUpdatedInvoke();

				return true;
			}
			else if (PinnedQuests[1] == -1)
			{
				PinnedQuests[1] = questUID;

				OnPinsUpdatedInvoke();


				return true;
			}
			else if (PinnedQuests[2] == -1)
			{
				PinnedQuests[2] = questUID;

				OnPinsUpdatedInvoke();


				return true;
			}
		}
		else
		{
			int a = PinnedQuests[0];
			int b = PinnedQuests[1];

			PinnedQuests[0] = questUID;
			PinnedQuests[1] = a;
			PinnedQuests[2] = b;

			OnPinsUpdatedInvoke();

			return true;
		}

		Debug.LogError("Very bad things happened!", gameObject);

		return false;
	}
	#endregion


	#region UnPinQuest
	/// <summary>
	/// Unpins a quest with the UID.
	/// </summary>
	/// <param name="questUID">The UID of the quest you wish to unpin.</param>
	public void UnPinQuest(int questUID)
	{
		if (PinnedQuests[0] == questUID)
		{
			PinnedQuests[0] = -1;
		}
		else if (PinnedQuests[1] == questUID)
		{
			PinnedQuests[1] = -1;
		}
		else if (PinnedQuests[2] == questUID)
		{
			PinnedQuests[2] = -1;
		}

		OnPinsUpdatedInvoke();
	}
	#endregion


	#region CheckQuestIsPinned
	/// <summary>
	/// Checks if the quest with the UID is pinned.
	/// </summary>
	/// <param name="questUID">The UID to check for.</param>
	/// <returns>True if the quest is pinned.</returns>
	public bool CheckQuestIsPinned(int questUID)
	{
		if (PinnedQuests.Contains(questUID))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	#endregion


	#region GetQuestsForHub
	/// <summary>
	/// Gets all the quests that are ment to be on that hub that have not been accepted yet.
	/// </summary>
	/// <param name="HubID">The UID of the hub.</param>
	/// <returns>List of QuestInfos of avalible quests for that hub.</returns>
	public List<QuestInfo> GetQuestsForHub(int HubID)
	{
		List<QuestInfo> infos = new List<QuestInfo>();

		foreach (QuestInfo questInfo in QuestData.Values)
		{
			if (questInfo.Hub == HubID && questInfo.State == QuestState.Avalible)
			{
				infos.Add(questInfo);
			}
		}

		return infos;
	}
	#endregion
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|