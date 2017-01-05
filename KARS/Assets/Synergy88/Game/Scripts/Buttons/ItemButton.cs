namespace Synergy88
{
    /// <summary>
    /// This button is for items displayed with ItemViews.
    /// </summary>
    public class ItemButton : Button
    {
        /// <summary>
        /// This should be called when the button is clicked.
        /// This publishes ButtonClickedSignal along with the ID of the button's item.
        /// </summary>
        /// <param name="itemView"></param>
        public void OnClickedButtonWithItem(ItemView itemView)
        {
            Publish(new ButtonClickedSignal()
            {
                ButtonType = _Button,
                Data = itemView.ItemId
            });
        }
    }
}
