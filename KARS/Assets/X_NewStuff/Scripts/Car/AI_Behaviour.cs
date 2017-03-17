using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_Behaviour : MonoBehaviour {
    #region VARIABLES
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
    public float AI_Health;
    #endregion
    //==========================================================================================================================================
    #region INIT
    void Start ()
    {
        currentChild = 0;
        SetPatternAndWaypoints(Random.RandomRange(0,Patterns.Length));
        movementSpeed = 5;
        rotSpeed = 55;
        currentRotation_Y = _carRotate.transform.rotation.y;
        AI_Health = 5;
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
    #endregion
    //==========================================================================================================================================

    float accelerationSpeed_Counter;

    float accelerationTimer;
    float accelerationSpeed_Max;

    void FixedUpdate ()
    {
        if (!ifStart || isDead)
            return;

        newRaycastFunc();

        accelerationTimer = _tronGameManager.accelerationTimerMax;
        accelerationSpeed_Max = _tronGameManager.accelerationSpeedMax;

        if (accelerationSpeed_Counter < accelerationSpeed_Max)
        {
            accelerationSpeed_Counter += Time.fixedDeltaTime * ((accelerationSpeed_Max) / accelerationTimer);
        }
        movementSpeed = accelerationSpeed_Counter;
        if (ifstuneed)
            movementSpeed = 1;


        if (Input.GetKeyDown(KeyCode.K))
        {
            PowerUpManager.Instance.LockOnTarget(2,_tronGameManager.PlayerObjects[0],MissleScript.MISSLE_TYPE.STUN);
        }

        if (findNextPath == false)
        {
            if (Vector3.Distance(transform.position, Waypoints[currentChild].transform.position) > 3)
            {
                //transform.position = Vector3.MoveTowards(transform.position, Waypoints[currentChild].transform.position, movementSpeed * Time.fixedDeltaTime);

                transform.position += _carRotate.transform.forward * Time.fixedDeltaTime * movementSpeed;
                
                //var lookPos = Waypoints[currentChild].transform.position - transform.position;
                //var rotation = Quaternion.LookRotation(lookPos);
                //_carRotate.transform.rotation = Quaternion.Slerp(_carRotate.transform.rotation, rotation, Time.deltaTime * rotSpeed);

                Debug.DrawRay(radayRayPivot.transform.position, radayRayPivot.transform.forward * rayRange, Color.red);
                if (Physics.Raycast(radayRayPivot.transform.position, radayRayPivot.transform.forward * rayRange, out radarRay2))
                {
                    //Debug.LogError("ray: "+radarRay2.collider.gameObject.name);
                    if(radarRay2.collider.tag == "Waypoint" && (radarRay2.collider.transform.parent.gameObject == Waypoints[currentChild].gameObject))
                    {
                    }
                    else
                    {
                        if (isRight == false)
                            moveLeft();
                        else
                            moveRight();
                    }
                }
                
            }
            else
            {
                if (currentChild < Waypoints.Count - 1)
                    currentChild++;
                else
                    currentChild = 0;

                isRight = Random.RandomRange(0, 2) == 1 ? true : false;
            }
        }
    }
    //==========================================================================================================================================
    void OnTriggerEnter(Collider hit)
    {
        return;
        if(TronGameManager.Instance.NetworkStart == false)
        if (hit.gameObject.tag == "Trail" || hit.gameObject.name.Contains("Wall") || (hit.gameObject.tag == "Car" && hit.gameObject.name != gameObject.name))
        {
            if (!SHieldSwithc)
            {
                AI_Health -= 1;
                DIE();
            }
        }
        if (hit.gameObject.name.Contains ( "Missle") && hit.gameObject.name != gameObject.name)
        {
            if(hit.GetComponent<MissleScript>().PlayerController_ID != 2)
            {
                if (hit.GetComponent<MissleScript>()._missleType == MissleScript.MISSLE_TYPE.STUN)
                {
                    //GetComponent<Car_DataReceiver>().StunObject.SetActive(true);
                    StartCoroutine("DElayRemoveDebuff");
                    ifstuneed = true;
                }
                /*
                if (hit.GetComponent<MissleScript>()._missleType == MissleScript.MISSLE_TYPE.CONFUSE)
                {
                    GetComponent<Car_DataReceiver>().ConfuseObject.SetActive(true);
                    StartCoroutine("DElayRemoveDebuff");
                }
                if (hit.GetComponent<MissleScript>()._missleType == MissleScript.MISSLE_TYPE.BLIND)
                {
                    GetComponent<Car_DataReceiver>().BlindObject.SetActive(true);
                    StartCoroutine("DElayRemoveDebuff");
                }*/
            }
        }
    }
    IEnumerator DElayRemoveDebuff()
    {
        yield return new WaitForSeconds(3);
        /*
        //GetComponent<Car_DataReceiver>().StunObject.SetActive(false);
        GetComponent<Car_DataReceiver>().ConfuseObject.SetActive(false);
        GetComponent<Car_DataReceiver>().BlindObject.SetActive(false);
        ifstuneed = false;*/
    }

    
    //==========================================================================================================================================
    public bool SHieldSwithc;
    public GameObject AIShield;

    bool ifstuneed;
    public void ActivateAIShieldFromButton()
    {
        ActivateAIShield(!SHieldSwithc);
    }
    public void ActivateAIShield(bool _switch)
    {
        SHieldSwithc = !SHieldSwithc;
        AIShield.SetActive(_switch);
    }
    //==========================================================================================================================================
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
        transform.position = new Vector3(25, 1, 0);
        isDead = false;
        _trailcollision.SetEmiision(true);
    }
    //==========================================================================================================================================
    public Vector3  clearPathPositionTotal;
    public Transform  radayRayPivot , radayRayPos1 , radayRayPos2, lockOnRayPos;
    RaycastHit  radarRay1, radarRay2, lockOnRay;

    bool clearRadat1, clearRadar2;


    float rayRange = 100;
    bool findNextPath;
   

    int[] RAyvalIndividual = new int[5];
    public Transform[] rayHitsPosition;
    RaycastHit[] RayHits = new RaycastHit[5];
    public Transform waypointLockerTransform;


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
                    if(Vector3.Distance( transform.position, RayHits[i].point) <5)
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
                    transform.position += _carRotate.transform.forward * Time.fixedDeltaTime * (movementSpeed);
                    if (isRight)
                        radayRayPivot.transform.Rotate(0, 5, 0);
                    else
                        radayRayPivot.transform.Rotate(0, -5, 0);
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

                            if (RAyvalIndividual[0] == 1 && RAyvalIndividual[5] == 0)
                                isRight = true;
                            else
                                isRight = false;
                        }
                    }
                }
                break;
            case AIPhase.ROTATE_CAR:
                {
                    transform.position += _carRotate.transform.forward * Time.fixedDeltaTime * (movementSpeed);
                    
                    timerDElay += Time.fixedDeltaTime;
                    //_carRotate.transform.rotation = Quaternion.Slerp(_carRotate.transform.rotation, referenceRotation, Time.deltaTime * rotSpeed);
                    if (isRight)
                        moveRight();
                    else
                        moveLeft();
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

    float currentRotation_Y;

    void moveRight()
    {

        currentRotation_Y += rotSpeed * Time.fixedDeltaTime;
        _carRotate.transform.eulerAngles = new Vector3(0, currentRotation_Y, 0);
    }

    void moveLeft()
    {


        currentRotation_Y -= rotSpeed * Time.fixedDeltaTime;
        _carRotate.transform.eulerAngles = new Vector3(0, currentRotation_Y, 0);
    }
    //==========================================================================================================================================
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
