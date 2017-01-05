using UnityEngine;
using System.Collections.Generic;

public class InputLayerStack : MonoBehaviour {
	
	// the layer at zero is the top of the stack
	[SerializeField]
	private InputLayer[] initialLayers;
	
	// QQQ serialize for debugging only
	[SerializeField]
	private InputLayer topLayer;
	
	// implemented as a list as we will be traversing the list whenever we push or pop
	// QQQ serialize for debugging only
	[SerializeField]
	private List<InputLayer> layerStack;
	
	void Awake() {
		// populate with initial layers
		layerStack = new List<InputLayer>();
		ResetStack();
	}
	
	/**
	 * Resets the input layer stack.
	 * Note that we don't use the name Reset() since it is invoked by Unity editor
	 */
	public void ResetStack() {
		Clear();
		
		for(int i = initialLayers.Length - 1; i >= 0; --i) {
			Assertion.AssertNotNull(initialLayers[i]);
			Push(initialLayers[i]);
		}
	}
	
	/**
	 * Pushes an input layer.
	 */
	public void Push(InputLayer layer) {
		if(!IsEmpty() && layer == Top()) {
			// the specified layer is already at the top
			return;
		}
		
		//Debug.Log("Pushed: " + layer.gameObject.name);
		
		layerStack.Add(layer);
		EnsureModality();
		
		// QQQ for debugging only
		topLayer = Top();
	}
	
	/**
	 * Pops the top layer.
	 */
	public void Pop() {
		if(IsEmpty()) {
			// there are no layers in the stack
			return;
		}
		
		InputLayer layerToBeRemoved = Top();
		layerToBeRemoved.Deactivate();
		
		layerStack.RemoveAt(layerStack.Count - 1);
		EnsureModality();
		
		// QQQ for debugging only
		if(!IsEmpty()) {
			topLayer = Top();
		}
	}
	
	private void EnsureModality() {
		bool activateLayer = true;
		for(int i = layerStack.Count - 1; i >= 0; --i) {
			// the topmost layer is always activated since we set activateLayer as true
			InputLayer layer = layerStack[i];
			if(activateLayer) {
				layer.Activate();
			} else {
				layer.Deactivate();
			}
			
			// if the current layer is modal, then the succeeding layers are deactivated
			if(activateLayer && layer.IsModal) {
				activateLayer = false;
			}
		}
	}
	
	/**
	 * Returns the InputLayer instance that is at the top.
	 */
	public InputLayer Top() {
		Assertion.Assert(!IsEmpty(), "Can't get top if stack is empty.");
		return layerStack[layerStack.Count - 1];
	}
	
	/**
	 * Returns whether or not the stack is empty.
	 */
	public bool IsEmpty() {
		return layerStack.Count == 0;
	}
	
	/**
	 * Clears the stack of layers.
	 */
	public void Clear() {
		while(!IsEmpty()) {
			Pop();
		}
	}
	
	/**
	 * Returns whether or not there is an active layer from the top that could respond to the specified screen touch position.
	 * This is used to block input if a layer does indeed responds to the specified touch position.
	 */
	public bool RespondsToTouchPosition(Vector3 touchPos, InputLayer requesterLayer = null) {
		for(int i = layerStack.Count - 1; i >= 0; --i) {
			InputLayer layer = layerStack[i];
			
			if(layer == requesterLayer) {
				// the layer of the requester has already been reached and it was not blocked by layers above it
				return false;
			}
			
			if(layer.IsActive && layer.RespondsToTouchPosition(touchPos)) {
				return true;
			}
		}
		
		return false;
	}
	
}
