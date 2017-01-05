using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/**
 * An element used by Input Layer to identify whether it responds to a certain touch position or not. It uses colliders for now. We can refactor it to become an interface if needed later on.
 */
public class InputLayerElement : MonoBehaviour {

	[SerializeField]
	private Collider[] touchColliders;
	
	[SerializeField]
	private string referenceCameraName;
	
	[SerializeField] // for debugging purposes
	private Camera referenceCamera;
	
	private RaycastHit hit = new RaycastHit();
	
	/**
	 * Returns whether or not the element responds to the specified touch position.
	 */
	public bool RespondsToTouchPosition(Vector3 touchPos) {
		if(ReferenceCamera == null) {
			// may not be resolved like it is deactivated
			return false;
		}
		
		Ray touchRay = ReferenceCamera.ScreenPointToRay(touchPos);
		foreach(Collider collider in touchColliders) {
			if(collider.Raycast(touchRay, out hit, 1000)) {
				return true;
			}
		}
		
		return false;
	}
	
	private Camera ReferenceCamera {
		get {
			// we lazy initialize so that we don't have a problem when to get this instance
			if(referenceCamera == null) {
				referenceCamera = CommonUtils.GetComponent<Camera>(referenceCameraName);
			}
			
			return referenceCamera;
		}
	}

	/**
	 * Sets the colliders
	 * This was made so that editor code could set this data
	 */
	public void SetColliders(List<Collider> colliderList) {
		this.touchColliders = new Collider[colliderList.Count];

		for(int i = 0; i < colliderList.Count; ++i) {
			this.touchColliders[i] = colliderList[i];
		}
	}
	
}
