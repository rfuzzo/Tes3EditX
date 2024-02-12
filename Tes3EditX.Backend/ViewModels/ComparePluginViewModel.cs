#define PARALLEL

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Tes3EditX.Backend.Extensions;
using Tes3EditX.Backend.Services;
using TES3Lib;
using TES3Lib.Base;

namespace Tes3EditX.Backend.ViewModels;

public partial class ComparePluginViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly INotificationService _notificationService;
    private readonly ICompareService _compareService;
    private readonly ISettingsService _settingsService;
    private readonly IFileApiService _folderPicker;

    public bool StartupLoaded { get; set; } = false;


    public List<PluginItemViewModel> PluginsList { get; set; } = new();

    [ObservableProperty]
    private List<PluginItemViewModel> _pluginsDisplay = new();

    public string PluginFilterText { get; set; } = "";

    //[ObservableProperty]
    //private List<PluginItemViewModel> _selectedPlugins = new();

    [ObservableProperty]
    private DirectoryInfo _folderPath;

    public ComparePluginViewModel(
        INavigationService navigationService,
        INotificationService notificationService,
        ICompareService compareService,
        ISettingsService settingsService,
        IFileApiService folderPicker)
    {
        _navigationService = navigationService;
        _notificationService = notificationService;
        _compareService = compareService;
        _settingsService = settingsService;
        _folderPicker = folderPicker;

        FolderPath = _settingsService.GetWorkingDirectory();


    }

    partial void OnFolderPathChanged(DirectoryInfo value)
    {
        _compareService.DataFiles = value;
    }

    [RelayCommand]
    public async Task InitPluginsAsync()
    {
        var stopwatch = Stopwatch.StartNew();

        var pluginPaths = FolderPath.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
            .Where(x =>
                x.Extension.Equals(".esp", StringComparison.OrdinalIgnoreCase) ||
                x.Extension.Equals(".esm", StringComparison.OrdinalIgnoreCase)).ToList();

        _notificationService.Progress = 0;
        _notificationService.Maximum = pluginPaths.Count;
        _notificationService.Enabled = false;

        var plugins = new ConcurrentBag<PluginItemViewModel>();

#if PARALLEL
        var progress = new Progress<int>(_ => _notificationService.Progress++) as IProgress<int>;

        await Task.Run(() =>
        {
            Parallel.ForEach(pluginPaths, (item, token) =>
            {
                var plugin = TES3.TES3Load(item.FullName);
                plugins.Add(new(item, plugin));
                progress.Report(0);
            });
        });
#else
         foreach (var item in pluginPaths)
         {
            var plugin = await Task.Run(() => TES3.TES3Load(item.FullName));
            plugins.Add(new(item, plugin));
            _notificationService.Progress++;
         }
#endif

        // sort by load order
        var final = plugins.OrderBy(x => x.Info.Extension.ToLower()).ThenBy(x => x.Info.LastWriteTime).ToList();
        PluginsList = new(final);
        Filter("");

        stopwatch.Stop();
        _notificationService.Text = stopwatch.Elapsed.TotalSeconds.ToString();
        _notificationService.Enabled = true;

        await Task.CompletedTask;
    }

    private static List<Type> GetTypesOfRecord(Record record, List<string> names)
    {
        Dictionary<string, (Subrecord subrecord, PropertyInfo propertyInfo)?> map = [];
        List<Type> fields = [];

        foreach (string name in names)
        {
            map.Add(name, null);
        }

        // get properties with reflection recursively
        foreach (Subrecord? subrecord in record.GetType().GetProperties(
               BindingFlags.Public |
               BindingFlags.Instance |
               BindingFlags.DeclaredOnly).Select(x => x.GetValue(record) as Subrecord).Where(x => x is not null))
        {
            if (subrecord is not null)
            {
                PropertyInfo[] properties = subrecord.GetType().GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.DeclaredOnly);

                foreach (PropertyInfo propertyInfo in properties)
                {
                    if (map.ContainsKey($"{subrecord.Name}.{propertyInfo.Name}"))
                    {
                        map[$"{subrecord.Name}.{propertyInfo.Name}"] = (subrecord, propertyInfo);
                    }
                    else if (map.ContainsKey(propertyInfo.Name))
                    {
                        map[propertyInfo.Name] = (subrecord, propertyInfo);
                    }
                }
            }
        }



        // flatten
        foreach ((string name, var val) in map)
        {
            if (val is not null)
            {
                object? field = val.Value.propertyInfo.GetValue(val.Value.subrecord);
                if (field is not null)
                {
                    fields.Add(field.GetType());
                }
            }
        }

        return fields;
    }

    [RelayCommand]
    private async Task SelectFolder()
    {
        var result = await _folderPicker.PickAsync(CancellationToken.None);

        if (!string.IsNullOrEmpty(result))
        {
            FolderPath = new DirectoryInfo(result);

            if (FolderPath.Exists)
            {
                await InitPluginsAsync();
            }
        }
    }

    public void Filter(string value)
    {
        // keep selection
        var selected = PluginsList.Where(x => x.Enabled).Select(x => x.Name).ToList();

        PluginsDisplay = PluginsList.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
        PluginsDisplay = PluginsDisplay.OrderBy(x => x.Info.Extension.ToLower()).ThenBy(x => x.Info.LastWriteTime).ToList();
        //Plugins.Sort((a,b) => a.Info.LastWriteTime.CompareTo(b.Info.LastWriteTime));

        foreach (PluginItemViewModel item in PluginsList)
        {
            if (selected.Contains(item.Name))
            {
                item.Enabled = true;
            }

        }
    }

    public void SelectionUpdated()
    {
        _compareService.Selectedplugins = PluginsList
            .Where(x => x.Enabled)
            .OrderBy(x => x.Info.Extension.ToLower())
            .ThenBy(x => x.Info.LastWriteTime);
    }
}
