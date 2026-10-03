using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Hatrack;
public static class Shell
{
    [StructLayout(LayoutKind.Sequential)] public struct PropertyKey { public Guid Format; public uint Id; public PropertyKey(uint id) { Format=new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3");Id=id; } }
    [StructLayout(LayoutKind.Explicit,Size=24)] public struct PropVariant { [FieldOffset(0)]public ushort Type;[FieldOffset(8)]public IntPtr Value; public static PropVariant Text(string text)=>new(){Type=31,Value=Marshal.StringToCoTaskMemUni(text)}; }
    [ComImport,Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)] public interface IPropertyStore {
        void GetCount(out uint count);void GetAt(uint i,out PropertyKey key);void GetValue(ref PropertyKey key,out PropVariant value);void SetValue(ref PropertyKey key,ref PropVariant value);void Commit();
    }
    [ComImport,Guid("00021401-0000-0000-C000-000000000046")]class ShellLink { }
    [ComImport,Guid("000214F9-0000-0000-C000-000000000046"),InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]interface IShellLink {
        void GetPath([Out,MarshalAs(UnmanagedType.LPWStr)]StringBuilder path,int count,IntPtr data,uint flags);void GetIDList(out IntPtr id);void SetIDList(IntPtr id);void GetDescription([Out,MarshalAs(UnmanagedType.LPWStr)]StringBuilder text,int count);void SetDescription([MarshalAs(UnmanagedType.LPWStr)]string text);void GetWorkingDirectory([Out,MarshalAs(UnmanagedType.LPWStr)]StringBuilder text,int count);void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)]string text);void GetArguments([Out,MarshalAs(UnmanagedType.LPWStr)]StringBuilder text,int count);void SetArguments([MarshalAs(UnmanagedType.LPWStr)]string text);void GetHotkey(out short key);void SetHotkey(short key);void GetShowCmd(out int cmd);void SetShowCmd(int cmd);void GetIconLocation([Out,MarshalAs(UnmanagedType.LPWStr)]StringBuilder text,int count,out int index);void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)]string path,int index);void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)]string path,uint reserved);void Resolve(IntPtr hwnd,uint flags);void SetPath([MarshalAs(UnmanagedType.LPWStr)]string path);
    }
    [DllImport("shell32.dll",CharSet=CharSet.Unicode)] public static extern int SetCurrentProcessExplicitAppUserModelID(string id);
    [DllImport("shell32.dll")]static extern int SHGetPropertyStoreForWindow(IntPtr hwnd,ref Guid iid,[MarshalAs(UnmanagedType.Interface)]out IPropertyStore store);
    [DllImport("shell32.dll")]static extern void SHChangeNotify(uint eventId,uint flags,IntPtr a,IntPtr b);
    [DllImport("ole32.dll")]static extern int PropVariantClear(ref PropVariant value);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)]static extern IntPtr SendMessageTimeout(IntPtr hwnd,uint message,IntPtr wParam,IntPtr lParam,uint flags,uint timeout,out IntPtr result);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)]static extern IntPtr LoadImage(IntPtr instance,string filename,uint type,int width,int height,uint flags);
    [DllImport("user32.dll")]public static extern bool DestroyIcon(IntPtr icon);
    public static void Put(IPropertyStore store,uint id,string text) { var key=new PropertyKey(id);var value=PropVariant.Text(text);try {store.SetValue(ref key,ref value);} finally{Marshal.FreeCoTaskMem(value.Value);} }
    public static void Shortcut(string path,Profile p,string? exe=null)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var obj=new ShellLink();try {
            exe??=Paths.OwnExe;var link=(IShellLink)obj;link.SetPath(exe);link.SetArguments("--launch "+p.Id.ToString("D"));link.SetWorkingDirectory(Path.GetDirectoryName(exe)!);link.SetIconLocation(p.IconPath,0);link.SetDescription(p.Display);link.SetShowCmd(1);
            var store=(IPropertyStore)obj;Put(store,5,p.AppId);Put(store,2,$"\"{exe}\" --launch {p.Id:D}");Put(store,3,p.IconPath);Put(store,4,p.Display);store.Commit();
            ((System.Runtime.InteropServices.ComTypes.IPersistFile)obj).Save(path,true);
        }finally {Marshal.FinalReleaseComObject(obj);}
        SHChangeNotify(0x08000000,0,IntPtr.Zero,IntPtr.Zero);
    }
    public static (string Target,string Arguments) ReadShortcut(string path)
    {
        var obj=new ShellLink();try { ((System.Runtime.InteropServices.ComTypes.IPersistFile)obj).Load(path,0);var link=(IShellLink)obj;var target=new StringBuilder(32768);var args=new StringBuilder(32768);link.GetPath(target,target.Capacity,IntPtr.Zero,0);link.GetArguments(args,args.Capacity);return(target.ToString(),args.ToString()); }finally {Marshal.FinalReleaseComObject(obj);}
    }
    public static bool Owns(string path,Guid id) { if(!File.Exists(path))return false;try{var link=ReadShortcut(path);return link.Arguments=="--launch "+id.ToString("D") && (Paths.Same(link.Target,Paths.OwnExe)||IsLegacyLauncher(link.Target));}catch{return false;} }
    // Shortcuts written before the rename target Profiles.exe; RepairShortcuts moves them to this executable.
    public static bool IsLegacyLauncher(string target)=>Path.GetFileName(target).Equals("Profiles.exe",StringComparison.OrdinalIgnoreCase);
    public static IntPtr LoadIcon(string path,int size)=>LoadImage(IntPtr.Zero,path,1,size,size,0x10);
    public static string WindowAppId(IntPtr hwnd){var iid=typeof(IPropertyStore).GUID;if(SHGetPropertyStoreForWindow(hwnd,ref iid,out var store)!=0)return "";try{var key=new PropertyKey(5);store.GetValue(ref key,out var value);try{return value.Type==31?Marshal.PtrToStringUni(value.Value)??"":"";}finally{PropVariantClear(ref value);}}finally{Marshal.FinalReleaseComObject(store);}}
    public static void Stamp(IntPtr hwnd,Profile p,IntPtr small,IntPtr large)
    {
        var iid=typeof(IPropertyStore).GUID;
        if(SHGetPropertyStoreForWindow(hwnd,ref iid,out var store)==0)try{Put(store,5,p.AppId);Put(store,2,$"\"{Paths.OwnExe}\" --launch {p.Id:D}");Put(store,3,p.IconPath);Put(store,4,p.Display);store.Commit();}finally{Marshal.FinalReleaseComObject(store);}
        SendMessageTimeout(hwnd,0x80,IntPtr.Zero,small,2,200,out _);SendMessageTimeout(hwnd,0x80,new IntPtr(1),large,2,200,out _);
    }
}
