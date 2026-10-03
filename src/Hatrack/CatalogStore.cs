using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hatrack;
public static class CatalogStore
{
    public static readonly JsonSerializerOptions Json = new() { WriteIndented=true, Converters={new JsonStringEnumConverter()} };
    private static FileStream Lock()
    {
        Directory.CreateDirectory(Paths.Root);
        for(var i=0;i<50;i++) try { return new FileStream(Path.Combine(Paths.Root,"catalog.lock"),FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None); } catch(IOException) { Thread.Sleep(100); }
        throw new IOException("Another Hatrack operation is still saving. Try again in a moment.");
    }
    public static Catalog Read()
    {
        using var guard=Lock();
        return ReadUnlocked();
    }
    private static Catalog ReadUnlocked()
    {
        if(!File.Exists(Paths.Catalog)) return new Catalog();
        try { var c=JsonSerializer.Deserialize<Catalog>(File.ReadAllText(Paths.Catalog),Json) ?? throw new JsonException();
            if(c.SchemaVersion!=1) throw new InvalidOperationException("This catalogue needs a newer version of Hatrack. Your data has not been changed.");
            if(c.Profiles.Select(p=>p.Id).Distinct().Count()!=c.Profiles.Count||c.Profiles.Any(p=>p.Id==Guid.Empty||string.IsNullOrWhiteSpace(p.DataPath))) throw new JsonException();
            return c;
        } catch(JsonException) { throw new InvalidDataException("The profile catalogue cannot be read. Your app data is intact. A previous catalogue may be available at catalog.json.bak; restore it before continuing."); }
    }
    public static T Update<T>(Func<Catalog,T> operation,Action? rollback=null)
    {
        using var guard=Lock(); var c=ReadUnlocked();
        var tmp=Paths.Catalog+"."+Guid.NewGuid().ToString("N")+".tmp";
        try {
            var value=operation(c);
            using(var f=new FileStream(tmp,FileMode.CreateNew,FileAccess.Write,FileShare.None)) { JsonSerializer.Serialize(f,c,Json); f.Flush(true); }
            if(File.Exists(Paths.Catalog)) File.Replace(tmp,Paths.Catalog,Paths.Catalog+".bak"); else File.Move(tmp,Paths.Catalog);
            return value;
        } catch {
            // Restore shortcut mutations before another process can acquire the catalogue lock.
            rollback?.Invoke();
            throw;
        } finally { if(File.Exists(tmp)) File.Delete(tmp); }
    }
}
