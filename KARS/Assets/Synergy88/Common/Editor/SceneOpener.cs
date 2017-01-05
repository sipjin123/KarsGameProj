using System.Collections;

using UnityEngine;
using UnityEditor;

public class SceneOpener : EditorWindow {

	[MenuItem("Window/SceneOpener #%r")]
	public static void Init() {
		SceneOpener window = EditorWindow.GetWindow<SceneOpener>();
	}
	
	private string sceneToOpen = "";
	
	private const string SCENE_CONTROL_NAME = "SceneToOpen";
	
	void OnGUI() {
		GUILayout.BeginVertical();
		GUILayout.Label("Scene Opener");
		
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal(GUILayout.Width(10));
		GUILayout.Label("Scene to open: ");
		GUI.SetNextControlName(SCENE_CONTROL_NAME);
		this.sceneToOpen = EditorGUILayout.TextField(this.sceneToOpen, GUILayout.Width(150));
		GUILayout.EndHorizontal();
		
		GUILayout.Space(10);
		
		ListScenes();
		
		GUILayout.EndVertical();
		
		if(string.IsNullOrEmpty(GUI.GetNameOfFocusedControl())) {
			EditorGUI.FocusTextInControl(SCENE_CONTROL_NAME);
		}
		
		CheckKeyEvents();
	}
	
	private void ListScenes() {
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if(string.IsNullOrEmpty(this.sceneToOpen)) {
				// no need to filter if scene to open is empty
				GUILayout.Label(scene.path);
			} else if(scene.path.ToLower().Contains(this.sceneToOpen.ToLower())) {
				GUILayout.Label(scene.path);
			}
		}
	}
	
	private void CheckKeyEvents() {
		Event e = Event.current;
		
		switch(e.type) {
		case EventType.keyDown:
			if(e.keyCode == KeyCode.Return) {
				LoadScene();
			}
			break;
		}
	}
	
	private void LoadScene() {
		if(string.IsNullOrEmpty(this.sceneToOpen)) {
			// don't load if scene to load is not specified
			return;
		}
		
		// load the first scene that meets the criteria
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if(scene.path.ToLower().Contains(this.sceneToOpen.ToLower())) {
				EditorApplication.SaveCurrentSceneIfUserWantsTo();
				EditorApplication.OpenScene(scene.path);
				this.Close();
				return;
			}
		}
	}

}
