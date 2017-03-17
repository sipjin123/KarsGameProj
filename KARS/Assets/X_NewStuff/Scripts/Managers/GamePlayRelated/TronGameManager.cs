using GameSparks.RT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TronGameManager : GameStatsTweaker {



    private static TronGameManager _instance;
    public static TronGameManager Instance { get { return _instance; } }

    public Transform[] spawnPlayerPosition;



    private bool blockMatchFinding;
    public bool BlockMatchFinding
    {
        get { return blockMatchFinding; }
        set { blockMatchFinding = value; }
    }

    //==================================================================================================================================
    #region VARIABLES
    public bool NetworkStart;
    public GameObject singlePlayerUI;


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
        selectedSkin = carMeshIndex;
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
        SelectedCarHighlights[selectedSkin].SetActive(true);

        if(carMeshIndex == selectedSkin)
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
    //MATCH FIND INITIALIZATION
    #region PLAYER START SYNC
    public void ReceiveSignalToStartGame()
    {
        NetworkStart = true;
        if (GameSparkPacketHandler.Instance.GetPeerID() == 0)
        {
            StopCoroutine("RetryToSTart");
            StartCoroutine("RetryToSTart");
            return;
        }
        SendStartSignalConcent();

        StateManager.Instance.Access_ChangeState(MENUSTATE.START_GAME);
    }
    IEnumerator RetryToSTart()
    {
        yield return new WaitForSeconds(2);
        ReceiveSignalToStartGame();
    }
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
        }
    }
    #endregion
    //==================================================================================================================================
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

        UIManager.Instance.Set_Canvas_Countdown(false);
        UIManager.Instance.Set_Canvas_Waiting(false);
        if(UIManager.Instance.GetRespawnScreen())
        UIManager.Instance.Set_Canvas_GameInit(true);
        PlayerObjects[0].GetComponent<Car_Movement>().DisableWheels = false;
        PlayerObjects[1].GetComponent<Car_Movement>().DisableWheels = false;
    }
}