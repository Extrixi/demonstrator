using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class GrindSystem : MonoBehaviour
{
	public SplineContainer splineContainer;


	public bool isGrinding = false;

	public float YAxisOffSet = 3f;
	public float XAxisOffSet = 1f;


	private Spline currentSpline;

	private Rigidbody rb;

	private Transform parent;

	private MovementController movementController;


	public void HitJunction(Spline rail)
	{
		currentSpline = rail;
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();

		movementController = GetComponent<MovementController>();

		//currentSpline = splineContainer.Splines[0];
	}

	public void StartGrind(SplineContainer splineComp)
	{
		splineContainer = splineComp;

		currentSpline = splineContainer.Splines[0];

		splineContainer.GetComponent<Collider>().enabled = false;

		movementController.IsLocked = true;


		rb.useGravity = false;

		isGrinding = true;
	}

	public void StopGrinding()
	{
		isGrinding = false;

		rb.useGravity = true; // TODO change into manager

		movementController.IsLocked = false;



		StartCoroutine(StopGrindingIEnumerator());
	}

	private void FixedUpdate()
	{
		if (!isGrinding || splineContainer == null)
		{
			return;
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





		//movementController.isGrounded = true;

		NativeSpline native = new NativeSpline(currentSpline);
		float distance = SplineUtility.GetNearestPoint(native, transform.position - splineContainer.transform.position, out float3 nearest, out float t);

		transform.position = nearest + new float3(0, 1 * YAxisOffSet, 0) + new float3(1 * XAxisOffSet, 0, 0) + (float3)splineContainer.transform.position;

		Vector3 forward = Vector3.Normalize(native.EvaluateTangent(t));
		Vector3 up = native.EvaluateUpVector(t);

		Vector3 remappedForward = new Vector3(0, 0, 1);
		Vector3 remappedUp = new Vector3(0, 1, 0);
		Quaternion axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

		movementController.Orientation.rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;

		// Vector3 newForward = movementController._orientation.forward;

		// Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));


		if (rb.velocity.magnitude > movementController.TargetSpeed)
		{
			float y = rb.velocity.y;

			Vector3 newVel = rb.velocity.normalized * movementController.TargetSpeed;

			newVel.y = y;

			rb.velocity = newVel;
		}

		if (Input.GetKey(KeyCode.S))
		{
			rb.AddForce((-movementController.Orientation.forward * movementController.TargetSpeed) - rb.velocity, ForceMode.Force);
			// newForward *= -1;
		}
		else if (Input.GetKey(KeyCode.W))
		{
			rb.AddForce((movementController.Orientation.forward * movementController.TargetSpeed) - rb.velocity, ForceMode.Force);
		}

		// rb.velocity = rb.velocity.magnitude * newForward;

	}


	private IEnumerator StopGrindingIEnumerator()
	{
		yield return new WaitForSeconds(2f);

		splineContainer.GetComponent<Collider>().enabled = true;

	}
}
