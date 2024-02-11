using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Services;

namespace Tes3EditX.Backend.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    public ISettingsService SettingsService;

    public SettingsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;

        CullConflicts = SettingsService.CullConflicts;
        MinConflicts = SettingsService.MinConflicts;

        Name = $"{SettingsService.GetName()} Version: {SettingsService.GetVersionString()}";
    }

    [ObservableProperty]
    private bool _cullConflicts;
    partial void OnCullConflictsChanged(bool value)
    {
        SettingsService.CullConflicts = value;
    }

    [ObservableProperty]
    private int _minConflicts;
    partial void OnMinConflictsChanged(int value)
    {
        SettingsService.MinConflicts = value;
    }

    public void SetTheme(string color)
    {
        SettingsService.Theme = color;
    }

    [ObservableProperty]
    private string _name;
}
