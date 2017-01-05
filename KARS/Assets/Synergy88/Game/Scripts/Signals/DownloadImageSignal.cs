namespace Synergy88
{
    /// <summary>
    /// A request to download an image with given ID from the given URL.
    /// </summary>
    public class DownloadImageSignal
    {
        /// <summary>
        /// The ID of the image to be downloaded.
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// The URL from which the image is to be downloaded.
        /// </summary>
        public string Url { get; set; }
    }
}
