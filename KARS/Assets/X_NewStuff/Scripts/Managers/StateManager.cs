using GameSparks.Core;
using UnityEngine;

public class StateManager : MonoBehaviour {

    public static StateManager Instance { get { return instance; } }
    private static StateManager instance;
    void Awake()
    {
        instance = this;
    }
    


    public void Access_ChangeState(MENUSTATE _state)
    {
        switch (_state)
        {
            case MENUSTATE.HOME:
                {
                    UIManager.Instance.SetInnterCharacterSelectScreen(false);
                    UIManager.Instance.SetOuterCharacterSelectScreen(false);
                    UIManager.Instance.SetMenuCanvas(true);
                }
                break;
            case MENUSTATE.CHARACTER_SELECT:
                {
                    UIManager.Instance.SetMenuCanvas(false);
                    UIManager.Instance.SetInnterCharacterSelectScreen(false);
                    UIManager.Instance.SetOuterCharacterSelectScreen(true);
                }
                break;
            case MENUSTATE.CHARACTER_STATS_VIEW:
                {
                    UIManager.Instance.SetMenuCanvas(false);
                    UIManager.Instance.SetInnterCharacterSelectScreen(true);
                    UIManager.Instance.SetOuterCharacterSelectScreen(false);
                }
                break;


            case MENUSTATE.MATCH_FOUND:
                {
                    TronGameManager.Instance.ReceiveSignalToStartGame();
                }
                break;
            case MENUSTATE.MATCH_FIND:
                {
                    UIManager.Instance.SetWaitingScreen(true);
                    UIManager.Instance.SetMenuCanvas(false);
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
                    UIManager.Instance.SetDebugScreen(true);
                    UIManager.Instance.SetInGameScreen(true);
                    UIManager.Instance.SetRespawnScreen(true);

                    UIManager.Instance.SetWaitingScreen(false);
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
                    UIManager.Instance.SetMenuCanvas(true);
                    UIManager.Instance.SetOuterCharacterSelectScreen(false);
                    UIManager.Instance.SetInnterCharacterSelectScreen(false);
                    UIManager.Instance.SetResultScreen(false);
                    UIManager.Instance.SetRespawnScreen(false);
                    UIManager.Instance.SetInGameScreen(false);
                    UIManager.Instance.SetDebugScreen(false);
                    UIManager.Instance.SetWaitingScreen(false);

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