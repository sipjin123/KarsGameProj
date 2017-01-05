using UnityEngine;
using System.Collections.Generic;

using Common.Utils;

/**
 * A custom SwarmItemManager for Shopping game.
 */
public class SelfManagingSwarmItemManager : SwarmItemManager {

	void Awake() {
		Initialize();
	}
	
	// Update is called once per frame
	void Update() {
		FrameUpdate();
	}
	
	/**
	 * Kills all active items.
	 */
	public void KillAllActiveItems() {
		int length = this._prefabItemLists.Length;
		for(int i = 0; i < length; ++i) {
			// only bother if the active list has some items
			if (_prefabItemLists[i].activeItems.Count <= 0) {
				continue;
			}
			
			// we don't iterate through the active list using foreach here
			// because there would be errors if the item was killed in its FrameUpdate method.
			// instead we manually move to the next linkedlist nod/
			
			LinkedListNode<SwarmItem> item = null;
			LinkedListNode<SwarmItem> nextItem = null;
			
			item = _prefabItemLists[i].activeItems.First;
			
			// while there are items left to process
			while(item != null) {
				// cache the next item because the current item will be killed
				nextItem = item.Next;
				DeactiveItem(item.Value);
				item = nextItem;
			}
		}
	}
	
	/**
	 * Prunes the inactive list to the specified max count.
	 */
	public void PruneInactiveList(int itemPrefabIndex, int maxCount) {
		Assertion.Assert(maxCount > 0, "Max count can't be zero.");
		
		int numToPrune = _prefabItemLists[itemPrefabIndex].inactiveItems.Count - maxCount;
		if(numToPrune <= 0) {
			// nothing to prune
			// the number of inactive items does not even reach the maxCount
			return;
		}
		
		this.PruneList(itemPrefabIndex, numToPrune);
	}

	// we need this to store activated SwarmItem temporarily
	// they will be kill right after
	// we can't Activate() then Kill() because it will only reuse the killed item
	private SimpleList<SwarmItem> preloadList;

	/**
	 * Preloads a certain prefab to a certain amount
	 */
	public void Preload(int itemPrefabIndex, int preloadCount) {
		if(_prefabItemLists[itemPrefabIndex].inactiveItems.Count >= preloadCount) {
			// there are already enough inactive items
			// no need to proceed
			return;
		}

		// we lazy initialize because not all item managers needs preloading
		if(this.preloadList == null) {
			this.preloadList = new SimpleList<SwarmItem>();
		}

		this.preloadList.Clear();

		// note here that we only preload the remaining amount
		// to preload, we need to passes
		// first to activate items
		// then another pass to kill them
		// we can't simple Activate() then Kill() because it will just reuse the previously killed unit
		int remaining = preloadCount - _prefabItemLists[itemPrefabIndex].inactiveItems.Count;
		for(int i = 0; i < remaining; ++i) {
			SwarmItem item = ActivateItem(itemPrefabIndex);
			this.preloadList.Add(item); // add temporarily so we could kill later
		}

		for(int i = 0; i < this.preloadList.Count; ++i) {
			this.preloadList[i].Kill();
		}

		this.preloadList.Clear();
	}
	
}
