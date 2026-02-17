using PropertyChanged;

namespace YtDlpGui.ViewModels;

/// <summary>
/// UI state flags for various dialogs and UI elements
/// </summary>
[AddINotifyPropertyChangedInterface]
public class UIStateViewModel
{
    /// <summary>
    /// Whether the About dialog is currently open
    /// </summary>
    public bool IsAbouted { get; set; } = false;
    
    /// <summary>
    /// Whether the Analyze operation is in progress
    /// </summary>
    public bool IsAnalyze { get; set; } = false;
    
    /// <summary>
    /// Whether clipboard monitoring is enabled
    /// </summary>
    public bool IsMonitor { get; set; } = false;
    
    /// <summary>
    /// Whether auto-download after analysis is enabled
    /// </summary>
    public bool AutoDownloadAnalysed { get; set; } = false;
    
    /// <summary>
    /// Whether video package format is selected (combined video+audio)
    /// </summary>
    public bool IsPackage { get; set; } = false;
    
    /// <summary>
    /// Whether chapters are available for the current video
    /// </summary>
    public bool hasChapter { get; set; } = false;
    
    /// <summary>
    /// Whether subtitles are available for the current video
    /// </summary>
    public bool hasSubtitle { get; set; } = false;
    
    private double _imageWidth = 0;
    
    /// <summary>
    /// Thumbnail image width for 16:9 aspect ratio binding
    /// </summary>
    public double ImageWidth 
    { 
        get => _imageWidth;
        set
        {
            _imageWidth = value;
            ImageHeight = value * 0.5625d; // Auto-calculate height for 16:9
        }
    }
    
    /// <summary>
    /// Thumbnail image height (automatically calculated from ImageWidth * 0.5625)
    /// </summary>
    public double ImageHeight { get; set; } = 0;
    
    /// <summary>
    /// Text displayed on the execute/download button
    /// </summary>
    public string ExecText { get; set; } = string.Empty;
}
