using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tes3EditX.Backend.Extensions;
using Tes3EditX.Backend.Services;
using TES3Lib.Base;
using TES3Lib.Subrecords.TES3;
using TES3 = TES3Lib.TES3;

namespace Tes3EditX.Backend.ViewModels;

public partial class CompareViewModel : ObservableObject
{
    public INotificationService NotificationService;

    private readonly ICompareService _compareService;
    private readonly INavigationService _navigationService;
    
    public ISettingsService SettingsService;

    public CompareViewModel(
        ISettingsService settingsService,
        INavigationService navigationService,
        INotificationService notificationService,
        ICompareService compareService)
    {
        NotificationService = notificationService;
        SettingsService = settingsService;
        _navigationService = navigationService;
        _compareService = compareService;
    }

    [ObservableProperty]
    private bool _isPaneOpen = true;


    [RelayCommand]
    private void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    [RelayCommand]
    private async Task Compare()
    {
        await _compareService.CalculateConflicts();
    }




    [RelayCommand]
    private async Task Save()
    {
        /*
         * save modes:
         * 1. save as patch: gather all dirty records and stick them into a new esp
         *      this is stupid when editing merged_objects.esp since this will mess up merging
         *      solution: hand-held mode where merged objects and esms are not editable
         * 2. overwrite: saves all edited plugins completely, and create a backup
        */

        // get dirty records
        if (_compareService.DirtyRecords.Count > 0)
        {
            if (SettingsService.OverwriteOnSave)
            {
                var keys = _compareService.DirtyRecords.Keys.OrderBy(x => x.Extension.ToLower())
                    .ThenBy(x => x.LastWriteTime);

                foreach (var path in keys)
                {
                    // get plugin
                    if (!_compareService.Plugins.TryGetValue(path, out var plugin))
                    {
                        continue;
                    }

                    //backup
                    if (await TryBackup(path))
                    {
                        // overwrite
                        try
                        {
                            plugin.TES3Save(path.FullName);

                            var name = Path.GetFileName(path.FullName);
                            await NotificationService.ShowMessageBox($"{name}", "Plugin saved");
                        }
                        catch (Exception e)
                        {
                            var name = Path.GetFileName(path.FullName);
                            await NotificationService.ShowMessageBox($"{name}\nError: {e}", "Could not save plugin");

                        }
                    }
                }
            }
            else
            {
                List<(MAST MAST, DATA DATA)> masters = new();
                var records = new Dictionary<RecordId, Record>();
                var keys = _compareService.DirtyRecords.OrderBy(x => x.Key.Extension.ToLower())
                    .ThenBy(x => x.Key.LastWriteTime);
                foreach (var (path, ids) in keys)
                {
                    // get plugin
                    if (!_compareService.Plugins.TryGetValue(path, out var plugin))
                    {
                        continue;
                    }

                    foreach (var id in ids)
                    {
                        // get record
                        var r = plugin.Records.FirstOrDefault(x => x.GetUniqueId() == id);
                        if (r != null)
                        {
                            records[id] = r;
                        }
                    }

                    // TODO
                    var size = 0;
                    (MAST MAST, DATA DATA) master = (
                        new MAST() { 
                            Filename = Path.GetFileName(path.FullName) 
                        }, 
                        new DATA() { 
                            MasterDataSize = size 
                        });
                    masters.Add(master);
                }

                // save
                if (_compareService.DataFiles is not null)
                {
                    var savePath = Path.Combine(_compareService.DataFiles.FullName, "tes3editx_patch.esp");
                    try
                    {
                        var plugin = new TES3();
                        // add header and masters
                        var header = new TES3Lib.Records.TES3
                        {
                            Masters = masters,
                            HEDR = new HEDR()
                            {
                                Version = 1.0f,
                                ESMFlag = 0,
                                CompanyName = "Tes3EditX",
                                Description = "https://github.com/rfuzzo/Tes3EditX",
                                NumRecords = plugin.Records.Count,
                            }
                        };
                        plugin.AddRecordThreadSafe(header);

                        // add records
                        foreach (var record in records.Values)
                        {
                            plugin.AddRecordThreadSafe(record);
                        }
              
                        plugin.TES3Save(savePath);

                        var name = Path.GetFileName(savePath);
                        await NotificationService.ShowMessageBox($"{name}", "Plugin saved");
                    }
                    catch (Exception e)
                    {
                        var name = Path.GetFileName(savePath);
                        await NotificationService.ShowMessageBox($"{name}\nError: {e}", "Could not save plugin");

                    }
                }

            }

        }

    }

    private async Task<bool> TryBackup(FileInfo path)
    {
        var backupPath = GetBackupPath(path);
        try
        {
            File.Copy(path.FullName, backupPath);
            return true;
        }
        catch (Exception e)
        {
            // logging
            var name = Path.GetFileName(path.FullName);
            await NotificationService.ShowMessageBox($"{name}\nError: {e}", "Could not back-up plugin");

            return false;
        }
    }

    private static string GetBackupPath(FileInfo path, int index = 0)
    {
        var backupPath = Path.ChangeExtension(path.FullName, $"{path.Extension}.{index}.bak");
        var newIndex = index + 1;
        return Path.Exists(backupPath) ? GetBackupPath(path, newIndex) : backupPath;
    }
}
