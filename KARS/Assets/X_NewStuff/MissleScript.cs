using GameSparks.RT;
using Synergy88;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissleScript : MonoBehaviour {
    public struct State
    {
        internal Vector3 pos;
        internal Vector3 rot;
    }

    public State[] m_BufferedState = new State[20];
    //================================================================================================================================
    #region VARIABLES
    [SerializeField]
    private int _playerController_ID;
    public int PlayerController_ID
    {
        get { return _playerController_ID; }
        set { _playerController_ID = value; }
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

    public Transform missleParent;
    float missleSpeed = 0.5f;

    private GameSparksRTUnity GetRTSession;

    public Vector3 SyncMovement;
    public Vector3 SyncRot;
    #endregion
    //================================================================================================================================
    #region UPDATE AND SYNC
    void Update()
    {
        if(TronGameManager.Instance.NetworkStart == false)
        {
            transform.position += transform.forward * 1f;
            return;
        }


        if (_playerController_ID == GameSparkPacketReceiver.Instance.PeerID)
        {
            //Debug.LogError("missle is controlling");
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.blue;
            if (lockOnObject)
            {
                if (Vector3.Distance(transform.position, objectToHit.transform.position) < 1)
                {
                    SendToSErverTheCollision();
                }
                else
                {
                    SendMissleData(1);
                    transform.position += transform.forward * 1f; //Vector3.MoveTowards(transform.position, objectToHit.transform.position, missleSpeed);
                    //transform.LookAt(objectToHit.transform.position);
                }
            }
        }
        else
        {
            //Debug.LogError("missle is syncing");
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
            transform.position = Vector3.Lerp(transform.position, SyncMovement, 1);
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, SyncRot, 1);
        }
    }
    public void SetSYnc(Vector3 _pos,Vector3 _rot)
    {
        for (int q = m_BufferedState.Length - 1; q >= 1; q--)
        {
            m_BufferedState[q] = m_BufferedState[q - 1];
        }

        State state;
        state.pos = _pos;
        state.rot =_rot;
        m_BufferedState[0] = state;


        try
        {
            SyncMovement = m_BufferedState[0].pos + (m_BufferedState[0].pos - m_BufferedState[1].pos);
        }
        catch
        {
            SyncMovement = _pos;
        }
        SyncRot = _rot;
    }
    #endregion
    //================================================================================================================================
    #region SEND DATA
    void SendMissleData(int _var)
    {
        try
        {
            GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, missle_ID);
                data.SetInt(2, PlayerController_ID);
                data.SetFloat(3, transform.position.x);
                data.SetFloat(4, transform.position.y);
                data.SetFloat(5, transform.position.z);
                data.SetVector3(6, transform.eulerAngles);
                data.SetInt(7, _var);

                GetRTSession.SendData(115, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        catch { }
    }
    #endregion
    //================================================================================================================================
    #region PUBLIC FUNCTIONS
    public void Set_MissleID(int _var)
    {
        missleParent = transform.parent;
        lockOnObject = false;
        missle_ID = _var;
        gameObject.name += _var;
        gameObject.SetActive(false);
    }
    public void LockOnToThisObject(GameObject _sender,GameObject _obj)
    {
        //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nMissle "+gameObject.name+" Locking on to: "+_obj.name;
       // Debug.LogError("locking on to object "+_obj.gameObject.name);
        transform.position = _sender.transform.position;
        transform.rotation = _sender.GetComponent<Car_Movement>().CarRotationObject.transform.rotation;
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
        objectToHit = null;
        lockOnObject = false;
        gameObject.SetActive(false);
    }
    #endregion
    //================================================================================================================================
    void SendToSErverTheCollision()
    {
        try
        {

            if (objectToHit.GetComponent<Car_DataReceiver>()._shieldSwitch)
            {
                ResetMissle();
                return;
            }

            GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, objectToHit.GetComponent<Car_DataReceiver>().Network_ID);
                data.SetInt(2, 2);

                GetRTSession.SendData(117, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nServer (" + GameSparkPacketReceiver.Instance.PeerID + ") Owner (" + _playerController_ID + "Missle # " + Missle_ID + " hit " + hit.gameObject.name;
            }
            ResetMissle();
        }
        catch { }
    }
}
