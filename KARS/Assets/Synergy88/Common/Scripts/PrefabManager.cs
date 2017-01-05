using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/**
 * A general class that manages prefab instances.
 */
public class PrefabManager : MonoBehaviour {
	
	[SerializeField]
	private SelfManagingSwarmItemManager itemManager;
	
	[SerializeField]
	private PruneData[] pruneDataList;

	[SerializeField]
	private PreloadData[] preloadDataList;
	
	[SerializeField]
	private float pruneIntervalTime = 1.0f;
	
	private IDictionary<string, int> nameToIndexMapping;
	
	private CountdownTimer pruneTimer;
	
	void Awake() {
		// populate moduleMapping
		nameToIndexMapping = new Dictionary<string, int>();
		for(int i = 0; i < itemManager.itemPrefabs.Length; ++i) {
			SwarmItemManager.PrefabItem current = itemManager.itemPrefabs[i];
			nameToIndexMapping[current.prefab.name] = i;
		}

		StartCoroutine(Preload());
	
		if(!(pruneDataList == null || pruneDataList.Length == 0)) {
			pruneTimer = new CountdownTimer(pruneIntervalTime);
		}
	}

	private IEnumerator Preload() {
		// give chance for SwarmItemManager to prepare itself
		yield return true;

		if(this.preloadDataList == null || this.preloadDataList.Length == 0) {
			// nothing to preload
			yield return false;
		}

		for(int i = 0; i < this.preloadDataList.Length; ++i) {
			Preload(this.preloadDataList[i].PrefabName, this.preloadDataList[i].PreloadCount);
		}
	}

	/**
	 * Prunes inactive items
	 * Should be invoked at certain time in the game where frame rate is not important like transitioning to another screen
	 */
	public void Prune() {
		// pruning is done across several frames
		StartCoroutine(PruneItems());
	}
	
	private IEnumerator PruneItems() {
		foreach(PruneData data in pruneDataList) {
			int itemPrefabIndex = nameToIndexMapping[data.PrefabName];
			itemManager.PruneInactiveList(itemPrefabIndex, data.MaxInactiveCount);
			
			yield return 0; // distribute pruning in different frames so that it won't hog down runtime
		}
	}
	
	/**
	 * Requests for a prefab instance.
	 */
	public GameObject Request(string prefabName) {
		Assertion.Assert(nameToIndexMapping.ContainsKey(prefabName)); // "nameToIndexMapping should contain the specified prefab name: " + prefabName
		int prefabIndex = nameToIndexMapping[prefabName];
		return Request(prefabIndex);
	}
	
	/**
	 * Requests for a prefab instance with specified position and orientation.
	 */
	public GameObject Request(string prefabName, Vector3 position, Quaternion rotation) {
		GameObject instantiated = Request(prefabName);
		instantiated.transform.position = position;
		instantiated.transform.rotation = rotation;
		return instantiated;
	}
	
	/**
     * Requests for a prefab instance using the specified index
     */
	public GameObject Request(int prefabIndex) {
		SwarmItem item = itemManager.ActivateItem(prefabIndex);
		return item.gameObject;
	}

	/**
	 * Preloads the specified prefab for a certain amount
	 */
	public void Preload(string prefabName, int count) {
		Assertion.Assert(nameToIndexMapping.ContainsKey(prefabName)); // "nameToIndexMapping should contain the specified prefab name: " + prefabName
		int prefabIndex = nameToIndexMapping[prefabName];
		this.itemManager.Preload(prefabIndex, count);
	}
	
	/**
	 * Kills all active items.
	 */
	public void KillAllActiveItems() {
		itemManager.KillAllActiveItems();
	}
	
	/**
	 * A utility data that describes pruning process of each prefab in the prefab manager.
	 */
	[Serializable]
	public class PruneData {
		
		[SerializeField]
		private string prefabName;
		
		[SerializeField]
		private int maxInactiveCount;

		/**
		 * Sets the values of the data
		 */
		public void Set(string prefabName, int maxInactiveCount) {
			this.prefabName = prefabName;
			this.maxInactiveCount = maxInactiveCount;
		}
		
		public string PrefabName {
			get {
				return prefabName;
			}
		}
		
		public int MaxInactiveCount {
			get {
				return maxInactiveCount;
			}
		}
		
	}

	/**
	 * A data class that contains information on how prefab instances are preloaded
	 */
	[Serializable]
	public class PreloadData {

		[SerializeField]
		private string prefabName;

		[SerializeField]
		private int preloadCount;

		/**
		 * Sets the values of the data class
		 */
		public void Set(string prefabName, int preloadCount) {
			this.prefabName = prefabName;
			this.preloadCount = preloadCount;
		}

		public string PrefabName {
			get {
				return prefabName;
			}
		}

		public int PreloadCount {
			get {
				return preloadCount;
			}
		}

	}
	
	public SelfManagingSwarmItemManager ItemManager {
		get {
			return itemManager;
		}
	}

	/**
	 * Sets the prune data list. May be invoked in editor when populating the prune data.
	 */
	public void SetPruneDataList(PruneData[] pruneDataList) {
		this.pruneDataList = pruneDataList;
	}

	/**
	 * Sets the preload data list
	 */
	public void SetPreloadDataList(PreloadData[] dataList) {
		this.preloadDataList = dataList;
	}
	
	/**
	 * Returns the number of prefabs that the manager can instantiate
	 */
	public int PrefabCount {
		get {
			return itemManager.itemPrefabs.Length;
		}
	}

}

