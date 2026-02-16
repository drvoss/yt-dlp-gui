using PropertyChanged;

namespace YtDlpGui.ViewModels {
    /// <summary>
    /// ViewModel for network and proxy settings
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class NetworkSettingsViewModel {
        /// <summary>
        /// Whether proxy is enabled
        /// </summary>
        public bool ProxyEnabled { get; set; } = false;

        /// <summary>
        /// Proxy server URL
        /// </summary>
        public string ProxyUrl { get; set; } = string.Empty;
    }
}
