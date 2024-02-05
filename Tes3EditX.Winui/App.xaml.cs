using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Tes3EditX.Backend.Services;
using Tes3EditX.Winui.Services;
using AppUIBasics.Helper;
using System.Reflection;
using Tes3EditX.Winui.Pages;
using Tes3EditX.Backend.ViewModels;
using System.Diagnostics.CodeAnalysis;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tes3EditX.Winui;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Services = ConfigureServices();

        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        StartupWindow = WindowHelper.CreateWindow();
        EnsureWindow();
    }

    private void EnsureWindow()
    {
        ThemeHelper.Initialize(Services.GetRequiredService<ISettingsService>());

        MainRoot = (FrameworkElement)StartupWindow.Content;

        (StartupWindow as MainWindow)?.Navigate(typeof(ComparePage), "");

        // Ensure the current window is active
        StartupWindow.Activate();
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // Get the initial window created for this app
    // On UWP, this is simply Window.Current
    // On Desktop, multiple Windows may be created, and the StartupWindow may have already
    // been closed.
    public static Window StartupWindow { get; private set; }

    public static FrameworkElement MainRoot { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static new App Current => (App)Application.Current;


    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Appservices
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ICompareService, CompareService>();

        services.AddSingleton<IFileApiService, FileApiService>();

        // ViewModels
        services.AddSingleton<ComparePluginViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<CompareViewModel>();
        services.AddSingleton<ConflictsViewModel>();
        services.AddTransient<SettingsViewModel>();


        // Views
        services.AddSingleton<ComparePluginPage>();
        services.AddSingleton<ComparePage>();


        return services.BuildServiceProvider();
    }

   

    public static TEnum GetEnum<TEnum>(string text) where TEnum : struct
    {
        return !typeof(TEnum).GetTypeInfo().IsEnum
            ? throw new InvalidOperationException("Generic parameter 'TEnum' must be an enum.")
            : (TEnum)Enum.Parse(typeof(TEnum), text);
    }

    public string Name => AppInfo.Current.Package.DisplayName;

    public string VersionString => string.Format("Version: {0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);

    public DirectoryInfo GetWorkingDirectory()
    {
        return NativeHelper.IsAppPackaged
            ? new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)
            : new DirectoryInfo(Directory.GetCurrentDirectory());

    }
}