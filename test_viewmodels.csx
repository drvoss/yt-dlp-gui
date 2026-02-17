using System;
using YtDlpGui.ViewModels;

var uiState = new UIStateViewModel();
Console.WriteLine("Testing UIStateViewModel ImageHeight calculation...");

uiState.ImageWidth = 1920;
Console.WriteLine($"ImageWidth = 1920 -> ImageHeight = {uiState.ImageHeight} (Expected: 1080)");

if (Math.Abs(uiState.ImageHeight - 1080) < 0.01) {
    Console.WriteLine("✅ ImageHeight auto-calculation PASSED");
} else {
    Console.WriteLine("❌ ImageHeight auto-calculation FAILED");
}

// Test all ViewModels instantiation
Console.WriteLine("\nTesting ViewModel instantiation...");
try {
    var update = new UpdateViewModel();
    Console.WriteLine("✅ UpdateViewModel");
    
    var windowState = new WindowStateViewModel();
    Console.WriteLine("✅ WindowStateViewModel");
    
    var network = new NetworkSettingsViewModel();
    Console.WriteLine("✅ NetworkSettingsViewModel");
    
    var downloadProgress = new DownloadProgressViewModel();
    Console.WriteLine("✅ DownloadProgressViewModel");
    
    var outputPath = new OutputPathViewModel();
    Console.WriteLine("✅ OutputPathViewModel");
    
    var formatSelection = new FormatSelectionViewModel();
    Console.WriteLine("✅ FormatSelectionViewModel");
    
    var downloadOptions = new DownloadOptionsViewModel();
    Console.WriteLine("✅ DownloadOptionsViewModel");
    
    var cookieSettings = new CookieSettingsViewModel();
    Console.WriteLine("✅ CookieSettingsViewModel");
    
    var paths = new PathsViewModel();
    Console.WriteLine("✅ PathsViewModel");
    
    Console.WriteLine("\n✅ All 10 ViewModels instantiated successfully!");
    
} catch (Exception ex) {
    Console.WriteLine($"❌ Error: {ex.Message}");
}
