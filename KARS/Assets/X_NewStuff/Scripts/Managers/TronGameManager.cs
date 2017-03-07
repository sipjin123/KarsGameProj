using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TronGameManager : GameStatsTweaker {

    public GameObject InGame_PANEL, Debug_PANEL, Waiting_PANEL;
    NetworkPlayerStatus SkillSlot1, SkillSlot2;

    string[] skillStringList = new string[]
    {
        "Shield",
        "Stun",
        "Blind",
        "Confuse",
        "Slow",
        "Silence",
        "Fly",
        "Nitro",
        "Extend",
    };

    public Transform SkillButtonParent;
    public GameObject SkillButton;

    public void InitSkillList()
    {
        for (int i = 0; i < skillStringList.Length; i++)
        {
            GameObject temp = Instantiate(SkillButton, transform.position, Quaternion.identity) as GameObject;
            temp.transform.SetParent(SkillButtonParent);
            temp.transform.localScale = Vector3.one;

            temp.GetComponent<Image>().sprite = SpriteManager.Instance.SkillIcons[i].GetComponent<Image>().sprite;
            temp.SetActive(true);

            temp.transform.GetChild(0).GetComponent<Text>().text = skillStringList[i];
            temp.GetComponent<Button>().onClick.AddListener(() => {
                SelectThisSkill(temp);
            });
        }
        SelectThisSkill(SkillButtonParent.GetChild(0).gameObject);
        SelectSkillSlot(1);
        SelectThisSkill(SkillButtonParent.GetChild(1).gameObject);
        SelectSkillSlot(0);
    }

    int currentSlotIndex;
    public Image[] selected_currentSkill_Image;
    public Text[] selected_currentSkill_Text;

    public GameObject[] slotIndicator;
    public GameObject[] skillSlotIndicator;
    public void SelectSkillSlot(int _val)
    {
        currentSlotIndex = _val;

        skillSlotIndicator[0].SetActive(false);
        skillSlotIndicator[1].SetActive(false);

        skillSlotIndicator[_val].SetActive(true);
    }
    public void SelectThisSkill(GameObject _obj)
    {
        if (selected_currentSkill_Text[0].text == _obj.transform.GetChild(0).GetComponent<Text>().text || selected_currentSkill_Text[1].text == _obj.transform.GetChild(0).GetComponent<Text>().text)
            return;

        slotIndicator[currentSlotIndex].transform.position = _obj.transform.position;
        

        selected_currentSkill_Image[currentSlotIndex].sprite = _obj.GetComponent<Image>().sprite;
        selected_currentSkill_Text[currentSlotIndex].text = _obj.transform.GetChild(0).GetComponent<Text>().text;
    }

    public GameObject SkillPanel;
    public void ToggleSkillSelection()
    {
        SkillPanel.SetActive(!SkillPanel.activeInHierarchy);
    }









    private static TronGameManager _instance;
    public static TronGameManager Instance { get { return _instance; } }





    //==================================================================================================================================
    #region VARIABLES
    public bool NetworkStart;
    public GameObject[] NetworkCanvas;

    public GameObject DEbugUI;
    public GameObject singlePlayerUI;
    
    public GameObject CharacterSelectPanel;

    int carMeshIndex;
    public GameObject[] carMeshList;
    public GameObject[] StatList;
    
    public Image HealthBar1, HealthBar2;
    public Text Var_HP_1, Var_HP_2;
    

    
    #endregion
    //==================================================================================================================================
    #region INITALIZATION
    void Awake()
    {
        _instance = this;
        Initer();
    }
    public override void Initer()
    {
        base.Initer();
    }
    void Start()
    {
        InitSkillList();

        for (int i = 0; i < PlayerObjects.Length; i++)
        {
            PlayerObjects[i].GetComponent<Car_DataReceiver>().ClearBufferState();
            PlayerObjects[i].SetActive(true);
            PlayerObjects[i].GetComponent<Car_Movement>().CarRotationObject.eulerAngles = Vector3.zero;
        }
        UIManager.instance.SetRespawnScreen(true);

    }
    #endregion
    //==================================================================================================================================
    #region PLAYER DEATH AND HP REDUCTION
    public void ReduceHPOfPlayer(int player, float life)
    {
        if (!NetworkStart)
        {
            if (player == 1)
            {
                Var_HP_1.text = life.ToString();
                HealthBar1.fillAmount = life / 5;
            }
            else
            {
                Var_HP_2.text = life.ToString();
                HealthBar2.fillAmount = life / 5;
            }
            if (life <= 0)
            {
                StartCoroutine("delaydeath");
            }
        }
    }
    IEnumerator delaydeath()
    {
        yield return new WaitForSeconds(1);

        PlayerObjects[0].GetComponent<Car_Movement>().AIMode_HpBar = 6;
        PlayerObjects[1].GetComponent<AI_Behaviour>().AI_Health = 6;
        PlayerObjects[0].GetComponent<Car_Movement>().Die();
        PlayerObjects[1].GetComponent<AI_Behaviour>().DIE();
    }
    #endregion
    //==================================================================================================================================
    #region CHARACTER SELECT
    public void SetNetworkStart(bool _switch)
    {
        NetworkStart = _switch;
        if (_switch)//MULTIPLAYER
        {
            for (int i = 0; i < PlayerObjects.Length; i++)
                PlayerObjects[i].SetActive(true);
        }
        /*
        else if (!_switch)//SINGLE PLAYER
        {
            for (int i = 0; i < NetworkCanvas.Length; i++)
                NetworkCanvas[i].SetActive(false);
            PlayerObjects[0].GetComponent<Car_DataReceiver>().enabled = false;
            PlayerObjects[0].GetComponent<Car_Movement>().enabled = true;

            PlayerObjects[0].GetComponent<Car_Movement>().myCam.enabled = true;
            PlayerObjects[0].GetComponent<Car_Movement>().StartGame = true;
            PlayerObjects[0].GetComponent<Car_Movement>()._trailCollision.SetEmiision(true);


            PlayerObjects[1].GetComponent<AI_Behaviour>().enabled = true;
            PowerUpManager.Instance.StartNetwork();
            DEbugUI.gameObject.SetActive(true);
            singlePlayerUI.SetActive(true);
            //UIManager.instance.Player1Panel.SetActive(true);
        }*/
    }

    public void OpenCharacterSelect(bool _switch)
    {
        CharacterSelectPanel.SetActive(_switch);

    }
    public void NextCar()
    {
        carMeshIndex++;
        if (carMeshIndex > carMeshList.Length - 1)
        {
            carMeshIndex = 0;
        }
        for (int i = 0; i < carMeshList.Length; i++)
        {
            carMeshList[i].SetActive(false);
            StatList[i].SetActive(false);
        }
        carMeshList[carMeshIndex].SetActive(true);
        StatList[carMeshIndex].SetActive(true);
    }
    public void PreviousCar()
    {
        carMeshIndex--;
        if (carMeshIndex <= -1)
        {
            carMeshIndex = carMeshList.Length - 1;
        }
        for (int i = 0; i < carMeshList.Length; i++)
        {
            carMeshList[i].SetActive(false);
            StatList[i].SetActive(false);
        }
        carMeshList[carMeshIndex].SetActive(true);
        StatList[carMeshIndex].SetActive(true);
    }
    
    public void ReceiveSignalToStartGame()
    {

        TweakMoveSpeed(0);
        TweakrotationSpeed(0);

        TweaktrailDistanceTotal(0);

        Tweakconst_StunDuration(0);

        Tweak_missleCooldown(0);
        Tweak_shieldCooldown(0);

        Tweak_nitroCooldown(0);
        Tweak_nitroSpeed(0);
        Tweak_nitroDuration(0);

        Tweak_accelerationSpeedMax(0);
        Tweak_accelerationTimerMax(0);

        OpenCharacterSelect(false);

        NetworkStart = true;

        if (NetworkStart)
        {
            if (GameSparkPacketReceiver.Instance.PeerID == 0)
            {
                StartCoroutine(RetryToSTart());
                return;
            }
            ReadyPlayer(GameSparkPacketReceiver.Instance.PeerID);
         
            Transform skillParent;
            if (GameSparkPacketReceiver.Instance.PeerID == 1)
                skillParent = UIManager.instance.Player1_SkillsParent.transform;
            else
                skillParent = UIManager.instance.Player2_SkillsParent.transform;
            
            foreach (Transform T in skillParent)
            {
                if (T.gameObject.name == selected_currentSkill_Text[0].text || T.gameObject.name == selected_currentSkill_Text[1].text)
                {
                    T.gameObject.SetActive(true);
                }
            }
            Debug_PANEL.SetActive(true);
            InGame_PANEL.SetActive(true);
            Waiting_PANEL.SetActive(false);
            UIManager.instance.SetResultScreen(false);
        }
        else
        {

        }
    }
    IEnumerator RetryToSTart()
    {
        yield return new WaitForSeconds(2);
        ReceiveSignalToStartGame();
    }
    public void StartGame()
    {
        Waiting_PANEL.SetActive(true);
        RegisterGameSpark.Instance.LoginButton();

    }
    #endregion
    //==================================================================================================================================
    #region PLAYER START SYNC
    void ReadyPlayer(int _player)
    {
        StartCoroutine(DelayStartChecker(_player));
    }
    IEnumerator DelayStartChecker(int _player)
    {

        yield return new WaitForSeconds(3);
        GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();

        //READY LOCAL PLAYER 
        PlayerObjects[_player - 1].GetComponent<Car_Movement>().SetReady(true);
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, _player);
            data.SetInt(2, 1);
            data.SetInt(3, (int)NetworkPlayerStatus.SET_READY);
            GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }

        //SET LOCAL CAR MESH
        PlayerObjects[_player-1].GetComponent<Car_DataReceiver>().SetCarAvatar(carMeshIndex);
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, _player);
            data.SetInt(2, carMeshIndex);
            GetRTSession.SendData(114, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }

        //2 PLAYERS READY
        if (PlayerObjects[0].GetComponent<Car_Movement>().isREady && PlayerObjects[1].GetComponent<Car_Movement>().isREady)
        {
            for (int i = 0; i < PlayerObjects.Length; i++)
            {

                PlayerObjects[i].GetComponent<Car_Movement>().SetStartGame(true);
                using (RTData data = RTData.Get())
                {
                    data.SetInt(1, i+1);
                    data.SetInt(2, 1);
                    data.SetInt(3, (int)NetworkPlayerStatus.SET_START);
                    GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                }
            }
        }
        else
        {
            StartCoroutine(DelayRetryReadyPlayers(_player));
        }
    }
    IEnumerator DelayRetryReadyPlayers(int val)
    {
        yield return new WaitForSeconds(2);
        //2 PLAYERS READY
        if (PlayerObjects[0].GetComponent<Car_Movement>().isREady && PlayerObjects[1].GetComponent<Car_Movement>().isREady)
        {
            for (int i = 0; i < PlayerObjects.Length; i++)
            {

                PlayerObjects[i].GetComponent<Car_Movement>().SetStartGame(true);
                using (RTData data = RTData.Get())
                {
                    data.SetInt(1, i + 1);
                    data.SetInt(2, 1);
                    data.SetInt(3, (int)NetworkPlayerStatus.SET_START);
                    GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                }
            }
        }
        else
        {
            StartCoroutine(DelayRetryReadyPlayers(val));
        }
    }
    #endregion
    //==================================================================================================================================


    public GameObject GameSparksObject;
    public GameObject CurrentGameSparksObject;
    public void ResetGameToMenu()
    {
        UIManager.instance.SetRespawnScreen(false);
        Destroy(CurrentGameSparksObject);
        CurrentGameSparksObject = Instantiate(GameSparksObject, transform.position, Quaternion.identity);

        UIManager.instance.GameUpdateText.text += "\nRESETING MENU PANELS";
        InGame_PANEL.SetActive(false);
        Debug_PANEL.SetActive(false);
        CharacterSelectPanel.SetActive(true);
        PlayerObjects[0].GetComponent<Car_DataReceiver>().ifMy_Network_Player = false;
        PlayerObjects[1].GetComponent<Car_DataReceiver>().ifMy_Network_Player = false;
        PlayerObjects[0].GetComponent<Car_DataReceiver>().Network_ID = 0;
        PlayerObjects[1].GetComponent<Car_DataReceiver>().Network_ID = 0;

        PlayerObjects[0].GetComponent<Car_Movement>().SetReady(false);
        PlayerObjects[1].GetComponent<Car_Movement>().SetReady(false);

        PlayerObjects[0].GetComponent<Car_Movement>().SetStartGame(false);
        PlayerObjects[1].GetComponent<Car_Movement>().SetStartGame(false);
        

        UIManager.instance.Player1Panel.SetActive(false);
        UIManager.instance.Player2Panel.SetActive(false);

        foreach(Transform t in UIManager.instance.Player1_SkillsParent.transform)
        {
            t.gameObject.SetActive(false);
        }
        foreach (Transform t in UIManager.instance.Player2_SkillsParent.transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    public Transform[] spawnPlayerPosition;







    
}
