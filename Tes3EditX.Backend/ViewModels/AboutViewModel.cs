using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Tes3EditX.Backend.Services;

namespace Tes3EditX.Backend.ViewModels;

public class AboutViewModel
{
    private readonly ISettingsService _settingsService;

    public string Title => _settingsService.Name;
    public string Version => _settingsService.VersionString;
    public string MoreInfoUrl => "https://aka.ms/maui";
    public string Message => "This app is written in XAML and C# with .NET MAUI.";
    public ICommand ShowMoreInfoCommand { get; }

    public AboutViewModel(ISettingsService settingsService)
    {
        ShowMoreInfoCommand = new AsyncRelayCommand(ShowMoreInfo);
        _settingsService = settingsService;
    }

    async Task ShowMoreInfo()
    {
        await Task.CompletedTask;
        // TODO
        //await Launcher.Default.OpenAsync(MoreInfoUrl);
    }
}