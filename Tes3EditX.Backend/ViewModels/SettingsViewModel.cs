﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Services;

namespace Tes3EditX.Backend.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        CullConflicts = _settingsService.CullConflicts;
        MinConflicts = _settingsService.MinConflicts;
    }

    [ObservableProperty]
    private bool _cullConflicts;
    partial void OnCullConflictsChanged(bool value)
    {
        _settingsService.CullConflicts = value;
    }

    [ObservableProperty]
    private int _minConflicts;
    partial void OnMinConflictsChanged(int value)
    {
        _settingsService.MinConflicts = value;
    }
}
