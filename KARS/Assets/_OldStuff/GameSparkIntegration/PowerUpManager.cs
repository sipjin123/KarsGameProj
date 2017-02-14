using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour {
    //=======================================================================================================================
    //VARIABLES
    #region VARIABLES
    private static PowerUpManager instance;
    public static PowerUpManager Instance
    {
        get { return instance; }
    }

    int ServerPeerID;

    [SerializeField]
    GameObject Player1, Player2;
    public GameObject Shield;

    [SerializeField]
    Transform MisslePool_Player1, MisslePool_Player2, TnTPool;

    public List<GameObject> MissleList_Player1;
    public List<GameObject> MissleList_Player2;
    public List<GameObject> TnTList;
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
        TnTList = new List<GameObject>();
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
        for (int i = 0; i < TnTPool.childCount; i++)
        {
            temp = TnTPool.GetChild(i).gameObject;
            TnTList.Add(temp);
            temp.GetComponent<TnTScript>().InitializeObj(i);
        }
        Debug.LogError("PEER SETUP");
        ServerPeerID = (GameSparkPacketReceiver.Instance.PeerID);
    }
    #endregion
    //=======================================================================================================================
    //TNT SEND AND RECEIVE FROM SERVER
    #region TNT SEND AND RECEIVE FROM SERVER
    public void SetUpTNT(int _id, Vector3 _pos, bool _enable)
    {

        try
        {
            GameObject temp = TnTPool.transform.GetChild(0).gameObject;
            temp.GetComponent<TnTScript>().DispatchTNTToDestination(_id, _pos, _enable);
            SendToServer(_id, temp.GetComponent<TnTScript>().TNT_ID, _pos, _enable);
        }
        catch
        {
            GameObject temp = TnTList[0];
            for (int i = 0; i < TnTList.Count - 1; i++)
            {
                TnTList[i] = TnTList[i + 1];
            }
            TnTList[TnTList.Count - 1] = temp;
            TnTList[TnTList.Count - 1].GetComponent<TnTScript>().ResetTnT();
            SetUpTNT(_id,_pos,_enable);
        }
    }

    public void ReceiveFromServer(int _id, int _tntID, Vector3 _pos, bool _enable)
    {
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nReceived From Server";
        for(int i = 0; i < TnTList.Count; i++)
        {
            if(TnTList[i].GetComponent<TnTScript>().TNT_ID == _tntID)
            {
                TnTList[i].GetComponent<TnTScript>().DispatchTNTToDestination(_id,_pos,_enable);
            }
        }
    }

    public void SendToServer(int _id, int _tntID,Vector3 _pos, bool _enable)
    {
        GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, _id);
            data.SetInt(2, _tntID);
            data.SetVector3(3, _pos);
            if (_enable)
                data.SetInt(4, 1);
            else
                data.SetInt(4, 0);

            GetRTSession.SendData(116, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
    #endregion
    //=======================================================================================================================
    //MISSLE SEND AND RECEIVE FROM SERVER
    #region MISSLE SEND AND RECEIVE FROM SERVER
    public void LockOnTarget(int senderID,GameObject _obj)
    {
        if(senderID == 1)
        {
            MissleList_Player1[0].GetComponent<MissleScript>().LockOnToThisObject(Player1,_obj);
            GameObject temp = MissleList_Player1[0];
            for (int i = 0; i < MissleList_Player1.Count - 1; i++)
            {
                MissleList_Player1[i] = MissleList_Player1[i + 1];
            }
            MissleList_Player1[MissleList_Player1.Count - 1] = temp;
        }
        else if (senderID == 2)
        {
            MissleList_Player2[0].GetComponent<MissleScript>().LockOnToThisObject(Player2,_obj);

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
            GetRTSession.SendData(114, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }*/
    #endregion
    //=======================================================================================================================
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchMissleFromBUtton();
        }
        if(Input.GetKeyDown(KeyCode.RightAlt))
        {
            DropTNTFromButton();
        }


        return;
        if(Input.GetKeyDown(KeyCode.M))
        {
            if (GameSparkPacketReceiver.Instance.PeerID == 2)
            {
                GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
                using (RTData data = RTData.Get())
                {
                    data.SetInt(1, 1);
                    data.SetInt(2, 1);

                    GetRTSession.SendData(117, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                }
            }
            else if (GameSparkPacketReceiver.Instance.PeerID == 1)
            {
                GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
                using (RTData data = RTData.Get())
                {
                    data.SetInt(1,2);
                    data.SetInt(2, 1);

                    GetRTSession.SendData(117, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {

            GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
        }
    }

    public void LaunchMissleFromBUtton()
    {
        if (GameSparkPacketReceiver.Instance.PeerID == 2)
        {
            LockOnTarget(2, Player1);
        }
        else if (GameSparkPacketReceiver.Instance.PeerID == 1)
        {
            LockOnTarget(1, Player2);
        }
    }

    internal void LaunchMissle()
    {
        if (GameSparkPacketReceiver.Instance.PeerID == 2)
        {
            LockOnTarget(2, Player1);
        }
        else if (GameSparkPacketReceiver.Instance.PeerID == 1)
        {
            LockOnTarget(1, Player2);
        }
    }

    public void DropTNTFromButton()
    {

        if (GameSparkPacketReceiver.Instance.PeerID == 2)
        {
            SetUpTNT(2, Player2.transform.position, true);
        }
        else if (GameSparkPacketReceiver.Instance.PeerID == 1)
        {
            SetUpTNT(1, Player1.transform.position, true);
        }
    }
    internal void DropTNT()
    {
        Debug.LogError("Drop TNT");

        if (GameSparkPacketReceiver.Instance.PeerID == 2)
        {
            SetUpTNT(2, Player2.transform.position, true);
        }
        else if (GameSparkPacketReceiver.Instance.PeerID == 1)
        {
            SetUpTNT(1, Player1.transform.position, true);
        }
    }

}
