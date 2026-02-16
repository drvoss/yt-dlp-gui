using System.Collections.Generic;
using System.ComponentModel;

namespace YtDlpGui.Core.Models {
    public enum ChaptersType {
        None, Split, Segment
    }
    public class Chapters : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        public decimal start_time { get; set; } = 0;
        public string title { get; set; } = string.Empty;
        public decimal end_time { get; set; } = 0;
        public ChaptersType type { get; set; } = ChaptersType.Segment;
    }
}
