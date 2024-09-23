using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookController : MonoBehaviour
{
	[Header("If this script can run and enables the camera")]
	public bool IsEnabled = true;

	[Header("If the looking is frozen")]
	public bool IsLocked = false;


	public Transform CameraHolder;

	// TODO make this use save data sens so player can change and save it.
	public float Sensitivity = 1f;


	// refs.
	private float _yRotation;

	private Camera cam;



	void Awake()
	{
		cam = Camera.main;
	}

	// Update is called once per frame
	void Update()
	{
		cam.enabled = IsEnabled;

		if (IsLocked) return;



		// dont bother. verysimple rotate camera based on inputs from mouse axis.
		// I wont. :3

		Vector2 MouseLook = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		_yRotation -= MouseLook.y * Sensitivity;

		_yRotation = Mathf.Clamp(_yRotation, -90, 90);

		CameraHolder.Rotate(transform.up * MouseLook.x * Sensitivity);

		CameraHolder.rotation = Quaternion.Euler(_yRotation, CameraHolder.rotation.eulerAngles.y, 0);
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
