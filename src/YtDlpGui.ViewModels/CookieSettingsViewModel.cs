using PropertyChanged;
using YtDlpGui.Core.Models;

namespace YtDlpGui.ViewModels;

[AddINotifyPropertyChangedInterface]
public class CookieSettingsViewModel
{
    public UseCookie UseCookie { get; set; } = UseCookie.WhenNeeded;
    public CookieType CookieType { get; set; } = CookieType.Chrome;
    public bool NeedCookie { get; set; } = false;
}
