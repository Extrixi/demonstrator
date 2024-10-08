using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DSGraphView : GraphView
{
	public DSGraphView()
	{
		AddManipulators();
		AddGridBackground();

		//CreateNode();

		AddStyles();
	}

	#region Overrided Methods
	public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
	{
		List<Port> compatiblePorts = new List<Port>();

		ports.ForEach(port =>
		{
			if (startPort == port)
			{
				return;
			}

			if (startPort.node == port.node)
			{
				return;
			}

			if (startPort.direction == port.direction)
			{
				return;
			}

			compatiblePorts.Add(port);

		});


		return compatiblePorts;
	}
	#endregion


	#region AddManipulators
	private void AddManipulators()
	{
		SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

		this.AddManipulator(new ContentDragger());

		this.AddManipulator(new SelectionDragger());
		this.AddManipulator(new RectangleSelector());

		this.AddManipulator(CreateNodeContextMenu("Add Node (Single Choice)", DSDialogueType.SingleChoice));
		this.AddManipulator(CreateNodeContextMenu("Add Node (Multiple Choice)", DSDialogueType.MultipleChoice));

		this.AddManipulator(CreateGroupContextualMenu());

	}

	private IManipulator CreateGroupContextualMenu()
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
			menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup("DialogueGroup", actionEvent.eventInfo.localMousePosition)))
		);

		return contextualMenuManipulator;
	}

	private IManipulator CreateNodeContextMenu(string actionTitle, DSDialogueType dialogueType)
	{
		ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
			menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, actionEvent.eventInfo.localMousePosition)))
		);

		return contextualMenuManipulator;
	}
	#endregion


	#region Elements Creation
	private Group CreateGroup(string title, Vector2 localMousePosition)
	{
		Group group = new Group()
		{
			title = title
		};

		group.SetPosition(new Rect(localMousePosition, Vector2.zero));

		return group;
	}

	private DSNode CreateNode(DSDialogueType dialogueType, Vector2 position)
	{
		Type nodeType = Type.GetType($"DS{dialogueType}Node");

		DSNode node = (DSNode)Activator.CreateInstance(nodeType);

		node.Initialize(position);

		node.Draw();

		return node;
	}
	#endregion


	#region Elements Addition
	private void AddStyles()
	{
		StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DSGraphViewStyles.uss");
		StyleSheet nodeStyleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DSNodeStyles.uss");

		styleSheets.Add(styleSheet);
		styleSheets.Add(nodeStyleSheet);
	}

	private void AddGridBackground()
	{
		GridBackground gridBackground = new GridBackground();

		gridBackground.StretchToParentSize();

		Insert(0, gridBackground);
	}
	#endregion

}
