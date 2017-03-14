﻿using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSparkPacketReceiver : MonoBehaviour
{

    //=========================================================================================================================================================================
    //VARIABLES
    #region VARIABLES
    private static GameSparkPacketReceiver _instance;
    public static GameSparkPacketReceiver Instance { get { return _instance; } }
    
    public TronGameManager _tronGameManager;

    public enum MethodUsed
    {
        LINEAR,
        CUBIC,
        INSTANT,
    }
    public MethodUsed _curMethod;
    public int PeerID = 0;

    public List<Car_DataReceiver> _carPool;
    void Awake()
    {
        _instance = this;
        _curMethod = MethodUsed.LINEAR;
    }

    float FiveSecUpdateTime;
    bool InitiateNetwork;
    #endregion
    //=========================================================================================================================================================================
    //
    //                                                               GAME SPARKS RELATED
    //
    //=========================================================================================================================================================================
    //GAMESPARKS SESSION INITIALIZATION
    #region GAMESPARKS SESSION INITIALIZATION
    private GameSparksRTUnity gameSparksRTUnity;
    public GameSparksRTUnity GetRTSession()
    {
        return gameSparksRTUnity;
    }

    public RTSessionInfo sessionInfo;
    public void StartNewRTSession(RTSessionInfo _info)
    {

        Debug.Log("GSM| Creating New RT Session Instance...");
        sessionInfo = _info;
        gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity>(); // Adds the RT script to the game
                                                                               // In order to create a new RT game we need a 'FindMatchResponse' //
                                                                               // This would usually come from the server directly after a successful MatchmakingRequest //
                                                                               // However, in our case, we want the game to be created only when the first player decides using a button //
                                                                               // therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse //
                                                                               // is passed in. //
        GSRequestData mockedResponse = new GSRequestData()
                                            .AddNumber("port", (double)_info.GetPortID())
                                            .AddString("host", _info.GetHostURL())
                                            .AddString("accessToken", _info.GetAccessToken()); // construct a dataset from the game-details

        FindMatchResponse response = new FindMatchResponse(mockedResponse); // create a match-response from that data and pass it into the game-config
                                                                            // So in the game-config method we pass in the response which gives the instance its connection settings //
                                                                            // In this example, I use a lambda expression to pass in actions for 
                                                                            // OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions //
                                                                            // These methods are self-explanatory, but the important one is the OnPacket Method //
                                                                            // this gets called when a packet is received //


        gameSparksRTUnity.Configure(response,
            (peerId) => { OnPlayerConnectedToGame(peerId); },
            (peerId) => { OnPlayerDisconnected(peerId); },
            (ready) => { OnRTReady(ready); },
            (packet) => { OnPacketReceived(packet); });
        gameSparksRTUnity.Connect(); // when the config is set, connect the game


        UIManager.Instance.GameUpdateText.text += "\nPhase 3: OnMatchFound";
        StateManager.Instance.Access_ChangeState(MENUSTATE.MATCH_FOUND);
        _tronGameManager.SetProgressValueHolder(10);
    }

    private void OnPlayerConnectedToGame(int _peerId)
    {
        Debug.Log("GSM| Player Connected, " + _peerId);
        
    }

    private void OnPlayerDisconnected(int _peerId)
    {
        Debug.Log("GSM| Player Disconnected, " + _peerId);
        GetRTSession().Disconnect();
        GS.Disconnect();
        StateManager.Instance.Access_ChangeState(MENUSTATE.RESULT);
    }

    private void OnRTReady(bool _isReady)
    {
        if (_isReady)
        {
            Debug.Log("GSM| RT Session Connected...");
            PeerID = RegisterGameSpark.Instance.PeerID;
            StartCoroutine(SendTimeStamp());
            //_tronGameManager.SetNetworkStart(true);
        }
    }
    #endregion
    //====================================================================================
    #region CLOCK SYNC
    private IEnumerator SendTimeStamp()
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
    private int timeDelta, latency, roundTrip;

    /// <summary>
    /// Calculates the time-difference between the client and server
    /// </summary>
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
    //====================================================================================
    #region DATA RECEIVE
    private void OnPacketReceived(RTPacket _packet)
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
                        
                        for (int i = 0; i < _carPool.Count; i++)
                        {
                            _carPool[i].InitCam();
                        }
                        InitiateNetwork = true;
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
                    if (_netPlayerEvent.playerStatus == NetworkPlayerStatus.SET_START)
                    {
                        UIManager.Instance.GameUpdateText.text += "\n\t==OPCODE RECEIVE: START";
                        GAMEshudStartAT = DateTime.Parse( _packet.Data.GetString(4) );
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

                    for (int i = 0; i < _carPool.Count; i++)
                    {
                        GameObject _obj = _carPool[i].gameObject;
                        Car_DataReceiver _GameSparks_DataSender = _obj.GetComponent<Car_DataReceiver>();

                        if (_GameSparks_DataSender.Network_ID == receivedPlayerToMove)
                        {
                            _GameSparks_DataSender.SetCarAvatar(_packet.Data.GetInt(2).Value);
                        }
                    }
                    UIManager.Instance.GameUpdateText.text += "\nPhase 6: Car has been Selected: "+ _packet.Data.GetInt(2).Value;

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

                    if (PeerID == 1)
                        _objList = PowerUpManager.Instance.MissleList_Player2;
                    else if (PeerID == 2)
                        _objList = PowerUpManager.Instance.MissleList_Player1;

                    if (PlayerController != PeerID)
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
                                    _missleScript.transform.SetParent(_missleScript.missleParent);
                                    _missleScript.gameObject.SetActive(false);
                                    return;
                                }
                                else
                                {
                                    _missleScript.gameObject.SetActive(true);
                                    _missleScript.transform.SetParent(null);
                                    _missleScript.SetSYnc(temp, _packet.Data.GetVector3(6).Value);
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
                    NetworkPlayerVariables _netPlayerVar= new NetworkPlayerVariables();
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
    //====================================================================================
    //=========================================================================================================================================================================
    //
    //                                                               NON GAME SPARKS RELATED
    //
    //=========================================================================================================================================================================
    //GAME TIME
    #region GAME TIME
    void FixedUpdate()
    {
        serverClock = serverClock.AddSeconds(Time.fixedDeltaTime);

        gameTimeInt = (float)((serverClock.Second * 1000) + serverClock.Millisecond);
        ActualTime.text = serverClock.Minute + " : " + serverClock.Second + " : " + serverClock.Millisecond + "\n" + timeDelta + " " + latency + " " + roundTrip;
        
        if(FiveSecUpdateTime >= 180)
        {
            _tronGameManager.Global_SendState(MENUSTATE.RESTART_GAME);
        }
    }

    public double gameTimeInt;
    string gameTimeText = "";

    [SerializeField]
    Text ActualTime;

    #endregion
    //====================================================================================
    //GAME TEST
    #region PING TEST
    public double playerPingOffset = 1000f;

    public void changeMethod(int _meth)
    {
        _curMethod = (MethodUsed)_meth;
        displayMethod.text = _curMethod.ToString();
    }
    public Text displayMethod;
    #endregion
    //====================================================================================
    #region PUBLIC FUNCTIONS
    public void Access_ResetClock()
    {
        FiveSecUpdateTime = 0;

        serverClock = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        gameTimeInt = 0;
    }
    public void Access_PlayerSpawn(int _player)
    { 
        Car_DataReceiver _obj = null;
        if (_player == 1)
        {
            _obj = GameObject.Find("Car1").GetComponent<Car_DataReceiver>();
            _obj.SetNetworkObject(_player);
            _obj.GetComponent<Car_Movement>().enabled = true;
        }

        if (_player == 2)
        {
            _obj = GameObject.Find("Car2").GetComponent<Car_DataReceiver>();
            _obj.SetNetworkObject(_player);
            _obj.GetComponent<Car_Movement>().enabled = true;
        }
    }

    public void Access_SentReadyToServer()
    {
        //SEND READY
        GameSparksRTUnity RT = GetRTSession();
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, PeerID);
            data.SetInt(2, 1);
            data.SetInt(3, (int)NetworkPlayerStatus.SET_READY);
            RT.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
        UIManager.Instance.GameUpdateText.text += "\n\n\t***OPCODE SEND: READY\n";
    }
    public void Access_SentStartToServer()
    {
        //SEND START
        GameSparksRTUnity RT = GetRTSession();
        for (int i = 0; i < 2; i++)
        {
            TronGameManager.Instance.PlayerObjects[i].GetComponent<Car_Movement>().SetStartGame(true);
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, i + 1);
                data.SetInt(2, 1);
                data.SetInt(3, (int)NetworkPlayerStatus.SET_START);
                data.SetString(4, serverClock.AddSeconds(5).ToString() );
                UIManager.Instance.GameUpdateText.text += "\n\tcurrent game time: " + serverClock;
                UIManager.Instance.GameUpdateText.text += "\n\tGAME SHUD START AT: " + serverClock.AddSeconds(10);
                RT.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        UIManager.Instance.GameUpdateText.text += "\n\n\t***OPCODE SEND: MESH";
        //SEND MESH
        RT = GetRTSession();
        _tronGameManager.PlayerObjects[PeerID - 1].GetComponent<Car_DataReceiver>().SetCarAvatar(_tronGameManager.SelectedSkin);
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, PeerID);
            data.SetInt(2, _tronGameManager.SelectedSkin);
            RT.SendData(OPCODE_CLASS.MeshOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
        UIManager.Instance.GameUpdateText.text += "\n\t***OPCODE SEND: START\n";
    }
    #endregion
    public DateTime GAMEshudStartAT ;
}