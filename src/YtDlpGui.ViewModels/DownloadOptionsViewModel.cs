using PropertyChanged;
using YtDlpGui.Core.Models;

namespace YtDlpGui.ViewModels {
    /// <summary>
    /// ViewModel for download options and post-processing settings
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class DownloadOptionsViewModel {
        /// <summary>
        /// Whether to save video thumbnail
        /// </summary>
        public bool SaveThumbnail { get; set; } = true;

        /// <summary>
        /// Whether to embed subtitles (legacy)
        /// </summary>
        public bool EmbedSub { get; set; } = false;

        /// <summary>
        /// Whether to embed thumbnail in video file
        /// </summary>
        public bool EmbedThumbnail { get; set; } = false;

        /// <summary>
        /// Whether to embed chapters in video file
        /// </summary>
        public bool EmbedChapters { get; set; } = false;

        /// <summary>
        /// Whether to embed subtitles in video file
        /// </summary>
        public bool EmbedSubtitles { get; set; } = false;

        /// <summary>
        /// Whether to use aria2c for downloading
        /// </summary>
        public bool UseAria2 { get; set; } = true;

        /// <summary>
        /// Whether to use system notifications
        /// </summary>
        public bool UseNotifications { get; set; } = true;

        /// <summary>
        /// Download rate limit (e.g., "1M", "500K")
        /// </summary>
        public string LimitRate { get; set; } = string.Empty;

        /// <summary>
        /// Time range for download sections
        /// </summary>
        public string TimeRange { get; set; } = string.Empty;

        /// <summary>
        /// File modification time setting
        /// </summary>
        public ModifiedType ModifiedType { get; set; } = ModifiedType.Modified;
    }
}
