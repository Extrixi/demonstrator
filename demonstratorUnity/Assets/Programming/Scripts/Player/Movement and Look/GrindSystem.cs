using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class GrindSystem : MonoBehaviour
{
	public SplineContainer SplineContainer;

	public Transform SplineContainerTransform;

	private NativeSpline native;


	public bool IsGrinding = false;

	public float YAxisOffSet = 3f;
	public float XAxisOffSet = 1f;


	private Spline currentSpline;

	private Rigidbody rb;

	private MovementController movementController;

	private Vector3 remappedForward = new Vector3(0, 0, 1);
	private Vector3 remappedUp = new Vector3(0, 1, 0);


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

		native = new NativeSpline(currentSpline);

		SplineContainer.GetComponent<Collider>().enabled = false;

		movementController.IsLocked = true;


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

	private void FixedUpdate()
	{
		if (!IsGrinding || SplineContainer == null)
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


		SplineUtility.GetNearestPoint(native, Quaternion.Inverse(SplineContainerTransform.rotation) * (transform.position - SplineContainerTransform.position), out float3 nearest, out float t);

		transform.position = (SplineContainerTransform.rotation * (Vector3)nearest) + SplineContainerTransform.position;

		Vector3 forward = Vector3.Normalize(native.EvaluateTangent(t));
		Vector3 up = native.EvaluateUpVector(t);

		Vector3 remappedForward = new Vector3(0, 0, 1);
		Vector3 remappedUp = new Vector3(0, 1, 0);
		Quaternion axisRemapRotation = SplineContainerTransform.rotation * Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

		movementController.Orientation.rotation = Quaternion.LookRotation(forward, up) * axisRemapRotation;

		// Vector3 newForward = movementController._orientation.forward;

		// Vector3 wishDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));


		// if (rb.velocity.magnitude > 5f)
		// {
		// 	float y = rb.velocity.y;

		// 	Vector3 newVel = rb.velocity.normalized * movementController.TargetSpeed;

		// 	newVel.y = y;

		// 	rb.velocity = newVel;
		// }

		if (Input.GetKey(KeyCode.S))
		{
			rb.AddForce((Quaternion.LookRotation(forward, up) * axisRemapRotation * Vector3.back) * 2f, ForceMode.Force);


			// newForward *= -1;
		}
		else if (Input.GetKey(KeyCode.W))
		{
			rb.AddForce((Quaternion.LookRotation(forward, up) * axisRemapRotation * Vector3.forward) * 2f, ForceMode.Force);



		}

		Debug.DrawRay(transform.position, (Quaternion.LookRotation(forward, up) * axisRemapRotation * Vector3.back), Color.red);
		Debug.DrawRay(transform.position, (Quaternion.LookRotation(forward, up) * axisRemapRotation * Vector3.back), Color.blue);

		// rb.velocity = rb.velocity.magnitude * newForward;

	}


	private IEnumerator StopGrindingIEnumerator()
	{
		yield return new WaitForSeconds(2f);

		SplineContainer.GetComponent<Collider>().enabled = true;

	}
}
