using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;


namespace Synergy88
{
    /// <summary>
    /// Editor tool for removing template modules.
    /// This removes module files and if the module is wrapped by a template service, the service is also removed from ServicesRoot.
    /// </summary>
    public class Synergy88ModuleRemover : EditorWindow
    {
        [Serializable]
        public class Module
        {
            public string Name;
            public bool ShouldRemove { get; set; }
            public List<string> PathsToRemove;
            public List<string> PrefabObjectsToRemove;
            public List<string> PrefabsToUpdate;
        }

        private List<Module> Modules;

        [MenuItem("Synergy88/Modules/Remove...")]
        public static void Init()
        {
            Synergy88ModuleRemover window = EditorWindow.GetWindow<Synergy88ModuleRemover>();
        }

        void OnGUI()
        {
            if (GUILayout.Button("Initialize Module List"))
            {
                InitializeModuleList();
            }

            if (Modules == null)
            {
                GUILayout.Label("Module list not initialized");
                return;
            }

            if (Modules == null)
            {
                GUILayout.Label("No modules in list");
                return;
            }
            else
            {
                GUILayout.Label("Select modules to be removed:");
                foreach (Module module in Modules)
                {
                    module.ShouldRemove = GUILayout.Toggle(module.ShouldRemove, module.Name);
                }

                if (GUILayout.Button("Select All"))
                {
                    SelectAll();
                }

                if (GUILayout.Button("Deselect All"))
                {
                    DeselectAll();
                }

                if (GUILayout.Button("Remove Selected Modules"))
                {
                    RemoveSelectedModules();
                }
            }
        }

        private void InitializeModuleList()
        {
            string[] moduleJsons = File.ReadAllLines("Synergy88Files/Synergy88Modules.dat");
            Modules = new List<Module>();
            foreach (string moduleJson in moduleJsons)
            {
                //Debug.LogWarning(moduleJson);
                Module module = new Module();
                EditorJsonUtility.FromJsonOverwrite(moduleJson, module);
                if (string.IsNullOrEmpty(module.Name)) continue;

                Modules.Add(module);
            }
            /*
            Module unityIAP = new Module()
            {
                Name = "UnityIAP",
                PathsToRemove = new List<string>() { "Plugins/UnityPurchasing" }
            };
            Module bannerAds = new Module()
            {
                Name = "BannerAds",
                PathsToRemove = new List<string>() { "Plugins/Android" }
            };
            Modules = new List<Module>() { unityIAP, bannerAds };

            File.WriteAllText("Synergy88Files/Synergy88Modules.dat", EditorJsonUtility.ToJson(unityIAP));
            */
        }

        private void SelectAll()
        {
            foreach (Module module in Modules)
            {
                module.ShouldRemove = true;
            }
        }

        private void DeselectAll()
        {
            foreach (Module module in Modules)
            {
                module.ShouldRemove = false;
            }
        }

        private void RemoveSelectedModules()
        {
            foreach (Module module in Modules)
            {
                if (!module.ShouldRemove) continue;
                
                // remove relevant files and directories
                foreach (string toRemove in module.PathsToRemove)
                {
                    if (AssetDatabase.DeleteAsset(toRemove))
                    {
                        Debug.LogFormat("Successfully removed {0}", toRemove);
                    }
                    else
                    {
                        Debug.LogWarningFormat("Failed to remove {0}", toRemove);
                    }
                }

                // update affected prefabs
                foreach (string prefabToUpdate in module.PrefabsToUpdate)
                {
                    if (string.IsNullOrEmpty(prefabToUpdate)) continue;

                    string[] updateInfo = prefabToUpdate.Split(':');
                    if (updateInfo.Length < 2) continue;

                    // get prefab path
                    string prefabPath = updateInfo[0];
                    if (string.IsNullOrEmpty(prefabPath)) continue;

                    // get prefab child to remove
                    string childToRemove = updateInfo[1];
                    if (string.IsNullOrEmpty(childToRemove)) continue;

                    // get prefab reference
                    GameObject prefabReference = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
                    if (prefabReference == null)
                    {
                        Debug.LogWarningFormat("failed to find prefab: {0}", prefabPath);
                        continue;
                    }

                    // instantiate prefab in current scene
                    GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabReference) as GameObject;

                    // get a reference to child to be removed
                    Transform prefabInstanceChildT = prefabInstance.transform.FindChild(childToRemove);
                    if (prefabInstanceChildT == null)
                    {
                        Debug.LogWarningFormat("failed to find {0} under {1}", childToRemove, prefabPath);
                        continue;
                    }

                    // destroy child
                    DestroyImmediate(prefabInstanceChildT.gameObject);

                    // update prefab
                    PrefabUtility.ReplacePrefab(prefabInstance, prefabReference);
                    Debug.LogWarningFormat("removed {0} under {1}", childToRemove, prefabPath);

                    // destroy prefab instance
                    DestroyImmediate(prefabInstance);
                }
            }
        }
    }
}
