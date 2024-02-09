using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using Tes3EditX.Backend.Extensions;
using Tes3EditX.Backend.Models;
using Tes3EditX.Backend.Services;
using Tes3EditX.Backend.ViewModels.ItemViewModels;
using TES3Lib.Base;
using TES3Lib.Records;

namespace Tes3EditX.Backend.ViewModels;

public partial class ConflictsViewModel : ObservableRecipient
{
    private readonly INavigationService _navigationService;
    private readonly ICompareService _compareService;
    private readonly ISettingsService _settingsService;


    // Record Select View
    private readonly List<RecordViewModel> _records = [];

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
    private ObservableCollection<ConflictRecordFieldViewModel> _fields = [];

    public ConflictsViewModel(
        INavigationService navigationService,
        ICompareService compareService,
        ISettingsService settingsService)
    {
        _navigationService = navigationService;
        _compareService = compareService;
        _settingsService = settingsService;

        // init
        GroupedRecords = [];
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
        WeakReferenceMessenger.Default.Register<FieldChangedMessage>(this, (r, m) =>
        {
            // Handle the message here, with r being the recipient and m being the
            // input message. Using the recipient passed as input makes it so that
            // the lambda expression doesn't capture "this", improving performance.
            if (r is ConflictsViewModel vm)
            {
                vm.UpdateStatusForField(m.Value);
            }
        });
    }

    public void UpdateStatusForField(string id)
    {
        foreach (ConflictRecordFieldViewModel? vm in Fields.Where(x => x.FieldName == id))
        {
            // check and set conflicts
            bool anyConflict = false;
            List<RecordFieldViewModel> items = vm.FieldByPlugins.Where(x => x is RecordFieldViewModel).Cast<RecordFieldViewModel>().ToList();

            for (int i = 1; i < items.Count; i++)
            {
                RecordFieldViewModel f = items[i];
                RecordFieldViewModel f_last = items[i - 1];

                if (f_last.WrappedField is not null && f.WrappedField is not null)
                {
                    if (!CompareService.Tes3Equals(f_last.WrappedField, f.WrappedField))
                    {
                        f.IsConflict = true;
                        anyConflict = true;
                    }
                }
                else if (f_last.WrappedField is null && f.WrappedField is null)
                {
                    // do nothing
                }
                else
                {
                    f.IsConflict = true;
                    anyConflict = true;
                }

            }

            // update vm
            vm.HasConflict = anyConflict;
        }
    }

    public void RegenerateRecords(Dictionary<RecordId, List<FileInfo>> conflicts)
    {
        List<string> tags = new() { "_" };
        _records.Clear();
        foreach ((RecordId? id, List<FileInfo> plugins) in conflicts)
        {
            // add record item vm
            _records.Add(new RecordViewModel(id.Tag, id.EditorId, plugins));

            // add tag
            if (!tags.Contains(id.Tag))
            {
                tags.Add(id.Tag);
            }
        }

        Tags = new(tags);

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

    /// <summary>
    /// Populate conflicts view when a record is selected
    /// </summary>
    /// <param name="value"></param>
    partial void OnSelectedRecordChanged(object? value)
    {
        if (value is not RecordViewModel recordItemViewModel)
        {
            return;
        }

        var recordId = recordItemViewModel.GetUniqueId();
        var names = _compareService.GetNames(recordItemViewModel.Tag);
        var conflicts = _compareService.GetConflictMap(recordItemViewModel.Plugins, recordId, names);

        // loop again to get field equality
        CompareService.SetConflictStatus(conflicts);

        // transform to vertical layout
        Fields.Clear();
        // header is just the plugin names
        Fields.Add(new ConflictRecordFieldViewModel("Plugins", conflicts.Select(x => x.Item1).Cast<object>().ToList(), false));

        // add the record fields
        foreach (var name in names)
        {
            var hasConflict = false;
            List<object> list = new();
            foreach (var (_, recordFieldViewModels) in conflicts)
            {
                var recordFieldViewModel = recordFieldViewModels.FirstOrDefault(x => x.Name.Equals(name));
                if (recordFieldViewModel is not null)
                {
                    list.Add(recordFieldViewModel);
                    hasConflict = hasConflict | recordFieldViewModel.IsConflict;
                }
            }

            Fields.Add(new ConflictRecordFieldViewModel(name, list, hasConflict));
        }

        // TODO notify parent?
        _compareService.CurrentRecordId = recordId;
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
