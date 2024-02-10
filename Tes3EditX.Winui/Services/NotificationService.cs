using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Tes3EditX.Winui;

namespace Tes3EditX.Backend.Services
{
    public partial class NotificationService : ObservableObject, INotificationService
    {
        [ObservableProperty]
        private int _progress = 0;

        [ObservableProperty]
        private int _maximum = 0;

        [ObservableProperty]
        private string _text = "";

        [ObservableProperty]
        private bool _enabled;

        public async Task ShowMessageBox(
            string message,
            string? title = null,
            string closeButtonText = "Ok",
            string? primaryButtonText = null)
        {
            ContentDialog dlg = new()
            {
                XamlRoot = App.MainRoot.XamlRoot,
                Content = message,
                CloseButtonText = closeButtonText
            };
            if (primaryButtonText is not null)
            {
                dlg.PrimaryButtonText = primaryButtonText;
            }
            if (title is not null)
            {
                dlg.Title = title;
            }

            await dlg.ShowAsync();
        }
    }
}
