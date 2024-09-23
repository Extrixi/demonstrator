using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Handles the movement of the player.
/// </summary>
public class MovementController : MonoBehaviour
{
	#region Variables
	[Header("If this script can run and shows the player")]
	public bool IsEnabled = true;

	[Header("If the movement is frozen")]
	public bool IsLocked = false;


	#region Movement
	[Header("Movement")]
	public float MaxAccelerationSpeed = 18;

	[SerializeField]
	private float _accellerationRate = 0.4f;

	[SerializeField]
	private float _deaccellerationRate = 0.5f;

	[SerializeField]
	private float _minimumStopSpeed = 0.1f;
	#endregion


	#region Rotation
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

	// Used to calculate the speed to turn at current speed.
	private float _currentTurnSpeed = 1f;
	#endregion


	#region Physics
	[Header("Physics")]
	public float JumpForce = 3f;


	[Header("Checks")]
	[SerializeField]
	private float _groundRaycastHeight = 1.1f;


	[Space, Header("Result checks, Don't modify")]
	public bool isGrounded = false;

	public bool _onSlope = false;
	#endregion


	#region Chached Variables 
	// Private refs - chached data.
	private GrindSystem _grindSystem;

	private Rigidbody _rb;

	private RaycastHit _hit;

	private CapsuleCollider _capsuleCollider;
	#endregion
	#endregion


	#region Awake
	void Awake()
	{
		// get the references.
		_rb = GetComponent<Rigidbody>();

		_grindSystem = GetComponent<GrindSystem>();

		_capsuleCollider = GetComponent<CapsuleCollider>();
	}
	#endregion


	#region Update
	void Update()
	{
		// set the visibility of the player.
		_playerModel.gameObject.SetActive(IsEnabled);

		// Guard clause - We dont need to run if we are locked.
		if (IsLocked) return;

		// Guard clause - If we are grinding, we dont want to be able to move as well. 
		// (movement on rails are handled in the grind system).
		if (_grindSystem.IsGrinding) return;

		GroundCheck();

		SlopeCheck();

		RotatePlayerWithSlopeAngle();

		HandleJumping();

		HandleCrouching();
	}
	#endregion


	#region FixedUpdate
	void FixedUpdate()
	{
		if (!IsEnabled || IsLocked) return;

		if (_grindSystem.IsGrinding) return;


		Vector3 wishDir = GetInputAsVector3();

		CalculateTurnSpeed();

		RotatePlayer(wishDir.x);

		MovePlayerZAxis(wishDir.z);

		AlignMomentumWithSkateboard();

	}
	#endregion


	#region GroundCheck
	/// <summary>
	/// Checks if the player is touching ground.
	/// </summary>
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
	#endregion


	#region SlopeCheck
	/// <summary>
	/// Checks if the player is on a slope and chaches the slope's normal.
	/// </summary>
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
	#endregion


