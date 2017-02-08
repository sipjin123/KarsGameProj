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

    public int PeerID = 0;

    public List<Car_DataReceiver> _carPool;
    void Awake()
    {
        _instance = this;
       // _carPool = new List<Car_DataReceiver>();
        _curMethod = CurrentMethod.LINEAR;
    }

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
                break;
            case 102:
                {
                    //UPDATES GAME TIME EVERY 5 SECONDS
                    #region SERVER TIME
                    if (!InitiateNetwork)
                    {
                        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n DAta cam init 1: "+_carPool.Count;
                        for (int i = 0; i < _carPool.Count; i++)
                        {
                            _carPool[i].InitCam();

                            GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n DAta cam init 2";
                        }
                        InitiateNetwork = true;
                        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n DAta cam init 3";
                    }
                    GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n DAta cam init 4";
                    SyncClock(_packet);
                    #endregion
                }
                break;
            case 111:
                {
                    Debug.LogError("Received a Packer: " + _packet);
                    return;
                    //UPDATES PLAYER MOVEMENT
                    #region MOVEMENT
                    int receivedPlayerToMove = 0;
                    receivedPlayerToMove = _packet.Data.GetInt(1).Value;
                    for (int i = 0; i < _carPool.Count; i++)
                    {
                        GameObject _obj = _carPool[i].gameObject;
                        GameSparks_DataSender _GameSparks_DataSender = _obj.GetComponent<GameSparks_DataSender>();

                        if (_GameSparks_DataSender.NetworkID == receivedPlayerToMove)
                        {
                            _obj.GetComponent<GameSparks_DataSender>().ReceiveBufferState(_packet);
                        }
                    }
                    #endregion
                }
                break;
            case 118:
                {
                    //UPDATES PLAYER HEALTH
                    #region UPDATES PLAYER HEALTH
                    /*
                    int receivedPlayerID = _packet.Data.GetInt(1).Value;
                    float receivedPlayerHealth = _packet.Data.GetFloat(2).Value;

                    //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nRECEIVED: Player " + receivedPlayerID + " Health now is: " + receivedPlayerHealth;
                    if (receivedPlayerID == 1)
                    {
                        PlayerHealthBar_1.fillAmount = receivedPlayerHealth / 100;
                    }
                    if (receivedPlayerID == 2)
                    {
                        PlayerHealthBar_2.fillAmount = receivedPlayerHealth / 100;
                    }
                    */
                    #endregion
                }
                break;
            case 122:
                {
                    
                    int receivedPlayerToMove = _packet.Data.GetInt(1).Value;
                    int receivedPlayerAction = _packet.Data.GetInt(2).Value;

                    if (receivedPlayerToMove != PeerID)
                    {
                        _curMethod = (CurrentMethod)receivedPlayerAction;
                    }
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

}


