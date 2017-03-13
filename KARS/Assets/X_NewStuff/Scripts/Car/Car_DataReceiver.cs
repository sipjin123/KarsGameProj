using GameSparks.RT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car_DataReceiver : Car_Network_Interpolation
{
    bool initSkills;
    //================================================================================================================================
    #region VARIABLES
    private Car_Movement _carMovement;
    private GameSparksRTUnity GetRTSession;

    //NETWORK STUFF
    [SerializeField]
    public int Network_ID;
    public bool ifMy_Network_Player;

    [SerializeField]
    public Camera NetworkCam;



    //POWER UP AND DEBUFF

    //NITROS---------
    private bool NitroSwitch;
    public bool GetNitroSwitch() { return NitroSwitch; }
    //SHIELD---------
    private bool ShieldSwitch;
    public bool GetShieldSwitch() { return ShieldSwitch; }
    public GameObject ShieldObject;

    //FLY---------
    private bool FlySwitch;
    public bool GetFlySwitch() { return FlySwitch; }
    public GameObject FlyObject;

    //EXPAND---------
    private bool ExpandSwitch;
    public bool GetExpandSwitch() { return ExpandSwitch; }
    public GameObject ExpandObject;


    //STUN---------
    private bool StunSwitch;
    public bool GetStunSwitch() { return StunSwitch; }
    [SerializeField]
    private GameObject StunObject;

    //BLIND---------
    private bool BlindSwitch;
    public bool GetBlindSwitch() { return BlindSwitch; }
    public GameObject BlindObjectBlocker;
    public GameObject BlindObject;

    //CONFUSE---------
    private bool ConfuseSwitch;
    public GameObject ConfuseObject;

    //SLOW---------
    private bool SlowSwitch;
    public bool GetSlowSwitch() { return SlowSwitch; }
    [SerializeField]
    private GameObject SlowObject;

    //SILENCE---------
    private bool SilenceSwitch;
    public bool GetSilenceSwitch() { return SilenceSwitch; }
    [SerializeField]
    private GameObject SilenceObject;
    [SerializeField]
    private GameObject SilenceBlocker;

    //EXPLOSION---------
    private bool ExplosionSwitch;
    public bool GetExplosionSwitch() { return ExplosionSwitch; }
    [SerializeField]
    private GameObject ExplosionObject;


    public float Health;
    public GameObject[] AvatarList;
    #endregion
    //================================================================================================================================
    #region INIT
    public void StartGame(bool _switch)
    {
        if (Network_ID == _gameSparkPacketReceiver.PeerID)
        {
            _carMovement.StartGame = _switch;
        }
    }
    #endregion
    //================================================================================================================================
    void Update()
    {
        if (!ifMy_Network_Player && Network_ID != 0)
        {
            UpdateFunctInterpolate();
            return;
        }
        else
        {
            SendCarMovement(Network_ID, _objToTranslate.position, _objToRotate.eulerAngles);

            Test_Input();
        }
    }
    //================================================================================================================================
    #region DEBUG FEATURE
    public GameObject _testPanel;
    public void FlipTestPanel()
    {
        _testPanel.SetActive(!_testPanel.activeInHierarchy);
    }
    void Test_Input()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            ReduceHealth();
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            ResetTrail(true);
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            ResetTrail(false);
        }
    }
    #endregion
    //================================================================================================================================
    #region DEBUFF COOLDOWNS
    IEnumerator StartConfuseTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.ConfuseDuration);
        ConfuseSwitch = false;
        ReceiveDisableSTate(ConfuseSwitch, NetworkPlayerStatus.ACTIVATE_CONFUSE);
        SendNetworkDisable(ConfuseSwitch, NetworkPlayerStatus.ACTIVATE_CONFUSE);
    }
    IEnumerator StartStunTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.const_StunDuration);
        StunSwitch = false;
        ReceiveDisableSTate(StunSwitch, NetworkPlayerStatus.ACTIVATE_STUN);
        SendNetworkDisable(StunSwitch, NetworkPlayerStatus.ACTIVATE_STUN);
    }
    IEnumerator StartBlindTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.BlindDuration);
        BlindSwitch = false;
        ReceiveDisableSTate(BlindSwitch, NetworkPlayerStatus.ACTIVATE_BLIND);
        SendNetworkDisable(BlindSwitch, NetworkPlayerStatus.ACTIVATE_BLIND);
    }
    IEnumerator StartSlowTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.BlindDuration);
        SlowSwitch = false;
        ReceiveDisableSTate(SlowSwitch, NetworkPlayerStatus.ACTIVATE_SLOW);
        SendNetworkDisable(SlowSwitch, NetworkPlayerStatus.ACTIVATE_SLOW);
    }
    IEnumerator StartSilenceTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.BlindDuration);
        SilenceSwitch = false;
        ReceiveDisableSTate(SilenceSwitch, NetworkPlayerStatus.ACTIVATE_SILENCE);
        SendNetworkDisable(SilenceSwitch, NetworkPlayerStatus.ACTIVATE_SILENCE);
    }
    #endregion
    #region BUFF COOLDOWNS
    IEnumerator CoolDown_Shield()
    {
        yield return new WaitForSeconds(5);
        ShieldSwitch = false;
        ShieldObject.SetActive(false);
        SendNetworkPowerUp(false, NetworkPlayerStatus.ACTIVATE_SHIELD);
    }
    IEnumerator CoolDown_Fly()
    {
        yield return new WaitForSeconds(5);
        FlySwitch = false;
        FlyObject.SetActive(false);
        SendNetworkPowerUp(false, NetworkPlayerStatus.ACTIVATE_FLY);
    }
    IEnumerator CoolDown_Expand()
    {
        yield return new WaitForSeconds(5);
        ExpandSwitch = false;
        ExpandObject.SetActive(false);
        SendNetworkPowerUp(false, NetworkPlayerStatus.ACTIVATE_EXPAND);
    }
    IEnumerator CoolDown_Nitro()
    {
        yield return new WaitForSeconds(5);
        NitroSwitch = false;
    }
    #endregion
    //================================================================================================================================
    //
    //                                                       LOCAL DATA
    //
    //================================================================================================================================
    public void ResetPowerups()
    {
        StunObject.SetActive(false);
        StunSwitch = false;

        BlindObjectBlocker.SetActive(false);
        BlindObject.SetActive(false);
        BlindSwitch = false;

        ConfuseObject.SetActive(false);
        ConfuseSwitch = false;
        //_carMovement.FlipSwitch = false;

        SlowObject.SetActive(false);
        SlowSwitch = false;

        SilenceObject.SetActive(false);
        SilenceBlocker.SetActive(false);
        SilenceSwitch = false;

        ShieldSwitch = false;
        ShieldObject.SetActive(false);

        FlySwitch = false;
        FlyObject.SetActive(false);

        ExpandSwitch = false;
        ExpandObject.SetActive(false);

        NitroSwitch = false;
    }
    #region STATUS
    public void Activate_StateFromButton(NetworkPlayerStatus _status)
    {
        if (Network_ID == 0)
            return;
        switch (_status)
        {
            case NetworkPlayerStatus.ACTIVATE_BLIND:
                {
                    if (BlindSwitch == false)
                    {
                        BlindSwitch = true;
                        BlindObjectBlocker.SetActive(true);
                        BlindObject.SetActive(true);

                        SendNetworkDisable(true, NetworkPlayerStatus.ACTIVATE_BLIND);
                        StartCoroutine("StartBlindTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_STUN:
                {
                    if (StunSwitch == false)
                    {
                        StunSwitch = true;
                        StunObject.SetActive(true);

                        SendNetworkDisable(true, NetworkPlayerStatus.ACTIVATE_STUN);
                        StartCoroutine("StartStunTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_CONFUSE:
                {
                    if (ConfuseSwitch == false)
                    {
                        ConfuseSwitch = true;
                        ConfuseObject.SetActive(true);

                        SendNetworkDisable(true, NetworkPlayerStatus.ACTIVATE_CONFUSE);
                        StartCoroutine("StartConfuseTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_SLOW:
                {
                    if (SlowSwitch == false)
                    {
                        SlowSwitch = true;
                        SlowObject.SetActive(true);

                        SendNetworkDisable(true, NetworkPlayerStatus.ACTIVATE_SLOW);
                        StartCoroutine("StartSlowTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_SILENCE:
                {
                    if (SilenceSwitch == false)
                    {
                        SilenceSwitch = true;
                        SilenceObject.SetActive(true);
                        SilenceBlocker.SetActive(true);

                        SendNetworkDisable(true, NetworkPlayerStatus.ACTIVATE_SILENCE);
                        StartCoroutine("StartSilenceTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_EXPLOSION:
                {
                    UIManager.Instance.GameUpdateText.text += "\nSEND EXPLOSION LOCALLY";
                    {
                        ExplosionSwitch = true;
                        ExplosionObject.SetActive(true);

                        SendNetworkDisable(true, NetworkPlayerStatus.ACTIVATE_EXPLOSION);
                    }
                }
                break;
        }
    }
    #endregion
    #region POWERUP
    public void ActivatePowerUpFromButton(int _val)
    {
        NetworkPlayerStatus _netStat = NetworkPlayerStatus.ACTIVATE_SHIELD;
        switch (_val)
        {
            case 0:
                _netStat = NetworkPlayerStatus.ACTIVATE_SHIELD;
                break;
            case 1:
                _netStat = NetworkPlayerStatus.ACTIVATE_FLY;
                break;
            case 2:
                _netStat = NetworkPlayerStatus.ACTIVATE_EXPAND;
                break;
            case 3:
                _netStat = NetworkPlayerStatus.ACTIVATE_NITRO;
                break;
        }

        switch (_netStat)
        {
            case NetworkPlayerStatus.ACTIVATE_SHIELD:
                {
                    ShieldSwitch = true;
                    ShieldObject.SetActive(true);
                    SendNetworkPowerUp(true, NetworkPlayerStatus.ACTIVATE_SHIELD);
                    StartCoroutine(CoolDown_Shield());
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_FLY:
                {
                    FlySwitch = true;
                    FlyObject.SetActive(true);
                    SendNetworkPowerUp(true, NetworkPlayerStatus.ACTIVATE_FLY);
                    StartCoroutine(CoolDown_Fly());
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_EXPAND:
                {
                    ExpandSwitch = true;
                    ExpandObject.SetActive(true);
                    SendNetworkPowerUp(true, NetworkPlayerStatus.ACTIVATE_EXPAND);
                    StartCoroutine(CoolDown_Expand());
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_GHOST:
                {

                }
                break;
            case NetworkPlayerStatus.ACTIVATE_NITRO:
                {
                    NitroSwitch = true;
                    StartCoroutine(CoolDown_Nitro());
                }
                break;
        }
    }
    #endregion
    //================================================================================================================================
    //
    //                                                       SEND DATA
    //
    //================================================================================================================================
    #region DATA_SENDING
    public void SendCarMovement(int _id, Vector3 _pos, Vector3 _rot)
    {
        if (TronGameManager.Instance.NetworkStart)
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, _id);
                data.SetFloat(2, _pos.x);
                data.SetFloat(3, _pos.y);
                data.SetFloat(4, _pos.z);
                data.SetVector3(5, _rot);
                data.SetDouble(6, Network.time);
                data.SetDouble(7, _gameSparkPacketReceiver.gameTimeInt);


                GetRTSession.SendData(OPCODE_CLASS.MovementOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
    }

    public void ReduceHealth()
    {
        Health -= 1;
        UIManager.Instance.AdjustHPBarAndText(Network_ID, Health);

        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            data.SetInt(2, (int)NetworkPlayerVariableList.HEALTH);
            data.SetFloat(3, Health);
            GetRTSession.SendData(OPCODE_CLASS.HealthOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }

        if (Health <= 0)
        {
            TronGameManager.Instance.Global_SendState(MENUSTATE.RESULT);
        }
    }

    public void SendNetworkPowerUp(bool _switch, NetworkPlayerStatus _status)
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            if (_switch)
                data.SetInt(2, 1);
            else
                data.SetInt(2, 0);

            data.SetInt(3, (int)_status);
            GetRTSession.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }

    public void ResetTrail(bool _switch)
    {
        _carMovement._trailCollision.SetEmiision(_switch);

        if (TronGameManager.Instance.NetworkStart == true)
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, Network_ID);
                data.SetInt(2, _switch == true ? 1 : 0);
                data.SetInt(3, (int)NetworkPlayerStatus.ACTIVATE_TRAIL);

                GetRTSession.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
    }

    public void SendNetworkDisable(bool _switch, NetworkPlayerStatus _status)
    {
        if (TronGameManager.Instance.NetworkStart == false)
            return;
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            if (_switch)
                data.SetInt(2, 1);
            else
                data.SetInt(2, 0);

            data.SetInt(3, (int)_status);
            GetRTSession.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
    #endregion
    //================================================================================================================================
    //
    //                                                       RECEIVE DATA
    //
    //================================================================================================================================
    #region RECEIVE POWERUP
    public void ReceivePowerUpState(bool _switch, NetworkPlayerStatus _netStatus)
    {
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_SHIELD)
        {
            ShieldObject.SetActive(_switch);
            ShieldSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_FLY)
        {
            FlyObject.SetActive(_switch);
            FlySwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_EXPAND)
        {
            ExpandObject.SetActive(_switch);
            ExpandSwitch = _switch;
        }
    }
    #endregion
    #region RECEIVE DISABLE
    public void ReceiveDisableSTate(bool _switch, NetworkPlayerStatus _netStatus)
    {
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_STUN)
        {
            StunObject.SetActive(_switch);
            StunSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_BLIND)
        {
            BlindObjectBlocker.SetActive(_switch);
            BlindObject.SetActive(_switch);
            BlindSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_CONFUSE)
        {
            ConfuseObject.SetActive(_switch);
            ConfuseSwitch = _switch;
            _carMovement.FlipCarSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_SLOW)
        {
            SlowObject.SetActive(_switch);
            SlowSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_SILENCE)
        {
            SilenceObject.SetActive(_switch);
            SilenceBlocker.SetActive(_switch);
            SilenceSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_EXPLOSION)
        {
            UIManager.Instance.GameUpdateText.text += "\n\tEXPLOSION Command: "+_switch;
            ExplosionObject.SetActive(_switch);
            ExplosionSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.SET_READY)
        {
            try
            {
                _carMovement.SetReady(_switch);
                UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: SUCCESSFULLY READY THIS PLAYER";

                //2 PLAYERS READY
                if (TronGameManager.Instance.PlayerObjects[0].GetComponent<Car_Movement>().isREady
                    && TronGameManager.Instance.PlayerObjects[1].GetComponent<Car_Movement>().isREady)
                {
                    GameSparkPacketReceiver.Instance.Access_SentStartToServer();
                    UIManager.Instance.GameUpdateText.text += "\nPhase 5: Both players are ready";
                }
                else
                {
                    GameSparkPacketReceiver.Instance.Access_SentReadyToServer();
                    UIManager.Instance.GameUpdateText.text += "\n\tBoth players are NOT ready, tryng again";
                }
            }
            catch
            {
                UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: FAILED TO READY, RETRYING, The cound of player objects is: " + TronGameManager.Instance.PlayerObjects.Length;
                StopCoroutine("delayRestartReady");
                StartCoroutine(delayRestartReady(_switch, _netStatus));
            }
        }
        if (_netStatus == NetworkPlayerStatus.SET_START)
        {
            try
            {
                UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: SUCCESSFULLY START THIS PLAYER";
                _carMovement.SetStartGame(_switch);
                StateButtonManager.Instance.OnClick_ResetGame();
            }
            catch
            {
                UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: FAILED TO START, RETRYING";
                StartCoroutine(delayRestartReady(_switch, _netStatus));
            }
        }
    }

    IEnumerator delayRestartReady(bool _switch, NetworkPlayerStatus _netstatus)
    {
        yield return new WaitForSeconds(2);
        ReceiveDisableSTate(_switch, _netstatus);
    }

    #endregion
    #region AVATAR SELECTION
    public void SetCarAvatar(int _avatarNumber)
    {
        for (int i = 0; i < AvatarList.Length; i++)
        {
            AvatarList[i].SetActive(false);
        }

        AvatarList[_avatarNumber].SetActive(true);
    }
    #endregion
    #region RECEIVE NETWORK INITIALIZATION
    public void SetNetworkObject(int netID)
    {
        Network_ID = netID;
    }

    public void InitCam()
    {
        _carMovement = GetComponent<Car_Movement>();
        _gameSparkPacketReceiver = GameSparkPacketReceiver.Instance.GetComponent<GameSparkPacketReceiver>();
        GetRTSession = _gameSparkPacketReceiver.GetRTSession();
        if (Network_ID == _gameSparkPacketReceiver.PeerID)
        {
            if (Network_ID == 0)
            {
                StopCoroutine("DelayREsetCam");
                StartCoroutine("DelayREsetCam");
                return;
            }
            NetworkCam.enabled = true;
            ifMy_Network_Player = true;

            ResetTrail(true);
            Health = 5;


            UIManager.Instance.ActivatePlayerPanel(Network_ID);
            
            InitSkillsFunc();
        }
    }

    void InitSkillsFunc()
    {

        Transform skillParent;
        Transform skillRoster;
        if (GameSparkPacketReceiver.Instance.PeerID == 1)
        {
            skillParent = UIManager.Instance.Player1_SkillsParent.transform;
            skillRoster = UIManager.Instance.Player1_SkillsRoster.transform;
        }
        else
        {
            skillParent = UIManager.Instance.Player2_SkillsParent.transform;
            skillRoster = UIManager.Instance.Player2_SkillsRoster.transform;
        }

        foreach (Transform T in skillParent)
        {
            if (Network_ID == 1)
            {
                T.GetComponent<Button>().onClick.RemoveListener(() => CDThisSkillSlot(0, T.gameObject));
                T.GetComponent<Button>().onClick.RemoveListener(() => CDThisSkillSlot(1, T.gameObject));
                T.SetParent(UIManager.Instance.Player1_SkillsRoster.transform);
            }
            if (Network_ID == 2)
            {
                T.GetComponent<Button>().onClick.RemoveListener(() => CDThisSkillSlot(0, T.gameObject));
                T.GetComponent<Button>().onClick.RemoveListener(() => CDThisSkillSlot(1, T.gameObject));
                T.SetParent(UIManager.Instance.Player2_SkillsRoster.transform);
            }
        }

        foreach (Transform T in skillRoster)
        {
            if (T.gameObject.name == TronGameManager.Instance.selected_currentSkill_Text[0].text)
            {
                T.SetParent(skillParent);
                T.localScale = Vector3.one;
                T.gameObject.SetActive(true);
                T.GetComponent<Button>().onClick.AddListener(() => CDThisSkillSlot(0, T.gameObject));
                Debug.LogError("injected a func 0");
                CooldownCap[0] = CheckCoolDownCap(TronGameManager.Instance.selected_currentSkill_Text[0].text);

                coolDown_Timer[0] = 0;
                break;
            }
        }
        foreach (Transform T in skillRoster)
        {
            if (T.gameObject.name == TronGameManager.Instance.selected_currentSkill_Text[1].text)
            {
                T.SetParent(skillParent);
                T.localScale = Vector3.one;
                T.gameObject.SetActive(true);
                T.GetComponent<Button>().onClick.AddListener(() => CDThisSkillSlot(1, T.gameObject));
                Debug.LogError("injected a func 1");
                CooldownCap[1] = CheckCoolDownCap(TronGameManager.Instance.selected_currentSkill_Text[1].text);
                coolDown_Timer[1] = 0;
                break;
            }
        }
    }

    float CheckCoolDownCap(string _val)
    {
        SKILL_LIST test = (SKILL_LIST)Enum.Parse(typeof(SKILL_LIST), _val);
        Debug.LogError("SKILL IS: " + test);
        switch(test)
        {
            case SKILL_LIST.Blind:
                    return TronGameManager.Instance.BlindDuration;
            case SKILL_LIST.Confuse:
                return TronGameManager.Instance.ConfuseDuration;
            case SKILL_LIST.Expand:
                return TronGameManager.Instance.const_StunDuration;
            case SKILL_LIST.Fly:
                return TronGameManager.Instance.const_StunDuration;
            case SKILL_LIST.Nitro:
                return TronGameManager.Instance.nitroDuration;
            case SKILL_LIST.Shield:
                return TronGameManager.Instance.shieldCooldown;
            case SKILL_LIST.Silence:
                return TronGameManager.Instance.const_StunDuration;
            case SKILL_LIST.Slow:
                return TronGameManager.Instance.const_StunDuration;
            case SKILL_LIST.Stun:
                return TronGameManager.Instance.const_StunDuration;
        }
        return 0;
    }

    IEnumerator DelayREsetCam()
    {
        yield return new WaitForSeconds(2);
        InitCam();
    }
    #endregion

    #region TRAILS
    private float _TrailValue = 50;
    private float _TrailValueDividend = 5;
    public float TrailValue()
    {
        return _TrailValue;
    }
    public float TrailValueDividend()
    {
        return _TrailValueDividend;
    }
    public void ReceiveTrailVAlue(float _trailValue)
    {
        _TrailValue = _trailValue;
    }
    public void ReceiveTrailChildVAlue(float _trailValueDividend)
    {
        _TrailValueDividend = _trailValueDividend;
    }
    #endregion
    //================================================================================================================================
    public void CDThisSkillSlot(int _val , GameObject _obj)
    {
        Debug.LogError("SKILL COOLDOWN: " + _val + " " + _obj.name);
        coolDown_Switch[_val] = true;
    }

    float[] CooldownCap = new float[2];
    bool [] coolDown_Switch = new bool[2];
    float []coolDown_Timer = new float[2];

    void FixedUpdate()
    {
        for (int q = 0; q < coolDown_Switch.Length; q++)
        {
            if (coolDown_Switch[q] == true)
            {
                if (coolDown_Timer[q] < CooldownCap[q])
                {
                    UIManager.Instance.StartCooldDownForBlockers(Network_ID, q, coolDown_Timer[q], CooldownCap[q]);
                    coolDown_Timer[q] += Time.deltaTime;
                }
                else
                {
                    coolDown_Switch[q] = false;
                    coolDown_Timer[q] = 0;
                }
            }
        }
    }
}
