using GameSparks.RT;
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

    public void OnClick_BackToMainMenu()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.RETURN_TO_MAIN_MENU);
    }
    public void OnClick_ResetGame()
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, 0);
            GameSparkPacketReceiver.Instance.GetRTSession().SendData(OPCODE_CLASS.ResetOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
        StateManager.Instance.Access_ChangeState(MENUSTATE.RESTART_GAME);
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
}
