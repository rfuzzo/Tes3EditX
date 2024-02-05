using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Tes3EditX.Backend.ViewModels;
using Tes3EditX.Winui.Pages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tes3EditX.Winui;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }

    public MainViewModel ViewModel => App.Current.Services.GetRequiredService<MainViewModel>();

    public NavigationView NavigationView => NavigationViewControl;

    private void NavigationViewControl_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            if (rootFrame.CurrentSourcePageType != typeof(SettingsPage))
            {
                Navigate(typeof(SettingsPage));
            }
        }
        else
        {
            var selectedItem = args.SelectedItemContainer;
            var selectedTagStr = selectedItem.Tag as string;

            if (selectedTagStr == nameof(ComparePage))
            {
                if (rootFrame.CurrentSourcePageType != typeof(ComparePage))
                {
                    Navigate(typeof(ComparePage));
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    // Wraps a call to rootFrame.Navigate to give the Page a way to know which NavigationRootPage is navigating.
    // Please call this function rather than rootFrame.Navigate to navigate the rootFrame.
    public void Navigate(
        Type pageType,
        object? targetPageArguments = null,
        NavigationTransitionInfo? navigationTransitionInfo = null)
    {
        var args = new NavigationRootPageArgs
        {
            //NavigationRootPage = this,
            Parameter = targetPageArguments
        };
        rootFrame.Navigate(pageType, args, navigationTransitionInfo);
    }

    public class NavigationRootPageArgs
    {
        //public NavigationRootPage NavigationRootPage;
        public object? Parameter;
    }
}
