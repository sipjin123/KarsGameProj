using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerLocker : MonoBehaviour {

    private GameSparksRTUnity GetRTSession;
    public GameObject TargetObject;
    public Transform BulletPool;

    bool LockOn;

    public int Tower_ID;
    public int Controller_ID;

    public Transform objPivot;
    Vector3  syncRotation;

    public List<TowerBullet> BulletList;

    float shootTimer;
    void Fire()
    {
        BulletList[0].Initbullet(TargetObject);
        TowerBullet _temp = BulletList[0];
        for (int i = 0; i < BulletList.Count - 1; i++)
        {
            BulletList[i] = BulletList[i + 1];
        }
        BulletList[BulletList.Count - 1] = _temp;
        shootTimer = 0;
    }

    void Start()
    {
        BulletList = new List<TowerBullet>();
        for(int i = 0; i < BulletPool.transform.childCount;i++)
        {
            BulletList.Add(BulletPool.transform.GetChild(i).GetComponent< TowerBullet>());
        }
    }

    void Update()
    {
        if(GameSparksManager.Instance.PeerID == Controller_ID.ToString())
        {
            if (LockOn)
            {
                shootTimer += Time.deltaTime;
                if(shootTimer >= .5f)
                {
                    Fire();
                }
                objPivot.transform.LookAt(TargetObject.transform.position);
                SendDate();
            }
        }
        else
        {
            objPivot.eulerAngles = syncRotation;
        }
    }

    void SendDate()
    {
        try
        {
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, Controller_ID);
                data.SetInt(2, Tower_ID);
                data.SetInt(3, LockOn == true ? 1 : 0);
                data.SetFloat(4, objPivot.transform.eulerAngles.x);
                data.SetFloat(5, objPivot.transform.eulerAngles.y);
                data.SetFloat(6, objPivot.transform.eulerAngles.z);
                GetRTSession.SendData(119, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        catch
        {
            GetRTSession = GameSparksManager.Instance.GetRTSession();
            SendDate();
        }
    }
    public void Sync( Vector3 _rot)
    {
        syncRotation = _rot;
    }

    void OnTriggerEnter(Collider hit)
    {
        if(hit.tag == "Car")
        {
            TargetObject = hit.gameObject;
            LockOn = true;
        }
    }
    void OnTriggerExit(Collider hit)
    {
        if (hit.tag == "Car")
        {
            TargetObject = null;
            LockOn = false;
        }
    }

}
