using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;


public class MovementController : MonoBehaviour
{
	[Header("If this script can run and shows the player")]
	public bool IsEnabled = true;

	[Header("If the movement is frozen")]
	public bool IsLocked = false;

	[Header("Movement")]
	public float MaxAccelerationSpeed = 18;

	[SerializeField]
	private float _accellerationRate = 0.4f;

	[SerializeField]
	private float _deaccellerationRate = 0.5f;

	[SerializeField]
	private float _minimumStopSpeed = 0.1f;



	[Header("Rotation Refs")]
	public Transform Orientation;

	public Transform _playerModel;

	[Header("Rotation Vars")]
	[SerializeField]
	private float _turnForce = 10f;

	[SerializeField]
	private float _maxTurnSpeed = 0.5f;

	[SerializeField]
	private float _minTurnSpeed = 0.5f;

	[SerializeField]
	private float _airTurnSpeed = 2f;

	// Data storage
	private float _currentTurnSpeed = 1f;



	// physics?
	[Header("Physics")]
	public float JumpForce = 3f;


	[Header("Checks")]
	[SerializeField]
	private float _groundRaycastHeight = 1.1f;


	[Space, Header("Result checks, Don't modify")]
	public bool isGrounded = false;

	public bool _onSlope = false;


	// Private refs
	private GrindSystem _grindSystem;

	private Rigidbody _rb;

	private RaycastHit _hit;

	private CapsuleCollider _capsuleCollider;














	void Awake()
	{
		_rb = GetComponent<Rigidbody>();

		_grindSystem = GetComponent<GrindSystem>();

		_capsuleCollider = GetComponent<CapsuleCollider>();
	}

	void Update()
	{
		_playerModel.gameObject.SetActive(IsEnabled);

		if (IsLocked) return;

		if (_grindSystem.IsGrinding) return;

		GroundCheck();

		SlopeCheck();

		RotatePlayerWithSlopeAngle();

		HandleJumping();

		HandleCrouching();
	}

	void FixedUpdate()
	{
		if (!IsEnabled || IsLocked) return;

		if (_grindSystem.IsGrinding) return;


		Vector3 wishDir = GetInputAsVector3();

		// the current turn speed based on current player velocity.
		_currentTurnSpeed = Mathf.Lerp(_maxTurnSpeed, _minTurnSpeed, _rb.velocity.magnitude / MaxAccelerationSpeed);








		// We know the player wants to go forward and is grounded, we can add a forward force, like pushing yourself on the skateboard.
		if (wishDir.z > 0 && isGrounded)
		{
			// we get a forward vector based on the player and convert it into an acceleration.
			// we get the required force needed to reach the target speed with current velocity then we reduce it so it accelerates.
			Vector3 direction = Orientation.forward;

			AccelerateInDirection(direction);

		}
		// If the player is pressing the S key and we are on the ground, we will slow down.
		else if (wishDir.z < 0 && isGrounded)
		{
			if (_rb.velocity.z <= 0)
			{
				Vector3 direction = -(Orientation.forward);


				AccelerateInDirection(direction);
			}
			else if (_rb.velocity.z > 0)
			{
				// we get the invert vector of the current velocity, then reduce the vector so it deaccelerates the player.
				Vector3 targ = -_rb.velocity * _deaccellerationRate;

				if (!_onSlope)
				{
					// dont effect the Y.
					targ.y = 0;
				}

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

		}



		// rotate the player and add fore. A and D.
		if (wishDir.x != 0 && isGrounded)
		{
			// We rotate the orientation.
			Orientation.Rotate(Vector3.up * wishDir.x * _currentTurnSpeed);

			// we get the vector that is |_ => / a diagonal from right and forward. then add a force. 
			// want to make this into a lerp between a forward and side vector.
			//Vector3 targ = (Orientation.right * wishDir.x * _turnForce + Orientation.forward * _turnForce).normalized * _rb.velocity.magnitude - _rb.velocity;


			if (_rb.velocity.z > 0)
				_rb.velocity = _playerModel.forward * _rb.velocity.magnitude;
			else
				_rb.velocity = -_playerModel.forward * _rb.velocity.magnitude;



			// DONT EFFECT Y.
			// targ.y = 0;

			// add the force to the player.
			// _rb.AddForce(targ, ForceMode.Force);
		}


		Vector3 calcVel = _rb.velocity;
		calcVel.y = 0;
		// ! HERE needs fixing, sets velocity forward.

		if (isGrounded && !_onSlope && Vector3.Dot(calcVel, Orientation.forward) < 0.9f)
		{
			float y = _rb.velocity.y;

			_rb.velocity = Orientation.forward * calcVel.magnitude + new Vector3(0, y, 0);

		}





	}

	private void AccelerateInDirection(Vector3 direction)
	{
		Vector3 targ = ((direction * MaxAccelerationSpeed) - _rb.velocity) * _accellerationRate;

		if (!_onSlope)
		{
			// we dont want to effect the y. otherwise we fly or go down to hell.
			targ.y = 0;
		}

		// we add the fore to the player.
		_rb.AddForce(targ, ForceMode.Force);
	}

	private static Vector3 GetInputAsVector3()
	{
		// the target direction from the inputs.
		return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
	}

	private void HandleCrouching()
	{
		if (Input.GetKey(KeyCode.LeftControl))
		{
			_capsuleCollider.center = new Vector3(0f, -0.5f, 0f);
			_capsuleCollider.height = 1;
		}
		else
		{
			_capsuleCollider.center = new Vector3(0f, 0f, 0f);
			_capsuleCollider.height = 2;
		}
	}

	private void HandleJumping()
	{
		if (isGrounded && Input.GetKeyDown(KeyCode.Space))
		{
			//rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

			Vector3 addition = new Vector3(0, -_rb.velocity.y, 0);

			Vector3 Force = new Vector3();

			if (_rb.velocity.y < 0)
			{
				Force = addition + Orientation.up * JumpForce;
			}
			else
			{
				Force = Orientation.up * JumpForce;
			}



			_rb.AddForce(Force, ForceMode.Impulse);

			print("jumped");
		}
	}

	private void RotatePlayerWithSlopeAngle()
	{
		if (isGrounded && !_onSlope)
		{

			_playerModel.localRotation = Quaternion.FromToRotation(Vector3.up, _hit.normal);

			Orientation.localRotation = Quaternion.Euler(0, Orientation.localRotation.eulerAngles.y, 0);

		}
		else if (isGrounded && _onSlope)
		{

			_playerModel.rotation = (Quaternion.FromToRotation(Vector3.up, _hit.normal)) * Orientation.rotation;
		}
		else if (!isGrounded && !_onSlope && false) // ! disabled rotating in air.
		{
			Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

			//_playerModel.localRotation = Quaternion.Euler(_playerModel.localRotation.eulerAngles.x, 0, 0);
			_playerModel.Rotate(Vector3.right * wishDir.z * _airTurnSpeed);
			Orientation.Rotate(Vector3.up * wishDir.x * _airTurnSpeed);
		}
	}

	private void SlopeCheck()
	{
		if (isGrounded)
		{
			_onSlope = Vector3.Dot(_hit.normal, Vector3.up) < 1;
		}
		else
		{
			_onSlope = false;
		}
	}

	private void GroundCheck()
	{
		if (!_onSlope)
		{

			isGrounded = Physics.Raycast(_rb.position, Vector3.down, out _hit, _groundRaycastHeight);
		}
		else
		{
			isGrounded = Physics.Raycast(_playerModel.position, -_playerModel.transform.up, out _hit, _groundRaycastHeight);
		}
	}



}
