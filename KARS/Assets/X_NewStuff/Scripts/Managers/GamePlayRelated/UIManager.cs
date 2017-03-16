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
    #region NETWORK
    [Header("NETWORK")]
    public Text GameUpdateText;
    public Text PingText;
    public Text NetworkTimeText, GameTimeText;
    #endregion

    //POWERUP ICONS
    [Header("POWERUPS")]
    public Image SpeedBar_1;
    public Image SpeedBar_2;
    public Text SpeedTexT_1, SpeedText_2, 
                SpeedTimeText_1, SpeedTimeText_2, 
                SpeedMaxText_1, SpeedMaxText_2;
    
    
    //SKILLS
    [Header("PLAYER UI")]
    [SerializeField]
    private GameObject Player1Panel;
    [SerializeField]
    private GameObject Player2Panel;

    public GameObject[] Player1_SkillsParent, Player2_SkillsParent;
    public GameObject Player1_SkillsRoster, Player2_SkillsRoster;

    public GameObject[] SkillIcons;

    [SerializeField]
    private Image[] 
        Player1_SkillBlockers, 
        Player2_SkillBlockers;
    [SerializeField]
    private Text[]
        Player1_SkillSlotNames,
        Player2_SkillSlotNames;

    //SCREENS
    #region SCREENS
    [Header("SCREENS")]
    [SerializeField]
    private GameObject RespawningScreen;

    [SerializeField]
    private GameObject 
        InGame_Result_PANEL,
        InGame_CANVAS,
        Debug_CANVAS,
        Waiting_CANVAS,
        GameInit_CANVAS,
        Countdown_CANVAS,
        MainMenu_CANVAS,
        MainMenu_StatusPreview_PANEL,
        MainMenu_CarPreview_PANEL,
        MainMenu_Home_PANEL,
        MainMenu_Quest_PANEL,
        MainMenu_Shop_PANEL,
        MainMenu_Social_PANEL;
    #endregion

    //STATS
    [Header("STATS")]
    [SerializeField]
    private GameObject Player1_Win, Player2_Win;

    [SerializeField]
    private Image   
        Player1_HPResult, Player2_HPResult, 
        Player1_InGameHealthBar, Player2_InGameHealthBar;
    public Text Var_HP_1, Var_HP_2;


    [SerializeField]
    private GameObject 
        SelectCarButton, 
        SelectedCarImage, 
        MatchCancelButton;

    [SerializeField]
    private GameObject ExplosionPanel;


    [SerializeField]
    private Text ProgressText, ProgressTimerText, CountdownTimerText;


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
    public void Set_Canvas_Countdown(bool _switch)
    {
        Countdown_CANVAS.SetActive(_switch);
    }
    public void Set_Canvas_Debug(bool _switch)
    {
        Debug_CANVAS.SetActive(_switch);
    }
    public void Set_Canvas_GameInit(bool _switch)
    {
        GameInit_CANVAS.SetActive(_switch);
    }
    #endregion

    #region IN GAME UI
    public void SetRespawnScreen(bool _switch)
    {
        RespawningScreen.SetActive(_switch);
    }

    public bool GetRespawnScreen()
    {
        return RespawningScreen.activeInHierarchy;
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
    public void StartCooldDownForBlockers(int _player, int _slot, float _currentVal, float _maxVal,string _name)
    {
        Image imgToCd = null;
        Text textToRefer = null;
        if(_player == 1)
        {
            imgToCd = Player1_SkillBlockers[_slot];
            textToRefer = Player1_SkillSlotNames[_slot];
        }
        else if (_player == 2)
        {
            imgToCd = Player2_SkillBlockers[_slot];
            textToRefer = Player2_SkillSlotNames[_slot];
        }
        else
        {
            return;
        }

        imgToCd.fillAmount = _currentVal / _maxVal;
        textToRefer.text = _name+"\n"+ ((int)(_maxVal - _currentVal));
        if (imgToCd.fillAmount > .98f)
        {
            imgToCd.gameObject.SetActive( false );
        }
        else
        {
            imgToCd.gameObject.SetActive( true );
        }
    }

    public void SetExplosionPanel(bool _switch)
    {
        ExplosionPanel.SetActive(_switch);
    }

    public void SetProgressText(string _val)
    {
        ProgressText.text = _val+"%";
    }
    public void SetProgressTimerText(string _val)
    {
        ProgressTimerText.text =  _val;
    }
    public void SetCountdownTimerText(string _val)
    {
        CountdownTimerText.text = _val;
    }

    #region MENU BUTTONS
    public void ActivateSelectCarButton(bool _switch)
    {
        SelectCarButton.SetActive(_switch);
        SelectedCarImage.SetActive(!_switch);
    }
    public void SetMatchCancelButton(bool _switch)
    {
        MatchCancelButton.SetActive(_switch);
    }
    #endregion
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            GameUpdateText.text = "";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StateManager.Instance.Access_ChangeState(MENUSTATE.RESULT);
        }
    }

    public void MoveUpLog()
    {
        GameUpdateText.transform.position += transform.up;
    }
    public void MoveDownLog()
    {
        GameUpdateText.transform.position -= transform.up;
    }
}
