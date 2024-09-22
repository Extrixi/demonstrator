using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Handles grinding on splines aka rails.
/// </summary>
public class GrindSystem : MonoBehaviour
{
	[HideInInspector]
	public SplineContainer SplineContainer;

	[HideInInspector]
	public Transform SplineContainerTransform;

	[Header("Grinding settings")]
	public float Threshold = .2f;

	[HideInInspector]
	public bool IsGrinding = false;

	[Tooltip("The force to apply if the player is not reaching the speed.")]
	public float MinGrindSpeed = 10f;

	// needed other wise, player sits inside the spline.
	public float YOffset = 1f;

	public float RerailDelayTime = 2f;

	// a class in SplineContainer, needed to interact with the spline.
	private Spline currentSpline;

	// use to rotate the player along the spline.
	private Quaternion AxisRemap;

	private Vector3 SplinePosition;
	private Quaternion SplineRotation;

	Vector3 remappedForward = new Vector3(0, 0, 1);
	Vector3 remappedUp = new Vector3(0, 1, 0);

	Vector3 forward;
	Vector3 up;

	// used to apply a force to the player on the spline.
	private Rigidbody rb;

	private MovementController movementController;

	bool IsGoingForward = false;
	bool IsFacingRight = false;

	private Vector3 rbVel;

	private bool _underRequiredSpeed = false;


	#region Awake
	private void Awake()
	{
		rb = GetComponent<Rigidbody>();

		movementController = GetComponent<MovementController>();
	}
	#endregion


	#region Update
	void Update()
	{
		if (!IsGrinding)
		{
			return;
		}

		GetSplineData();

		Vector3 facing = Vector3.zero;

		if (IsFacingRight)
		{
			facing = Vector3.right;
		}
		else
		{
			facing = Vector3.left;
		}

		// rotates the player.
		Quaternion Rot = SplineContainerTransform.rotation * Quaternion.Inverse(Quaternion.LookRotation(facing, Vector3.up));
		movementController.Orientation.rotation = Quaternion.LookRotation(forward, up) * Rot;




		// if we reach any end of the spline, we will be dismounted.
		if (Vector3.Distance(transform.position, (SplineContainerTransform.rotation * (Vector3)currentSpline.Knots.First().Position)
		+ SplineContainerTransform.position) < Threshold && !currentSpline.Closed && IsGrinding)
		{
			StopGrinding();
		}
		else if (Vector3.Distance(transform.position, (SplineContainerTransform.rotation * (Vector3)currentSpline.Knots.Last().Position)
		+ SplineContainerTransform.position) < Threshold && !currentSpline.Closed && IsGrinding)
		{
			StopGrinding();
		}

		HandleRailJumping();
	}

	private void HandleRailJumping()
	{
		// TODO add slight angle to force so player is pushed away a little.
		if (Input.GetKeyDown(KeyCode.Space))
		{
			StopGrinding();

			Vector3 addition = new Vector3(0, -rb.velocity.y, 0);

			Vector3 Force = new Vector3();

			if (rb.velocity.y < 0)
			{
				Force = addition + movementController.Orientation.up * movementController.JumpForce;
			}
			else
			{
				Force = movementController.Orientation.up * movementController.JumpForce;
			}

			rb.AddForce(Force, ForceMode.Impulse);
		}
	}
	#endregion


