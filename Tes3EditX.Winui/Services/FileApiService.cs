using AppUIBasics.Helper;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tes3EditX.Backend.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Tes3EditX.Winui.Services;

public class FileApiService : IFileApiService
{



    public async Task<string> PickAsync(CancellationToken none)
    {
        // Create a folder picker
        FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();


        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        //var window = WindowHelper.GetWindowForElement(this);
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.StartupWindow);

        // Initialize the folder picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your folder picker
        openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
        openPicker.FileTypeFilter.Add("*");

        // Open the picker for the user to pick a folder
        StorageFolder folder = await openPicker.PickSingleFolderAsync();

        if (folder != null)
        {
            return folder.Path;
        }
        else
        { 
            return ""; 
        }
            
    }
}
