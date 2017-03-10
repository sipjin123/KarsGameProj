using UnityEngine;

public class StateButtonManager : MonoBehaviour {

    public static StateButtonManager Instance { get { return instance; } }
    private static StateButtonManager instance;
    void Awake()
    {
        instance = this;
    }


    public void OnClick_StartGame()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.MATCH_FIND);
    }

    public void OnClick_CancelMathcFind()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.CANCEL_MATCH_FIND);
    }
    public void OnClick_BackToMainMenu()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.RETURN_TO_MAIN_MENU);
    }
    public void OnClick_ResetGame()//FOR DEBUG PURPOSES ONLY
    {
        TronGameManager.Instance.Global_SendState(MENUSTATE.RESTART_GAME);
    }
    public void OnClick_QuitGame()//FOR DEBUG PURPOSES ONLY
    {
        TronGameManager.Instance.Global_SendState(MENUSTATE.RETURN_TO_MAIN_MENU);
    }
    public void OnClick_ViewCarStats()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.CHARACTER_STATS_VIEW);
    }
    public void OnClick_SelectCurrentCar()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.CHARACTER_SELECT);
    }
    public void OnClick_CharacterScreen()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.CHARACTER_SELECT);
    }
    public void OnClick_HomeScreen()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.HOME);
    }
    public void OnClick_QuestScreen()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.QUEST);
    }
    public void OnClick_ShopScreen()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.SHOP);
    }
    public void OnClick_SocialScreen()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.SOCIAL);
    }
}
