using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	[SerializeField]
	private Transform _orientation;

	[SerializeField]
	private float _TargetSpeed = 50;

	private Rigidbody rb;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		// we want to go forward.
		// so we add a force to the player.
		if (wishDir.z > 0)
		{

			Vector3 targ = (_orientation.forward * _TargetSpeed) - rb.velocity;

			targ *= 0.5f;


			rb.AddForce(targ, ForceMode.Force);

			print(rb.velocity);
		}

		// rotate the player
		if (wishDir.x != 0)
		{

			_orientation.Rotate(_orientation.up * wishDir.x * 0.5f);
		}

		// wishDir = wishDir.normalized * _TargetSpeed;

		// Vector3 targetVel = wishDir - rb.velocity;


	}
}
