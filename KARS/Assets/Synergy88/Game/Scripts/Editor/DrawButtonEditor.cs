using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Common;
using Common.Extensions;

// alias
using UObject = UnityEngine.Object;
using CColor = Common.Extensions.Color;

namespace Synergy88
{

    public enum EFoldOut
    {
        ON,
        OFF,
    };

    public class DrawButtonEditor : Editor
    {

        protected static bool DrawButton(string title, string tooltip, float width)
        {
            return GUILayout.Button(new GUIContent(title, tooltip));
        }

        protected static void DrawButton(string title, Action visitor)
        {
            EditorGUILayout.BeginHorizontal();
            if (DrawButton(title, string.Empty, 40f))
            {
                visitor();
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void DrawFoldOut(out bool holder, bool defaultValue, string foldout, Action action)
        {
            holder = defaultValue;
            holder = EditorGUILayout.Foldout(holder, foldout);

            if (holder)
            {
                if (Selection.activeTransform)
                {
                    //Selection.activeTransform.position = EditorGUILayout.Vector3Field("Position", Selection.activeTransform.position);
                    //status = Selection.activeTransform.name;
                    //this.gameCommandType = (GameCommandType)EditorGUILayout.EnumMaskField(this.gameCommandType);
                    action();
                }
            }

            if (!Selection.activeTransform)
            {
                holder = false;
            }
        }

        protected T GetTarget<T>() where T : MonoBehaviour
        {
            return (T)this.target;
        }

        protected void Label(string label)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        private void TestDrawFoldOut()
        {
            bool foldOutFlag = false;
            EFoldOut foldOut = EFoldOut.ON;
            this.DrawFoldOut(out foldOutFlag, foldOutFlag, "Sample FoldOut", delegate ()
            {
                foldOut = (EFoldOut)EditorGUILayout.EnumMaskField(foldOut);
            });
        }

        protected void CreateLevelData(string name, string path, string json)
        {
#if !UNITY_WEBPLAYER
            string fileName = String.Format("{0}/{1}.json", path, name);
            CColor colorLog = CColor.yellow;

            Debug.LogFormat("{0} Path:{1}\n", colorLog.LogHeader("Editor::CreateLevelData"), fileName);

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine(json);
            }
#endif
        }

        //Texture2D texture = AssetDatabase.LoadAssetAtPath(rootPath + "/" + selected.name + ".png", typeof(Texture2D)) as Texture2D;
        protected TextAsset[] LoadTextData(string lccation)
        {
            //TextAsset item = AssetDatabase.LoadAssetAtPath(file, typeof(TextAsset)) as TextAsset;
            //return item.text;
            //UObject[] rawItems = (UObject[])AssetDatabase.LoadAllAssetsAtPath(lccation);
            //TextAsset[] items = new TextAsset[rawItems.Length];
            //for (int i = 0; i < rawItems.Length; i++) {
            //    items[i] = (TextAsset)rawItems[i];
            //}
            //return items;

            TextAsset[] patterns = Resources.LoadAll<TextAsset>(lccation);
            return patterns;
        }
    }

}