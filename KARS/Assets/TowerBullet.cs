using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBullet : MonoBehaviour {

    private GameSparksRTUnity GetRTSession;

    public int Bullet_ID;
    public int Controller_ID;

    public   bool LockOn;
    public  Transform TargetObj;
    public Vector3 SyncMove, SyncRot;

    public  Transform DefaultParent;

    void Awake()
    {
    }

    public void Initbullet(GameObject _obj)
    {
        gameObject.SetActive(false);

        transform.position = DefaultParent.transform.position;

        gameObject.SetActive(true);
        transform.SetParent(null);
        LockOn = true;
        TargetObj = _obj.transform;
    }

    void Update()
    {
        if(GameSparksManager.Instance.PeerID == Controller_ID.ToString())
        {
            if (LockOn)
            {
                if (Vector3.Distance(transform.position, TargetObj.position) > 0)
                {
                    transform.position = Vector3.MoveTowards(transform.position, TargetObj.position, 0.5f);
                    SendBullet();
                }
                else
                {
                    transform.position = DefaultParent.transform.position;
                    LockOn = false;
                    SendBullet();
                    transform.SetParent(DefaultParent);
                    gameObject.SetActive(false);
                }
            }
        }
        else
        {
            transform.position = SyncMove;
            transform.eulerAngles = SyncRot;
        }
    }

    void SendBullet()
    {
        try
        {
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, Controller_ID);
                data.SetInt(2, Bullet_ID);
                data.SetInt(3, LockOn == true ? 1 : 0);
                data.SetFloat(4, transform.position.x);
                data.SetFloat(5, transform.position.y);
                data.SetFloat(6, transform.position.z);
                data.SetVector3(7, transform.eulerAngles);

                GetRTSession.SendData(120, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        catch
        {
            GetRTSession = GameSparksManager.Instance.GetRTSession();
            SendBullet();
        }
    }

    public void SyncBullet(Vector3 _pos, Vector3 _rot, int _ifActive)
    {
        if(_ifActive == 1)
            transform.SetParent(null);
        else
            transform.SetParent(DefaultParent);

        gameObject.SetActive(_ifActive == 1 ? true : false);
        SyncMove = _pos;
        SyncRot = _rot;
    }
        
}
