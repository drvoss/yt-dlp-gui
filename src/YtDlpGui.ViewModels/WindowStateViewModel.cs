using PropertyChanged;

namespace YtDlpGui.ViewModels {
    /// <summary>
    /// ViewModel for managing main window state and position
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class WindowStateViewModel {
        /// <summary>
        /// Whether window should always stay on top
        /// </summary>
        public bool AlwaysOnTop { get; set; } = false;

        /// <summary>
        /// Whether to remember window position on close
        /// </summary>
        public bool RememberWindowStatePosition { get; set; } = false;

        /// <summary>
        /// Whether to remember window size on close
        /// </summary>
        public bool RememberWindowStateSize { get; set; } = false;

        /// <summary>
        /// Window top position
        /// </summary>
        public double Top { get; set; } = 0;

        /// <summary>
        /// Window left position
        /// </summary>
        public double Left { get; set; } = 0;

        /// <summary>
        /// Window width
        /// </summary>
        public double Width { get; set; } = 600;

        /// <summary>
        /// Window height
        /// </summary>
        public double Height { get; set; } = 380;

        /// <summary>
        /// UI scale percentage (100 = 100%)
        /// </summary>
        public int Scale { get; set; } = 100;
    }
}
