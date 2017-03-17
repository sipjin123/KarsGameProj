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
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.MATCH_FIND);
    }
    public void OnClick_CancelMathcFind()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.CANCEL_MATCH_FIND);
    }
    public void OnClick_BackToMainMenu()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.RETURN_TO_MAIN_MENU);
    }
    public void OnClick_ResetGame()//FOR DEBUG PURPOSES ONLY
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        GameSparkPacketHandler.Instance.Global_SendState(MENUSTATE.RESTART_GAME);
    }
    public void OnClick_QuitGame()//FOR DEBUG PURPOSES ONLY
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        GameSparkPacketHandler.Instance.Global_SendState(MENUSTATE.RETURN_TO_MAIN_MENU);
    }
    public void OnClick_ViewCarStats()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.CHARACTER_STATS_VIEW);
    }
    public void OnClick_SelectCurrentCar()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.CHARACTER_SELECT);
    }
    public void OnClick_CharacterScreen()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.CHARACTER_SELECT);
    }
    public void OnClick_HomeScreen()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.HOME);
    }
    public void OnClick_QuestScreen()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.QUEST);
    }
    public void OnClick_ShopScreen()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.SHOP);
    }
    public void OnClick_SocialScreen()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.SOCIAL);
    }
}
