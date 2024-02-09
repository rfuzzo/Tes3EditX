using CommunityToolkit.Mvvm.ComponentModel;

namespace Tes3EditX.Backend.ViewModels.ItemViewModels;

/// <summary>
/// 
/// </summary>
public partial class ConflictRecordFieldViewModel : ObservableObject
{
    public ConflictRecordFieldViewModel(string fieldName, List<object> fieldByPlugins, bool hasConflict)
    {
        FieldName = fieldName;
        FieldByPlugins = fieldByPlugins;
        HasConflict = hasConflict;
    }

    [ObservableProperty]
    private string _fieldName;

    [ObservableProperty]
    private bool _hasConflict;

    public List<object> FieldByPlugins { get; }

    
}
