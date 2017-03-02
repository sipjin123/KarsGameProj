using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TronGameManager : GameStatsTweaker {

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
    

    #region TWEAKABLE VARIABLES


    public void UpdateTexts()
    {
        Text_MovementSpeed.text = MovementSpeed.ToString("F1");
        Text_rotationSpeed.text = rotationSpeed.ToString("F1");

        Text_accelerationSpeedMax.text = accelerationSpeedMax.ToString("F1");
        Text_accelerationTimerMax.text = accelerationTimerMax.ToString("F1");

        Text_trailDistanceTotal.text = trailDistanceTotal.ToString("F1");

        Text_const_StunDuration.text = const_StunDuration.ToString("F1");
        Text_BlindDuration.text = BlindDuration.ToString("F1");
        Text_ConfuseDuration.text = ConfuseDuration.ToString("F1");

        Text_missleCooldown.text = missleCooldown.ToString("F1");
        Text_shieldCooldown.text = shieldCooldown.ToString("F1");
        Text_nitroCooldown.text = nitroCooldown.ToString("F1");
        Text_nitroSpeed.text = nitroSpeed.ToString("F1");
        Text_nitroDuration.text = nitroDuration.ToString("F1");


        IncrementText.text = IncrementValue.ToString("F1");
        DivisibleTrailText.text = DivisibleTrailValue.ToString("F1");

        BaseText_Speed.text = Base_Value_Speed.ToString("F1");
        BaseText_Acceleration.text = Base_Value_Acceleration.ToString("F1");
        BaseText_Rotation.text = Base_Value_Rotation.ToString("F1");
        BaseText_Trail.text = Base_Value_Trail.ToString("F1");

        IncrementText_Speed.text = Increment_Value_Speed.ToString("F1");
        IncrementText_Acceleration.text = Increment_Value_Acceleration.ToString("F1");
        IncrementText_Rotation.text = Increment_Value_Rotation.ToString("F1");
        IncrementText_Trail.text = Increment_Value_Trail.ToString("F1");


        Force_Text.text = Force_Value.ToString("F1");
        Torque_Text.text = Torque_Value.ToString("F1");
        Drag_Text.text = Drag_Value.ToString("F1");
        AngularDrag_Text.text = AngularDrag_Value.ToString("F1");
        Mass_Text.text = Mass_Value.ToString("F1");


        PlayerPrefs.SetFloat(PrefKey_Movement, MovementSpeed);
        PlayerPrefs.SetFloat(PrefKey_Rotation, rotationSpeed);
        PlayerPrefs.SetFloat(PrefKey_AccelerationSpeedMax, accelerationSpeedMax);
        PlayerPrefs.SetFloat(PrefKey_AccelerationTimerMax, accelerationTimerMax);
        
        PlayerPrefs.SetFloat(PrefKey_TrailTotal, trailDistanceTotal);

        PlayerPrefs.SetFloat(PrefKey_Stun, const_StunDuration);
        PlayerPrefs.SetFloat(PrefKey_Blind, BlindDuration);
        PlayerPrefs.SetFloat(PrefKey_Confuse, ConfuseDuration);

        PlayerPrefs.SetFloat(PrefKey_MissleCooldown, missleCooldown);
        PlayerPrefs.SetFloat(PrefKey_ShieldCooldown, shieldCooldown);
        PlayerPrefs.SetFloat(PrefKey_NitroCooldown, nitroCooldown);
        PlayerPrefs.SetFloat(PrefKey_NitroSpeed, nitroSpeed);
        PlayerPrefs.SetFloat(PrefKey_NitroDuration, nitroDuration);

        Speed_Text.text = Speed_Stat.ToString();
        Acceleration_Text.text = Acceleration_Stat.ToString();
        Rotation_Text.text = Rotation_Stat.ToString();
        Trail_Text.text = Trail_Stat.ToString();
    }
    
    public GameObject _testPanel;
    public void FlipTestPanel()
    {
        _testPanel.SetActive(!_testPanel.activeInHierarchy);
    }
    #endregion
    
    #endregion
    //==================================================================================================================================
    #region INITALIZATION
    void Awake()
    {
        Initer();
    }
    public override void Initer()
    {
        base.Initer();



        _instance = this;

        TweakMoveSpeed(0);
        TweakrotationSpeed(0);
        
        TweaktrailDistanceTotal(0);

        Tweakconst_StunDuration(0);
        Tweakconst_BlindDuration(0);
        Tweakconst_ConfuseDuration(0);

        Tweak_missleCooldown(0);
        Tweak_shieldCooldown(0);

        Tweak_nitroCooldown(0);
        Tweak_nitroSpeed(0);
        Tweak_nitroDuration(0);

        Tweak_accelerationSpeedMax(0);
        Tweak_accelerationTimerMax(0);

        Add_Stat_Speed(1);
        Add_Stat_Acceleration(1);
        Add_Stat_Rotation(1);
        Add_Stat_Trail(1);




        UpdateTexts();
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
        }
        OpenCharacterSelect(true);
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
    public void ReturnStatsToDefault()
    {
        MovementSpeed = (DefaultMovement);
        rotationSpeed =  (DefaultRotation);

        trailDistanceTotal = (DefaulttrailDistanceTotal);

        const_StunDuration = (DefaultStun);
        BlindDuration = (DefaultBlind);
        ConfuseDuration = (DefaultConfuse);

        missleCooldown = (DefaultMissleCooldown);
        shieldCooldown = (DefaultShieldCooldown);

        nitroCooldown = (DefaultNitroCooldown);
        nitroSpeed = (DefaultNitroSpeed);
        nitroDuration = (DefaultNitroDuration);



        TweakMoveSpeed(0);
        TweakrotationSpeed(0);
        
        TweaktrailDistanceTotal(0);

        Tweakconst_StunDuration(0);
        Tweakconst_BlindDuration(0);
        Tweakconst_ConfuseDuration(0);

        Tweak_missleCooldown(0);
        Tweak_shieldCooldown(0);

        Tweak_nitroCooldown(0);
        Tweak_nitroSpeed(0);
        Tweak_nitroDuration(0);

        Tweak_accelerationSpeedMax(0);
        Tweak_accelerationTimerMax(0);
    }
    public void StartGame()
    {
        /*
        switch (carMeshIndex)
        {
            case 0:
                MovementSpeed = 3;
                rotationSpeed = 55;
                const_StunDuration = 5;
                break;
            case 1:
                MovementSpeed = 6;
                rotationSpeed = 55;
                const_StunDuration = 5;
                break;
            case 2:
                MovementSpeed = 3;
                rotationSpeed = 75;
                const_StunDuration = 5;
                break;
        }*/

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

        if (NetworkStart)
        {
            ReadyPlayer(GameSparkPacketReceiver.Instance.PeerID);
        }
        else
        {

        }
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

        PlayerObjects[_player - 1].GetComponent<Car_Movement>().SetReady(true);
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, _player);
            data.SetInt(2, 1);
            data.SetInt(3, (int)NetworkPlayerStatus.SET_READY);
            GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }


        PlayerObjects[_player-1].GetComponent<Car_DataReceiver>().SetCarAvatar(carMeshIndex);
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, _player);
            data.SetInt(2, carMeshIndex);
            GetRTSession.SendData(114, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }


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
    }
    #endregion
    //==================================================================================================================================








    //--------------------------------------------------------------------------------------
    #region GAME STATS TWEAKER
    public void Add_Stat_Speed(int _stat)
    {
        if (_stat < 0)
            Speed_Stat -= IncrementValue;
        else
            Speed_Stat += IncrementValue;

        accelerationSpeedMax = Base_Value_Speed + ( (Speed_Stat-1) * Increment_Value_Speed);
    }
    public void Add_Stat_Acceleration(int _stat)
    {
        if (_stat < 0)
            Acceleration_Stat -= IncrementValue;
        else
            Acceleration_Stat += IncrementValue;

        accelerationTimerMax = Base_Value_Acceleration + ( (Acceleration_Stat-1) * Increment_Value_Acceleration);
    }
    public void Add_Stat_Rotation(int _stat)
    {
        if (_stat < 0)
            Rotation_Stat -= IncrementValue;
        else
            Rotation_Stat += IncrementValue;

        rotationSpeed = Base_Value_Rotation + ((Rotation_Stat - 1) * Increment_Value_Rotation);
    }
    public void Add_Stat_Trail(int _stat)
    {
        if (_stat < 0)
            Trail_Stat -= IncrementValue;
        else
            Trail_Stat += IncrementValue;

        trailDistanceTotal = Base_Value_Trail+ ((Trail_Stat- 1) * Increment_Value_Trail);

        SendTrailData();
    }
    #endregion
    
    //==================================================================================================================================
    #region TEST INPUTS G,K,L
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine("delaydeath");
        }
    }
    #endregion
    //==================================================================================================================================
}
