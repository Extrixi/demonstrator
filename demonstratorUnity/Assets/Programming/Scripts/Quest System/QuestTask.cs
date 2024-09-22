using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static QuestManager;

/// <summary>
/// A task for use to link a quest to a task / script using events. (unity event).
/// </summary>
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

	public UnityEvent OnQuestTaskAlreadyCompleate;
	private bool _ranTaskAlreadyCompleateEvent = false;


	// Start is called before the first frame update
	void Start()
	{
		if (QuestManager.current != null)
		{
			_hasInfo = QuestManager.current.GetQuestInfo(QuestUID).HasValue;

			UpdateInfo();

		}
	}

	// Update is called once per frame
	void Update()
	{
		if (QuestManager.current == null) return;


		// stop unessary calls to update so we can have nice fps.
		if (_nextToDo || (_taskCompleated && !_ranTaskCompleateEvent)) UpdateInfo();

		if (_acceptedQuest && _nextToDo && _hasInfo && !_ranTaskReadyEvent)
		{
			OnQuestTaskReadyToDo.Invoke();
			_ranTaskReadyEvent = true;
		}
		else if (_taskCompleated && !_ranTaskCompleateEvent)
		{
			OnQuestTaskCompleate.Invoke();
			_ranTaskCompleateEvent = true;
		}
		else if (info.State == QuestState.Completed && !_ranTaskAlreadyCompleateEvent)
		{
			OnQuestTaskAlreadyCompleate.Invoke();
			_ranTaskAlreadyCompleateEvent = true;
		}

	}

	void UpdateInfo()
	{
		info = QuestManager.current.GetQuestInfo(QuestUID).Value;
		_hasInfo = true;

		if (!_hasInfo) return;

		_acceptedQuest = info.State == QuestState.Accepted;

		_nextToDo = info.CurrentProgress == ProgressInQuest;

		_taskCompleated = info.CurrentProgress > ProgressInQuest;
	}

	public void CompleateTask()
	{
		if (!_hasInfo && !_taskCompleated) return;

		QuestManager.current.CompleateQuestTask(QuestUID);
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
