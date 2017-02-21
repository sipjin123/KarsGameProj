using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Synergy88;
public class TnTScript : MonoBehaviour {

    private Transform TnTParent;

    private GameSparksRTUnity GetRTSession;
    [SerializeField]
    private int owner_Id;
    public int Owner_Id
    {
        get { return owner_Id; }
        set { owner_Id = value; }
    }

    [SerializeField]
    private int tnt_Id;
    public int TNT_ID
    {
        get { return tnt_Id; }
        set{ tnt_Id = value; }
    }
    void Awake()
    {

        TnTParent = transform.parent;
    }
    public void InitializeObj(int _id)
    {
        TnTParent = transform.parent;
        tnt_Id = _id;
        gameObject.name = "TNT_" + _id;
        gameObject.SetActive(false);
    }
    public void ResetTnT()
    {
        try
        {
            owner_Id = 0;
            transform.SetParent(TnTParent);
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }
        catch { }
    }
    public void DispatchTNTToDestination(int _id,Vector3 _pos, bool _enable)
    {
        try
        {
            GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nTNT # (" + tnt_Id + ") of (" + _id + ") HAS been dispatched to " + _pos;
        }catch
        { }
        owner_Id = _id;
        if (owner_Id == 1)
            GetComponent<MeshRenderer>().material.color = Color.blue;
         else
            GetComponent<MeshRenderer>().material.color = Color.red;
        transform.position = _pos;
        gameObject.SetActive(_enable);
        transform.SetParent(null);
    }

    void OnTriggerEnter(Collider hit)
    {
        if(hit.tag == "Car")
        {
            if(TronGameManager.Instance.NetworkStart == false)
            {
                if(hit.gameObject.name.Contains("2"))
                try
                {
                    hit.GetComponent<AI_Behaviour>().DIE();
                        ResetTnT();
                }
                catch
                {
                    hit.GetComponent<Car_Movement>().Die();
                        ResetTnT();
                    }
                return;
            }



            GameSparks_DataSender _dataSender = hit.GetComponent<GameSparks_DataSender>();
            if (_dataSender.NetworkID != owner_Id)
            {
                if (hit.GetComponent<GameSparks_DataSender>()._shieldSwitch)
                {
                    ResetTnT();
                    return;
                }
                _dataSender.gameObject.GetComponent<SimpleCarController>().PlayerExplode();
                ResetTnT();
            }
        }
    }
}
