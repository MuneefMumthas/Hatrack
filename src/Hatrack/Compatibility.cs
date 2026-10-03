using System.Text.Json;

namespace Hatrack;
public static class Compatibility
{
    public static string Tested(DesktopApp app)
    {
        using var stream=typeof(Compatibility).Assembly.GetManifestResourceStream("Hatrack.compatibility.json") ?? throw new InvalidOperationException("Compatibility information is missing. Reinstall Hatrack.");
        using var json=JsonDocument.Parse(stream);
        return json.RootElement.GetProperty("apps").GetProperty(app.ToString()).GetProperty("observedPackageVersion").GetString()??"";
    }
    // Untested vendor versions are confirmed once per version instead of blocked, because vendor apps update often.
    public static bool NeedsConfirmation(Installation installation,Catalog catalog)=>installation.Version!=Tested(installation.App)&&!(catalog.AcceptedVersions.TryGetValue(installation.App,out var accepted)&&accepted==installation.Version);
    public static string Warning(Installation installation)=>$"{installation.App} Desktop {installation.Version} has not been tested with this version of Hatrack. It was tested with {Tested(installation.App)}.\n\nHatrack does not modify the desktop app; it only gives each profile its own data folder. A vendor update can still change how separate profiles behave. If something looks wrong, close the profile and report it on GitHub.\n\nOpen this profile anyway? You won't be asked again for this version.";
}
