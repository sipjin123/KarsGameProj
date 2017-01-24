using System;
using System.Collections;
using System.Diagnostics;

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// alias
using UDebug = UnityEngine.Debug;

namespace Synergy88
{

    [InitializeOnLoad]
    public static class S88Editor
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // CONSTANTS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public const string S88_ROOT = "Synergy88/";
        public const string S88_TOOLS = "Tools/";
        public const string S88_BUILD = "Build/";

        // scenes
        public const string SYSTEM_SCENE = "Assets/KARS/Scenes/System.unity";
        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // BUILD
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // cmd/alt + shift + r = &#r
        // ctr + shift + r = %#r
        //[MenuItem(S88_ROOT + S88_DEBUG + "Aries/Run MainGame %#r", false)]
        // alt + shift + r
        [MenuItem(S88_ROOT + S88_BUILD + "Run System &#r", false)]
        public static void RunSystem()
        {
            Recompile();

            // Apply your debug settings here
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(SYSTEM_SCENE);
            EditorApplication.isPlaying = true;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // DEBUG
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        [MenuItem(S88_ROOT + S88_BUILD + "Recompile", false)]
        public static void Recompile()
        {
            // Apply your debug settings here
            AssetDatabase.StartAssetEditing();
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string assetPath in allAssetPaths)
            {
                MonoScript script = AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript)) as MonoScript;
                if (script != null)
                {
                    AssetDatabase.ImportAsset(assetPath);
                    break;
                }
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

#if UNITY_ANDROID
        [MenuItem(S88_ROOT + S88_BUILD + "Build (Android)", false)]
#elif UNITY_IOS
        [MenuItem(S88_ROOT + S88_BUILD + "Build (iOS)", false)]
#else
        [MenuItem(S88_ROOT + S88_BUILD + "Build (Invalid Platform)", false)]
#endif
        public static void Build()
        {
#if UNITY_ANDROID
            string keyStoreName = "pathtokeystore/keystore.keystore";
            string keyAliasName = "your.bundle.identifier";
            string password = "y0urP@ssw0rD";

            PlayerSettings.Android.keystoreName = keyStoreName;
            PlayerSettings.Android.keystorePass = password;
            PlayerSettings.Android.keyaliasName = keyAliasName;
            PlayerSettings.Android.keyaliasPass = password;

            /*
            string path = "D:/Projects/Builds/Barangay143/Android";

            BuildOptions options = BuildOptions.None;
            options |= BuildOptions.AllowDebugging;
            options |= BuildOptions.ConnectWithProfiler;
            options |= BuildOptions.Development;

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
            BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.Android, options);
            //*/
#elif UNITY_IOS
            /*
            BuildOptions options = BuildOptions.None;
            options |= BuildOptions.AllowDebugging;
            options |= BuildOptions.ConnectWithProfiler;
            options |= BuildOptions.Development;

            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
		    BuildPipeline.BuildPlayer(GetScenePaths(), path, BuildTarget.iOS, options);
            //*/
#endif
        }
        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // TOOLS
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // cmd/alt + shift + d = &#d
        // ctr + shift + d = %#d
        [MenuItem(S88_ROOT + S88_TOOLS + "Clear Prefs %#d", false)]
        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
        
        // cmd + shift + x
        [MenuItem(S88_ROOT + S88_TOOLS + "Clear Logs %#x", false)]
        public static void ClearLogs()
        {
            var logEntries = Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
        
    }

}