using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateButtonManager : MonoBehaviour {

    public static StateButtonManager Instance { get { return instance; } }
    public static StateButtonManager instance;
    void Awake()
    {
        instance = this;
    }


    public void OnClick_StartGame()
    {
        StateManager.Instance.Access_ChangeState(MENUSTATE.MATCH_FIND);
    }
}
