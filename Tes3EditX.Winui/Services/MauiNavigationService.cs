using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Services;
using Tes3EditX.Winui.Pages;

namespace Tes3EditX.Winui.Services;

public class NavigationService : INavigationService
{
    private readonly ISettingsService _settingsService;
    private readonly ICompareService _compareService;

    public NavigationService(ISettingsService settingsService, ICompareService compareService)
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

    public async Task NavigateToAsync(string route, IDictionary<string, object>? routeParameters = null)
    {
        if (string.IsNullOrEmpty(route))
        {
            await Task.CompletedTask;
            return;
        }
        else
        {
            return;
            //throw new NotImplementedException();
            //(App.StartupWindow as MainWindow).Navigate(typeof(ComparePage), "");
        }
    }

    public Task PopAsync()
    {
        throw new NotImplementedException();
    }
}
