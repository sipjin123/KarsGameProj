using Synergy88;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Use at own Risk
public class DebugMode : MonoBehaviour {


    private static DebugMode _instance;
    public static DebugMode GetInstance { get { return _instance; } }

    [SerializeField]
    private Transform DebugPanel;
    [SerializeField]
    private DebugField debugfield;
    [SerializeField]
    private ScrollRect scrollRect;

    private List<int> _addressList;
    private List<string> _dataNames;

    //private float* tempAddress;

    private bool isDebugOn = false;
    private GameRoot _game;

    void Awake()
    {
        _instance = this;
        _addressList = new List<int>();
        _dataNames = new List<string>();

        _game = GameObject.FindObjectOfType<GameRoot>();
    }

    void OnGUI()
    {
        if (!isDebugOn)
        {
            if (GUI.Button(new Rect(25, 25, 100, 100), "Open Debug"))
            {
                isDebugOn = true;
                scrollRect.gameObject.SetActive(true);
                _game.PauseGame();
            }
        }
        else
        {
            if (GUI.Button(new Rect(25, 25, 100, 100), "Close Debug"))
            {
                isDebugOn = false;
                scrollRect.gameObject.SetActive(false);
                _game.UnPauseGame();
            }

        }

    }


    void Start()
    {
        Invoke("EnableDebugMode",1);
    }



    void Update()
    {
        if (scrollRect.content.localPosition.y < 0)
        {
            scrollRect.content.localPosition = Vector3.zero;
        }
        else if (scrollRect.content.localPosition.y > (scrollRect.content.childCount) * 200)
        {

            scrollRect.content.localPosition = Vector3.up * (scrollRect.content.childCount) * 200;
        }
    }

    //Stores the Data Type's Address
    internal void RegisterDataType(ref float value, string name)
    {     
        /*
        fixed (float* val = &value){
            float* pMyInt = val;
            _addressList.Add((int)pMyInt);
            //addressInt = (int)pMyInt;
            //address = (float*)addressInt;
        }
        */
        _dataNames.Add(name);


    }
    /*
    internal void ChangeValue(int x, InputField displayText)
    {
        tempAddress = (float*)_addressList[x];
        *tempAddress = float.Parse(displayText.text);
        displayText.text = *tempAddress + "";
    }
    */
    #region UI Creation


    internal void EnableDebugMode()
    {
        //CreateUI();
    }
    /*
    private void CreateUI()
    {
        DebugField newdbField = new DebugField();
        int x = 0;
        foreach(int dataAddress in _addressList)
        {
            newdbField = Instantiate(debugfield, DebugPanel);
            newdbField.SetFields(ref *(float*)_addressList[x],_dataNames[x]);
            int tempx = x;
            InputField tempInputField = newdbField.GetInput;

            newdbField.GetButton.onClick.AddListener(() => ChangeValue(tempx, tempInputField));
            x++;
        }
        scrollRect.content.localPosition = Vector3.zero;

    }*/
    #endregion
}
