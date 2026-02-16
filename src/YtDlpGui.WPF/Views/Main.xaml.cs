using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using Swordfish.NET.Collections.Auxiliary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shell;
using WK.Libraries.SharpClipboardNS;
using YtDlpGui.WPF.Controls;
using YtDlpGui.Core.Models;
using YtDlpGui.Services.DLP;
using YtDlpGui.Services.Web;
using YtDlpGui.Services.Configuration;
using YtDlpGui.Services.Wrappers;
using YtDlpGui.WPF.Libs;


namespace YtDlpGui.WPF.Views {
    public partial class Main :Window {
        private readonly ViewData Data = new();
        private List<DLP> RunningDLP = new();
        public Main() {
            InitializeComponent();
            DataContext = Data;
            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;

            //Load Configs
            InitGUIConfig();

            Topmost = Data.WindowState.AlwaysOnTop;
            if (Data.WindowState.RememberWindowStatePosition) {
                Top = Data.WindowState.Top;
                Left = Data.WindowState.Left;
            }
            if (Data.WindowState.RememberWindowStateSize) {
                Width = Data.WindowState.Width;
                Height = Data.WindowState.Height;
            } else {
                Width = 600 * (Data.WindowState.Scale / 100d);
                Height = 380 * (Data.WindowState.Scale / 100d);
            }

            //Configuration Checking (./configs/*.*)
            InitConfiguration();

            //ScanDeps (YT-DLP, FFMPEG...)
            ScanDepends();

            //if `Target` Not exist, default app location
            if (!Directory.Exists(Data.OutputPath.TargetPath)) {
                Data.OutputPath.TargetPath = App.AppPath;
            }
            //Default Temp Dir
            if (string.IsNullOrWhiteSpace(Data.Paths.PathTEMP) || !Directory.Exists(GetTempPath)) {
                Data.Paths.PathTEMP = "%YTDLPGUI_TARGET%";
            }

            //Monitor Clipboard
            InitClipboard();

            //run update check
            Task.Run(Inits);

            //Output Lang templet
            //Yaml.Save(App.Path(App.Folders.root, "lang.yaml"), new Lang());
        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e) {
            var args = ToastArguments.Parse(e.Argument);
            if (args.Contains("action")) {
                switch (args["action"]) {
                    case "browse":
                        if (File.Exists(args["file"])) _ = Util.Explorer(args["file"]);
                        break;
                }
            }
        }

