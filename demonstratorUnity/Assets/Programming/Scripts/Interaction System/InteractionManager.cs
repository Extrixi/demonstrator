using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
	public float MaxInteractionDistance = 5f;

	private Transform InteractableItem = null;

	// Start is called before the first frame update
	void Start()
	{

	}

	void Update()
	{
		if (InteractableItem != null)
		{


			if (Input.GetKeyDown(KeyCode.E))
			{
				InteractableItem.GetComponent<IInteractable>().OnInteract();
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		// if (InteractableItem != null)
		// {
		// 	InteractableItem.GetComponent<Outline>().OutlineWidth = 0f;
		// }

		Collider[] PotentialInteractableItems = Physics.OverlapSphere(transform.position, MaxInteractionDistance);

		List<Transform> InteractableItemsTransform = new List<Transform>();

		for (int i = 0; i < PotentialInteractableItems.Length; i++)
		{
			if (PotentialInteractableItems[i].transform.tag != "Interactable") continue;

			InteractableItemsTransform.Add(PotentialInteractableItems[i].transform);

		}

		if (InteractableItemsTransform.Count <= 0)
		{
			DeSelectItem();
			return;

		}

		if (InteractableItemsTransform.Count == 1)
		{
			SelectItem(InteractableItemsTransform[0]);
			return;
		}

		Transform ClosestOne = null;

		float ClosestDistance = Mathf.Infinity;

		foreach (Transform interactableTransform in InteractableItemsTransform.ToList<Transform>())
		{
			if (ClosestOne == null)
			{
				ClosestOne = interactableTransform;
				ClosestDistance = Vector3.Distance(transform.position, ClosestOne.position);
			}
			else if (Vector3.Distance(transform.position, interactableTransform.position) < ClosestDistance)
			{
				ClosestOne = interactableTransform;
				ClosestDistance = Vector3.Distance(transform.position, ClosestOne.position);
			}
		}

		if (ClosestOne == null)
		{
			DeSelectItem();
		}
		else
		{

			SelectItem(ClosestOne);
		}


	}

	public void SelectItem(Transform itemToSelect)
	{
		if (InteractableItem != null) DeSelectItem();

		InteractableItem = itemToSelect;

		InteractableItem.GetComponent<Outline>().OutlineWidth = 10f;
	}

	public void DeSelectItem()
	{
		if (InteractableItem != null) InteractableItem.GetComponent<Outline>().OutlineWidth = 0f;
		InteractableItem = null;
	}
}
