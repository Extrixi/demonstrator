using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles interacting with interactable items.
/// </summary>
public class InteractionManager : MonoBehaviour
{
	public float MaxInteractionDistance = 5f;

	private Transform InteractableItem = null;


	void Update()
	{
		if (InteractableItem != null && Input.GetKeyDown(KeyCode.E))
		{
			InteractableItem.GetComponent<IInteractable>().OnInteract();
		}
	}


	void FixedUpdate()
	{
		List<Transform> InteractableItemsTransform;
		GetNearbyInteractableItems(out InteractableItemsTransform, MaxInteractionDistance);

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

		SelectClosetItem(InteractableItemsTransform);

	}

	/// <summary>
	/// Gets selects the closets item, if there are none, then it will deselect (null).
	/// </summary>
	/// <param name="InteractableItemsTransform">The list of transforms of interactable items, this should ONLY contain interactable items.</param>
	private void SelectClosetItem(List<Transform> InteractableItemsTransform)
	{
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

	/// <summary>
	/// Gets the nearby interactable items around the player. Can be null.
	/// </summary>
	/// <param name="InteractableItemsTransform">The list to put all interactable item transforms.</param>
	/// <param name="maxInteractionDistance">The radius to check around the player. </param>
	private void GetNearbyInteractableItems(out List<Transform> InteractableItemsTransform, float maxInteractionDistance)
	{
		// A low of resouces per second. 50fps for fixed update.
		Collider[] PotentialInteractableItems = Physics.OverlapSphere(transform.position, maxInteractionDistance);

		InteractableItemsTransform = new List<Transform>();
		for (int i = 0; i < PotentialInteractableItems.Length; i++)
		{
			if (PotentialInteractableItems[i].transform.tag != "Interactable") continue;

			InteractableItemsTransform.Add(PotentialInteractableItems[i].transform);

		}
	}

	/// <summary>
	/// Selects a interactable item.
	/// </summary>
	/// <param name="itemToSelect">The transform of a known interactable item.</param>
	public void SelectItem(Transform itemToSelect)
	{
		if (InteractableItem != null) DeSelectItem();

		InteractableItem = itemToSelect;

		InteractableItem.GetComponent<IInteractable>().OnSelect();
	}

	/// <summary>
	/// Deselects the currently selected interactable item.
	/// </summary>
	public void DeSelectItem()
	{
		if (InteractableItem != null) InteractableItem.GetComponent<IInteractable>().OnDeSelect();
		InteractableItem = null;
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
