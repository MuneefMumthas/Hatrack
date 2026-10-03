using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Hatrack;
public static class Launcher
{
    public static ProcessStartInfo LaunchSpec(Profile p,Installation install)
    {
        if(p.App==DesktopApp.Codex) {
            var config=Path.Combine(p.DataPath,"config.toml");
            var topLevel=File.Exists(config)?string.Join("\n",File.ReadAllLines(config).TakeWhile(line=>!line.TrimStart().StartsWith('['))):"";
            if(!System.Text.RegularExpressions.Regex.IsMatch(topLevel,"(?m)^\\s*cli_auth_credentials_store\\s*=\\s*[\"']file[\"']\\s*(?:#.*)?$"))throw new InvalidOperationException("This Codex home must use profile-local credential storage. Set cli_auth_credentials_store = \"file\" at the top level of this profile's config.toml before launching. Hatrack will not overwrite an imported configuration.");
        }
        var psi=new ProcessStartInfo(install.Executable){UseShellExecute=false,WorkingDirectory=Path.GetDirectoryName(install.Executable)!,RedirectStandardOutput=true,RedirectStandardError=true};
        psi.Environment.Remove("HATRACK_TEST_HOME");
        if(p.App==DesktopApp.Claude){psi.ArgumentList.Add("--user-data-dir="+p.DataPath);psi.ArgumentList.Add("--disk-cache-dir="+p.CachePath);}
        else{
            psi.Environment["CODEX_HOME"]=p.DataPath;
            psi.Environment["CODEX_ELECTRON_USER_DATA_PATH"]=Path.Combine(Paths.ProfileRoot(p.Id),"desktop");
            // Owl/Chromium and Electron must select the same profile-local browser state.
            psi.ArgumentList.Add("--user-data-dir="+psi.Environment["CODEX_ELECTRON_USER_DATA_PATH"]);
            psi.Environment.Remove("CODEX_SQLITE_HOME");
            foreach(var key in new[]{"CODEX_WINDOWS_REGISTERED_CORE","CODEX_WINDOWS_SANDBOX_PACKAGE_FAMILY","OPENAI_API_KEY","CODEX_API_KEY","CODEX_ACCESS_TOKEN"})psi.Environment.Remove(key);
        }
        return psi;
    }
    public static void Start(Guid id)
    {
        var psi=new ProcessStartInfo(Paths.OwnExe){UseShellExecute=false};psi.ArgumentList.Add("--launch");psi.ArgumentList.Add(id.ToString("D"));Process.Start(psi);
    }
    public static int Run(Guid id)
    {
        // Pre-rename names: a profile opened by an older build is still detected as running.
        using var mutex=new Mutex(true,"Local\\Profiles.Launch."+id.ToString("N"),out var first);
        using var activate=new EventWaitHandle(false,EventResetMode.AutoReset,"Local\\Profiles.Activate."+id.ToString("N"));
        if(!first){activate.Set();return 0;}
        var p=CatalogStore.Read().Profiles.SingleOrDefault(p=>p.Id==id)??throw new InvalidOperationException("This profile is no longer in the catalogue. Open Hatrack to create a new shortcut.");
        var install=p.ExecutableOverride!=""&&!p.ExecutableOverride.Contains("\\WindowsApps\\",StringComparison.OrdinalIgnoreCase)&&File.Exists(p.ExecutableOverride)?Discovery.Validate(p.App,p.ExecutableOverride):Discovery.Detect(CatalogStore.Read()).SingleOrDefault(i=>i.App==p.App);
        if(install==null)throw new InvalidOperationException($"{p.App} Desktop is not installed or has moved. Open Hatrack and choose Locate app.");
        if(Compatibility.NeedsConfirmation(install,CatalogStore.Read())) {
            if(System.Windows.MessageBox.Show(Compatibility.Warning(install),"Hatrack",System.Windows.MessageBoxButton.YesNo,System.Windows.MessageBoxImage.Warning)!=System.Windows.MessageBoxResult.Yes)return 0;
            CatalogStore.Update(c=>{c.AcceptedVersions[install.App]=install.Version;return true;});
        }
        Directory.CreateDirectory(p.DataPath);Directory.CreateDirectory(p.CachePath);
        Shell.SetCurrentProcessExplicitAppUserModelID(p.AppId);
        var small=Shell.LoadIcon(p.IconPath,16);var large=Shell.LoadIcon(p.IconPath,32);
        var reopenedProcesses=new List<Process>();
        try {
            using var process=Process.Start(LaunchSpec(p,install))??throw new IOException("Windows could not start the desktop app.");
            process.OutputDataReceived+=(_,_)=>{};process.ErrorDataReceived+=(_,_)=>{};process.BeginOutputReadLine();process.BeginErrorReadLine();
            var known=new HashSet<uint>{(uint)process.Id};var stamped=new HashSet<IntPtr>();var emptySince=DateTime.UtcNow;var everVisible=false;
            while(true){
                var alive=Descendants(known);var vendorIds=alive.Where(pid=>IsVendorProcess(pid,install.Executable)).ToHashSet();var windows=Windows(vendorIds);
                foreach(var hwnd in windows){try{if(stamped.Add(hwnd)||Shell.WindowAppId(hwnd)!=p.AppId)Shell.Stamp(hwnd,p,small,large);}catch(COMException){stamped.Remove(hwnd);}}
                stamped.IntersectWith(windows);
                if(windows.Count>0){everVisible=true;emptySince=DateTime.UtcNow;}
                if(activate.WaitOne(0)){if(windows.Count==0){var reopened=Process.Start(LaunchSpec(p,install));if(reopened!=null){reopenedProcesses.Add(reopened);known.Add((uint)reopened.Id);reopened.OutputDataReceived+=(_,_)=>{};reopened.ErrorDataReceived+=(_,_)=>{};reopened.BeginOutputReadLine();reopened.BeginErrorReadLine();}}else foreach(var hwnd in windows){ShowWindow(hwnd,9);SetForegroundWindow(hwnd);break;}}
                if(alive.Count==0&&DateTime.UtcNow-emptySince>TimeSpan.FromSeconds(everVisible?3:15))break;
                if(!everVisible&&DateTime.UtcNow-emptySince>TimeSpan.FromSeconds(60))throw new IOException("The desktop app did not open a profile window. Check compatibility and Windows package updates in Hatrack.");
                Thread.Sleep(500);
            }
            if(!everVisible)throw new IOException("The desktop app exited before opening a window. Windows may be updating or holding its app package. Save your work, finish updates, and restart Windows if the file-in-use error persists. Your profile data has not been reset.");
            return 0;
        } finally{foreach(var reopened in reopenedProcesses)reopened.Dispose();if(small!=IntPtr.Zero)Shell.DestroyIcon(small);if(large!=IntPtr.Zero)Shell.DestroyIcon(large);}
    }
    [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]struct ProcessEntry {public uint Size,Usage,Id;public UIntPtr Heap;public uint Module,Threads,Parent;public int Priority;public uint Flags;[MarshalAs(UnmanagedType.ByValTStr,SizeConst=260)]public string Name;}
    [DllImport("kernel32.dll")]static extern IntPtr CreateToolhelp32Snapshot(uint flags,uint id);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)]static extern bool Process32First(IntPtr snapshot,ref ProcessEntry entry);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)]static extern bool Process32Next(IntPtr snapshot,ref ProcessEntry entry);
    [DllImport("kernel32.dll")]static extern bool CloseHandle(IntPtr h);
    public static HashSet<uint> Descendants(HashSet<uint> known)
    {
        var snapshot=CreateToolhelp32Snapshot(2,0);if(snapshot==new IntPtr(-1))return new();
        var all=new List<ProcessEntry>();try {var entry=new ProcessEntry{Size=(uint)Marshal.SizeOf<ProcessEntry>()};if(Process32First(snapshot,ref entry))do{all.Add(entry);}while(Process32Next(snapshot,ref entry));}finally{CloseHandle(snapshot);}
        bool changed;do{changed=false;foreach(var e in all)if(known.Contains(e.Parent)&&known.Add(e.Id))changed=true;}while(changed);
        var alive=all.Where(e=>known.Contains(e.Id)).Select(e=>e.Id).ToHashSet();known.IntersectWith(alive);return alive;
    }
    private static bool IsVendorProcess(uint pid,string executable){try{using var p=Process.GetProcessById((int)pid);return p.ProcessName.Equals(Path.GetFileNameWithoutExtension(executable),StringComparison.OrdinalIgnoreCase);}catch{return false;}}
    delegate bool EnumWindowsCallback(IntPtr window,IntPtr data);
    [DllImport("user32.dll")]static extern bool EnumWindows(EnumWindowsCallback callback,IntPtr data);
    [DllImport("user32.dll")]static extern uint GetWindowThreadProcessId(IntPtr window,out uint id);
    [DllImport("user32.dll")]static extern bool IsWindowVisible(IntPtr window);
    [DllImport("user32.dll")]static extern bool ShowWindow(IntPtr window,int command);
    [DllImport("user32.dll")]static extern bool SetForegroundWindow(IntPtr window);
    public static List<IntPtr> Windows(HashSet<uint> ids){var windows=new List<IntPtr>();EnumWindows((hwnd,_)=>{GetWindowThreadProcessId(hwnd,out var pid);if(ids.Contains(pid)&&IsWindowVisible(hwnd))windows.Add(hwnd);return true;},IntPtr.Zero);return windows;}
}
