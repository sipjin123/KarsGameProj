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
<<<<<<< HEAD
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
        StateManager.Instance.Access_ChangeState(MENUSTATE.MATCH_FIND);
    }
    public void OnClick_CancelMathcFind()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
=======
        StateManager.Instance.Access_ChangeState(MENUSTATE.MATCH_FIND);
    }

    public void OnClick_CancelMathcFind()
    {
>>>>>>> 3e3b2bc (Sorting files)
        StateManager.Instance.Access_ChangeState(MENUSTATE.CANCEL_MATCH_FIND);
    }
    public void OnClick_BackToMainMenu()
    {
<<<<<<< HEAD
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
=======
>>>>>>> 3e3b2bc (Sorting files)
        StateManager.Instance.Access_ChangeState(MENUSTATE.RETURN_TO_MAIN_MENU);
    }
    public void OnClick_ResetGame()//FOR DEBUG PURPOSES ONLY
    {
<<<<<<< HEAD
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
=======
        TronGameManager.Instance.Global_SendState(MENUSTATE.RESTART_GAME);
    }
    public void OnClick_QuitGame()//FOR DEBUG PURPOSES ONLY
    {
        TronGameManager.Instance.Global_SendState(MENUSTATE.RETURN_TO_MAIN_MENU);
    }
    public void OnClick_ViewCarStats()
    {
>>>>>>> 3e3b2bc (Sorting files)
        StateManager.Instance.Access_ChangeState(MENUSTATE.CHARACTER_STATS_VIEW);
    }
    public void OnClick_SelectCurrentCar()
    {
<<<<<<< HEAD
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
=======
>>>>>>> 3e3b2bc (Sorting files)
        StateManager.Instance.Access_ChangeState(MENUSTATE.CHARACTER_SELECT);
    }
    public void OnClick_CharacterScreen()
    {
<<<<<<< HEAD
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
=======
>>>>>>> 3e3b2bc (Sorting files)
        StateManager.Instance.Access_ChangeState(MENUSTATE.CHARACTER_SELECT);
    }
    public void OnClick_HomeScreen()
    {
<<<<<<< HEAD
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
=======
>>>>>>> 3e3b2bc (Sorting files)
        StateManager.Instance.Access_ChangeState(MENUSTATE.HOME);
    }
    public void OnClick_QuestScreen()
    {
<<<<<<< HEAD
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
=======
>>>>>>> 3e3b2bc (Sorting files)
        StateManager.Instance.Access_ChangeState(MENUSTATE.QUEST);
    }
    public void OnClick_ShopScreen()
    {
<<<<<<< HEAD
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
=======
>>>>>>> 3e3b2bc (Sorting files)
        StateManager.Instance.Access_ChangeState(MENUSTATE.SHOP);
    }
    public void OnClick_SocialScreen()
    {
<<<<<<< HEAD
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.BUTTON);
=======
>>>>>>> 3e3b2bc (Sorting files)
        StateManager.Instance.Access_ChangeState(MENUSTATE.SOCIAL);
    }
}
