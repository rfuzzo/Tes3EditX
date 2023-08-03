#define PARALLEL

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Extensions;
using Tes3EditX.Backend.Models;
using Tes3EditX.Backend.ViewModels;
using TES3Lib;
using TES3Lib.Base;
using TES3Lib.Subrecords.Shared;

namespace Tes3EditX.Backend.Services;

public partial class CompareService : ObservableObject, ICompareService
{
    public Dictionary<FileInfo,TES3> Plugins { get; } = new();

    [ObservableProperty]
    private Dictionary<string, List<FileInfo>> _conflicts = new();

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

        var pluginMap = new Dictionary<FileInfo, HashSet<string>>();
        foreach (var model in Selectedplugins)
        {
            var plugin = model.Plugin;
            var records = plugin.Records
                .Where(x => x is not null)
                .Select(x => x.GetUniqueId())
                .ToHashSet();

            
            pluginMap.Add(model.Info, records);
            Plugins.Add(model.Info, plugin);
        }

        // map of record ids and according plugin paths
        Dictionary<string, List<FileInfo>> conflict_map = new();
        foreach (var (pluginKey, records) in pluginMap)
        {
            List<string> newrecords = new();
            foreach (var record in records)
            {
                // then we have a conflict
                if (conflict_map.ContainsKey(record))
                {
                    conflict_map[record].Add(pluginKey);
                }
                // no conflict, store for later adding
                else
                {
                    newrecords.Add(record);
                }
            }

            foreach (var item in newrecords)
            {
                conflict_map.Add(item, new() { pluginKey });
            }
        }

        // TODO dedup?
        // remove single entries (no conflicts)
        // a true conflict is only at > 2 conflicting entries
        var singleRecords = conflict_map.Where(x => x.Value.Count < _settingsService.MinConflicts).Select(x => x.Key);
        foreach (var item in singleRecords)
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

            var toRemove = new List<string>();

            var stopwatch = Stopwatch.StartNew();

#if PARALLEL
            var progress = new Progress<int>(_ => _notificationService.Progress++) as IProgress<int>;

            await Parallel.ForEachAsync(conflict_map, async (item, token) =>
            {
                //await Task.Run(() => CheckForConflict(toRemove, item.Key, item.Value));
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

            foreach (var item in toRemove)
            {
                conflict_map.Remove(item);
            }

        }

        Conflicts = conflict_map;

        await Task.CompletedTask;
    }

    public bool HasAnyConflict(string key, List<FileInfo> plugins)
    {
        var tag = key.Split(',').First();
        var records = new List<Record>();
        foreach (var pluginPath in plugins)
        {
            // get plugin
            if (Plugins.TryGetValue(pluginPath, out var plugin))
            {
                // get record
                var record = plugin.Records.FirstOrDefault(x => x is not null && x.GetUniqueId() == key);
                if (record is not null)
                {
                    records.Add(record);
                }
            }
        }

        // check for equality
        var isConflict = false;
        for (int i = 0; i < records.Count; i++)
        {
            var r = records[i];
            for (int j = i + 1; j < records.Count; j++)
            {
                var r2 = records[j];
                if (!r.DeepEquals(r2))
                {
                    isConflict = true;
                    break;
                }
            }
        }

        return isConflict;
    }

    partial void OnConflictsChanged(Dictionary<string, List<FileInfo>> value)
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
        var record = Plugins.Values.SelectMany(x => x.Records).FirstOrDefault(x => x.Name.Equals(tag));
        if (record is not null)
        {
            var instance = (Record?)Activator.CreateInstance(record.GetType()!);
            return instance!.GetPropertyNames();
        }

        // should never happen
        throw new ArgumentException();
    }

    public List<(string, List<RecordFieldViewModel>)> GetConflictMap(List<FileInfo> plugins, string recordId, List<string> names)
    {
        // fields by plugin
        var conflictsMap = new List<(string, List<RecordFieldViewModel>)>();

        // loop through plugins to get a vm with fields for each plugin
        foreach (var pluginPath in plugins)
        {
            // get plugin
            if (Plugins.TryGetValue(pluginPath, out var plugin))
            {
                // get record
                var record = plugin.Records.FirstOrDefault(x => x is not null && x.GetUniqueId() == recordId);
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
        Dictionary<string, object?> map = new();
        List<RecordFieldViewModel> fields = new();

        foreach (var name in names)
        {
            map.Add(name, null);
        }

        // get properties with reflection recursively
        var recordProperties = record.GetType().GetProperties(
               BindingFlags.Public |
               BindingFlags.Instance |
               BindingFlags.DeclaredOnly).ToList();
        foreach (PropertyInfo prop in recordProperties)
        {
            var v = prop.GetValue(record);

            if (v is Subrecord subrecord)
            {
                var subRecordProperties = subrecord.GetType().GetProperties(
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
        foreach (var (name, field) in map)
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
        var anyConflict = false;

        for (var i = 1; i < conflicts.Count; i++)
        {
            var c = conflicts[i].Item2;
            var c_last = conflicts[i - 1].Item2;

            for (var j = 0; j < c.Count; j++)
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
