using System.IO;
using System.Text.RegularExpressions;

namespace Hatrack;

public enum DesktopApp { Claude, Codex }
public sealed record Installation(DesktopApp App, string Executable, string Version, string Source);
public sealed class Profile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DesktopApp App { get; set; }
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#171717";
    public string Shape { get; set; } = "Windows";
    public string DataPath { get; set; } = "";
    public string IconPath { get; set; } = "";
    public string IconSource { get; set; } = "";
    public string ExecutableOverride { get; set; } = "";
    public string CachePath { get; set; } = "";
    public string DesktopPath { get; set; } = "";
    public string StartMenuPath { get; set; } = "";
    public bool Favorite { get; set; }
    public bool Imported { get; set; }
    // Pre-rename identifier kept so existing pinned taskbar items stay grouped with their profile.
    public string AppId => "Profiles.Desktop." + Id.ToString("N");
    public string Display => $"{Name} · {App} Desktop";
}
public sealed class Catalog
{
    public int SchemaVersion { get; set; } = 1;
    public string Theme { get; set; } = "System";
    public Dictionary<DesktopApp,string> Locations { get; set; } = new();
    public List<Profile> Profiles { get; set; } = new();
    public Dictionary<DesktopApp,string> AcceptedVersions { get; set; } = new();
}
public sealed record DiscoveredProfile(DesktopApp App, string Name, string DataPath, string Executable, string Evidence);
public static class Paths
{
    public static string Root => Environment.GetEnvironmentVariable("HATRACK_TEST_HOME") ?? DataRoot(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
    // Catalogues created before the rename stay in place; profile data paths inside them are absolute.
    public static string DataRoot(string local)
    {
        var current=Path.Combine(local,"Hatrack");var legacy=Path.Combine(local,"Profiles");
        return !File.Exists(Path.Combine(current,"catalog.json"))&&File.Exists(Path.Combine(legacy,"catalog.json"))?legacy:current;
    }
    public static string Catalog => Path.Combine(Root,"catalog.json");
    public static string ProfileRoot(Guid id) => Path.Combine(Root,"profiles",id.ToString("N"));
    public static string Desktop => Environment.GetEnvironmentVariable("HATRACK_TEST_HOME") is {} test ? Path.Combine(test,"shortcuts","Desktop") : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    public static string Start => Environment.GetEnvironmentVariable("HATRACK_TEST_HOME") is {} test ? Path.Combine(test,"shortcuts","Start") : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs),"Hatrack");
    public static string OwnExe => Environment.ProcessPath ?? throw new InvalidOperationException("Cannot locate Hatrack.");
    public static string ValidateName(string name)
    {
        name = name.Trim();
        if (name.Length is < 1 or > 60 || name.Any(char.IsControl) || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.EndsWith('.') || name.EndsWith(' '))
            throw new ArgumentException("Use 1-60 characters, without file-name symbols or control characters.");
        if (Regex.IsMatch(name,@"^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])(\.|$)",RegexOptions.IgnoreCase)) throw new ArgumentException("Choose a different name; Windows reserves this one.");
        return name;
    }
    public static bool Same(string a,string b) => string.Equals(Path.GetFullPath(a).TrimEnd(Path.DirectorySeparatorChar),Path.GetFullPath(b).TrimEnd(Path.DirectorySeparatorChar),StringComparison.OrdinalIgnoreCase);
    public static bool Inside(string path,string parent) => Path.GetFullPath(path).StartsWith(Path.GetFullPath(parent).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase);
    public static void ValidateProfileFolder(string path)
    {
        if (!Path.IsPathFullyQualified(path) || !Directory.Exists(path)) throw new ArgumentException("Choose an existing absolute profile folder.");
        var full=Path.GetFullPath(path);
        var forbidden=new[]{Environment.GetFolderPath(Environment.SpecialFolder.Windows),Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)};
        if (forbidden.Any(p=>!string.IsNullOrEmpty(p)&&(Same(full,p)|| (p==forbidden[0]||p==forbidden[1])&&Inside(full,p))) || Same(full,Path.GetPathRoot(full)!)) throw new ArgumentException("Choose the specific profile data folder, not a system folder or your home folder.");
        if ((File.GetAttributes(full)&FileAttributes.ReparsePoint)!=0) throw new ArgumentException("Choose a real profile folder instead of a symbolic link.");
    }
}
