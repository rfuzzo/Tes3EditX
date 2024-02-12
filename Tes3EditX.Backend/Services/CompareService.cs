#define PARALLEL

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Tes3EditX.Backend.Extensions;
using Tes3EditX.Backend.Models;
using Tes3EditX.Backend.ViewModels;
using TES3Lib;
using TES3Lib.Base;
using TES3Lib.Records;
using TES3Lib.Subrecords.Shared;
using TES3 = TES3Lib.TES3;

namespace Tes3EditX.Backend.Services;

public partial class CompareService(INotificationService notificationService, ISettingsService settingsService) 
    : ObservableObject, ICompareService
{
    public Dictionary<FileInfo, TES3> Plugins { get; } = [];

    [ObservableProperty]
    private Dictionary<RecordId, List<FileInfo>> _conflicts = [];

    private readonly INotificationService _notificationService = notificationService;
    private readonly ISettingsService _settingsService = settingsService;

    public IEnumerable<PluginItemViewModel> Selectedplugins { get; set; } = new List<PluginItemViewModel>();
    public RecordId? CurrentRecordId { get; set; }
    public Dictionary<FileInfo, List<RecordId>> DirtyRecords { get; set; } = [];
    public DirectoryInfo? DataFiles { get; set ; }


    // todo get load order right
    // todo use hashes
    public async Task CalculateConflicts()
    {
        Conflicts.Clear();
        Plugins.Clear();
        DirtyRecords.Clear();

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
        List<Record> records = [];
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
        Record? record = Plugins.Values.SelectMany(x => x.Records).FirstOrDefault(x => x is not null && x.Name.Equals(tag));
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
        List<(string, List<RecordFieldViewModel>)> conflictsMap = [];

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
                    bool isReadonly = !_settingsService.OverwriteOnSave && pluginPath.Extension.Equals(".esm", StringComparison.InvariantCultureIgnoreCase);

                    var fields = GetFieldsOfRecord(pluginPath, record, names, isReadonly);
                    conflictsMap.Add((pluginPath.Name, fields));
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
    private static List<RecordFieldViewModel> GetFieldsOfRecord(FileInfo pluginPath, Record record, List<string> names, bool isReadonly)
    {
        Dictionary<string, (Subrecord subrecord, PropertyInfo propertyInfo)?> map = [];
        List<RecordFieldViewModel> fields = [];

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
                var isReadonlyForce = isReadonly;
                if (name == "EditorId")
                {
                    isReadonlyForce = true;
                }
                fields.Add(new(pluginPath, val.Value.subrecord, val.Value.propertyInfo, field, name, isReadonlyForce));
            }

        }

        

        return fields;
    }

    public static bool Tes3Equals(object value1, object value2)
    {
        if (value1 is string a && value2 is string b)
        {
            return a.Trim('\0').Equals(b.Trim('\0'));
        }
        else if (value1 is IEnumerable ea && value2 is IEnumerable eb)
        {
            var la = ea.Cast<object>();
            var lb = eb.Cast<object>();
            return Enumerable.SequenceEqual(la, lb);
        }
        else
        {
            return value1.Equals(value2);
        }
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
                if (j >= c_last.Count)
                {
                    f.IsConflict = true;
                    anyConflict = true;
                }
                else
                {
                    RecordFieldViewModel f_last = c_last[j];
                    if (f_last.WrappedField is not null && f.WrappedField is not null)
                    {
                        if (!Tes3Equals(f_last.WrappedField, f.WrappedField))
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
