using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tes3EditX.Backend.ViewModels;

/// <summary>
/// This viewmodel wraps a TES3 record id
/// and is used to display records in a list
/// Tag is the record class
/// Name is the record id (not-unique!)
/// Plugins is a list of Plugins this record is found in
/// </summary>
public class RecordItemViewModel
{
    public RecordItemViewModel(string tag, string name, List<FileInfo> plugins )
    {
        Name = name;
        Tag = tag;

        Plugins = plugins;
    }

    public string Tag { get; set; }
    public string Name { get; set; }

    public List<FileInfo> Plugins { get; set; }

    public override string ToString()
    {
        return Name;
    }

    public string GetUniqueId() => $"{Tag},{Name}";
}
