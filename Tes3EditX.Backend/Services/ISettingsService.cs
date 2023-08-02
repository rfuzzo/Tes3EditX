using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tes3EditX.Backend.Services;

public interface ISettingsService
{
    string Name { get; }
    string VersionString { get; }
    int MinConflicts { get; set; }
    bool CullConflicts { get; set; }

    DirectoryInfo GetWorkingDirectory();
    void SetWorkingDirectory(DirectoryInfo value);
}
