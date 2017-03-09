using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {

    public Transform objToFollow;

    float followSpeed = 7f;
    float distanceGap;
    float rotationSpeed = 2f;
	// Use this for initialization
	void Start () {

        transform.SetParent(objToFollow);
        GetComponent<CamFollow>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        distanceGap = Vector3.Distance(transform.position, objToFollow.position);
        //transform.position = Vector3.MoveTowards(transform.position, objToFollow.position, (followSpeed * (distanceGap) )*Time.fixedDeltaTime);
        transform.position = objToFollow.transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, objToFollow.rotation, (rotationSpeed ) * Time.fixedDeltaTime);

	}
}
