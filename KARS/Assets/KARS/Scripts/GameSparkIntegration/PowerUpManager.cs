using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour {

    private static PowerUpManager instance;
    public static PowerUpManager Instance
    {
        get { return instance; }
    }

    [SerializeField]
    GameObject BLueCar, RedCar;
    public GameObject Shield;

    [SerializeField]
    Transform MisslePool,TnTPool;

    public List<GameObject> MissleList;
    public List<GameObject> TnTList;
    private GameSparksRTUnity GetRTSession;
    //=======================================================================================================================
    //INITIALIZATION
    #region INITIALIZATION
    void Awake()
    {
        MissleList = new List<GameObject>();
        TnTList = new List<GameObject>();
        instance = this;
    }
    void Start()
    {
        BLueCar = GameObject.Find("BlueCar");
        RedCar = GameObject.Find("RedCar");
        StartCoroutine(DelayStartup());
    }

    IEnumerator DelayStartup()
    {
        yield return new WaitForSeconds(1);
        GameObject temp = null;
        
        for (int i = 0; i < MisslePool.childCount; i++)
        {
            temp = MisslePool.GetChild(i).gameObject;
            MissleList.Add(temp);
            temp.GetComponent<MissleScript>().Set_MissleID(i);
        }
        yield return new WaitForSeconds(1);
        temp = null;
        for (int i = 0; i < TnTPool.childCount; i++)
        {
            temp = TnTPool.GetChild(i).gameObject;
            TnTList.Add(temp);
            temp.GetComponent<TnTScript>().InitializeObj(i);
        }
    }
    #endregion
    //=======================================================================================================================
    public void SetUpTNT(int _id, Vector3 _pos, bool _enable)
    {

        try
        {
            GameObject temp = TnTPool.transform.GetChild(0).gameObject;
            temp.GetComponent<TnTScript>().DispatchTNTToDestination(_id, _pos, _enable);
            SendToServer(_id, temp.GetComponent<TnTScript>().TNT_ID, _pos, _enable);
        }
        catch
        {
            GameObject temp = TnTList[0];
            for (int i = 0; i < TnTList.Count - 1; i++)
            {
                TnTList[i] = TnTList[i + 1];
            }
            TnTList[TnTList.Count - 1] = temp;
            TnTList[TnTList.Count - 1].GetComponent<TnTScript>().ResetTnT();
            SetUpTNT(_id,_pos,_enable);
        }
    }

    public void ReceiveFromServer(int _id, int _tntID, Vector3 _pos, bool _enable)
    {
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nReceived From Server";
        for(int i = 0; i < TnTList.Count; i++)
        {
            if(TnTList[i].GetComponent<TnTScript>().TNT_ID == _tntID)
            {
                TnTList[i].GetComponent<TnTScript>().DispatchTNTToDestination(_id,_pos,_enable);
            }
        }
    }

    public void SendToServer(int _id, int _tntID,Vector3 _pos, bool _enable)
    {
        GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nSend Server Position "+_pos;
        GetRTSession = GameSparksManager.Instance.GetRTSession();
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, _id);
            data.SetInt(2, _tntID);
            data.SetVector3(3, _pos);
            if (_enable)
                data.SetInt(4, 1);
            else
                data.SetInt(4, 0);

            GetRTSession.SendData(116, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }
    //=======================================================================================================================
    public void LockOnTarget(int senderID,GameObject _obj)
    {
        try
        {
            MisslePool.transform.GetChild(0).GetComponent<MissleScript>().LockOnToThisObject(senderID,_obj);
        }
        catch
        {
            GameObject temp = MissleList[0];
            for (int i = 0; i < MissleList.Count-1; i++)
            {
                MissleList[i] = MissleList[i + 1];
            }
            MissleList[MissleList.Count - 1] = temp;
            MissleList[MissleList.Count - 1].GetComponent<MissleScript>().ResetMissle();
            LockOnTarget(senderID, _obj);
        }
    }

    void ClientSendToServerMissleLock(int sender, int receiver)
    {
        GetRTSession = GameSparksManager.Instance.GetRTSession();
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, sender);
            data.SetInt(2, receiver);
            GetRTSession.SendData(114, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GameSparksManager.Instance.PeerID == "2")
            {
                LockOnTarget(2, BLueCar);
                //ClientSendToServerMissleLock(2,1);
            }
            else if (GameSparksManager.Instance.PeerID == "1")
            {
                LockOnTarget(1, RedCar);
                //ClientSendToServerMissleLock(1,2);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (GameSparksManager.Instance.PeerID == "2")
            {
                SetUpTNT(2, RedCar.transform.position, true);
            }
            else if (GameSparksManager.Instance.PeerID == "1")
            {
                SetUpTNT(1, BLueCar.transform.position, true);
            }
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {

            GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
        }
    }

    internal void LaunchMissle()
    {
        if (GameSparksManager.Instance.PeerID == "2")
        {
            LockOnTarget(2, BLueCar);
            //ClientSendToServerMissleLock(2,1);
        }
        else if (GameSparksManager.Instance.PeerID == "1")
        {
            LockOnTarget(1, RedCar);
            //ClientSendToServerMissleLock(1,2);
        }
    }

    internal void DropTNT()
    {
        Debug.LogError("Drop TNT");

        if (GameSparksManager.Instance.PeerID == "2")
        {
            SetUpTNT(2, RedCar.transform.position, true);
        }
        else if (GameSparksManager.Instance.PeerID == "1")
        {
            SetUpTNT(1, BLueCar.transform.position, true);
        }
    }

}
