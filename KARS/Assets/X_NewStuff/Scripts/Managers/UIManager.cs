using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    //SINGLETON
    public static UIManager Instance { get { return instance; } }
    private static UIManager instance;
    void Awake()
    {
        instance = this;
    }

    //NETWORKRELATED
    [Header("NETWORK")]
    public Text GameUpdateText;
    public Text PingText;
    public Text NetworkTimeText, GameTimeText;


    //POWERUP ICONS
    [Header("POWERUPS")]
    public Image SpeedBar_1;
    public Image SpeedBar_2;
    public Text SpeedTexT_1, SpeedText_2, 
                SpeedTimeText_1, SpeedTimeText_2, 
                SpeedMaxText_1, SpeedMaxText_2;

    public Image MissleBar_1, MissleBar_2,
                 ShieldBar_1, ShieldBar_2,
                 NitrosBar_1, NitrosBar_2;


    //STATS
    [Header("STATS")]
    public Image HealthBar_1;
    public Image HealthBar_2;
    public Text HealthText_1;
    public Text HealthText_2;


    //STATS
    [Header("PLAYER UI")]
    [SerializeField]
    private GameObject Player1Panel;
    [SerializeField]
    private GameObject Player2Panel;

    public GameObject Player1_SkillsParent, Player2_SkillsParent;

    public GameObject[] SkillIcons;


    //SCREENS
    [Header("SCREENS")]
    [SerializeField]
    private GameObject RespawningScreen;
    [SerializeField]
    private GameObject InGame_Result_PANEL,
                    InGame_CANVAS,
                    Debug_CANVAS,
                    Waiting_CANVAS,
                    MainMenu_CANVAS,
                    MainMenu_StatusPreview_PANEL,
                    MainMenu_CarPreview_PANEL,
                    MainMenu_Home_PANEL,
                    MainMenu_Quest_PANEL,
                    MainMenu_Shop_PANEL,
                    MainMenu_Social_PANEL;


    [SerializeField]
    private GameObject Player1_Win, Player2_Win;
    [SerializeField]
    private Image Player1_HPResult, Player2_HPResult;
    [SerializeField]
    private Image Player1_InGameHealthBar, Player2_InGameHealthBar;
    public Text Var_HP_1, Var_HP_2;
    //==========================================================================================
    #region MAIN MENU VARIABLES

    public void SetStatsPreviewScreen(bool _switch)
    {
        MainMenu_StatusPreview_PANEL.SetActive(_switch);
    }
    public void SetCarPreviewScreen(bool _switch)
    {
        MainMenu_CarPreview_PANEL.SetActive(_switch);
    }
    public void SetMainMenuPanel(bool _switch)
    {
        MainMenu_Home_PANEL.SetActive(_switch);
    }
    public void SetQuestPanel(bool _switch)
    {
        MainMenu_Quest_PANEL.SetActive(_switch);
    }
    public void SetShopPanel(bool _switch)
    {
        MainMenu_Shop_PANEL.SetActive(_switch);
    }
    public void SetSocialPanel(bool _switch)
    {
        MainMenu_Social_PANEL.SetActive(_switch);
    }
    #endregion


    #region CANVAS
    public void Set_Canvas_Main(bool _switch)
    {
        MainMenu_CANVAS.SetActive(_switch);
    }
    public void Set_Canvas_InGame(bool _switch)
    {
        InGame_CANVAS.SetActive(_switch);
    }
    public void Set_Canvas_Waiting(bool _switch)
    {
        Waiting_CANVAS.SetActive(_switch);
    }
    public void Set_Canvas_Debug(bool _switch)
    {
        Debug_CANVAS.SetActive(_switch);
    }
    #endregion

    #region IN GAME UI
    public void SetRespawnScreen(bool _switch)
    {
        RespawningScreen.SetActive(_switch);
    }
    public void SetResultScreen(bool _switch)
    {
        InGame_Result_PANEL.SetActive(_switch);
    }
    #endregion

    #region PLAYER STATS
    public void SetPlayerWin(bool _switch, int _player)
    {
        if (_player == 1)
        {
            Player1_Win.SetActive(_switch);
            Player2_Win.SetActive(!_switch);
        }
        else
        {
            Player1_Win.SetActive(!_switch);
            Player2_Win.SetActive(_switch);
        }
    }
    public void MirrorPlayerHp()
    {
        Player1_HPResult.fillAmount = Player1_InGameHealthBar.fillAmount;
        Player2_HPResult.fillAmount = Player2_InGameHealthBar.fillAmount;
    }
    public void AdjustHPBarAndText(int _player, float _val)
    {
        if(_player == 1)
        {
            Var_HP_1.text = _val.ToString();
            Player1_InGameHealthBar.fillAmount = _val / 5;
        }
        else
        {
            Var_HP_2.text = _val.ToString();
            Player2_InGameHealthBar.fillAmount = _val / 5;
        }
    }
    #endregion

    public void ActivatePlayerPanel(int _player)
    {
        if(_player == 1)
        {
            Player1Panel.SetActive(true);
            Player2Panel.SetActive(false);
        }
        else if (_player == 2)
        {
            Player1Panel.SetActive(false);
            Player2Panel.SetActive(true);
        }
        else
        {
            Player1Panel.SetActive(false);
            Player2Panel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            GameUpdateText.text = "";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StateManager.Instance.Access_ChangeState(MENUSTATE.RETURN_TO_MAIN_MENU);
        }
    }

}
