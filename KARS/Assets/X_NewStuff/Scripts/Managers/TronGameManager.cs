using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TronGameManager : MonoBehaviour {

    private static TronGameManager _instance;
    public static TronGameManager Instance { get { return _instance; } }
    //==================================================================================================================================
    #region VARIABLES
    public bool NetworkStart;
    public GameObject[] PlayerObjects;
    public GameObject[] NetworkCanvas;

    public GameObject DEbugUI;
    public GameObject singlePlayerUI;
    
    public GameObject CharacterSelectPanel;

    int carMeshIndex;
    public GameObject[] carMeshList;
    public GameObject[] StatList;
    
    public Image HealthBar1, HealthBar2;
    public Text Var_HP_1, Var_HP_2;
    
    GameSparksRTUnity GetRTSession;

    #region TWEAKABLE VARIABLES
    #region MOVEMENT
    public Text Text_MovementSpeed;
    public float MovementSpeed;

    public Text Text_rotationSpeed;
    public float rotationSpeed;

    public Text Text_accelerationSpeedMax;
    public float accelerationSpeedMax;
    
    public Text Text_accelerationTimerMax;
    public float accelerationTimerMax;


    public void TweakMoveSpeed(float _var)
    {
        MovementSpeed += _var;
    }
    public void TweakrotationSpeed(float _var)
    {
        rotationSpeed += _var;
    }

    public void Tweak_accelerationSpeedMax(float _var)
    {
        accelerationSpeedMax += _var;
    }
    public void Tweak_accelerationTimerMax(float _var)
    {
        accelerationTimerMax += _var;
    }
    #endregion

    #region TRAIL
    public Text Text_trailDistanceTotal;
    public float trailDistanceTotal;

    public Text Text_trailDistanceChild;
    public float trailDistanceChild;

    public void TweaktrailDistanceTotal(float _var)
    {
        trailDistanceTotal += _var;
    }

    public void Tweakconst_trailDistanceChild(float _var)
    {
        trailDistanceChild += _var;
    }
    #endregion

    #region DISABLES 
    public Text Text_const_StunDuration;
    public float const_StunDuration;

    public Text Text_BlindDuration;
    public float BlindDuration;
    
    public Text Text_ConfuseDuration;
    public float ConfuseDuration;

    public void Tweakconst_StunDuration(float _var)
    {
        const_StunDuration += _var;
    }

    public void Tweakconst_BlindDuration(float _var)
    {
        BlindDuration += _var;
    }

    public void Tweakconst_ConfuseDuration(float _var)
    {
        ConfuseDuration += _var;
    }
    #endregion

    #region POWERUPS
    public Text Text_missleCooldown;
    public float missleCooldown;

    public Text Text_shieldCooldown;
    public float shieldCooldown;
    
    public Text Text_nitroCooldown;
    public float nitroCooldown;

    public Text Text_nitroSpeed;
    public float nitroSpeed;

    public Text Text_nitroDuration;
    public float nitroDuration;

    public void Tweak_missleCooldown(float _var)
    {
        missleCooldown += _var;
    }

    public void Tweak_shieldCooldown(float _var)
    {
        shieldCooldown += _var;
    }

    public void Tweak_nitroCooldown(float _var)
    {
        nitroCooldown += _var;
    }

    public void Tweak_nitroSpeed(float _var)
    {
        nitroSpeed += _var;
    }

    public void Tweak_nitroDuration(float _var)
    {
        nitroDuration += _var;
    }
    #endregion

    public void UpdateTexts()
    {
        Text_MovementSpeed.text = MovementSpeed.ToString("F1");
        Text_rotationSpeed.text = rotationSpeed.ToString("F1");

        Text_accelerationSpeedMax.text = accelerationSpeedMax.ToString("F1");
        Text_accelerationTimerMax.text = accelerationTimerMax.ToString("F1");

        Text_trailDistanceTotal.text = trailDistanceTotal.ToString("F1");
        Text_trailDistanceChild.text = trailDistanceChild.ToString("F1");

        Text_const_StunDuration.text = const_StunDuration.ToString("F1");
        Text_BlindDuration.text = BlindDuration.ToString("F1");
        Text_ConfuseDuration.text = ConfuseDuration.ToString("F1");

        Text_missleCooldown.text = missleCooldown.ToString("F1");
        Text_shieldCooldown.text = shieldCooldown.ToString("F1");
        Text_nitroCooldown.text = nitroCooldown.ToString("F1");
        Text_nitroSpeed.text = nitroSpeed.ToString("F1");
        Text_nitroDuration.text = nitroDuration.ToString("F1");

        PlayerPrefs.SetFloat(PrefKey_Movement, MovementSpeed);
        PlayerPrefs.SetFloat(PrefKey_Rotation, rotationSpeed);
        PlayerPrefs.SetFloat(PrefKey_AccelerationSpeedMax, accelerationSpeedMax);
        PlayerPrefs.SetFloat(PrefKey_AccelerationTimerMax, accelerationTimerMax);
        
        PlayerPrefs.SetFloat(PrefKey_TrailTotal, trailDistanceTotal);
        PlayerPrefs.SetFloat(PrefKey_TrailCap, trailDistanceChild);

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
    
    #region DEFAULT VALUES
    private static float DefaultMovement = 1;
    private static float DefaultRotation= 40;

    private static float DefaulttrailDistanceTotal = 5;
    private static float DefaulttrailDistanceChild = DefaulttrailDistanceTotal/3;

    private static float DefaultStun = 5;
    private static float DefaultBlind = 5;
    private static float DefaultConfuse = 5;

    private static float DefaultMissleCooldown = 5;
    private static float DefaultShieldCooldown = 5;

    private static float DefaultNitroCooldown = 5;
    private static float DefaultNitroSpeed= 20;
    private static float DefaultNitroDuration= 5;

    private static float DefaultAccelerationSpeedMax= 10;
    private static float DefaultAccelerationTimerMax = 5;
    #endregion
    
    #region PLAYER PREF KEYS
    private static string PrefKey_Movement = "MovementKey";
    private static string PrefKey_Rotation = "RotationKey";

    private static string PrefKey_TrailTotal = "TrailTotalKey";
    private static string PrefKey_TrailCap = "TrailCapKey";

    private static string PrefKey_Stun = "StunKey";
    private static string PrefKey_Blind = "BlindKey";
    private static string PrefKey_Confuse = "ConfuseKey";

    private static string PrefKey_MissleCooldown = "MissleCooldownKey";
    private static string PrefKey_ShieldCooldown = "ShieldCooldownKey";

    private static string PrefKey_NitroCooldown = "NitroCooldownKey";
    private static string PrefKey_NitroSpeed= "NitroSpeedKey";
    private static string PrefKey_NitroDuration = "NitroDurationKey";

    private static string PrefKey_AccelerationSpeedMax = "AccelerationSpeedMaxKey";
    private static string PrefKey_AccelerationTimerMax = "AccelerationTimerMaxKey";
    #endregion

    #endregion
    //==================================================================================================================================
    #region INITALIZATION
    void Awake()
    {
        _instance = this;
        MovementSpeed = PlayerPrefs.GetFloat(PrefKey_Movement, DefaultMovement);
        rotationSpeed = PlayerPrefs.GetFloat(PrefKey_Rotation, DefaultRotation);

        trailDistanceTotal = PlayerPrefs.GetFloat(PrefKey_TrailTotal, DefaulttrailDistanceTotal);
        trailDistanceChild = PlayerPrefs.GetFloat(PrefKey_TrailCap, DefaulttrailDistanceChild);

        const_StunDuration = PlayerPrefs.GetFloat(PrefKey_Stun, DefaultStun);
        BlindDuration = PlayerPrefs.GetFloat(PrefKey_Blind, DefaultBlind);
        ConfuseDuration = PlayerPrefs.GetFloat(PrefKey_Confuse, DefaultConfuse);

        missleCooldown = PlayerPrefs.GetFloat(PrefKey_MissleCooldown, DefaultMissleCooldown);
        shieldCooldown = PlayerPrefs.GetFloat(PrefKey_ShieldCooldown, DefaultShieldCooldown);

        nitroCooldown = PlayerPrefs.GetFloat(PrefKey_NitroCooldown, DefaultNitroCooldown);
        nitroSpeed = PlayerPrefs.GetFloat(PrefKey_NitroSpeed, DefaultNitroSpeed);
        nitroDuration = PlayerPrefs.GetFloat(PrefKey_NitroDuration, DefaultNitroDuration);

        accelerationSpeedMax = PlayerPrefs.GetFloat(PrefKey_AccelerationSpeedMax, DefaultAccelerationSpeedMax);
        accelerationTimerMax = PlayerPrefs.GetFloat(PrefKey_AccelerationTimerMax, DefaultAccelerationTimerMax);

        TweakMoveSpeed(0);
        TweakrotationSpeed(0);

        Tweakconst_trailDistanceChild(0);
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


        Speed_Stat = 0;
        Acceleration_Stat = 0;
        Rotation_Stat = 0;
        Rotation_Stat = 0;

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
        trailDistanceChild = (DefaulttrailDistanceChild);

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

        Tweakconst_trailDistanceChild(0);
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

        Tweakconst_trailDistanceChild(0);
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
        ReadyPlayer(GameSparkPacketReceiver.Instance.PeerID);
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


    public float Base_Value_Speed = 10, 
        Base_Value_Acceleration = 5, 
        Base_Value_Rotation = 40, 
        Base_Value_Trail = 5;

    public float Increment_Value_Speed = .25f,
        Increment_Value_Acceleration = -.1f,
        Increment_Value_Rotation = 2.5f,
        Increment_Value_Trail = .5f;

    float Speed_Stat, Acceleration_Stat, Rotation_Stat, Trail_Stat;
    [SerializeField]
    private Text Speed_Text, Acceleration_Text, Rotation_Text, Trail_Text;


    public void Add_Stat_Speed(int _stat)
    {
        Speed_Stat += _stat;
        Speed_Stat = Mathf.Clamp(Speed_Stat, 0, 20);

        accelerationSpeedMax = Base_Value_Speed + ( (Speed_Stat-1) * Increment_Value_Speed);
    }
    public void Add_Stat_Acceleration(int _stat)
    {
        Acceleration_Stat += _stat;
        Acceleration_Stat = Mathf.Clamp(Acceleration_Stat, 0, 20);

        accelerationTimerMax = Base_Value_Acceleration + ( (Acceleration_Stat-1) * Increment_Value_Acceleration);
    }
    public void Add_Stat_Rotation(int _stat)
    {
        Rotation_Stat += _stat;
        Rotation_Stat = Mathf.Clamp(Rotation_Stat, 0, 20);

        rotationSpeed = Base_Value_Rotation + ((Rotation_Stat - 1) * Increment_Value_Rotation);
    }
    public void Add_Stat_Trail(int _stat)
    {
        Trail_Stat += _stat;
        Trail_Stat = Mathf.Clamp(Trail_Stat, 0, 20);

        trailDistanceTotal = Base_Value_Trail+ ((Trail_Stat- 1) * Increment_Value_Trail);
        trailDistanceChild = trailDistanceTotal / 4;

        if (GameSparkPacketReceiver.Instance.PeerID == 1)
            PlayerObjects[0].GetComponent<Car_DataReceiver>().ReceiveTrailVAlue(trailDistanceTotal);
        if (GameSparkPacketReceiver.Instance.PeerID == 2)
            PlayerObjects[1].GetComponent<Car_DataReceiver>().ReceiveTrailVAlue(trailDistanceTotal);

        try
        {
            GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, GameSparkPacketReceiver.Instance.PeerID);
                data.SetFloat(2, trailDistanceTotal);
                GetRTSession.SendData(116, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        catch { }
    }



    //==================================================================================================================================
    #region TEST INPUTS G,K,L
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine("delaydeath");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ReadyPlayer(1);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ReadyPlayer(2);
        }
    }
    #endregion
    //==================================================================================================================================
}
