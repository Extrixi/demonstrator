using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkatingAudioManager : MonoBehaviour
{
	public AudioSource SkateAudioSource;

	// refs.
	MovementController _movementController;

	Rigidbody _rb;

	void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_movementController = GetComponent<MovementController>();
	}

	// Start is called before the first frame update
	void Start()
	{
		// presuming that SkateAudioSource is not null.
		SkateAudioSource.loop = true;
	}

	// Update is called once per frame
	void Update()
	{
		// if we are not on the floor. no need to play sound.
		if (!_movementController.isGrounded)
		{
			SkateAudioSource.Stop();
			return;
		}

		// we get the velocity, so we can modulate the sound.
		Vector3 vel = _rb.velocity;
		// ? do we need to remove y, what about slopes?
		//vel.y = 0;

		// if the player is not moving, then dont play any sound.
		if (vel.magnitude <= 0)
		{
			SkateAudioSource.Stop();
			return;
		}

		// if we are not playing the sound, play it.
		if (!SkateAudioSource.isPlaying)
		{
			SkateAudioSource.Play();
		}


		// (0.5f + (2f - 0.5f)) this is a lerp fyi.
		// this sets the pitch based on the speed of the player.
		SkateAudioSource.pitch = (0.5f + (2f - 0.5f)) * vel.magnitude / (_movementController.MaxAccelerationSpeed * 2f);
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
