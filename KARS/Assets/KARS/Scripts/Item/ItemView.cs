namespace Synergy88
{
    /// <summary>
    /// Base view for items that exposes an ItemId and signal publishing and receipt methods.
    /// </summary>
    public class ItemView : SignalComponent
    {
        /// <summary>
        /// The ID of the item this view is for.
        /// </summary>
        public virtual string ItemId { get; protected set; }
    }
}
