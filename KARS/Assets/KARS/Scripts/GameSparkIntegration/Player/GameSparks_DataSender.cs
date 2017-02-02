using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System;
using GameSparks.RT;
using UnityEngine.UI;

public class GameSparks_DataSender : MonoBehaviour
{
    //=====================================================================================================================
    #region VARIABLES
    //NETWORK RELATED
    //-----------------------------------------------------
    [SerializeField]
    private string DisplayName;

    private GameSparksManager _sparksManager;
    private GameSparksRTUnity GetRTSession;

    [SerializeField]
    private Camera playerCam;
    public Camera PlayerCam
    {
        get   {   return playerCam;  }
        set   {   playerCam = value; }
    }
    private bool hasControllableObject;
    public bool HasControllableObject
    {
        get  {  return hasControllableObject;    }
        set  {  hasControllableObject = value; }
    }
    private GameObject _objToTranslate;
    public GameObject ObjToTranslate
    {
        get {   return _objToTranslate;  }
        set {   _objToTranslate = value; }
    }
    private GameObject _objToRotate;
    public GameObject ObjToRotate
    {
        get {   return _objToRotate;   }
        set {  _objToRotate = value;   }
    }
    [SerializeField]
    private int _networkID;
    public int NetworkID
    {
        get {  return _networkID;  }
        set {  _networkID = value;  }
    }
    //-----------------------------------------------------
    [SerializeField]
     GameObject _shieldObject;
    public bool _shieldSwitch;

