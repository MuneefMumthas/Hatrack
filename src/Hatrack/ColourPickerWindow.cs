using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hatrack;
public sealed class ColourPickerWindow:Window
{
    public string SelectedColour {get;private set;}="";
    public ColourPickerWindow(Window owner,string initial)
    {
        Owner=owner;Title="Choose icon colour";Width=420;Height=440;ResizeMode=ResizeMode.NoResize;WindowStartupLocation=WindowStartupLocation.CenterOwner;FontFamily=new FontFamily("Segoe UI");FontSize=14;AppTheme.Attach(this,owner);
        var panel=new StackPanel();var frame=new Border{Padding=new Thickness(28),Child=panel};frame.SetResourceReference(Border.BackgroundProperty,"Canvas");Content=frame;
        panel.Children.Add(new TextBlock{Text="Icon colour",FontSize=24,FontWeight=FontWeights.SemiBold,Margin=new Thickness(0,0,0,20)});
        var tile=new Border{Height=64,CornerRadius=new CornerRadius(14),Margin=new Thickness(0,0,0,18)};panel.Children.Add(tile);
        Color parsed;try{parsed=(Color)ColorConverter.ConvertFromString(initial);}catch{parsed=Colors.Black;}
        var sliders=new List<Slider>();var hex=new TextBlock{FontWeight=FontWeights.SemiBold,Margin=new Thickness(0,14,0,16)};
        foreach(var channel in new[]{("Red",parsed.R),("Green",parsed.G),("Blue",parsed.B)}){
            var row=new Grid{Margin=new Thickness(0,5,0,5)};row.ColumnDefinitions.Add(new ColumnDefinition{Width=new GridLength(56)});row.ColumnDefinitions.Add(new ColumnDefinition());row.ColumnDefinitions.Add(new ColumnDefinition{Width=new GridLength(40)});
            row.Children.Add(new TextBlock{Text=channel.Item1,VerticalAlignment=VerticalAlignment.Center});var slider=new Slider{Minimum=0,Maximum=255,Value=channel.Item2,IsSnapToTickEnabled=true,TickFrequency=1};System.Windows.Automation.AutomationProperties.SetName(slider,channel.Item1+" colour channel");sliders.Add(slider);Grid.SetColumn(slider,1);row.Children.Add(slider);
            var value=new TextBlock{Text=channel.Item2.ToString(),HorizontalAlignment=HorizontalAlignment.Right,VerticalAlignment=VerticalAlignment.Center};Grid.SetColumn(value,2);row.Children.Add(value);slider.ValueChanged+=(_,_)=>{value.Text=((int)slider.Value).ToString();Update();};panel.Children.Add(row);
        }
        panel.Children.Add(hex);var buttons=new StackPanel{Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right};var cancel=new Button{Content="Cancel"};cancel.Click+=(_,_)=>Close();var use=new Button{Content="Use colour",Style=(Style)FindResource("PrimaryButton")};use.Click+=(_,_)=>DialogResult=true;buttons.Children.Add(cancel);buttons.Children.Add(use);panel.Children.Add(buttons);Update();
        void Update(){if(sliders.Count!=3)return;var c=Color.FromRgb((byte)sliders[0].Value,(byte)sliders[1].Value,(byte)sliders[2].Value);SelectedColour=$"#{c.R:X2}{c.G:X2}{c.B:X2}";tile.Background=new SolidColorBrush(c);hex.Text=SelectedColour;}
    }
}
