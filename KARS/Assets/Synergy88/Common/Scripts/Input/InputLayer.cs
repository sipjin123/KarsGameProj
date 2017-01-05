using UnityEngine;
using System.Collections;

public class InputLayer : MonoBehaviour {
	
	[SerializeField]
	private bool modal = false;
	
	[SerializeField]
	private bool active = false;
	
	[SerializeField]
	private InputLayerElement[] elements;
	
	/**
	 * Returns whether or not the input layer is modal or not.
	 */
	public bool IsModal {
		get {
			return modal;
		}
	}
	
	/**
	 * Returns whether or not the input layer is active.
	 */
	public bool IsActive {
		get {
			return active;
		}
	}
	
	/**
	 * Activates the input layer.
	 */
	public void Activate() {
		active = true;
	}
	
	/**
	 * Deactivates the input layer.
	 */
	public void Deactivate() {
		active = false;
	}
	
	/**
	 * Returns whether or not the layer responds to the specified screen touch. Note that the specified Vector3 is treated as screen position.
	 */
	public bool RespondsToTouchPosition(Vector3 touchPos, InputLayer requesterLayer = null) {
		if(this == requesterLayer) {
			// this is already the layer where requester can be found on
			return false;
		}
		
		foreach(InputLayerElement element in elements) {
			if(element.RespondsToTouchPosition(touchPos)) {
				return true;
			}
		}
		
		return false;
	}
	
}
