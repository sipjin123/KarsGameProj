using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {


    public static UIManager Instance { get { return instance; } }
    public static UIManager instance;
    void Awake()
    {
        instance = this;
    }
    public Image HealthBar_1, HealthBar_2;
    public Text  HealthText_1,HealthText_2;

    public Text GameUpdateText;
    public Text PingText;
    public Text NetworkTimeText, GameTimeText;

    public GameObject Player1Panel, Player2Panel;
    public Image SpeedBar_1, SpeedBar_2;
    public Text SpeedTexT_1, SpeedText_2, SpeedTimeText_1,SpeedTimeText_2, SpeedMaxText_1, SpeedMaxText_2;

    public Image MissleBar_1, MissleBar_2;
    public Image ShieldBar_1, ShieldBar_2;
    public Image NitrosBar_1, NitrosBar_2;

}
