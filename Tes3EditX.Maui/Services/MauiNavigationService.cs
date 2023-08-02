using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Services;

namespace Tes3EditX.Maui.Services;

public class MauiNavigationService : INavigationService
{
    private readonly ISettingsService _settingsService;
    private readonly ICompareService _compareService;

    public MauiNavigationService(ISettingsService settingsService, ICompareService compareService)
    {
        _settingsService = settingsService;
        _compareService = compareService;
    }

    public Task InitializeAsync()
    {   
        // If any conflicts loaded, go to compare view
        return NavigateToAsync(_compareService.Conflicts is not null && _compareService.Conflicts.Any()
            ? "//Main/Main"
            : "//Main/Select");
    }

    public Task NavigateToAsync(string route, IDictionary<string, object>? routeParameters = null)
    {
        return
            routeParameters != null
                ? Shell.Current.GoToAsync(route, routeParameters)
                : Shell.Current.GoToAsync(route);
    }

    public Task PopAsync()
    {
        throw new NotImplementedException();
    }
}
