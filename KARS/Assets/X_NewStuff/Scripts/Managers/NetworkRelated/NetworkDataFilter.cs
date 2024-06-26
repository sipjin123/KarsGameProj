﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkDataFilter : MonoBehaviour
{
    public static NetworkDataFilter Instance { get { return instance; } }
    private static NetworkDataFilter instance;
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
            if(Network_Data_Receiver[i].GetNetwork_ID() == _netData.playerID)
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
            if (Network_Data_Receiver[i].GetNetwork_ID() == _networkPlayerEvent.playerID)
            {
                carReceiver = Network_Data_Receiver[i];
                carMovement = Network_Data_Receiver[i].gameObject.GetComponent<Car_Movement>();
            }
        }

        carReceiver.ReceivePlayerSTate(_networkPlayerEvent.playerStatusSwitch, _networkPlayerEvent.playerStatus);
        if (_networkPlayerEvent.playerStatus == NetworkPlayerStatus.ACTIVATE_TRAIL)
        {
            carMovement._trailCollision.SetEmiision(_networkPlayerEvent.playerStatusSwitch);
        }
    }

    public void ReceivedNetworkPlayerVariable(NetworkPlayerVariables _networkPlayerVariables)
    {
        Car_DataReceiver carReceiver = new Car_DataReceiver();
        for (int i = 0; i < Network_Data_Receiver.Length; i++)
        {
            if (Network_Data_Receiver[i].GetNetwork_ID() == _networkPlayerVariables.playerID)
            {
                carReceiver = Network_Data_Receiver[i];
            }
        }


        if (_networkPlayerVariables.playerVariable == NetworkPlayerVariableList.HEALTH)
        {
            int receivedPlayerID = _networkPlayerVariables.playerID;
            float receivedPlayerHealth = _networkPlayerVariables.variableValue;
            UIManager.Instance.AdjustHPBarAndText(receivedPlayerID, receivedPlayerHealth);

            if (receivedPlayerHealth <= 0)
            {
                GameSparkPacketHandler.Instance.Global_SendONLYState(MENUSTATE.PRE_RESULT);
            }
        }
        else if (_networkPlayerVariables.playerVariable == NetworkPlayerVariableList.TRAIL)
        {
            carReceiver.ReceiveTrailVAlue(_networkPlayerVariables.variableValue);
        }
        else if (_networkPlayerVariables.playerVariable == NetworkPlayerVariableList.CHILD_TRAIL)
        {
            carReceiver.ReceiveTrailChildVAlue(_networkPlayerVariables.variableValue);
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

#endregion