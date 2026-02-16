using PropertyChanged;

namespace YtDlpGui.ViewModels {
    /// <summary>
    /// ViewModel for managing output file paths and names
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class OutputPathViewModel {
        /// <summary>
        /// Target directory path for downloads
        /// </summary>
        public string TargetPath { get; set; } = string.Empty;

        /// <summary>
        /// Target filename without path
        /// </summary>
        public string TargetName { get; set; } = string.Empty;

        /// <summary>
        /// Full target file path (TargetPath + TargetName)
        /// </summary>
        public string TargetFile { get; set; } = string.Empty;

        /// <summary>
        /// Display-friendly target path (with special paths replaced)
        /// </summary>
        public string TargetDisplay { get; set; } = string.Empty;

        /// <summary>
        /// URL or path to video thumbnail
        /// </summary>
        public string? Thumbnail { get; set; } = null;
    }
}
