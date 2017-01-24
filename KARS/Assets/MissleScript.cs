using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleScript : MonoBehaviour {

    [SerializeField]
    private int missle_ID;
    public int Missle_ID
    {
        get { return missle_ID; }
        set { missle_ID = value; }
    }
    [SerializeField]
    private GameObject objectToHit;
    public GameObject ObjectToHit
    {
        get { return objectToHit; }
        set { objectToHit = value; }
    }
    [SerializeField]
    private bool lockOnObject;
    public bool LockOnObject
    {
        get { return lockOnObject; }
        set { lockOnObject = value; }
    }

    Transform missleParent;
    float missleSpeed = 0.5f;

    private GameSparksRTUnity GetRTSession;

    public Vector3 SyncMovement;
    public Vector3 SyncRot;


    void Update()
    {
        if (GameSparksManager.Instance.PeerID == "1")
        {
            if (lockOnObject)
            {
                if (Vector3.Distance(transform.position, objectToHit.transform.position) < 3)
                {
                    ResetMissle();
                    return;
                }
                else
                {
                    SendMissleData(1);
                    transform.position = Vector3.MoveTowards(transform.position, objectToHit.transform.position, missleSpeed);
                    transform.LookAt(objectToHit.transform.position);
                }
            }
        }
        else
        {
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

                GetRTSession.SendData(112, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
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
    public void LockOnToThisObject(GameObject _obj)
    {
        transform.position = missleParent.transform.position;
        objectToHit = _obj;
        transform.SetParent(null);
        lockOnObject = true;
        gameObject.SetActive(true);
    }
    public void ResetMissle()
    {
        SendMissleData(0);
        objectToHit = null;
        lockOnObject = false;
        transform.SetParent(missleParent);
        gameObject.SetActive(false);
    }
}
