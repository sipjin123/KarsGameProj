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

        if (_networkPlayerEvent.playerStatus == NetworkPlayerStatus.ACTIVATE_STUN ||
            _networkPlayerEvent.playerStatus == NetworkPlayerStatus.ACTIVATE_BLIND ||
            _networkPlayerEvent.playerStatus == NetworkPlayerStatus.ACTIVATE_CONFUSE ||
            _networkPlayerEvent.playerStatus == NetworkPlayerStatus.SET_START ||
            _networkPlayerEvent.playerStatus == NetworkPlayerStatus.SET_READY)
        {
            //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nDisable: "+_networkPlayerEvent.playerStatus.ToString()+": " + _networkPlayerEvent.playerStatusSwitch;
            carReceiver.ReceiveDisableSTate(_networkPlayerEvent.playerStatusSwitch, _networkPlayerEvent.playerStatus);
        }
        else if (_networkPlayerEvent.playerStatus == NetworkPlayerStatus.ACTIVATE_SHIELD)
        {
            carReceiver.ReceivePowerUpState(_networkPlayerEvent.playerStatusSwitch, _networkPlayerEvent.playerStatus);
        }
        else if (_networkPlayerEvent.playerStatus == NetworkPlayerStatus.ACTIVATE_TRAIL)
        {
            carMovement._trailCollision.SetEmiision(_networkPlayerEvent.playerStatusSwitch);
        }
    }

    public void ReceivedNetworkPlayerVariable(NetworkPlayerVariables _networkPlayerVariables)
    {
        Car_DataReceiver carReceiver = new Car_DataReceiver();
        for (int i = 0; i < Network_Data_Receiver.Length; i++)
        {
            if (Network_Data_Receiver[i].Network_ID == _networkPlayerVariables.playerID)
            {
                carReceiver = Network_Data_Receiver[i];
            }
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

public struct NetworkPlayerVariables
{
    internal int playerID;
    internal NetworkPlayerVariableList playerVariable;
    internal float variableValue;
}

public enum NetworkPlayerStatus
{
    NONE,
    ACTIVATE_SHIELD,
    ACTIVATE_TRAIL,
    ACTIVATE_NITRO,
    ACTIVATE_STUN,
    ACTIVATE_BLIND,
    ACTIVATE_CONFUSE,
    SET_READY,
    SET_START
}

public enum NetworkPlayerVariableList
{
    NONE,
    HEALTH,
    TRAIL,
}

#endregion