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
                


            case MENUSTATE.MATCH_FOUND:
                {
                    UIManager.Instance.Set_Canvas_Main(false);
                    TronGameManager.Instance.ReceiveSignalToStartGame();
                }
                break;
            case MENUSTATE.MATCH_FIND:
                {
                    ResetAllMainMenuPanels();
                    UIManager.Instance.Set_Canvas_Waiting(true);
                    RegisterGameSpark.Instance.Access_LoginAuthentication();
                }
                break;
            case MENUSTATE.RESTART_GAME:
                {
                    UIManager.Instance.SetResultScreen(false);
                    GameSparkPacketReceiver.Instance.Access_ResetClock();

                    for (int i = 0; i < GameSparkPacketReceiver.Instance._carPool.Count; i++)
                    {
                        GameObject _obj = GameSparkPacketReceiver.Instance._carPool[i].gameObject;
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
                    UIManager.Instance.Set_Canvas_Debug(true);
                    UIManager.Instance.Set_Canvas_InGame(true);
                    UIManager.Instance.SetRespawnScreen(true);

                    UIManager.Instance.Set_Canvas_Waiting(false);
                    UIManager.Instance.SetResultScreen(false);


                    Transform skillParent;
                    if (GameSparkPacketReceiver.Instance.PeerID == 1)
                        skillParent = UIManager.Instance.Player1_SkillsParent.transform;
                    else
                        skillParent = UIManager.Instance.Player2_SkillsParent.transform;

                    foreach (Transform T in skillParent)
                    {
                        if (T.gameObject.name == TronGameManager.Instance.selected_currentSkill_Text[0].text 
                            || T.gameObject.name == TronGameManager.Instance.selected_currentSkill_Text[1].text)
                        {
                            T.gameObject.SetActive(true);
                        }
                    }

                }
                break;
            case MENUSTATE.RESULT:
                {
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
                }
                break;
            case MENUSTATE.RETURN_TO_MAIN_MENU:
                {
                    //GAMESPARKS DISCONNECTION
                    try
                    {
                        GameSparkPacketReceiver.Instance.GetRTSession().Disconnect();
                    }
                    catch
                    {
                        UIManager.Instance.GameUpdateText.text += "\nNonExisting RT Session";
                    }
                    GS.Disconnect();

                    //UI SETUP
                    ResetAllMainMenuPanels();
                    UIManager.Instance.Set_Canvas_Main(true);
                    UIManager.Instance.SetMainMenuPanel(true);

                    UIManager.Instance.SetResultScreen(false);
                    UIManager.Instance.SetRespawnScreen(false);
                    UIManager.Instance.Set_Canvas_InGame(false);
                    UIManager.Instance.Set_Canvas_Debug(false);
                    UIManager.Instance.Set_Canvas_Waiting(false);

                    //PLAYER OBJECT RESET
                    TronGameManager.Instance.Access_ReInitializeGameSparks();
                    TronGameManager.Instance.Access_PlayerReset();

                    //DEACTIVATE ALL SKILLS PANEL
                    UIManager.Instance.Player1Panel.SetActive(false);
                    UIManager.Instance.Player2Panel.SetActive(false);
                    //DEACTIVATE ALL SKILLS ICON IN GAME
                    foreach (Transform t in UIManager.Instance.Player1_SkillsParent.transform)
                    {
                        t.gameObject.SetActive(false);
                    }
                    foreach (Transform t in UIManager.Instance.Player2_SkillsParent.transform)
                    {
                        t.gameObject.SetActive(false);
                    }
                }
                break;
        }
    }
}