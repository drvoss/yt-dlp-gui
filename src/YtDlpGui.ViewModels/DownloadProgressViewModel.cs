using PropertyChanged;
using Swordfish.NET.Collections;
using System.Collections.Generic;

namespace YtDlpGui.ViewModels {
    /// <summary>
    /// ViewModel for managing download progress and status
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class DownloadProgressViewModel {
        /// <summary>
        /// Whether a download is currently in progress
        /// </summary>
        public bool IsDownload { get; set; } = false;

        /// <summary>
        /// Whether the download can be cancelled
        /// </summary>
        public bool CanCancel { get; set; } = false;

        /// <summary>
        /// Video download progress percentage
        /// </summary>
        public decimal VideoPersent { get; set; } = 0;

        /// <summary>
        /// Video download estimated time remaining
        /// </summary>
        public string VideoETA { get; set; } = "0:00";

        /// <summary>
        /// Audio download progress percentage
        /// </summary>
        public decimal AudioPersent { get; set; } = 0;

        /// <summary>
        /// Audio download estimated time remaining
        /// </summary>
        public string AudioETA { get; set; } = "0:00";

        /// <summary>
        /// Subtitle download progress percentage
        /// </summary>
        public decimal SubtitlePersent { get; set; } = 0;

        /// <summary>
        /// Video download status details
        /// </summary>
        public DownloadStatus DNStatus_Video { get; set; } = new();

        /// <summary>
        /// Audio download status details
        /// </summary>
        public DownloadStatus DNStatus_Audio { get; set; } = new();

        /// <summary>
        /// Download status information dictionary (speed, size, etc.)
        /// </summary>
        public ConcurrentObservableDictionary<string, string> DNStatus_Infos { get; set; } = new();

        /// <summary>
        /// View-friendly representation of download status info
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> DNStatus_InfosView
            => DNStatus_Infos.CollectionView;
    }

    /// <summary>
    /// Represents download status for a single stream (video/audio)
    /// </summary>
    public class DownloadStatus {
        public decimal Persent { get; set; } = 0;
        public decimal Downloaded { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public decimal Speed { get; set; } = 0;
        public decimal Elapsed { get; set; } = 0;
    }
}
