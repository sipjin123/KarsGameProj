using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;

using Common;
using Common.Signal;
using Common.Query;

namespace Synergy88
{
	public class ShopItem : ItemView
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
        /// <param name="data">Shop item data</param>
		public void SetupView(ShopItemData data)
        {
            ItemId = data.ItemId;

            this.labelTitle.text = data.ItemId;
            this.labelStoreId.text = data.ItemStoreId;
            this.labelPrice.text = data.ItemPrice;
        }
	}
}