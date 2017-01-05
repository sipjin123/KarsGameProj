using UnityEngine;

/**
 * Displays a frame rate using a 3D Text.
 */
[RequireComponent(typeof(TextMesh))]
public class FrameRateView : MonoBehaviour {
	private TextMesh text;
	private FrameRate frameRate;
	
	void Start() {
		text = this.GetComponent<TextMesh>();
		this.frameRate = new FrameRate();
	}
	
	void Update() {
		frameRate.Update(Time.deltaTime);
		text.text = this.frameRate.GetFrameRate().ToString();
	}
}
