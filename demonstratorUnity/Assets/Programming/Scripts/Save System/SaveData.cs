using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
	private static SaveData _current;
	public static SaveData current
	{
		get
		{
			if (_current == null)
			{
				_current = new SaveData();
			}

			return _current;
		}
		set
		{
			_current = value;
		}
	}

	public LevelData.IDKey CurrentLevelKey = new LevelData.IDKey(0);

	public Dictionary<string, bool> CountryData = new Dictionary<string, bool>()
	{
		{ "Test", false},
		{ "Face", false}
	};

	//! needs to be managed. cannot do this as it is passing a referance.
	public Dictionary<int, QuestManager.QuestInfo> Quests;


	public override string ToString()
	{
		string textToDisplay = "";

		textToDisplay += CurrentLevelKey.ToString();

		textToDisplay += $"\n";



		textToDisplay += $"\nDictionary<string, bool> CountryData\n{{";

		foreach (string Country in CountryData.Keys)
		{
			textToDisplay += $"\n	{{{Country}, {CountryData[Country].ToString()}}}";
		}

		textToDisplay += $"\n}}\n";



		QuestManager.CopyQuests(QuestManager.QuestDefualt, ref Quests);

		textToDisplay += $"\nDictionary<int, QuestManager.QuestInfo> Quests\n{{";

		foreach (int quest in Quests.Keys)
		{
			textToDisplay += $"\n	{{{quest}, {Quests[quest].ToString()}}}";
		}

		textToDisplay += $"\n}}\n";



		return textToDisplay;
	}
}