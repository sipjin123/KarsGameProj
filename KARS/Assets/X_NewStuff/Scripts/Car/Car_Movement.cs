using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Movement : MonoBehaviour
{
    //==========================================================================================================================
    #region VARIABLES
    //NETWORK ACCESSIBLE
    public Transform CarRotationObject;
    public TrailCollision _trailCollision;

    private bool StartGame;
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
    private bool flipCarSwitch;
    public bool FlipCarSwitch
    {
        get { return flipCarSwitch; }
        set { flipCarSwitch = value; }
    }


    //LOCAL COMPONENTS
    TronGameManager _tronGameManager;
    Car_DataReceiver MyCarDataReceiver;
    CharacterController _characterController;
    Rigidbody myRigid;
    
    [SerializeField]
    private AudioSource engineSounds;
    [SerializeField]
    private Camera_Behaviour _camBehaviour;
    [SerializeField]
    private Transform centerOfMass;
    [SerializeField]
    private Transform[] Wheels;



    float WheelLerpSpeed = .5f;
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
    bool _moveLeft, _moveRight;



    //PLAYER STATS
    bool isDead;
    private float movementSpeed;
    float rotationSpeed;
    
    float accelerationSpeed_Counter;
    float accelerationTimer;
    float accelerationSpeed_Max;

    float NitrosSpeed;

    //ROBERT STATS
    float _currentTurningForce = 0;

    float turningRate = 5f;
    float turningForce = 50;
    float turningStraightDamping = 0.9f;

    public float CurrentTurningForceRatio
    {
        get
        {
            return _currentTurningForce / turningForce;
        }
    }
    float driftTimer;

    #endregion
    //==========================================================================================================================
    #region INIT
    void Awake()
    {
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
    #region MOVEMENT

    float SlowThreshold()
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
        //COPY FROM TWEAKERS
        movementSpeed = _tronGameManager.MovementSpeed;
        rotationSpeed = _tronGameManager.rotationSpeed;

        NitrosSpeed = _tronGameManager.nitroSpeed;

        accelerationTimer = _tronGameManager.accelerationTimerMax;
        accelerationSpeed_Max = _tronGameManager.accelerationSpeedMax;

        myRigid.drag = _tronGameManager.Drag_Value;
        myRigid.angularDrag = _tronGameManager.AngularDrag_Value;
        myRigid.mass = _tronGameManager.Mass_Value;


        if (!isDead && !disableWheels && NetworkCapable())
        {
            if (accelerationSpeed_Counter < accelerationSpeed_Max)
            {
                if (MyCarDataReceiver.GetNetwork_ID() == 1)
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
                    ((_tronGameManager.Force_Value + NitrosSpeed + accelerationSpeed_Counter * Time.fixedDeltaTime) * SlowThreshold()));
            }

            if (MyCarDataReceiver.GetFlySwitch() == true)
            {
                if (transform.position.y < 6)
                    myRigid.AddForce(transform.up * 25);
            }
            InputSystem();

        }

    }
    #endregion
    //==========================================================================================================================
    #region INPUT
    void InputSystem()
    {
        turningRate = _tronGameManager.turningRate;
        turningForce = _tronGameManager.turningForce;
        turningStraightDamping = _tronGameManager.turningStraightDamping;

        if (_moveRight || Input.GetKey(KeyCode.D))
        {
            if (flipCarSwitch == false)
                MoveRight();
            else
                MoveLeft();
        }
        else if (_moveLeft || Input.GetKey(KeyCode.A))
        {
            if (flipCarSwitch == false)
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
        if (!isDead && !disableWheels && NetworkCapable())
        {
            if (hit.gameObject.tag == "Wall" || hit.gameObject.tag == "Trail" || (hit.gameObject.tag == "Car" && hit.gameObject.name != gameObject.name))
            {
                if (MyCarDataReceiver.GetShieldSwitch() == false)
                {
                    MyCarDataReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_EXPLOSION);
                    UIManager.Instance.SetExplosionPanel(true);
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
        if (NetworkCapable())
        {
            if (!isDead)
            {
                isDead = true;
                myRigid.Sleep();
                myRigid.useGravity = false;
                transform.position = new Vector3(transform.position.x,  -2,  transform.position.z);

                MyCarDataReceiver.SendNetworkStatus(false, NetworkPlayerStatus.ACTIVATE_TRAIL);
                MyCarDataReceiver.ReduceHealth();
                engineSounds.mute = true;

                StopCoroutine("DelayRespawn");
                StartCoroutine("DelayRespawn");
                accelerationSpeed_Counter = 0;
            }
        }
    }
    bool NetworkCapable()
    {
        if ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.GetNetwork_ID() == GameSparkPacketHandler.Instance.GetPeerID()))
        {
            return true;
        }
        return false;
    }
    IEnumerator DelayRespawn()
    {
        yield return new WaitForSeconds(2);

        UIManager.Instance.SetRespawnScreen(true);
        UIManager.Instance.SetExplosionPanel(false);
        UIManager.Instance.Set_Canvas_GameInit(false);

        yield return new WaitForSeconds(3);

        MyCarDataReceiver.SendNetworkStatus(false, NetworkPlayerStatus.ACTIVATE_EXPLOSION);
        MyCarDataReceiver.ReceivePlayerSTate(false, NetworkPlayerStatus.ACTIVATE_EXPLOSION);

        if(MyCarDataReceiver.GetNetwork_ID() == 0)
            transform.position = _tronGameManager.spawnPlayerPosition[0].position;
        else
            transform.position = _tronGameManager.spawnPlayerPosition[MyCarDataReceiver.GetNetwork_ID() - 1].position;
        CarRotationObject.transform.localEulerAngles = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;

        myRigid.useGravity = true;
        engineSounds.mute = false;
        isDead = false;

        MyCarDataReceiver.Access_ResetPowerups();
        _moveLeft = false;
        _moveRight= false;

        UIManager.Instance.SetRespawnScreen(false);
        UIManager.Instance.Set_Canvas_GameInit(false);
        MyCarDataReceiver.SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_TRAIL);
    }
    #endregion
}
