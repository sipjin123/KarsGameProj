using GameSparks.RT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car_DataReceiver : Car_Network_Interpolation
{
    //================================================================================================================================
    #region VARIABLES
    private Car_Movement _carMovement;
    private GameSparksRTUnity GetRTSession;

    [SerializeField]
    private GameObject[] AvatarList;

    //PLAYER STATS
    float health;

    //NETWORK STUFF
    #region NETWORK
    private bool ifMy_Network_Player;
    private int network_ID;
    [SerializeField]
    private Camera NetworkCam;
    [SerializeField]
    private AudioListener audioListener;
    #endregion

    //SKILLS 
    #region SKILLS
    bool initSkills;
    Transform[] ButtonObject = new Transform[2];
    string[] CooldownName = new string[2];
    float[] CooldownCap = new float[2];
    float[] coolDown_Timer = new float[2];
    bool[] coolDown_Switch = new bool[2];
    #endregion
    
    //POWER UP AND DEBUFF
    #region DEBUFF AND POWERUPS
    //NITROS---------
    private bool NitroSwitch;
    public bool GetNitroSwitch() { return NitroSwitch; }
    //SHIELD---------
    private bool ShieldSwitch;
    public bool GetShieldSwitch() { return ShieldSwitch; }
    [SerializeField]
    private GameObject ShieldObject;

    //FLY---------
    private bool FlySwitch;
    public bool GetFlySwitch() { return FlySwitch; }
    [SerializeField]
    private GameObject FlyObject;

    //EXPAND---------
    private bool ExpandSwitch;
    public bool GetExpandSwitch() { return ExpandSwitch; }
    [SerializeField]
    private GameObject ExpandObject;


    //STUN---------
    private bool StunSwitch;
    public bool GetStunSwitch() { return StunSwitch; }
    [SerializeField]
    private GameObject StunObject;

    //BLIND---------
    private bool BlindSwitch;
    public bool GetBlindSwitch() { return BlindSwitch; }
    [SerializeField]
    private GameObject BlindObjectBlocker;
    [SerializeField]
    private GameObject BlindObject;

    //CONFUSE---------
    private bool ConfuseSwitch;
    [SerializeField]
    private GameObject ConfuseObject;

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
    #endregion

    //PUBLIC GET/SET VARIABLES
    #region PUBLIC GET/SET VARIABLES
    public int GetNetwork_ID()
    {
        return network_ID;
    }
    public void SetHealth(float _val)
    {
        health = _val;
    }
    #endregion
    #endregion
    //================================================================================================================================
    void Awake()
    {
        _carMovement = GetComponent<Car_Movement>();
    }
    
    void Update()
    {
        if (!ifMy_Network_Player && network_ID != 0)
        {
            UpdateFunctInterpolate();
            return;
        }
        else
        {
            SendCarMovement(network_ID, _objToTranslate.position, _objToRotate.eulerAngles);

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
            SendNetworkStatus(true,NetworkPlayerStatus.ACTIVATE_TRAIL);
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            SendNetworkStatus(false, NetworkPlayerStatus.ACTIVATE_TRAIL);
        }
    }
    #endregion
    //================================================================================================================================
    #region DEBUFF COOLDOWNS
    IEnumerator StartConfuseTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.ConfuseDuration);
        ConfuseSwitch = false;
        ReceivePlayerSTate(ConfuseSwitch, NetworkPlayerStatus.ACTIVATE_CONFUSE);
        SendNetworkStatus(ConfuseSwitch, NetworkPlayerStatus.ACTIVATE_CONFUSE);
    }
    IEnumerator StartStunTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.const_StunDuration);
        StunSwitch = false;
        ReceivePlayerSTate(StunSwitch, NetworkPlayerStatus.ACTIVATE_STUN);
        SendNetworkStatus(StunSwitch, NetworkPlayerStatus.ACTIVATE_STUN);
    }
    IEnumerator StartBlindTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.BlindDuration);
        BlindSwitch = false;
        ReceivePlayerSTate(BlindSwitch, NetworkPlayerStatus.ACTIVATE_BLIND);
        SendNetworkStatus(BlindSwitch, NetworkPlayerStatus.ACTIVATE_BLIND);
    }
    IEnumerator StartSlowTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.BlindDuration);
        SlowSwitch = false;
        ReceivePlayerSTate(SlowSwitch, NetworkPlayerStatus.ACTIVATE_SLOW);
        SendNetworkStatus(SlowSwitch, NetworkPlayerStatus.ACTIVATE_SLOW);
    }
    IEnumerator StartSilenceTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.BlindDuration);
        SilenceSwitch = false;
        ReceivePlayerSTate(SilenceSwitch, NetworkPlayerStatus.ACTIVATE_SILENCE);
        SendNetworkStatus(SilenceSwitch, NetworkPlayerStatus.ACTIVATE_SILENCE);
    }
    #endregion
    #region BUFF COOLDOWNS
    IEnumerator CoolDown_Shield()
    {
        yield return new WaitForSeconds(5);
        ShieldSwitch = false;
        ShieldObject.SetActive(false);
        SendNetworkStatus(false, NetworkPlayerStatus.ACTIVATE_SHIELD);
    }
    IEnumerator CoolDown_Fly()
    {
        yield return new WaitForSeconds(5);
        FlySwitch = false;
        FlyObject.SetActive(false);
        SendNetworkStatus(false, NetworkPlayerStatus.ACTIVATE_FLY);
    }
    IEnumerator CoolDown_Expand()
    {
        yield return new WaitForSeconds(5);
        ExpandSwitch = false;
        ExpandObject.SetActive(false);
        SendNetworkStatus(false, NetworkPlayerStatus.ACTIVATE_EXPAND);
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
    public void Access_ResetPowerups()
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

        ExplosionObject.SetActive(false);
        ExplosionSwitch = false;
    }
    public void Access_ResetNetwork()
    {
        NetworkCam.enabled = false;
        network_ID = 0;
        ifMy_Network_Player = false;
        audioListener.enabled = false;
    }
    #region STATUS
    public void Activate_StateFromButton(NetworkPlayerStatus _status)
    {
        if (network_ID == 0)
            return;
        switch (_status)
        {
            case NetworkPlayerStatus.ACTIVATE_BLIND:
                {
                    if (BlindSwitch == false)
                    {
                        ReceivePlayerSTate(true, NetworkPlayerStatus.ACTIVATE_BLIND);
                        //BlindSwitch = true;
                        //BlindObjectBlocker.SetActive(true);
                        //BlindObject.SetActive(true);

                        SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_BLIND);
                        StartCoroutine("StartBlindTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_STUN:
                {
                    if (StunSwitch == false)
                    {
                        ReceivePlayerSTate(true, NetworkPlayerStatus.ACTIVATE_STUN);
                        //StunSwitch = true;
                        //StunObject.SetActive(true);

                        SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_STUN);
                        StartCoroutine("StartStunTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_CONFUSE:
                {
                    if (ConfuseSwitch == false)
                    {
                        ReceivePlayerSTate(true, NetworkPlayerStatus.ACTIVATE_CONFUSE);
                        //ConfuseSwitch = true;
                        //ConfuseObject.SetActive(true);

                        SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_CONFUSE);
                        StartCoroutine("StartConfuseTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_SLOW:
                {
                    if (SlowSwitch == false)
                    {
                        ReceivePlayerSTate(true, NetworkPlayerStatus.ACTIVATE_SLOW);
                        //SlowSwitch = true;
                        //SlowObject.SetActive(true);

                        SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_SLOW);
                        StartCoroutine("StartSlowTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_SILENCE:
                {
                    if (SilenceSwitch == false)
                    {
                        ReceivePlayerSTate(true, NetworkPlayerStatus.ACTIVATE_SILENCE);
                        //SilenceSwitch = true;
                        //SilenceObject.SetActive(true);
                        //SilenceBlocker.SetActive(true);

                        SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_SILENCE);
                        StartCoroutine("StartSilenceTimer");
                    }
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_EXPLOSION:
                {
                    {
                        ReceivePlayerSTate(true, NetworkPlayerStatus.ACTIVATE_EXPLOSION);
                        //ExplosionSwitch = true;
                        //ExplosionObject.SetActive(true);

                        SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_EXPLOSION);
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
                    SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_SHIELD);
                    StartCoroutine(CoolDown_Shield());
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_FLY:
                {
                    FlySwitch = true;
                    FlyObject.SetActive(true);
                    SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_FLY);
                    StartCoroutine(CoolDown_Fly());
                }
                break;
            case NetworkPlayerStatus.ACTIVATE_EXPAND:
                {
                    ExpandSwitch = true;
                    ExpandObject.SetActive(true);
                    SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_EXPAND);
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
                data.SetDouble(7, gameSparksPacketHandler.GetGameClockINT());

                GetRTSession.SendData(OPCODE_CLASS.MovementOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
    }

    public void ReduceHealth()
    {
        health -= 1;
        UIManager.Instance.AdjustHPBarAndText(network_ID, health);

        using (RTData data = RTData.Get())
        {
            data.SetInt(1, network_ID);
            data.SetInt(2, (int)NetworkPlayerVariableList.HEALTH);
            data.SetFloat(3, health);
            GetRTSession.SendData(OPCODE_CLASS.HealthOpcode, GameSparksRT.DeliveryIntent.RELIABLE, data);
        }
    }

    public void SendNetworkStatus(bool _switch, NetworkPlayerStatus _status)
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, network_ID);
            data.SetInt(2, _switch == true ? 1 : 0);
            data.SetInt(3, (int)_status);
            GetRTSession.SendData(OPCODE_CLASS.StatusOpcode, GameSparksRT.DeliveryIntent.RELIABLE, data);
        }
        if(_status == NetworkPlayerStatus.ACTIVATE_TRAIL)
            _carMovement._trailCollision.SetEmiision(_switch);
    }
    #endregion
    //================================================================================================================================
    //
    //                                                       RECEIVE DATA
    //
    //================================================================================================================================
    #region RECEIVE PLAYER STAT
    public void ReceivePlayerSTate(bool _switch, NetworkPlayerStatus _netStatus)
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

        if (_netStatus == NetworkPlayerStatus.ACTIVATE_STUN)
        {
            if(_switch)
            {
                AudioManager.Instance.SpawnableAudio(transform.position, AUDIO_CLIP.MISSLE_HIT);
            }
            StunObject.SetActive(_switch);
            StunSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_BLIND)
        {
            if (_switch)
            {
                AudioManager.Instance.SpawnableAudio(transform.position, AUDIO_CLIP.MISSLE_HIT);
            }
            BlindObjectBlocker.SetActive(_switch);
            BlindObject.SetActive(_switch);
            BlindSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_CONFUSE)
        {
            if (_switch)
            {
                AudioManager.Instance.SpawnableAudio(transform.position, AUDIO_CLIP.MISSLE_HIT);
            }
            ConfuseObject.SetActive(_switch);
            ConfuseSwitch = _switch;
            _carMovement.FlipCarSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_SLOW)
        {
            if (_switch)
            {
                AudioManager.Instance.SpawnableAudio(transform.position, AUDIO_CLIP.MISSLE_HIT);
            }
            SlowObject.SetActive(_switch);
            SlowSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_SILENCE)
        {
            if (_switch)
            {
                AudioManager.Instance.SpawnableAudio(transform.position, AUDIO_CLIP.MISSLE_HIT);
            }
            SilenceObject.SetActive(_switch);
            SilenceBlocker.SetActive(_switch);
            SilenceSwitch = _switch;
        }
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_EXPLOSION)
        {
            ExplosionObject.SetActive(_switch);
            ExplosionSwitch = _switch;
        }

        if (_netStatus == NetworkPlayerStatus.SET_READY)
        {
            Process_Ready();
        }
        if (_netStatus == NetworkPlayerStatus.SET_START)
        {
            Process_Start();
        }
    }

    IEnumerator delayRestartReady(bool _switch, NetworkPlayerStatus _netstatus)
    {
        yield return new WaitForSeconds(2);
        ReceivePlayerSTate(_switch, _netstatus);
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
        network_ID = netID;
    }

    public void InitCam()
    {
        gameSparksPacketHandler = GameSparkPacketHandler.Instance.GetComponent<GameSparkPacketHandler>();
        GetRTSession = gameSparksPacketHandler.GetRTSession();
        if (network_ID == gameSparksPacketHandler.GetPeerID())
        {
            if (network_ID == 0)
            {
                StopCoroutine("DelayREsetCam");
                StartCoroutine("DelayREsetCam");
                return;
            }
            NetworkCam.enabled = true;
            ifMy_Network_Player = true;
            audioListener.enabled = true;

            SendNetworkStatus(true, NetworkPlayerStatus.ACTIVATE_TRAIL);
            health = 5;


            UIManager.Instance.ActivatePlayerPanel(network_ID);
            
            InitSkillsFunc();
        }
    }
    IEnumerator DelayREsetCam()
    {
        yield return new WaitForSeconds(2);
        InitCam();
    }

    void InitSkillsFunc()
    {

        Transform[] skillParent = new Transform[2];
        Transform skillRoster;
        
        //DETERMINING SKILL HOLDERS
        if (GameSparkPacketHandler.Instance.GetPeerID() == 1)
        {
            skillParent[0] = UIManager.Instance.Player1_SkillsParent[0].transform;
            skillParent[1] = UIManager.Instance.Player1_SkillsParent[1].transform;
            skillRoster = UIManager.Instance.Player1_SkillsRoster.transform;
        }
        else
        {
            skillParent[0] = UIManager.Instance.Player2_SkillsParent[0].transform;
            skillParent[1] = UIManager.Instance.Player2_SkillsParent[1].transform;
            skillRoster = UIManager.Instance.Player2_SkillsRoster.transform;
        }

        //CLEARING LISTENERS
        for (int i = 0; i < skillParent.Length; i++)
        {
            foreach (Transform T in skillParent[i])
            {
                if (network_ID == 1)
                {
                    T.SetParent(UIManager.Instance.Player1_SkillsRoster.transform);
                }
                if (network_ID == 2)
                {
                    T.SetParent(UIManager.Instance.Player2_SkillsRoster.transform);
                }
            }
        }
        

        foreach (Transform T in skillRoster)
        {
            if (T.gameObject.name == TronGameManager.Instance.selected_currentSkill_Text[0].text)
            {
                SetThisSkillButton(0, T, skillParent[0]);
                break;
            }
        }
        foreach (Transform T in skillRoster)
        {
            if (T.gameObject.name == TronGameManager.Instance.selected_currentSkill_Text[1].text)
            {
                SetThisSkillButton(1, T, skillParent[1]);
                break;
            }
        }
    }

    void SetThisSkillButton(int _var, Transform T, Transform SkillParent)
    {
        T.GetComponent<Button>().onClick.RemoveListener(() => CDThisSkillSlot(0, T.gameObject));
        T.GetComponent<Button>().onClick.RemoveListener(() => CDThisSkillSlot(1, T.gameObject));
        T.GetComponent<Button>().onClick.RemoveAllListeners();

        T.GetComponent<Button>().onClick.AddListener(() => CDThisSkillSlot(_var, T.gameObject));
        CooldownName[_var] = T.name;
        ButtonObject[_var] = T;
        coolDown_Timer[_var] = 0;
        CooldownCap[_var] = CheckCoolDownCap(TronGameManager.Instance.selected_currentSkill_Text[_var].text);

        UIManager.Instance.StartCooldDownForBlockers(network_ID, _var, CooldownCap[_var], CooldownCap[_var], CooldownName[_var]);
        T.SetParent(SkillParent);
        T.localScale = Vector3.one;
        T.localPosition = Vector3.zero;
        T.gameObject.SetActive(true);
    }

    float CheckCoolDownCap(string _val)
    {
        SKILL_LIST test = (SKILL_LIST)Enum.Parse(typeof(SKILL_LIST), _val);
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
    void CDThisSkillSlot(int _val, GameObject _obj)
    {
        coolDown_Switch[_val] = true;
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
  

    void FixedUpdate()
    {
        for (int q = 0; q < coolDown_Switch.Length; q++)
        {
            if (coolDown_Switch[q] == true)
            {
                if (coolDown_Timer[q] < CooldownCap[q])
                {
                    UIManager.Instance.StartCooldDownForBlockers(network_ID, q, coolDown_Timer[q], CooldownCap[q],CooldownName[q]);
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
    void Process_Ready()
    {
        _carMovement.SetReady(true);
        UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: SUCCESSFULLY READY THIS PLAYER: " + network_ID;

        UIManager.Instance.GameUpdateText.text += "\n\t\t" + TronGameManager.Instance.PlayerObjects[0].GetComponent<Car_Movement>().GetReady() + " - "
                                                         + TronGameManager.Instance.PlayerObjects[1].GetComponent<Car_Movement>().GetReady();

        //2 PLAYERS READY
        if (TronGameManager.Instance.PlayerObjects[0].GetComponent<Car_Movement>().GetReady()
            && TronGameManager.Instance.PlayerObjects[1].GetComponent<Car_Movement>().GetReady())
        {
            GameSparkPacketHandler.Instance.Access_SentStartToServer();
        }
        else
        {
            UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: FAILED TO READY THIS PLAYER_____ RETRYNG  CODE 000";
            GameSparkPacketHandler.Instance.Access_SentReadyToServer();

            //TO RETURN
            /*
            StopCoroutine("delayRestartReady");
            StartCoroutine(delayRestartReady(_switch, _netStatus));
            UIManager.Instance.GameUpdateText.text += "\n\tBoth players are NOT ready, tryng again";
            */
        }
    }
    void Process_Start()
    {
        
        if(_carMovement.GetReady() == false)
        {
            UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: FAILED TO START THIS PLAYER_____ RETRYNG CODE 001";
            //GameSparkPacketHandler.Instance.Access_SentReadyToServer();
            GameSparkPacketHandler.Instance.Access_SentStartToServer();
            return;
        }
        try
        {
            _carMovement.SetStartGame(true);
            if (network_ID == 2)
            {
                UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: SUCCESSFULLY START THIS PLAYER";
                UIManager.Instance.GameUpdateText.text += "\n\t\t" + TronGameManager.Instance.PlayerObjects[0].GetComponent<Car_Movement>().GetReady() + " - "
                                                                 + TronGameManager.Instance.PlayerObjects[1].GetComponent<Car_Movement>().GetReady();
                if (TronGameManager.Instance.PlayerObjects[0].GetComponent<Car_Movement>().GetReady() != TronGameManager.Instance.PlayerObjects[1].GetComponent<Car_Movement>().GetReady())
                {
                    UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: READY PHASE HAS BEEN SKIPEED";
                }
            }
            //UIManager.Instance.Set_Canvas_Waiting(false);
        }
        catch
        {
            UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: FAILED TO START THIS PLAYER_____ RETRYNG  CODE 002";
            GameSparkPacketHandler.Instance.Access_SentReadyToServer();

            //TO RETURN
            /*
                StopCoroutine("delayRestartReady");
                StartCoroutine(delayRestartReady(_switch, _netStatus));
                UIManager.Instance.GameUpdateText.text += "\n\tCAR_RECEIVER: FAILED TO START, RETRYING";
                */
        }
    }
    void StartGame(bool _switch)
    {
        if (network_ID == gameSparksPacketHandler.GetPeerID())
        {
            _carMovement.SetStartGame(_switch);
        }
    }
}
