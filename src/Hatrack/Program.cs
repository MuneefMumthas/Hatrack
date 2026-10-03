using System.IO;
using System.Reflection;
using System.Windows;
namespace Hatrack;
public static class Program
{
    public static string Version=>(typeof(Program).Assembly.GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()?.InformationalVersion??"0.0.0").Split('+')[0];
    [STAThread]public static int Main(string[] args)
    {
        try{
            if(args.Length==2&&args[0]=="--launch"&&Guid.TryParse(args[1],out var id))return Launcher.Run(id);
            if(args.Length==2&&args[0]=="--generate-assets"){GenerateAssets(Path.GetFullPath(args[1]));return 0;}
            if(args.Length==2&&args[0]=="--generate-social"){GenerateSocial(Path.GetFullPath(args[1]));return 0;}
            if(args.Length==2&&args[0]=="--smoke")return Smoke.Run(args[1]);
            if(args.Length==1&&args[0]=="--repair-shortcuts"){ProfileService.RepairShortcuts();return 0;}
            if(args.Length==1&&args[0]=="--uninstall-shortcuts"){foreach(var p in CatalogStore.Read().Profiles)foreach(var link in new[]{p.DesktopPath,p.StartMenuPath}.Where(x=>x!=""))if(Shell.Owns(link,p.Id))File.Delete(link);return 0;}
            if(args.Length==2&&args[0].StartsWith("--capture",StringComparison.Ordinal)){
                if(Environment.GetEnvironmentVariable("HATRACK_TEST_HOME")==null)throw new ArgumentException("Capture requires an isolated HATRACK_TEST_HOME.");
                var dark=args[0].Contains("dark");CatalogStore.Update(c=>{c.Theme=dark?"Dark":"Light";return true;});
                if(!CatalogStore.Read().Profiles.Any()){ProfileService.Save(null,DesktopApp.Claude,"Personal",new("Claude logo","#D97757",""),false,false);ProfileService.Save(null,DesktopApp.Codex,"Work",new("Codex logo","#7C3AED",""),false,false);}
                var app=new System.Windows.Application();var w=new MainWindow();w.Show();w.Dispatcher.InvokeAsync(async()=>{await Task.Delay(1800);Window target=w;
                    if(args[0].Contains("editor")){var editor=new EditorWindow(Discovery.Detect(CatalogStore.Read())){Owner=w};editor.Show();target=editor;await Task.Delay(500);if(args[0].Contains("colour")){var picker=new ColourPickerWindow(editor,"#7C3AED");picker.Show();target=picker;await Task.Delay(200);}}
                    var visual=(System.Windows.FrameworkElement)target.Content;var width=(int)visual.ActualWidth;var height=(int)visual.ActualHeight;var drawing=new System.Windows.Media.DrawingVisual();using(var dc=drawing.RenderOpen()){dc.DrawRectangle(target.Background,null,new Rect(0,0,width,height));dc.DrawRectangle(new System.Windows.Media.VisualBrush(visual){ViewboxUnits=System.Windows.Media.BrushMappingMode.Absolute,Viewbox=new Rect(0,0,width,height)},null,new Rect(0,0,width,height));}var bmp=new System.Windows.Media.Imaging.RenderTargetBitmap(width,height,96,96,System.Windows.Media.PixelFormats.Pbgra32);bmp.Render(drawing);Icons.WritePng(bmp,args[1]);if(target!=w)target.Close();w.Close();});app.Run();return 0;}
            if(args.Length!=0)throw new ArgumentException("Use Hatrack normally, or launch a profile with --launch <profile-id>.");
            Shell.SetCurrentProcessExplicitAppUserModelID("Hatrack.Manager");
            using var manager=new Mutex(true,"Local\\Hatrack.Manager",out var first);if(!first){System.Windows.MessageBox.Show("Hatrack is already open. Switch to its window from the taskbar.","Hatrack");return 0;}
            var application=new System.Windows.Application();application.DispatcherUnhandledException+=(_,e)=>{System.Windows.MessageBox.Show(e.Exception.Message,"Hatrack",MessageBoxButton.OK,MessageBoxImage.Warning);e.Handled=true;};application.Run(new MainWindow());return 0;
        }catch(Exception e){System.Windows.MessageBox.Show(e is System.ComponentModel.Win32Exception?e.Message+"\n\nIf Windows reports that another program is using the executable, finish app updates and restart Windows. Your profile data has not been changed.":e.Message,"Hatrack",MessageBoxButton.OK,MessageBoxImage.Warning);return 1;}
    }
    private static void GenerateAssets(string folder){Directory.CreateDirectory(folder);Icons.WritePng(Icons.Brand(512),Path.Combine(folder,"hatrack.png"));Icons.WriteIco(Path.Combine(folder,"hatrack.ico"),size=>Icons.Brand(size));foreach(var shape in Icons.Shapes)Icons.WritePng(Icons.Render("#171717",shape,"P"),Path.Combine(folder,shape.ToLowerInvariant()+".png"));}
    private static void GenerateSocial(string path)
    {
        var visual=new System.Windows.Media.DrawingVisual();using(var dc=visual.RenderOpen()){
            var ink=new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(22,22,23));
            dc.DrawRectangle(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(250,250,250)),null,new Rect(0,0,1200,630));
            dc.DrawImage(Icons.Brand(120),new Rect(64,66,120,120));
            Text("Hatrack",205,92,64,ink);Text("Your desktop profiles.",78,270,72,ink);Text("One place to manage them.",78,360,72,ink);
            Text("Claude Desktop · Codex Desktop · Windows · Open source",82,510,28,new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(98,98,103)));
            void Text(string value,double x,double y,double size,System.Windows.Media.Brush brush){dc.DrawText(new System.Windows.Media.FormattedText(value,System.Globalization.CultureInfo.InvariantCulture,FlowDirection.LeftToRight,new System.Windows.Media.Typeface("Segoe UI"),size,brush,1),new System.Windows.Point(x,y));}
        }
        var image=new System.Windows.Media.Imaging.RenderTargetBitmap(1200,630,96,96,System.Windows.Media.PixelFormats.Pbgra32);image.Render(visual);Icons.WritePng(image,path);
    }
}
