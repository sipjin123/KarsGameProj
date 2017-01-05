using UnityEngine;

namespace Synergy88
{
    /// <summary>
    /// A downloaded image with given ID.
    /// </summary>
    public class ImageDownloadedSignal
    {
        /// <summary>
        /// The ID of the downloaded image.
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// The downloaded image.
        /// </summary>
        public Texture2D Texture { get; set; }
    }
}
