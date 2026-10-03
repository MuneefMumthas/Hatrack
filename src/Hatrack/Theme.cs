using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using Microsoft.Win32;

namespace Hatrack;
public static class AppTheme
{
    public static void Attach(Window window,Window? owner=null)
    {
        window.Resources.MergedDictionaries.Add(new ResourceDictionary { Source=new Uri("/Hatrack;component/Theme.xaml",UriKind.Relative) });
        if(owner!=null)foreach(var key in Keys)if(owner.TryFindResource(key) is Brush brush)window.Resources[key]=brush;
        window.SetResourceReference(Window.BackgroundProperty,"Canvas");window.SetResourceReference(Window.ForegroundProperty,"Ink");
        window.SourceInitialized+=(_,_)=>Chrome(window);
        Chrome(window);
    }
    private static readonly string[] Keys={"Canvas","Surface","Inset","Ink","Muted","Line","Hover","Accent","OnAccent","Selection","Error"};
    public static void Apply(Window window,string theme)
    {
        var dark=theme=="Dark"||theme=="System"&&(int?)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize","AppsUseLightTheme",1)==0;
        var values=dark?new[]{"#111113","#1E1E21","#27272B","#F5F5F7","#B5B5BD","#65656E","#303035","#F5F5F7","#161617","#3D3D44","#FFB4AB"}:new[]{"#F5F5F7","#FFFFFF","#EEEEF0","#161617","#626267","#C7C7CD","#E7E7EB","#161617","#FFFFFF","#DCDCE1","#B42318"};
        for(var i=0;i<Keys.Length;i++)window.Resources[Keys[i]]=new SolidColorBrush((Color)ColorConverter.ConvertFromString(values[i]));
        Chrome(window);
    }
    private static void Chrome(Window window)
    {
        var handle=new WindowInteropHelper(window).Handle;if(handle==IntPtr.Zero)return;
        var dark=(window.TryFindResource("Canvas") as SolidColorBrush)?.Color.R<80?1:0;var round=2;
        DwmSetWindowAttribute(handle,20,ref dark,4);DwmSetWindowAttribute(handle,33,ref round,4);
    }
    [DllImport("dwmapi.dll")]private static extern int DwmSetWindowAttribute(IntPtr window,int attribute,ref int value,int size);
}
