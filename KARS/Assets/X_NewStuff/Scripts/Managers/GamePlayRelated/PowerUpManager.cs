using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour {
    //=======================================================================================================================
    //VARIABLES
    #region VARIABLES
    //SINGLETON
    private static PowerUpManager instance;
    public static PowerUpManager Instance
    {
        get { return instance; }
    }

    [SerializeField]
    GameObject Player1, Player2;

    [Header("POOLS")]
    [SerializeField]
    Transform MisslePool_Player1;
    [SerializeField]
    Transform MisslePool_Player2;

    [Header("LISTS")]
    public List<GameObject> MissleList_Player1;
    public List<GameObject> MissleList_Player2;


    int ServerPeerID;
    private GameSparksRTUnity GetRTSession;
    

    #endregion
    //=======================================================================================================================
    //INITIALIZATION
    #region INITIALIZATION
    void Awake()
    {
        instance = this;
        Player1 = GameObject.Find("Car1");
        Player2 = GameObject.Find("Car2");
    }

    public void StartNetwork()
    {
        MissleList_Player1 = new List<GameObject>();
        MissleList_Player2 = new List<GameObject>();
        StartCoroutine(DelayStartup());
    }

    IEnumerator DelayStartup()
    {
        yield return new WaitForSeconds(1);
        GameObject temp = null;
        
        for (int i = 0; i < MisslePool_Player1.childCount; i++)
        {
            temp = MisslePool_Player1.GetChild(i).gameObject;
            MissleList_Player1.Add(temp);
            temp.GetComponent<MissleScript>().Set_MissleID(i);
        }
        for (int i = 0; i < MisslePool_Player2.childCount; i++)
        {
            temp = MisslePool_Player2.GetChild(i).gameObject;
            MissleList_Player2.Add(temp);
            temp.GetComponent<MissleScript>().Set_MissleID(i);
        }


        yield return new WaitForSeconds(1);
        temp = null;
        try
        {
            ServerPeerID = (GameSparkPacketHandler.Instance.GetPeerID());
        }
        catch { }
    }
    #endregion
    //=======================================================================================================================
    //MISSLE SEND AND RECEIVE FROM SERVER
    #region MISSLE SEND AND RECEIVE FROM SERVER
    public void LockOnTarget(int senderID,GameObject _obj, MissleScript.MISSLE_TYPE _misType)
    {
        if(senderID == 1 || senderID == 0)
        {
            MissleList_Player1[0].GetComponent<MissleScript>().LockOnToThisObject(Player1,_obj, _misType);
            GameObject temp = MissleList_Player1[0];
            for (int i = 0; i < MissleList_Player1.Count - 1; i++)
            {
                MissleList_Player1[i] = MissleList_Player1[i + 1];
            }
            MissleList_Player1[MissleList_Player1.Count - 1] = temp;
        }
        else if (senderID == 2)
        {
            MissleList_Player2[0].GetComponent<MissleScript>().LockOnToThisObject(Player2,_obj, _misType);

            GameObject temp = MissleList_Player2[0];
            for (int i = 0; i < MissleList_Player2.Count - 1; i++)
            {
                MissleList_Player2[i] = MissleList_Player2[i + 1];
            }
            MissleList_Player2[MissleList_Player2.Count - 1] = temp;
        }
    }
    /*
    void ClientSendToServerMissleLock(int sender, int receiver)
    {
        GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, sender);
            data.SetInt(2, receiver);
            GetRTSession.SendData(MeshOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }*/
    #endregion
    //=======================================================================================================================

    public void LaunchMissleFromBUtton(int _misNum)
    {

        if (GameSparkPacketHandler.Instance.GetPeerID() == 2)
        {
            LockOnTarget(2, Player1,(MissleScript.MISSLE_TYPE)_misNum);
        }
        else if (GameSparkPacketHandler.Instance.GetPeerID() == 1)
        {
            LockOnTarget(1, Player2, (MissleScript.MISSLE_TYPE)_misNum);
        }
        else
        {
            LockOnTarget(0,Player2, (MissleScript.MISSLE_TYPE)_misNum);
        }
    }
}
