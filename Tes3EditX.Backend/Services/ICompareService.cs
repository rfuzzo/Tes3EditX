using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Extensions;
using Tes3EditX.Backend.ViewModels;
using TES3Lib;

namespace Tes3EditX.Backend.Services;

public interface ICompareService
{
    public DirectoryInfo? DataFiles { get; set; }
    public Dictionary<FileInfo, TES3> Plugins { get; }
    public Dictionary<RecordId, List<FileInfo>> Conflicts { get; set; }
    public IEnumerable<PluginItemViewModel> Selectedplugins { get; set; }

    public Dictionary<FileInfo, List<RecordId>> DirtyRecords { get; set; }

    Task CalculateConflicts();
    List<(string, List<RecordFieldViewModel>)> GetConflictMap(List<FileInfo> plugins, RecordId recordId, List<string> names);
    List<string> GetNames(string tag);
    bool HasAnyConflict(RecordId key, List<FileInfo> plugins);
}
