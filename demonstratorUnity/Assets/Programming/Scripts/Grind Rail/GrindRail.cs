using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Sends the spline container atteched to this object to the grind system.
/// </summary>
public class GrindRail : MonoBehaviour
{

	public SplineContainer SplineContainer;


#if UNITY_EDITOR
	#region Debug display
	[Header("DEBUGGING")]
	public bool DebugEnabled = false;

	public Vector3 BluePos = Vector3.zero;
	#endregion
#endif

	// Start is called before the first frame update
	void Start()
	{
		SplineContainer = GetComponent<SplineContainer>();

	}

	void OnCollisionEnter(Collision other)
	{

		if (other.transform.tag == "Player")
		{
			other.transform.GetComponentInChildren<GrindSystem>().StartGrind(SplineContainer);
		}


	}

#if UNITY_EDITOR
	#region Debug
	void OnDrawGizmos()
	{

		if (!DebugEnabled) return;

		NativeSpline nsplis = new NativeSpline(GetComponent<SplineContainer>().Splines[0]);
		// converting local space into world space because float3 is not in local and the transofrm is.
		SplineUtility.GetNearestPoint(nsplis, Quaternion.Inverse(transform.rotation) * (BluePos - transform.position), out float3 nearest, out float t);


		Gizmos.color = Color.green;
		Vector3 posis = (transform.rotation * (Vector3)nearest) + transform.position;

		Gizmos.DrawSphere(posis, 0.5f);

		// we take the spline lerp ammount between two points and plug that in to find the up vector. we store this.
		Vector3 forward = Vector3.Normalize(nsplis.EvaluateTangent(t));
		// we store this to the vector.
		Vector3 up = nsplis.EvaluateUpVector(t);

		// vector moment. World space btw.
		Vector3 remappedForward = new Vector3(0, 0, 1);
		Vector3 remappedUp = new Vector3(0, 1, 0);

		// uses the current rotatiojn and adds the spline rotation on top so its aligned with the spline and in local space.
		Quaternion axisRemapRotation = transform.rotation * Quaternion.Inverse(Quaternion.LookRotation(remappedForward, remappedUp));

		// Quaternion.LookRotation(forward, up) * axisRemapRotation gets the rotation


		Gizmos.DrawSphere(posis, 0.5f);

		Gizmos.color = Color.red;
		Gizmos.DrawRay(posis, Quaternion.LookRotation(forward, up) * axisRemapRotation * Vector3.right);
		Gizmos.color = Color.green;
		Gizmos.DrawRay(posis, Quaternion.LookRotation(forward, up) * axisRemapRotation * Vector3.up);


		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(BluePos, 0.5f);
	}
	#endregion
#endif
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
