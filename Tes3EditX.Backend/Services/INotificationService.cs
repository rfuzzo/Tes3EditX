using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tes3EditX.Backend.Services
{
    public interface INotificationService
    {
        public int Progress { get; set; }
        public int Maximum { get; set; }

        public string Text { get; set; }
        public bool Enabled { get; set; }
    }

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
    }
}
