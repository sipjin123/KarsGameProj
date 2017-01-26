using GameSparks.RT;
using Synergy88;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissleScript : MonoBehaviour {

    [SerializeField]
    private int playerController_ID;
    public int PlayerController_ID
    {
        get { return playerController_ID; }
        set { playerController_ID = value; }
    }


    [SerializeField]
    private int missle_ID;
    public int Missle_ID
    {
        get { return missle_ID; }
        set { missle_ID = value; }
    }
    [SerializeField]
    private GameObject objectToHit;
    [SerializeField]
    private bool lockOnObject;

    Transform missleParent;
    float missleSpeed = 0.5f;

    private GameSparksRTUnity GetRTSession;

    public Vector3 SyncMovement;
    public Vector3 SyncRot;


    void Update()
    {

        if (GameSparksManager.Instance.PeerID == PlayerController_ID.ToString())
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.blue;
            if (lockOnObject)
            {
                if (Vector3.Distance(transform.position, objectToHit.transform.position) < 0)
                {
                    ResetMissle();
                }
                else
                {
                    SendMissleData(1);
                    transform.position = Vector3.MoveTowards(transform.position, objectToHit.transform.position, missleSpeed);
                    transform.LookAt(objectToHit.transform.position);
                }
            }
        }
        else if(GameSparksManager.Instance.PeerID != PlayerController_ID.ToString())
        {
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
            transform.position = Vector3.Lerp(transform.position, SyncMovement, 1);
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, SyncRot, 1);
        }
    }
    public void SetSYnc(Vector3 _pos,Vector3 _rot)
    {
        SyncMovement = _pos;
        SyncRot = _rot;
    }

    #region SEND DATA
    void SendMissleData(int _var)
    {
        try
        {
            GetRTSession = GameSparksManager.Instance.GetRTSession();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, missle_ID);
                data.SetFloat(2, transform.position.x);
                data.SetFloat(3, transform.position.y);
                data.SetFloat(4, transform.position.z);
                data.SetVector3(5, transform.eulerAngles);
                data.SetInt(6, _var);

                GetRTSession.SendData(115, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        catch { }
    }
    #endregion

    public void Set_MissleID(int _var)
    {
        missleParent = transform.parent;
        lockOnObject = false;
        missle_ID = _var;
        gameObject.name += _var;
        gameObject.SetActive(false);
    }
    public void LockOnToThisObject(int senderID,GameObject _obj)
    {
        playerController_ID = senderID;
        transform.position = missleParent.transform.position;
        objectToHit = _obj;
        transform.SetParent(null);
        lockOnObject = true;
        gameObject.SetActive(true);
    }
    public void ResetMissle()
    {
        transform.SetParent(missleParent);
        transform.position = missleParent.transform.position;
        SendMissleData(0);
        playerController_ID = 0;
        objectToHit = null;
        lockOnObject = false;
        gameObject.SetActive(false);
    }


    void OnTriggerEnter(Collider hit)
    {
        if (hit.tag == "Car")
        {
            try
            {
                ResetMissle();

                if (hit.GetComponent<GameSparks_DataSender>()._shieldSwitch)
                    return;

                GetRTSession = GameSparksManager.Instance.GetRTSession();
                using (RTData data = RTData.Get())
                {
                    data.SetInt(1, hit.GetComponent<GameSparks_DataSender>().NetworkID);
                    data.SetInt(2,1);

                    GetRTSession.SendData(117, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                    GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nServer (" + GameSparksManager.Instance.PeerID + ") Owner (" + playerController_ID + "Missle # " + Missle_ID + " hit " + hit.gameObject.name;
                }
            }
            catch { }
        }
        else
        {

        }
    }
}
