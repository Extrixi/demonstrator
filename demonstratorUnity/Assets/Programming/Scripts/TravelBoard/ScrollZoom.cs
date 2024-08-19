using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
