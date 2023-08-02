using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Services;

namespace Tes3EditX.Maui.Services;

public class SettingsService : ISettingsService
{
    public static string WDIR = "WDIR";

    public string Name => AppInfo.Name;

    public string VersionString => AppInfo.VersionString;

    public DirectoryInfo GetWorkingDirectory()
    {
        return Preferences.Default.Get(WDIR, new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory));
    }

    public void SetWorkingDirectory(DirectoryInfo value)
    {
        Preferences.Default.Set(WDIR, value);
    }
}
