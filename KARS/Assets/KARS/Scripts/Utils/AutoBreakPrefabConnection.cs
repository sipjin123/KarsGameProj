using UnityEngine;
using UnityEditor;

// source: http://answers.unity3d.com/answers/323886/view.html
[ExecuteInEditMode]
public class AutoBreakPrefabConnection : MonoBehaviour
{
    void Start()
    {
        #if UNITY_EDITOR
        PrefabUtility.DisconnectPrefabInstance(gameObject);
        #endif
        DestroyImmediate(this); // Remove this script
    }
}
