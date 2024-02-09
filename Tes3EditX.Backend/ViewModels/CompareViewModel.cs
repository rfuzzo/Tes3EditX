using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tes3EditX.Backend.Services;
using TES3Lib;

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
            

            
        }

    }
}
