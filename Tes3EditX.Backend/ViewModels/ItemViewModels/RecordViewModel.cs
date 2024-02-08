using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Extensions;

namespace Tes3EditX.Backend.ViewModels.ItemViewModels;

/// <summary>
/// This viewmodel wraps a TES3 record id
/// and is used to display records in a list
/// Tag is the record class
/// Name is the record id (not-unique!)
/// Plugins is a list of Plugins this record is found in
/// </summary>
public class RecordViewModel(string tag, string name, List<FileInfo> plugins)
{
    public string Tag { get; set; } = tag;
    public string Name { get; set; } = name;

    public List<FileInfo> Plugins { get; set; } = plugins;

    public override string ToString()
    {
        return Name;
    }

    public RecordId GetUniqueId() => new(Tag, Name);
}
