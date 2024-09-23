using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interactable object for use in scenes. This allows other scripts and classes functions 
/// to be triggers with a event when interaction manager interacts with this.
/// </summary>
[RequireComponent(typeof(Outline))]
public class InteractableObject : MonoBehaviour, IInteractable
{

	public UnityEvent OnInteractEvent;

	public Color OutlineColour = Color.yellow;
	public float OutlineWidth = 10f;

	private Outline ObjectOutline;

	// ? what is this? why is this needed?
	public string HintText;

	public string InteractionHint
	{
		get => HintText;

		set => HintText = value;
	}

	void Awake()
	{
		ObjectOutline = GetComponent<Outline>();

		ObjectOutline.OutlineWidth = 0f;
		ObjectOutline.OutlineColor = OutlineColour;

		transform.tag = "Interactable";
	}

	/// <summary>
	/// Used to invoke the OnInteractEvent.
	/// </summary>
	public void OnInteract()
	{
		OnInteractEvent.Invoke();
	}

	/// <summary>
	/// Called when interaction manager has selected this item. (more like hovering)
	/// </summary>
	public void OnSelect()
	{
		ObjectOutline.OutlineWidth = OutlineWidth;
		ObjectOutline.OutlineColor = OutlineColour;
	}

	/// <summary>
	/// When the interaction manager deselects this interactable item.
	/// </summary>
	public void OnDeSelect()
	{
		ObjectOutline.OutlineWidth = 0f;

	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