	#region RotatePlayerWithSlopeAngle
	/// <summary>
	/// Rotates the player according to the slope angle, and reset the rotation when on the ground.
	/// </summary>
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
		// else if (!isGrounded && !_onSlope && false) // * disabled rotating in air.
		// {
		// 	Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		// 	//_playerModel.localRotation = Quaternion.Euler(_playerModel.localRotation.eulerAngles.x, 0, 0);
		// 	_playerModel.Rotate(Vector3.right * wishDir.z * _airTurnSpeed);
		// 	Orientation.Rotate(Vector3.up * wishDir.x * _airTurnSpeed);
		// }
	}
	#endregion


	#region HandleJumping
	/// <summary>
	/// Adds a upwards force that counteracts gravity.
	/// </summary>
	private void HandleJumping()
	{
		if (isGrounded && Input.GetKeyDown(KeyCode.Space))
		{
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
		}
	}
	#endregion


	#region HandleCrouching
	/// <summary>
	/// Changes the collider if the LeftControl key is being held.
	/// </summary>
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
	#endregion


	#region GetInputAsVector3
	/// <summary>
	/// Gets the WASD keys input as a normalised vector 3.
	/// </summary>
	/// <returns>Returns a vector 3 with X as A(-1) and D (1), and Z as W(1) and S(-1)</returns>
	private static Vector3 GetInputAsVector3()
	{
		// the target direction from the inputs.
		return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
	}
	#endregion


	#region MovePlayerZAxis
	/// <summary>
	/// Moves the player in the direction according to the zAxis. Z > 0 is forward and Z < 0 is backwards.
	/// </summary>
	/// <param name="zAxis">The Z axis of the input</param>
	private void MovePlayerZAxis(float zAxis)
	{
		if (zAxis > 0 && isGrounded)
		{
			// if (_rb.velocity.z >= 0)
			// {
			Vector3 direction = Orientation.forward;

			AccelerateInDirection(direction, MaxAccelerationSpeed, _accellerationRate);
			// }
			// else if (_rb.velocity.z < 0)
			// {
			// 	DeacceleratePlayer(_deaccellerationRate, _minimumStopSpeed);
			// }
		}


		else if (zAxis < 0 && isGrounded)
		{
			// if (_rb.velocity.z <= 0)
			// {
			Vector3 direction = -(Orientation.forward);

			AccelerateInDirection(direction.normalized, MaxAccelerationSpeed, _accellerationRate);
			// }
			// else if (_rb.velocity.z > 0)
			// {
			// 	DeacceleratePlayer(_deaccellerationRate, _minimumStopSpeed);
			// }

		}

		else if (zAxis < 0.01f && zAxis > -0.01f && isGrounded && _rb.velocity.magnitude < _minimumStopSpeed)
		{

			_rb.velocity = new Vector3(0, _rb.velocity.y, 0);
		}

	}
	#endregion


	#region AccelerateInDirection
	/// <summary>
	/// Accelerates the player in a specified direction.
	/// </summary>
	/// <param name="direction">The direction to accelerate towards.</param>
	/// <param name="targetSpeed">The target / max speed achivable.</param>
	/// <param name="accellerationRate">The rate the accelration will be applied (1 - Immedietly, 0.5 - Half).</param>
	private void AccelerateInDirection(Vector3 direction, float targetSpeed, float accellerationRate)
	{
		Vector3 targ = ((direction.normalized * targetSpeed) - _rb.velocity) * accellerationRate;

		if (!_onSlope)
		{
			// we dont want to effect the y. otherwise we fly or go down to hell.
			targ.y = 0;
		}

		// we add the fore to the player.
		_rb.AddForce(targ, ForceMode.Force);
	}
	#endregion


	#region DeacceleratePlayer
	/// <summary>
	/// Slows down the player gradually.
	/// </summary>
	/// <param name="deaccellerationRate">The rate to slow down the player (1 - Immedietly, 0.5 - Half).</param>
	private void DeacceleratePlayer(float deaccellerationRate, float minimumStopSpeed)
	{
		// we get the invert vector of the current velocity, then reduce the vector so it deaccelerates the player.
		Vector3 targ = -_rb.velocity * deaccellerationRate;

		if (!_onSlope)
		{
			// dont effect the Y.
			targ.y = 0;
		}

		// if the speed is below the minimum stop speed, then just reset the velocity.
		// Otherwise, just add the deacceleration speed to the player.
		if (_rb.velocity.magnitude < minimumStopSpeed)
		{
			_rb.velocity = new Vector3(0, _rb.velocity.y, 0);
		}
		else
		{
			_rb.AddForce(targ, ForceMode.Force);
		}
	}
	#endregion


	#region RotatePlayer
	/// <summary>
	/// Rotates the player depending on the X axis of movement.
	/// </summary>
	/// <param name="xAxisOfMovement">The X axis of the movement input.</param>
	private void RotatePlayer(float xAxisOfMovement)
	{
		// rotate the player and add fore. A and D.
		if (xAxisOfMovement != 0 && isGrounded)
		{
			// We rotate the orientation.
			Orientation.Rotate(Vector3.up * xAxisOfMovement * _currentTurnSpeed);

			// ! DO NOT USE _rb.velocity.z > 0!!!!
			// ? Why do we need this when we have AlignMomentumWithSkateboard? no point doing more work.

			// if (Vector3.Dot(_rb.velocity.normalized, Orientation.forward) > 0)
			// 	_rb.velocity = _playerModel.forward.normalized * _rb.velocity.magnitude;
			// else if (Vector3.Dot(_rb.velocity.normalized, Orientation.forward) < 0)
			// 	_rb.velocity = -_playerModel.forward.normalized * _rb.velocity.magnitude;

		}
	}
	#endregion


	#region CalculateTurnSpeed
	/// <summary>
	/// Calculates the speed which to turn at.
	/// </summary>
	private void CalculateTurnSpeed()
	{
		// the current turn speed based on current player velocity.
		_currentTurnSpeed = Mathf.Lerp(_maxTurnSpeed, _minTurnSpeed, _rb.velocity.magnitude / MaxAccelerationSpeed);
	}
	#endregion


	#region AlignMomentumWithSkateboard
	/// <summary>
	/// Make the velocity follow the scateboard's direction
	/// </summary>
	private void AlignMomentumWithSkateboard()
	{
		Vector3 calcVel = _rb.velocity;
		calcVel.y = 0;

		float maxDiviation = .95f;

		// TODO make it so only the speed align with the scateboard is kept. (might need a diff func)

		if (isGrounded && !_onSlope && (Vector3.Dot(calcVel.normalized, Orientation.forward) < maxDiviation || Vector3.Dot(calcVel.normalized, Orientation.forward) > -maxDiviation))
		{
			float y = _rb.velocity.y;

			if (Vector3.Dot(calcVel.normalized, Orientation.forward) > 0)
			{
				_rb.velocity = Orientation.forward.normalized * calcVel.magnitude + new Vector3(0, y, 0);
			}
			else if (Vector3.Dot(calcVel.normalized, Orientation.forward) < 0)
			{
				_rb.velocity = -Orientation.forward.normalized * calcVel.magnitude + new Vector3(0, y, 0);
			}

		}
	}
	#endregion
}


//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|