using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;


namespace Synergy88
{
    /// <summary>
    /// Editor tool for exporting template modules.
    /// </summary>
    public class Synergy88ModuleExporter : EditorWindow
    {
        [Serializable]
        public class Module
        {
            public string Name;
            public bool ShouldExport { get; set; }
            public List<string> PathsToInclude;
        }

        private List<Module> Modules;

        [MenuItem("Synergy88/Modules/Export...")]
        public static void Init()
        {
            Synergy88ModuleExporter window = EditorWindow.GetWindow<Synergy88ModuleExporter>();
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
                GUILayout.Label("Select modules to be exported:");
                foreach (Module module in Modules)
                {
                    module.ShouldExport = GUILayout.Toggle(module.ShouldExport, module.Name);
                }

                if (GUILayout.Button("Select All"))
                {
                    SelectAll();
                }

                if (GUILayout.Button("Deselect All"))
                {
                    DeselectAll();
                }

                if (GUILayout.Button("Export Selected Modules"))
                {
                    ExportSelectedModules();
                }
            }
        }

        private void InitializeModuleList()
        {
            string[] moduleJsons = File.ReadAllLines("Synergy88Files/Synergy88ExportModules.dat");
            Modules = new List<Module>();
            foreach (string moduleJson in moduleJsons)
            {
                //Debug.LogWarning(moduleJson);
                Module module = new Module();
                EditorJsonUtility.FromJsonOverwrite(moduleJson, module);
                if (string.IsNullOrEmpty(module.Name)) continue;

                Modules.Add(module);
            }
        }

        private void SelectAll()
        {
            foreach (Module module in Modules)
            {
                module.ShouldExport = true;
            }
        }

        private void DeselectAll()
        {
            foreach (Module module in Modules)
            {
                module.ShouldExport = false;
            }
        }

        private void ExportSelectedModules()
        {
            foreach (Module module in Modules)
            {
                if (!module.ShouldExport) continue;

                AssetDatabase.ExportPackage(module.PathsToInclude.ToArray(), string.Format("{0}.unitypackage", module.Name), ExportPackageOptions.Recurse);
                Debug.LogFormat("Exported {0}", module.Name);
            }
        }
    }
}
