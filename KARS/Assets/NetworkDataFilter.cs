using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDataFilter : MonoBehaviour
{
    public static NetworkDataFilter Instance { get { return instance; } }
    public static NetworkDataFilter instance;
    void Awake()
    {
        instance = this;
    }


    [SerializeField]
    private NetworkDataReceiver[] Network_Data_Receiver;


    public void ReceiveNetworkPlayerData(NetworkPlayerData _netData)
    {

    }
    public void ReceiveNetworkPlayerEvent(NetworkPlayerEvent _networkPlayerEvent)
    {

    }

}

public struct NetworkPlayerData
{
    internal int playerID;
    internal double timeStamp;
    internal Vector3 playerPos;
    internal Vector3 playerRot;
}

public struct NetworkPlayerEvent
{
    internal int playerID;
    internal NetworkPlayerStatus playerStatus;
    internal bool playerStatusSwitch; 
}
enum NetworkPlayerStatus
{
    ACTIVATE_SHIELD,
    ACTIVATE_TRAIL,
    ACTIVATE_NITRO
}

