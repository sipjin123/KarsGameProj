using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_Behaviour : MonoBehaviour {

    public Transform WAypointParent;
    public GameObject _carRotate;

    public Transform[] Patterns;
    public List<Transform> Waypoints;

    bool ifStart;
    public int currentChild;

    public TrailCollision _trailcollision;

    float movementSpeed;
    float rotSpeed;
    TronGameManager _tronGameManager;

    float randomRotationValueAI;

    bool isDead; 

    void Start ()
    {
        currentChild = 0;
        SetPatternAndWaypoints(Random.RandomRange(0,Patterns.Length));
        movementSpeed = 2;
        rotSpeed = 5;
    }
	
    void SetPatternAndWaypoints(int _pattternVAl)
    {
        randomRotationValueAI = Random.RandomRange(-1, 2);
        movVal.text = _pattternVAl.ToString();
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
    //==========================================
    void FixedUpdate ()
    {
        if (!ifStart || isDead)
            return;

        RAycastFunct();

        if (findNextPath == false)
        {
            if (Vector3.Distance(transform.position, Waypoints[currentChild].transform.position) > 3)
            {
                transform.position = Vector3.MoveTowards(transform.position, Waypoints[currentChild].transform.position, movementSpeed * Time.fixedDeltaTime);

                var lookPos = Waypoints[currentChild].transform.position - transform.position;
                var rotation = Quaternion.LookRotation(lookPos);
                _carRotate.transform.rotation = Quaternion.Slerp(_carRotate.transform.rotation, rotation, Time.deltaTime * rotSpeed);
            }
            else
            {
                if (currentChild < Waypoints.Count - 1)
                    currentChild++;
                else
                    currentChild = 0;
            }
        }
    }
    //==========================================
    void OnTriggerEnter(Collider hit)
    {
        if(TronGameManager.Instance.NetworkStart == false)
        if (hit.gameObject.tag == "Trail" || hit.gameObject.name.Contains("Missle")|| (hit.gameObject.tag == "Car" && hit.gameObject.name != gameObject.name))
        {
            if(!SHieldSwithc)
            DIE();
        }
    }
    //==========================================

    public bool SHieldSwithc;
    public GameObject AIShield;
    public void ActivateAIShieldFromButton()
    {
        ActivateAIShield(!SHieldSwithc);
    }
    public void ActivateAIShield(bool _switch)
    {
        SHieldSwithc = !SHieldSwithc;
        AIShield.SetActive(_switch);
    }
    //==========================================
    public void DIE()
    {
        transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        isDead = true;
        _trailcollision.SetEmiision(false);
        StartCoroutine("delayREspawn");
    }
    IEnumerator delayREspawn()
    {
        yield return new WaitForSeconds(3);
        SetPatternAndWaypoints(Random.RandomRange(0, Patterns.Length));
        transform.position = new Vector3(3, 1, 0);
        isDead = false;
        _trailcollision.SetEmiision(true);
    }
    //==========================================
    public Vector3  clearPathPositionTotal;
    public Transform  radayRayPivot , radayRayPos1 , radayRayPos2, lockOnRayPos;
    RaycastHit  radarRay1, radarRay2, lockOnRay;

    bool clearRadat1, clearRadar2;


    float rayRange = 100;
    bool findNextPath;
    void RAycastFunct()
    {
        newRaycastFunc();
        return;


        Debug.DrawRay(radayRayPos1.transform.position, radayRayPos1.transform.forward * rayRange, Color.red);
        Debug.DrawRay(radayRayPos2.transform.position, radayRayPos2.transform.forward * rayRange, Color.red);
        if (Physics.Raycast(radayRayPos1.transform.position, radayRayPos1.transform.forward * rayRange, out radarRay1))
        {
            if (radarRay1.collider.tag != "Trail")
            {
                Debug.LogError("Ray1: " + radarRay1.collider.gameObject.name);
                clearRadat1 = true;
            }
            else
            {
                clearRadat1 = false;
            }
        }
        if (Physics.Raycast(radayRayPos2.transform.position, radayRayPos2.transform.forward * rayRange, out radarRay2))
        {
            if (radarRay2.collider.tag != "Trail")
            {
                Debug.LogError("Ray2: " + radarRay2.collider.gameObject.name);
                clearRadar2 = true;
            }
            else
            {
                clearRadar2 = false;
            }
        }

        if (clearRadat1 && clearRadar2)
        {
            Debug.DrawRay(radayRayPivot.transform.position, radayRayPivot.transform.forward * rayRange, Color.red);
            if (Physics.Raycast(radayRayPivot.transform.position, radayRayPivot.transform.forward * rayRange, out radarRay2))
            {
                clearPathPositionTotal = radarRay2.point;
            }



            Debug.DrawRay(lockOnRayPos.transform.position, lockOnRayPos.transform.forward * rayRange, Color.red);
            lockOnRayPos.transform.LookAt(Waypoints[currentChild].transform.position);
            if (Physics.Raycast(lockOnRayPos.transform.position, lockOnRayPos.transform.forward * rayRange, out lockOnRay))
            {
                Debug.LogError("lock: " + lockOnRay.collider.gameObject.name);
                if (lockOnRay.collider.tag == "Waypoint")
                {
                    Debug.LogError("WAWAWAWAAWWAWA");
                    findNextPath = false;
                }
            }

            if (Vector3.Distance(transform.position, clearPathPositionTotal) > 3)
            {
                transform.position = Vector3.MoveTowards(transform.position, clearPathPositionTotal, movementSpeed * Time.fixedDeltaTime);
            }
            else
            {
            }
        }
        else
        {
            findNextPath = true;
            _carRotate.transform.Rotate(0, randomRotationValueAI, 0);
        }



    }


    int[] RAyvalIndividual = new int[5];
    public Transform[] rayHitsPosition;
    RaycastHit[] RayHits = new RaycastHit[5];


    bool startRotate;
    bool isRight;

    bool carLockRotate;

    Quaternion referenceRotation;
    float timerDElay;

    public enum AIPhase
    {
        NONE,
        LOOK_AROUND,
        ROTATE_CAR,
        MOVE_TOWARD
    }
    public AIPhase _aiPHase;
    public bool ifPathisClearOfTrail()
    {
        int totalVal = 0;
        for (int i = 0; i < RayHits.Length; i++)
        {
            Debug.DrawRay(rayHitsPosition[i].position, rayHitsPosition[i].forward * rayRange, Color.red);
            if (Physics.Raycast(rayHitsPosition[i].position, rayHitsPosition[i].forward * rayRange, out RayHits[i]))
            {
                if (RayHits[i].collider.tag == "Trail")
                {
                    RAyvalIndividual[i] = 1;
                }
                else
                {
                    RAyvalIndividual[i] = 0;
                }
            }
            totalVal += RAyvalIndividual[i];
        }

        if( totalVal == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void newRaycastFunc()
    {
        switch (_aiPHase)
        {
            case AIPhase.NONE:
                {
                    if (!ifPathisClearOfTrail())
                    {
                        findNextPath = true;

                        if (RAyvalIndividual[0] == 1 && RAyvalIndividual[4] == 0)
                            isRight = true;
                        else
                            isRight = false;


                        _aiPHase = AIPhase.LOOK_AROUND;
                    }
                }
                break;
            case AIPhase.LOOK_AROUND:
                {
                    transform.position += _carRotate.transform.forward * Time.fixedDeltaTime * (movementSpeed * .25f);
                    if (isRight)
                        radayRayPivot.transform.Rotate(0, 1, 0);
                    else
                        radayRayPivot.transform.Rotate(0, -1, 0);
                    if(ifPathisClearOfTrail())
                    {
                        referenceRotation = radayRayPivot.transform.rotation;

                        Debug.DrawRay(radayRayPivot.transform.position, radayRayPivot.transform.forward * rayRange, Color.red);
                        if (Physics.Raycast(radayRayPivot.transform.position, radayRayPivot.transform.forward * rayRange, out radarRay2))
                        {
                            clearPathPositionTotal = radarRay2.point;
                            timerDElay = 0;
                            radayRayPivot.transform.localRotation = Quaternion.Euler(Vector3.zero);
                            _aiPHase = AIPhase.ROTATE_CAR;
                        }
                    }
                }
                break;
            case AIPhase.ROTATE_CAR:
                {
                    transform.position += _carRotate.transform.forward * Time.fixedDeltaTime * (movementSpeed*.25f);
                    timerDElay += Time.fixedDeltaTime;
                    /*
                    var lookPos = clearPathPositionTotal - transform.position;
                    var rotation = Quaternion.LookRotation(lookPos);
                    _carRotate.transform.rotation = Quaternion.Slerp(_carRotate.transform.rotation, rotation, Time.deltaTime * rotSpeed);*/

                    _carRotate.transform.rotation = Quaternion.Slerp(_carRotate.transform.rotation, referenceRotation, Time.deltaTime * rotSpeed);
                    
                    if (timerDElay > 1)
                    {
                        _aiPHase = AIPhase.MOVE_TOWARD;
                    }
                }
                break;
            case AIPhase.MOVE_TOWARD:
                {
                    if(ifPathisClearOfTrail() == false)
                    {
                        _aiPHase = AIPhase.NONE;
                        return;
                    }

                    transform.position += _carRotate.transform.forward * Time.fixedDeltaTime * movementSpeed;

                    Debug.DrawRay(lockOnRayPos.transform.position, lockOnRayPos.transform.forward * rayRange, Color.red);
                    lockOnRayPos.transform.LookAt(Waypoints[currentChild].transform.position);
                    if (Physics.Raycast(lockOnRayPos.transform.position, lockOnRayPos.transform.forward * rayRange, out lockOnRay))
                    {
                        Debug.LogError("lock: " + lockOnRay.collider.gameObject.name);
                        if (lockOnRay.collider.tag == "Waypoint")
                        {
                            _aiPHase = AIPhase.NONE;
                            findNextPath = false;
                        }
                    }
                }
                break;
        }
    }

    //==========================================
    public void EnableDisableTest()
    {
        testPanel.SetActive(!testPanel.activeInHierarchy);
    }
    public Text rotVal, movVal, PatternVal;
    public GameObject testPanel;

    public void AddMoveSpeed(float _var)
    {
        movementSpeed += _var;
        movVal.text = movementSpeed.ToString();
    }
    public void AddRotSpeed(float _var)
    {
        rotSpeed += _var;
        movVal.text = rotSpeed.ToString();
    }
    public void SetPatternRand()
    {
        SetPatternAndWaypoints(Random.RandomRange(0, Patterns.Length));
    }
}
