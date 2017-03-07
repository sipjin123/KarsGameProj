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

    float ReduceValues()
    {
        float _finalValue = 1;

        if (MyCarDataReceiver.GetSlowSwitch() == true)
            _finalValue = .8f;


        return _finalValue;
    }

    void FixedUpdate()
    {
        movementSpeed = _tronGameManager.MovementSpeed;
        rotationSpeed = _tronGameManager.rotationSpeed;

        NitrosSpeed = _tronGameManager.nitroSpeed;

        accelerationTimer = _tronGameManager.accelerationTimerMax;
        accelerationSpeed_Max = _tronGameManager.accelerationSpeedMax;

        myRigid.drag = _tronGameManager.Drag_Value;
        myRigid.angularDrag = _tronGameManager.AngularDrag_Value;
        myRigid.mass = _tronGameManager.Mass_Value;

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



            if (MyCarDataReceiver.GetNitroSwitch() == true)
            {
                NitrosSpeed = _tronGameManager.nitroSpeed;
                Debug.LogError("NITROS IS ACTIVTE!");
            }
            else
            {
                NitrosSpeed = 0;
            }

            myRigid.AddForce(CarRotationObject.transform.forward *
                ((_tronGameManager.Force_Value + NitrosSpeed + accelerationSpeed_Counter  * Time.fixedDeltaTime) * ReduceValues()));
            
            if (MyCarDataReceiver.GetFlySwitch() == true)
            {
                if (transform.position.y < 6)
                    myRigid.AddForce(transform.up * 25);
            }
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

    float _currentTurningForce = 0;

    public float turningRate = 5f;
    public float turningForce = 50;
    public float turningStraightDamping = 0.9f;

    public float CurrentTurningForceRatio
    {
        get
        {
            return _currentTurningForce / turningForce;
        }
    }
    bool _moveLeft, _moveRight;
    void InputSystem()
    {
        turningRate = _tronGameManager.turningRate;
        turningForce = _tronGameManager.turningForce;
        turningStraightDamping = _tronGameManager.turningStraightDamping;

        if (_moveRight || Input.GetKey(KeyCode.D))
        {
            _currentTurningForce = Mathf.Clamp(_currentTurningForce - turningRate, -turningForce, turningForce);
            //MoveRight();
        }
        else if (_moveLeft || Input.GetKey(KeyCode.A))
        {
            _currentTurningForce = Mathf.Clamp(_currentTurningForce + turningRate, -turningForce, turningForce);
            //MoveLeft();
        }
        else
        {
            _currentTurningForce *= turningStraightDamping;
        }

        myRigid.AddRelativeTorque(Vector3.down * _currentTurningForce);
    }

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
        float torqer = _tronGameManager.Torque_Value;
        torqer = _currentTurningForce;
        float tf = Mathf.Lerp(0, torqer, myRigid.velocity.magnitude / 2);

        if (!FlipSwitch)
            myRigid.angularVelocity = new Vector3(0, 1 * tf, 0);
        else
            myRigid.angularVelocity = new Vector3(0, -1 * tf, 0);
    }
    void MoveLeft()
    {
        float torqer = _tronGameManager.Torque_Value;
        torqer = _currentTurningForce;
        float tf = Mathf.Lerp(0, torqer, myRigid.velocity.magnitude / 2);

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
        myRigid.Sleep();
        myRigid.useGravity = false;
        //if(MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID)
        if ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart)
        {
            isDead = true;

            if(MyCarDataReceiver.Network_ID == 1)
            transform.position = new Vector3(_tronGameManager.spawnPlayerPosition[0].position.x, 10, _tronGameManager.spawnPlayerPosition[0].position.z);
            else
                transform.position = new Vector3(_tronGameManager.spawnPlayerPosition[1].position.x, 10, _tronGameManager.spawnPlayerPosition[1].position.z);

            UIManager.instance.SetRespawnScreen(true);
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
            transform.position = _tronGameManager.spawnPlayerPosition[0].position;
        if (MyCarDataReceiver.Network_ID == 2)
            transform.position = _tronGameManager.spawnPlayerPosition[1].position;
        else
            transform.position = _tronGameManager.spawnPlayerPosition[0].position;

        UIManager.instance.SetRespawnScreen(false);

        CarRotationObject.transform.localEulerAngles = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

        myRigid.useGravity = true;

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
