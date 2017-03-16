using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSparkPacketHandler : GameSparkPacketReceiver
{
    //=============================================================================================================================
    private static GameSparkPacketHandler _instance;
    public static GameSparkPacketHandler Instance { get { return _instance; } }
    void Awake()
    {
        _instance = this;
    }
    //=============================================================================================================================
    //GAME TIME
    #region GAME TIME
    void FixedUpdate()
    {
        serverClock = serverClock.AddSeconds(Time.fixedDeltaTime);

        gameClockINT = (float)((serverClock.Second * 1000) + serverClock.Millisecond);
        ActualTime.text = serverClock.Minute + " : " + serverClock.Second + " : " + serverClock.Millisecond + "\n" + timeDelta + " " + latency + " " + roundTrip;

        if (FiveSecUpdateTime >= 180)
        {
            TronGameManager.Instance.Global_SendState(MENUSTATE.RESTART_GAME);
        }
    }
    #endregion
    //=============================================================================================================================
    //GAMESPARKS SESSION INITIALIZATION
    #region GAMESPARKS SESSION INITIALIZATION
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


        UIManager.Instance.GameUpdateText.text += "\nPhase 1 & 2 & 3: OnMatchFound";
        StateManager.Instance.Access_ChangeState(MENUSTATE.MATCH_FOUND);
        TronGameManager.Instance.SetProgressValueHolder(30);
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
            peerID = RegisterGameSpark.Instance.PeerID;
            StartCoroutine(SendTimeStamp());
        }
    }
    #endregion
    //=============================================================================================================================
    //PUBLIC FUNCTIONS
    #region PUBLIC FUNCTIONS
    public void Access_ResetNetwork()
    {
        InitiateNetwork = false;
    }
    public void Access_ResetClock()
    {
        FiveSecUpdateTime = 0;

        serverClock = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        gameClockINT = 0;
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
    #endregion
    //=============================================================================================================================
    //SEND DATA TO SERVER
    #region SEND DATA TO SERVER
    public void Access_SentReadyToServer()
    {
        //SEND READY
        GameSparksRTUnity RT = GetRTSession();
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, peerID);
            data.SetInt(2, 1);
            data.SetInt(3, (int)NetworkPlayerStatus.SET_READY);
            RT.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
        UIManager.Instance.GameUpdateText.text += "\n=========================================================";
        UIManager.Instance.GameUpdateText.text += "\n\t***OPCODE SEND: READY";
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
                RT.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        UIManager.Instance.GameUpdateText.text += "\n=========================================================";
        UIManager.Instance.GameUpdateText.text += "\n\t***OPCODE SEND: START";

        StopCoroutine("DelaySendMesh");
        StartCoroutine("DelaySendMesh");
    }
    IEnumerator DelaySendMesh()
    {
        yield return new WaitForSeconds(1);
        //SEND MESH
        GameSparksRTUnity RT = GetRTSession();
        TronGameManager.Instance.PlayerObjects[peerID - 1].GetComponent<Car_DataReceiver>().SetCarAvatar(TronGameManager.Instance.SelectedSkin);
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, peerID);
            data.SetInt(2, TronGameManager.Instance.SelectedSkin);
            RT.SendData(OPCODE_CLASS.MeshOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
        UIManager.Instance.GameUpdateText.text += "\n=========================================================";
        UIManager.Instance.GameUpdateText.text += "\n\t***OPCODE SEND: MESH";
    }
    #endregion
    //=============================================================================================================================

    
    //FOR TESTING
    public double playerPingOffset = 1000f;
    public MethodUsed _curMethod;
}