    float rotSpeed = 1;
    #endregion
    //=====================================================================================================================
    #region INITIALIZATION
    void Start()
    {
        _sparksManager = GameSparksManager.Instance.GetComponent<GameSparksManager>();
        GetRTSession = GameSparksManager.Instance.GetRTSession();

        InterpolateObj =  GameObject.CreatePrimitive(PrimitiveType.Sphere);
        InterpolateObj.transform.SetParent(transform);
        InterpolateObj.transform.localScale = new Vector3(2,0.1f,3);
        InterpolateObj.transform.transform.localPosition = new Vector3(0, -0.55f, 0.75f);
        InterpolateObj.GetComponent<MeshRenderer>().material.color = Color.red;
        Destroy(InterpolateObj.GetComponent<SphereCollider>());
        InterpolateObj.gameObject.layer = LayerMask.NameToLayer("3D");

        ExtrapoalteObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ExtrapoalteObj.transform.SetParent(transform);
        ExtrapoalteObj.transform.localScale = new Vector3(5, 0.1f, 2);
        ExtrapoalteObj.transform.transform.localPosition = new Vector3(0,- 0.55f, 0.75f);
        ExtrapoalteObj.GetComponent<MeshRenderer>().material.color = Color.blue;
        Destroy(ExtrapoalteObj.GetComponent<SphereCollider>());
        ExtrapoalteObj.gameObject.layer = LayerMask.NameToLayer("3D");


        _shieldObject = Instantiate(PowerUpManager.Instance.Shield, Vector3.one, Quaternion.identity) as GameObject;
        _shieldObject.transform.SetParent(transform);
        _shieldObject.transform.localPosition = Vector3.zero;
        _shieldObject.transform.localEulerAngles = Vector3.zero;

        _shieldObject.gameObject.name = "Shield";
        _shieldObject.SetActive(false);
    }
    public void setWhatToControl()
    {
        DisplayName = "Player "+ NetworkID;
        hasControllableObject = true;

        if (_sparksManager.PeerID == _networkID.ToString())
        {
            playerCam.enabled = true;
        }
        else
        {
            StartCoroutine(Delaycamera());
        }
    }
    IEnumerator Delaycamera()
    {
        yield return new WaitForSeconds(1);
        {
            setWhatToControl();
        }
    }
    #endregion
    //=====================================================================================================================
    //===============================================================================================================================================================================================
    void Update()
    {
        if (_sparksManager.PeerID == NetworkID.ToString())
        {
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
        }
        if (!hasControllableObject)
           return;






        if (_sparksManager.PeerID != NetworkID.ToString())
        {
            UpdateFunctInterpolate();
        }
    }
    //===============================================================================================================================================================================================
    //=====================================================================================================================
    //SEND DATA
    public void SendHealth(float hp)
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, NetworkID);
            data.SetFloat(2, hp);

            GetRTSession.SendData(118, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
    
    public void SendInteractStatus(int playerId, int isBumped, int isFlying, int isFalling, float forceToDeplete)
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, NetworkID);
            data.SetInt(2, isBumped);
            data.SetInt(3, isFlying);
            data.SetInt(4, isFalling);
            data.SetFloat(5, forceToDeplete);

            GetRTSession.SendData(114, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }

    #region SEND DATA
    public void SendTankMovement(int _id ,Vector3 _pos ,Vector3 _rot)
    {
        try
        {
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, _id);
                data.SetFloat(2, _pos.x);
                data.SetFloat(3, _pos.y);
                data.SetFloat(4, _pos.z);
                data.SetVector3(5, _rot);
                data.SetDouble(6, Network.time);
                data.SetDouble(7, GameSparksManager.Instance.gameTimeInt);

                GetRTSession.SendData(111, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        catch
        {
            try
            {
                GetRTSession = GameSparksManager.Instance.GetRTSession();
                SendTankMovement(_id, _pos, _rot);
            }
            catch
            {

            }
        }
    }

   void ActivatePowerup()
    {
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, _networkID);
            if (_shieldSwitch) 
                data.SetInt(2, 1);
            else
                data.SetInt(2,0);

            GetRTSession.SendData(113, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
    #endregion
    //=====================================================================================================================
    //=====================================================================================================================
    //DATA RECEIVER
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
            if (_packet.Data.GetDouble(7).Value == m_BufferedState[0].timestamp)
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

            /*

            GameObject.Find("GameUpdateText2").GetComponent<Text>().text += "\nReceivedPos: (" + state.pos.x.ToString("F2") + " : " + state.pos.z.ToString("F2")+")";
            GameObject.Find("GameUpdateText2").GetComponent<Text>().text += "\nReceivedTime: (" + state.timestamp+ ") During: "+_sparksManager.gameTimeInt;
            GameObject.Find("GameUpdateText2").GetComponent<Text>().text += "\nGAP IS: (" + (state.timestamp - _sparksManager.gameTimeInt)+") ABS: (" + Mathf.Abs((int)m_BufferedState[0].timestamp - (int)_sparksManager.gameTimeInt) + "\n";

            */








            PlayerPing = _sparksManager.gameTimeInt - state.timestamp;

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

    public void ReceivePowerUpState(bool _switch)
    {
        _shieldObject.SetActive(_switch);
        _shieldSwitch = _switch;
    }
    #endregion
    //=====================================================================================================================
    //INTERPOLATION EXTRAPOLATION
    #region INTERPOLATION EXTRAPOLATION
    double PlayerPing;
    double interpolationTime;

    public GameObject InterpolateObj, ExtrapoalteObj;

    void UpdateFunctInterpolate()
    {
        try
        {
            double currentTime = _sparksManager.gameTimeInt;
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

            /*
            _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, m_BufferedState[0].pos, 1);
            _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(m_BufferedState[0].rot), 1);
            return;
            */
            Extrapolate();
            return;
            if (m_BufferedState[0].timestamp > interpolationTime)
            {
                interpolationTime = currentTime - (PlayerPing + _sparksManager.playerPingOffset);
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
                //Debug.LogWarning("I AM INTERPOLATING TO: " + lhs.pos + " with speed of "+t);

                // if t=0 => lhs is used directly
                /*
                GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nTime Used: "+t+"\n";

                if (GameObject.Find("GameUpdateText").GetComponent<Text>().text.Length > 3000)
                {
                    GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
                }
                */



                if (_sparksManager._curMethod == GameSparksManager.CurrentMethod.LINEAR)
                {
                    _objToTranslate.transform.position = Vector3.Lerp(_objToTranslate.transform.position, lhs.pos, t);
                    _objToRotate.transform.rotation = Quaternion.Lerp(_objToRotate.transform.rotation, Quaternion.Euler(lhs.rot), rotSpeed);
                    Debug.LogWarning("DOING LINEAR");
                }
                else if (_sparksManager._curMethod == GameSparksManager.CurrentMethod.CUBIC)
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
        if (_sparksManager._curMethod == GameSparksManager.CurrentMethod.LINEAR)
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
    #endregion
    //=====================================================================================================================

    public void ActivateShield(bool _switch)
    {
            _shieldSwitch = _switch;
            _shieldObject.SetActive(_switch);
            ActivatePowerup();
    }

    void OnGUI()
    {
        if (NetworkID.ToString() == _sparksManager.PeerID)
        {
            GUI.Box(new Rect(0, Screen.height-60, 100, 30), "Player: " + NetworkID.ToString());
        }
        else
        {
            GUI.Box(new Rect(100, Screen.height - 60, 100, 30), "Player: " + PlayerPing);
        }
        GUI.Box(new Rect(0, Screen.height-30, 100, 30), "Network: " + _sparksManager.PeerID);
    }

}