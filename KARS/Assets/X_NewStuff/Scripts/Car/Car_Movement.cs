using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Movement : MonoBehaviour
{
    //==========================================================================================================================
    #region VARIABLES
    [SerializeField]
    private AudioSource engineSounds;

    [SerializeField]
    private Camera_Behaviour _camBehaviour;
    [SerializeField]
    private Transform centerOfMass;
    [SerializeField]
    private Transform[] Wheels;
    [SerializeField]
    private TronGameManager _tronGameManager;


    //NETWORK ACCESSIBLE
    public Camera myCam;
    public Transform CarRotationObject;
    public TrailCollision _trailCollision;

    public bool StartGame;
    private bool isREady;
    public void SetStartGame(bool _switch)
    {
        StartGame = _switch;
    }
    public void SetReady(bool _Switch)
    {
        isREady = _Switch;
    }
    public bool GetReady()
    {
        return isREady;
    }
    private bool FlipSwitch;
    public bool FlipCarSwitch
    {
        get { return FlipSwitch; }
        set { FlipSwitch = value; }
    }


    //LOCAL COMPONENTS
    Car_DataReceiver MyCarDataReceiver;
    CharacterController _characterController;
    Rigidbody myRigid;




    private float movementSpeed;


    float rotationSpeed;



    bool isDead;

    bool signalSent;
    float DieTimer;

    //SINGLE PLAYER
    public float AIMode_HpBar;
    public bool localShieldIsActive;
    public GameObject localShield;


    float accelerationSpeed_Counter;

    float accelerationTimer;
    float accelerationSpeed_Max;

    float NitrosSpeed;

    [SerializeField]
    private bool disableWheels;

    public bool DisableWheels
    {
        get
        {
            return disableWheels;
        }

        set
        {
            disableWheels = value;
        }
    }

    #endregion
    //==========================================================================================================================
    #region INIT
    void Awake()
    {
        AIMode_HpBar = 5;
        _characterController = GetComponent<CharacterController>();
        MyCarDataReceiver = GetComponent<Car_DataReceiver>();
        myRigid = GetComponent<Rigidbody>();
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
        if (MyCarDataReceiver.GetStunSwitch() == true)
            _finalValue = .1f;


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

        if (!isDead && !disableWheels && ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart))
        {
            if (accelerationSpeed_Counter < accelerationSpeed_Max)
            {
                if (MyCarDataReceiver.Network_ID == 1)
                {
                    UIManager.Instance.SpeedBar_1.fillAmount = accelerationSpeed_Counter / accelerationSpeed_Max;
                    UIManager.Instance.SpeedTimeText_1.text = accelerationTimer.ToString();
                    UIManager.Instance.SpeedMaxText_1.text = accelerationSpeed_Max.ToString();
                    UIManager.Instance.SpeedTexT_1.text = accelerationSpeed_Counter.ToString();
                }
                else
                {
                    UIManager.Instance.SpeedBar_2.fillAmount = accelerationSpeed_Counter / accelerationSpeed_Max;
                    UIManager.Instance.SpeedTimeText_2.text = accelerationTimer.ToString();
                    UIManager.Instance.SpeedMaxText_2.text = accelerationSpeed_Max.ToString();
                    UIManager.Instance.SpeedText_2.text = accelerationSpeed_Counter.ToString();
                }
                accelerationSpeed_Counter += Time.fixedDeltaTime * ((accelerationSpeed_Max) / accelerationTimer);
            }
            engineSounds.pitch = (accelerationSpeed_Counter / accelerationSpeed_Max)*3;
            //_characterController.Move(CarRotationObject.transform.forward * ((ComputedValues() * ReduceValues() )* Time.fixedDeltaTime));


            if (MyCarDataReceiver.GetNitroSwitch() == true)
                NitrosSpeed = _tronGameManager.nitroSpeed;
            else
                NitrosSpeed = 0;

            if (!Input.GetKey(KeyCode.S))
            {
                myRigid.AddForce(CarRotationObject.transform.forward *
                    ((_tronGameManager.Force_Value + NitrosSpeed + accelerationSpeed_Counter * Time.fixedDeltaTime) * ReduceValues()));
            }

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

                        transform.position = new Vector3(transform.position.x,
                                                            -2,
                                                            transform.position.z);
                        //StopCoroutine("DelayRespawn");
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
    float driftTimer;


    void InputSystem()
    {
        turningRate = _tronGameManager.turningRate;
        turningForce = _tronGameManager.turningForce;
        turningStraightDamping = _tronGameManager.turningStraightDamping;

        if (_moveRight || Input.GetKey(KeyCode.D))
        {
            if (FlipSwitch == false)
                MoveRight();
            else
                MoveLeft();
        }
        else if (_moveLeft || Input.GetKey(KeyCode.A))
        {
            if (FlipSwitch == false)
                MoveLeft();
            else
                MoveRight();
        }
        else
        {
            driftTimer = 0;
            _camBehaviour.ReturnToDefault();
            for (int i = 0; i < Wheels.Length; i++)
            {
                Wheels[i].localRotation = Quaternion.Lerp(Wheels[i].localRotation, Quaternion.Euler(new Vector3(0, 0, 90)), WheelLerpSpeed);
            }
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
        driftTimer = 0;
        _moveRight = false;
    }
    public void HoldLeft()
    {
        _moveLeft = true;
        _moveRight = false;
    }
    public void ReleaseLeft()
    {
        driftTimer = 0;
        _moveLeft = false;
    }

    float WheelLerpSpeed = .5f;
    void MoveRight()
    {
        _camBehaviour.RotateLeft(1);
        for (int i = 0; i < Wheels.Length; i++)
        {
            Wheels[i].localRotation = Quaternion.Lerp(Wheels[i].localRotation, Quaternion.Euler(new Vector3(0, 35, 90)), WheelLerpSpeed);
        }
        _currentTurningForce -= turningRate;
        driftTimer += Time.deltaTime;
        if(driftTimer > 1)
            AudioManager.Instance.Play_Loop(AUDIO_CLIP.CAR_DRIFT);
        
        _currentTurningForce = Mathf.Clamp(_currentTurningForce, -turningForce, turningForce);
    }
    void MoveLeft()
    {
        _camBehaviour.RotateRight(1);
        for (int i = 0; i < Wheels.Length; i++)
        {
            Wheels[i].localRotation = Quaternion.Lerp(Wheels[i].localRotation, Quaternion.Euler(new Vector3(0, -35, 90)), WheelLerpSpeed);
        }
        _currentTurningForce += turningRate;
        driftTimer += Time.deltaTime;
        if (driftTimer > 1)
            AudioManager.Instance.Play_Loop(AUDIO_CLIP.CAR_DRIFT);
        _currentTurningForce = Mathf.Clamp(_currentTurningForce, -turningForce, turningForce);
    }
    #endregion

    //==========================================================================================================================
    #region COLLISION
    void OnTriggerEnter(Collider hit)
    {
        if (!isDead && !disableWheels && ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart))
        {
            if (hit.gameObject.tag == "Wall" || hit.gameObject.tag == "Trail" || (hit.gameObject.tag == "Car" && hit.gameObject.name != gameObject.name))
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
                    MyCarDataReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_EXPLOSION);
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
        if (!isDead)
        {
            isDead = true;
            myRigid.Sleep();
            myRigid.useGravity = false;
            if ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart)
            {
                transform.position = new Vector3(transform.position.x,
                                                    -2,
                                                    transform.position.z);

                UIManager.Instance.SetExplosionPanel(true);
                if (_tronGameManager.NetworkStart == false)
                {
                    if (gameObject.name == "Car1")
                    {
                        AIMode_HpBar -= 1;
                        _tronGameManager.ReduceHPOfPlayer(1, AIMode_HpBar);
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
                    engineSounds.mute = true;
                }
            }
        }
    }
    IEnumerator DelayRespawn()
    {
        yield return new WaitForSeconds(2);
        UIManager.Instance.SetRespawnScreen(true);
        UIManager.Instance.SetExplosionPanel(false);
        yield return new WaitForSeconds(5);

        MyCarDataReceiver.SendNetworkDisable(false, NetworkPlayerStatus.ACTIVATE_EXPLOSION);
        MyCarDataReceiver.ReceiveDisableSTate(false, NetworkPlayerStatus.ACTIVATE_EXPLOSION);
        if (MyCarDataReceiver.Network_ID == 1)
            transform.position = _tronGameManager.spawnPlayerPosition[0].position;
        if (MyCarDataReceiver.Network_ID == 2)
            transform.position = _tronGameManager.spawnPlayerPosition[1].position;
        else
            transform.position = _tronGameManager.spawnPlayerPosition[0].position;

        UIManager.Instance.SetRespawnScreen(false);

        CarRotationObject.transform.localEulerAngles = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

        myRigid.useGravity = true;

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

        _trailCollision.SetEmiision(true);
        engineSounds.mute = false;
    }
    #endregion

    
}
