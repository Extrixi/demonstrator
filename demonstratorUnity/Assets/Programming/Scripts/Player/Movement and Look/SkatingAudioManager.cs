using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkatingAudioManager : MonoBehaviour
{
	public AudioSource SkateAudioSource;

	MovementController _movementController;
	Rigidbody _rb;

	// Start is called before the first frame update
	void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_movementController = GetComponent<MovementController>();

		SkateAudioSource.loop = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (!_movementController.isGrounded)
		{
			SkateAudioSource.Stop();
			return;
		}

		Vector3 vel = _rb.velocity;
		vel.y = 0;

		if (vel.magnitude <= 0)
		{
			SkateAudioSource.Stop();
			return;
		}


		if (!SkateAudioSource.isPlaying)
		{
			SkateAudioSource.Play();
		}


		// modified lerp to get desired result.
		SkateAudioSource.pitch = (0.5f + (2f - 0.5f)) * vel.magnitude / (_movementController.TargetSpeed * 2f);
	}
}
