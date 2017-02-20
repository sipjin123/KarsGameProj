using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkDataFilter : MonoBehaviour
{
    public static NetworkDataFilter Instance { get { return instance; } }
    public static NetworkDataFilter instance;
    void Awake()
    {
        instance = this;
    }


    [SerializeField]
    private Car_DataReceiver[] Network_Data_Receiver;
    //===================================================================================================================================================================================================
    #region RECEIVE DATA
    //PLAYER MOVEMENT
    public void ReceiveNetworkPlayerData(NetworkPlayerData _netData)
    {
        Car_DataReceiver carReceiver = new Car_DataReceiver();
        for (int i = 0; i < Network_Data_Receiver.Length; i++)
        {
            if(Network_Data_Receiver[i].Network_ID == _netData.playerID)
            {
                carReceiver = Network_Data_Receiver[i];
            }
        }
        carReceiver.ReceiveBufferState(_netData.timeStamp, _netData.playerPos,_netData.playerRot);

    }
    //==========================================================================================================
    //PLAYER STATS
    public void ReceiveNetworkPlayerEvent(NetworkPlayerEvent _networkPlayerEvent)
    {
        Car_DataReceiver carReceiver = new Car_DataReceiver();
        Car_Movement carMovement = new Car_Movement();
        for (int i = 0; i < Network_Data_Receiver.Length; i++)
        {
            if (Network_Data_Receiver[i].Network_ID == _networkPlayerEvent.playerID)
            {
                carReceiver = Network_Data_Receiver[i];
                carMovement = Network_Data_Receiver[i].gameObject.GetComponent<Car_Movement>();
            }
        }

        switch (_networkPlayerEvent.playerStatus)
        {
            case NetworkPlayerStatus.ACTIVATE_SHIELD:
                {
                    GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nShield: " + _networkPlayerEvent.playerStatusSwitch;
                    carReceiver.ReceivePowerUpState(_networkPlayerEvent.playerStatusSwitch);
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_TRAIL:
                {
                    GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nTRAIL: "+ _networkPlayerEvent.playerStatusSwitch;
                    carMovement._trailCollision.SetEmiision(_networkPlayerEvent.playerStatusSwitch);
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_STUN:
                {
                    GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nStun: " + _networkPlayerEvent.playerStatusSwitch;
                    carReceiver.ReceiveDisableSTate( _networkPlayerEvent.playerStatusSwitch, _networkPlayerEvent.playerStatus);
                }
                break;
        }
    }
    #endregion
    //===================================================================================================================================================================================================
}



#region PUBLIC CLASSES
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
public enum NetworkPlayerStatus
{
    NONE,
    ACTIVATE_SHIELD,
    ACTIVATE_TRAIL,
    ACTIVATE_NITRO,
    ACTIVATE_STUN,
   
}
#endregion