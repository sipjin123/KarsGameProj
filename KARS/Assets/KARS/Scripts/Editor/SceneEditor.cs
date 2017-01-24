using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Common;
using Common.Extensions;

// alias
using CColor = Common.Extensions.Color;

namespace Synergy88
{

    [CustomEditor(typeof(Scene), true)]
    public class SceneEditor : DrawButtonEditor
    {
        private static readonly string ERROR = CColor.red.LogHeader("[ERROR]");
        private static readonly string WARNING = CColor.yellow.LogHeader("[WARNING]");
        private static readonly string[] HIDDEN = new string[] { "m_Script" };

        /// <summary>
        /// List of SceneTypes (Enum Representation)
        /// </summary>
        private string[] CachedSceneTypes;
        private List<string> SceneTypes;

        /// <summary>
        /// List of SceneDepths (Enum Representation)
        /// </summary>
        private string[] CachedSceneDepths;
        private static readonly List<string> SceneDepths = new List<string>()
        {
            "Invalid",

            "Background",
            "Middleground",
            "Foreground",

            "PopUp",
            "Overlay",
            
            "Max",
        };
        
        private static readonly Dictionary<string, int> SceneDepthValues = new Dictionary<string, int>()
        {
            { SceneDepths[0], 1 },      // Invalid

            { SceneDepths[1], 2 },      // Background
            { SceneDepths[2], 3 },      // Middleground
            { SceneDepths[3], 4 },      // Foreground,

            { SceneDepths[4], 7 },      // PopUp (Reserve Values: 7:Active-6:Blocker-5:Inactives)
            { SceneDepths[5], 8 },      // Overlay
            
            { SceneDepths[6], 10 },      // Max
        };

        private int SceneTypeIndex = -1;
        private int SceneDepthIndex = -1;

        private SerializedProperty SceneTypeString;
        private SerializedProperty SceneDepthString;

        /// <summary>
        /// This method is called on every focus of this gameobject (Root).
        /// This updates the scene enum automatically.
        /// </summary>
        private void Awake()
        {
            CachedSceneTypes = File.ReadAllLines("Synergy88Files/Synergy88Scenes.dat");
            SceneTypes = new List<string>(CachedSceneTypes);

            GenerateSceneEnum();
            EditorUtility.SetDirty(this.GetTarget<Scene>().gameObject);
        }

        private void OnEnable()
        {
            // Update the cache values
            CachedSceneDepths = SceneDepths.ToArray();

            // Update data from editor
            SceneTypeString = serializedObject.FindProperty("_SceneTypeString");
            SceneDepthString = serializedObject.FindProperty("_SceneDepthString");
            SceneTypeIndex = SceneTypes.IndexOf(SceneTypeString.stringValue);
            SceneDepthIndex = SceneDepths.IndexOf(SceneDepthString.stringValue);

            Assertion.Assert(SceneTypeIndex >= 0, string.Format(ERROR + " SceneEditor::OnEnable Invalid cached SceneType:{0} Scene:{1}\n", SceneTypeString.stringValue, GetTarget<Scene>().name));
            Assertion.Assert(SceneDepthIndex >= 0, string.Format(ERROR + " SceneEditor::OnEnable Invalid cached SceneDepth:{0} Scene:{1}\n", SceneDepthString.stringValue, GetTarget<Scene>().name));
        }

