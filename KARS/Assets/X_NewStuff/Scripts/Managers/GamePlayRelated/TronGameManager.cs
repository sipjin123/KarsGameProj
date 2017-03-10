using GameSparks.RT;
<<<<<<< HEAD
using System;
=======
>>>>>>> 3e3b2bc (Sorting files)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TronGameManager : GameStatsTweaker {



    private static TronGameManager _instance;
    public static TronGameManager Instance { get { return _instance; } }

    public Transform[] spawnPlayerPosition;

<<<<<<< HEAD

=======
    public GameObject GameSparksObject;
    public GameObject CurrentGameSparksObject;

    public GameObject[] SelectedCarHighlights;
>>>>>>> 3e3b2bc (Sorting files)

    private bool blockMatchFinding;
    public bool BlockMatchFinding
    {
        get { return blockMatchFinding; }
        set { blockMatchFinding = value; }
    }

<<<<<<< HEAD
=======
    #region SKILLS RELATED
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
        "Expand",
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

            temp.GetComponent<Image>().sprite = UIManager.Instance.SkillIcons[i].GetComponent<Image>().sprite;
            temp.SetActive(true);

            temp.transform.GetChild(0).GetComponent<Text>().text = skillStringList[i];
            temp.GetComponent<Button>().onClick.AddListener(() => {
                SelectThisSkill(temp);
            });
        }
        SelectThisSkill(SkillButtonParent.GetChild(0).gameObject);
        SelectSkillSlot(1);
        SelectThisSkill(SkillButtonParent.GetChild(2).gameObject);
        SelectSkillSlot(0);
    }

    public int SelectedSkin;
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
    #endregion

    

>>>>>>> 3e3b2bc (Sorting files)
    //==================================================================================================================================
    #region VARIABLES
    public bool NetworkStart;
    public GameObject singlePlayerUI;
