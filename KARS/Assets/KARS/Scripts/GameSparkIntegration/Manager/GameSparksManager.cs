using GameSparks.RT;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Responses;
using System.Collections.Generic;
using Synergy88;
using System;

public class GameSparksManager : MonoBehaviour {
    //=========================================================================================================================================================================
    //VARIABLES
    #region VARIABLES
    private static GameSparksManager _instance;
    public static GameSparksManager Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
        tankPool = new List<GameObject>();
        _curMethod = CurrentMethod.LINEAR;
    }


    public GameObject StartPanel;
    public GameObject RegisterCanvas;

    public string PeerID = "0";

    public List<GameObject> tankPool;
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
        RegisterCanvas.SetActive(false);
        StartPanel.SetActive(true);
        

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
        Application.Quit();
    }

    private void OnRTReady(bool _isReady)
    {
        if (_isReady)
        {
            Debug.Log("GSM| RT Session Connected...");
            PeerID = RegistrationSparks.Instance.PeerID;
            StartCoroutine(SendTimeStamp());
        }
    }
    #endregion
    //====================================================================================

    bool InitiateNetwork;

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
    DateTime serverClock;
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
        DateTime dateNow = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc); // get the current time
        serverClock = dateNow.AddMilliseconds(_packet.Data.GetLong(1).Value + timeDelta).ToLocalTime(); // adjust current time to match clock from server
    }
    #endregion
    #region DATA RECEIVE
    private void OnPacketReceived(RTPacket _packet)
    {
        switch (_packet.OpCode)
        {
            case 101:

                CalculateTimeDelta(_packet);

                /*
                using (RTData data = RTData.Get())
                {
                    data.SetLong(1, _packet.Data.GetLong(1).Value);
                    data.SetLong(2, _packet.Data.GetLong(2).Value);

                    GetRTSession().SendData(121, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                }*/
                break;
            case 102://UPDATES GAME TIME EVERY 5 SECONDS
                {
                    if (!InitiateNetwork)
                    {
                        PowerUpManager.Instance.StartNetwork();
                        for (int i = 0; i < tankPool.Count; i++)
                        {
                            tankPool[i].GetComponent<SimpleCarController>().StartNetwork();
                        }
                        InitiateNetwork = true;
                    }
                    SyncClock(_packet);
                    //REFACTOR GAME TIME
                    /*
                    return;
                    if (gameTimeText == "")
                    {
                        gameTimeText = (_packet.Data.GetLong(1).Value / 1000).ToString();
                        gameTimeInt = int.Parse(gameTimeText);
                        ActualTime.text = gameTimeText;
                    }
                    else
                    {
                        if (!enableFiveSec)
                            return;

                        //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n"+ (float.Parse(gameTimeText) - (_packet.Data.GetLong(1).Value / 1000));

                        gameTimeText = (_packet.Data.GetLong(1).Value / 1000).ToString();
                        gameTimeInt = int.Parse(gameTimeText);
                        ActualTime.text = gameTimeText;
                    }*/
                }
                break;
            case 111://UPDATES PLAYER MOVEMENT
                {
                    int receivedPlayerToMove = 0;
                    receivedPlayerToMove = _packet.Data.GetInt(1).Value;
                    for (int i = 0; i < tankPool.Count; i++)
                    {
                        GameObject _obj = tankPool[i];
                        GameSparks_DataSender _GameSparks_DataSender = _obj.GetComponent<GameSparks_DataSender>();

                        if (_GameSparks_DataSender.NetworkID == receivedPlayerToMove)
                        {
                            _obj.GetComponent<GameSparks_DataSender>().ReceiveBufferState(_packet);
                        }
                    }
                }
                break;
            case 113://UPDATES PLAYER POWERUPS
                {
                    int playerIndex = _packet.Data.GetInt(1).Value;
                    bool powerUpSwitch = false;
                    if (_packet.Data.GetInt(2).Value == 0)
                        powerUpSwitch = false;
                    else
                        powerUpSwitch = true;

                    for (int i = 0; i < tankPool.Count; i++)
                    {
                        GameObject _obj = tankPool[i];
                        GameSparks_DataSender _GameSparks_DataSender = _obj.GetComponent<GameSparks_DataSender>();

                        if (_GameSparks_DataSender.NetworkID == playerIndex)
                        {
                            _obj.GetComponent<GameSparks_DataSender>().ReceivePowerUpState(powerUpSwitch);
                        }
                    }
                }
                break;
            case 114://UPDATES MISSLE MOVEMENT
                {
                    return;
                    int receivedPlayerBump = _packet.Data.GetInt(1).Value;

                    for (int i = 0; i < tankPool.Count; i++)
                    {
                        if (tankPool[i].GetComponent<GameSparks_DataSender>().NetworkID == receivedPlayerBump)
                        {
                            int isBumped = _packet.Data.GetInt(2).Value;
                            int isFlying = _packet.Data.GetInt(3).Value;
                            int isFalling = _packet.Data.GetInt(4).Value;
                            float forceToDeplete = _packet.Data.GetFloat(5).Value;
                            tankPool[i].GetComponent<SimpleCarController>().SetupInteractionVariables(isBumped, isFlying, isFalling, forceToDeplete);
                        }
                    }
                    /*
                    int sender_ID = _packet.Data.GetInt(1).Value;
                    int receiver_ID = _packet.Data.GetInt(2).Value;

                    //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n(" + GameSparksManager.Instance.PeerID + ") received missle on Server";

                    for (int i = 0; i < tankPool.Count; i++)
                    {
                        GameObject _obj = tankPool[i];
                        GameSparks_DataSender _GameSparks_DataSender = _obj.GetComponent<GameSparks_DataSender>();

                        if (_GameSparks_DataSender.NetworkID == receiver_ID)
                        {
                            //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n(" + GameSparksManager.Instance.PeerID + ") will send to lock on target parent";
                            //PowerUpManager.Instance.LockOnTarget(receiver_ID ,_obj);
                        }
                    }*/
                }
                break;
            case 115://UPDATES MISSLE MOVEMENT
                {
                    #region MISSLE DATA RECEIVE
                    int missleIndex = _packet.Data.GetInt(1).Value;
                    int PlayerController = _packet.Data.GetInt(2).Value;
                    List<GameObject> _objList = new List<GameObject>();

                    if (PeerID == "1")
                        _objList = PowerUpManager.Instance.MissleList_Player2;
                    else if (PeerID == "2")
                        _objList = PowerUpManager.Instance.MissleList_Player1;

                    if (PlayerController.ToString() != PeerID)
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
            case 116://UPDATES TNT SPAWN
                {
                    int receivedServer_ID = _packet.Data.GetInt(1).Value;
                    int receivedTNT_ID = _packet.Data.GetInt(2).Value;
                    Vector3 receivedPosition = _packet.Data.GetVector3(3).Value;
                    bool ifEnabled = false;
                    if (_packet.Data.GetInt(4).Value == 0)
                        ifEnabled = false;
                    else if (_packet.Data.GetInt(4).Value == 1)
                        ifEnabled = true;


                    // GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nRECEIVED TNT # (" + receivedTNT_ID + ") of (" + receivedServer_ID + ") HAS been dispatched to " + receivedPosition;
                    PowerUpManager.Instance.ReceiveFromServer(receivedServer_ID, receivedTNT_ID, receivedPosition, ifEnabled);

                }
                break;
            case 117://UPDATES PLAYER EXPLOSION
                {
                    int receivedPlayerToMove = _packet.Data.GetInt(1).Value;
                    int receivedPlayerAction = _packet.Data.GetInt(2).Value;

                    for (int i = 0; i < tankPool.Count; i++)
                    {
                        if (tankPool[i].GetComponent<GameSparks_DataSender>().NetworkID == receivedPlayerToMove)
                        {
                            tankPool[i].GetComponent<SimpleCarController>().BumpThisObjWithForce();
                        }
                    }
                }
                break;
            case 118://UPDATES PLAYER COLLISION
                {
                    return;
                    int receivedPlayerBump = _packet.Data.GetInt(1).Value;
                    int receivedPlayerAction = _packet.Data.GetInt(2).Value;

                    for (int i = 0; i < tankPool.Count; i++)
                    {
                        if (tankPool[i].GetComponent<GameSparks_DataSender>().NetworkID == receivedPlayerBump)
                        {
                            tankPool[i].GetComponent<SimpleCarController>().BumpThisObjWithForce();
                        }
                    }
                }
                break;
            case 119://UPDATES PLAYER COLLISION
                {
                    Tower_AI_Network.Instance.Receive_Packet(_packet);
                }
                break;
            case 120://UPDATES PLAYER COLLISION
                {
                    Tower_AI_Network.Instance.Receive_Packet(_packet);
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

        gameTimeInt = (float)((serverClock.Second * 1000)  + serverClock.Millisecond);
        ActualTime.text = serverClock.Minute + " : " + serverClock.Second + " : " + serverClock.Millisecond + "\n" + timeDelta + " " + latency + " " + roundTrip;

        return;
        if (Input.GetKeyDown(KeyCode.J))
        {
            StopCoroutine("SendTimeStamp");
            StartCoroutine("SendTimeStamp");
        }
        if (gameTimeText != "")
        {
            gameTimeInt += Time.fixedDeltaTime;
                ActualTime.text = gameTimeInt.ToString();
        }
        //REFACTOR GAME TIME
    }


    public double gameTimeInt;
    string gameTimeText = "";

    [SerializeField]
    Text ActualTime;

    #endregion
    //====================================================================================
    //GAME TEST
    #region GAME TEST
    public double playerPingOffset = 1000f;
    public enum CurrentMethod
    {
        LINEAR,
        CUBIC,
        INSTANT,
    }
    public CurrentMethod _curMethod;

    public void changeMethod(int _meth)
    {
        _curMethod = (CurrentMethod)_meth;
        displayMethod.text = _curMethod.ToString();
    }
    public void adjustOffset(float _off)
    {
        playerPingOffset += _off;
        displayOffset.text = playerPingOffset.ToString();
    }
    public Text displayOffset;
    public Text displayMethod;
    public Text displayFiveSec;
    public Text displayfixInterTime;


    bool enableFiveSec = true;
    public bool fixedInterTime = true;


    public void DisableFiveSecUpdate(bool tes )
    {
        enableFiveSec = tes;
        displayFiveSec.text = enableFiveSec.ToString();
    }
    public void fixInterTime(bool tes)
    {
        fixedInterTime = tes;
        if(tes)
        displayfixInterTime.text = "Fixed 0.3f";
        else
        displayfixInterTime.text = "adjustable";
    }
    #endregion
    //====================================================================================
    
    void OdnGUI()
    {
        try
        {
            for (int q = 0; q < tankPool.Count; q++)
            {
                GameSparks_DataSender _sparksSender = tankPool[q].GetComponent<GameSparks_DataSender>();
                for (int i = 0; i < _sparksSender.m_BufferedState.Length; i++)
                {
                    GUI.Box(new Rect(q * 200, i * 30, 200, 30), i + " (" + (int)_sparksSender.m_BufferedState[i].pos.x + " : " + (int)_sparksSender.m_BufferedState[i].pos.z + ") "+ (int)_sparksSender.m_BufferedState[i].timestamp);//+ _sparksSender.m_BufferedState[i].timestamp.ToString());
                }
            }
        }
        catch
        { }
    }
}


