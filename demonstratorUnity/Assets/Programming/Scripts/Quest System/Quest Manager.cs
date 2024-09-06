using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
	public static QuestManager current;

	[Serializable]
	public enum QuestState
	{
		Hidden = 0,
		Avalible = 1,
		Accepted = 2,
		Completed = 3,
	}

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


	public Dictionary<int, QuestInfo> QuestData;

	public int[] PinnedQuests = new int[] { -1, -1, -1 };

	public event Action onPinsUpdated;
	public void OnPinsUpdatedInvoke()
	{
		if (onPinsUpdated != null)
		{
			onPinsUpdated.Invoke();
		}
	}

	public event Action onQuestsUpdated;
	void OnUpdateQuests()
	{
		//SaveData.current.Quests = QuestData;
		CopyQuests(QuestData, ref SaveData.current.Quests);

		SaveManager.current.ForceSave();

		if (onQuestsUpdated != null)
		{
			onQuestsUpdated.Invoke();
		}
	}


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
			Debug.LogError("CATASTROPHIC ERROR - Quest sysyem cannot access save data!", this.gameObject);
			QuestData = QuestDataSheet.QuestDefualt;
			PinnedQuests = new int[] { -1, -1, -1 };


		}
	}

	void OnDisable()
	{
		SaveManager.current.onLoad -= OnSaveDataLoad;
		// SaveManager.current.onSave -= OnSaveDataSave;

	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnSaveDataLoad()
	{
		QuestData = SaveData.current.Quests;
	}


	private void SaveQuets()
	{
		SaveData.current.Quests = QuestData;
		SaveManager.current.ForceSave();
	}



	public void UpdateQuest(int id, QuestInfo questInfo)
	{
		if (!QuestData.ContainsKey(id)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestData[id] = questInfo;

		OnUpdateQuests();
	}

	public void UpdateQuest(QuestInfo questInfo)
	{
		if (!QuestData.ContainsKey(questInfo.UID)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestData[questInfo.UID] = questInfo;

		OnUpdateQuests();
	}

	public void UpdateQuest(int id, QuestState progress)
	{
		if (!QuestData.ContainsKey(id)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = QuestData[id];

		questInfo.State = progress;

		QuestData[id] = questInfo;

		OnUpdateQuests();
	}

	public void UpdateQuest(int id, int currentState)
	{
		if (!QuestData.ContainsKey(id)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = QuestData[id];

		questInfo.CurrentProgress = currentState;

		QuestData[id] = questInfo;

		OnUpdateQuests();
	}

	public void AcceptQuest(int id)
	{
		if (!QuestData.ContainsKey(id)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = QuestData[id];

		questInfo.State = QuestState.Accepted;
		// questInfo.CurrentProgress = 1;

		QuestData[id] = questInfo;

		OnUpdateQuests();
	}



#nullable enable
	public QuestInfo? GetQuestInfo(int id)
	{
		if (QuestData.ContainsKey(id))
			return QuestData[id];
		else
			return null;
	}
#nullable disable

	public static void CopyQuests(Dictionary<int, QuestInfo> dataToCopy, ref Dictionary<int, QuestInfo> Target)
	{
		var originalDictionary = dataToCopy;
		Target = originalDictionary.ToDictionary(
			x => x.Key, // Typically no cloning necessary (immuable)
			x => (QuestInfo)x.Value.Clone()  // Do the copy how you want
		);
	}

#nullable enable
	public static string? GetTask(QuestInfo quest)
	{
		if (quest.CurrentProgress >= quest.Tasks.Length)
		{
			return null;
		}

		return quest.Tasks[quest.CurrentProgress];
	}
#nullable disable

	public void CompleateQuestTask(int questID)
	{
		if (!QuestData.ContainsKey(questID)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = GetQuestInfo(questID).Value;

		questInfo.CurrentProgress++;

		if (questInfo.CurrentProgress >= questInfo.Tasks.Length)
		{
			questInfo.State = QuestState.Completed;
		}


		UpdateQuest(questInfo);
	}

	public bool PinQuest(int questUID)
	{
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

}
