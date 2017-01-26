using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void InitializeObj(int _id)
    {
        TnTParent = transform.parent;
        tnt_Id = _id;
        gameObject.name = "TNT_" + _id;
        gameObject.SetActive(false);
    }
    public void ResetTnT()
    {
        owner_Id = 0;
        transform.SetParent(TnTParent);
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
    public void DispatchTNTToDestination(int _id,Vector3 _pos, bool _enable)
    {
            GameObject.Find("GameUpdateText").GetComponent<Text>().text += "\nTNT # ("+tnt_Id+") of ("+_id+") HAS been dispatched to "+_pos;
            owner_Id = _id;
            transform.position = _pos;
            gameObject.SetActive(_enable);
            transform.SetParent(null);
    }
}
