using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower_AI_Network : MonoBehaviour
{
    public static Tower_AI_Network Instance
    {  get  {  return _instance;  }  }
    private static Tower_AI_Network _instance;

    public Transform TowerPool;
    public TowerLocker[] Towers;

    void Awake()
    {
        _instance = this;
    }

    public void Receive_Packet(RTPacket _packet)
    {
        switch (_packet.OpCode)
        {
            case 119:
                {
                    //GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nRECEIVED: Tower Data";
                    int controllerId = _packet.Data.GetInt(1).Value;
                    int towerId = _packet.Data.GetInt(2).Value;
                    bool lockOn = _packet.Data.GetInt(3).Value == 1 ? true : false;

                    Vector3 _rot = new Vector3(_packet.Data.GetFloat(4).Value, _packet.Data.GetFloat(5).Value, _packet.Data.GetFloat(6).Value);
                    for (int i = 0; i < Towers.Length; i++)
                    {
                        if (Towers[i].Tower_ID == towerId)
                        {
                            Towers[i].Sync(_rot);
                        }
                    }
                }
                break;
            case 120:
                {
                    int controllerId = _packet.Data.GetInt(1).Value;
                    int BulletId = _packet.Data.GetInt(2).Value;
                    bool lockOn = _packet.Data.GetInt(3).Value == 1 ? true : false;


                    Vector3 _pos = new Vector3(_packet.Data.GetFloat(4).Value, _packet.Data.GetFloat(5).Value, _packet.Data.GetFloat(6).Value);
                    Vector3 _rot = _packet.Data.GetVector3(7).Value;

                    for (int i = 0; i < Towers.Length; i++)
                    {
                        if (Towers[i].Controller_ID == controllerId)
                        {
                            for (int q = 0; q < Towers[i].BulletList.Count; q++)
                            {
                                if (Towers[i].BulletList[q].Bullet_ID == BulletId)
                                {
                                    Towers[i].BulletList[q].SyncBullet(_pos, _rot, lockOn == true ? 1 : 0);
                                }
                            }
                        }
                    }
                }
                break;
        }
    }
}
