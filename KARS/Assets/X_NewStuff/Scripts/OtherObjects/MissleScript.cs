using GameSparks.RT;
using Synergy88;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissleScript : MonoBehaviour {
    public struct State
    {
        internal Vector3 pos;
        internal Vector3 rot;
    }

    public State[] m_BufferedState = new State[20];
    public enum MISSLE_TYPE
    {
        STUN,
        BLIND,
        CONFUSE,
        SLOW,
        SILENCE
    }
    public MISSLE_TYPE _missleType;

    public void SetMissleType(MISSLE_TYPE _misType)
    {
        _missleType = _misType;
    }
    //================================================================================================================================
    #region VARIABLES
    [SerializeField]
    private int _playerController_ID;
    public int PlayerController_ID
    {
        get { return _playerController_ID; }
        set { _playerController_ID = value; }
    }

    [SerializeField]
    private int missle_ID;
    public int Missle_ID
    {
        get { return missle_ID; }
        set { missle_ID = value; }
    }


    [SerializeField]
    private GameObject objectToHit;

    [SerializeField]
    private bool lockOnObject;

    public Transform missleParent;
    float missleSpeed = 0.5f;

    private GameSparksRTUnity GetRTSession;

    public Vector3 SyncMovement;
    public Vector3 SyncRot;
    #endregion
    //================================================================================================================================
    #region UPDATE AND SYNC
    void Update()
    {
        if(TronGameManager.Instance.NetworkStart == false)
        {
            transform.position += transform.forward * 1f;
            return;
        }


        if (_playerController_ID == GameSparkPacketHandler.Instance.GetPeerID())
        {
            //Debug.LogError("missle is controlling");
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.blue;
            if (lockOnObject)
            {
                if (Vector3.Distance(transform.position, objectToHit.transform.position) < 3)
                {
                    SendToSErverTheCollision();
                }
                else
                {
                    SendMissleData(1);
                    transform.position += transform.forward * 1f; //Vector3.MoveTowards(transform.position, objectToHit.transform.position, missleSpeed);
                    //transform.LookAt(objectToHit.transform.position);
                }
            }
        }
        else
        {
            //Debug.LogError("missle is syncing");
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
            transform.position = Vector3.Lerp(transform.position, SyncMovement, 1);
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, SyncRot, 1);
        }
    }
    public void SetSYnc(Vector3 _pos,Vector3 _rot)
    {
        for (int q = m_BufferedState.Length - 1; q >= 1; q--)
        {
            m_BufferedState[q] = m_BufferedState[q - 1];
        }

        State state;
        state.pos = _pos;
        state.rot =_rot;
        m_BufferedState[0] = state;


        try
        {
            SyncMovement = m_BufferedState[0].pos + (m_BufferedState[0].pos - m_BufferedState[1].pos);
        }
        catch
        {
            SyncMovement = _pos;
        }
        SyncRot = _rot;
    }
    #endregion
    //================================================================================================================================
    #region SEND DATA
    void SendMissleData(int _var)
    {
        try
        {
            GetRTSession = GameSparkPacketHandler.Instance.GetRTSession();
            using (RTData data = RTData.Get())
            {
                data.SetInt(1, missle_ID);
                data.SetInt(2, PlayerController_ID);
                data.SetFloat(3, transform.position.x);
                data.SetFloat(4, transform.position.y);
                data.SetFloat(5, transform.position.z);
                data.SetVector3(6, transform.eulerAngles);
                data.SetInt(7, _var);

                GetRTSession.SendData(OPCODE_CLASS.MissleOpcode, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
        }
        catch { }
    }
    #endregion
    //================================================================================================================================
    #region PUBLIC FUNCTIONS
    public void Set_MissleID(int _var)
    {
        missleParent = transform.parent;
        lockOnObject = false;
        missle_ID = _var;
        gameObject.name += _var;
        gameObject.SetActive(false);
    }
    public void LockOnToThisObject(GameObject _sender,GameObject _obj, MISSLE_TYPE _misType)
    {
        _missleType = _misType;
        //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nMissle "+gameObject.name+" Locking on to: "+_obj.name;
        // Debug.LogError("locking on to object "+_obj.gameObject.name);
        transform.position = _sender.transform.position;
        transform.rotation = _sender.GetComponent<Car_Movement>().CarRotationObject.transform.rotation;
        AudioManager.Instance.SpawnableAudio(transform.position, AUDIO_CLIP.MISSLE_ACTIVE);
        objectToHit = _obj;
        transform.SetParent(null);
        lockOnObject = true;
        gameObject.SetActive(true);
    }
    public void ResetMissle()
    {
        transform.SetParent(missleParent);
        transform.position = missleParent.transform.position;
        SendMissleData(0);
        objectToHit = null;
        lockOnObject = false;
        gameObject.SetActive(false);
    }
    #endregion
    void OnTriggerEnter(Collider hit)
    {
        if(hit.gameObject.tag == "Wall")
        {
            ResetMissle();
        }
        
    }
    //================================================================================================================================
    void SendToSErverTheCollision()
    {
        try
        {
            Car_DataReceiver carReceiver = objectToHit.GetComponent<Car_DataReceiver>();
            if (_playerController_ID != carReceiver.GetNetwork_ID())
            {
                if (carReceiver.GetShieldSwitch())
                {
                    carReceiver.ReceivePowerUpState(false,NetworkPlayerStatus.ACTIVATE_SHIELD);
                    ResetMissle();
                    return;
                }

                switch (_missleType)
                {
                    case MISSLE_TYPE.BLIND:
                        {
                            carReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_BLIND);
                            ResetMissle();
                        }
                        break;
                    case MISSLE_TYPE.STUN:
                        {
                            carReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_STUN);
                            ResetMissle();
                        }
                        break;
                    case MISSLE_TYPE.CONFUSE:
                        {
                            carReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_CONFUSE);
                            ResetMissle();
                        }
                        break;
                    case MISSLE_TYPE.SLOW:
                        {
                            carReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_SLOW);
                            ResetMissle();
                        }
                        break;
                    case MISSLE_TYPE.SILENCE:
                        {
                            carReceiver.Activate_StateFromButton(NetworkPlayerStatus.ACTIVATE_SILENCE);
                            ResetMissle();
                        }
                        break;
                }
            }
        }
        catch { }
    }
}
