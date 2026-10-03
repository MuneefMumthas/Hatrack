using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Hatrack;
public static class Smoke
{
    [DllImport("user32.dll")]static extern bool PostMessage(IntPtr window,uint message,IntPtr wParam,IntPtr lParam);
    public static int Run(string root)
    {
        root=Path.GetFullPath(root);
        if(!Path.GetFileName(root).StartsWith("Hatrack-smoke-",StringComparison.Ordinal)||Directory.Exists(root))throw new ArgumentException("Smoke checks require a new folder named Hatrack-smoke-<unique id>. Existing folders are never modified.");
        Environment.SetEnvironmentVariable("HATRACK_TEST_HOME",root);
        var installs=Discovery.Detect(new Catalog());var results=new List<object>();var wrappers=new List<Process>();var owners=new List<HashSet<uint>>();
        try{
            foreach(var app in Enum.GetValues<DesktopApp>()){
                var install=installs.SingleOrDefault(i=>i.App==app);if(install==null){results.Add(new{app=app.ToString(),status="not installed"});continue;}
                var profiles=new[]{ProfileService.Save(null,app,"Smoke A "+app,new("Windows","#0F766E",""),false,false),ProfileService.Save(null,app,"Smoke B "+app,new("Diamond","#7C3AED",""),false,false)};
                var ids=new List<HashSet<uint>>();
                foreach(var profile in profiles){var psi=new ProcessStartInfo(Paths.OwnExe){UseShellExecute=false};psi.ArgumentList.Add("--launch");psi.ArgumentList.Add(profile.Id.ToString("D"));var process=Process.Start(psi)!;wrappers.Add(process);var known=new HashSet<uint>{(uint)process.Id};ids.Add(known);owners.Add(known);}
                var deadline=DateTime.UtcNow.AddSeconds(35);var visible=new int[2];
                while(DateTime.UtcNow<deadline){for(var i=0;i<2;i++){var alive=Launcher.Descendants(ids[i]);visible[i]=Launcher.Windows(alive).Count;}if(visible.All(n=>n>0))break;Thread.Sleep(500);}
                Thread.Sleep(3000);
                var identity=ids.Select((known,i)=>{var windows=Launcher.Windows(Launcher.Descendants(known));return windows.Any()&&windows.All(w=>Shell.WindowAppId(w)==profiles[i].AppId);}).ToArray();
                results.Add(new{app=app.ToString(),version=install.Version,windows=visible,separateWindows=visible.All(n=>n>0)&&!ids[0].Overlaps(ids[1]),distinctWindowsIdentities=identity,dataFiles=profiles.Select(p=>Directory.EnumerateFiles(p.DataPath,"*",SearchOption.AllDirectories).Count()).ToArray(),desktopFiles=app==DesktopApp.Codex?profiles.Select(p=>Directory.Exists(Path.Combine(Paths.ProfileRoot(p.Id),"desktop"))?Directory.EnumerateFiles(Path.Combine(Paths.ProfileRoot(p.Id),"desktop"),"*",SearchOption.AllDirectories).Count():0).ToArray():Array.Empty<int>(),authentication="not tested | user-selected sign-in required"});
            }
            Directory.CreateDirectory(root);File.WriteAllText(Path.Combine(root,"smoke-results.json"),JsonSerializer.Serialize(results,CatalogStore.Json));Console.WriteLine(JsonSerializer.Serialize(results,CatalogStore.Json));return 0;
        } finally{
            foreach(var known in owners){var alive=Launcher.Descendants(known);foreach(var hwnd in Launcher.Windows(alive))PostMessage(hwnd,0x10,IntPtr.Zero,IntPtr.Zero);}
            foreach(var wrapper in wrappers){if(!wrapper.WaitForExit(8000)&&!wrapper.HasExited)wrapper.Kill();wrapper.Dispose();}
            // Test profile state is retained for evidence; only its own windows are closed.
        }
    }
}
