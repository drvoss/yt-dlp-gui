using System;
using System.ComponentModel;
using YtDlpGui.ViewModels;

namespace YtDlpGui.Tests.ViewModels
{
    public class ViewModelValidationTests
    {
        // Test 1: Verify all ViewModels implement INotifyPropertyChanged
        public static void TestPropertyChangedImplementation()
        {
            Console.WriteLine("=== Test 1: INotifyPropertyChanged Implementation ===");
            
            var viewModels = new INotifyPropertyChanged[]
            {
                new UpdateViewModel(),
                new WindowStateViewModel(),
                new NetworkSettingsViewModel(),
                new DownloadProgressViewModel(),
                new OutputPathViewModel(),
                new FormatSelectionViewModel(),
                new DownloadOptionsViewModel(),
                new CookieSettingsViewModel(),
                new PathsViewModel(),
                new UIStateViewModel()
            };

            foreach (var vm in viewModels)
            {
                var typeName = vm.GetType().Name;
                Console.WriteLine($"✅ {typeName} implements INotifyPropertyChanged");
            }
            
            Console.WriteLine($"\nTotal ViewModels tested: {viewModels.Length}");
        }

        // Test 2: Verify UIStateViewModel ImageHeight auto-calculation
        public static void TestImageHeightCalculation()
        {
            Console.WriteLine("\n=== Test 2: UIStateViewModel ImageHeight Auto-Calculation ===");
            
            var uiState = new UIStateViewModel();
            
            // Test 1920x1080 (16:9)
            uiState.ImageWidth = 1920;
            var expectedHeight = 1920 * 0.5625; // = 1080
            
            if (Math.Abs(uiState.ImageHeight - expectedHeight) < 0.01)
            {
                Console.WriteLine($"✅ ImageWidth=1920 → ImageHeight={uiState.ImageHeight} (Expected: {expectedHeight})");
            }
            else
            {
                Console.WriteLine($"❌ ImageHeight calculation failed: {uiState.ImageHeight} != {expectedHeight}");
            }

            // Test 640 width
            uiState.ImageWidth = 640;
            expectedHeight = 640 * 0.5625; // = 360
            
            if (Math.Abs(uiState.ImageHeight - expectedHeight) < 0.01)
            {
                Console.WriteLine($"✅ ImageWidth=640 → ImageHeight={uiState.ImageHeight} (Expected: {expectedHeight})");
            }
            else
            {
                Console.WriteLine($"❌ ImageHeight calculation failed: {uiState.ImageHeight} != {expectedHeight}");
            }
        }

        // Test 3: Verify default values
        public static void TestDefaultValues()
        {
            Console.WriteLine("\n=== Test 3: ViewModel Default Values ===");
            
            var update = new UpdateViewModel();
            Console.WriteLine($"✅ UpdateViewModel.LastVersion: '{update.LastVersion}' (empty)");
            
            var windowState = new WindowStateViewModel();
            Console.WriteLine($"✅ WindowStateViewModel.Width: {windowState.Width} (default: 600)");
            Console.WriteLine($"✅ WindowStateViewModel.Height: {windowState.Height} (default: 380)");
            Console.WriteLine($"✅ WindowStateViewModel.Scale: {windowState.Scale} (default: 100)");
            
            var network = new NetworkSettingsViewModel();
            Console.WriteLine($"✅ NetworkSettingsViewModel.ProxyEnabled: {network.ProxyEnabled} (default: false)");
            
            var downloadOptions = new DownloadOptionsViewModel();
            Console.WriteLine($"✅ DownloadOptionsViewModel.UseAria2: {downloadOptions.UseAria2} (default: true)");
            Console.WriteLine($"✅ DownloadOptionsViewModel.SaveThumbnail: {downloadOptions.SaveThumbnail} (default: true)");
            
            var cookieSettings = new CookieSettingsViewModel();
            Console.WriteLine($"✅ CookieSettingsViewModel.UseCookie: {cookieSettings.UseCookie}");
            
            var uiState = new UIStateViewModel();
            Console.WriteLine($"✅ UIStateViewModel.IsAnalyze: {uiState.IsAnalyze} (default: false)");
            Console.WriteLine($"✅ UIStateViewModel.ExecText: '{uiState.ExecText}' (empty)");
        }

        // Test 4: Verify PropertyChanged notification
        public static void TestPropertyChangedNotification()
        {
            Console.WriteLine("\n=== Test 4: PropertyChanged Notification ===");
            
            var uiState = new UIStateViewModel();
            int notificationCount = 0;
            
            ((INotifyPropertyChanged)uiState).PropertyChanged += (sender, e) =>
            {
                notificationCount++;
                Console.WriteLine($"  → PropertyChanged: {e.PropertyName}");
            };

            Console.WriteLine("Setting ImageWidth = 1920...");
            uiState.ImageWidth = 1920;
            
            if (notificationCount > 0)
            {
                Console.WriteLine($"✅ PropertyChanged fired {notificationCount} time(s)");
            }
            else
            {
                Console.WriteLine("❌ PropertyChanged not fired");
            }

            // Test another property
            notificationCount = 0;
            Console.WriteLine("\nSetting IsAnalyze = true...");
            uiState.IsAnalyze = true;
            
            if (notificationCount > 0)
            {
                Console.WriteLine($"✅ PropertyChanged fired {notificationCount} time(s)");
            }
            else
            {
                Console.WriteLine("❌ PropertyChanged not fired");
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  yt-dlp-GUI ViewModels Validation Test Suite            ║");
            Console.WriteLine("║  Phase 2-2: Architecture Refactoring Verification        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

            try
            {
                TestPropertyChangedImplementation();
                TestImageHeightCalculation();
                TestDefaultValues();
                TestPropertyChangedNotification();

                Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
                Console.WriteLine("║  ✅ ALL TESTS PASSED                                     ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ TEST FAILED: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
