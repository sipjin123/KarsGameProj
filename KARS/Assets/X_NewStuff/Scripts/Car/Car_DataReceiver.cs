using GameSparks.RT;
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

    //NETWORK STUFF
    [SerializeField]
    public int Network_ID;
    [SerializeField]
    private bool ifMy_Network_Player;

    [SerializeField]
    private Camera NetworkCam;



    //POWER UP AND DEBUFF

    //SHIELD---------
    private bool ShieldSwitch;
    public bool GetShieldSwitch() { return ShieldSwitch; }
    public GameObject ShieldObject;


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

    public float Health;
    public GameObject[] AvatarList;
    #endregion
    //================================================================================================================================
    #region NETWORK INIT
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
            NetworkCam.enabled = true;
            ifMy_Network_Player = true;

            ResetTrail(true);
            Health = 5;

            if (Network_ID == 1)
            {
                UIManager.instance.Player1Panel.SetActive(true);
            }
            else if (Network_ID == 2)
            {
                UIManager.instance.Player2Panel.SetActive(true);
            }
        }
    }
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
        if (!ifMy_Network_Player)
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
    void Test_Input()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
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
    //================================================================================================================================
    //
    //                                                       SEND DATA
    //
    //================================================================================================================================
    #region DATA_SENDING
    public void SendCarMovement(int _id, Vector3 _pos, Vector3 _rot)
    {
        if(TronGameManager.Instance.NetworkStart)
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, _id);
            data.SetFloat(2, _pos.x);
            data.SetFloat(3, _pos.y);
            data.SetFloat(4, _pos.z);
            data.SetVector3(5, _rot);
            data.SetDouble(6, Network.time);
            data.SetDouble(7, _gameSparkPacketReceiver.gameTimeInt);


            GetRTSession.SendData(111, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    } 

    public void ReduceHealth()
    {
        Health -= 1;
        if (Network_ID == 1)
        {
            UIManager.instance.HealthBar_1.fillAmount = Health / 5;
            UIManager.instance.HealthText_1.text = Health.ToString();
        }
        else if (Network_ID == 2)
        {
            UIManager.instance.HealthBar_2.fillAmount = Health / 5;
            UIManager.instance.HealthText_2.text = Health.ToString();
        }
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            data.SetFloat(2, Health);
            GetRTSession.SendData(118, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }

    public void ResetTrail(bool _switch)
    {
        _carMovement._trailCollision.SetEmiision(_switch);

        if(TronGameManager.Instance.NetworkStart == true)
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            data.SetInt(2, _switch == true ? 1 : 0);
            data.SetInt(3, (int)NetworkPlayerStatus.ACTIVATE_TRAIL);

            GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }


    //SHIELD
    //-------------------------------------------
    public void ActiveShieldFromButton()
    {
        ShieldSwitch = !ShieldSwitch;
        ShieldObject.SetActive(ShieldSwitch);
        SendNetworkPowerUp(NetworkPlayerStatus.ACTIVATE_SHIELD);
    }
    private void SendNetworkPowerUp(NetworkPlayerStatus _status)
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            if (ShieldSwitch)
                data.SetInt(2, 1);
            else
                data.SetInt(2, 0);

            data.SetInt(3, (int)_status);
            GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
    public void ReceivePowerUpState(bool _switch, NetworkPlayerStatus _netStatus)
    {
        if (_netStatus == NetworkPlayerStatus.ACTIVATE_SHIELD)
        {
            ShieldObject.SetActive(_switch);
            ShieldSwitch = _switch;
        }
    }
    #endregion
    //================================================================================================================================
    //
    //                                                       RECEIVE DATA
    //
    //================================================================================================================================


    public void Activate_StateFromButton(NetworkPlayerStatus _status)
    {
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
        }
    }

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
            _carMovement.FlipSwitch = _switch;
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
        if (_netStatus == NetworkPlayerStatus.SET_READY)
        {
            _carMovement.SetReady(_switch);
        }
        if (_netStatus == NetworkPlayerStatus.SET_START)
        {
            _carMovement.SetStartGame(_switch);
        }
    }

    //-------------------------------------------
    private void SendNetworkDisable(bool _switch, NetworkPlayerStatus _status)
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
            GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
    public void SetCarAvatar(int _avatarNumber)
    {
        for (int i = 0; i < AvatarList.Length; i++)
        {
            AvatarList[i].SetActive(false);
        }

        AvatarList[_avatarNumber].SetActive(true);
    }

    private float _TrailValue=5;
    private float _TrailValueDividend = 5;
    public float TrailValue()
    {
        return _TrailValue;
    }
    public float TrailValueDividend()
    {
        return _TrailValueDividend;
    }
    public void ReceiveTrailVAlue(float _trailValue, float _trailValueDividend)
    {
        _TrailValue = _trailValue;
        _TrailValueDividend = _trailValueDividend;
    }
}
