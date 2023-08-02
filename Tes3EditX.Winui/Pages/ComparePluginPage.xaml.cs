using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Tes3EditX.Backend.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using TES3Lib.Records;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tes3EditX.Winui.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ComparePluginPage : Page
{
    public ComparePluginPage()
    {
        this.InitializeComponent();
        Loaded += ComparePluginPage_Loaded;

        this.DataContext = App.Current.Services.GetService<ComparePluginViewModel>();
    }

    private async void ComparePluginPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitPluginsAsync();
    }

    public ComparePluginViewModel ViewModel => (ComparePluginViewModel)DataContext;

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // activate masters
        var toActivate = new List<string>();
        foreach (var item in e.AddedItems)
        {
            if (item is PluginItemViewModel vm)
            {
                var header = vm.Plugin.Records.FirstOrDefault(x => x.Name == "TES3");
                if (header is not null && header is TES3 tes3)
                {
                    var masters = tes3.Masters.ToList();
                    foreach (var (master, _) in masters)
                    {
                        var m = master.Filename;
                        toActivate.Add(m.TrimEnd('\0'));
                    }
                }
            }
        }

        foreach (var item in toActivate)
        {
            var x = ViewModel.Plugins.FirstOrDefault(x => x.Name == item);
            if (x is not null)
            {
                var i = ViewModel.Plugins.IndexOf(x);
                listview.SelectRange(new ItemIndexRange(i, 1));
            }
            
        }

        // sort them again
        ViewModel.SelectedPlugins = listview.SelectedItems
            .Cast<PluginItemViewModel>()
            .OrderBy(x => x.Info.Extension.ToLower()).ThenBy(x => x.Info.LastWriteTime)
            .ToList();
    }
}
