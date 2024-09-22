using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// allows scrolling and zoming on the travel board.
/// </summary>
public class ScrollZoom : MonoBehaviour
{
	public RectTransform ObjectToScale;

	public float ScrollSpeed = 2f;

	public Vector3 StartScale = Vector3.one;

	public Vector3 EndScale = new Vector3(3, 3, 3);

	private float _currentZoomAmmount = 0f;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{


		_currentZoomAmmount += Input.mouseScrollDelta.y * ScrollSpeed;

		_currentZoomAmmount = Mathf.Clamp(_currentZoomAmmount, 0, 1);

		ObjectToScale.localScale = Vector3.Lerp(StartScale, EndScale, _currentZoomAmmount);
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
