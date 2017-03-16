using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car_Network_Interpolation : MonoBehaviour {

    #region INTERPOLATION EXTRAPOLATION
    double PlayerPing;
    double interpolationTime;

    [SerializeField]
    protected GameObject InterpolateObj, ExtrapoalteObj;

    float rotSpeed = .1f;
    [SerializeField]
    protected Transform _objToTranslate, _objToRotate;
    protected GameSparkPacketHandler gameSparksPacketHandler;
    #region STATE_UPDATER
    public struct State
    {
        internal double timestamp;
        internal Vector3 pos;
        internal Vector3 rot;
    }

    public void ClearBufferState()
    {
        m_BufferedState = new State[20];
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

            PlayerPing = gameSparksPacketHandler.GetGameClockINT() - state.timestamp;
            UIManager.Instance.PingText.text = PlayerPing.ToString();
            // Increment state count but never exceed buffer size
            m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);

            // Check integrity, lowest numbered state in the buffer is newest and so on
            for (int w = 0; w < m_TimestampCount - 1; w++)
            {
                if (m_BufferedState[w].timestamp < m_BufferedState[w + 1].timestamp)
                {
                    //Debug.Log("State inconsistent");
                }
            }
        }
        catch
        {
            //Debug.LogError("CANNOT UPDATE STATE");
        }
    }
    #endregion
    protected void UpdateFunctInterpolate()
    {
        try
        {
        }
        catch { }

        try
        {
            double currentTime = gameSparksPacketHandler.GetGameClockINT();
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


            if (Mathf.Abs(Vector3.Distance(m_BufferedState[0].pos, m_BufferedState[1].pos)) > 10)
            {
                interpolationTime = currentTime - (PlayerPing + gameSparksPacketHandler.playerPingOffset);

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

                if (gameSparksPacketHandler._curMethod == MethodUsed.LINEAR)
                {
                    _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, lhs.pos, t);
                    _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(lhs.rot), rotSpeed);
                    //Debug.LogWarning("DOING LINEAR");
                }
            }
        }
    }
    void Extrapolate()
    {
        double currentTime = gameSparksPacketHandler.GetGameClockINT();
        interpolationTime = currentTime - 0.1f;

        State latest = m_BufferedState[0];
        if (gameSparksPacketHandler._curMethod == MethodUsed.LINEAR)
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

            if (Vector3.Distance(NEW_POS, m_BufferedState[0].pos) > 55)
            {
                //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nExceed: " + (Vector3.Distance(NEW_POS, m_BufferedState[0].pos) );
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

}
