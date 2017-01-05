using UnityEngine;
using UnityEngine.Purchasing;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Query;
using Common.Signal;
using Common.Utils;

using UniRx;

namespace Synergy88 {
	
	public class ShopRoot : Scene {

		[SerializeField]
		private GameObject template;

		[SerializeField]
		private List<ShopItemData> items;

		protected override void Awake() {
			base.Awake();
			Assertion.AssertNotNull(this.template);

			this.AddButtonHandler(EButtonType.ShopItem, (ButtonClickedSignal signal) => {
				string itemId = (string)signal.Data;
                PurchaseItem(itemId);
			});
        }

        protected override void Start() {
			base.Start();

			//this.PopulateItems();
		}

		protected override void OnEnable() {
			base.OnEnable();
			this.StartCoroutine(this.ProcessStoreItems());

            this.Receive<StoreItemPurchaseSuccessSignal>()
                .Subscribe(sig => OnStorePurchaseSuccessful(sig.Item))
                .AddTo(OnDisableDisposables);

            this.Receive<StoreItemPurchaseFailureSignal>()
                .Subscribe(_ => OnStorePurchaseFailed())
                .AddTo(OnDisableDisposables);
		}

		protected override void OnDisable() {
			base.OnDisable();
			this.StopAllCoroutines();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
		}

		private IEnumerator ProcessStoreItems() {
			yield return new WaitForSeconds(1.0f);

			if (!QuerySystem.Query<bool>(QueryIds.StoreIsReady)) {
				yield break;
			}
				
			//IEnumerable<Product> products = QuerySystem.Query<IEnumerable<Product>>(QueryIds.StoreItems);
            // create request for store items
			IQueryRequest request = QuerySystem.Start(QueryIds.StoreItemsWithType);
            // set store item type parameter to non-consumable
			request.AddParameter(QueryIds.StoreItemType, ProductType.NonConsumable);
            // get store items from request
			IEnumerable<Product> products = QuerySystem.Complete<IEnumerable<Product>>();

			foreach (Product product in products) {
				ProductMetadata meta = product.metadata;
				ProductDefinition definition = product.definition;
				//Debug.LogFormat("ShopRoot::ProcessStoreItems Product Id:{0} StoreId:{1} Details:{2}\n", definition.id, definition.storeSpecificId, meta.ToString());

				// push rpdocut to items
				if (!this.items.Exists(p => p.ItemId.Equals(definition.id))) {
					Debug.LogFormat("ShopRoot::ProcessStoreItems Product:{0}\n", product.definition.ToString());
					ShopItemData item = new ShopItemData();
					item.ItemId = definition.id;
					item.ItemStoreId = definition.storeSpecificId;
					item.ItemPrice = meta.localizedPriceString;
					this.items.Add(item);
				}
			}

			// propulate views
			this.PopulateItems();
		}

		public void PopulateItems() {
			Transform parent = this.template.transform.parent;
			Vector3 localScale = this.template.transform.localScale;
			int len = this.items.Count;
			int children = parent.childCount - 1;;
			bool create = len == children;

			for (int i = 0; i < len; i++) {
				ShopItemData item = null;
				GameObject itemObject = null;
				ShopItem gameItem = null;

				try {
					item = this.items[i];
					itemObject = parent.GetChild(i + 1).gameObject;
					gameItem = itemObject.GetComponent<ShopItem>();
				}
				catch (UnityException e) {
					item = this.items[i];
					itemObject = (GameObject)GameObject.Instantiate(this.template);
					gameItem = itemObject.GetComponent<ShopItem>();
				}

				gameItem.SetupView(item);

				// display item
				itemObject.transform.SetParent(parent);
				itemObject.transform.localScale = localScale;
				itemObject.SetActive(true);
			}
        }

        public void PurchaseItem(string itemId)
        {
            if (QuerySystem.Query<bool>(QueryIds.PurchaseInProgress))
            {
                return;
            }

            this.Publish(new PurchaseStoreItemSignal() { ItemId = itemId });
        }

        #region Signals

        private void OnStorePurchaseSuccessful(Product product) {
			Debug.LogFormat("ShopRoot::OnStorePurchaseSuccessful ItemId:{0}\n", product.definition.id);
		}

		private void OnStorePurchaseFailed() {
			Debug.LogFormat("ShopRoot::OnStorePurchaseFailed\n");
		}
		
		#endregion

	}

}