using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace Hatrack;
public static class Icons
{
    public static readonly string[] Shapes={"Claude logo","Codex logo"};
    public static readonly string[] Presets={"#171717","#FFFFFF","#2563EB","#7C3AED","#EAB308","#DC2626","#EA580C","#DB2777"};
    private static readonly Dictionary<string,BitmapSource> Marks=new();
    private static BitmapSource Mark(string shape)
    {
        if(Marks.TryGetValue(shape,out var cached))return cached;
        using var stream=typeof(Icons).Assembly.GetManifestResourceStream(shape=="Claude logo"?"Hatrack.claude-mark.png":"Hatrack.codex-mark.png") ?? throw new InvalidDataException("The app logo is missing. Reinstall Hatrack.");
        var image=BitmapDecoder.Create(stream,BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad).Frames[0];image.Freeze();Marks[shape]=image;return image;
    }
    public static BitmapSource Render(string color,string shape,string name,string? upload=null,double zoom=1,double offsetX=0,double offsetY=0,string? background=null,int size=256)
    {
        if(!double.IsFinite(zoom)||zoom<0.5||zoom>4||Math.Abs(offsetX)>1||Math.Abs(offsetY)>1)throw new ArgumentException("Invalid icon crop.");
        var brush=new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        var visual=new DrawingVisual();
        using(var dc=visual.RenderOpen()) {
            dc.PushTransform(new ScaleTransform(size/256.0,size/256.0));
            if(!string.IsNullOrEmpty(background)&&background!="Transparent")dc.DrawRoundedRectangle(new SolidColorBrush((Color)ColorConverter.ConvertFromString(background)),null,new Rect(0,0,256,256),40,40);
            if(!string.IsNullOrEmpty(upload)) {
                var image=Load(upload); dc.PushClip(new RectangleGeometry(new Rect(0,0,256,256)));
                var scale=Math.Min(256.0/image.PixelWidth,256.0/image.PixelHeight)*zoom;
                var w=image.PixelWidth*scale;var h=image.PixelHeight*scale;
                dc.DrawImage(image,new Rect((256-w)/2+offsetX*96,(256-h)/2+offsetY*96,w,h));dc.Pop();
            } else if(shape is "Claude logo" or "Codex logo") {
                dc.PushClip(new RectangleGeometry(new Rect(0,0,256,256)));
                var width=256*zoom;var rect=new Rect((256-width)/2+offsetX*96,(256-width)/2+offsetY*96,width,width);
                dc.PushOpacityMask(new ImageBrush(Mark(shape)){Stretch=Stretch.Fill,ViewportUnits=BrushMappingMode.Absolute,Viewport=rect,TileMode=TileMode.None});
                dc.DrawRectangle(brush,null,rect);dc.Pop();dc.Pop();
            } else if(shape=="Windows") {
                var pen=new Pen(brush,16);dc.DrawRoundedRectangle(null,pen,new Rect(36,32,148,152),22,22);dc.DrawRoundedRectangle(brush,null,new Rect(76,76,148,152),22,22);
                dc.DrawRoundedRectangle(Brushes.White,null,new Rect(97,99,104,12),6,6);
            } else if(shape=="Folder") {
                dc.DrawRoundedRectangle(brush,null,new Rect(32,64,106,60),12,12);dc.DrawRoundedRectangle(brush,null,new Rect(32,94,192,116),18,18);dc.DrawRoundedRectangle(Brushes.White,null,new Rect(58,128,140,12),6,6);
            } else if(shape=="Diamond") {
                dc.PushTransform(new RotateTransform(45,128,128));dc.DrawRoundedRectangle(brush,null,new Rect(55,55,146,146),22,22);dc.DrawRoundedRectangle(Brushes.White,null,new Rect(102,102,52,52),8,8);dc.Pop();
            } else {
                dc.DrawRoundedRectangle(brush,null,new Rect(24,24,208,208),40,40);
                var initial=StringInfo.GetNextTextElement(string.IsNullOrWhiteSpace(name)?"P":name.Trim()).ToUpperInvariant();
                var text=new FormattedText(initial,CultureInfo.InvariantCulture,FlowDirection.LeftToRight,new Typeface("Segoe UI"),110,Brushes.White,1);
                dc.DrawText(text,new System.Windows.Point((256-text.Width)/2,(256-text.Height)/2-2));
            }
            dc.Pop();
        }
        var bitmap=new RenderTargetBitmap(size,size,96,96,PixelFormats.Pbgra32);bitmap.Render(visual);bitmap.Freeze();return bitmap;
    }
    // Hatrack mark on a 64-unit grid: two shelves, a top hat above two bowlers. Mirrors assets/logo.svg.
    public static Geometry Mark()
    {
        var g=new GeometryGroup{FillRule=FillRule.Nonzero};
        foreach(var (x,y,w,h,r) in new[]{(12.0,29.5,40.0,4.5,2.25),(2,58,60,4.5,2.25),(24.5,2,15,21,2.5),(19,21,26,5,2.5),(3,50,26,5,2.5),(35,50,26,5,2.5)})g.Children.Add(new RectangleGeometry(new Rect(x,y,w,h),r,r));
        g.Children.Add(Geometry.Parse("M7 52V45A9 9 0 0 1 25 45V52Z"));g.Children.Add(Geometry.Parse("M39 52V45A9 9 0 0 1 57 45V52Z"));
        g.Freeze();return g;
    }
    public static BitmapSource Brand(int size=256,bool tile=true)
    {
        var visual=new DrawingVisual();
        using(var dc=visual.RenderOpen()) {
            dc.PushTransform(new ScaleTransform(size/256.0,size/256.0));
            var ink=new SolidColorBrush(Color.FromRgb(22,22,23));
            if(tile)dc.DrawRoundedRectangle(ink,null,new Rect(0,0,256,256),56,56);
            var scale=tile?2.75:4.0;var offset=tile?40.0:0;
            dc.PushTransform(new MatrixTransform(scale,0,0,scale,offset,offset));dc.DrawGeometry(tile?Brushes.White:ink,null,Mark());dc.Pop();dc.Pop();
        }
        var bitmap=new RenderTargetBitmap(size,size,96,96,PixelFormats.Pbgra32);bitmap.Render(visual);bitmap.Freeze();return bitmap;
    }
    public static BitmapSource Load(string path)
    {
        var ext=Path.GetExtension(path).ToLowerInvariant();
        if(ext is not ".png" and not ".jpg" and not ".jpeg" and not ".ico")throw new ArgumentException("Upload a PNG, JPEG, or ICO image.");
        if(new FileInfo(path).Length>10*1024*1024)throw new ArgumentException("Choose an image smaller than 10 MB.");
        try {
            using var stream=File.OpenRead(path);
            var decoder=BitmapDecoder.Create(stream,BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);
            var frame=decoder.Frames.OrderByDescending(f=>f.PixelWidth).First();
            if(frame.PixelWidth>4096||frame.PixelHeight>4096)throw new ArgumentException("Choose an image no larger than 4096 pixels on either side.");
            frame.Freeze();return frame;
        } catch(Exception e) when(e is not ArgumentException) { throw new ArgumentException("This image cannot be decoded. Choose another PNG, JPEG, or ICO.",e); }
    }
    public static void WritePng(BitmapSource source,string path) { var e=new PngBitmapEncoder();e.Frames.Add(BitmapFrame.Create(source));using var f=File.Create(path);e.Save(f); }
    public static void WriteIco(string path,Func<int,BitmapSource> renderer)
    {
        var sizes=new[]{16,24,32,48,64,128,256};var buffers=new List<byte[]>();
        foreach(var size in sizes){using var m=new MemoryStream();var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(renderer(size)));encoder.Save(m);buffers.Add(m.ToArray());}
        using var f=File.Create(path);using var writer=new BinaryWriter(f);writer.Write((ushort)0);writer.Write((ushort)1);writer.Write((ushort)sizes.Length);
        var offset=6+16*sizes.Length;
        for(var i=0;i<sizes.Length;i++){writer.Write((byte)(sizes[i]==256?0:sizes[i]));writer.Write((byte)(sizes[i]==256?0:sizes[i]));writer.Write((byte)0);writer.Write((byte)0);writer.Write((ushort)1);writer.Write((ushort)32);writer.Write(buffers[i].Length);writer.Write(offset);offset+=buffers[i].Length;}
        foreach(var buffer in buffers)writer.Write(buffer);
    }
}
