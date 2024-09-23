using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;




#if UNITY_EDITOR
// ! ===============================
using UnityEditor;

[CustomEditor(typeof(SaveDy)), CanEditMultipleObjects]
public class UIForSaveDy : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		GUILayoutOption[] option = new GUILayoutOption[]
		{
			GUILayout.Height(100)
		};

		SaveDy myScript = (SaveDy)target;
		if (GUILayout.Button("DecriptSave", option)) myScript.DycriptSave();


		if (GUILayout.Button("DeleteSave", option)) myScript.DeleteSave();
	}
}
// ! ===============================
#endif

/// <summary>
/// Save File Dycrypt. (SFD)
/// </summary>
public class SaveDy : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void DycriptSave()
	{
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}

		// string save = "0.save";
		string path = Application.persistentDataPath + "/saves/";

		// FileStream file = File.Create(Path.Combine(path, save));

		if (!File.Exists(Application.persistentDataPath + "/saves/0.save"))
		{

			print("No Save File");
			return;
		}

		SaveData saveData = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/0.save");


		using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "Decrypt.txt")))
		{

			outputFile.Write(saveData.ToString());

			outputFile.Close();
		}

		print("Decrypted Save File");
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


	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
