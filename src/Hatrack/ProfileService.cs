using System.IO;

namespace Hatrack;
public sealed record IconOptions(string Shape,string Color,string Upload,double Zoom=1,double X=0,double Y=0,string Background="Transparent");
public static class ProfileService
{
    public static Profile Save(Guid? existing,DesktopApp app,string name,IconOptions icon,bool desktop,bool start,string? importedPath=null,string? exe=null,bool favorite=false)
    {
        name=Paths.ValidateName(name);
        if(importedPath!=null){Paths.ValidateProfileFolder(importedPath);if(app==DesktopApp.Codex&&Paths.Same(importedPath,Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),".codex")))throw new ArgumentException("The original Codex home cannot be adopted. Create a separate profile instead.");}
        var undo=new Dictionary<string,byte[]?>();string? created=null;string? iconPath=null;string? normalized=null;
        void Snapshot(string path){if(!undo.ContainsKey(path))undo.Add(path,File.Exists(path)?File.ReadAllBytes(path):null);}
        return CatalogStore.Update(c=>{
                if(c.Profiles.Any(p=>p.Id!=existing&&p.App==app&&p.Name.Equals(name,StringComparison.OrdinalIgnoreCase)))throw new ArgumentException("A profile for this app already uses that name.");
                if(importedPath!=null&&c.Profiles.Any(p=>p.Id!=existing&&Paths.Same(p.DataPath,importedPath)))throw new ArgumentException("That folder is already managed by Hatrack.");
                var old=existing==null?null:c.Profiles.Single(p=>p.Id==existing);
                var p=old==null?new Profile{App=app}:System.Text.Json.JsonSerializer.Deserialize<Profile>(System.Text.Json.JsonSerializer.Serialize(old,CatalogStore.Json),CatalogStore.Json)!;
                p.Name=name;p.App=app;p.Color=icon.Color;p.Shape=string.IsNullOrEmpty(icon.Upload)?icon.Shape:"Upload";p.Favorite=favorite;
                var root=Paths.ProfileRoot(p.Id);
                if(!Directory.Exists(root)){created=root;Directory.CreateDirectory(root);}
                if(old==null){p.Imported=importedPath!=null;p.DataPath=importedPath??Path.Combine(root,app==DesktopApp.Claude?"data":"codex-home");p.CachePath=Path.Combine(root,"cache");}
                if(!string.IsNullOrEmpty(exe))p.ExecutableOverride=Discovery.Validate(app,exe).Executable;
                // Render before mutating any existing shortcut or account data.
                var revision=Guid.NewGuid().ToString("N");iconPath=Path.Combine(root,"icon-"+revision+".ico");normalized=Path.Combine(root,"art-"+revision+".png");
                var image=Icons.Render(icon.Color,icon.Shape,name,icon.Upload,icon.Zoom,icon.X,icon.Y,icon.Background);Icons.WritePng(image,normalized);
                Icons.WriteIco(iconPath,size=>Icons.Render(icon.Color,icon.Shape,name,icon.Upload,icon.Zoom,icon.X,icon.Y,icon.Background,size));p.IconPath=iconPath;p.IconSource=normalized;
                var filename=$"{name} ({app}).lnk";
                var newDesktop=desktop?Path.Combine(Paths.Desktop,filename):"";var newStart=start?Path.Combine(Paths.Start,filename):"";
                foreach(var target in new[]{newDesktop,newStart}.Where(x=>x!="")) {
                    if(File.Exists(target)&&!Shell.Owns(target,p.Id))throw new IOException("A shortcut already exists: "+Path.GetFileName(target)+". Choose another name; the existing shortcut was not changed.");
                    Snapshot(target);var temp=target+"."+Guid.NewGuid().ToString("N")+".lnk";
                    try{Shell.Shortcut(temp,p);File.Move(temp,target,true);}finally{if(File.Exists(temp))File.Delete(temp);}
                }
                if(old!=null)foreach(var target in new[]{old.DesktopPath,old.StartMenuPath}.Where(x=>x!=""&&x!=newDesktop&&x!=newStart)) {
                    if(Shell.Owns(target,p.Id)){Snapshot(target);File.Delete(target);}
                }
                p.DesktopPath=newDesktop;p.StartMenuPath=newStart;
                Directory.CreateDirectory(p.DataPath);Directory.CreateDirectory(p.CachePath);
                if(app==DesktopApp.Codex&&!p.Imported) {
                    var config=Path.Combine(p.DataPath,"config.toml");
                    if(!File.Exists(config))File.WriteAllText(config,"# Profile-local authentication. No credentials are copied.\ncli_auth_credentials_store = \"file\"\n");
                }
                if(old!=null)c.Profiles.Remove(old);c.Profiles.Add(p);return p;
            },()=>{
            foreach(var pair in undo){if(pair.Value==null){if(File.Exists(pair.Key))File.Delete(pair.Key);}else File.WriteAllBytes(pair.Key,pair.Value);}
            if(created!=null&&Directory.Exists(created))Directory.Delete(created,true);
            else{if(iconPath!=null&&File.Exists(iconPath))File.Delete(iconPath);if(normalized!=null&&File.Exists(normalized))File.Delete(normalized);}
        });
    }
    public static void Remove(Guid id)
    {
        var undo=new Dictionary<string,byte[]>();
        CatalogStore.Update(c=>{var p=c.Profiles.Single(p=>p.Id==id);foreach(var link in new[]{p.DesktopPath,p.StartMenuPath}.Where(x=>x!=""))if(Shell.Owns(link,id)){undo[link]=File.ReadAllBytes(link);File.Delete(link);}c.Profiles.Remove(p);return true;},()=>{foreach(var pair in undo)File.WriteAllBytes(pair.Key,pair.Value);});
    }
    public static int RepairShortcuts()
    {
        var repaired=0;
        foreach(var p in CatalogStore.Read().Profiles)foreach(var link in new[]{p.DesktopPath,p.StartMenuPath}.Where(x=>x!="")) {
            if(!Shell.Owns(link,p.Id)||Paths.Same(Shell.ReadShortcut(link).Target,Paths.OwnExe))continue;
            var temp=link+"."+Guid.NewGuid().ToString("N")+".lnk";
            try{Shell.Shortcut(temp,p);File.Move(temp,link,true);repaired++;}finally{if(File.Exists(temp))File.Delete(temp);}
        }
        return repaired;
    }
}
