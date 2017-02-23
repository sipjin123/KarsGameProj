using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car_DataReceiver : MonoBehaviour {
    //================================================================================================================================
    #region VARIABLES
    [SerializeField]
    Transform _objToTranslate, _objToRotate;
    float rotSpeed = .1f;
    [SerializeField]
    public int Network_ID;
    [SerializeField]
    private bool ifMy_Network_Player;
    [SerializeField]
    private GameSparksRTUnity GetRTSession;

    [SerializeField]
    private Camera NetworkCam;

    [SerializeField]
    private GameSparkPacketReceiver _gameSparkPacketReceiver;
    [SerializeField]
    private Car_Movement _carMovement;

    private bool ShieldSwitch;
    public bool GetShieldSwitch(){ return ShieldSwitch; }
    public GameObject ShieldObject;


    private bool StunSwitch;
    public bool GetStunSwitch() { return StunSwitch; }
    public GameObject StunObject;

    public float Health;
    #endregion
    //================================================================================================================================
    #region NETWORK INIT
    public void SetNetworkObject(int netID)
    {
        Network_ID = netID;
    }

    public void InitCam()
    {
        _gameSparkPacketReceiver = GameSparkPacketReceiver.Instance.GetComponent<GameSparkPacketReceiver>();
        GetRTSession = _gameSparkPacketReceiver.GetRTSession();
        _carMovement = GetComponent<Car_Movement>();
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
        if(Input.GetKeyDown(KeyCode.Alpha5))
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
        if(Network_ID == 1)
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
            data.SetInt(2, _switch == true ? 1:0);
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
    #region STATE_UPDATER
    public struct State
    {
        internal double timestamp;
        internal Vector3 pos;
        internal Vector3 rot;
    }

    public State[] m_BufferedState = new State[20];
    public int m_TimestampCount;
    public void ReceiveBufferState(double _receivedTimeStamp, Vector3 _receivedPos, Vector3 _receivedRot)//RTPacket _packet)
    {

        try
        {

            if (_receivedTimeStamp == m_BufferedState[0].timestamp)// if (_packet.Data.GetDouble(7).Value == m_BufferedState[0].timestamp )
                return;

            // Shift buffer contents, oldest data erased, 18 becomes 19, ... , 0 becomes 1
            for (int q = m_BufferedState.Length - 1; q >= 1; q--)
            {
                m_BufferedState[q] = m_BufferedState[q - 1];
            }

            // Save currect received state as 0 in the buffer, safe to overwrite after shifting
            State state;
            state.pos = _receivedPos; //state.pos = new Vector3(_packet.Data.GetFloat(2).Value, _packet.Data.GetFloat(3).Value, _packet.Data.GetFloat(4).Value);

            state.rot = _receivedRot;//state.rot = _packet.Data.GetVector3(5).Value;
            state.timestamp = _receivedTimeStamp;//state.timestamp = _packet.Data.GetDouble(7).Value;
            m_BufferedState[0] = state;
            
            PlayerPing = _gameSparkPacketReceiver.gameTimeInt - state.timestamp;
            UIManager.instance.PingText.text = PlayerPing.ToString();
            // Increment state count but never exceed buffer size
            m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

            // Check integrity, lowest numbered state in the buffer is newest and so on
            for (int w = 0; w < m_TimestampCount - 1; w++)
            {
                if (m_BufferedState[w].timestamp < m_BufferedState[w + 1].timestamp)
                    Debug.Log("State inconsistent");
            }
        }
        catch
        {
            Debug.LogError("CANNOT UPDATE STATE");
        }
    }
    #endregion
    //================================================================================================================================
    //
    //                                                       EXTRAPOLATION/INTERPOLATION
    //
    //================================================================================================================================
    #region INTERPOLATION EXTRAPOLATION
    double PlayerPing;
    double interpolationTime;

    public GameObject InterpolateObj, ExtrapoalteObj;

    void UpdateFunctInterpolate()
    {
        try
        {
            if (GameObject.Find("GameUpdateText").GetComponent<Text>().text.Length > 3000)
            {
                GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
            }
        }
        catch { }

        try
        {
            double currentTime = _gameSparkPacketReceiver.gameTimeInt;
            interpolationTime = 0;

            //REFACTOR GAME TIME
            //interpolationTime = currentTime - 0.1f;
            /*
            if (!_sparksManager.fixedInterTime)
            {
                //REFACTOR GAME TIME
                interpolationTime = currentTime;
                //interpolationTime = currentTime - (PlayerPing + _sparksManager.playerPingOffset);
            }
            else
                interpolationTime = currentTime - 0.2f;
            */

            /*
            _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, m_BufferedState[0].pos, .8f);
            _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(m_BufferedState[0].rot), .8f);
            return;*/

            /*
            interpolationTime = currentTime - (PlayerPing+_gameSparkPacketReceiver.playerPingOffset);
            GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nIT: " + interpolationTime + "=" + currentTime + "-" + (PlayerPing + _gameSparkPacketReceiver.playerPingOffset) + "(" + PlayerPing + "+" + _gameSparkPacketReceiver.playerPingOffset;

            Interpolate();
            return;
            */

            //if (m_BufferedState[0].timestamp > interpolationTime)
            

            if( Mathf.Abs(Vector3.Distance( m_BufferedState[0].pos, m_BufferedState[1].pos )) > 10)
            {
                interpolationTime = currentTime - (PlayerPing+ _gameSparkPacketReceiver.playerPingOffset);

                InterpolateObj.SetActive(true);
                ExtrapoalteObj.SetActive(false);

                /*
                GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nInterpolate Time: ("+interpolationTime.ToString("F2")+ ") BufferTime[0]: ("+m_BufferedState[0].timestamp.ToString("F2")+")\n"+
                    //+currentTime+" - "+"(( "+PlayerPing+" + "+1000+" ) = "+(PlayerPing + 1000) + ")\n"; 
                +currentTime + " - " + "(( " + PlayerPing + " + " + _sparksManager.playerPingOffset.ToString("F2") + " ) = " + (PlayerPing + _sparksManager.playerPingOffset).ToString("F2")+")";


                if (GameObject.Find("GameUpdateText").GetComponent<Text>().text.Length > 3000)
                {
                    GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
                }
                */
                //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n\nINTERPOLATE\n";
                Interpolate();
            }
            else
            {
                InterpolateObj.SetActive(false);
                ExtrapoalteObj.SetActive(true);
                //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nEXTRAPOLATE";
                Extrapolate();
            }
        }
        catch
        {
        }
    }
    void Interpolate()
    {
        for (int i = 0; i < m_TimestampCount; i++)
        {
            if (m_BufferedState[i].timestamp <= interpolationTime || i == m_TimestampCount - 1)
            {
                // The state one slot newer (<100ms) than the best playback state
                State rhs = m_BufferedState[Mathf.Max(i - 1, 0)];
                // The best playback state (closest to 100 ms old (default time))
                State lhs = m_BufferedState[i];

                // Use the time between the two slots to determine if interpolation is necessary

                double length = rhs.timestamp - lhs.timestamp;
                float t = 0.0F;
                // As the time difference gets closer to 100 ms t gets closer to 1 in 
                // which case rhs is only used
                if (length > 0.1)
                {
                    t = (float)((interpolationTime - lhs.timestamp) / length);
                }
                // if t=0 => lhs is used directly
                t = 1;

                //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nT: " + t + "=" + (interpolationTime - lhs.timestamp) + "(" + interpolationTime + "-" + lhs.timestamp + ")/" +length +"("+rhs.timestamp+"-"+lhs.timestamp+")";

                if (_gameSparkPacketReceiver._curMethod == GameSparkPacketReceiver.MethodUsed.LINEAR)
                {
                    _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, lhs.pos, t);
                    _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(lhs.rot), rotSpeed);
                    Debug.LogWarning("DOING LINEAR");
                }
            }
        }
    }
    void Extrapolate()
    {
        double currentTime = _gameSparkPacketReceiver.gameTimeInt;
        interpolationTime = currentTime - 0.1f;

        State latest = m_BufferedState[0];
        if (_gameSparkPacketReceiver._curMethod == GameSparkPacketReceiver.MethodUsed.LINEAR)
        {
            double timeDiff = 0;
            timeDiff = (m_BufferedState[0].timestamp - m_BufferedState[1].timestamp);
            if (timeDiff == 0)
            {
                timeDiff += 0.1f;
                // GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nZERO VAL: (" + m_BufferedState[0].pos.x+" : "+ m_BufferedState[0].pos.z + ") " + m_BufferedState[0].timestamp + "\n";
                // GameObject.Find("GameUpdateText").GetComponent<Text>().text += "ZERO VAL:(" + m_BufferedState[1].pos.x + " : " + m_BufferedState[1].pos.z + ") " + m_BufferedState[1].timestamp + "\n";
            }
            
            Vector3 SPEED = (m_BufferedState[0].pos - m_BufferedState[1].pos) / (float)timeDiff;
            double ELAPSED_TIME = interpolationTime - m_BufferedState[0].timestamp;
            Vector3 NEW_POS = m_BufferedState[0].pos + (SPEED * (float)ELAPSED_TIME);

            if(Vector3.Distance(NEW_POS,m_BufferedState[0].pos) >55)
            {
                //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nExceed: " + (Vector3.Distance(NEW_POS, m_BufferedState[0].pos) );
                if (GameObject.Find("GameUpdateText").GetComponent<Text>().text.Length > 3000)
                {
                    GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
                }
                GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nExceed WILL ITNERPOALTE: " + (Vector3.Distance(NEW_POS, m_BufferedState[0].pos));
                Interpolate();
                return;
            }


            float lerpSpeed = .9f;
            _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, NEW_POS, lerpSpeed);
             lerpSpeed = .8f;
            _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(latest.rot), lerpSpeed);
        }
    }
    #endregion
    //================================================================================================================================
    public void SetCarAvatar(int _avatarNumber)
    {
        for(int i = 0; i < AvatarList.Length; i++)
        {
            AvatarList[i].SetActive(false);
        }

        AvatarList[_avatarNumber].SetActive(true);
    }
    public GameObject[] AvatarList;

    private bool BlindSwitch;
    public bool GetBlindSwitch() { return BlindSwitch; }
    public GameObject BlindObjectBlocker;
    public GameObject BlindObject;

    private bool ConfuseSwitch;
    public GameObject ConfuseObject;
    


    //STUN
    //-------------------------------------------
    public void ActiveStunFromButton()
    {
        if (StunSwitch == false)
        {
            StunSwitch = true;
            StunObject.SetActive(StunSwitch);
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


    //-------------------------------------------
    //CONFUSE
    #region CONFUSE FEATURE
    public void ActiveConfuseFromButton()
    {
        if (ConfuseSwitch == false)
        {
            ConfuseSwitch = true;
            ConfuseObject.SetActive(ConfuseSwitch);

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

            SendNetworkDisable(BlindSwitch, NetworkPlayerStatus.ACTIVATE_BLIND);
            StartCoroutine("StartBlindTimer");
        }
    }
    IEnumerator StartBlindTimer()
    {
        yield return new WaitForSeconds(TronGameManager.Instance.BlindDuration);
        ReceiveDisableSTate(false, NetworkPlayerStatus.ACTIVATE_BLIND);
        SendNetworkDisable(BlindSwitch,NetworkPlayerStatus.ACTIVATE_BLIND);
    }
    #endregion
    //-------------------------------------------
    private void SendNetworkDisable(bool _switch , NetworkPlayerStatus _status)
    {
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
}
