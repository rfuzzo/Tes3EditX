#define PARALLEL

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using System.Reflection;
using Tes3EditX.Backend.Extensions;
using Tes3EditX.Backend.Models;
using Tes3EditX.Backend.ViewModels;
using TES3Lib;
using TES3Lib.Base;

namespace Tes3EditX.Backend.Services;

public partial class CompareService : ObservableObject, ICompareService
{
    public Dictionary<FileInfo, TES3> Plugins { get; } = [];

    [ObservableProperty]
    private Dictionary<RecordId, List<FileInfo>> _conflicts = [];

    private readonly INotificationService _notificationService;
    private readonly ISettingsService _settingsService;

    public IEnumerable<PluginItemViewModel> Selectedplugins { get; set; } = new List<PluginItemViewModel>();

    public CompareService(INotificationService notificationService, ISettingsService settingsService)
    {
        _notificationService = notificationService;
        _settingsService = settingsService;
    }

    // todo get load order right
    // todo use hashes
    public async Task CalculateConflicts()
    {
        if (Selectedplugins is null)
        {
            return;
        }

        Conflicts.Clear();
        // TODO optimize load
        Plugins.Clear();

        // map plugin records

        Dictionary<FileInfo, HashSet<RecordId>> pluginMap = [];
        foreach (PluginItemViewModel model in Selectedplugins)
        {
            TES3 plugin = model.Plugin;
            HashSet<RecordId> records = plugin.Records
                .Where(x => x is not null)
                .Select(x => x.GetUniqueId())
                .ToHashSet();


            pluginMap.Add(model.Info, records);
            Plugins.Add(model.Info, plugin);
        }

        // map of record ids and according plugin paths
        Dictionary<RecordId, List<FileInfo>> conflict_map = [];
        foreach ((FileInfo pluginKey, HashSet<RecordId> records) in pluginMap)
        {
            List<RecordId> newrecords = [];
            foreach (RecordId record in records)
            {
                // then we have a conflict
                if (conflict_map.TryGetValue(record, out List<FileInfo>? value))
                {
                    value.Add(pluginKey);
                }
                // no conflict, store for later adding
                else
                {
                    newrecords.Add(record);
                }
            }

            foreach (RecordId item in newrecords)
            {
                conflict_map.Add(item, [pluginKey]);
            }
        }

        // TODO dedup?
        // remove single entries (no conflicts)
        // a true conflict is only at > 2 conflicting entries
        IEnumerable<RecordId> singleRecords = conflict_map.Where(x => x.Value.Count < _settingsService.MinConflicts).Select(x => x.Key);
        foreach (RecordId? item in singleRecords)
        {
            conflict_map.Remove(item);
        }

        // TODO cache field names
        _notificationService.Progress = 0;
        _notificationService.Maximum = conflict_map.Count;
        _notificationService.Enabled = false;

        // check for false positives
        // TODO why is that necessary?
        if (_settingsService.CullConflicts)
        {

            List<RecordId> toRemove = [];

            Stopwatch stopwatch = Stopwatch.StartNew();

#if PARALLEL
            IProgress<int> progress = new Progress<int>(_ => _notificationService.Progress++);

            await Parallel.ForEachAsync(conflict_map, async (item, token) =>
            {
                if (!HasAnyConflict(item.Key, item.Value))
                {
                    toRemove.Add(item.Key);
                }
                progress.Report(0);

                await Task.CompletedTask;
            });
#else
         foreach (var item in conflict_map)
         {
                if (!HasAnyConflict(item.Key, item.Value))
                {
                    toRemove.Add(item.Key);
                }
                _notificationService.Progress++;
         }
#endif

            stopwatch.Stop();
            _notificationService.Text = stopwatch.Elapsed.TotalSeconds.ToString();
            _notificationService.Enabled = true;

            foreach (RecordId item in toRemove)
            {
                conflict_map.Remove(item);
            }

        }

        Conflicts = conflict_map;

#if !PARALLEL
        await Task.CompletedTask;
#endif

    }

    public bool HasAnyConflict(RecordId key, List<FileInfo> plugins)
    {
        string tag = key.Tag;
        List<Record> records = new();
        foreach (FileInfo pluginPath in plugins)
        {
            // get plugin
            if (Plugins.TryGetValue(pluginPath, out TES3? plugin))
            {
                // get record
                Record? record = plugin.Records.FirstOrDefault(x => x is not null && x.GetUniqueId() == key);
                if (record is not null)
                {
                    records.Add(record);
                }
            }
        }

        // check for equality
        bool isConflict = false;
        for (int i = 0; i < records.Count; i++)
        {
            Record r = records[i];
            for (int j = i + 1; j < records.Count; j++)
            {
                Record r2 = records[j];
                if (!r.DeepEquals(r2))
                {
                    isConflict = true;
                    break;
                }
            }
        }

        return isConflict;
    }

