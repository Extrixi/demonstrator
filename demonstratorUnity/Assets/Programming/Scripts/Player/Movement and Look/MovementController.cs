using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	public bool IsEnabled = true;

	public bool IsLocked = false;


	[SerializeField]
	private Transform _orientation;

	[SerializeField]
	private Transform _playerModel;

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
	private float _maxTurnSpeed = 0.5f;

	[SerializeField]
	private float _minTurnSpeed = 0.5f;

	private float _currentTurnSpeed = 1f;

	[SerializeField]
	private float _airTurnSpeed = 2f;

	[SerializeField]
	private float _jumpForce = 3f;

	private bool _isGrounded = false;

	private Rigidbody _rb;

	private RaycastHit _hit;

	// Start is called before the first frame update
	void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		_playerModel.gameObject.SetActive(IsEnabled);

		if (IsLocked) return;

		// This determins where the raycast comes from. Because the player has "two"  points to rotate.
		// The first is the player rotation of the model, we can raycast straight down from the player's perspective.
		// We dont want to roate the player in the air so we can have air controls.
		// But if the player was in the air and touches the ground, we need to raycast into the ground, because the player could be upside down.
		if (_isGrounded)
		{
			// we also store a hit. this is used to rotated the player and for slope calcs.
			_isGrounded = Physics.Raycast(_playerModel.position, -_playerModel.transform.up, out _hit, 1.1f);
		}
		else
		{
			_isGrounded = Physics.Raycast(_rb.position, Vector3.down, out _hit, 1.1f);
		}

		if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
		{
			//rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

			Vector3 addition = new Vector3(0, -_rb.velocity.y, 0);

			Vector3 Force = new Vector3();

			if (_rb.velocity.y < 0)
			{
				Force = addition + _orientation.up * _jumpForce;
			}
			else
			{
				Force = _orientation.up * _jumpForce;
			}



			_rb.AddForce(Force, ForceMode.Impulse);

			print("jumped");
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!IsEnabled || IsLocked) return;



		// the target direction from the inputs.
		Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		// the current turn speed based on current player velocity.
		_currentTurnSpeed = Mathf.Lerp(_maxTurnSpeed, _minTurnSpeed, _rb.velocity.magnitude / _targetSpeed);

		// funny math, could be used later.
		//print(Mathf.Abs(Mathf.Log(rb.velocity.magnitude / _targetSpeed)) / 10f);


		// We know the player wants to go forward and is grounded, we can add a forward force, like pushing yourself on the skateboard.
		if (wishDir.z > 0 && _isGrounded)
		{
			// we get a forward vector based on the player and convert it into an acceleration.
			// we get the required force needed to reach the target speed with current velocity then we reduce it so it accelerates.
			Vector3 targ = ((_orientation.forward * _targetSpeed) - _rb.velocity) * _accellerationRate;

			// we dont want to effect the y. otherwise we fly or go down to hell.
			targ.y = 0;

			// we add the fore to the player.
			_rb.AddForce(targ, ForceMode.Force);



		}
		// If the player is pressing the S key and we are on the ground, we will slow down.
		else if (wishDir.z < 0 && _isGrounded)
		{
			// we get the invert vector of the current velocity, then reduce the vector so it deaccelerates the player.
			Vector3 targ = -_rb.velocity * _deAccellerationRate;

			// dont effect the Y.
			targ.y = 0;

			// if the speed is below the minimum stop speed, then just reset the velocity.
			// Otherwise, just add the deacceleration speed to the player.
			if (_rb.velocity.magnitude < _minimumStopSpeed)
			{
				_rb.velocity = new Vector3(0, _rb.velocity.y, 0);
			}
			else
			{
				_rb.AddForce(targ, ForceMode.Force);
			}

		}

		// rotate the player and add fore. A and D.
		if (wishDir.x != 0 && _isGrounded)
		{
			// We rotate the orientation.
			_orientation.Rotate(Vector3.up * wishDir.x * _currentTurnSpeed);

			// we get the vector that is |_ => / a diagonal from right and forward. then add a force. 
			// want to make this into a lerp between a forward and side vector.
			Vector3 targ = (_orientation.right * wishDir.x * _turnForce + _orientation.forward * _turnForce).normalized * _rb.velocity.magnitude - _rb.velocity;

			// DONT EFFECT Y.
			targ.y = 0;

			// add the force to the player.
			_rb.AddForce(targ, ForceMode.Force);
		}




		// if we are grounded we can rotate the player's model to the angle of the floor.
		// if not, the player can rotate the player character in the air.
		if (_isGrounded)
		{
			_playerModel.localRotation = Quaternion.FromToRotation(Vector3.up, _hit.normal);
		}
		else
		{
			_playerModel.Rotate(Vector3.right * wishDir.z * _airTurnSpeed + Vector3.forward * wishDir.x * _airTurnSpeed + Vector3.up * wishDir.x * _airTurnSpeed);
		}

	}
}
