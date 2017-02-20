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

}
