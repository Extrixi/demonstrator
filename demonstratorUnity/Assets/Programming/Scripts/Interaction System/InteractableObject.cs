using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class InteractableObject : MonoBehaviour, IInteractable
{

	public UnityEvent OnInteractEvent;


	private Outline ObjectOutline;

	void Awake()
	{
		ObjectOutline = GetComponent<Outline>();

		ObjectOutline.OutlineWidth = 0f;

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

}
