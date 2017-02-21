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


    



    //SINGLE PLAYER
    public float AIMode_HpBar;
    public bool localShieldIsActive;
    public GameObject localShield;



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
    public void ActiveShieldFromButton()
    {
        ActiveLocalShield(!localShieldIsActive);
    }
    public void ActiveLocalShield(bool _switch)
    {
        localShieldIsActive = _switch;
        localShield.SetActive(_switch);
    }
    //==========================================================================================================================
    void FixedUpdate()
    {
        movementSpeed = _tronGameManager.MovementSpeed;
        rotationSpeed = _tronGameManager.rotationSpeed;



        if (!isDead && StartGame && ((TronGameManager.Instance.NetworkStart && MyCarDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart))
        {
            if (MyCarDataReceiver.GetStunSwitch() == false)
            {
                _characterController.Move(CarRotationObject.transform.forward * movementSpeed * Time.fixedDeltaTime);
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
                    }
                }
            }
        }

    }
    //==========================================================================================================================
    void InputSystem()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).position.x > Screen.width * .5f)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
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
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
    }
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
    bool signalSent;
    float DieTimer;
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

    void MoveRight()
    {
        currentRotation_Y += rotationSpeed * Time.fixedDeltaTime;
        CarRotationObject.transform.eulerAngles = new Vector3(0,currentRotation_Y,0);
    }
    void MoveLeft()
    {
        currentRotation_Y -= rotationSpeed * Time.fixedDeltaTime;
        CarRotationObject.transform.eulerAngles = new Vector3(0, currentRotation_Y, 0);
    }
}
