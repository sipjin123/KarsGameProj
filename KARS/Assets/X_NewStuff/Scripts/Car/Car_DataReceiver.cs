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
    private bool ShieldSwitch;
    public bool GetShieldSwitch() { return ShieldSwitch; }
    public GameObject ShieldObject;


    private bool StunSwitch;
    public bool GetStunSwitch() { return StunSwitch; }
    public GameObject StunObject;

    private bool BlindSwitch;
    public bool GetBlindSwitch() { return BlindSwitch; }
    public GameObject BlindObjectBlocker;
    public GameObject BlindObject;

    private bool ConfuseSwitch;
    public GameObject ConfuseObject;

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
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ActiveStunFromButton();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ShieldSwitch = true;
            ShieldObject.SetActive(true);
            SendNetworkPowerUp();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ShieldSwitch = false;
            ShieldObject.SetActive(false);
            SendNetworkPowerUp();
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
        SendNetworkPowerUp();
    }
    private void SendNetworkPowerUp()
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            if (ShieldSwitch)
                data.SetInt(2, 1);
            else
                data.SetInt(2, 0);

            data.SetInt(3, (int)NetworkPlayerStatus.ACTIVATE_SHIELD);
            GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
    public void ReceivePowerUpState(bool _switch)
    {
        ShieldObject.SetActive(_switch);
        ShieldSwitch = _switch;
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
        if (_netStatus == NetworkPlayerStatus.SET_READY)
        {
            _carMovement.SetReady(_switch);
        }
        if (_netStatus == NetworkPlayerStatus.SET_START)
        {
            _carMovement.SetStartGame(_switch);
        }
    }
    #endregion
    //================================================================================================================================
    //
    //                                                       RECEIVE DATA
    //
    //================================================================================================================================


    //STUN
    //-------------------------------------------
    #region STUN FEATURE
    public void ActiveStunFromButton()
    {
        if (StunSwitch == false)
        {
            StunSwitch = true;
            StunObject.SetActive(StunSwitch);

            if (TronGameManager.Instance.NetworkStart == true)
                SendNetworkDisable(StunSwitch, NetworkPlayerStatus.ACTIVATE_STUN);
            StartCoroutine("StartStunTimer");
        }
    }
    IEnumerator StartStunTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.const_StunDuration);
        ReceiveDisableSTate(false, NetworkPlayerStatus.ACTIVATE_STUN);

        SendNetworkDisable(StunSwitch, NetworkPlayerStatus.ACTIVATE_STUN);
    }
    #endregion
    //-------------------------------------------
    //CONFUSE
    #region CONFUSE FEATURE
    public void ActiveConfuseFromButton()
    {
        if (ConfuseSwitch == false)
        {
            ConfuseSwitch = true;
            ConfuseObject.SetActive(ConfuseSwitch);

            if (TronGameManager.Instance.NetworkStart == true)
                SendNetworkDisable(ConfuseSwitch, NetworkPlayerStatus.ACTIVATE_CONFUSE);
            StartCoroutine("StartConfuseTimer");
        }
    }
    IEnumerator StartConfuseTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.ConfuseDuration);
        ReceiveDisableSTate(false, NetworkPlayerStatus.ACTIVATE_CONFUSE);
        SendNetworkDisable(ConfuseSwitch, NetworkPlayerStatus.ACTIVATE_CONFUSE);
    }
    #endregion
    //-------------------------------------------
    //BLIND
    #region BLIND FEATURE
    public void ActiveBlindFromButton()
    {
        if (BlindSwitch == false)
        {
            BlindSwitch = true;
            BlindObjectBlocker.SetActive(BlindSwitch);
            BlindObject.SetActive(BlindSwitch);

            if(TronGameManager.Instance.NetworkStart == true)
            SendNetworkDisable(BlindSwitch, NetworkPlayerStatus.ACTIVATE_BLIND);
            StartCoroutine("StartBlindTimer");
        }
    }
    IEnumerator StartBlindTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.BlindDuration);
        ReceiveDisableSTate(false, NetworkPlayerStatus.ACTIVATE_BLIND);
        SendNetworkDisable(BlindSwitch, NetworkPlayerStatus.ACTIVATE_BLIND);
    }
    #endregion
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
