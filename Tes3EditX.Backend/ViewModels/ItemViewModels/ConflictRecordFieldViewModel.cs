using CommunityToolkit.Mvvm.ComponentModel;

namespace Tes3EditX.Backend.ViewModels.ItemViewModels;

/// <summary>
/// 
/// </summary>
public class ConflictRecordFieldViewModel(string fieldName, List<object> fieldByRecords, bool hasConflict) : ObservableObject
{
    public string FieldName { get; } = fieldName;
    public List<object> FieldByPlugins { get; } = fieldByRecords;
    public bool HasConflict { get; } = hasConflict;
}
