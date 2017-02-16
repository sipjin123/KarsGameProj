using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using UnityEngine.UI;
using GameSparks.Api.Messages;
using System.Text;
using System;

public class RegistrationSparks : MonoBehaviour {
    //===========================================================================================
    #region VARIABLES
    private static RegistrationSparks _instance;
    public static RegistrationSparks Instance { get { return _instance; } }
    [SerializeField]
    Text ConnectionStatus;
    
    public InputField DisplayName, UserName, Password;
    public string PeerID;
    public Text playerList;
    
    public GameObject PlayerTemplate;
    public GameObject PlayerMSTemplate;
    public Transform PlayerMSTemplateParent;
    #endregion
    //===========================================================================================
    //INITIALIZATION
    #region INITIALIZATION
    void Awake()
    {
        _instance = this;
        Application.logMessageReceivedThreaded += HandleLog;
        PlayerPrefs.SetInt("TestMode", 0);
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
        LoginButton();
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
        Debug.LogError("Attempting Matchmaking...");
        new MatchmakingRequest()
            .SetMatchShortCode("B0ntakun") // set the shortCode to be the same as the one we created in the first tutorial
            .SetSkill(0) // in this case we assume all players have skill level zero and we want anyone to be able to join so the skill level for the request is set to zero
            .Send((response) => {
                if (response.HasErrors)
                { // check for errors
                    Debug.LogError(" Match make Error...\n" + response.Errors.JSON);
                }
            });
    }
    
    private void OnMatchFound(MatchFoundMessage _message)
    {
        //tempRTSessionInfo = new RTSessionInfo(_message);
        Debug.LogError(" Match Found!...");
        StringBuilder sBuilder = new StringBuilder();
        sBuilder.AppendLine("Match Found...");
        sBuilder.AppendLine("Host URL:" + _message.Host);
        sBuilder.AppendLine("Port:" + _message.Port);
        sBuilder.AppendLine("Access Token:" + _message.AccessToken);
        sBuilder.AppendLine("MatchId:" + _message.MatchId);
        sBuilder.AppendLine("Opponents:" + _message.Participants);

        sBuilder.AppendLine("_________________");
        sBuilder.AppendLine(); // we'll leave a space between the player-list and the match data
        int totalPlayers = 0;
        foreach (MatchFoundMessage._Participant player in _message.Participants)
        {
            if (_message.JSONData["playerId"].ToString() != player.Id)
            {
                sBuilder.AppendLine("Enemy Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
                SpawnPlayers((int)player.PeerId);

                try
                {
                    GameObject tempr = Instantiate(PlayerMSTemplate, transform.position, Quaternion.identity) as GameObject;
                    tempr.transform.SetParent(PlayerMSTemplateParent);
                    tempr.transform.localScale = Vector3.one;
                }
                catch { }
            }
            else
            {
                sBuilder.AppendLine("Your Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
                PeerID = player.PeerId.ToString();
                SpawnPlayers((int)player.PeerId);

                try
                {
                    GameObject tempr = Instantiate(PlayerMSTemplate, transform.position, Quaternion.identity) as GameObject;
                    tempr.transform.SetParent(PlayerMSTemplateParent);
                    tempr.transform.localScale = Vector3.one;
                }
                catch { }
            }
            totalPlayers += 1;
        }
        playerList.text = sBuilder.ToString(); // set the string to be the player-list field
        
        RTSessionInfo sessionInfo = new RTSessionInfo(_message);
        Debug.LogError("Writen builder: "+ sessionInfo);
        GameSparksManager.Instance.StartNewRTSession(sessionInfo);
        
    }
    #endregion
    //===========================================================================================
    //BUTTONS
    #region LOGIN/REGISTER BUTTONS
    public void LoginButton()
    {
        AuthenticateUser(UserName.text, "test", OnRegistration, OnAuthentication);
    }
    public void RegisterButton()
    {
       
        Debug.LogError("Trynhg to register");
        new RegistrationRequest()
            .SetDisplayName(DisplayName.text)
            .SetPassword(Password.text)
            .SetUserName(UserName.text)
            .Send((response) => {
                string authToken = response.AuthToken;
                string displayName = response.DisplayName;
                bool? newPlayer = response.NewPlayer;
                GSData scriptData = response.ScriptData;
                var switchSummary = response.SwitchSummary;
                string userId = response.UserId;

                if (!response.HasErrors)
                {
                    Debug.LogError("Player Registered: " + displayName);
                }
                else
                {
                    Debug.LogError("Registry Fail " + response.Errors.JSON.ToString());
                }
            });
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
        GS.GSPlatform.ExecuteOnMainThread(() => {
            HandleLog(logString, null, LogType.Log);
        });
    }

    void HandleLog(string logString, string stackTrace, LogType logType)
    {
 
    }

    private void OnRegistration(RegistrationResponse _resp)
    {
        Debug.LogError("REGISTEREDD: " + _resp.UserId);
        ConnectionStatus.text += "Registered " + _resp.UserId;
        StartCoroutine("DelayFindPlayer");
    }
    private void OnAuthentication(AuthenticationResponse _resp)
    {
        Debug.LogError("USER IS AUTHENTICATED? :" + _resp.DisplayName);
        ConnectionStatus.text += "Authenticated " + _resp.UserId;
        StartCoroutine("DelayFindPlayer");
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
    //GAME TESTING
    #region GAME TESTING

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameSparksManager.Instance.GetRTSession().Disconnect();
            GS.Disconnect();
            Application.Quit();
        }
    }
    #endregion
    //===========================================================================================




    void SpawnPlayers(int _peerID)
    {
        GameSparks_DataSender _obj = null;
        if (_peerID == 1)
        {
            _obj = GameObject.Find("BlueCar").GetComponent<GameSparks_DataSender>();
            _obj.NetworkID = _peerID;
            _obj.setWhatToControl();
            GameSparksManager.Instance.tankPool.Add(_obj.gameObject);
        }
        if (_peerID == 2)
        {
            _obj = GameObject.Find("RedCar").GetComponent<GameSparks_DataSender>();
            _obj.NetworkID = _peerID;
            _obj.setWhatToControl();
            GameSparksManager.Instance.tankPool.Add(_obj.gameObject);
        }
        /*
        GameObject temp = Instantiate(PlayerTemplate, transform.position, Quaternion.identity) as GameObject;
        temp.GetComponent<GameSparks_DataSender>().NetworkID = peerID;
        temp.transform.SetParent(GameSparksManager.Instance.tankPool);
        temp.SetActive(true);*/
    }
}
