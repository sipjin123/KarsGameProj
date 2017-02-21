using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TronGameManager : MonoBehaviour {

    private static TronGameManager _instance;
    public static TronGameManager Instance { get { return _instance; } }


    #region TWEAKABLE VARIABLES

    public Text Text_MovementSpeed;
    public float MovementSpeed ;
    public void TweakMoveSpeed(float _var)
    {
        MovementSpeed += _var;
        Text_MovementSpeed.text = MovementSpeed.ToString();
        PlayerPrefs.SetFloat(PrefKey_Movement, MovementSpeed);
    }

    public Text Text_rotationSpeed;
    public float rotationSpeed;
    public void TweakrotationSpeed(float _var)
    {
        rotationSpeed += _var;
        Text_rotationSpeed.text = rotationSpeed.ToString();
        PlayerPrefs.SetFloat(PrefKey_Rotation, rotationSpeed);
    }


    public Text Text_trailDistanceTotal;
    public float trailDistanceTotal;
    public void TweaktrailDistanceTotal(float _var)
    {
        trailDistanceTotal += _var;
        Text_trailDistanceTotal.text = trailDistanceTotal.ToString();
        PlayerPrefs.SetFloat(PrefKey_TrailTotal, trailDistanceTotal);
    }

    public Text Text_trailDistanceChild;
    public float trailDistanceChild;
    public void Tweakconst_trailDistanceChild(float _var)
    {
        trailDistanceChild += _var;
        Text_trailDistanceChild.text = trailDistanceChild.ToString();
        PlayerPrefs.SetFloat(PrefKey_TrailCap, trailDistanceChild);
    }


    public Text Text_const_StunDuration;
    public float const_StunDuration;
    public void Tweakconst_StunDuration(float _var)
    {
        const_StunDuration += _var;
        Text_const_StunDuration.text = const_StunDuration.ToString();
        PlayerPrefs.SetFloat(PrefKey_Stun, const_StunDuration);
    }



    public Text Text_missleCooldown;
    public float missleCooldown;
    public void Tweak_missleCooldown(float _var)
    {
        missleCooldown += _var;
        Text_missleCooldown.text = missleCooldown.ToString();
        PlayerPrefs.SetFloat(PrefKey_MissleCooldown, missleCooldown);
    }


    public Text Text_shieldCooldown;
    public float shieldCooldown;
    public void Tweak_shieldCooldown(float _var)
    {
        shieldCooldown += _var;
        Text_shieldCooldown.text = shieldCooldown.ToString();
        PlayerPrefs.SetFloat(PrefKey_ShieldCooldown, shieldCooldown);
    }

    public GameObject _testPanel;
    public void FlipTestPanel()
    {
        _testPanel.SetActive(!_testPanel.activeInHierarchy);
    }
    #endregion



    private static float DefaultMovement = 3;
    private static float DefaultRotation= 55;
    private static float DefaultStun = 5;
    private static float DefaulttrailDistanceTotal = 20;
    private static float DefaulttrailDistanceChild = 5;
    private static float DefaultMissleCooldown = 5;
    private static float DefaultShieldCooldown = 5;

    private static string PrefKey_Movement = "MovementKey";
    private static string PrefKey_Rotation = "RotationKey";
    private static string PrefKey_Stun = "StunKey";
    private static string PrefKey_TrailTotal = "TrailTotalKey";
    private static string PrefKey_TrailCap = "TrailCapKey";
    private static string PrefKey_MissleCooldown = "MissleCooldownKey";
    private static string PrefKey_ShieldCooldown = "ShieldCooldownKey";

    void Awake()
    {
        _instance = this;
        MovementSpeed = PlayerPrefs.GetFloat(PrefKey_Movement, DefaultMovement);
        rotationSpeed = PlayerPrefs.GetFloat(PrefKey_Rotation, DefaultRotation);
        const_StunDuration = PlayerPrefs.GetFloat(PrefKey_Stun, DefaultStun);
        trailDistanceTotal = PlayerPrefs.GetFloat(PrefKey_TrailTotal, DefaulttrailDistanceTotal);
        trailDistanceChild = PlayerPrefs.GetFloat(PrefKey_TrailCap, DefaulttrailDistanceChild);

        missleCooldown = PlayerPrefs.GetFloat(PrefKey_MissleCooldown, DefaultMissleCooldown);
        shieldCooldown = PlayerPrefs.GetFloat(PrefKey_ShieldCooldown, DefaultShieldCooldown);
    }

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

    //==================================================================================================================================
    #region TEST INPUTS G,K,L
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine("delaydeath");
        }
        if(Input.GetKeyDown(KeyCode.K))
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
        const_StunDuration = (DefaultStun);
        trailDistanceTotal = (DefaulttrailDistanceTotal);
        trailDistanceChild = (DefaulttrailDistanceChild);

        missleCooldown = (DefaultMissleCooldown);
        shieldCooldown = (DefaultShieldCooldown);
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
        Tweakconst_StunDuration(0);
        Tweakconst_trailDistanceChild(0);
        TweaktrailDistanceTotal(0);

        Tweak_missleCooldown(0);
        Tweak_shieldCooldown(0);

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
}