	#region FixedUpdate
	private void FixedUpdate()
	{
		if (!IsGrinding)
		{
			return;
		}


		transform.position = SplinePosition + new Vector3(0, YOffset, 0);

		// adds a constance force to the player in forward or backwards depending on IsGoingForward.
		// additonal check as the player can be facing 2 directions. IsFacingRight

		if (IsGoingForward && IsFacingRight)
		{
			if (_underRequiredSpeed) rb.AddForce((movementController.Orientation.right * MinGrindSpeed) - rb.velocity);
			else rb.AddForce((movementController.Orientation.right * rbVel.magnitude) - rb.velocity);

		}


		else if (IsGoingForward && !IsFacingRight)
		{
			if (_underRequiredSpeed) rb.AddForce((-movementController.Orientation.right * MinGrindSpeed) - rb.velocity);
			else rb.AddForce((-movementController.Orientation.right * rbVel.magnitude) - rb.velocity);
		}


		else if (!IsGoingForward && IsFacingRight)
		{
			if (_underRequiredSpeed) rb.AddForce((-movementController.Orientation.right * MinGrindSpeed) - rb.velocity);
			else rb.AddForce((-movementController.Orientation.right * rbVel.magnitude) - rb.velocity);

		}


		else if (!IsGoingForward && !IsFacingRight)
		{
			if (_underRequiredSpeed) rb.AddForce((movementController.Orientation.right * MinGrindSpeed) - rb.velocity);
			else rb.AddForce((movementController.Orientation.right * rbVel.magnitude) - rb.velocity);

		}

	}
	#endregion


	#region StartGrind
	/// <summary>
	/// Sets the data and begins grinding the rail.
	/// </summary>
	/// <param name="splineComp"></param>
	public void StartGrind(SplineContainer splineComp)
	{
		SplineContainer = splineComp;

		SplineContainerTransform = splineComp.transform;

		currentSpline = SplineContainer.Splines[0];



		SplineContainer.GetComponent<Collider>().enabled = false;

		movementController.IsLocked = true;

		GetSplineData();



		rbVel = rb.velocity;
		// rbVel.y = 0; // ? why are we removing Y, this will impacy downwards or upwards splines.

		if (Vector3.Dot(SplineRotation * Vector3.right, movementController.Orientation.forward) <= 0)
		{
			IsFacingRight = true;
		}
		else
		{
			IsFacingRight = false;
		}

		if (Vector3.Dot(SplineRotation * Vector3.forward, rbVel) >= 0)
		{
			IsGoingForward = true;
		}
		else
		{
			IsGoingForward = false;
		}

		if (rbVel.magnitude <= MinGrindSpeed)
		{
			_underRequiredSpeed = true;
		}
		else
		{
			_underRequiredSpeed = false;
		}



		rb.useGravity = false;

		IsGrinding = true;
	}
	#endregion


	#region StopGrinding
	/// <summary>
	/// Stops grinding. Ruturns any control back to the movement controller.
	/// </summary>
	public void StopGrinding()
	{
		IsGrinding = false;

		rb.useGravity = true;

		movementController.IsLocked = false;



		StartCoroutine(StopGrindingIEnumerator());
	}
	#endregion


	#region GetSplineData
	/// <summary>
	/// Used to get the data of the spline. This is nearest point and rotation.
	/// </summary>
	public void GetSplineData()
	{
		NativeSpline native = new NativeSpline(currentSpline);

		SplineUtility.GetNearestPoint(native, Quaternion.Inverse(SplineContainerTransform.rotation) * (transform.position - SplineContainerTransform.position), out float3 nearest, out float t);

		SplinePosition = (SplineContainerTransform.rotation * (Vector3)nearest) + SplineContainerTransform.position;

		forward = Vector3.Normalize(native.EvaluateTangent(t));
		up = native.EvaluateUpVector(t);

		// Vector3 remappedForward = new Vector3(0, 0, 1);
		// Vector3 remappedUp = new Vector3(0, 1, 0);
		AxisRemap = SplineContainerTransform.rotation * Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

		SplineRotation = Quaternion.LookRotation(forward, up) * AxisRemap;
	}
	#endregion


	#region StopGrindingIEnumerator
	/// <summary>
	/// Need this to prevent the player from entering the rail when they dismount.
	/// </summary>
	/// <returns></returns>
	private IEnumerator StopGrindingIEnumerator()
	{
		yield return new WaitForSeconds(RerailDelayTime);

		SplineContainer.GetComponent<Collider>().enabled = true;

	}
	#endregion
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|