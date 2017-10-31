using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: ComVisible(false)]
[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsPrefix("http://schemas.csharpstudio.com/winfx/xaml/controls", "ui")]
[assembly: XmlnsDefinition("http://schemas.csharpstudio.com/winfx/xaml/controls", "Css.Wpf.UI.Controls")]
[assembly: XmlnsDefinition("http://schemas.csharpstudio.com/winfx/xaml/controls", "Css.Wpf.UI.Controls.AvalonDock")]
[assembly: XmlnsDefinition("http://schemas.csharpstudio.com/winfx/xaml/controls", "Css.Wpf.UI.Controls.AvalonDock.Layout")]
[assembly: XmlnsDefinition("http://schemas.csharpstudio.com/winfx/xaml/controls", "Css.Wpf.UI.Controls.Dashboards")]
[assembly: XmlnsDefinition("http://schemas.csharpstudio.com/winfx/xaml/controls", "Css.Wpf.UI.Controls.ToolbarIcon")]
[assembly: XmlnsDefinition("http://schemas.csharpstudio.com/winfx/xaml/controls", "Css.Wpf.UI.Controls.MultiComboBox")]

[assembly: AssemblyTitle("Css.Wpf.UI")]
[assembly: AssemblyProduct("Css.Wpf.UI")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyVersion(AssemblyInformation.Version)]
[assembly: AssemblyFileVersion(AssemblyInformation.Version)]
[assembly: AssemblyCopyright(AssemblyInformation.AssemblyCopyright)]
[assembly: AssemblyTrademark(AssemblyInformation.AssemblyTrademark)]
[assembly: AssemblyDescription(AssemblyInformation.Description)]
[assembly: AssemblyCompany(AssemblyInformation.AssemblyCompany)]

internal static class AssemblyInformation
{
    public const string AssemblyCopyright = "Copyright © 2018 www.csharpstudio.org";
    public const string AssemblyTrademark = "csharpstudio";
    public const string AssemblyCompany = "www.csharpstudio.org";
    public const string Version = "1.0.9.0";
    public const string Description = "";
}