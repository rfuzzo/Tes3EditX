using AppUIBasics.Helper;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Tes3EditX.Backend.Services;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Tes3EditX.Winui.Services;

public class SettingsService : ISettingsService
{
    public int MinConflicts
    {
        get => Get(2); 
        set
        {
            Set(value);
        }
    }
    public bool CullConflicts { get => Get(false); set => Set(value); }
    public string Theme { get => Get("Dark"); set => Set(value); }

    public static T Get<T>(T defaultValue, [CallerMemberName] string? key = null) where T : notnull
    {
        if (NativeHelper.IsAppPackaged)
        {
            var obj = ApplicationData.Current.LocalSettings.Values[key];
            if (obj is T value)
            {
                return value;
            }
        }
        else
        {
            throw new NotImplementedException();
        }

        return defaultValue is null ? default! : defaultValue;
    }

    public static void Set<T>(T value, [CallerMemberName] string? key = null)
    {
        if (NativeHelper.IsAppPackaged)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}
