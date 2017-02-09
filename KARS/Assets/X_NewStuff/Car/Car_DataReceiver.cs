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

    public float Health;
    public Image HealthBar;
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
            Health = 100;
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
    public void ResetTrail(bool _switch)
    {
        if (!_switch)
        {
            Health -= 20;
            HealthBar.fillAmount = Health / 100;

            using (RTData data = RTData.Get())
            {
                data.SetInt(1, Network_ID);
                data.SetFloat(2, Health);
                GetRTSession.SendData(118, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }

        _carMovement._trailCollision.SetEmiision(_switch);
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, Network_ID);
            data.SetInt(2, _switch == true ? 2:1);

            GetRTSession.SendData(122, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
        SwitchInterpolation(_switch == true ? 0: 2);
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
            double currentTime = _gameSparkPacketReceiver.gameTimeInt;
            interpolationTime = 0;

            //REFACTOR GAME TIME
            interpolationTime = currentTime - 0.2f;
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
            
            Extrapolate();
            
            return;
            if (m_BufferedState[0].timestamp > interpolationTime)
            {
                interpolationTime = currentTime - (PlayerPing + _gameSparkPacketReceiver.playerPingOffset);
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
                Interpolate();
            }
            else
            {
                InterpolateObj.SetActive(false);
                ExtrapoalteObj.SetActive(true);
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
                if (length > 0.0001)
                {
                    t = (float)((interpolationTime - lhs.timestamp) / length);
                }
                // if t=0 => lhs is used directly
                
                if (_gameSparkPacketReceiver._curMethod == GameSparkPacketReceiver.MethodUsed.LINEAR)
                {
                    _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, lhs.pos, t);
                    _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(lhs.rot), rotSpeed);
                    Debug.LogWarning("DOING LINEAR");
                }
                else if (_gameSparkPacketReceiver._curMethod == GameSparkPacketReceiver.MethodUsed.CUBIC)
                {
                    Vector3 newposs = CubicInterpolate(m_BufferedState[0].pos, m_BufferedState[1].pos, m_BufferedState[2].pos, m_BufferedState[3].pos, t);

                    _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, newposs, t);
                    _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(lhs.rot), rotSpeed);
                    Debug.LogWarning("DOING CUBIC");
                }
                else
                {
                    _objToTranslate.transform.position = m_BufferedState[0].pos;
                    _objToRotate.transform.rotation = Quaternion.Euler(m_BufferedState[0].rot);
                    Debug.LogWarning("DOING INSTANT");
                }
                return;
            }
        }
    }
    void Extrapolate()
    {
        State latest = m_BufferedState[0];
        float lerpSpeed = 1;
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

            _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, NEW_POS, lerpSpeed);
            _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(latest.rot), lerpSpeed);
            Debug.Log("DOING LINEAR");
        }
        /*
        else if (_sparksManager._curMethod == GameSparksManager.CurrentMethod.CUBIC)
        {
            Vector3 newposs = CubicInterpolate(m_BufferedState[0].pos, m_BufferedState[1].pos, m_BufferedState[2].pos, m_BufferedState[3].pos, lerpSpeed);

            _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, newposs, lerpSpeed);
            _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(latest.rot), lerpSpeed);
            Debug.Log("DOING CUBIC");
        }*/
        else
        {
            _objToTranslate.transform.position = m_BufferedState[0].pos;
            _objToRotate.transform.rotation = Quaternion.Euler(m_BufferedState[0].rot);
            Debug.Log("DOING INSTANT");
        }
    }

    Vector3 CubicInterpolate(Vector3 y0, Vector3 y1, Vector3 y2, Vector3 y3, float mu)
    {
        Vector3 a0, a1, a2, a3;
        float mu2;

        mu2 = mu * mu;
        a0 = y3 - y2 - y0 + y1;
        a1 = y0 - y1 - a0;
        a2 = y2 - y0;
        a3 = y1;
        return (a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3);
    }

    void OnGUI()
    {
        try
        {
            if (Network_ID == RegisterGameSpark.Instance.PeerID)
                    GUI.Box(new Rect(Screen.width - 100, Screen.height - 30, 100, 30), "" + Network_ID);
        }
        catch
        { }
    }

    #endregion
    //================================================================================================================================
}
