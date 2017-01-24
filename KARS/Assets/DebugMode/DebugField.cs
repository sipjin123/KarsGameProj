using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DebugField : MonoBehaviour {

    [SerializeField]
    private Button ModifyButton;
    [SerializeField]
    private Text DataName;
    [SerializeField]
    private InputField DataValue;

    public Button GetButton { get { return ModifyButton; } }
    public InputField GetInput { get { return DataValue; } }
    public void SetFields(ref float newDataValue,string name)
    {
        DataName.text = name;
        DataValue.text = newDataValue.ToString();
    }


}
