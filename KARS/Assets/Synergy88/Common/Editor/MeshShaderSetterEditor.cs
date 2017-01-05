using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshShaderSetter))]
public class MeshShaderSetterEditor : Editor
{
	private MeshShaderSetter targetComponent;
	
	public MeshShaderSetterEditor () {
	}
	
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		
		targetComponent = (MeshShaderSetter)target;
		
		EditorGUILayout.BeginVertical();
		if(GUILayout.Button("Change Shaders")) {
			Assertion.Assert(targetComponent.GetShaderToSet() != null && targetComponent.GetShaderToSet().Length > 0, "A shader must be specified.");
			ChangeShaders();
		}
		EditorGUILayout.EndVertical();
	}
	
	private void ChangeShaders() {
		string shaderToSet = targetComponent.GetShaderToSet();
		
		Renderer[] renderers = targetComponent.GetComponentsInChildren<Renderer>();
		foreach(Renderer renderer in renderers) {
			renderer.sharedMaterial.shader = Shader.Find(shaderToSet);
			Material[] materials = renderer.sharedMaterials;
			foreach(Material material in materials) {
				material.shader = Shader.Find(shaderToSet);
			}
		}
	}
}
