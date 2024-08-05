using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class GrindRail : MonoBehaviour
{
	public SplineContainer splineCont;

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
}
