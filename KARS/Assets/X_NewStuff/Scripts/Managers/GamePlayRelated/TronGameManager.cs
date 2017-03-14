using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TronGameManager : GameStatsTweaker {



    private static TronGameManager _instance;
    public static TronGameManager Instance { get { return _instance; } }

    public Transform[] spawnPlayerPosition;

    public GameObject GameSparksObject;
    public GameObject CurrentGameSparksObject;

    public GameObject[] SelectedCarHighlights;

    private bool blockMatchFinding;
    public bool BlockMatchFinding
    {
        get { return blockMatchFinding; }
        set { blockMatchFinding = value; }
    }

    #region SKILLS RELATED

    public int SkillListCount = 9;

    public Transform SkillButtonParent;
    public GameObject SkillButton;

    public int SelectedSkin;
    int currentSlotIndex;
    public Image[] selected_currentSkill_Image;
    public Text[] selected_currentSkill_Text;
    public GameObject SkillPanel;

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

    public void ToggleSkillSelection()
    {
        SkillPanel.SetActive(!SkillPanel.activeInHierarchy);
    }
    #endregion

    

    //==================================================================================================================================
    #region VARIABLES
    public bool NetworkStart;
    public GameObject singlePlayerUI;
    
    int carMeshIndex;
    public GameObject[] carMeshList;
    public GameObject[] StatList;
    
    

    
    #endregion
    //==================================================================================================================================
    #region INITALIZATION
    void Awake()
    {
        SelectedSkin = 1;
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
        UIManager.Instance.SetRespawnScreen(true);

    }
    #endregion
    //==================================================================================================================================
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
    public void OnClick_SelectThisCarFrame(int _val)
    {
        carMeshIndex = _val;
        Access_UpdateCarSelection();
    }
    public void OnClick_SelectCarButton()
    {
        SelectedSkin = carMeshIndex;
        Access_UpdateCarSelection();
    }
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
        SelectedCarHighlights[SelectedSkin].SetActive(true);

        if(carMeshIndex == SelectedSkin)
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

    #region PLAYER START SYNC
    public void ReceiveSignalToStartGame()
    {
        NetworkStart = true;
        if (GameSparkPacketReceiver.Instance.PeerID == 0)
        {
            StopCoroutine("RetryToSTart");
            StartCoroutine("RetryToSTart");
            return;
        }
        StartCoroutine("DelayStartChecker");

        StateManager.Instance.Access_ChangeState(MENUSTATE.START_GAME);
    }
    IEnumerator RetryToSTart()
    {
        yield return new WaitForSeconds(2);
        ReceiveSignalToStartGame();
    }
    
    IEnumerator DelayStartChecker()
    {
        yield return new WaitForSeconds(3);
        SendStartSignalConcent();
    }
    private void SendStartSignalConcent()
    {
        PlayerObjects[GameSparkPacketReceiver.Instance.PeerID - 1].GetComponent<Car_Movement>().SetReady(true);
        GameSparkPacketReceiver.Instance.Access_SentReadyToServer();

        return;
        UIManager.Instance.GameUpdateText.text += "\n\tDelay Ready Player: " + GameSparkPacketReceiver.Instance.PeerID;

        GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
        PlayerObjects[GameSparkPacketReceiver.Instance.PeerID - 1].GetComponent<Car_Movement>().SetReady(true);
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, GameSparkPacketReceiver.Instance.PeerID);
            data.SetInt(2, 1);
            data.SetInt(3, (int)NetworkPlayerStatus.SET_READY);
            GetRTSession.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
        UIManager.Instance.GameUpdateText.text += "\nSignal Sent To Server :: Ready Player: " + GameSparkPacketReceiver.Instance.PeerID;

    }
    #endregion
    //==================================================================================================================================

    float progressValueHolder;
    float currentProgressValue;
    bool progressValueSwitch;
    float progressTimer;
    public void SetProgressValueHolder(float _val)
    {
        progressValueHolder += _val;
    }
    public void StartProgressSession()
    {
        progressTimer = 0;
        progressValueHolder = 0;
        currentProgressValue = 0;
        UIManager.Instance.SetProgressText("");
        progressValueSwitch = true;
    }
    void Update()
    {
        if(progressValueSwitch)
        {
            progressTimer += Time.deltaTime;
            UIManager.Instance.SetProgressTimerText(((int)progressTimer).ToString());
            if (progressValueHolder > 0)
            {
                progressValueHolder -= Time.deltaTime * 10;
                currentProgressValue += Time.deltaTime * 10;
                if (currentProgressValue >= 70 && currentProgressValue < 70.5)
                {
                    currentProgressValue = 71;
                    SetProgressValueHolder(29);
                }
                if (currentProgressValue >= 99)
                {
                    currentProgressValue = 100;
                    UIManager.Instance.SetProgressText(((int)currentProgressValue).ToString());
                    progressValueSwitch = false;
                    UIManager.Instance.Set_Canvas_Waiting(false);
                    UIManager.Instance.Set_Canvas_Countdown(true);
                    StopCoroutine("CountDownTimer");
                    StartCoroutine("CountDownTimer");
                }
            }
            UIManager.Instance.SetProgressText(((int)currentProgressValue).ToString());
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        GameSparkPacketReceiver.Instance.Access_SentReadyToServer();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            GameSparkPacketReceiver.Instance.Access_SentStartToServer();
    }
    IEnumerator CountDownTimer()
    {
        UIManager.Instance.SetCountdownTimerText("3");
        yield return new WaitForSeconds(1);
        UIManager.Instance.SetCountdownTimerText("2");
        yield return new WaitForSeconds(1);
        if(GameSparkPacketReceiver.Instance.PeerID == 1)
        StateButtonManager.Instance.OnClick_ResetGame();
        UIManager.Instance.SetCountdownTimerText("1");
        yield return new WaitForSeconds(1);
        UIManager.Instance.SetCountdownTimerText("Go");
        yield return new WaitForSeconds(.5f);
        UIManager.Instance.SetRespawnScreen(true);
        UIManager.Instance.Set_Canvas_Countdown(false);
    }
    

    #region PUBLIC FUNCTIONS
    public void Global_SendState(MENUSTATE _state)
    {
        UIManager.Instance.GameUpdateText.text += "\n\tSuppose To Do This State: "+_state;
        StateManager.Instance.Access_ChangeState(_state);

        GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, 0);
            data.SetInt(2, (int)_state);
            GetRTSession.SendData(OPCODE_CLASS.MenuStateOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
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