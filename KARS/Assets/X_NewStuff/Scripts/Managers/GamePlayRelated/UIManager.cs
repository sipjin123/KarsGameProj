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

<<<<<<< HEAD

    //SKILLS
    #region SKILLS
=======
    //POWERUP ICONS
    [Header("POWERUPS")]
    public Image SpeedBar_1;
    public Image SpeedBar_2;
    public Text SpeedTexT_1, SpeedText_2, 
                SpeedTimeText_1, SpeedTimeText_2, 
                SpeedMaxText_1, SpeedMaxText_2;
    
    
    //SKILLS
>>>>>>> 3e3b2bc (Sorting files)
    [Header("PLAYER UI")]
    [SerializeField]
    private GameObject Player1Panel;
    [SerializeField]
    private GameObject Player2Panel;

<<<<<<< HEAD
    public GameObject[] Player1_SkillsParent, Player2_SkillsParent;
    public GameObject Player1_SkillsRoster, Player2_SkillsRoster;
=======
    public GameObject Player1_SkillsParent, Player2_SkillsParent;
>>>>>>> 3e3b2bc (Sorting files)

    public GameObject[] SkillIcons;

    [SerializeField]
    private Image[] 
        Player1_SkillBlockers, 
        Player2_SkillBlockers;
<<<<<<< HEAD
    [SerializeField]
    private Text[]
        Player1_SkillSlotNames,
        Player2_SkillSlotNames;
    #endregion
=======
>>>>>>> 3e3b2bc (Sorting files)

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
<<<<<<< HEAD
        GameInit_CANVAS,
        Countdown_CANVAS,
=======
>>>>>>> 3e3b2bc (Sorting files)
        MainMenu_CANVAS,
        MainMenu_StatusPreview_PANEL,
        MainMenu_CarPreview_PANEL,
        MainMenu_Home_PANEL,
        MainMenu_Quest_PANEL,
        MainMenu_Shop_PANEL,
        MainMenu_Social_PANEL;
    #endregion

    //STATS
<<<<<<< HEAD
    #region STATS
    //STATS
=======
>>>>>>> 3e3b2bc (Sorting files)
    [Header("STATS")]
    [SerializeField]
    private GameObject Player1_Win, Player2_Win;

    [SerializeField]
    private Image   
        Player1_HPResult, Player2_HPResult, 
        Player1_InGameHealthBar, Player2_InGameHealthBar;
    public Text Var_HP_1, Var_HP_2;
<<<<<<< HEAD
    #endregion

    //BUTTONS
    #region BUTTONS
    [Header("Buttons")]
    [SerializeField]
    private GameObject SelectedCarImage;
    [SerializeField]
    private GameObject SelectCarButton;
    [SerializeField]
    private GameObject MatchCancelButton;
    #endregion

    //LOADING PREGAME
    #region LOADING PREGAME
    [Header("PREGAME LOADERS")]
    [SerializeField]
    private Text ProgressText;
    [SerializeField]
    private Text ProgressTimerText;
    [SerializeField]
    private Text CountdownTimerText;
    #endregion

    //POWERUP ICONS
    [Header("POWERUPS")]
    public Image SpeedBar_1;
    public Image SpeedBar_2;
    public Text SpeedTexT_1, SpeedText_2,
                SpeedTimeText_1, SpeedTimeText_2,
                SpeedMaxText_1, SpeedMaxText_2;


    [Header("UNASSIGNED")]
    [SerializeField]
    private GameObject ExplosionPanel;


    [SerializeField]
    private GameObject DisconnectPanel;

    public void SetDisconnectedPanel(bool _switch)
    {
        DisconnectPanel.SetActive(_switch);
    }

=======


    [SerializeField]
    private GameObject 
        SelectCarButton, 
        SelectedCarImage, 
        MatchCancelButton;
>>>>>>> 3e3b2bc (Sorting files)
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

<<<<<<< HEAD
=======

>>>>>>> 3e3b2bc (Sorting files)
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
<<<<<<< HEAD
    public void Set_Canvas_Countdown(bool _switch)
    {
        Countdown_CANVAS.SetActive(_switch);
    }
=======
>>>>>>> 3e3b2bc (Sorting files)
    public void Set_Canvas_Debug(bool _switch)
    {
        Debug_CANVAS.SetActive(_switch);
    }
<<<<<<< HEAD
    public void Set_Canvas_GameInit(bool _switch)
    {
        GameInit_CANVAS.SetActive(_switch);
    }
    #endregion

    #region IN GAME UI
    public void ActivatePlayerPanel(int _player)
    {
        if (_player == 1)
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
=======
    #endregion

    #region IN GAME UI
>>>>>>> 3e3b2bc (Sorting files)
    public void SetRespawnScreen(bool _switch)
    {
        RespawningScreen.SetActive(_switch);
    }
<<<<<<< HEAD

    public void SetExplosionPanel(bool _switch)
    {
        ExplosionPanel.SetActive(_switch);
    }
    public bool GetRespawnScreen()
    {
        return RespawningScreen.activeInHierarchy;
    }
=======
>>>>>>> 3e3b2bc (Sorting files)
    public void SetResultScreen(bool _switch)
    {
        InGame_Result_PANEL.SetActive(_switch);
    }
<<<<<<< HEAD

    public void StartCooldDownForBlockers(int _player, int _slot, float _currentVal, float _maxVal, string _name)
    {
        Image imgToCd = null;
        Text textToRefer = null;
        if (_player == 1)
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
        textToRefer.text = _name + "\n" + ((int)(_maxVal - _currentVal));
        if (imgToCd.fillAmount > .98f)
        {
            imgToCd.gameObject.SetActive(false);
        }
        else
        {
            imgToCd.gameObject.SetActive(true);
        }
    }

=======
>>>>>>> 3e3b2bc (Sorting files)
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

<<<<<<< HEAD
    #region PRE GAME LOADING 
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
    #endregion
=======
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
    public void StartCooldDownForBlockers(int _player, int _slot, float _currentVal, float _maxVal)
    {
        Image imgToCd = null;
        if(_player == 1)
        {
            imgToCd = Player1_SkillBlockers[_slot];
        }
        else if (_player == 2)
        {
            imgToCd = Player2_SkillBlockers[_slot];
        }
        else
        {
            return;
        }

        imgToCd.fillAmount = _currentVal / _maxVal;
        if(imgToCd.fillAmount > .98f)
        {
            imgToCd.enabled = false;
        }
        else
        {
            imgToCd.enabled = true;
        }
    }
>>>>>>> 3e3b2bc (Sorting files)

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
<<<<<<< HEAD

    #region DEBUG
=======
>>>>>>> 3e3b2bc (Sorting files)
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

<<<<<<< HEAD
    public void MoveUpLog()
    {
        GameUpdateText.transform.position += transform.up;
    }
    public void MoveDownLog()
    {
        GameUpdateText.transform.position -= transform.up;
    }
    #endregion
=======
>>>>>>> 3e3b2bc (Sorting files)
}
