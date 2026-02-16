using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YtDlpGui.Core.Models {
    public class Config :INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string name { get; set; } = string.Empty;
        public string file { get; set; } = string.Empty;
    }
}
