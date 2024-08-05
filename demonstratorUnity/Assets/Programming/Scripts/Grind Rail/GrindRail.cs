using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class GrindRail : MonoBehaviour
{
	public SplineContainer splineCont;

	public Vector3 pos = Vector3.zero;

	// Start is called before the first frame update
	void Start()
	{
		splineCont = GetComponent<SplineContainer>();

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnCollisionEnter(Collision other)
	{

		if (other.transform.tag == "Player")
		{
			other.transform.GetComponentInChildren<GrindSystem>().StartGrind(splineCont);
		}


	}


	void OnDrawGizmos()
	{
		NativeSpline nsplis = new NativeSpline(GetComponent<SplineContainer>().Splines[0]);
		SplineUtility.GetNearestPoint(nsplis, transform.rotation * (pos - transform.position), out float3 nearest, out float t);

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.rotation * Vector3.right);

		Gizmos.color = Color.green;
		Vector3 posis = (transform.rotation * (Vector3)nearest) + transform.position;

		Gizmos.DrawSphere(posis, 0.5f);

		Vector3 forward = Vector3.Normalize(nsplis.EvaluateUpVector(t));
		Vector3 up = new NativeSpline(GetComponent<SplineContainer>().Splines[0]).EvaluateUpVector(t);

		Vector3 remappedForward = new Vector3(0, 0, 1);
		Vector3 remappedUp = new Vector3(0, 1, 0);
		Quaternion axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp)) * transform.rotation;

		Gizmos.color = Color.black;
		Gizmos.DrawRay(posis, (Quaternion.LookRotation(forward, up) * axisRemapRotation).eulerAngles);



		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(pos, 0.5f);
	}
}
