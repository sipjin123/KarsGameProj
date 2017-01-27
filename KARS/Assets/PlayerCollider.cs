using GameSparks.RT;
using Synergy88;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour {

    public enum COLLIDER_TYPE
    {
        FRONT,
        BODY
    }
    public COLLIDER_TYPE _colliderType;

    private GameSparksRTUnity GetRTSession ;
    public SimpleCarController _simpleCarController;

    void OnTriggerEnter(Collider hit)
    {
        return;
        if (_colliderType == COLLIDER_TYPE.FRONT)
        {
            if (hit.gameObject.name == "BodyBumper")
            {
                _simpleCarController.BumpThisObj();
                try
                {
                    GetRTSession = GameSparksManager.Instance.GetRTSession();
                    using (RTData data = RTData.Get())
                    {
                        data.SetInt(1, hit.gameObject.GetComponent<PlayerCollider>()._simpleCarController.GetComponent<GameSparks_DataSender>().NetworkID);
                        data.SetInt(2, 1);
                        GetRTSession.SendData(117, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                    }
                }
                catch { }
            }
        }
        if (_colliderType == COLLIDER_TYPE.FRONT)
        {
            if (hit.gameObject.name == "BodyBumper")
            {

            }
        }
    }
}