        private void ChangeScale(int present) {
            var scaleRatio = present / 100d;
            var grid = Template.FindName("MainGrid", this) as Grid; 
            if (grid != null) {
                var scaleTransform = new ScaleTransform(scaleRatio, scaleRatio);
                grid.LayoutTransform = scaleTransform;
                WindowChrome.SetWindowChrome(this, new() {
                    CaptionHeight = 22 * scaleRatio,
                    ResizeBorderThickness = new Thickness(6),
                    CornerRadius = new CornerRadius(0),
                    GlassFrameThickness = new Thickness(1),
                    NonClientFrameEdges = NonClientFrameEdges.None,
                    UseAeroCaptionButtons = false
                });
                grid.UpdateLayout();
            }
        }
        private string GetEnvPath(string path) {
            Dictionary<string, string> replacements = new() {
                {"%YTDLPGUI_TARGET%", Data.OutputPath.TargetPath},
                {"%YTDLPGUI_LOCALE%", App.AppPath}
            };
            foreach (KeyValuePair<string, string> pair in replacements) {
                string placeholder = pair.Key;
                string replacement = pair.Value;

                // Replace the placeholder with the replacement string
                path = path.Replace(placeholder, replacement);

                // Remove the part to the left of the replacement string
                int index = path.IndexOf(replacement);
                if (index >= 0) {
                    path = path.Substring(index);
                }

                // Remove duplicate directory separators
                path = path.Replace('/', '\\');
                while (path.Contains("\\\\")) {
                    path = path.Replace("\\\\", "\\");
                }
            }
            return Environment.ExpandEnvironmentVariables(path);
        }
        private string GetTempPath {
            get => GetEnvPath(Data.Paths.PathTEMP);
        }
        //Regex For Clipboard
        private Regex _frgPat = new Regex("<!--StartFragment-->(.*)<!--EndFragment-->", RegexOptions.Multiline | RegexOptions.Compiled);
        private Regex _matchUrls = new Regex(@"(https?|ftp|file)\://[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*", RegexOptions.Compiled);
        public void InitClipboard() {
            Data.PropertyChanged += (s, e) => {
                switch (e.PropertyName) {
                    case nameof(Data.ClipboardText):
                        var content = Data.ClipboardText;
                        var m = _matchUrls.Match(content);
                        if (m.Success) {
                            var capUrl = m.Value;
                            if (Util.UrlVaild(capUrl)) {
                                Data.Url = capUrl;
                                Analyze_Start();
                            }
                        }
                        break;
                    case nameof(Data.WindowState):
                        Topmost = Data.WindowState.AlwaysOnTop;
                        break;
                }
            };

            var sc = new SharpClipboard();
            sc.ClipboardChanged += (s, e) => {
                if (!Data.IsMonitor || Data.IsAnalyze || Data.DownloadProgress.IsDownload) return;
                if (e.ContentType == SharpClipboard.ContentTypes.Text) {
                    Data.ClipboardText = GetClipbaordText();
                }
            };
        }
        private string GetClipbaordText() {
            int maxTries = 10;
            int delayTime = 1000; // milliseconds
            int numTries = 0;
            while (numTries < maxTries) {
                try {
                    var content = System.Windows.Clipboard.GetText(System.Windows.TextDataFormat.Html);
                    if (!string.IsNullOrWhiteSpace(content)) {
                        content = _frgPat.Match(content).Groups?[1].Value.Trim() ?? "";
                    } else {
                        content = System.Windows.Clipboard.GetText(System.Windows.TextDataFormat.Text);
                    }
                    numTries = 0;
                    return content;
                } catch (Exception) {
                    numTries++;
                    Thread.Sleep(delayTime);
                }
            }
            return string.Empty;
        }
        public void InitGUIConfig() {
            Data.GUIConfig.Load(App.Path(App.Folders.root, App.AppName + ".yaml"));
            Util.PropertyCopy(Data.GUIConfig, Data);
            //Loaded and enabled auto save config
            Data.AutoSaveConfig = true;
        }
        public void InitConfiguration() {
            Data.Configs.Clear();
            Data.Configs.Add(new Config() { name = App.Lang.Main.ConfigurationNone });
            var cp = App.Path(App.Folders.configs);
            var fs = Directory.Exists(cp)
                ? Directory.EnumerateFiles(cp).OrderBy(x => x)
                : Enumerable.Empty<string>();
            fs.ForEach(x => {
                Data.Configs.Add(new Config() {
                    name = Path.GetFileNameWithoutExtension(x),
                    file = x
                });
            });
            Data.selectedConfig = Data.Configs.FirstOrDefault(x => x.file == Data.GUIConfig.ConfigurationFile, Data.Configs.First());
        }
        public void ScanDepends() {
            var isYoutubeDl = @"^youtube-dl\.exe";
            if (!string.IsNullOrWhiteSpace(Data.Paths.PathYTDLP) && File.Exists(Data.Paths.PathYTDLP)) {
                DLP.Path_DLP = Data.Paths.PathYTDLP;
            }
            if (!string.IsNullOrWhiteSpace(Data.Paths.PathAria2) && File.Exists(Data.Paths.PathAria2)) {
                DLP.Path_Aria2 = Data.Paths.PathAria2;
            }
            if (!string.IsNullOrWhiteSpace(Data.Paths.PathFFMPEG) && File.Exists(Data.Paths.PathFFMPEG)) {
                DLP.Path_FFMPEG = Data.Paths.PathFFMPEG;
                FFMPEG.Path_FFMPEG = Data.Paths.PathFFMPEG;
            }
            if (string.IsNullOrWhiteSpace(DLP.Path_DLP) ||
                string.IsNullOrWhiteSpace(DLP.Path_Aria2) ||
                string.IsNullOrWhiteSpace(FFMPEG.Path_FFMPEG)) {
                var deps = Directory.EnumerateFiles(App.AppPath, "*.exe", SearchOption.AllDirectories).ToList();
                deps = deps.Where(x => Path.GetFileName(App.AppExe) != Path.GetFileName(x)).ToList();
                var dep_ytdlp = deps.FirstOrDefault(x => Regex.IsMatch(Path.GetFileName(x), @"^(yt-dlp(_min|_x86|_x64)?|ytdl-patched.*?)\.exe"), "");
                var dep_ffmpeg = deps.FirstOrDefault(x => Regex.IsMatch(Path.GetFileName(x), @"^ffmpeg"), "");
                var dep_aria2 = deps.FirstOrDefault(x => Regex.IsMatch(Path.GetFileName(x), @"^aria2"), "");
                var dep_youtubedl = deps.FirstOrDefault(x => Regex.IsMatch(Path.GetFileName(x), isYoutubeDl), "");
                if (string.IsNullOrWhiteSpace(DLP.Path_DLP)) {
                    if (!string.IsNullOrWhiteSpace(dep_ytdlp)) {
                        Data.Paths.PathYTDLP = DLP.Path_DLP = dep_ytdlp;
                    } else if (!string.IsNullOrWhiteSpace(dep_youtubedl)) {
                        Data.Paths.PathYTDLP = DLP.Path_DLP = dep_youtubedl;
                    }

                }
                if (Regex.IsMatch(DLP.Path_DLP, isYoutubeDl)) DLP.Type = DLP.DLPType.youtube_dl;
                if (string.IsNullOrWhiteSpace(DLP.Path_Aria2)) {
                    Data.Paths.PathAria2 = DLP.Path_Aria2 = dep_aria2;
                }
                if (string.IsNullOrWhiteSpace(FFMPEG.Path_FFMPEG)) {
                    Data.Paths.PathFFMPEG = DLP.Path_FFMPEG = FFMPEG.Path_FFMPEG = dep_ffmpeg;
                }
            }
        }
        public async void Inits() {
            //check update
            var needcheck = false;
            var currentDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"); //"";

            if (!string.IsNullOrWhiteSpace(Data.Update.LastVersion)) needcheck = true; //not yaml
            if (currentDate != Data.Update.LastCheckUpdate) needcheck = true; //cross date

            if (needcheck) {
                var releaseData = await Web.GetLastTag();
                var last = releaseData.FirstOrDefault();
                if (last != null) {
                    Data.Update.ReleaseData = releaseData;
                    Data.Update.LastVersion = last.tag_name;
                    Data.Update.LastCheckUpdate = currentDate;
                }
            }
            if (string.Compare(App.CurrentVersion, Data.Update.LastVersion) < 0) {
                Data.Update.NewVersion = true;
            }
        }
        private void Button_Analyze(object sender, RoutedEventArgs e) {
            Analyze_Start();
        }
        private void Analyze_Start() {
            Data.IsAnalyze = true;
            cc.SelectedIndex = -1;
            cv.SelectedIndex = -1;
            ca.SelectedIndex = -1;
            cs.SelectedIndex = -1;
            Data.OutputPath.Thumbnail = null;
            Data.Video = new();
            Data.CookieSettings.NeedCookie = Data.CookieSettings.UseCookie == UseCookie.Always;

            Task.Run(() => {
                GetInfo();
                Data.IsAnalyze = false;

                if (Data.AutoDownloadAnalysed) {
                    //Download_Start();
                    if (Data.FormatSelection.selectedVideo != null && Data.FormatSelection.selectedAudio != null) {
                        Download_Start_Native();
                    }
                }
            });
        }
        private void GetInfo() {
            //Analyze
            var dlp = new DLP(Data.Url);
            if (Data.CookieSettings.NeedCookie) dlp.Cookie(Data.CookieSettings.CookieType);
            dlp.Proxy(Data.Network.ProxyUrl, Data.Network.ProxyEnabled);
            dlp.GetInfo();
            if (!string.IsNullOrWhiteSpace(Data.selectedConfig.file)) {
                dlp.LoadConfig(Data.selectedConfig.file);
            }
            if (Data.UseOutput) dlp.Output("%(title)s.%(ext)s"); //if not used config, default template
            ClearStatus();
            dlp.Exec(null, std => {
                // Get JSON
                Data.Video = JsonConvert.DeserializeObject<Video>(std, new JsonSerializerSettings() {
                    NullValueHandling = NullValueHandling.Ignore
                });

                //Reading Chapters
                {
                    Data.Chapters.Clear();
                    if (Data.Video.chapters != null && Data.Video.chapters.Any()) {
                        Data.Chapters.Add(new Chapters() { title = App.Lang.Main.ChaptersAll, type = ChaptersType.None });
                        Data.Chapters.Add(new Chapters() { title = App.Lang.Main.ChaptersSplite, type = ChaptersType.Split });
                        Data.Chapters.AddRange(Data.Video.chapters);
                        Data.hasChapter = true;
                    } else {
                        Data.Chapters.Add(new Chapters() { title = App.Lang.Main.ChaptersNone, type = ChaptersType.None });
                        Data.hasChapter = false;
                    }
                    //Data.FormatSelection.selectedChapter = Data.Chapters.First();
                }
                // Read Formats and Thumbnails
                {
                    //Debug.WriteLine(JsonConvert.SerializeObject(Data.Video.chapters, Formatting.Indented));
                    Data.Formats.LoadFromVideo(Data.Video.formats);
                    Data.Thumbnails.Reset(Data.Video.thumbnails);
                    Data.RequestedFormats.LoadFromVideo(Data.Video.requested_formats);
                }
                // Read Subtitles
                {
                    var subs = Data.Video.subtitles.Select(x => {
                        var s = x.Value.FirstOrDefault(y => y.ext == "vtt");
                        if (s == null) return null;
                        s.key = x.Key;
                        return s;
                    }).Where(x => x != null).ToList();
                    Data.Subtitles.Clear();
                    if (subs.Any()) {
                        Data.Subtitles.Add(new Subs() { name = App.Lang.Main.SubtitleIgnore });
                        Data.hasSubtitle = true;
                    } else {
                        Data.Subtitles.Add(new Subs() { name = App.Lang.Main.SubtitleNone });
                        Data.hasSubtitle = false;
                    }
                    Data.Subtitles.AddRange(subs);
                }
                var BestUrl = Data.Thumbnails.LastOrDefault()?.url;
                if (BestUrl != null && Web.Head(BestUrl)) {
                    Data.OutputPath.Thumbnail = BestUrl;
                } else {
                    Data.OutputPath.Thumbnail = Data.Video.thumbnail;
                }

                Data.SelectFormatBest(); //Make ComboBox Selected Item
                var full = string.Empty;
                if (Path.IsPathRooted(Data.Video._filename)) {
                    full = Path.GetFullPath(Data.Video._filename);
                } else {
                    full = Path.Combine(Data.OutputPath.TargetPath, Data.Video._filename);
                }
                //Data.OutputPath.TargetName = GetValidFileName(Data.Video.title) + ".tmp"; // Default filename
                Data.OutputPath.TargetName = full; // Default filename

            });
            dlp.Err(DLP.DLPError.Sign, () => {
                if (Data.CookieSettings.UseCookie == UseCookie.WhenNeeded) {
                    Data.CookieSettings.NeedCookie = true;
                    GetInfo();
                } else if (Data.CookieSettings.UseCookie == UseCookie.Ask) {
                    var mb = System.Windows.Forms.MessageBox.Show(
                        $"{App.Lang.Dialog.CookieRequired}\n",
                        $"{App.AppName}",
                        MessageBoxButtons.YesNo);

                    if (mb == System.Windows.Forms.DialogResult.Yes) {
                        Data.CookieSettings.NeedCookie = true;
                        GetInfo();
                    }
                }
            });
        }
        private void ClearStatus() {
            Data.DownloadProgress.DNStatus_Infos.Clear();
            Data.DownloadProgress.DNStatus_Video = new();
            Data.DownloadProgress.DNStatus_Audio = new();
            Data.DownloadProgress.VideoPersent = Data.DownloadProgress.AudioPersent = 0;
        }
        private void Button_SaveVideo(object sender, RoutedEventArgs e) {
            //SaveStream(0);
            var dialog = new SaveFileDialog();
            dialog.Filter =
                $"{App.Lang.Files.mkv}|*.mkv|" +
                $"{App.Lang.Files.mp4}|*.mp4|" +
                $"{App.Lang.Files.webm}|*.webm|" +
                $"{App.Lang.Files.mov}|*.mov|" +
                $"{App.Lang.Files.flv}|*.flv";
            dialog.DefaultExt = Data.FormatSelection.selectedVideo.video_ext.ToLower();
            dialog.FileName = Path.ChangeExtension(Path.GetFileName(Data.OutputPath.TargetFile), dialog.DefaultExt);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                var target = dialog.FileName;
                Download_Start_Native(DownloadType.Video, target);
            }
        }
        private void Button_SaveAudio(object sender, RoutedEventArgs e) {
            //SaveStream(1);
            var dialog = new SaveFileDialog();
            dialog.Filter =
                $"{App.Lang.Files.opus}|*.opus|" +
                $"{App.Lang.Files.aac}|*.aac|" +
                $"{App.Lang.Files.m4a}|*.m4a|" +
                $"{App.Lang.Files.mp3}|*.mp3|" +
                $"{App.Lang.Files.vorbis}|*.vorbis|" +
                $"{App.Lang.Files.alac}|*.alac|" +
                $"{App.Lang.Files.flac}|*.flac|" +
                $"{App.Lang.Files.wav}|*.wav";
            dialog.DefaultExt = Data.FormatSelection.selectedAudio.acodec.ToLower();
            dialog.FileName = Path.ChangeExtension(Path.GetFileName(Data.OutputPath.TargetFile), dialog.DefaultExt);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                var target = dialog.FileName;
                Download_Start_Native(DownloadType.Audio, target);
            }
        }
        private void Button_ExplorerTarget(object sender, RoutedEventArgs e) {
            Util.Explorer(Data.OutputPath.TargetFile);
        }
        private void Button_Cancel(object sender, RoutedEventArgs e) {
            if (Data.DownloadProgress.IsDownload) {
                Data.IsAbouted = true;
                foreach (var dlp in RunningDLP) {
                    dlp.Close();
                }
            }
        }
        private void Button_Download(object sender, RoutedEventArgs e) {
            //Download_Start();
            Download_Start_Native();
        }
        public enum DownloadType { Normal, Video, Audio, Thumbnail, Subtitle }
        private async void Download_Start_Native(DownloadType type = DownloadType.Normal, string target = "") {
            Data.DownloadProgress.CanCancel = false;
            Data.IsAbouted = false;
            if (Data.DownloadProgress.IsDownload) {
                Data.IsAbouted = true;
                foreach (var dlp in RunningDLP) {
                    dlp.Close();
                }
            } else {
                var overwrite = true;
                RunningDLP.Clear();
                // Check if file already exists
                if (File.Exists(Data.OutputPath.TargetFile) && type == DownloadType.Normal) {
                    var mb = System.Windows.Forms.MessageBox.Show(
                        $"{App.Lang.Dialog.FileExist}\n",
                        $"{App.AppName}",
                        MessageBoxButtons.YesNo);
                    overwrite = mb == System.Windows.Forms.DialogResult.Yes;
                    if (!overwrite) return; // Do not overwrite
                }
                Data.DownloadProgress.IsDownload = true;

                // Reset progress to 0
                ClearStatus();
                _ = Task.Run(() => {
                    var dlp = new DLP(Data.Url);
                    List<Task> tasks = new();
                    tasks.Add(Task.Run(() => {
                        //var dlp = new DLP(Data.Url);
                        RunningDLP.Add(dlp);
                        dlp.IsLive = Data.Video.is_live;
                        var vid = type switch {
                            DownloadType.Video => Data.FormatSelection.selectedVideo.format_id,
                            DownloadType.Audio => Data.FormatSelection.selectedAudio.format_id,
                            _ => $"{Data.FormatSelection.selectedVideo.format_id}+{Data.FormatSelection.selectedAudio.format_id}"
                        };
                        dlp
                        .Temp(GetTempPath)
                        .LoadConfig(Data.selectedConfig.file)
                        .MTime(Data.DownloadOptions.ModifiedType)
                        .Cookie(Data.CookieSettings.CookieType, Data.CookieSettings.NeedCookie)
                        .Proxy(Data.Network.ProxyUrl, Data.Network.ProxyEnabled)
                        .UseAria2(Data.DownloadOptions.UseAria2)
                        .LimitRate(Data.DownloadOptions.LimitRate)
                        .DownloadSections(Data.DownloadOptions.TimeRange)
                        .SplitChapters(Data.FormatSelection.selectedChapter, Data.OutputPath.TargetFile);

                        switch (type) {
                            case DownloadType.Video:
                                dlp
                                .EmbedChapters(Data.DownloadOptions.EmbedChapters)
                                .Thumbnail(Data.DownloadOptions.SaveThumbnail, Data.OutputPath.TargetFile, Data.DownloadOptions.EmbedThumbnail)
                                .Subtitle(Data.FormatSelection.selectedSub.key, Data.OutputPath.TargetFile, Data.DownloadOptions.EmbedSubtitles)
                                .DownloadVideo(vid, Data.FormatSelection.selectedVideo.video_ext, target);
                                break;
                            case DownloadType.Audio:
                                dlp
                                .EmbedChapters(Data.DownloadOptions.EmbedChapters)
                                .Thumbnail(Data.DownloadOptions.SaveThumbnail, Data.OutputPath.TargetFile, Data.DownloadOptions.EmbedThumbnail)
                                .DownloadAudio(vid, target);
                                break;
                            case DownloadType.Subtitle:
                                dlp.DownloadSubtitle(Data.FormatSelection.selectedSub.key, target);
                                break;
                            default:
                                dlp
                                .EmbedChapters(Data.DownloadOptions.EmbedChapters)
                                .Thumbnail(Data.DownloadOptions.SaveThumbnail, Data.OutputPath.TargetFile, Data.DownloadOptions.EmbedThumbnail)
                                .Subtitle(Data.FormatSelection.selectedSub.key, Data.OutputPath.TargetFile, Data.DownloadOptions.EmbedSubtitles)
                                .DownloadFormat(vid, Data.OutputPath.TargetFile, Data.OriginExt);
                                break;
                        }
                        var repoter = new StatusRepoter(Data);
                        repoter.type = type switch {
                            DownloadType.Video => 1,
                            DownloadType.Audio => 2,
                            _ => 0
                        };
                        dlp.Exec(std => {
                            repoter.GetStatus(std);
                        });
                    }));
                    //WaitAll Downloads, Merger Video and Audio
                    Data.DownloadProgress.CanCancel = true;
                    Task.WaitAll(tasks.ToArray());
                    if (!Data.IsAbouted) {
                        Data.DownloadProgress.DNStatus_Infos["Status"] = App.Lang.Status.Done;

                        //post-process
                        Dictionary<string, string> files = new Dictionary<string, string>();
                        foreach (string donepath in dlp.Files) {
                            if (File.Exists(donepath)) {
                                if (donepath.isVideo()) files["video"] = donepath;
                                if (donepath.isImage()) files["thumb"] = donepath;
                                if (Data.DownloadOptions.ModifiedType == ModifiedType.Upload) {
                                    if (DateTimeOffset.TryParseExact(Data.Video.upload_date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset tryDate)) {
                                        File.SetLastWriteTimeUtc(donepath, tryDate.DateTime);
                                    }
                                }
                            }
                        }

                        //Send notification when download completed
                        try {
                            if (Data.DownloadOptions.UseNotifications) {
                                Util.NotifySound(Data.Paths.PathNotify);
                                var toast = new ToastContentBuilder()
                                    .AddText(Data.Video.title)
                                    .AddText(App.Lang.Dialog.DownloadCompleted)
                                    .AddAudio(new ToastAudio() {
                                        Silent = true,
                                        Loop = false,
                                        Src = new Uri("ms-winsoundevent:Notification.Default")
                                    });
                                if (files.ContainsKey("video")) {
                                    toast.AddButton(
                                        new ToastButton()
                                        .SetContent(App.Lang.Dialog.OpenFolder)
                                        .AddArgument("action", "browse")
                                        .AddArgument("file", files["video"])
                                        .SetBackgroundActivation()
                                    );
                                }
                                if (files.ContainsKey("thumb")) {
                                    toast.AddAppLogoOverride(new Uri(files["thumb"]));
                                }
                                toast.AddButton(
                                    new ToastButton()
                                    .SetContent(App.Lang.Dialog.Close)
                                    .AddArgument("action", "none")
                                    .SetBackgroundActivation()
                                );
                                toast.Show();
                            }
                        } catch (Exception ex) { }
                    }
                    //Clear downloading status
                    Data.DownloadProgress.IsDownload = false;
                });
            }
        }

        private void Button_Browser(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(Data.OutputPath.TargetName)) {
                var dialog = new FolderBrowserDialog();
                dialog.SelectedPath = Data.OutputPath.TargetPath;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    Data.OutputPath.TargetPath = dialog.SelectedPath;
                }
            } else {
                var dialog = new SaveFileDialog();
                dialog.InitialDirectory = Path.GetDirectoryName(Data.OutputPath.TargetFile);
                dialog.FileName = Path.GetFileName(Data.OutputPath.TargetFile);
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    Data.FormatSelection.RemuxVideo = true;
                    Data.OutputPath.TargetPath = Path.GetDirectoryName(dialog.FileName);
                    Data.OutputPath.TargetName = Path.GetFileName(dialog.FileName);
                    Data.FormatSelection.RemuxVideo = false;
                    /*
                    if ((new string[] { ".mp4", ".webm", ".3gp", ".mkv" }).Any(x => Path.GetExtension(dialog.FileName).ToLower() == x)) {
                        Data.OutputPath.TargetName = Path.GetFileName(dialog.FileName);
                    } else {
                        Data.OutputPath.TargetName = Path.GetFileName(dialog.FileName) + ".tmp";
                    }
                    */
                }
            }
        }

        private static Regex RegexValues = new Regex(@"\${(.+?)}", RegexOptions.Compiled);
        private string GetValidFileName(string filename) {
            var regexSearch = new string(Path.GetInvalidFileNameChars());
            return Regex.Replace(filename, string.Format("[{0}]", Regex.Escape(regexSearch)), "_");
        }
        private async void CommandBinding_SaveAs_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e) {
            var dialog = new SaveFileDialog();
            dialog.InitialDirectory = Path.GetDirectoryName(Data.OutputPath.TargetFile);
            var OrigExt = Path.GetExtension(Data.OutputPath.Thumbnail);
            var OrigFileName = Path.ChangeExtension(Path.GetFileName(Data.OutputPath.TargetFile), OrigExt);
            dialog.DefaultExt = ".jpg";
            dialog.Filter = $"{App.Lang.Files.image}|*.jpg;*.webp";
            dialog.FileName = Path.ChangeExtension(OrigFileName, ".jpg");
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                DownloadThumbnail(dialog.FileName);
            }
        }
        private void DownloadThumbnail(string toFile) {
            var origExt = Path.GetExtension(Data.OutputPath.Thumbnail);
            var origin = Path.ChangeExtension(Data.OutputPath.TargetFile, origExt);
            //var target = Path.ChangeExtension(Data.OutputPath.TargetFile, ".jpg");
            var target = toFile;
            var progress = new Progress<double>(percentage => {
                Debug.Write($"\rDownloading... {percentage:0.00}%");
            });
            Web.Download(Data.OutputPath.Thumbnail, origin, progress, Data.Network.ProxyEnabled ? Data.Network.ProxyUrl : null).Wait();
            //convert to target ext
            if (Path.GetExtension(origin).ToLower() != Path.GetExtension(target)) {
                FFMPEG.DownloadUrl(origin, target);
                File.Delete(origin);
            }
        }

        private void CommandBinding_SaveAs_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e) {
            e.CanExecute = !string.IsNullOrWhiteSpace(Data.OutputPath.Thumbnail);
        }

        private void Button_Subtitle(object sender, RoutedEventArgs e) {
            var dialog = new SaveFileDialog();
            dialog.InitialDirectory = Path.GetDirectoryName(Data.OutputPath.TargetFile);
            //dialog.Filter = "SubRip | *.srt";
            //dialog.DefaultExt = Data.FormatSelection.selectedSub.key + ".srt";
            dialog.DefaultExt = ".srt";
            dialog.Filter =
                $"{App.Lang.Files.srt}|*.srt|" +
                $"{App.Lang.Files.ass}|*.ass|" +
                $"{App.Lang.Files.vtt}|*.vtt|" +
                $"{App.Lang.Files.lrc}|*.lrc|" +
                $"{App.Lang.Files.ttml}|*.ttml|" +
                $"{App.Lang.Files.srv3}|*.srv3|" +
                $"{App.Lang.Files.srv2}|*.srv2|" +
                $"{App.Lang.Files.srv1}|*.srv1|" +
                $"{App.Lang.Files.json3}|*.json3";
            //dialog.FileName = Path.ChangeExtension(Path.GetFileName(Data.OutputPath.TargetFile), Data.FormatSelection.selectedSub.key + ".srt");
            dialog.FileName = Path.ChangeExtension(Data.OutputPath.TargetFile, null);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                var target = dialog.FileName;
                Debug.WriteLine(dialog.FileName, "DIALOG");
                //var target = Path.ChangeExtension(dialog.FileName, ".srt");
                //FFMPEG.DownloadUrl(Data.FormatSelection.selectedSub.url, target);
                Download_Start_Native(DownloadType.Subtitle, target);
            }
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e) {
            var win = new About();
            win.Owner = GetWindow(this);
            win.ShowDialog();
        }

        private void Button_Release(object sender, RoutedEventArgs e) {
            var win = new Release();
            win.Owner = GetWindow(this);
            win.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e) {
            Data.WindowState.Left = Left;
            Data.WindowState.Top = Top;
            Data.WindowState.Width = Width;
            Data.WindowState.Height = Height;
        }
        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e) {
            var combo = sender as System.Windows.Controls.ComboBox;
            if (combo.SelectedIndex == -1) {
                Data.Paths.PathTEMP = combo.Text;
            } else {
                Data.Paths.PathTEMP = combo.SelectedValue.ToString();
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) {
            var b = sender as ToggleButton;
            if (b.IsChecked == true) {
                var menu = new List<MenuDataItem>() {
                    (App.Lang.Main.TemporaryTarget, () => { Data.Paths.PathTEMP = "%YTDLPGUI_TARGET%"; }),
                    (App.Lang.Main.TemporaryLocale, () => { Data.Paths.PathTEMP = "%YTDLPGUI_LOCALE%"; }),
                    (App.Lang.Main.TemporarySystem, () => { Data.Paths.PathTEMP = "%TEMP%"; }),
                    ("-"),
                    (App.Lang.Main.TemporaryBrowse, () => {
                        var dialog = new FolderBrowserDialog();
                        dialog.SelectedPath = GetEnvPath(Data.Paths.PathTEMP);
                        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                            Data.Paths.PathTEMP = dialog.SelectedPath;
                        }
                    })
                };
                Controls.Menu.Open(menu, b, MenuPlacement.BottomLeft);
            }
        }

        private void ToggleButton_Checked_Sound(object sender, RoutedEventArgs e) {
            var b = sender as ToggleButton;
            if (b.IsChecked == true) {
                var menu = new List<MenuDataItem>() {
                    (App.Lang.Main.SoundSystem, () => { Data.Paths.PathNotify = ""; }),
                    ("-"),
                    (App.Lang.Main.SoundBrowse, () => {
                        var dialog = new OpenFileDialog();
                        var dirname = Path.GetDirectoryName(Data.Paths.PathNotify);
                        Debug.WriteLine(dirname);
                        if (Directory.Exists(dirname)) {
                            dialog.InitialDirectory = dirname;
                            if (File.Exists(Data.Paths.PathNotify)) {
                                dialog.FileName = Path.GetFileName(Data.Paths.PathNotify);
                            }
                        } else {
                            dialog.InitialDirectory = App.AppPath;
                        }
                        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                            Data.Paths.PathNotify = dialog.FileName;
                        }
                    })
                };
                Controls.Menu.Open(menu, b, MenuPlacement.BottomLeft);
            }
        }

        private void Button_PlayNotify(object sender, RoutedEventArgs e) {
            Util.NotifySound(Data.Paths.PathNotify);
        }

        private void TextBoxNumber_Changed(object sender, EventArgs e) {
            if (Data.WindowState.Scale == 0) {
                Data.WindowState.Scale = 100;
            } else if (Data.WindowState.Scale < 80) {
                Data.WindowState.Scale = 80;
            } else if (Data.WindowState.Scale > 200) {
                Data.WindowState.Scale = 200;
            }
            ChangeScale(Data.WindowState.Scale);
        }
    }
    public class LanguageConverter :IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            // Check if input value is a string
            if (!(value is string key))
                return value;

            // Use reflection to check if Lang object contains the specified key property
            var Lang = App.Lang.Status;
            var propertyInfo = Lang.GetType().GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            // If Lang object does not contain the specified key property, return empty string
            if (propertyInfo == null)
                return key;

            // If Lang object contains the specified key property, return the corresponding value
            return propertyInfo.GetValue(Lang)?.ToString() ?? key;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            // ConvertBack not implemented as this converter is only for one-way binding
            throw new NotImplementedException();
        }
    }
}


