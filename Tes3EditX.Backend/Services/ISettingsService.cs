using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tes3EditX.Backend.Services;

public interface ISettingsService: INotifyPropertyChanged
{

    int MinConflicts { get; set; }
    bool CullConflicts { get; set; }
    string Theme { get; set; }
    bool OverwriteOnSave { get; set; }
    bool Readonly { get; set; }

    string GetName();
    string GetVersionString();
    DirectoryInfo GetWorkingDirectory();
}
