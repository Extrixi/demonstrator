using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookController : MonoBehaviour
{
	public bool IsEnabled = true;

	public bool IsLocked = false;


	public Transform CameraHolder;

	public float Sense = 1f;

	private float _yRotation;

	private Camera cam;




	// Start is called before the first frame update
	void Start()
	{
		cam = Camera.main;
	}

	// Update is called once per frame
	void Update()
	{
		cam.enabled = IsEnabled;

		if (IsLocked) return;

		// TODO very simple 3rd person cam. planning to replace with the cinimachine version. this sucks.

		// dont bother. verysimple rotate camera based on inputs from mouse axis.

		Vector2 MouseLook = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		_yRotation -= MouseLook.y * Sense;

		_yRotation = Mathf.Clamp(_yRotation, -90, 90);

		CameraHolder.Rotate(transform.up * MouseLook.x * Sense);

		CameraHolder.rotation = Quaternion.Euler(_yRotation, CameraHolder.rotation.eulerAngles.y, 0);
	}
}
