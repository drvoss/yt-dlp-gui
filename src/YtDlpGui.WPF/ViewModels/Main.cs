using Swordfish.NET.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using YamlDotNet.Serialization;
using YtDlpGui.Core.Models;
using YtDlpGui.Services.Configuration;
using YtDlpGui.Services.Web;
using YtDlpGui.ViewModels;
using YtDlpGui.WPF.Libs;

namespace YtDlpGui.WPF.Views {
    public partial class Main :Window {
        public class ViewData :INotifyPropertyChanged {
            public event PropertyChangedEventHandler? PropertyChanged;
            public ViewData() {
                Chapters.PropertyChanged += (s, e) => {
                    switch (e.PropertyName) {
                        case nameof(ConcurrentObservableCollection<Format>.CollectionView):
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChaptersView)));
                            break;
                    }
                };
                Formats.PropertyChanged += (s, e) => {
                    switch (e.PropertyName) {
                        case nameof(ConcurrentObservableCollection<Format>.CollectionView):
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FormatsVideo)));
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FormatsAudio)));
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FormatsView)));
                            break;
                    }
                };
                Thumbnails.PropertyChanged += (s, e) => {
                    switch (e.PropertyName) {
                        case nameof(ConcurrentObservableCollection<Format>.CollectionView):
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThumbnailsView)));
                            break;
                    }
                };
                Subtitles.PropertyChanged += (s, e) => {
                    switch (e.PropertyName) {
                        case nameof(ConcurrentObservableCollection<Format>.CollectionView):
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubtitlesView)));
                            break;
                    }
                };
                DownloadProgress.DNStatus_Infos.PropertyChanged += (s, e) => {
                    switch (e.PropertyName) {
                        case nameof(ConcurrentObservableDictionary<string, string>.CollectionView):
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadProgress.DNStatus_InfosView)));
                            break;
                    }
                };
                PropertyChanged += SelfData_PropertyChanged;
            }
            private Regex _isComment = new Regex(@"^\s*#");
            private Regex _hasOutput = new Regex(@"(-o|--output)\s+");
            private Regex _hasFormat = new Regex(@"(-f|--format)\s+");
            private void SelfData_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
                switch (e.PropertyName) {
                    case nameof(selectedConfig):
                        var f = selectedConfig.file;
                        Debug.WriteLine(f, "config file");
                        GUIConfig.ConfigurationFile = f;

                        FormatSelection.UseFormat = true;
                        UseOutput = true;

                        if (File.Exists(f)) {
                            foreach (var line in File.ReadLines(f)) {
                                if (string.IsNullOrWhiteSpace(line) || _isComment.IsMatch(line)) continue;
                                if (_hasFormat.IsMatch(line)) FormatSelection.UseFormat = false;
                                if (_hasOutput.IsMatch(line)) UseOutput = false;
                            }
                        }

                        break;
                    case nameof(FormatSelection):
                        // Update package connection
                        if (FormatSelection.selectedVideo != null && FormatSelection.selectedAudio != null) {
                            if (FormatSelection.selectedVideo.type == FormatType.package) {
                                IsPackage = true;
                                FormatSelection.selectedAudio = FormatSelection.selectedVideo;
                            } else {
                                IsPackage = false;
                                if (FormatSelection.selectedAudio.type == FormatType.package) {
                                    FormatSelection.selectedAudio = FormatsAudio.FirstOrDefault(x => x.type != FormatType.package);
                                }
                            }
                        }
                        CheckExtension();
                        UpdateOutputPath();
                        break;
                    case nameof(OutputPath):
                        UpdateOutputPath();
                        break;
                    case nameof(ImageWidth):
                        ImageHeight = ImageWidth * 0.5625d;
                        break;
                }
                CheckEnable();

                if (AutoSaveConfig) Util.PropertyCopy(this, GUIConfig);
            }
            
            private void UpdateOutputPath() {
                if (!string.IsNullOrEmpty(OutputPath.TargetPath)) {
                    if (OutputPath.TargetPath.Last() != Path.DirectorySeparatorChar) {
                        OutputPath.TargetPath += Path.DirectorySeparatorChar;
                    }
                }
                if (!string.IsNullOrEmpty(OutputPath.TargetPath) && !string.IsNullOrEmpty(OutputPath.TargetName)) {
                    OutputPath.TargetFile = Path.Combine(OutputPath.TargetPath, OutputPath.TargetName);
                    OutputPath.TargetDisplay = Util.ReplaceSpecialPath(OutputPath.TargetFile);
                }
            }
            public void SelectFormatBest() {
                FormatSelection.selectedChapter = Chapters.FirstOrDefault();
                var defVideoFmt = FormatsVideo.FirstOrDefault();
                var defAudioFmt = FormatsAudio.FirstOrDefault();
                if (FormatSelection.UseFormat) {
                    FormatSelection.selectedVideo = defVideoFmt;
                    FormatSelection.selectedAudio = defAudioFmt;
                } else {
                    FormatSelection.selectedVideo = FormatsVideo.FirstOrDefault(x => RequestedFormats.Any(r => r.format_id == x.format_id), defVideoFmt);
                    FormatSelection.selectedAudio = FormatsAudio.FirstOrDefault(x => RequestedFormats.Any(r => r.format_id == x.format_id), defAudioFmt);
                }
                FormatSelection.selectedSub = Subtitles.FirstOrDefault();
            }
            public void CheckExtension() {
                if (FormatSelection.RemuxVideo) return;
                if (!string.IsNullOrWhiteSpace(OutputPath.TargetName)) {
                    if (FormatSelection.selectedVideo != null && FormatSelection.selectedAudio != null) {
                        OutputPath.TargetName = Path.ChangeExtension(OutputPath.TargetName, FormatSelection.GetOriginExtension());
                    }
                }
                UpdateOutputPath();
            }
            public string OriginExt => FormatSelection.GetOriginExtension();
            public Lang Lang { get; set; } = new();
            public Video? Video { get; set; } = new();
            public ConcurrentObservableCollection<Config> Configs { get; set; } = new();
            public IEnumerable<Config> ConfigsView => Configs.CollectionView;
            public Config selectedConfig { get; set; } = new();
            public bool UseOutput { get; set; } = true;
            public ConcurrentObservableCollection<Chapters> Chapters { get; set; } = new();
            public IEnumerable<Chapters> ChaptersView => Chapters.CollectionView;
            public ConcurrentObservableCollection<Format> Formats { get; set; } = new();
            public IEnumerable<Format> FormatsView => Formats.CollectionView.OrderBy(x => x.width * x.height);
            public IEnumerable<Format> FormatsVideo => Formats.CollectionView.Where(x => x.type == FormatType.package || x.type == FormatType.video).OrderBy(x => x, ComparerVideo.Comparer);
            public IEnumerable<Format> FormatsAudio => Formats.CollectionView.Where(x => x.type == FormatType.package || x.type == FormatType.audio).OrderBy(x => x, ComparerAudio.Comparer);
            public ConcurrentObservableCollection<Format> RequestedFormats { get; set; } = new();
            public ConcurrentObservableCollection<Thumb> Thumbnails { get; set; } = new();
            public IEnumerable<Thumb> ThumbnailsView => Thumbnails.CollectionView;
            public ConcurrentObservableCollection<Subs> Subtitles { get; set; } = new();
            public IEnumerable<Subs> SubtitlesView => Subtitles.CollectionView;
            public bool hasChapter { get; set; } = false;
            public bool hasSubtitle { get; set; } = false;
            public bool IsAnalyze { get; set; } = false;
            
            // Format selection - extracted to FormatSelectionViewModel
            public FormatSelectionViewModel FormatSelection { get; set; } = new();
            
            // Download progress - extracted to DownloadProgressViewModel
            public DownloadProgressViewModel DownloadProgress { get; set; } = new();
            
            public bool IsAbouted { get; set; } = false;
            public bool IsMonitor { get; set; } = false;
            public bool AlwaysOnTop { get; set; } = false;
            public bool AutoDownloadAnalysed { get; set; } = false;
            
            // Window state - extracted to WindowStateViewModel
            public WindowStateViewModel WindowState { get; set; } = new();
            
            // Network settings - extracted to NetworkSettingsViewModel
            public NetworkSettingsViewModel Network { get; set; } = new();
            
            // Output path - extracted to OutputPathViewModel
            public OutputPathViewModel OutputPath { get; set; } = new();
            
            // Download options - extracted to DownloadOptionsViewModel
            public DownloadOptionsViewModel DownloadOptions { get; set; } = new();
            
            public string Url { get; set; } = string.Empty;
            public string CommandLine { get; set; } = string.Empty;
            public bool IsPackage { get; set; } = false;
            public double ImageWidth { get; set; } = 0; //Binding 16:9
            public double ImageHeight { get; set; } = 0;
            public string ExecText { get; set; } = string.Empty;
            public UseCookie UseCookie { get; set; } = UseCookie.WhenNeeded;
            public CookieType CookieType { get; set; } = CookieType.Chrome;
            public bool NeedCookie { get; set; } = false;
            public Enable Enable { get; set; } = new();
            public bool AutoSaveConfig { get; set; } = false;
            // Update-related properties - extracted to UpdateViewModel
            public UpdateViewModel Update { get; set; } = new();
            
            public string PathYTDLP { get; set; } = string.Empty;
            public string PathAria2 { get; set; } = string.Empty;
            public string PathFFMPEG { get; set; } = string.Empty;
            public string PathTEMP { get; set; } = string.Empty;
            public string PathNotify { get; set; } = string.Empty;
            public GUIConfig GUIConfig { get; set; } = new();
            public string ClipboardText { get; set; } = string.Empty;
            //
            private void CheckEnable() {
                Enable.Url = true;
                Enable.Analyze = true;
                Enable.SelectChapters = true;
                Enable.FormatVideo = true;
                Enable.FormatAudio = true;
                Enable.Download = true;
                Enable.Browser = true;
                Enable.SelectSubtitle = true;
                Enable.UseCookie = true;
                Enable.CookieType = true;
                Enable.SaveThumbnail = true;
                Enable.SaveVideo = true;
                Enable.SaveAudio = true;
                Enable.SaveSubtitle = true;
                Enable.UseNotifications = true;
                Enable.UseAria2 = true;

                if (string.IsNullOrWhiteSpace(Url)) Enable.Analyze = false;
                if (IsAnalyze) {
                    Enable.Url = false;
                    Enable.Analyze = false;
                    Enable.SelectChapters = false;
                    Enable.FormatVideo = false;
                    Enable.FormatAudio = false;
                    Enable.Download = false;
                    Enable.Browser = false;
                    Enable.SelectSubtitle = false;
                    Enable.UseCookie = false;
                    Enable.CookieType = false;
                    Enable.SaveThumbnail = false;
                    Enable.SaveVideo = false;
                    Enable.SaveAudio = false;
                    Enable.SaveSubtitle = false;
                }
                if (DownloadProgress.IsDownload) {
                    Enable.Url = false;
                    Enable.Analyze = false;
                    Enable.SelectChapters = false;
                    Enable.FormatVideo = false;
                    Enable.FormatAudio = false;
                    Enable.Browser = false;
                    Enable.SelectSubtitle = false;
                    Enable.UseCookie = false;
                    Enable.CookieType = false;
                    Enable.SaveThumbnail = false;
                    Enable.SaveVideo = false;
                    Enable.SaveAudio = false;
                    Enable.SaveSubtitle = false;
                    Enable.UseNotifications = false;
                    Enable.UseAria2 = false;
                }
                if (Video.chapters == null) Enable.SelectChapters = false;
                if (!FormatsVideo.Any()) {
                    Enable.FormatVideo = false;
                    Enable.SaveVideo = false;
                }
                if (!FormatsAudio.Any()) {
                    Enable.FormatAudio = false;
                    Enable.SaveAudio = false;
                }
                if (FormatSelection.selectedVideo == null || FormatSelection.selectedAudio == null) {
                    Enable.Download = false;
                    Enable.SaveVideo = false;
                    Enable.SaveAudio = false;
                } else {
                    if (string.IsNullOrWhiteSpace(FormatSelection.selectedVideo.format_id)) {
                        Enable.Download = false;
                        Enable.SaveVideo = false;
                    }
                    if (string.IsNullOrWhiteSpace(FormatSelection.selectedAudio.format_id)) {
                        Enable.Download = false;
                        Enable.SaveAudio = false;
                    }
                    if (FormatSelection.selectedVideo.type == FormatType.package) Enable.FormatAudio = false;
                }
                if (Subtitles.Count <= 1) {
                    Enable.SelectSubtitle = false;
                    Enable.SaveSubtitle = false;
                } else {
                    if (string.IsNullOrWhiteSpace(FormatSelection.selectedSub?.url)) {
                        Enable.SaveSubtitle = false;
                    }
                }
                if (Video.is_live == true) {
                    ExecText = DownloadProgress.IsDownload ? App.Lang.Main.Stop : App.Lang.Main.Record;
                } else {
                    ExecText = DownloadProgress.IsDownload ? App.Lang.Main.Cancel : App.Lang.Main.Download;
                }
            }
        }
        public class Enable :INotifyPropertyChanged {
            public event PropertyChangedEventHandler? PropertyChanged;
            public bool Url { get; set; } = true;
            public bool Analyze { get; set; } = true;
            public bool SelectChapters { get; set; } = true;
            public bool FormatVideo { get; set; } = true;
            public bool FormatAudio { get; set; } = true;
            public bool Download { get; set; } = true;
            public bool Browser { get; set; } = true;
            public bool SelectSubtitle { get; set; } = true;
            public bool UseCookie { get; set; } = true;
            public bool CookieType { get; set; } = true;
            public bool SaveThumbnail { get; set; } = true;
            public bool SaveVideo { get; set; } = true;
            public bool SaveAudio { get; set; } = true;
            public bool SaveSubtitle { get; set; } = true;
            public bool UseNotifications { get; set; } = true;
            public bool UseAria2 { get; set; } = true;
        }
        public class GUIConfig :IYamlConfig, INotifyPropertyChanged {
            public event PropertyChangedEventHandler? PropertyChanged;
            public GUIConfig() {
                PropertyChanged += Config_PropertyChanged;
            }
            private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
                this.Save();
            }
            [Description("Paths")]
            [YamlMember(Order = 1001)] public string TargetPath { get; set; } = string.Empty;
            [YamlMember(Order = 1002)] public string PathYTDLP { get; set; } = string.Empty;
            [YamlMember(Order = 1003)] public string PathAria2 { get; set; } = string.Empty;
            [YamlMember(Order = 1004)] public string PathFFMPEG { get; set; } = string.Empty;
            [YamlMember(Order = 1005)] public string PathTEMP { get; set; } = string.Empty;
            [YamlMember(Order = 1006)] public string PathNotify { get; set; } = string.Empty;

            [Description("Window")]
            [YamlMember(Order = 1101)] public bool AlwaysOnTop { get; set; } = false;
            [YamlMember(Order = 1102)] public bool RememberWindowStatePosition { get; set; } = false;
            [YamlMember(Order = 1103)] public bool RememberWindowStateSize { get; set; } = false;
            [YamlMember(Order = 1104)] public double Top { get; set; } = 0;
            [YamlMember(Order = 1105)] public double Left { get; set; } = 0;
            [YamlMember(Order = 1106)] public double Width { get; set; } = 600;
            [YamlMember(Order = 1107)] public double Height { get; set; } = 380;
            [YamlMember(Order = 1108)] public int Scale { get; set; } = 100;

            [Description("Network")]
            [YamlMember(Order = 1201)] public bool ProxyEnabled { get; set; } = false;
            [YamlMember(Order = 1202)] public string ProxyUrl { get; set; } = string.Empty;
            [YamlMember(Order = 1203)] public UseCookie UseCookie { get; set; } = UseCookie.WhenNeeded;
            [YamlMember(Order = 1204)] public CookieType CookieType { get; set; } = CookieType.Chrome;

            [Description("Advance")]
            [YamlMember(Order = 1301)] public string ConfigurationFile { get; set; } = string.Empty;
            [YamlMember(Order = 1302)] public bool UseAria2 { get; set; } = false;
            [YamlMember(Order = 1303)] public bool EmbedThumbnail { get; set; } = false;
            [YamlMember(Order = 1304)] public bool EmbedChapters { get; set; } = false;
            [YamlMember(Order = 1305)] public bool EmbedSubtitles { get; set; } = false;
            [YamlMember(Order = 1306)] public string LimitRate { get; set; } = string.Empty;
            [YamlMember(Order = 1307)] public ModifiedType ModifiedType { get; set; } = ModifiedType.Modified;

            [Description("Options")]
            [YamlMember(Order = 1401)] public bool IsMonitor { get; set; } = false;
            [YamlMember(Order = 1402)] public bool SaveThumbnail { get; set; } = true;
            [YamlMember(Order = 1403)] public bool UseNotifications { get; set; } = true;
            [YamlMember(Order = 1404)] public bool AutoDownloadAnalysed { get; set; } = false;
            //[Description("Embed Subtitles")] public bool EmbedSub { get; set; } = false;
        }
        public class StatusRepoter {
            public int type = 0;
            private static Regex regPart = new Regex(@"\[download\] Destination:.*\.f(?<fid>\d+(?:-\w+)?)\.\w+");
            private static Regex regDLP = new Regex(@"^\[yt-dlp]");
            private static Regex regAria = new Regex(@"(?<=\[#\w{6}).*?(?<downloaded>[\w]+).*?\/(?<total>[\w]+).*?(?<persent>[\w.]+)%.*?CN:(?<cn>\d+).*DL:(?<speed>\w+)(.*?ETA:(?<eta>\w+))?");
            private static Regex regFF = new Regex(@"frame=.*?(?<frame>\d+).*?fps=.*?(?<fps>[\d.]+).*?size=.*?(?<size>\w+).*?time=(?<time>\S+).*?bitrate=(?<bitrate>\S+)");
            private static Regex regYTDL = new Regex(@"^\[download\].*?(?<persent>[\d\.]+).*?(?<=of).*?(?<total>\S+).*?(?<=at).*?(?<speed>\S+).*?(?<=ETA).*?(?<eta>\S+)");

            private ViewData Data { get; set; }
            private DownloadStatus s { get; set; }
            public StatusRepoter(ViewData data) {
                Data = data;
                s = Data.DownloadProgress.DNStatus_Video;
            }
            public void GetStatus(string std) {
                if (regPart.IsMatch(std)) {
                    var r = Util.GetGroup(regPart, std);
                    if (r.GetValueOrDefault("fid", "0") == Data.FormatSelection.selectedVideo.format_id) {
                        type = 1;
                        s = Data.DownloadProgress.DNStatus_Video;
                    }
                    if (r.GetValueOrDefault("fid", "0") == Data.FormatSelection.selectedAudio.format_id) {
                        type = 2;
                        s = Data.DownloadProgress.DNStatus_Audio; ;
                    }
                }
                if (s != null) {
                    if (regDLP.IsMatch(std)) {
                        // yt-dlp
                        if (!Data.DownloadProgress.DNStatus_Infos.ContainsKey("Downloader")) Data.DownloadProgress.DNStatus_Infos["Downloader"] = App.Lang.Status.Native;
                        var d = std.Split(',');
                        if (decimal.TryParse(d[4], out decimal d_total)) {
                            s.Total = d_total;
                            s.Persent = decimal.Parse(d[3]) / d_total * 100; ;
                        } else {
                            if (decimal.TryParse(d[1].TrimEnd('%'), out decimal d_persent)) {
                                s.Persent = d_persent;
                            }
                        }
                        s.Downloaded = decimal.Parse(d[3]);
                        if (decimal.TryParse(d[5], out decimal d_speed)) s.Speed = d_speed;
                        if (decimal.TryParse(d[6], out decimal d_elapsed)) s.Elapsed = d_elapsed;

                        UpdatePersent(s.Persent);

                        if (Data.DownloadProgress.DNStatus_Infos.ContainsKey("Downloader") && Data.DownloadProgress.DNStatus_Infos["Downloader"] == App.Lang.Status.Native) {
                            Data.DownloadProgress.DNStatus_Infos["Downloaded"] = Util.GetAutoUnit((long)Data.DownloadProgress.DNStatus_Video.Downloaded + (long)Data.DownloadProgress.DNStatus_Audio.Downloaded);
                            Data.DownloadProgress.DNStatus_Infos["Total"] = Util.GetAutoUnit((long)Data.DownloadProgress.DNStatus_Video.Total + (long)Data.DownloadProgress.DNStatus_Audio.Total);
                            Data.DownloadProgress.DNStatus_Infos["Speed"] = Util.GetAutoUnit((long)Data.DownloadProgress.DNStatus_Video.Speed + (long)Data.DownloadProgress.DNStatus_Audio.Speed);
                            Data.DownloadProgress.DNStatus_Infos["Elapsed"] = Util.SecToStr(Data.DownloadProgress.DNStatus_Video.Elapsed + Data.DownloadProgress.DNStatus_Audio.Elapsed);
                            Data.DownloadProgress.DNStatus_Infos["Status"] = App.Lang.Status.Downloading;
                        }
                    } else if (regAria.IsMatch(std)) {
                        // aria2
                        Data.DownloadProgress.DNStatus_Infos["Downloader"] = "aria2c";
                        var d = Util.GetGroup(regAria, std);
                        if (decimal.TryParse(d["persent"], out decimal o_persent)) {
                            UpdatePersent(o_persent);
                        }
                        Data.DownloadProgress.DNStatus_Infos["Downloaded"] = d["downloaded"];
                        Data.DownloadProgress.DNStatus_Infos["Total"] = d["total"];
                        Data.DownloadProgress.DNStatus_Infos["Speed"] = d["speed"];
                        Data.DownloadProgress.DNStatus_Infos["Elapsed"] = d.GetValueOrDefault("eta", "0s");
                        Data.DownloadProgress.DNStatus_Infos["Connections"] = d["cn"];
                        Data.DownloadProgress.DNStatus_Infos["Status"] = App.Lang.Status.Downloading;
                    } else if (regFF.IsMatch(std)) {
                        // ffmpeg
                        Data.DownloadProgress.DNStatus_Infos["Downloader"] = "FFMPEG";
                        var d = Util.GetGroup(regFF, std);
                        Data.DownloadProgress.DNStatus_Infos["Downloaded"] = d.GetValueOrDefault("size", "");
                        Data.DownloadProgress.DNStatus_Infos["Speed"] = d.GetValueOrDefault("bitrate", "");
                        Data.DownloadProgress.DNStatus_Infos["Frame"] = d.GetValueOrDefault("frame", "");
                        Data.DownloadProgress.DNStatus_Infos["FPS"] = d.GetValueOrDefault("fps", "");
                        Data.DownloadProgress.DNStatus_Infos["Time"] = d.GetValueOrDefault("time", "");
                        Data.DownloadProgress.DNStatus_Infos["Status"] = "Downloading";
                    } else if (regYTDL.IsMatch(std)) {
                        // youtube-dl
                        if (!Data.DownloadProgress.DNStatus_Infos.ContainsKey("Downloader")) Data.DownloadProgress.DNStatus_Infos["Downloader"] = "youtube-dl";
                        var d = Util.GetGroup(regYTDL, std);
                        if (decimal.TryParse(d["persent"], out decimal o_persent)) {
                            UpdatePersent(o_persent);
                        }
                        Data.DownloadProgress.DNStatus_Infos["Total"] = d.GetValueOrDefault("total", "");
                        Data.DownloadProgress.DNStatus_Infos["Speed"] = d.GetValueOrDefault("speed", "");
                        Data.DownloadProgress.DNStatus_Infos["Elapsed"] = d.GetValueOrDefault("eta", "");
                        Data.DownloadProgress.DNStatus_Infos["Status"] = "Downloading";
                    }
                }
            }
            private void UpdatePersent(decimal persent) {
                switch (type) {
                    case 0:
                        Data.DownloadProgress.VideoPersent = Data.DownloadProgress.AudioPersent = persent;
                        break;
                    case 1:
                        Data.DownloadProgress.VideoPersent = persent;
                        break;
                    case 2:
                        Data.DownloadProgress.AudioPersent = persent;
                        break;
                }
            }
        }
    }
}
