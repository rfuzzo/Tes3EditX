using AppUIBasics.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Services;
using Windows.ApplicationModel;

namespace Tes3EditX.Winui.Services;

public class SettingsService : ISettingsService
{
    public static string WDIR = "WDIR";

    public string Name => AppInfo.Current.Package.DisplayName;

    public string VersionString => string.Format("Version: {0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);

    public DirectoryInfo GetWorkingDirectory()
    {
        //return Preferences.Default.Get(WDIR, new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory));
        if (ThemeHelper.IsPackaged())
        {
            return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        }
        else
        {
            return new DirectoryInfo(Directory.GetCurrentDirectory());
        }
        
    }

    public void SetWorkingDirectory(DirectoryInfo value)
    {
        //Preferences.Default.Set(WDIR, value);
    }

    public int MinConflicts { get; set; } = 2;

    public bool CullConflicts { get; set; } = false;
}
