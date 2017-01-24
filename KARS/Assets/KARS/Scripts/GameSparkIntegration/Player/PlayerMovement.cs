using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    bool InputingMovement;

    float speed = 0.25f;
    float rotSpee = 0.1f;



    GameSparks_DataSender _GSDataSender;

    [SerializeField]
    private GameObject ObjRotatePivot;


    void Awake()
    {
        _GSDataSender = GetComponent<GameSparks_DataSender>();
        _GSDataSender.ObjToTranslate = gameObject;
        _GSDataSender.ObjToRotate = ObjRotatePivot;
    }

    void Start()
    { 
        if (_GSDataSender.NetworkID == 1) 
        transform.position = new Vector3(-5, 1, 0);
        else
            transform.position = new Vector3(5, 1, 0);
        _GSDataSender.SendTankMovement(_GSDataSender.NetworkID, transform.position, ObjRotatePivot.transform.eulerAngles);
    }

	void Update ()
    {
        if(int.Parse( GameSparksManager.Instance.PeerID ) == _GSDataSender.NetworkID)
        MovementInput();
    }

    void MovementInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            InputingMovement = true;
            if (Input.GetKey(KeyCode.W) || Input.GetAxis("Pad Y") < 0)
            {
                MoveUp();
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetAxis("Pad Y") > 0)
            {
                MoveDown();
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetAxis("Pad X") < 0)
            {
                MoveLeft();
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetAxis("Pad X") > 0)
            {
                MoveRight();
            }
        }
        else
        {
            if (InputingMovement)
            {
                _GSDataSender.SendTankMovement(_GSDataSender.NetworkID,transform.position,ObjRotatePivot.transform.eulerAngles);
                _GSDataSender.SendTankMovement(_GSDataSender.NetworkID,transform.position,ObjRotatePivot.transform.eulerAngles);
                InputingMovement = false;
            }
        }
    }

    void MoveUp()
    {
        ObjRotatePivot.transform.rotation = Quaternion.Lerp(ObjRotatePivot.transform.rotation, Quaternion.Euler(new Vector3(0, 0, 0)), rotSpee);
        transform.position += transform.forward * speed;
        _GSDataSender.SendTankMovement(_GSDataSender.NetworkID,transform.position,ObjRotatePivot.transform.eulerAngles);
    }

    void MoveDown()
    {
        ObjRotatePivot.transform.rotation = Quaternion.Lerp(ObjRotatePivot.transform.rotation, Quaternion.Euler(new Vector3(0, 180, 0)), rotSpee);
        transform.position -= transform.forward * speed;
        _GSDataSender.SendTankMovement(_GSDataSender.NetworkID,transform.position,ObjRotatePivot.transform.eulerAngles);
    }

    void MoveRight()
    {
        ObjRotatePivot.transform.rotation = Quaternion.Lerp(ObjRotatePivot.transform.rotation, Quaternion.Euler(new Vector3(0, 90, 0)), rotSpee);
        transform.position += transform.right * speed;
        _GSDataSender.SendTankMovement(_GSDataSender.NetworkID,transform.position,ObjRotatePivot.transform.eulerAngles);
    }

    void MoveLeft()
    {
        ObjRotatePivot.transform.rotation = Quaternion.Lerp(ObjRotatePivot.transform.rotation, Quaternion.Euler(new Vector3(0, -90, 0)), rotSpee);
        transform.position -= transform.right * speed;
        _GSDataSender.SendTankMovement(_GSDataSender.NetworkID,transform.position,ObjRotatePivot.transform.eulerAngles);
    }
}
