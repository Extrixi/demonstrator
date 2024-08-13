using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class InteractableObject : MonoBehaviour, IInteractable
{

	public UnityEvent OnInteractEvent;

	public Color OutlineColour = Color.yellow;
	public float OutlineWidth = 10f;

	private Outline ObjectOutline;

	void Awake()
	{
		ObjectOutline = GetComponent<Outline>();

		ObjectOutline.OutlineWidth = 0f;
		ObjectOutline.OutlineColor = OutlineColour;

		transform.tag = "Interactable";
	}

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnInteract()
	{
		OnInteractEvent.Invoke();
	}

	public void OnSelect()
	{
		ObjectOutline.OutlineWidth = OutlineWidth;
		ObjectOutline.OutlineColor = OutlineColour;
	}

	public void OnDeSelect()
	{
		ObjectOutline.OutlineWidth = 0f;

	}
}
