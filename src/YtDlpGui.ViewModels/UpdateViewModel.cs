using PropertyChanged;
using System.Collections.Generic;
using YtDlpGui.Services.Web;

namespace YtDlpGui.ViewModels {
    /// <summary>
    /// ViewModel for managing application update checks and release information
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class UpdateViewModel {
        /// <summary>
        /// Last known version from GitHub release check
        /// </summary>
        public string LastVersion { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of last update check
        /// </summary>
        public string LastCheckUpdate { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether a new version is available
        /// </summary>
        public bool NewVersion { get; set; } = false;

        /// <summary>
        /// List of GitHub release data
        /// </summary>
        public List<GitRelease> ReleaseData { get; set; } = new();

        /// <summary>
        /// HTML content for release notes display
        /// </summary>
        public string Html { get; set; } = string.Empty;
    }
}
