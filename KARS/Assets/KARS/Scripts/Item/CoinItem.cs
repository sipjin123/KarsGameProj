using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using Common.Query;
using Common.Signal;

namespace Synergy88
{
	public class CoinItem : ItemView
    {
		[SerializeField]
		private Text labelTitle; 

		[SerializeField]
		private Text labelStoreId; 

		[SerializeField]
		private Text labelPrice; 

        private void Awake()
        {
			Assertion.AssertNotNull(this.labelTitle);
			Assertion.AssertNotNull(this.labelStoreId);
			Assertion.AssertNotNull(this.labelPrice);
		}

        /// <summary>
        /// Setup item view based on given data.
        /// </summary>
        /// <param name="data">Coin item data</param>
		public void SetupView(CoinItemData data)
        {
            ItemId = data.ItemId;

            this.labelTitle.text = data.ItemId;
            this.labelStoreId.text = data.ItemStoreId;
            this.labelPrice.text = data.ItemPrice;
        }
	}
}