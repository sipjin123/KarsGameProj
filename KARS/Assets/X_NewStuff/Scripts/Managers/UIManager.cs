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
    public GameObject Player1Panel;
    public GameObject Player2Panel;

    public GameObject Player1_SkillsParent, Player2_SkillsParent;

    public GameObject[] SkillIcons;


    //SCREENS
    [Header("SCREENS")]
    [SerializeField]
    private GameObject RespawningScreen;
    [SerializeField]
    private GameObject ResultScreen,
                    InGame_CANVAS, 
                    Debug_CANVAS, 
                    Waiting_CANVAS,
                    Innter_CharacterSelectPanel,
                    Outer_CharacterSelectPanel,
                    MainMenu_Canvas;
    //==========================================================================================
    public void SetRespawnScreen(bool _switch)
    {
        RespawningScreen.SetActive(_switch);
    }
    public void SetResultScreen(bool _switch)
    {
        ResultScreen.SetActive(_switch);
    }
    public void SetDebugScreen(bool _switch)
    {
        Debug_CANVAS.SetActive(_switch);
    }
    public void SetInGameScreen(bool _switch)
    {
        InGame_CANVAS.SetActive(_switch);
    }
    public void SetWaitingScreen(bool _switch)
    {
        Waiting_CANVAS.SetActive(_switch);
    }
    public void SetInnterCharacterSelectScreen(bool _switch)
    {
        Innter_CharacterSelectPanel.SetActive(_switch);
    }
    public void SetOuterCharacterSelectScreen(bool _switch)
    {
        Outer_CharacterSelectPanel.SetActive(_switch);
    }
    public void SetMenuCanvas(bool _switch)
    {
        MainMenu_Canvas.SetActive(_switch);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Delete))
        {
            GameUpdateText.text = "";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StateManager.Instance.Access_ChangeState(MENUSTATE.RETURN_TO_MAIN_MENU);
        }
    }
}
