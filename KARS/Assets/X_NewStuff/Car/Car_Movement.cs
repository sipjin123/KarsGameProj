using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Movement : MonoBehaviour {

    private float movementSpeed;

    CharacterController _characterController;

    float currentRotation_Y;
    float rotationSpeed;

    public Transform CarRotationObject;

    [SerializeField]
    public TrailCollision _trailCollision;
    public Camera myCam;


    TronGameManager _tronGameManager;

    Car_DataReceiver _carDataReceiver;
    bool isDead;
    public bool StartGame;
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _carDataReceiver = GetComponent<Car_DataReceiver>();
        currentRotation_Y = CarRotationObject.transform.eulerAngles.y;
    }

    void Start()
    {

        _tronGameManager = TronGameManager.Instance.GetComponent<TronGameManager>();
    }

    void FixedUpdate()
    {
        movementSpeed = _tronGameManager.MovementSpeed;
        rotationSpeed = _tronGameManager.rotationSpeed;
        if (!isDead && StartGame && ((TronGameManager.Instance.NetworkStart && _carDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart))
        {
            _characterController.Move(CarRotationObject.transform.forward * movementSpeed * Time.fixedDeltaTime);
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
                        if (_carDataReceiver.Network_ID == 1)
                            transform.position = new Vector3(-5, 10, 0);
                        if (_carDataReceiver.Network_ID == 2)
                            transform.position = new Vector3(5, 10, 0);
                        StartCoroutine("DelayRespawn");
                    }
                }
            }
        }

    }
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

    void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.tag == "Trail")
        {
            Die();
        }
    }

    bool signalSent;
    float DieTimer;
    public void Die()
    {
        //if(_carDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID)
        if ((TronGameManager.Instance.NetworkStart && _carDataReceiver.Network_ID == GameSparkPacketReceiver.Instance.PeerID) || !TronGameManager.Instance.NetworkStart)
        {
            isDead = true;
            try
            {
                _carDataReceiver.ResetTrail(false);
            }
            catch
            { }
            transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        }
    }
    IEnumerator DelayRespawn()
    {
        yield return new WaitForSeconds(3);
        if (_carDataReceiver.Network_ID == 1)
            transform.position = new Vector3(-5, 1, 0);
        if (_carDataReceiver.Network_ID == 2)
            transform.position = new Vector3(5, 1, 0);
        CarRotationObject.transform.eulerAngles = Vector3.zero;
        currentRotation_Y = 0;
        isDead = false;
        signalSent = false;
        DieTimer = 0;
        yield return new WaitForSeconds(.5f);
        try
        {
            _carDataReceiver.ResetTrail(true);
        }
        catch
        { }
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
