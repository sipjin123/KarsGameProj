using GameSparks.Api;
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
    private Text ConnectionStatus, UserName, playerList;
    
    public int PeerID;

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
    }

    public void Access_LoginAuthentication()
    {
        AuthenticateUser(UserName.text, "test", OnRegistration, OnAuthentication);
        UIManager.Instance.SetMatchCancelButton(true);
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
    void FindPlayers()
    {
        if(_tronGameManager.BlockMatchFinding == true)
        {
            UIManager.Instance.GameUpdateText.text += "\n\t\t-BLOCKED MATCH FINDING";
            return;
        }

        UIManager.Instance.GameUpdateText.text += "\nPhase 2: Find Players";
        _tronGameManager.SetProgressValueHolder(10);
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
    public void Access_StopFindingPlayers()
    {
        StopCoroutine("DelayFindPlayer");
    }

    private void OnMatchFound(MatchFoundMessage _message)
    {
        if (_tronGameManager.BlockMatchFinding == true)
        {
            UIManager.Instance.GameUpdateText.text += "\n\t\t-BLOCKED MATCH FINDING";
            return;
        }
        UIManager.Instance.SetMatchCancelButton(false);
        //NOTES
        /*
        _message.Host
        _message.Port
        _message.AccessToken
        _message.MatchId
            _message.Participants
            */



        int totalPlayers = 0;


        foreach (MatchFoundMessage._Participant player in _message.Participants)
        {
            GameSparkPacketReceiver.Instance.Access_PlayerSpawn(int.Parse( player.PeerId.ToString())); 
            if (_message.JSONData["playerId"].ToString() == player.Id)
            {
                PeerID = int.Parse(player.PeerId.ToString());
            }
            totalPlayers += 1;
        }

        RTSessionInfo sessionInfo = new RTSessionInfo(_message);
        Debug.LogError("Writen builder: " + sessionInfo);


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
        UIManager.Instance.GameUpdateText.text += "\nPhase 1: Registered Player";
        _tronGameManager.SetProgressValueHolder(10);
        StartCoroutine("DelayFindPlayer");

        ConnectionStatus.text += "Registered " + _resp.UserId;
    }
    private void OnAuthentication(AuthenticationResponse _resp)
    {
        UIManager.Instance.GameUpdateText.text += "\nPhase 1: Authenticated Player";
        _tronGameManager.SetProgressValueHolder(10);
        StartCoroutine("DelayFindPlayer");

        ConnectionStatus.text += "Authenticated " + _resp.UserId;
    }
    #endregion
    //===========================================================================================
}
