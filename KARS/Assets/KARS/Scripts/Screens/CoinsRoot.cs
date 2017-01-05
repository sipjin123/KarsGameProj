using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Common;
using Common.Signal;
using Common.Utils;
using Common.Query;

using UniRx;

namespace Synergy88 {

	public class CoinsRoot : Scene {

		[SerializeField]
		private GameObject template;

		[SerializeField]
		private List<CoinItemData> items;

		protected override void Awake() {
			base.Awake();
			Assertion.AssertNotNull(this.template);

			this.AddButtonHandler(EButtonType.CoinItem, (ButtonClickedSignal signal) => {
				string itemId = (string)signal.Data;
				PurchaseItem(itemId);
			});
        }

        protected override void Start() {
			base.Start();
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
            // set store item type parameter to consumable
			request.AddParameter(QueryIds.StoreItemType, ProductType.Consumable);
            // get store items from request
            IEnumerable<Product> products = QuerySystem.Complete<IEnumerable<Product>>();

			foreach (Product product in products) {
				ProductMetadata meta = product.metadata;
				ProductDefinition definition = product.definition;
				//Debug.LogFormat("ShopRoot::ProcessStoreItems Product Id:{0} StoreId:{1} Details:{2}\n", definition.id, definition.storeSpecificId, meta.ToString());

				// push rpdocut to items
				if (!this.items.Exists(p => p.ItemId.Equals(definition.id))) {
					CoinItemData item = new CoinItemData();
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
				CoinItemData item = null;
				GameObject itemObject = null;
				CoinItem gameItem = null;

				try {
					item = this.items[i];
					itemObject = parent.GetChild(i + 1).gameObject;
					gameItem = itemObject.GetComponent<CoinItem>();
				}
				catch (UnityException e) {
					item = this.items[i];
					itemObject = (GameObject)GameObject.Instantiate(this.template);
					gameItem = itemObject.GetComponent<CoinItem>();
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
			Debug.LogFormat("CoinsRoot::OnStorePurchaseSuccessful ItemId:{0}\n", product.definition.id);
		}

		private void OnStorePurchaseFailed() {
			Debug.LogFormat("CoinsRoot::OnStorePurchaseFailed\n");
		}

		#endregion
	}

}