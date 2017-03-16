using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSparkPacketReceiver : MonoBehaviour {
    //=============================================================================================================================
    #region VARIABLES
    protected bool InitiateNetwork;

    //GAME ID
    protected int peerID = 0;
    public int GetPeerID()
    {
        return peerID;
    }

    //GAME TIME INIT
    [SerializeField]
    protected DateTime gameShouldStartAt;
    public DateTime Get_gameShouldStartAt()
    {
        return gameShouldStartAt;
    }

    //GAME TIME
    [SerializeField]
    protected Text ActualTime;
    protected float FiveSecUpdateTime;

    protected double gameClockINT;
    public double GetGameClockINT()
    {
        return gameClockINT;
    }

    //GAMESPARKS
    protected GameSparksRTUnity gameSparksRTUnity;
    protected RTSessionInfo sessionInfo;
    public GameSparksRTUnity GetRTSession()
    {
        return gameSparksRTUnity;
    }
    #endregion
    //=============================================================================================================================
    #region DATA RECEIVE
    protected void OnPacketReceived(RTPacket _packet)
    {
        switch (_packet.OpCode)
        {
            #region SERVER TIME
            case 101:
                {
                    CalculateTimeDelta(_packet);
                }
                break;
            case 102:
                {
                    FiveSecUpdateTime += 5;
                    UIManager.Instance.GameTimeText.text = FiveSecUpdateTime.ToString();
                    SyncClock(_packet);
                    //UPDATES GAME TIME EVERY 5 SECONDS
                    if (!InitiateNetwork)
                    {
                        PowerUpManager.Instance.StartNetwork();

                        for (int i = 0; i < TronGameManager.Instance.PlayerObjects.Length; i++)
                        {
                            TronGameManager.Instance.PlayerObjects[i].GetComponent<Car_DataReceiver>().InitCam();
                        }
                        InitiateNetwork = true;
                        UIManager.Instance.GameUpdateText.text += "\n-=-=-=-=-=-=-=- NET INIT";
                        //TEST PLS RETURN LATER
                        TronGameManager.Instance.ReceiveSignalToStartGame();
                    }
                }
                break;
            #endregion
            case 111:
                {
                    //UPDATES PLAYER MOVEMENT
                    #region MOVEMENT

                    NetworkPlayerData netPlayerData;
                    netPlayerData.playerID = _packet.Data.GetInt(1).Value;
                    netPlayerData.playerPos = new Vector3(_packet.Data.GetFloat(2).Value, _packet.Data.GetFloat(3).Value, _packet.Data.GetFloat(4).Value);
                    netPlayerData.playerRot = _packet.Data.GetVector3(5).Value;
                    netPlayerData.timeStamp = _packet.Data.GetDouble(7).Value;

                    NetworkDataFilter.Instance.ReceiveNetworkPlayerData(netPlayerData);
                    #endregion
                }
                break;
            case 113:
                {
                    //UPDATES PLAYER POWERUPS AND MESH SWITCH
                    #region UPDATES PLAYER POWERUPS AND MESH SWITCH
                    NetworkPlayerEvent _netPlayerEvent = new NetworkPlayerEvent();
                    _netPlayerEvent.playerID = _packet.Data.GetInt(1).Value;
                    _netPlayerEvent.playerStatusSwitch = _packet.Data.GetInt(2).Value == 1 ? true : false;
                    _netPlayerEvent.playerStatus = (NetworkPlayerStatus)_packet.Data.GetInt(3).Value;

                    //UIManager.Instance.GameUpdateText.text += "\n\t\tOPCODE_DATA RECEIVE: " + _netPlayerEvent.playerStatus;
                    if (_netPlayerEvent.playerStatus == NetworkPlayerStatus.SET_READY)
                    {
                        UIManager.Instance.GameUpdateText.text += "\n\t==OPCODE RECEIVE: READY";
                    }
                    if (_netPlayerEvent.playerStatus == NetworkPlayerStatus.SET_START && _netPlayerEvent.playerID == 2)
                    {
                        UIManager.Instance.GameUpdateText.text += "\n\t==OPCODE RECEIVE: START";
                        gameShouldStartAt = DateTime.Parse(_packet.Data.GetString(4));
                    }
                    //UIManager.instance.GameUpdateText.text += "\nData Translation for Disable"+_netPlayerEvent.playerStatus;
                    NetworkDataFilter.Instance.ReceiveNetworkPlayerEvent(_netPlayerEvent);
                    #endregion
                }
                break;
            case 114:
                {
                    //CAR AVATAR SWITCH
                    #region CAR AVATAR SWITCH
                    int receivedPlayerToMove = 0;
                    receivedPlayerToMove = _packet.Data.GetInt(1).Value;

                    UIManager.Instance.GameUpdateText.text += "\n\t==OPCODE RECEIVE: CAR AVATAR";
                    for (int i = 0; i < TronGameManager.Instance.PlayerObjects.Length; i++)
                    {
                        GameObject _obj = TronGameManager.Instance.PlayerObjects[i].gameObject;
                        Car_DataReceiver _GameSparks_DataSender = _obj.GetComponent<Car_DataReceiver>();

                        if (_GameSparks_DataSender.Network_ID == receivedPlayerToMove)
                        {
                            TronGameManager.Instance.SetProgressValueHolder(10);
                            UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: SUCCESSFULLY RECEIVED AVATAR: " + _packet.Data.GetInt(2).Value;
                            UIManager.Instance.GameUpdateText.text += "\n=========================================================";
                            _GameSparks_DataSender.SetCarAvatar(_packet.Data.GetInt(2).Value);
                        }
                    }

                    #endregion
                }
                break;
            case 115:
                {
                    //MISSLE DATA RECEIVE
                    #region MISSLE DATA RECEIVE
                    int missleIndex = _packet.Data.GetInt(1).Value;
                    int PlayerController = _packet.Data.GetInt(2).Value;
                    List<GameObject> _objList = new List<GameObject>();

                    if (peerID == 1)
                        _objList = PowerUpManager.Instance.MissleList_Player2;
                    else if (peerID == 2)
                        _objList = PowerUpManager.Instance.MissleList_Player1;

                    if (PlayerController != peerID)
                    {
                        for (int i = 0; i < _objList.Count; i++)
                        {
                            if (missleIndex == _objList[i].GetComponent<MissleScript>().Missle_ID)
                            {
                                MissleScript _missleScript = _objList[i].GetComponent<MissleScript>();

                                Vector3 temp = new Vector3(_packet.Data.GetFloat(3).Value, _packet.Data.GetFloat(4).Value, _packet.Data.GetFloat(5).Value);

                                if (_packet.Data.GetInt(7).Value == 0)
                                {
                                    _missleScript.SetSYnc(temp, _packet.Data.GetVector3(6).Value);
                                    _missleScript.gameObject.SetActive(false);
                                    _missleScript.transform.SetParent(_missleScript.missleParent);
                                    return;
                                }
                                else
                                {
                                    _missleScript.SetSYnc(temp, _packet.Data.GetVector3(6).Value);
                                    _missleScript.gameObject.SetActive(true);
                                    AudioManager.Instance.SpawnableAudio(temp, AUDIO_CLIP.MISSLE_ACTIVE);
                                    _missleScript.transform.SetParent(null);
                                }
                            }
                        }
                    }
                    #endregion
                }
                break;
            case 118:
                {
                    //UPDATES PLAYER HEALTH AND TRAIL VALUE
                    #region UPDATES PLAYER HEALTH AND TRAIL VALUE
                    NetworkPlayerVariables _netPlayerVar = new NetworkPlayerVariables();
                    _netPlayerVar.playerID = _packet.Data.GetInt(1).Value;
                    _netPlayerVar.playerVariable = (NetworkPlayerVariableList)_packet.Data.GetInt(2).Value;
                    _netPlayerVar.variableValue = _packet.Data.GetFloat(3).Value;

                    NetworkDataFilter.Instance.ReceivedNetworkPlayerVariable(_netPlayerVar);
                    #endregion
                }
                break;
            case 066:
                {
                    //MENUSTATE
                    int receivedPlayerID = _packet.Data.GetInt(1).Value;
                    int receivedMenuState = _packet.Data.GetInt(2).Value;
                    StateManager.Instance.Access_ChangeState((MENUSTATE)receivedMenuState);
                }
                break;
        }

    }
    #endregion
    //=============================================================================================================================
    #region CLOCK SYNC
    protected IEnumerator SendTimeStamp()
    {

        using (RTData data = RTData.Get())
        {
            data.SetLong(1, (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
            GetRTSession().SendData(101, GameSparksRT.DeliveryIntent.UNRELIABLE, data, new int[] { 0 });
        }
        yield return new WaitForSeconds(0f);
        StartCoroutine(SendTimeStamp());
    }
    public DateTime serverClock;
    protected int timeDelta, latency, roundTrip;

    /// Calculates the time-difference between the client and server
    public void CalculateTimeDelta(RTPacket _packet)
    {
        // calculate the time taken from the packet to be sent from the client and then for the server to return it //
        roundTrip = (int)((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds - _packet.Data.GetLong(1).Value);
        latency = roundTrip / 2; // the latency is half the round-trip time
        // calculate the server-delta from the server time minus the current time
        int serverDelta = (int)(_packet.Data.GetLong(2).Value - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
        timeDelta = serverDelta + latency; // the time-delta is the server-delta plus the latency
    }

    public void SyncClock(RTPacket _packet)
    {
        DateTime dateNow = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); // get the current time
        serverClock = dateNow.AddMilliseconds(_packet.Data.GetLong(1).Value).ToLocalTime();
        //serverClock = dateNow.AddMilliseconds(_packet.Data.GetLong(1).Value + timeDelta).ToLocalTime(); // adjust current time to match clock from server

        /*
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nServer Clock: " + serverClock+" : : "+ _packet.Data.GetLong(1).Value;

        if (GameObject.Find("GameUpdateText").GetComponent<Text>().text.Length > 3000)
        {
            GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
        }
        */
    }
    #endregion
}
