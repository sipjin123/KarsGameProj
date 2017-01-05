using UnityEngine.Purchasing;

namespace Synergy88
{
    /// <summary>
    /// Store item purchase was successful.
    /// </summary>
    public class StoreItemPurchaseSuccessSignal
    {
        /// <summary>
        /// Data for purchased item.
        /// </summary>
        public Product Item { get; set; }
    }
}
