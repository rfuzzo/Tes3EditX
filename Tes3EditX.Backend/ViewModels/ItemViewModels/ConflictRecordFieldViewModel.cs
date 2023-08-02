using System.Collections.Generic;
using System.Reflection;
using TES3Lib.Base;
using TES3Lib.Interfaces;

namespace Tes3EditX.Backend.ViewModels;

/// <summary>
/// 
/// </summary>
public class ConflictRecordFieldViewModel
{
    public ConflictRecordFieldViewModel(string fieldName, List<object> fieldByRecords)
    {
        FieldName = fieldName;
        FieldByPlugins = fieldByRecords;
    }

    public string FieldName { get; }
    public List<object> FieldByPlugins { get; }
}
