using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class GrindSystem : MonoBehaviour
{
	public SplineContainer SplineContainer;

	public Transform SplineContainerTransform;

	public float Threshold = .2f;

	public bool IsGrinding = false;

	[Tooltip("The force to apply if the player is not reaching the speed.")]
	public float MinGrindSpeed = 10f;

	private Spline currentSpline;

	private Rigidbody rb;

	private MovementController movementController;

	private Quaternion AxisRemap;

	private Vector3 SplinePosition;
	private Quaternion SplineRotation;

	Vector3 remappedForward = new Vector3(0, 0, 1);
	Vector3 remappedUp = new Vector3(0, 1, 0);

	Vector3 forward;
	Vector3 up;

	bool IsGoingForward = false;
	bool IsFacingRight = false;

	private Vector3 rbVel;

	private float magnitude;

	private bool _useMinGrindSpeed = false;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();

		movementController = GetComponent<MovementController>();

		//currentSpline = splineContainer.Splines[0];
	}

	public void StartGrind(SplineContainer splineComp)
	{
		SplineContainer = splineComp;

		SplineContainerTransform = splineComp.transform;

		currentSpline = SplineContainer.Splines[0];



		SplineContainer.GetComponent<Collider>().enabled = false;

		movementController.IsLocked = true;

		GetSplineData();

		// print(Vector3.Dot(SplineRotation * Vector3.forward, movementController.Orientation.forward));
		// print(SplineRotation * Vector3.forward + " " + movementController.Orientation.forward);

		rbVel = rb.velocity;
		rbVel.y = 0;

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
			_useMinGrindSpeed = true;
		}
		else
		{
			_useMinGrindSpeed = false;
		}



		rb.useGravity = false;

		IsGrinding = true;
	}

	public void StopGrinding()
	{
		IsGrinding = false;

		rb.useGravity = true;

		movementController.IsLocked = false;



		StartCoroutine(StopGrindingIEnumerator());
	}

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

		Quaternion Rot = SplineContainerTransform.rotation * Quaternion.Inverse(Quaternion.LookRotation(facing, Vector3.up));
		movementController.Orientation.rotation = Quaternion.LookRotation(forward, up) * Rot;





		if (Vector3.Distance(transform.position, (SplineContainerTransform.rotation * (Vector3)currentSpline.Knots.First().Position) + SplineContainerTransform.position) < Threshold)
		{
			StopGrinding();
		}
		else if (Vector3.Distance(transform.position, (SplineContainerTransform.rotation * (Vector3)currentSpline.Knots.Last().Position) + SplineContainerTransform.position) < Threshold)
		{
			StopGrinding();
		}

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

	private void FixedUpdate()
	{
		if (!IsGrinding)
		{
			return;
		}


		transform.position = SplinePosition;

		if (IsGoingForward && IsFacingRight)
		{
			// print("forward right");
			if (_useMinGrindSpeed) rb.AddForce((movementController.Orientation.right * MinGrindSpeed) - rb.velocity);
			else rb.AddForce((movementController.Orientation.right * rbVel.magnitude) - rb.velocity);

		}
		else if (IsGoingForward && !IsFacingRight)
		{
			// print("forward left");
			if (_useMinGrindSpeed) rb.AddForce((-movementController.Orientation.right * MinGrindSpeed) - rb.velocity);
			else rb.AddForce((-movementController.Orientation.right * rbVel.magnitude) - rb.velocity);
		}
		else if (!IsGoingForward && IsFacingRight)
		{
			// print("backwards left");
			if (_useMinGrindSpeed) rb.AddForce((-movementController.Orientation.right * MinGrindSpeed) - rb.velocity);
			else rb.AddForce((-movementController.Orientation.right * rbVel.magnitude) - rb.velocity);

		}
		else if (!IsGoingForward && !IsFacingRight)
		{
			// print("backwards right");
			if (_useMinGrindSpeed) rb.AddForce((movementController.Orientation.right * MinGrindSpeed) - rb.velocity);
			else rb.AddForce((movementController.Orientation.right * rbVel.magnitude) - rb.velocity);

		}



		//movementController.isGrounded = true;




		// Vector3 newForward = movementController._orientation.forward;

		// Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));


		// if (rb.velocity.magnitude > 5f)
		// {
		// 	float y = rb.velocity.y;

		// 	Vector3 newVel = rb.velocity.normalized * movementController.TargetSpeed;

		// 	newVel.y = y;

		// 	rb.velocity = newVel;
		// }



		// rb.velocity = rb.velocity.magnitude * newForward;

	}

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


	private IEnumerator StopGrindingIEnumerator()
	{
		yield return new WaitForSeconds(2f);

		SplineContainer.GetComponent<Collider>().enabled = true;

	}
}
