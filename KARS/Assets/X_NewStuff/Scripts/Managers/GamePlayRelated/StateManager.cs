using GameSparks.Core;
using UnityEngine;

public class StateManager : MonoBehaviour {

    public static StateManager Instance { get { return instance; } }
    private static StateManager instance;
    void Awake()
    {
        instance = this;
    }
    


    void ResetAllMainMenuPanels()
    {
        UIManager.Instance.SetStatsPreviewScreen(false);
        UIManager.Instance.SetCarPreviewScreen(false);
        UIManager.Instance.SetMainMenuPanel(false);
        UIManager.Instance.SetQuestPanel(false);
        UIManager.Instance.SetShopPanel(false);
        UIManager.Instance.SetSocialPanel(false);
    }

    public void Access_ChangeState(MENUSTATE _state)
    {
        switch (_state)
        {
            case MENUSTATE.HOME:
                {
                    ResetAllMainMenuPanels();
                    UIManager.Instance.SetMainMenuPanel(true);
                }
                break;
            case MENUSTATE.QUEST:
                {
                    ResetAllMainMenuPanels();
                    UIManager.Instance.SetQuestPanel(true);
                }
                break;
            case MENUSTATE.SHOP:
                {
                    ResetAllMainMenuPanels();
                    UIManager.Instance.SetShopPanel(true);
                }
                break;
            case MENUSTATE.SOCIAL:
                {
                    ResetAllMainMenuPanels();
                    UIManager.Instance.SetSocialPanel(true);
                }
                break;
            case MENUSTATE.CHARACTER_SELECT:
                {
                    TronGameManager.Instance.Access_UpdateCarSelection();
                    ResetAllMainMenuPanels();
                    UIManager.Instance.SetCarPreviewScreen(true);
                }
                break;
            case MENUSTATE.CHARACTER_STATS_VIEW:
                {
                    ResetAllMainMenuPanels();
                    UIManager.Instance.SetStatsPreviewScreen(true);
                }
                break;


            case MENUSTATE.MATCH_FIND:
                {
                    RegisterGameSpark.Instance.Access_LoginAuthentication();
                    TronGameManager.Instance.ClearLogs();
                    TronGameManager.Instance.StartProgressSession();
                    TronGameManager.Instance.BlockMatchFinding = false;
                    ResetAllMainMenuPanels();
                    UIManager.Instance.Set_Canvas_Waiting(true);
                }
                break;
            case MENUSTATE.MATCH_FOUND:
                {
                    UIManager.Instance.Set_Canvas_Main(false);
                    //TEST PLS RETURN LATER
                    //TronGameManager.Instance.ReceiveSignalToStartGame();
                }
                break;

            case MENUSTATE.CANCEL_MATCH_FIND:
                {
                    AccessResetGameVariables();
                    Access_ChangeState(MENUSTATE.RETURN_TO_MAIN_MENU);
                }
                break;
            case MENUSTATE.RESTART_GAME:
                {
                    UIManager.Instance.SetResultScreen(false);
                    GameSparkPacketHandler.Instance.Access_ResetClock();

                    for (int i = 0; i < TronGameManager.Instance.PlayerObjects.Length; i++)
                    {
                        GameObject _obj = TronGameManager.Instance.PlayerObjects[i].gameObject;
                        Car_DataReceiver _GameSparks_DataSender = _obj.GetComponent<Car_DataReceiver>();
                        Car_Movement _carMovement = _obj.GetComponent<Car_Movement>();

                        //_carMovement._trailCollision.SetEmiision(false);
                        //_carMovement._trailCollision.Reset_Mesh();
                        _GameSparks_DataSender.InitCam();
                        _GameSparks_DataSender.Health = 6;
                        _carMovement.Die();
                    }
                }
                break;
            case MENUSTATE.START_GAME:
                {
                    UIManager.Instance.Set_Canvas_InGame(true);
                    UIManager.Instance.SetRespawnScreen(true);
                    
                    UIManager.Instance.SetResultScreen(false);
                }
                break;
            case MENUSTATE.RESULT:
                {
                    TronGameManager.Instance.BlockMatchFinding = true;
                    //RESULTS SCREEN
                    UIManager.Instance.Set_Canvas_Waiting(false);
                    UIManager.Instance.Set_Canvas_GameInit(false);
                    UIManager.Instance.SetResultScreen(true);

                    if(int.Parse( UIManager.Instance.Var_HP_1.text) >int.Parse(UIManager.Instance.Var_HP_2.text))
                    {
                        UIManager.Instance.SetPlayerWin(true, 1);
                    }
                    else
                    {
                        UIManager.Instance.SetPlayerWin(true, 2);
                    }
                    UIManager.Instance.MirrorPlayerHp();
                    AccessResetGameVariables();
                }
                break;
            case MENUSTATE.RETURN_TO_MAIN_MENU:
                {
                    AccessResetGameVariables();
                    GameSparkPacketHandler.Instance.Access_ResetNetwork();
                    //UI SETUP
                    ResetAllMainMenuPanels();
                    UIManager.Instance.Set_Canvas_Main(true);
                    UIManager.Instance.SetMainMenuPanel(true);

                    UIManager.Instance.SetResultScreen(false);
                    UIManager.Instance.SetRespawnScreen(false);
                    UIManager.Instance.Set_Canvas_InGame(false);
                    UIManager.Instance.Set_Canvas_Waiting(false);
                    UIManager.Instance.Set_Canvas_GameInit(false);
                }
                break;
        }
    }

    public void AccessResetGameVariables()
    {
        //PLAYER OBJECT RESET
        GameSparkPacketHandler.Instance.Access_ReInitializeGameSparks();
        GameSparkPacketHandler.Instance.Access_PlayerReset();

        GameSparkPacketHandler.Instance.AccessResetBoolList();

        //DEACTIVATE ALL SKILLS PANEL
        //DEACTIVATE ALL SKILLS ICON IN GAME
        UIManager.Instance.ActivatePlayerPanel(0);
        for (int i = 0; i < 2; i++)
        {
            foreach (Transform t in UIManager.Instance.Player1_SkillsParent[i].transform)
            {
                t.gameObject.SetActive(false);
            }
            foreach (Transform t in UIManager.Instance.Player2_SkillsParent[i].transform)
            {
                t.gameObject.SetActive(false);
            }
        }
        RegisterGameSpark.Instance.Access_StopFindingPlayers();
        //GAMESPARKS DISCONNECTION
        try
        {
            GameSparkPacketHandler.Instance.GetRTSession().Disconnect();
        }
        catch
        {
            UIManager.Instance.GameUpdateText.text += "\nNonExisting RT Session";
        }
        GS.Disconnect();
    }
}