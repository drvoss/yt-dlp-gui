using PropertyChanged;
using YtDlpGui.Core.Models;

namespace YtDlpGui.ViewModels {
    /// <summary>
    /// ViewModel for managing format selection (video, audio, subtitles, chapters)
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class FormatSelectionViewModel {
        /// <summary>
        /// Selected video format
        /// </summary>
        public Format selectedVideo { get; set; } = new();

        /// <summary>
        /// Selected audio format
        /// </summary>
        public Format selectedAudio { get; set; } = new();

        /// <summary>
        /// Selected chapter
        /// </summary>
        public Chapters? selectedChapter { get; set; } = null;

        /// <summary>
        /// Selected subtitle
        /// </summary>
        public Subs selectedSub { get; set; } = new();

        /// <summary>
        /// Whether to use format specification from config file
        /// </summary>
        public bool UseFormat { get; set; } = true;

        /// <summary>
        /// Whether to remux video (change container without re-encoding)
        /// </summary>
        public bool RemuxVideo { get; set; } = false;

        /// <summary>
        /// Get the appropriate file extension based on selected formats
        /// </summary>
        public string GetOriginExtension() {
            if (selectedVideo != null && selectedAudio != null) {
                if (selectedVideo.type == FormatType.package) {
                    return selectedVideo.video_ext.ToLower().Trim('.');
                } else if (selectedVideo.video_ext == "webm" && selectedAudio.audio_ext == "webm") {
                    return "webm";
                } else if (selectedVideo.video_ext == "mp4" && selectedAudio.audio_ext == "m4a") {
                    return "mp4";
                }
            }
            return "mkv";
        }
    }
}
