using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuestManager;

public class QuestTrackerUI : MonoBehaviour
{
	public GameObject QuestItemPrefabs;

	public Transform parent;

	// Start is called before the first frame update
	void Start()
	{
		if (QuestManager.current != null)
		{
			foreach (QuestInfo item in QuestManager.current.QuestData.Values)
			{
				GameObject gameObject = Instantiate(QuestItemPrefabs, parent);
				gameObject.GetComponent<QuestItem>().SetUpQuestItem(item.ID);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
