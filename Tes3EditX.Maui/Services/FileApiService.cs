using CommunityToolkit.Maui.Storage;
using Tes3EditX.Backend.Services;
using Tes3EditX.Maui.Services;

namespace Tes3EditX.Maui.Services;

public class FileApiService : IFileApiService
{
    private readonly IFolderPicker _folderPicker;

    public FileApiService(IFolderPicker folderPicker)
    {
        _folderPicker = folderPicker;
    }

    public async Task<string> PickAsync(CancellationToken none)
    {
        var result = await _folderPicker.PickAsync(CancellationToken.None);
        result.EnsureSuccess();

        return result.Folder.Path;
    }
}
