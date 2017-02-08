﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TronGameManager : MonoBehaviour {

    private static TronGameManager _instance;
    public static TronGameManager Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    bool NetworkStart;
    public GameObject[] PlayerObjects;
    public GameObject[] NetworkCanvas;

    public void SetNetworkStart( bool _switch)
    {
        NetworkStart = _switch;
        for (int i = 0; i < PlayerObjects.Length; i++)
            PlayerObjects[i].SetActive(true);
        
        if (!_switch)
        {
            for (int i = 0; i < NetworkCanvas.Length; i++)
                NetworkCanvas[i].SetActive(false);
        }
    }

}