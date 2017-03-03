using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Movement : MonoBehaviour {
    //==========================================================================================================================
    #region VARIABLES
    private float movementSpeed;

    CharacterController _characterController;

    float currentRotation_Y;
    float rotationSpeed;

    public Transform CarRotationObject;

    [SerializeField]
    public TrailCollision _trailCollision;
    public Camera myCam;

    public
    TronGameManager _tronGameManager;

    Car_DataReceiver MyCarDataReceiver;
    bool isDead;
    public bool StartGame;
    public bool isREady;

    public void SetStartGame(bool _switch)
    {
        StartGame = _switch;
    }
    public void SetReady(bool _Switch)
    {
        isREady = _Switch;
    }

    bool signalSent;
    float DieTimer;

    //SINGLE PLAYER
    public float AIMode_HpBar;
    public bool localShieldIsActive;
    public GameObject localShield;


    float accelerationSpeed_Counter;

    float accelerationTimer ;
    float accelerationSpeed_Max ;

    float NitrosSpeed ;
    public bool NitrosActive;

    public bool FlipSwitch;


    Rigidbody myRigid;
    [SerializeField]
    Transform centerOfMass;
    #endregion
    //==========================================================================================================================
    #region INIT
    void Awake()
    {
        AIMode_HpBar = 5;
        _characterController = GetComponent<CharacterController>();
        MyCarDataReceiver = GetComponent<Car_DataReceiver>();
        myRigid = GetComponent<Rigidbody>();
        currentRotation_Y = CarRotationObject.transform.eulerAngles.y;
        myRigid.centerOfMass = centerOfMass.localPosition;
    }

    void Start()
    {
        _tronGameManager = TronGameManager.Instance.GetComponent<TronGameManager>();
    }
    #endregion
    //==========================================================================================================================
    #region SINGLE PLAYER SHIELD
    public void ActiveShieldFromButton()
    {
        ActiveLocalShield(!localShieldIsActive);
    }
    public void ActiveLocalShield(bool _switch)
    {
        localShieldIsActive = _switch;
        localShield.SetActive(_switch);
    }
    #endregion
    //==========================================================================================================================
    #region MOVEMENT
    float ComputedValues()
    {
        float _finalValue = 0;
        if (MyCarDataReceiver.GetStunSwitch() == true)
            return 1;


        if (NitrosActive == false)
        {
            NitrosSpeed = 0;
        }
        _finalValue = movementSpeed + accelerationSpeed_Counter + NitrosSpeed;
        return _finalValue;
    }

    float ReduceValues()
    {
        float _finalValue = 100;

        if (FlipSwitch)
            _finalValue -= 50;
        _finalValue = _finalValue / 100;
        
        return _finalValue;
    }

    void FixedUpdate()
    {
        movementSpeed = _tronGameManager.MovementSpeed;
        rotationSpeed = _tronGameManager.rotationSpeed;

        NitrosSpeed = _tronGameManager.nitroSpeed;

        accelerationTimer = _tronGameManager.accelerationTimerMax;
        accelerationSpeed_Max = _tronGameManager.accelerationSpeedMax;


        if (!isDead && StartGame && ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart))
        {

            if (accelerationSpeed_Counter < accelerationSpeed_Max)
            {
                if (MyCarDataReceiver.Network_ID == 1)
                {
                    UIManager.instance.SpeedBar_1.fillAmount = accelerationSpeed_Counter / accelerationSpeed_Max;
                    UIManager.instance.SpeedTimeText_1.text = accelerationTimer.ToString();
                    UIManager.instance.SpeedMaxText_1.text = accelerationSpeed_Max.ToString();
                    UIManager.instance.SpeedTexT_1.text = accelerationSpeed_Counter.ToString();
                }
                else
                {
                    UIManager.instance.SpeedBar_2.fillAmount = accelerationSpeed_Counter / accelerationSpeed_Max;
                    UIManager.instance.SpeedTimeText_2.text = accelerationTimer.ToString();
                    UIManager.instance.SpeedMaxText_2.text = accelerationSpeed_Max.ToString();
                    UIManager.instance.SpeedText_2.text = accelerationSpeed_Counter.ToString();
                }
                accelerationSpeed_Counter += Time.fixedDeltaTime * ((accelerationSpeed_Max) / accelerationTimer);
            }
            //_characterController.Move(CarRotationObject.transform.forward * ((ComputedValues() * ReduceValues() )* Time.fixedDeltaTime));
            InputSystem();

        }
        else
        {
            if (isDead)
            {
                DieTimer += Time.fixedDeltaTime;
                if (DieTimer > 1)
                {
                    if (!signalSent)
                    {
                        signalSent = true;
                        if (MyCarDataReceiver.Network_ID == 1)
                            transform.position = new Vector3(-5, 10, 0);
                        if (MyCarDataReceiver.Network_ID == 2)
                            transform.position = new Vector3(5, 10, 0);
                        StopCoroutine("DelayRespawn");
                        StartCoroutine("DelayRespawn");
                        accelerationSpeed_Counter = 0;
                    }
                }
            }
        }

    }
    #endregion
    //==========================================================================================================================
    #region INPUT

    void InputSystem()
    {
        myRigid.drag = _tronGameManager.Drag_Value;
        myRigid.angularDrag = _tronGameManager.AngularDrag_Value;
        myRigid.mass = _tronGameManager.Mass_Value;
        //if (Input.GetKey(KeyCode.W))
        {
            //Debug.LogError("forceee " + _tronGameManager.Force_Value);
            //myRigid.AddForce(CarRotationObject.transform.forward * _tronGameManager.Force_Value);
            myRigid.AddForce(CarRotationObject.transform.forward *(_tronGameManager.Force_Value +(ComputedValues() * ReduceValues()) * Time.fixedDeltaTime));
        }
        /*
        if(FlipSwitch)
            myRigid.angularVelocity = new Vector3(0, -Input.GetAxis("Horizontal") * tf, 0);
        else
            myRigid.angularVelocity = new Vector3(0, Input.GetAxis("Horizontal") * tf, 0);

        
        //refactoring using force
        return;
        */


        if (_moveRight)
        {
            MoveRight();
        }
        if (_moveLeft)
        {
            MoveLeft();
        }
        if (Input.GetKey(KeyCode.A))
        {
                MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
                MoveRight();
        }
    }


    bool _moveLeft, _moveRight;
    public void HoldRight()
    {
        _moveRight = true;
        _moveLeft = false;
    }
    public void ReleaseRight()
    {
        _moveRight = false;
    }
    public void HoldLeft()
    {
        _moveLeft = true;
        _moveRight = false;
    }
    public void ReleaseLeft()
    {
        _moveLeft = false;
    }

    void MoveRight()
    {
        /*
        if(!FlipSwitch)
            currentRotation_Y += rotationSpeed * Time.fixedDeltaTime;
        else
            currentRotation_Y -= rotationSpeed * Time.fixedDeltaTime;*/

        float tf = Mathf.Lerp(0, _tronGameManager.Torque_Value, myRigid.velocity.magnitude / 2);
        if (!FlipSwitch)
            myRigid.angularVelocity = new Vector3(0, 1 * tf, 0);
        else
            myRigid.angularVelocity = new Vector3(0, -1 * tf, 0);
    }
    void MoveLeft()
    {
        /*
        if (!FlipSwitch)
            currentRotation_Y -= rotationSpeed * Time.fixedDeltaTime;
        else
            currentRotation_Y += rotationSpeed * Time.fixedDeltaTime;*/

        float tf = Mathf.Lerp(0, _tronGameManager.Torque_Value, myRigid.velocity.magnitude / 2);
        if(!FlipSwitch)
        myRigid.angularVelocity = new Vector3(0, -1 * tf, 0);
        else
            myRigid.angularVelocity = new Vector3(0, 1 * tf, 0);
    }
    #endregion
    //==========================================================================================================================
    #region COLLISION
    void OnTriggerEnter(Collider hit)
    {
        return;
        if (!TronGameManager.Instance.NetworkStart)
        {
            if (hit.gameObject.name.Contains("Missle") && hit.GetComponent<MissleScript>().PlayerController_ID != 1)
            {
                if (hit.GetComponent<MissleScript>()._missleType == MissleScript.MISSLE_TYPE.STUN)
                {
                    MyCarDataReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_STUN);
                }
                if (hit.GetComponent<MissleScript>()._missleType == MissleScript.MISSLE_TYPE.BLIND)
                {
                    MyCarDataReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_BLIND);
                }
                if (hit.GetComponent<MissleScript>()._missleType == MissleScript.MISSLE_TYPE.CONFUSE)
                {
                    MyCarDataReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_CONFUSE);
                }
            }
        }



        if (!isDead && StartGame && ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart))
        {
            if (hit.gameObject.tag == "Wall" ||hit.gameObject.tag == "Trail" || (hit.gameObject.tag == "Car" && hit.gameObject.name != gameObject.name))
            {
                if (!TronGameManager.Instance.NetworkStart)
                {
                    if (localShieldIsActive == false)
                    {
                        Die();
                    }
                    return;
                }

                if (MyCarDataReceiver.GetShieldSwitch() == false)
                {
                    Die();
                }
            }
            
        }
    }
    #endregion
    //==========================================================================================================================
    #region PLAYER DEATH
    public void Die()
    {
        //if(MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID)
        if ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart)
        {
            isDead = true;

            transform.position = new Vector3(transform.position.x, 10, transform.position.z);

            if (_tronGameManager.NetworkStart == false)
            {
                if (gameObject.name == "Car1")
                {
                    AIMode_HpBar -= 1;
                    _tronGameManager.ReduceHPOfPlayer(1, AIMode_HpBar );
                }
                _trailCollision.SetEmiision(false);
                try
                {
                    GetComponent<AI_Behaviour>().enabled = false;
                }
                catch
                {
                }
            }
            else
            {
                MyCarDataReceiver.ResetTrail(false);
                MyCarDataReceiver.ReduceHealth();
            }
        }
    }
    IEnumerator DelayRespawn()
    {
        yield return new WaitForSeconds(3);
        if (MyCarDataReceiver.Network_ID == 1)
            transform.position = new Vector3(-5, 1, 0);
        if (MyCarDataReceiver.Network_ID == 2)
            transform.position = new Vector3(5, 1, 0);
        else
            transform.position = new Vector3(0, 1, 0);

        CarRotationObject.transform.eulerAngles = Vector3.zero;
        currentRotation_Y = 0;
        isDead = false;
        signalSent = false;
        DieTimer = 0;
        yield return new WaitForSeconds(.5f);
        try
        {
            MyCarDataReceiver.ResetTrail(true);
        }
        catch
        {
        }

        if(_tronGameManager.NetworkStart == false)
        {
            try
            {
                GetComponent<AI_Behaviour>().enabled = true;
            }
            catch { }
            _trailCollision.SetEmiision(true);
        }
    }
    #endregion
}
