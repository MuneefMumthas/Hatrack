using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Hatrack;
public static class Discovery
{
    public static readonly Dictionary<DesktopApp,string> DownloadUrls = new() { [DesktopApp.Claude]="https://claude.ai/download",[DesktopApp.Codex]="https://developers.openai.com/codex/app/" };
    public static List<Installation> Detect(Catalog catalog)
    {
        var result=new List<Installation>();
        foreach(var pair in catalog.Locations.Where(p=>!p.Value.Contains("\\WindowsApps\\",StringComparison.OrdinalIgnoreCase))) TryAdd(result,pair.Key,pair.Value,"Selected location");
        var psi=new ProcessStartInfo("powershell.exe") { UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true };
        psi.ArgumentList.Add("-NoProfile"); psi.ArgumentList.Add("-NonInteractive"); psi.ArgumentList.Add("-Command");
        psi.ArgumentList.Add("Get-AppxPackage | Where-Object { $_.Name -eq 'Claude' -or $_.Name -eq 'OpenAI.Codex' } | Select-Object Name,Version,InstallLocation | ConvertTo-Json -Compress");
        try {
            using var p=Process.Start(psi)!; var output=p.StandardOutput.ReadToEndAsync(); var errors=p.StandardError.ReadToEndAsync();
            if(!p.WaitForExit(15000)) { p.Kill(); throw new IOException("Windows package detection timed out. Use Locate app to continue."); }
            var raw=output.GetAwaiter().GetResult(); errors.GetAwaiter().GetResult();
            if(!string.IsNullOrWhiteSpace(raw)) {
                using var json=JsonDocument.Parse(raw);
                var entries=json.RootElement.ValueKind==JsonValueKind.Array?json.RootElement.EnumerateArray().ToArray():new[]{json.RootElement};
                foreach(var e in entries.OrderByDescending(e=>Version.TryParse(e.GetProperty("Version").GetString(),out var v)?v:new Version())) {
                    var app=e.GetProperty("Name").GetString()=="Claude"?DesktopApp.Claude:DesktopApp.Codex;
                    var dir=e.GetProperty("InstallLocation").GetString(); if(string.IsNullOrEmpty(dir))continue;
                    var exe=Path.Combine(dir,"app",app==DesktopApp.Claude?"Claude.exe":"ChatGPT.exe");
                    TryAdd(result,app,exe,"Windows package",e.GetProperty("Version").GetString());
                }
            }
        } catch(Exception e) when(e is not InvalidDataException) { /* Manual locations remain usable when Windows package enumeration is unavailable. */ }
        foreach(var pair in catalog.Locations)TryAdd(result,pair.Key,pair.Value,"Selected location");
        var local=Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        TryAdd(result,DesktopApp.Claude,Path.Combine(local,"AnthropicClaude","claude.exe"),"Standard location");
        return result.GroupBy(i=>i.App).Select(g=>g.First()).ToList();
    }
    private static void TryAdd(List<Installation> entries,DesktopApp app,string exe,string source,string? version=null)
    {
        if(entries.Any(i=>i.App==app)||!File.Exists(exe))return;
        try { var i=Validate(app,exe); entries.Add(i with {Source=source,Version=version??i.Version}); } catch(ArgumentException) { }
    }
    public static Installation Validate(DesktopApp app,string exe)
    {
        exe=Path.GetFullPath(exe);
        if(!File.Exists(exe))throw new ArgumentException("The selected desktop executable does not exist.");
        var info=FileVersionInfo.GetVersionInfo(exe);
        var company=info.CompanyName??""; var product=(info.ProductName??"")+" "+(info.FileDescription??"");
        var name=Path.GetFileName(exe);
        var valid=app==DesktopApp.Claude
            ? name.Equals("Claude.exe",StringComparison.OrdinalIgnoreCase)&&company.Contains("Anthropic",StringComparison.OrdinalIgnoreCase)&&product.Contains("Claude",StringComparison.OrdinalIgnoreCase)
            : (name.Equals("ChatGPT.exe",StringComparison.OrdinalIgnoreCase)||name.Equals("Codex.exe",StringComparison.OrdinalIgnoreCase))&&company.Contains("OpenAI",StringComparison.OrdinalIgnoreCase)&&File.Exists(Path.Combine(Path.GetDirectoryName(exe)!,"resources","app.asar"))&&File.Exists(Path.Combine(Path.GetDirectoryName(exe)!,"resources","owl-app.ini"))&&File.ReadAllText(Path.Combine(Path.GetDirectoryName(exe)!,"resources","owl-app.ini")).Contains("UserDataDirectoryName=Codex",StringComparison.OrdinalIgnoreCase);
        if(!valid)throw new ArgumentException($"Select the official {app} desktop application. CLI tools, profile launchers, and ordinary ChatGPT installations are not supported.");
        var package=Regex.Match(exe,@"\\WindowsApps\\[^\\]+_(\d+\.\d+\.\d+\.\d+)_",RegexOptions.IgnoreCase);
        return new Installation(app,exe,package.Success?package.Groups[1].Value:info.ProductVersion??info.FileVersion??"Unknown","Selected location");
    }
    public static List<DiscoveredProfile> Existing(Catalog catalog)
    {
        var found=new List<DiscoveredProfile>();
        var local=Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var home=Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        foreach(var dir in Directory.EnumerateDirectories(local,"Claude-*")) {
            var name=Path.GetFileName(dir); if(name.EndsWith("-Cache",StringComparison.OrdinalIgnoreCase))continue;
            found.Add(new(DesktopApp.Claude,name,dir,"","Known Claude profile folder"));
        }
        if(Directory.Exists(Path.Combine(home,".codex")))found.Add(new(DesktopApp.Codex,"Original Codex",Path.Combine(home,".codex"),"","Default Codex home · launch with original shortcut"));
        foreach(var dir in Directory.EnumerateDirectories(home,".codex-*"))found.Add(new(DesktopApp.Codex,Path.GetFileName(dir),dir,"","Candidate Codex home · inspect before importing"));
        foreach(var directory in new[]{Paths.Desktop,Environment.GetFolderPath(Environment.SpecialFolder.Programs)}) {
            try {
                foreach(var path in Directory.EnumerateFiles(directory,"*.lnk",SearchOption.TopDirectoryOnly)) {
                    var link=Shell.ReadShortcut(path);
                    var app=link.Target.Contains("claude",StringComparison.OrdinalIgnoreCase)?DesktopApp.Claude:link.Target.Contains("codex",StringComparison.OrdinalIgnoreCase)||link.Target.EndsWith("ChatGPT.exe",StringComparison.OrdinalIgnoreCase)?DesktopApp.Codex:(DesktopApp?)null;
                    if(app==null)continue;
                    var match=Regex.Match(link.Arguments,"--user-data-dir(?:=|\\s+)(?:\"([^\"]+)\"|([^\\s]+))");
                    if(match.Success) { var folder=match.Groups[1].Success?match.Groups[1].Value:match.Groups[2].Value;
                        if(Directory.Exists(folder))found.Add(new(app.Value,Path.GetFileNameWithoutExtension(path),folder,link.Target,"Desktop/Start shortcut")); }
                }
            } catch(Exception) { }
        }
        return found.Where(f=>!catalog.Profiles.Any(p=>Paths.Same(p.DataPath,f.DataPath))).DistinctBy(f=>Path.GetFullPath(f.DataPath),StringComparer.OrdinalIgnoreCase).ToList();
    }
}
