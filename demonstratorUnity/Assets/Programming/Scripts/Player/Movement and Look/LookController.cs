using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookController : MonoBehaviour
{
	public Transform CameraHolder;

	public float Sense = 1f;

	private float _yRotation;


	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		Vector2 MouseLook = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		_yRotation -= MouseLook.y * Sense;

		_yRotation = Mathf.Clamp(_yRotation, -90, 90);

		CameraHolder.Rotate(transform.up * MouseLook.x * Sense);

		CameraHolder.rotation = Quaternion.Euler(_yRotation, CameraHolder.rotation.eulerAngles.y, 0);
	}
}