<<<<<<< HEAD


    //CHARACTER SELECT
    private int selectedSkin;
    [SerializeField]
    private GameObject[] SelectedCarHighlights;
    public int GetSelectedSkin()
    {
        return selectedSkin;
    }
    int carMeshIndex;
    [SerializeField]
    private GameObject[] carMeshList;
    [SerializeField]
    private GameObject[] StatList;

    //SKILLS
    public int SkillListCount = 9;
    int currentSlotIndex;
    [SerializeField]
    private GameObject[] slotIndicator;
    [SerializeField]
    private GameObject[] skillSlotIndicator;
    [SerializeField]
    private Image[] selected_currentSkill_Image;
    [SerializeField]
    private GameObject SkillPanel;
    [SerializeField]
    private Transform SkillButtonParent;
    [SerializeField]
    private GameObject SkillButton;
    public Text[] selected_currentSkill_Text;

    //LOADING SCREEN
    float progressValueHolder;
    float currentProgressValue;
    bool progressValueSwitch;
    float progressTimer;
    bool serverSecures;
    #endregion
    //==================================================================================================================================
    //INITIALIZATION
    #region INITALIZATION
    void Awake()
    {
        selectedSkin = 1;
        _instance = this;
        Initer();
        StartProgressSession();
=======
    
    int carMeshIndex;
    public GameObject[] carMeshList;
    public GameObject[] StatList;
    
    

    
    #endregion
    //==================================================================================================================================
    #region INITALIZATION
    void Awake()
    {
        _instance = this;
        Initer();
>>>>>>> 3e3b2bc (Sorting files)
    }
    public override void Initer()
    {
        base.Initer();
    }
    void Start()
    {
        SelectedSkin = 1;
        InitSkillList();

        for (int i = 0; i < PlayerObjects.Length; i++)
        {
            PlayerObjects[i].GetComponent<Car_DataReceiver>().ClearBufferState();
            PlayerObjects[i].SetActive(true);
            PlayerObjects[i].GetComponent<Car_Movement>().CarRotationObject.eulerAngles = Vector3.zero;
        }
<<<<<<< HEAD
=======
        UIManager.Instance.SetRespawnScreen(true);
>>>>>>> 3e3b2bc (Sorting files)

    }
    #endregion
    //==================================================================================================================================
<<<<<<< HEAD
    #region CHARACTER SELECT
=======
    #region PLAYER DEATH AND HP REDUCTION
    public void ReduceHPOfPlayer(int player, float life)
    {
        if (!NetworkStart)
        {
            UIManager.Instance.AdjustHPBarAndText(player, life);
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
    public void OnClick_SelectCarButton()
    {
        SelectedSkin = carMeshIndex;
        Access_UpdateCarSelection();
    }
>>>>>>> 3e3b2bc (Sorting files)
    public void OnClick_SelectThisCarFrame(int _val)
    {
        carMeshIndex = _val;
        Access_UpdateCarSelection();
    }
<<<<<<< HEAD
    public void OnClick_SelectCarButton()
    {
        selectedSkin = carMeshIndex;
        Access_UpdateCarSelection();
    }
=======
>>>>>>> 3e3b2bc (Sorting files)
    public void NextCar()
    {
        carMeshIndex++;
        Access_UpdateCarSelection();
    }
    public void PreviousCar()
    {
        carMeshIndex--;
        Access_UpdateCarSelection();
    }
<<<<<<< HEAD

=======
>>>>>>> 3e3b2bc (Sorting files)
    public void Access_UpdateCarSelection()
    {
        if (carMeshIndex <= -1)
        {
            carMeshIndex = carMeshList.Length - 1;
        }
        if (carMeshIndex >= carMeshList.Length)
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

        for (int i = 0; i < SelectedCarHighlights.Length; i++)
            SelectedCarHighlights[i].SetActive(false);
<<<<<<< HEAD
        SelectedCarHighlights[selectedSkin].SetActive(true);

        if(carMeshIndex == selectedSkin)
=======
        SelectedCarHighlights[SelectedSkin].SetActive(true);

        if(carMeshIndex == SelectedSkin)
>>>>>>> 3e3b2bc (Sorting files)
        {
            UIManager.Instance.ActivateSelectCarButton(false);
        }
        else
        {
            UIManager.Instance.ActivateSelectCarButton(true);
        }
    }
    #endregion
    //==================================================================================================================================
<<<<<<< HEAD
    //MATCH FIND INITIALIZATION
=======

>>>>>>> 3e3b2bc (Sorting files)
    #region PLAYER START SYNC
    public void ReceiveSignalToStartGame()
    {
        NetworkStart = true;
<<<<<<< HEAD
        if (GameSparkPacketHandler.Instance.GetPeerID() == 0)
=======

        if (GameSparkPacketReceiver.Instance.PeerID == 0)
>>>>>>> 3e3b2bc (Sorting files)
        {
            StopCoroutine("RetryToSTart");
            StartCoroutine("RetryToSTart");
            return;
        }
<<<<<<< HEAD
        SendStartSignalConcent();
=======
        ReadyPlayer(GameSparkPacketReceiver.Instance.PeerID);
>>>>>>> 3e3b2bc (Sorting files)

        StateManager.Instance.Access_ChangeState(MENUSTATE.START_GAME);
    }
    IEnumerator RetryToSTart()
    {
        yield return new WaitForSeconds(2);
        ReceiveSignalToStartGame();
    }
<<<<<<< HEAD
    private void SendStartSignalConcent()
    {
        PlayerObjects[GameSparkPacketHandler.Instance.GetPeerID() - 1].GetComponent<Car_Movement>().SetReady(true);
        GameSparkPacketHandler.Instance.Access_SentReadyToServer();
    }
    #endregion
    //==================================================================================================================================
    //SKILLS FUNCTION
    #region SKILLS FUNCTION
    public void InitSkillList()
    {
        for (int i = 0; i < SkillListCount; i++)
        {
            GameObject temp = Instantiate(SkillButton, transform.position, Quaternion.identity) as GameObject;
            temp.transform.SetParent(SkillButtonParent);
            temp.transform.localScale = Vector3.one;

            temp.GetComponent<Image>().sprite = UIManager.Instance.SkillIcons[i].GetComponent<Image>().sprite;
            temp.SetActive(true);

            temp.transform.GetChild(0).GetComponent<Text>().text = ((SKILL_LIST)i).ToString();
            temp.GetComponent<Button>().onClick.AddListener(() => {
                SelectThisSkill(temp);
            });
        }
        SelectThisSkill(SkillButtonParent.GetChild(0).gameObject);
        SelectSkillSlot(1);
        SelectThisSkill(SkillButtonParent.GetChild(2).gameObject);
        SelectSkillSlot(0);
    }
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
    public void ToggleSkillSelection()
    {
        SkillPanel.SetActive(!SkillPanel.activeInHierarchy);
    }
    #endregion
    //==================================================================================================================================
    //LOADING SCREEN
    #region LOADING SCREEN
    public void SetProgressValueHolder(float _val)
    {

        UIManager.Instance.GameUpdateText.text += "\n\t___ADDED: "+_val+" %";
        progressValueHolder += _val;
    }
    public void StartProgressSession()
    {
        progressTimer = 0;
        progressValueHolder = 0;
        currentProgressValue = 0;
        UIManager.Instance.SetProgressText("");
        progressValueSwitch = true;
        PlayerObjects[0].GetComponent<Car_Movement>().DisableWheels = true;
        PlayerObjects[1].GetComponent<Car_Movement>().DisableWheels = true;
    }
    void Update()
    {
        if(progressValueSwitch)
        {
            progressTimer += Time.deltaTime;
            UIManager.Instance.SetProgressTimerText(((int)progressTimer).ToString());
            if (progressValueHolder >= -.1f)
            {
                progressValueHolder -= Time.deltaTime * 20;
                currentProgressValue += Time.deltaTime * 20;
                if (currentProgressValue >= 99)
                {
                    serverSecures = true;
                    currentProgressValue = 100;
                    UIManager.Instance.SetProgressText(((int)currentProgressValue).ToString());
                    progressValueSwitch = false;
                    UIManager.Instance.Set_Canvas_Waiting(false);
                    UIManager.Instance.Set_Canvas_Countdown(true);
                }
                else
                {
                    serverSecures = false;
                }
            }
            UIManager.Instance.SetProgressText(((int)currentProgressValue).ToString());
        }
        else
        {
            if (serverSecures)
            {
                try
                {
                    DateTime tempDate = GameSparkPacketHandler.Instance.Get_gameShouldStartAt();
                    if (GameSparkPacketHandler.Instance.GetServerClock() > tempDate.AddSeconds(-3))
                    {
                        StopCoroutine("DelaySecTimer");
                        StartCoroutine("DelaySecTimer");
                        serverSecures = false;
                    }
                }
                catch(ArgumentException e)
                {
                    Debug.LogError("error checking CLOCK");
                    Debug.LogError(e.Message);
                }
            }
=======


    void ReadyPlayer(int _player)
    {
        StartCoroutine(DelayStartChecker(_player));
    }
    IEnumerator DelayStartChecker(int _player)
    {
        yield return new WaitForSeconds(3);
        GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();

        UIManager.Instance.GameUpdateText.text += "\nDelay Start Player: " + _player;
        //READY LOCAL PLAYER 
        if (_player == 0)
        {
            DelayStartChecker(_player);
            yield return null;
        }
        PlayerObjects[_player - 1].GetComponent<Car_Movement>().SetReady(true);
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, _player);
            data.SetInt(2, 1);
            data.SetInt(3, (int)NetworkPlayerStatus.SET_READY);
            GetRTSession.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
>>>>>>> 3e3b2bc (Sorting files)
        }
    }
    #endregion
    //==================================================================================================================================
<<<<<<< HEAD
    //IN GAME INITIALIZATION
    IEnumerator DelaySecTimer()
    {
        AudioManager.Instance.Play_Oneshot(AUDIO_CLIP.CAR_START);
        if (GameSparkPacketHandler.Instance.GetPeerID() == 2)
            StateButtonManager.Instance.OnClick_ResetGame();
        UIManager.Instance.Set_Canvas_Countdown(true);
        UIManager.Instance.SetCountdownTimerText("loading...");
        yield return new WaitForSeconds(2);
        UIManager.Instance.SetCountdownTimerText("3");

        yield return new WaitForSeconds(1);

        UIManager.Instance.SetCountdownTimerText("2");

        yield return new WaitForSeconds(1);

        UIManager.Instance.SetCountdownTimerText("1");

        yield return new WaitForSeconds(.5f);

        UIManager.Instance.SetCountdownTimerText("Go");

        yield return new WaitForSeconds(.5f);

        UIManager.Instance.GameUpdateText.text += "\nGAME START NOW!!";

        PowerUpManager.Instance.ResetMissles();
        UIManager.Instance.Set_Canvas_Countdown(false);
        UIManager.Instance.Set_Canvas_Waiting(false);

        if(UIManager.Instance.GetRespawnScreen())
        UIManager.Instance.Set_Canvas_GameInit(true);

        PlayerObjects[0].GetComponent<Car_Movement>().DisableWheels = false;
        PlayerObjects[1].GetComponent<Car_Movement>().DisableWheels = false;
    }
}
=======







    public void Global_SendState(MENUSTATE _state)
    {
        StateManager.Instance.Access_ChangeState(_state);

        using (RTData data = RTData.Get())
        {
            data.SetInt(1, 0);
            data.SetInt(2, (int)_state);
            GetRTSession.SendData(OPCODE_CLASS.MenuStateOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }


    #region PUBLIC FUNCTIONS
    public void Access_ReInitializeGameSparks()
    {
        Destroy(CurrentGameSparksObject);
        CurrentGameSparksObject = Instantiate(GameSparksObject, transform.position, Quaternion.identity);
    }

    public void Access_PlayerReset()
    {
        for (int i = 0; i < PlayerObjects.Length; i++)
        {
            PlayerObjects[i].GetComponent<Car_DataReceiver>().ClearBufferState();
            PlayerObjects[i].SetActive(true);
            PlayerObjects[i].GetComponent<Car_DataReceiver>().Network_ID = 0;
            PlayerObjects[i].GetComponent<Car_DataReceiver>().NetworkCam.enabled = false;
            PlayerObjects[i].GetComponent<Car_DataReceiver>().ifMy_Network_Player = false;

            PlayerObjects[i].GetComponent<Car_DataReceiver>().ResetPowerups();

            PlayerObjects[i].GetComponent<Car_Movement>().CarRotationObject.eulerAngles = Vector3.zero;
            PlayerObjects[i].transform.eulerAngles = Vector3.zero;
            PlayerObjects[i].transform.position = spawnPlayerPosition[i].position;


            PlayerObjects[i].GetComponent<Car_Movement>().SetStartGame(false);
            PlayerObjects[i].GetComponent<Car_Movement>().SetReady(false);
            PlayerObjects[i].GetComponent<Car_Movement>().enabled = false;
        }
    }
    #endregion
}
>>>>>>> 3e3b2bc (Sorting files)
