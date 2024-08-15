using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
	private static SaveData _current;
	public static SaveData Current
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
}