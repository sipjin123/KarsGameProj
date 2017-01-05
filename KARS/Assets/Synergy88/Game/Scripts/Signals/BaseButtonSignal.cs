namespace Synergy88
{
    /// <summary>
    /// Base signal for all button events.
    /// </summary>
    public class BaseButtonSignal
    {
        /// <summary>
        /// The type of button the signal is for.
        /// </summary>
        public EButtonType ButtonType { get; set; }

        /// <summary>
        /// Optional button signal data.
        /// </summary>
        public object Data { get; set; }
    }
}