    partial void OnConflictsChanged(Dictionary<RecordId, List<FileInfo>> value)
    {
        WeakReferenceMessenger.Default.Send(new ConflictsChangedMessage(value));
    }

    /// <summary>
    ///  Get names of all fields for record
    /// </summary>
    /// <param name="compareService"></param>
    /// <param name="recordViewModel"></param>
    /// <param name="recordId"></param>
    /// <returns></returns>
    public List<string> GetNames(string tag)
    {
        // get the first record object that matches the tag
        Record? record = Plugins.Values.SelectMany(x => x.Records).FirstOrDefault(x => x.Name.Equals(tag));
        if (record is not null)
        {
            Record? instance = (Record?)Activator.CreateInstance(record.GetType()!);
            return instance!.GetPropertyNames();
        }

        // should never happen
        throw new ArgumentException();
    }

    public List<(string, List<RecordFieldViewModel>)> GetConflictMap(List<FileInfo> plugins, RecordId recordId, List<string> names)
    {
        // fields by plugin
        List<(string, List<RecordFieldViewModel>)> conflictsMap = new();

        // loop through plugins to get a vm with fields for each plugin
        foreach (FileInfo pluginPath in plugins)
        {
            // get plugin
            if (Plugins.TryGetValue(pluginPath, out TES3? plugin))
            {
                // get record
                Record? record = plugin.Records.FirstOrDefault(x => x is not null && x.GetUniqueId() == recordId);
                if (record is not null)
                {
                    conflictsMap.Add((pluginPath.Name, GetFieldsOfRecord(record, names)));
                }
            }
        }

        return conflictsMap;
    }

    /// <summary>
    /// Get all fields of record
    /// </summary>
    /// <param name="record"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    private static List<RecordFieldViewModel> GetFieldsOfRecord(Record record, List<string> names)
    {
        Dictionary<string, object?> map = [];
        List<RecordFieldViewModel> fields = [];

        foreach (string name in names)
        {
            map.Add(name, null);
        }

        // get properties with reflection recursively
        List<PropertyInfo> recordProperties = record.GetType().GetProperties(
               BindingFlags.Public |
               BindingFlags.Instance |
               BindingFlags.DeclaredOnly).ToList();
        foreach (PropertyInfo prop in recordProperties)
        {
            object? v = prop.GetValue(record);

            if (v is Subrecord subrecord)
            {
                List<PropertyInfo> subRecordProperties = subrecord.GetType().GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.DeclaredOnly).ToList();
                foreach (PropertyInfo subProp in subRecordProperties)
                {
                    if (map.ContainsKey($"{subrecord.Name}.{subProp.Name}"))
                    {
                        map[$"{subrecord.Name}.{subProp.Name}"] = subProp.GetValue(subrecord);
                    }
                    else if (map.ContainsKey(subProp.Name))
                    {
                        map[subProp.Name] = subProp.GetValue(subrecord);
                    }
                }
            }
        }

        // fill fields
        // todo to refactor
        foreach ((string name, object? field) in map)
        {
            fields.Add(new(field, name));
        }

        return fields;
    }

    /// <summary>
    /// Loops through the conflict map and sets conflict status
    /// </summary>
    /// <param name="conflicts"></param>
    public static bool SetConflictStatus(List<(string, List<RecordFieldViewModel>)> conflicts)
    {
        bool anyConflict = false;

        for (int i = 1; i < conflicts.Count; i++)
        {
            List<RecordFieldViewModel> c = conflicts[i].Item2;
            List<RecordFieldViewModel> c_last = conflicts[i - 1].Item2;

            for (int j = 0; j < c.Count; j++)
            {
                RecordFieldViewModel f = c[j];
                if (j > c_last.Count)
                {
                    f.IsConflict = true;
                    anyConflict = true;
                }
                else
                {
                    RecordFieldViewModel f_last = c_last[j];
                    if (f_last.WrappedField is not null && f.WrappedField is not null)
                    {
                        if (!f_last.WrappedField.Equals(f.WrappedField))
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
            }
        }

        return anyConflict;
    }
}
