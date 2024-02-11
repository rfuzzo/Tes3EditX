using AppUIBasics.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Tes3EditX.Backend.Services;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Tes3EditX.Winui.Services;

public class SettingsServicePackaged : ObservableObject, ISettingsService
{
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

    public int MinConflicts
    {
        get => Get(2); 
        set
        {
            Set(value);
        }
    }
    public bool CullConflicts { get => Get(false); set => Set(value); }
    public bool OverwriteOnSave { get => Get(false); set => Set(value); }
    public bool Readonly { get => Get(false); set => Set(value); }
    public string Theme { get => Get("Dark"); set => Set(value); }

    public static T Get<T>(T defaultValue, [CallerMemberName] string? key = null) where T : notnull
    {
        var obj = ApplicationData.Current.LocalSettings.Values[key];
        if (obj is T value)
        {
            return value;
        }

        return defaultValue is null ? default! : defaultValue;
    }

    public static void Set<T>(T value, [CallerMemberName] string? key = null)
    {
        ApplicationData.Current.LocalSettings.Values[key] = value;
    }
}
