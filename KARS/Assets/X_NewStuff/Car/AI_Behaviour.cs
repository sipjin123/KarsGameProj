using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Behaviour : MonoBehaviour {

    public Transform WAypointParent;
    public GameObject _carRotate;

    public Transform[] Patterns;
    public List<Transform> Waypoints;

    bool ifStart;
    int currentChild;

    public TrailCollision _trailcollision;

    float movementSpeed;
    TronGameManager _tronGameManager;
    void Start ()
    {
        currentChild = 0;
        SetPatternAndWaypoints(2);

    }
	
    void SetPatternAndWaypoints(int _pattternVAl)
    {
        _tronGameManager = TronGameManager.Instance.GetComponent<TronGameManager>();
        Waypoints.Clear();
        ifStart = false;
        for(int i = 0; i < Patterns[_pattternVAl].childCount;i++)
        {
            Waypoints.Add( Patterns[_pattternVAl].GetChild(i).transform);
        }
        currentChild = 0;
        ifStart = true;
        _trailcollision.SetEmiision(true);
    }

	void FixedUpdate ()
    {
        if (!ifStart)
            return;
        movementSpeed = _tronGameManager.MovementSpeed;
        if (Vector3.Distance(transform.position, Waypoints[currentChild].transform.position) > 3)
        {
            transform.position = Vector3.MoveTowards(transform.position, Waypoints[currentChild].transform.position, movementSpeed * Time.fixedDeltaTime);
            _carRotate.transform.LookAt (Waypoints[currentChild].transform.position);
        }
        else
        {
            if (currentChild < Waypoints.Count-1)
                currentChild++;
            else
                currentChild = 0;
        }
    }
}
