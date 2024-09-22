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

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|