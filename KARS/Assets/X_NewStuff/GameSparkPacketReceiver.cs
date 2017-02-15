using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSparkPacketReceiver : MonoBehaviour {

    //=========================================================================================================================================================================
    //VARIABLES
    #region VARIABLES
    private static GameSparkPacketReceiver _instance;
    public static GameSparkPacketReceiver Instance { get { return _instance; } }


    public GameObject StartPanel;
    public GameObject RegisterCanvas;

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
       // _carPool = new List<Car_DataReceiver>();
        _curMethod = MethodUsed.LINEAR;
    }

    float FiveSecUpdateTime;
    public Text _5SecTimer;
    public Image PlayerHealthBar_1, PlayerHealthBar_2;
    public Text HP_NUM_1, HP_NUM_2;

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
        RegisterCanvas.SetActive(false);
        StartPanel.SetActive(false);


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


            //NETWORKTEST
            GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n Netowrk Ready";

            Debug.Log("GSM| RT Session Connected...");
            PeerID = RegisterGameSpark.Instance.PeerID;
            StartCoroutine(SendTimeStamp());




            _tronGameManager.SetNetworkStart(true);
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
                    


                    FiveSecUpdateTime += 5; _5SecTimer.text = FiveSecUpdateTime.ToString();
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
            #region MOVEMENT
            case 111:
                {
                    //UPDATES PLAYER MOVEMENT
                    #region MOVEMENT
                    int receivedPlayerToMove = 0;
                    receivedPlayerToMove = _packet.Data.GetInt(1).Value;
                    for (int i = 0; i < _carPool.Count; i++)
                    {
                        GameObject _obj = _carPool[i].gameObject;
                        Car_DataReceiver _GameSparks_DataSender = _obj.GetComponent<Car_DataReceiver>();

                        if (_GameSparks_DataSender.Network_ID == receivedPlayerToMove)
                        {
                            _obj.GetComponent<Car_DataReceiver>().ReceiveBufferState(_packet);
                        }
                    }
                    #endregion
                }
                break;
            #endregion
            case 131:
                {
                    int receivedPlayerToMove = 0;
                    receivedPlayerToMove = _packet.Data.GetInt(1).Value;
                    for (int i = 0; i < _carPool.Count; i++)
                    {
                        GameObject _obj = _carPool[i].gameObject;
                        Car_DataReceiver _GameSparks_DataSender = _obj.GetComponent<Car_DataReceiver>();

                        if (_GameSparks_DataSender.Network_ID == receivedPlayerToMove)
                        {
                            _obj.GetComponent<Car_Movement>()._trailCollision.Reset_Mesh();
                        }
                    }
                }
                break;
            case 113:
                {
                    //UPDATES PLAYER POWERUPS
                    #region UPDATES PLAYER POWERUPS
                    int playerIndex = _packet.Data.GetInt(1).Value;
                    bool powerUpSwitch = false;
                    if (_packet.Data.GetInt(2).Value == 0)
                        powerUpSwitch = false;
                    else
                        powerUpSwitch = true;

                    for (int i = 0; i < _carPool.Count; i++)
                    {
                        GameObject _obj = _carPool[i].gameObject;
                        Car_DataReceiver _GameSparks_DataSender = _obj.GetComponent<Car_DataReceiver>();

                        if (_GameSparks_DataSender.Network_ID == playerIndex)
                        {
                            _obj.GetComponent<Car_DataReceiver>().ReceivePowerUpState(powerUpSwitch);
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
            #region HEALTH
            case 118:
                {
                    //UPDATES PLAYER HEALTH
                    #region UPDATES PLAYER HEALTH
                    
                    int receivedPlayerID = _packet.Data.GetInt(1).Value;
                    float receivedPlayerHealth = _packet.Data.GetFloat(2).Value;
                    
                    if (receivedPlayerID == 1)
                    {
                        PlayerHealthBar_1.fillAmount = receivedPlayerHealth / 5;
                        HP_NUM_1.text = receivedPlayerHealth.ToString();

                    }
                    if (receivedPlayerID == 2)
                    {
                        PlayerHealthBar_2.fillAmount = receivedPlayerHealth / 5;
                        HP_NUM_2.text = receivedPlayerHealth.ToString();

                    }

                    if(receivedPlayerID != PeerID)
                    {
                        if (receivedPlayerHealth <= 0)
                        {
                            ResetGame();
                            using (RTData data = RTData.Get())
                            {
                                data.SetInt(1, 0);
                                GetRTSession().SendData(066, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                            }
                        }
                    }
                    #endregion
                }
                break;
            #endregion
            #region TRAIL
            case 122:
                {

                    int receivedPlayerToMove = _packet.Data.GetInt(1).Value;
                    int receivedPlayerAction = _packet.Data.GetInt(2).Value;

                    for (int i = 0; i < _carPool.Count; i++)
                    {
                        GameObject _obj = _carPool[i].gameObject;
                        Car_DataReceiver _GameSparks_DataSender = _obj.GetComponent<Car_DataReceiver>();

                        if (_GameSparks_DataSender.Network_ID == receivedPlayerToMove)
                        {
                            _obj.GetComponent<Car_Movement>()._trailCollision.SetEmiision(receivedPlayerAction == 2 ? true : false);
                        }
                    }
                }
                break;
            #endregion
            #region EXTRAPOLATION_SWITCH
            case 123:
                {
                    int receivedPlayerToMove = _packet.Data.GetInt(1).Value;
                    int receivedPlayerAction = _packet.Data.GetInt(2).Value;


                    if (receivedPlayerToMove != PeerID)
                    {
                        _curMethod = (MethodUsed)receivedPlayerAction;
                    }
                }
                break;
            #endregion
            case 066:
                {
                    ResetGame();
                }
                break;
        }

    }
    #endregion
    //====================================================================================
    public void ResetGame()
    {
        FiveSecUpdateTime = 0;

        serverClock = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        gameTimeInt = 0;

        for (int i = 0; i < _carPool.Count; i++)
        {
            GameObject _obj = _carPool[i].gameObject;
            Car_DataReceiver _GameSparks_DataSender = _obj.GetComponent<Car_DataReceiver>();
            Car_Movement _carMovement = _obj.GetComponent<Car_Movement>();

            //_carMovement._trailCollision.SetEmiision(false);
            //_carMovement._trailCollision.Reset_Mesh();
            _GameSparks_DataSender.InitCam();
            _GameSparks_DataSender.Health = 6;
            _carMovement.Die();
            if (i == 0)
            {
                _obj.transform.position = new Vector3(-5, 1, 0);
            }
            else
            {
                _obj.transform.position = new Vector3(5, 1, 0);
            }
        }
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nGAME RESET";
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            ResetGame();

            using (RTData data = RTData.Get())
            {
                data.SetInt(1, 0);
                GetRTSession().SendData(066, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
    }
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
            ResetGame();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, 0);
                GetRTSession().SendData(066, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }

        if(Input.GetKeyDown(KeyCode.Delete))
        {

            GameObject.Find("GameUpdateText").GetComponent<Text>().text="";
        }
    }

    public void LocalREset()
    {
        ResetGame();
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, 0);
            GetRTSession().SendData(066, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
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

    public void changeMethod(int _meth)
    {
        _curMethod = (MethodUsed)_meth;
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


    public void DisableFiveSecUpdate(bool tes)
    {
        enableFiveSec = tes;
        displayFiveSec.text = enableFiveSec.ToString();
    }
    public void fixInterTime(bool tes)
    {
        fixedInterTime = tes;
        if (tes)
            displayfixInterTime.text = "Fixed 0.3f";
        else
            displayfixInterTime.text = "adjustable";
    }
    #endregion
    //====================================================================================
    public Text PlayerPingText;
    void OnGUI()
    {
        GUI.Box(new Rect(0, Screen.height - 60, 100, 30), PeerID + ": " + _curMethod);
    }
}


