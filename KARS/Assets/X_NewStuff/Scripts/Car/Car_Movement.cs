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
    #endregion
    //==========================================================================================================================
    #region INIT
    void Awake()
    {
        AIMode_HpBar = 5;
        _characterController = GetComponent<CharacterController>();
        MyCarDataReceiver = GetComponent<Car_DataReceiver>();
        currentRotation_Y = CarRotationObject.transform.eulerAngles.y;
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
    void FixedUpdate()
    {
        movementSpeed = _tronGameManager.MovementSpeed;
        rotationSpeed = _tronGameManager.rotationSpeed;

        NitrosSpeed = _tronGameManager.nitroSpeed;

        accelerationTimer = _tronGameManager.accelerationTimerMax;
        accelerationSpeed_Max = _tronGameManager.accelerationSpeedMax;


        if (!isDead && StartGame && ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart))
        {
            if (MyCarDataReceiver.GetStunSwitch() == false)
            {
                if(accelerationSpeed_Counter < accelerationSpeed_Max)
                {
                    if (MyCarDataReceiver.Network_ID == 1)
                    {
                        UIManager.instance.SpeedBar_1.fillAmount = accelerationSpeed_Counter / accelerationSpeed_Max;
                        UIManager.instance.SpeedTimeText_1.text = accelerationSpeed_Counter.ToString();
                        UIManager.instance.SpeedMaxText_1.text = accelerationSpeed_Max.ToString();
                        UIManager.instance.SpeedTexT_1.text = accelerationSpeed_Counter.ToString();
                    }
                    else
                    {
                        UIManager.instance.SpeedBar_2.fillAmount = accelerationSpeed_Counter / accelerationSpeed_Max;
                        UIManager.instance.SpeedTimeText_2.text = accelerationSpeed_Counter.ToString();
                        UIManager.instance.SpeedMaxText_2.text = accelerationSpeed_Max.ToString();
                        UIManager.instance.SpeedText_2.text = accelerationSpeed_Counter.ToString();
                    }
                    accelerationSpeed_Counter += Time.fixedDeltaTime * ((accelerationSpeed_Max) /accelerationTimer);
                }
                if(NitrosActive == false)
                {
                    NitrosSpeed = 0;
                }
                _characterController.Move(CarRotationObject.transform.forward * ((movementSpeed + accelerationSpeed_Counter + NitrosSpeed) * Time.fixedDeltaTime));
            }
            else
            {
                accelerationSpeed_Counter = 1;
                if (NitrosActive == false)
                {
                    NitrosSpeed = 0;
                }
                _characterController.Move(CarRotationObject.transform.forward * ((movementSpeed + accelerationSpeed_Counter + NitrosSpeed) * Time.fixedDeltaTime));
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
    //==========================================================================================================================
    #region INPUT
    void InputSystem()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).position.x > Screen.width * .5f)
            {
                if (FlipSwitch)
                    MoveLeft();
                else
                    MoveRight();
            }
            else
            {
                if (FlipSwitch)
                    MoveRight();
                else
                    MoveLeft();
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (FlipSwitch)
                MoveRight();
            else
                MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (FlipSwitch)
                MoveLeft();
            else
                MoveRight();
        }
        if (Input.GetKey(KeyCode.X))
        {
            Die();
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.mousePosition.x > Screen.width * .5f)
            {
                if (FlipSwitch)
                    MoveLeft();
                else
                    MoveRight();
            }
            else
            {
                if (FlipSwitch)
                    MoveRight();
                else
                    MoveLeft();
            }
        }
    }

    void MoveRight()
    {
        currentRotation_Y += rotationSpeed * Time.fixedDeltaTime;
        CarRotationObject.transform.eulerAngles = new Vector3(0, currentRotation_Y, 0);
    }
    void MoveLeft()
    {
        currentRotation_Y -= rotationSpeed * Time.fixedDeltaTime;
        CarRotationObject.transform.eulerAngles = new Vector3(0, currentRotation_Y, 0);
    }
    #endregion
    //==========================================================================================================================
    void OnTriggerEnter(Collider hit)
    {
        if (!isDead && StartGame && ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart))
        {
            UIManager.instance.GameUpdateText.text += "\nI WAS hit by: "+hit.gameObject.name;
            /*
            if (hit.gameObject.name.Contains("Missle"))
            {
                MyCarDataReceiver.ActiveStunFromButton();
            }*/
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
