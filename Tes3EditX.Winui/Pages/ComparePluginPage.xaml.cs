using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System.Collections.Generic;
using System.Linq;
using Tes3EditX.Backend.ViewModels;
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
        foreach (object? item in e.RemovedItems)
        {
            if (item is PluginItemViewModel vm)
            {
                vm.Enabled = false;
            }
        }
        foreach (object? item in e.AddedItems)
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
                List<string> toActivate = new();
                foreach (object? item in e.AddedItems)
                {
                    if (item is PluginItemViewModel vm)
                    {
                        TES3Lib.Base.Record? header = vm.Plugin.Records.FirstOrDefault(x => x.Name == "TES3");
                        if (header is not null and TES3 tes3)
                        {
                            List<(TES3Lib.Subrecords.TES3.MAST MAST, TES3Lib.Subrecords.TES3.DATA DATA)> masters = tes3.Masters.ToList();
                            foreach ((TES3Lib.Subrecords.TES3.MAST master, TES3Lib.Subrecords.TES3.DATA _) in masters)
                            {
                                string m = master.Filename;
                                toActivate.Add(m.TrimEnd('\0'));
                            }
                        }
                    }
                }

                foreach (string item in toActivate)
                {
                    PluginItemViewModel? x = ViewModel.PluginsDisplay.FirstOrDefault(x => x.Name == item);
                    if (x is not null)
                    {
                        int i = ViewModel.PluginsDisplay.IndexOf(x);
                        listview.SelectRange(new ItemIndexRange(i, 1));
                    }

                    PluginItemViewModel? y = ViewModel.PluginsList.FirstOrDefault(x => x.Name == item);
                    if (y is not null)
                    {
                        y.Enabled = true;
                    }
                }
            }
        }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox box)
        {
            ViewModel.PluginFilterText = box.Text;
            ViewModel.Filter(box.Text);
        }

        // select items
        foreach (PluginItemViewModel? x in ViewModel.PluginsDisplay.Where(x => x.Enabled))
        {
            int i = ViewModel.PluginsDisplay.IndexOf(x);
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