        public override void OnInspectorGUI()
        {
            // Draw Script
            serializedObject.Update();
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            serializedObject.ApplyModifiedProperties();

            // Update SceneType from Editor cache
            UpdateSceneIndices();

            // Hides script field on Editor (Since it was already drawn above the Scene Indices)
            DrawPropertiesExcluding(serializedObject, HIDDEN);
            
            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateSceneIndices()
        {
            // Updates Scene Index based on the selected value from editor
            {
                int sceneIndex = EditorGUILayout.Popup("Scene Type", SceneTypeIndex, CachedSceneTypes);

                if (sceneIndex >= 0 && SceneTypeIndex != sceneIndex)
                {
                    Debug.LogFormat("SceneEditor::OnInspectorGUI Scene Index:{0} UpdatedIndex:{1} Scene:{2}\n", SceneTypeIndex, sceneIndex, SceneTypes[sceneIndex]);

                    SceneTypeIndex = sceneIndex;
                    SceneTypeString.stringValue = SceneTypes[sceneIndex];
                    serializedObject.Update();

                    GetTarget<Scene>().SceneTypeString = SceneTypes[sceneIndex];
                    GenerateSceneEnum();
                }
            }

            // Updates Depth Index based on the selected value from editor
            {
                int depthUIndex = EditorGUILayout.Popup("Scene Depth", SceneDepthIndex, CachedSceneDepths);

                if (depthUIndex >= 0 && SceneDepthIndex != depthUIndex)
                {
                    Debug.LogFormat("SceneEditor::OnInspectorGUI Depth Index:{0} UpdatedIndex:{1} Scene:{2}\n", SceneDepthIndex, depthUIndex, SceneDepths[depthUIndex]);

                    SceneDepthIndex = depthUIndex;
                    SceneDepthString.stringValue = SceneDepths[depthUIndex];
                    serializedObject.Update();
                    
                    GetTarget<Scene>().SceneDepthString = SceneDepths[depthUIndex];
                    GenerateDepthEnum();
                }
            }
        }

        /// <summary>
        /// Generates a class that contains the SceneTypes Enum
        /// </summary>
        private void GenerateSceneEnum()
        {
            string path = "Assets/KARS/Scripts/Data/SceneTypes.cs";

            // delete old class
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (File.Exists(path) == false)
            { 
                // do not overwrite
                using (StreamWriter outfile = new StreamWriter(path))
                {
                    outfile.WriteLine("// AUTOGENERATED: DO NOT EDIT");
                    outfile.WriteLine("using System;");
                    outfile.WriteLine(string.Empty);
                    outfile.WriteLine("namespace Synergy88");
                    outfile.WriteLine("{");
                    outfile.WriteLine("\t[Serializable]");
                    outfile.WriteLine("\t[Flags]");
                    outfile.WriteLine("\tpublic enum EScene");
                    outfile.WriteLine("\t{");

                    for (int i = 0; i < SceneTypes.Count; i++)
                    {
                        outfile.WriteLine("\t\t{0},", SceneTypes[i]);
                    }

                    outfile.WriteLine("\t}");
                    outfile.WriteLine("}");
                }
            }

            AssetDatabase.Refresh();

            Debug.LogFormat("SceneEditor::GenerateSceneEnum SceneTypes generated!\n");
        }

        private void GenerateDepthEnum()
        {
            // remove whitespace and minus
            string path = "Assets/KARS/Scripts/Data/SceneDepths.cs";

            // delete old class
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (File.Exists(path) == false)
            { 
                // do not overwrite
                using (StreamWriter outfile = new StreamWriter(path))
                {
                    outfile.WriteLine("// AUTOGENERATED: DO NOT EDIT");
                    outfile.WriteLine("using System;");
                    outfile.WriteLine("using System.Collections;");
                    outfile.WriteLine(string.Empty);
                    outfile.WriteLine("using UnityEngine;");
                    outfile.WriteLine(string.Empty);
                    outfile.WriteLine("namespace Synergy88");
                    outfile.WriteLine("{");
                    outfile.WriteLine("\t[Serializable]");
                    outfile.WriteLine("\tpublic enum ESceneDepth");
                    outfile.WriteLine("\t{");

                    for (int i = 0; i < SceneDepths.Count; i++)
                    {
                        outfile.WriteLine("\t\t{0} = {1},", SceneDepths[i], SceneDepthValues[SceneDepths[i]]);
                    }

                    outfile.WriteLine("\t}");
                    outfile.WriteLine("}");
                }
            }

            AssetDatabase.Refresh();

            Debug.LogFormat("SceneEditor::GenerateDepthEnum SceneDepths generated!\n");
        }

    }

}