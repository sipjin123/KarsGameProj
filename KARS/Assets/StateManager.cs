using System.Collections;
using System.Collections.Generic;
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
            case MENUSTATE.MATCH_FOUND:
                {

                    UIManager.Instance.GameUpdateText.text += "\nState: MatchFound";
                    TronGameManager.Instance.ReceiveSignalToStartGame();
                }
                break;
            case MENUSTATE.CHARACTER_SELECT:
                {

                }
                break;
            case MENUSTATE.MATCH_FIND:
                {
                    UIManager.Instance.SetWaitingScreen(true);
                    RegisterGameSpark.Instance.Access_LoginAuthentication();
                }
                break;
            case MENUSTATE.START_GAME:
                {
                    UIManager.Instance.GameUpdateText.text += "\nState: Start Game";
                    UIManager.Instance.SetDebugScreen(true);
                    UIManager.Instance.SetInGameScreen(true);

                    UIManager.Instance.SetWaitingScreen(false);
                    UIManager.Instance.SetResultScreen(false);
                    UIManager.Instance.SetRespawnScreen(true);


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
                    UIManager.Instance.SetCharacterSelectScreen(true);
                    UIManager.Instance.SetRespawnScreen(false);
                    UIManager.Instance.SetInGameScreen(false);
                    UIManager.Instance.SetDebugScreen(false);
                    UIManager.Instance.GameUpdateText.text += "\nRESETING MENU PANELS";

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

public enum MENUSTATE
{
    MATCH_FOUND,
    CHARACTER_SELECT,
    MATCH_FIND,
    START_GAME,
    RESULT,
    RETURN_TO_MAIN_MENU
}