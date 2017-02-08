using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Car_DataReceiver : MonoBehaviour {



    public int Network_ID;
    public bool ifMy_Network_Player;

    private GameSparksRTUnity GetRTSession;

    public Camera NetworkCam;

    [SerializeField]
    GameSparkPacketReceiver _gameSparkPacketReceiver;

    public void SetNetworkObject(int netID)
    {
        Network_ID = netID;
    }

    public void InitCam()
    {
        _gameSparkPacketReceiver = GameSparkPacketReceiver.Instance.GetComponent<GameSparkPacketReceiver>();
        GetRTSession = _gameSparkPacketReceiver.GetRTSession();

        if (Network_ID == _gameSparkPacketReceiver.PeerID)
        {
            NetworkCam.enabled = true;
            ifMy_Network_Player = true;
        }
    }
    void Update()
    {
        if (!ifMy_Network_Player)
            return;

        if(Input.GetKeyDown(KeyCode.W))
        {
            SendCarMovement(Network_ID, transform.position, transform.eulerAngles);
        }
    }

    public void SendCarMovement(int _id, Vector3 _pos, Vector3 _rot)
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
                //data.SetDouble(6, Network.time);
                //data.SetDouble(7, _gameSparkPacketReceiver.gameTimeInt);


                GetRTSession.SendData(111, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);

                //NETWORKTEST
                GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\n DAta SEnd";
            }
        }
        catch
        {
            try
            {
                GetRTSession = _gameSparkPacketReceiver.GetRTSession();
                SendCarMovement(_id, _pos, _rot);
            }
            catch
            {

            }
        }
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

}
