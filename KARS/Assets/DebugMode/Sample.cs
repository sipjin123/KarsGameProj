using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour {

    public float datatype1 = 0;
    public float datatype2 = 0;
    public float datatype3 = 0;
    public float datatype4 = 0;
    public string dataName;
	
    void Start()
    {
        DebugMode.GetInstance.RegisterDataType(ref datatype1, "Number1");
        DebugMode.GetInstance.RegisterDataType(ref datatype2, "Number2");
        DebugMode.GetInstance.RegisterDataType(ref datatype3, "Number3");
        DebugMode.GetInstance.RegisterDataType(ref datatype4, "Number4");
        DebugMode.GetInstance.EnableDebugMode();
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(25, 25, 100, 100), datatype1 + "-" + datatype2 + "-" + datatype2 + "-" + datatype4))
        {
        }
     
    }
    

}
