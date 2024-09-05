using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

public class QuestTrackerUI : MonoBehaviour
{
	public GameObject QuestItemPrefabs;

	public Transform parent;

	public GameObject UI;

	private bool _isVisible = false;

	// Start is called before the first frame update
	void Start()
	{
		if (QuestManager.current != null)
		{
			foreach (QuestInfo item in QuestManager.current.QuestData.Values)
			{
				if (item.State != QuestState.Accepted) continue;

				GameObject gameObject = Instantiate(QuestItemPrefabs, parent);
				gameObject.GetComponent<QuestItem>().SetUpQuestItem(item.UID);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (UI.activeSelf != _isVisible) UI.SetActive(_isVisible);

		if (Input.GetKeyDown(KeyCode.N))
		{
			_isVisible = !_isVisible;
		}
	}
}
