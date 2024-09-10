using System;
using UnityEditor;
using UnityEngine.UIElements;

public class DSEditorWindow : EditorWindow
{
	[MenuItem("Window/DS/Dialogue Window")]
	public static void ShowExample()
	{
		GetWindow<DSEditorWindow>("Dialogue Graph");
	}

	void CreateGUI()
	{
		AddGraphView();

		AddStyles();
	}


	#region Element Addition
	private void AddStyles()
	{
		StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DSVariables.uss");

		rootVisualElement.styleSheets.Add(styleSheet);
	}

	void AddGraphView()
	{
		DSGraphView graphView = new DSGraphView();

		graphView.StretchToParentSize();

		rootVisualElement.Add(graphView);
	}
	#endregion


}
