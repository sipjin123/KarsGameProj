using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleCarController : MonoBehaviour {

    [SerializeField]
    private Transform Objective;
    [SerializeField]
    private Transform Car;
    [SerializeField]
    private Camera carCamera;
    [SerializeField]
    private float camDist = 10;

    private Rigidbody my_rigid;
    [SerializeField]
    private float speed = 15;
    private float rotSpeed = 60;

    [SerializeField]
    private float currentRot = 0;

    private float rotTime = 0;

    [SerializeField]
    private Transform carObject;

    [SerializeField]
    private Transform flagCamParent;
    [SerializeField]
    private bool hasFlag = false;

    public float invis = 0;
    private float maxInvis = 10;


    [SerializeField]
    private Text ScoreText;
    [SerializeField]
    private Text ScoreText_p2;
    [SerializeField]
    private Text TimeText;

    private float p1Score = 0;
    private float p2Score = 0;
    private int timeLeft = 90;






    //*************************************************************
    //GAMESPARKS
    GameSparks_DataSender _GSDataSender;

    void Awake()
    {
        _GSDataSender = GetComponent<GameSparks_DataSender>();
        _GSDataSender.ObjToTranslate = gameObject;
        _GSDataSender.ObjToRotate = carObject.gameObject;
        _GSDataSender.PlayerCam = carCamera;
    }
    //*************************************************************



    public void LoseFlag()
    {
        if (hasFlag)
        {
            hasFlag = false;
            carCamera.transform.SetParent(this.transform);
            carCamera.transform.localPosition = new Vector3(0, 2.25f, -3.77f);
            Objective.SetParent(null);
        }
    }

    void Start()
    {
        Objective = GameObject.FindGameObjectWithTag("Flag").transform;
        my_rigid = this.GetComponent<Rigidbody>();


    }

    void OnTriggerEnter(Collider col)
    {
        if (!hasFlag && col.tag == "Flag" && invis <= 0)
        {

            hasFlag = true;
            Objective.transform.SetParent(this.transform);
            carCamera.transform.SetParent(flagCamParent);
            Objective.transform.localPosition = Vector3.zero;
            carCamera.transform.localPosition = new Vector3(0, 2.25f, -3.77f);
            carCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
            invis = maxInvis;

        }

    }
    
    void Update()
    {
        //*************************************************************
        //GAMESPARKS
        if (_GSDataSender.HasControllableObject == false)//GAME SPARK INITIALIZATION
            return;
        if (int.Parse(GameSparksManager.Instance.PeerID) != _GSDataSender.NetworkID)//GAME SPARK ID
            return;

        _GSDataSender.SendTankMovement(_GSDataSender.NetworkID, transform.position, carObject.transform.eulerAngles);
        //*************************************************************
        invis -= Time.deltaTime;
        if (Input.touchCount > 0)
        {
            foreach (Touch _touches in Input.touches)
            {
                if ((_touches.rawPosition.x > Screen.height / 2))
                {
                    currentRot += rotSpeed * Time.deltaTime;

                }
                else
                {
                    currentRot -= rotSpeed * Time.deltaTime;
                    //this.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);

                }
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            currentRot -= rotSpeed * Time.deltaTime;
            //my_rigid.AddForce(transform.forward * speed / 3);

        }
        else if (Input.GetKey(KeyCode.D))
        {
            currentRot += rotSpeed * Time.deltaTime;
            //this.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
            //my_rigid.AddForce(transform.forward * speed / 3);

        }
        else
        {
            //my_rigid.AddForce(transform.forward * speed); //Cannot rotate parent since it disrupts the camera orbit
        }
        //

        if (currentRot > 360)
        {
            currentRot = 0;
        }
        else if (currentRot < 0)
        {
            currentRot = 360;
        }
        this.transform.Translate(CarMovement());
        carObject.rotation = Quaternion.Euler(0, currentRot, 0);


        if (!hasFlag)
        {
            carCamera.transform.LookAt(Objective);
            Vector3 pivot = Vector3.Normalize(this.transform.localPosition - Objective.transform.localPosition) * 500;
            Car.transform.localPosition = pivot;
            Vector3 newCameraPos = Vector3.Normalize(Car.localPosition - Objective.transform.localPosition) * camDist;
            carCamera.transform.localPosition = new Vector3(newCameraPos.x, 2.25f, newCameraPos.z);
        }
        else
        {

            flagCamParent.transform.rotation = Quaternion.Euler(0, currentRot, 0);
        }
        if (Objective.parent != this.gameObject.transform)
        {
            hasFlag = false;
        }

        if (hasFlag)
        {
            p1Score += Time.deltaTime;
            ScoreText.text = "Player 1: "+  Mathf.RoundToInt(p1Score);
        }
    }

    private Vector3 CarMovement()
    {
        Vector3 carVelocity = Vector3.zero;
        if (currentRot >= 0 && currentRot <= 90)
        {
            carVelocity = new Vector3(
            1 - ((90 - currentRot) / 90),
            0,
            1 - (currentRot / 90)) * Time.deltaTime * speed;
        }
        else if (currentRot > 90 && currentRot <= 180)
        {
            carVelocity = new Vector3(
            1 - ((currentRot - 90) / 90),
            0,
           ((90 - (currentRot - 90)) / 90) - 1) * Time.deltaTime * speed;
        }
        else if (currentRot > 180 && currentRot <= 270)
        {
            carVelocity = new Vector3(
            ((90 - (currentRot - 180)) / 90) - 1,
            0,
            ((currentRot - 180) / 90) - 1) * Time.deltaTime * speed;
        }
        else if (currentRot > 270 && currentRot <= 360)
        {
            //360
            carVelocity = new Vector3(
            ((currentRot - 270) / 90) - 1,
            0,
            1 - ((90 - (currentRot - 270)) / 90)) * Time.deltaTime * speed;
        }




        return carVelocity;
    }
}