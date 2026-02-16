using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace YtDlpGui.WPF {
    using IoPath = System.IO.Path;
    public partial class App :Application {
        public static string AppExe;
        public static string AppPath;
        public static string AppName;
        private void LoadPath() {
            AppExe = Environment.ProcessPath;
            AppPath = IoPath.GetDirectoryName(AppExe);
            AppName = IoPath.GetFileNameWithoutExtension(AppExe);
        }
        public static string Path(Folders type, params string[] pathpart) {
            //var exe = Environment.ProcessPath;
            //var folder = IoPath.GetDirectoryName(exe);
            //var name = IoPath.GetFileNameWithoutExtension(exe);
            List<string> parmas = new() { AppPath };
            // Add default paths
            parmas.AddRange(type switch {
                Folders.root => Array.Empty<string>(),
                Folders.bin => new[] { "bin" },
                Folders.configs => new[] { "configs" },
                Folders.temp => new[] { "temp" },
                _ => throw new NotImplementedException(),
            });
            // Add extensions
            parmas.AddRange(pathpart);
            var res = "";
            try {
                res = IoPath.Combine(parmas.ToArray());
            } catch (Exception) { }
            return res;
        }
        public enum Folders {
            root, bin, configs, temp
        }
    }
}
