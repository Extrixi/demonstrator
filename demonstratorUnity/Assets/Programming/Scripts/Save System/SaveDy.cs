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
	}
}
// ! ===============================
#endif


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

		SaveData saveData = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/0.save");


		using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "Decrypt.txt")))
		{

			outputFile.Write(saveData.ToString());

			outputFile.Close();
		}

		print("Decrypted Save File");
	}
}
