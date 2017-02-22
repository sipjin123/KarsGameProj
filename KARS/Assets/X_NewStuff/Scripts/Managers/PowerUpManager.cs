using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour {
    //=======================================================================================================================
    //VARIABLES
    #region VARIABLES
    private static PowerUpManager instance;
    public static PowerUpManager Instance
    {
        get { return instance; }
    }

    int ServerPeerID;

    [SerializeField]
    GameObject Player1, Player2;
    public GameObject Shield;

    [SerializeField]
    Transform MisslePool_Player1, MisslePool_Player2, TnTPool;

    public List<GameObject> MissleList_Player1;
    public List<GameObject> MissleList_Player2;
    public List<GameObject> TnTList;
    private GameSparksRTUnity GetRTSession;
    #endregion
    //=======================================================================================================================
    //INITIALIZATION
    #region INITIALIZATION
    void Awake()
    {
        instance = this;
        Player1 = GameObject.Find("Car1");
        Player2 = GameObject.Find("Car2");
    }

    public void StartNetwork()
    {
        MissleList_Player1 = new List<GameObject>();
        MissleList_Player2 = new List<GameObject>();
        TnTList = new List<GameObject>();
        StartCoroutine(DelayStartup());
    }

    IEnumerator DelayStartup()
    {
        yield return new WaitForSeconds(1);
        GameObject temp = null;
        
        for (int i = 0; i < MisslePool_Player1.childCount; i++)
        {
            temp = MisslePool_Player1.GetChild(i).gameObject;
            MissleList_Player1.Add(temp);
            temp.GetComponent<MissleScript>().Set_MissleID(i);
        }
        for (int i = 0; i < MisslePool_Player2.childCount; i++)
        {
            temp = MisslePool_Player2.GetChild(i).gameObject;
            MissleList_Player2.Add(temp);
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
        UIManager.instance.GameUpdateText.text += "\nPeer is Setup";
        try
        {
            ServerPeerID = (GameSparkPacketReceiver.Instance.PeerID);
        }
        catch { }
    }
    #endregion
    //=======================================================================================================================
    //TNT SEND AND RECEIVE FROM SERVER
    #region TNT SEND AND RECEIVE FROM SERVER
    public void SetUpTNT(int _id, Vector3 _pos, bool _enable)
    {
        if(TronGameManager.Instance.NetworkStart == false)
        {
             try
            {
                GameObject temp = TnTPool.transform.GetChild(0).gameObject;
                temp.GetComponent<TnTScript>().DispatchTNTToDestination(_id, _pos, _enable);
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
                SetUpTNT(_id, _pos, _enable);
            }

            return;
        }


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
        GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
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
    #endregion
    //=======================================================================================================================
    //MISSLE SEND AND RECEIVE FROM SERVER
    #region MISSLE SEND AND RECEIVE FROM SERVER
    public void LockOnTarget(int senderID,GameObject _obj, MissleScript.MISSLE_TYPE _misType)
    {
        if(senderID == 1 || senderID == 0)
        {
            MissleList_Player1[0].GetComponent<MissleScript>().LockOnToThisObject(Player1,_obj, _misType);
            GameObject temp = MissleList_Player1[0];
            for (int i = 0; i < MissleList_Player1.Count - 1; i++)
            {
                MissleList_Player1[i] = MissleList_Player1[i + 1];
            }
            MissleList_Player1[MissleList_Player1.Count - 1] = temp;
        }
        else if (senderID == 2)
        {
            MissleList_Player2[0].GetComponent<MissleScript>().LockOnToThisObject(Player2,_obj, _misType);

            GameObject temp = MissleList_Player2[0];
            for (int i = 0; i < MissleList_Player2.Count - 1; i++)
            {
                MissleList_Player2[i] = MissleList_Player2[i + 1];
            }
            MissleList_Player2[MissleList_Player2.Count - 1] = temp;
        }
    }
    /*
    void ClientSendToServerMissleLock(int sender, int receiver)
    {
        GetRTSession = GameSparkPacketReceiver.Instance.GetRTSession();
        using (RTData data = RTData.Get())
        {
            data.SetInt(1, sender);
            data.SetInt(2, receiver);
            GetRTSession.SendData(114, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
        }
    }*/
    #endregion
    //=======================================================================================================================
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchMissleFromBUtton(1);
        }
        if(Input.GetKeyDown(KeyCode.RightAlt))
        {
            DropTNTFromButton();
        }



            if (MissleCooldownTimer_Switch == true)
            {
                if (ServerPeerID == 1)
                    UIManager.instance.MissleBar_1.fillAmount = MissleCooldownTimer_Count / TronGameManager.Instance.missleCooldown;
                else
                    UIManager.instance.MissleBar_2.fillAmount = MissleCooldownTimer_Count / TronGameManager.Instance.missleCooldown;

                if (MissleCooldownTimer_Count < TronGameManager.Instance.missleCooldown)
                {
                    MissleCooldownTimer_Count += Time.fixedDeltaTime;
                }
                else
                {
                    MissleCooldownTimer_Switch = false;
                }
            }

            if(NitroActiveTimer_Switch)
            {

                if (ServerPeerID == 1)
                    UIManager.instance.NitrosBar_1.fillAmount = 1- (NitroActiveTimer_Timer / TronGameManager.Instance.nitroDuration);
                else
                    UIManager.instance.NitrosBar_2.fillAmount = 1-(NitroActiveTimer_Timer / TronGameManager.Instance.nitroDuration);

                if (NitroActiveTimer_Timer < TronGameManager.Instance.nitroDuration)
                {
                    NitroActiveTimer_Timer += Time.deltaTime;
                }
                else 
                {
                    NitroActiveTimer_Switch = false;

                    NitroCooldownTimer_Switch = true;
                    NitroCooldownTimer_Timer = 0;
                    if (ServerPeerID == 1)
                        Player1.GetComponent<Car_Movement>().NitrosActive = false;
                    else
                        Player2.GetComponent<Car_Movement>().NitrosActive = false;
                }
            }
            if(NitroCooldownTimer_Switch)
            {
                if (ServerPeerID == 1)
                    UIManager.instance.NitrosBar_1.fillAmount = NitroCooldownTimer_Timer / TronGameManager.Instance.nitroCooldown;
                else
                    UIManager.instance.NitrosBar_2.fillAmount = NitroCooldownTimer_Timer / TronGameManager.Instance.nitroCooldown;

                if (NitroCooldownTimer_Timer < TronGameManager.Instance.nitroCooldown)
                {
                    NitroCooldownTimer_Timer += Time.fixedDeltaTime;
                }
                else
                {
                    NitroCooldownTimer_Switch = false;
                }
            }

        if(Input.GetKeyDown(KeyCode.Y))
        {

            GameObject.Find("GameUpdateText").GetComponent<Text>().text = "";
        }
    }


    bool MissleCooldownTimer_Switch;
    float MissleCooldownTimer_Count;
    
    public void LaunchMissleFromBUtton(int _misNum)
    {
        if (MissleCooldownTimer_Switch == true)
            return;
        MissleCooldownTimer_Switch = true;
        MissleCooldownTimer_Count = 0;
        if (GameSparkPacketReceiver.Instance.PeerID == 2)
        {
            LockOnTarget(2, Player1,(MissleScript.MISSLE_TYPE)_misNum);
        }
        else if (GameSparkPacketReceiver.Instance.PeerID == 1)
        {
            LockOnTarget(1, Player2, (MissleScript.MISSLE_TYPE)_misNum);
        }
        else
        {
            LockOnTarget(0,Player2, (MissleScript.MISSLE_TYPE)_misNum);
        }
    }

    internal void LaunchMissle()
    {
        if (GameSparkPacketReceiver.Instance.PeerID == 2)
        {
         //   LockOnTarget(2, Player1, (MissleScript.MISSLE_TYPE)_misNum);
        }
        else if (GameSparkPacketReceiver.Instance.PeerID == 1)
        {
         //   LockOnTarget(1, Player2, (MissleScript.MISSLE_TYPE)_misNum);
        }
    }


    bool NitroActiveTimer_Switch;
    float NitroActiveTimer_Timer;

    bool NitroCooldownTimer_Switch;
    float NitroCooldownTimer_Timer;

    public void ActivateNitrosFromButton()
    {
        if (NitroActiveTimer_Switch == true)
            return;

        NitroActiveTimer_Switch = true;
        NitroActiveTimer_Timer = 0;

        if (ServerPeerID == 1)
            Player1.GetComponent<Car_Movement>().NitrosActive = true;
        else
            Player2.GetComponent<Car_Movement>().NitrosActive = true;

    }










    public void DropTNTFromButton()
    {

        if (GameSparkPacketReceiver.Instance.PeerID == 2)
        {
            SetUpTNT(2, Player2.transform.position, true);
        }
        else if (GameSparkPacketReceiver.Instance.PeerID == 1)
        {
            SetUpTNT(1, Player1.transform.position, true);
        }
        else
        {
            SetUpTNT(0, Player1.transform.position, true);
        }
    }
    internal void DropTNT()
    {
        Debug.LogError("Drop TNT");

        if (GameSparkPacketReceiver.Instance.PeerID == 2)
        {
            SetUpTNT(2, Player2.transform.position, true);
        }
        else if (GameSparkPacketReceiver.Instance.PeerID == 1)
        {
            SetUpTNT(1, Player1.transform.position, true);
        }
    }

}
