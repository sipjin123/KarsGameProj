﻿using GameSparks.Api;
using GameSparks.Api.Messages;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RegisterGameSpark : MonoBehaviour {

    //===========================================================================================
    #region VARIABLES
    private static RegisterGameSpark _instance;
    public static RegisterGameSpark Instance { get { return _instance; } }


    [SerializeField]
    private Text ConnectionStatus,UserName,playerList;
    
    public int PeerID;

    public GameObject Canvas_GameType,
        Canvas_Login;

    [SerializeField]
    TronGameManager _tronGameManager;
    #endregion
    //===========================================================================================
    //INITIALIZATION
    #region INITIALIZATION
    void Awake()
    {
        _instance = this;
        Application.logMessageReceivedThreaded += HandleLog;
    }
    void Start()
    {
        GSMessageHandler._AllMessages = HandleGameSparksMessageReceived;
        GS.GameSparksAvailable += (isAvailable) => {
            if (isAvailable)
            {
                ConnectionStatus.text = "GameSparks Connected...";
            }
            else
            {
                ConnectionStatus.text = "GameSparks Disconnected...";
            }
        };

        MatchNotFoundMessage.Listener = (message) => {
            Debug.LogError("No Match Found...");
        };
        MatchFoundMessage.Listener += OnMatchFound;

        UserName.text = DateTime.UtcNow.Second.ToString() + DateTime.UtcNow.Millisecond.ToString() + UnityEngine.Random.Range(0, 1000);


        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n Start";
    }

    public void LoginButton()
    {
        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n ToAut";
        AuthenticateUser(UserName.text, "test", OnRegistration, OnAuthentication);
        Canvas_Login.SetActive(true);
        Canvas_GameType.SetActive(false);
    }
    #endregion
    //===========================================================================================
    //USER AUTHENTICATION
    #region USER AUTHENTICATION
    public delegate void AuthCallback(AuthenticationResponse _authresp2);
    public delegate void RegCallback(RegistrationResponse _authResp);


    public void AuthenticateUser(string _userName, string _password, RegCallback _regcallback, AuthCallback _authcallback)
    {
        new RegistrationRequest()
                  .SetDisplayName(_userName)
                  .SetUserName(_userName)
                  .SetPassword(_password)
                  .Send((regResp) =>
                  {
                      if (!regResp.HasErrors)
                      {
                          Debug.Log("GSM| Registration Successful...");
                          _regcallback(regResp);
                      }
                      else
                      {
                          if (!(bool)regResp.NewPlayer)
                          {
                              Debug.LogWarning("GSM| Existing User, Switching to Authentication");
                              new GameSparks.Api.Requests.AuthenticationRequest()
                                  .SetUserName(_userName)
                                  .SetPassword(_password)
                                  .Send((authResp) =>
                                  {
                                      if (!authResp.HasErrors)
                                      {
                                          Debug.Log("Authentication Successful...");
                                          _authcallback(authResp);
                                      }
                                      else
                                      {
                                          Debug.LogWarning("GSM| Error Authenticating User \n" + authResp.Errors.JSON);
                                      }
                                  });
                          }
                          else
                          {
                              Debug.LogWarning("GSM| Error Authenticating User \n" + regResp.Errors.JSON);
                          }
                      }
                  });
    }
    #endregion
    //===========================================================================================
    //MATCH FIDNING
    #region MATCH FINDING
    IEnumerator DelayFindPlayer()
    {
        yield return new WaitForSeconds(3);
        FindPlayers();
    }
    public void FindPlayers()
    {
        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n Finding Player";


        Debug.LogError("Attempting Matchmaking...");
        new MatchmakingRequest()
            .SetMatchShortCode("B0ntakun") 
            .SetSkill(0) 
            .Send((response) => 
            {
                if (response.HasErrors)
                {
                    Debug.LogError(" Match make Error...\n" + response.Errors.JSON);
                }
            });
    }

    private void OnMatchFound(MatchFoundMessage _message)
    {
        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n Found Match";


        //NOTES
        /*
        _message.Host
        _message.Port
        _message.AccessToken
        _message.MatchId
            _message.Participants
            */



        int totalPlayers = 0;



        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n Init Player Data\n";

        foreach (MatchFoundMessage._Participant player in _message.Participants)
        {
            //NETWORKTEST
            GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n Declare Player : " + ((int)player.PeerId);
            SpawnPlayers(int.Parse( player.PeerId.ToString() ) );
            if (_message.JSONData["playerId"].ToString() != player.Id)
            {

               // SpawnPlayers((int)player.PeerId);

            }
            else
            {
                //NETWORKTEST
                GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n----------------Declare Local Player : " + ((int)player.PeerId+"");
                PeerID = int.Parse(player.PeerId.ToString());
                // SpawnPlayers((int)player.PeerId);
                // PeerID = player.PeerId.ToString();



            }
            totalPlayers += 1;
        }


        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n\n Declare total Player : " + (totalPlayers);
        RTSessionInfo sessionInfo = new RTSessionInfo(_message);
        Debug.LogError("Writen builder: " + sessionInfo);

        
        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n Start Session Init";

        GameSparkPacketReceiver.Instance.StartNewRTSession(sessionInfo);

    }
    #endregion
    //===========================================================================================
    //CALLBACKS
    #region CALLBACKS
    void HandleGameSparksMessageReceived(GSMessage message)
    {
        HandleLog("MSG:" + message.JSONString);
    }

    void HandleLog(string logString)
    {
        GS.GSPlatform.ExecuteOnMainThread(() => 
        {
            HandleLog(logString, null, LogType.Log);
        });
    }

    void HandleLog(string logString, string stackTrace, LogType logType)
    {

    }

    private void OnRegistration(RegistrationResponse _resp)
    {
        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n OnRegister";
        ConnectionStatus.text += "Registered " + _resp.UserId;
        StartCoroutine("DelayFindPlayer");
    }
    private void OnAuthentication(AuthenticationResponse _resp)
    {
        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n OnAuth";
        ConnectionStatus.text += "Authenticated " + _resp.UserId;
        StartCoroutine("DelayFindPlayer");
    }
    #endregion
    //===========================================================================================


    void SpawnPlayers(int _peerID)
    {
        //NETWORKTEST
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nSpawning this: " + _peerID ;

        Car_DataReceiver _obj = null;
        if (_peerID == 1)
        {
            _obj = GameObject.Find("Car1").GetComponent<Car_DataReceiver>();
            _obj.SetNetworkObject( _peerID );
        }

        if (_peerID == 2)
        {
            _obj = GameObject.Find("Car2").GetComponent<Car_DataReceiver>();
            _obj.SetNetworkObject(_peerID);
        }
    }
}