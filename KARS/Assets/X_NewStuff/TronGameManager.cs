using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TronGameManager : MonoBehaviour {

    private static TronGameManager _instance;
    public static TronGameManager Instance { get { return _instance; } }

    

    public Text Text_rotationSpeed;
    public float rotationSpeed;
    public void TweakrotationSpeed(float _var)
    {
        rotationSpeed += _var;
        Text_rotationSpeed.text = rotationSpeed.ToString();
    }


    public Text Text_MovementSpeed;
    public float MovementSpeed ;
    public void TweakMoveSpeed(float _var)
    {
        MovementSpeed += _var;
        Text_MovementSpeed.text = MovementSpeed.ToString();
    }

    public Text Text_trailDistanceCap;
    public float trailDistanceCap;
    public void TweaktrailDistanceCap(float _var)
    {
        trailDistanceCap += _var;
        Text_trailDistanceCap.text = trailDistanceCap.ToString();
    }

    public Text Text_const_trailDistance;
    public float const_trailDistance ;
    public void Tweakconst_trailDistance(float _var)
    {
        const_trailDistance += _var;
        Text_const_trailDistance.text = const_trailDistance.ToString();
    }

    public GameObject _testPanel;
    public void FlipTestPanel()
    {
        _testPanel.SetActive(!_testPanel.activeInHierarchy);
    }

    void Awake()
    {
        _instance = this;
        MovementSpeed = 5;
        trailDistanceCap = 20f;
        const_trailDistance = 5;
        rotationSpeed = 55;
    }

    public bool NetworkStart;
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
            PlayerObjects[0].GetComponent<Car_DataReceiver>().enabled = false;
            PlayerObjects[0].GetComponent<Car_Movement>().enabled = true;

            PlayerObjects[0].GetComponent<Car_Movement>().myCam.enabled = true;
            PlayerObjects[0].GetComponent<Car_Movement>().StartGame = true;
        }
    }

}
