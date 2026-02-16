using PropertyChanged;

namespace YtDlpGui.ViewModels;

[AddINotifyPropertyChangedInterface]
public class PathsViewModel
{
    public string PathYTDLP { get; set; } = string.Empty;
    public string PathAria2 { get; set; } = string.Empty;
    public string PathFFMPEG { get; set; } = string.Empty;
    public string PathTEMP { get; set; } = string.Empty;
    public string PathNotify { get; set; } = string.Empty;
}
