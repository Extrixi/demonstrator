using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
// ! ===============================
using UnityEditor;

[CustomEditor(typeof(OverideSaveData)), CanEditMultipleObjects]
public class OverideSaveDataButtons : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		OverideSaveData myScript = (OverideSaveData)target;

		if (GUILayout.Button("Force Save")) myScript.SaveNewSens();
	}
}
// ! ===============================
#endif

/// <summary>
/// Used to overide save data.
/// </summary>
public class OverideSaveData : MonoBehaviour
{

	[SerializeField] public float sensitivity = 1f;

	void Start()
	{

		// sensitivity = SaveData.current.Sensitivity;

		// event listener
		SaveManager.current.onSave += OnSaveGame;
	}

	public void OnSaveGame()
	{


		// sensitivity = SaveData.current.Sensitivity;

	}

	public void SaveNewSens()
	{
		// SaveData.current.Sensitivity = sensitivity;
		// SerializationManager.Save("0", SaveData.current);
		if (SaveManager.current == null)
		{
			print("Must be in runtime!");
			return;
		}
		SaveManager.current.ForceSave();
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|