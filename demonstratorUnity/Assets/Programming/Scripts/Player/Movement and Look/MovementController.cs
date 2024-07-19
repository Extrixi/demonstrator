using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	[SerializeField]
	private Transform _orientation;

	[SerializeField]
	private float _targetSpeed = 30;

	[SerializeField]
	private float _accellerationRate = 0.5f;

	[SerializeField]
	private float _deAccellerationRate = 0.5f;

	[SerializeField]
	private float _turnForce = 10f;

	[SerializeField]
	private float _minimumStopSpeed = 0.1f;

	[SerializeField]
	private float _turnSpeed = 0.5f;

	private bool _isGrounded = false;

	private Rigidbody rb;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		RaycastHit hit;
		_isGrounded = Physics.Raycast(rb.position, Vector3.down, out hit, 1.1f);




		Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		// we want to go forward.
		// so we add a force to the player.
		if (wishDir.z > 0 && _isGrounded)
		{

			Vector3 targ = ((_orientation.forward * _targetSpeed) - rb.velocity) * _accellerationRate;

			// we dont want to effect the y.
			targ.y = 0;

			rb.AddForce(targ, ForceMode.Force);



		}
		else if (wishDir.z < 0 && _isGrounded)
		{
			Vector3 targ = -rb.velocity * _deAccellerationRate;

			targ.y = 0;

			if (rb.velocity.magnitude < _minimumStopSpeed)
			{
				rb.velocity = new Vector3(0, rb.velocity.y, 0);
			}
			else
			{
				rb.AddForce(targ, ForceMode.Force);
			}

		}

		// rotate the player
		if (wishDir.x != 0 && _isGrounded)
		{

			_orientation.Rotate(_orientation.up * wishDir.x * _turnSpeed);

			Vector3 targ = (_orientation.right * wishDir.x * _turnForce + _orientation.forward * _turnForce).normalized * rb.velocity.magnitude - rb.velocity;

			targ.y = 0;


			rb.AddForce(targ, ForceMode.Force);
		}

		if (_isGrounded && Vector3.Dot(hit.normal, Vector3.up) > 0.99f)
		{
			_orientation.rotation = Quaternion.Euler(0, _orientation.eulerAngles.y, 0);
		}
		else if (_isGrounded)
		{
			_orientation.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		}

		print(Vector3.Dot(hit.normal, Vector3.up));

		//print(rb.velocity + " | " + rb.velocity.magnitude);

	}
}
