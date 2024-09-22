using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System;

/// <summary>
/// This handles loading and saving the save data to the save file.
/// </summary>

[DefaultExecutionOrder(-5)]
public class SaveManager : MonoBehaviour
{
	public static SaveManager current;

	public string saveName = "0";

	/// <summary>
	/// C# event
	/// </summary>
	public event Action onSave;
	public void GameSaveInvoke()
	{
		if (onSave != null)
		{
			onSave();
		}
	}

	public event Action onLoad;
	public void GameLoadInvoke()
	{
		if (onSave != null)
		{
			onLoad();
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
			DontDestroyOnLoad(this);
		}

		if (SerializationManager.Load(Application.persistentDataPath + "/saves/0.save") == null)
		{
			SaveData.current = new SaveData();
			SerializationManager.Save("0", SaveData.current);
		}
		else
		{
			SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/0.save");
			GameLoadInvoke();
		}
	}

	/// <summary>
	/// This should be called to save not the SerializationManager.
	/// This has a event tied to it.
	/// </summary>
	public void ForceSave()
	{
		if (SerializationManager.Load(Application.persistentDataPath + "/saves/0.save") == null) { SaveData.current = new SaveData(); }
		SerializationManager.Save(saveName, SaveData.current);
		GameSaveInvoke();
	}

	public void ForceLoad()
	{
		SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/0.save");
		GameLoadInvoke();
	}

	public void DeleteSave()
	{
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			print("Nothing to delete");

			return;
		}

		if (!File.Exists(Application.persistentDataPath + "/saves/0.save"))
		{

			print("No Save File");
			return;
		}

		string path = Application.persistentDataPath + "/saves/0.save";
		File.Delete(path);

		print("Deleted Save File");

		ForceSave();

	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
