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
        // deactivate
        foreach (var item in e.RemovedItems)
        {
            if (item is PluginItemViewModel vm)
            {
                vm.Enabled = false;
            }
        }
        foreach (var item in e.AddedItems)
        {
            if (item is PluginItemViewModel vm)
            {
                vm.Enabled = true;
            }
        }

        // activate masters
        if (activateMastersButton.IsChecked.HasValue)
        {
            if ((bool)activateMastersButton.IsChecked)
            {
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
                    var x = ViewModel.PluginsDisplay.FirstOrDefault(x => x.Name == item);
                    if (x is not null)
                    {
                        var i = ViewModel.PluginsDisplay.IndexOf(x);
                        listview.SelectRange(new ItemIndexRange(i, 1));
                    }

                    var y = ViewModel.PluginsList.FirstOrDefault(x => x.Name == item);
                    if (y is not null)
                    {
                        y.Enabled = true;
                    }
                }
            }
        }

        // sort them again
        //ViewModel.SelectedPlugins = listview.SelectedItems
        //    .Cast<PluginItemViewModel>()
        //    .OrderBy(x => x.Info.Extension.ToLower()).ThenBy(x => x.Info.LastWriteTime)
        //    .ToList();
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox box)
        {
            ViewModel.PluginFilterText = box.Text;
            ViewModel.Filter(box.Text);
        }

        // select items
        foreach (var x in ViewModel.PluginsDisplay.Where(x => x.Enabled))
        {
            var i = ViewModel.PluginsDisplay.IndexOf(x);
            listview.SelectRange(new ItemIndexRange(i, 1));
        }
    }

    private void AppBarButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        listview.SelectAll();
    }

    private void AppBarButtonClear_Click(object sender, RoutedEventArgs e)
    {
        listview.SelectedItems.Clear();
    }
}
