using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static QuestManager;

public class QuestTask : MonoBehaviour
{
	public int QuestUID;

	public int ProgressInQuest;

	private bool _acceptedQuest = false;
	private bool _nextToDo = false;

	private bool _hasInfo = false;

	private bool _taskCompleated = false;

	QuestManager.QuestInfo info;

	[Tooltip("Like start for when this quest task is ready to do.")]
	public UnityEvent OnQuestTaskReadyToDo;
	private bool _ranTaskReadyEvent = false;

	public UnityEvent OnQuestTaskCompleate;
	private bool _ranTaskCompleateEvent = false;


	// Start is called before the first frame update
	void Start()
	{
		if (QuestManager.current != null)
		{
			_hasInfo = QuestManager.current.GetQuestInfo(QuestUID).HasValue;



		}
	}

	// Update is called once per frame
	void Update()
	{
		// stop unessary calls to update so we can have nice fps.
		if (_nextToDo || (_taskCompleated && !_ranTaskCompleateEvent)) UpdateInfo();

		if (_acceptedQuest && _nextToDo && _hasInfo && !_ranTaskReadyEvent)
		{
			OnQuestTaskReadyToDo.Invoke();
		}
		else if (_taskCompleated && !_ranTaskCompleateEvent)
		{
			OnQuestTaskCompleate.Invoke();
		}

	}

	void UpdateInfo()
	{
		info = QuestManager.current.GetQuestInfo(QuestUID).Value;
		_hasInfo = true;

		_acceptedQuest = info.State == QuestState.Accepted;

		_nextToDo = info.CurrentProgress == ProgressInQuest;

		_taskCompleated = info.CurrentProgress > ProgressInQuest;
	}

	public void CompleateTask()
	{
		if (!_hasInfo) return;

		QuestManager.current.CompleateQuestTask(QuestUID);
	}
}
