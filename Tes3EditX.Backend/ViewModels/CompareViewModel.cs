using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tes3EditX.Backend.Services;

namespace Tes3EditX.Backend.ViewModels;

public partial class CompareViewModel : ObservableObject
{
    public NotificationService? NotificationService;
    private readonly ICompareService _compareService;
    public CompareViewModel(INotificationService notificationService, ICompareService compareService)
    {
        NotificationService = notificationService as NotificationService;
        _compareService = compareService;
    }

    [ObservableProperty]
    private bool _isPaneOpen = true;

    [RelayCommand]
    private void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    [RelayCommand]
    private void Save()
    {
        // get dirty records
        if (_compareService.DirtyRecords.Count > 0)
        {
            foreach (var path in _compareService.DirtyRecords.Keys)
            {
                // get plugin
                if (_compareService.Plugins.TryGetValue(path, out var plugin))
                {
                    var ext = path.Extension.ToLower();
                    var newPath = Path.ChangeExtension(path.FullName, $".patch{ext}");
                    plugin.TES3Save(newPath);
                }
            }


        }

    }
}
