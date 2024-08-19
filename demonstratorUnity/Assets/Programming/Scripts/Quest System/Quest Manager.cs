using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
	public static QuestManager current;

	public enum QuestProgress
	{
		Hidden = 0,
		Avalible = 1,
		Accepted = 2,
		Compleated = 3,
	}

	public struct QuestInfo
	{
		public int ID;
		public string Name;
		public int Hub;
		public int CurrentState;
		public QuestProgress Progress;

		public QuestInfo(int id, string name, int hub, int currentState = 0, QuestProgress progress = QuestProgress.Avalible)
		{
			ID = id;
			Name = name;
			Hub = hub;
			CurrentState = currentState;
			Progress = progress;
		}
	}

	public static Dictionary<int, QuestInfo> QuestDefualt = new Dictionary<int, QuestInfo>()
	{
		{1, new QuestInfo(1, "Test1", 0)},
		{2, new QuestInfo(2, "Test2", 0)},
	};

	private Dictionary<int, QuestInfo> QuestData = new Dictionary<int, QuestInfo>();

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

			SaveManager.current.onLoad += OnSaveDataLoad;
			// SaveManager.current.onSave += OnSaveDataSave;
		}
		else
		{
			Debug.LogError("CATASTROPHIC ERROR - Quest sysyem cannot access save data!", this.gameObject);
			QuestData = QuestDefualt;


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

	private void OnSaveDataSave()
	{
		// ! WILL CAUSE A LOOP AND FORCE A CRASH! DO NOT RUN THIS!
		// SaveData.current.Quests = QuestData;
		// SaveManager.current.ForceSave();
	}

	public void UpdateQuest(int id, QuestInfo questInfo)
	{
		if (!QuestData.ContainsKey(id)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestData[id] = questInfo;

		OnUpdateQuests();
	}

	public void UpdateQuest(int id, QuestProgress progress)
	{
		if (!QuestData.ContainsKey(id)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = QuestData[id];

		questInfo.Progress = progress;

		QuestData[id] = questInfo;

		OnUpdateQuests();
	}

	public void UpdateQuest(int id, int currentState)
	{
		if (!QuestData.ContainsKey(id)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = QuestData[id];

		questInfo.CurrentState = currentState;

		QuestData[id] = questInfo;

		OnUpdateQuests();
	}

	public void AcceptQuest(int id)
	{
		if (!QuestData.ContainsKey(id)) throw new NullReferenceException("Cannot find quest with that ID!");

		QuestInfo questInfo = QuestData[id];

		questInfo.Progress = QuestProgress.Accepted;
		questInfo.CurrentState = 1;

		QuestData[id] = questInfo;

	}

	void OnUpdateQuests()
	{
		SaveData.current.Quests = QuestData;
		SaveManager.current.ForceSave();
	}
}
