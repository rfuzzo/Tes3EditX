using AppUIBasics.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Text.Json;
using Tes3EditX.Backend.Services;
using Windows.ApplicationModel;

namespace Tes3EditX.Winui.Services;

public partial class SettingsServiceUnpackaged : ObservableObject, ISettingsService
{
    private const string FileName = "appsettings.json";
    public string GetName() => AppInfo.Current.Package.DisplayName;

    public string GetVersionString() => string.Format("Version: {0}.{1}.{2}.{3}",
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


    public SettingsServiceUnpackaged()
    {
        PropertyChanged += SettingsServiceUnpackaged_PropertyChanged;
    }

    private void SettingsServiceUnpackaged_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            default:
                {
                    // save to file
                    string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                    });
                    File.WriteAllText(GetFullPath(), jsonString);
                    break;
                }
        }
    }

    [ObservableProperty]
    private int minConflicts = 2;

    [ObservableProperty]
    private bool cullConflicts = false;

    [ObservableProperty]
    private string theme = ElementTheme.Dark.ToString();

    private static string GetFullPath()
    {
        return Path.Combine(System.AppContext.BaseDirectory, FileName);
    }

    public static SettingsServiceUnpackaged Load()
    {
        if (File.Exists(GetFullPath()))
        {
            string jsonString = File.ReadAllText(GetFullPath());
            SettingsServiceUnpackaged instance = JsonSerializer.Deserialize<SettingsServiceUnpackaged>(jsonString)!;
            return instance;
        }
        else
        {
            return new SettingsServiceUnpackaged();
        }
    }
}
