using AppUIBasics.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Tes3EditX.Backend.Services;
using Windows.ApplicationModel;

namespace Tes3EditX.Winui.Services;

public partial class SettingsServiceUnpackaged : ObservableObject, ISettingsService
{
    private const string FileName = "appsettings.json";

    [ObservableProperty]
    private int minConflicts = 2;

    [ObservableProperty]
    private bool cullConflicts = false;

    [ObservableProperty]
    private string theme = ElementTheme.Dark.ToString();

    [ObservableProperty]
    private bool _overwriteOnSave;

    [ObservableProperty]
    private bool _readonly;

    public string GetName()
    {
        return "Tes3EditX";
    }

    public string GetVersionString()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        version ??= "";

        return version;
    }

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
                    JsonSerializerOptions jsonSerializerOptions = new()
                    {
                        WriteIndented = true,
                    };
                    JsonSerializerOptions options = jsonSerializerOptions;
                    string jsonString = JsonSerializer.Serialize(this, options);
                    File.WriteAllText(GetFullPath(), jsonString);
                    break;
                }
        }
    }    

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
