using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Tes3EditX.Backend.Extensions;
using Tes3EditX.Backend.Models;
using Tes3EditX.Backend.Services;
using TES3Lib.Base;

namespace Tes3EditX.Backend.ViewModels;

public partial class ConflictsViewModel : ObservableRecipient
{
    private readonly INavigationService _navigationService;
    private readonly ICompareService _compareService;
    private readonly ISettingsService _settingsService;


    // Record Select View
    private readonly List<RecordItemViewModel> _records = new();

    [ObservableProperty]
    private ObservableCollection<GroupInfoList> _groupedRecords;

    [ObservableProperty]
    private object? _selectedRecord = null;

    public string FilterName { get; set; } = "";

    [ObservableProperty]
    private ObservableCollection<string> _tags;

    [ObservableProperty]
    private string _selectedTag = "";

    // Conflicts view

    [ObservableProperty]
    private ObservableCollection<ConflictRecordFieldViewModel> _fields = new();

    public ConflictsViewModel(
        INavigationService navigationService,
        ICompareService compareService,
        ISettingsService settingsService)
    {
        _navigationService = navigationService;
        _compareService = compareService;
        _settingsService = settingsService;

        // init
        GroupedRecords = new();
        Tags = new ObservableCollection<string>(Tes3Extensions.GetAllTags().Order());

        RegenerateRecords(_compareService.Conflicts);

        // Register a message in some module
        WeakReferenceMessenger.Default.Register<ConflictsChangedMessage>(this, (r, m) =>
        {
            // Handle the message here, with r being the recipient and m being the
            // input message. Using the recipient passed as input makes it so that
            // the lambda expression doesn't capture "this", improving performance.
            if (r is ConflictsViewModel vm)
            {
                vm.RegenerateRecords(m.Value);
            }
        });
    }

    public void RegenerateRecords(Dictionary<string, List<FileInfo>> conflicts)
    {
        _records.Clear();
        foreach ((var id, List<FileInfo> plugins) in conflicts)
        {
            var tag = id.Split(',').First();
            var name = id.Split(',').Last();
            _records.Add(new RecordItemViewModel(tag, name, plugins));
        }

        FilterRecords();
    }

    public void FilterRecords()
    {
        IEnumerable<GroupInfoList> query = _records
            .Where(x =>
            {
                if (!string.IsNullOrEmpty(SelectedTag) && SelectedTag != "_")
                {
                    return x.Tag.Equals(SelectedTag, StringComparison.CurrentCultureIgnoreCase);
                }
                else
                {
                    return true;
                }
            })
            .Where(x =>
            {
                if (!string.IsNullOrEmpty(FilterName))
                {
                    return x.Name.Contains(FilterName, StringComparison.CurrentCultureIgnoreCase);
                }
                else
                {
                    return true;
                }
            })
            .GroupBy(x => x.Tag)
            .OrderBy(x => x.Key)
            .Select(g => new GroupInfoList(g, g.Key));

        GroupedRecords = new ObservableCollection<GroupInfoList>(query);


    }

    // TODO refactor this shit

    /// <summary>
    /// Populate conflicts view when a record is selected
    /// </summary>
    /// <param name="value"></param>
    partial void OnSelectedRecordChanged(object? value)
    {
        if (value is not RecordItemViewModel recordViewModel)
        {
            return;
        }

        var recordId = recordViewModel.GetUniqueId();
        var names = _compareService.GetNames(recordViewModel.Tag);
        var conflicts = _compareService.GetConflictMap(recordViewModel.Plugins, recordId, names);

        // -----------------------------------------
        // loop again to get field equality
        CompareService.SetConflictStatus(conflicts);

        // -----------------------------------------
        // transform to vertical layout
        Fields.Clear();
        Fields.Add(new("Plugins", conflicts.Select(x => x.Item1).Cast<object>().ToList()));
        foreach (var name in names)
        {
            List<object> list = new();
            foreach (var c in conflicts)
            {
                var f = c.Item2.FirstOrDefault(x => x.Name.Equals(name));
                if (f is not null)
                {
                    list.Add(f);
                }
            }

            Fields.Add(new ConflictRecordFieldViewModel(name, list));
        }

    }

   


    partial void OnSelectedTagChanged(string value)
    {
        FilterRecords();
    }

    [RelayCommand]
    private void PerformSearch(string value)
    {
        FilterRecords();
    }

    

    

}
