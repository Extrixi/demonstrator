using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{

	public void OnInteract();

	public string InteractionHint { get; set; }

	public void OnSelect();

	public void OnDeSelect();
}
