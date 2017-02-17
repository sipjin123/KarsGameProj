﻿using GameSparks.RT;
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

    public float Health;
    public Image HealthBar;
    public Text hp_indicator;
    #endregion
    //================================================================================================================================
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

            _carMovement.StartGame = true;
            ResetTrail(true);
            Health = 5;
        }
    }
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



            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                _shieldSwitch = true;
                _shieldObject.SetActive(true);
                ActivatePowerup();
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                _shieldSwitch = false;
                _shieldObject.SetActive(false);
                ActivatePowerup();
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
        HealthBar.fillAmount = Health / 5;
        hp_indicator.text = Health.ToString();
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
            data.SetInt(2, _switch == true ? 2:1);

            GetRTSession.SendData(122, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
        //SwitchInterpolation(_switch == true ? 0: 2);
    }

    void SwitchInterpolation(int _bool)
    {
        /*
        if (_bool == 0)
        {
            StartCoroutine("DelayExtrapolation");
            return;
        }
        */
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            data.SetInt(2, _bool);

            GetRTSession.SendData(123, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
    IEnumerator DelayExtrapolation()
    {

        yield return new WaitForSeconds(.1f);
        SendCarMovement(Network_ID, _objToTranslate.position, _objToRotate.eulerAngles);
        yield return new WaitForSeconds(.1f);
        SendCarMovement(Network_ID, _objToTranslate.position, _objToRotate.eulerAngles);
        yield return new WaitForSeconds(.1f);
        SendCarMovement(Network_ID, _objToTranslate.position, _objToRotate.eulerAngles);


        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            data.SetInt(2, 0);

            GetRTSession.SendData(122, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
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
    public void ReceiveBufferState(RTPacket _packet)
    {
        


        try
        {
            if (_packet.Data.GetDouble(7).Value == m_BufferedState[0].timestamp )
                return;

            // Shift buffer contents, oldest data erased, 18 becomes 19, ... , 0 becomes 1
            for (int q = m_BufferedState.Length - 1; q >= 1; q--)
            {
                m_BufferedState[q] = m_BufferedState[q - 1];
            }

            // Save currect received state as 0 in the buffer, safe to overwrite after shifting
            State state;
            state.pos = new Vector3(_packet.Data.GetFloat(2).Value, _packet.Data.GetFloat(3).Value, _packet.Data.GetFloat(4).Value);

            state.rot = _packet.Data.GetVector3(5).Value;
            state.timestamp = _packet.Data.GetDouble(7).Value;
            m_BufferedState[0] = state;
            
            PlayerPing = _gameSparkPacketReceiver.gameTimeInt - state.timestamp;
            _gameSparkPacketReceiver.PlayerPingText.text = PlayerPing.ToString();
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
    //INTERPOLATION EXTRAPOLATION
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
                GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n\nINTERPOLATE\n";
                Interpolate();
            }
            else
            {
                InterpolateObj.SetActive(false);
                ExtrapoalteObj.SetActive(true);
                GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nEXTRAPOLATE";
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

                GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nT: " + t + "=" + (interpolationTime - lhs.timestamp) + "(" + interpolationTime + "-" + lhs.timestamp + ")/" +length +"("+rhs.timestamp+"-"+lhs.timestamp+")";

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

    public bool _shieldSwitch;
    public GameObject _shieldObject;
    public void ReceivePowerUpState(bool _switch)
    {
        _shieldObject.SetActive(_switch);
        _shieldSwitch = _switch;
    }
    void ActivatePowerup()
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            if (_shieldSwitch)
                data.SetInt(2, 1);
            else
                data.SetInt(2, 0);

            GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }

    public void ActiveShieldFromButton()
    {
        _shieldSwitch = !_shieldSwitch;
        _shieldObject.SetActive(_shieldSwitch);
        ActivatePowerup();
    }
    public Text PingText;
}
