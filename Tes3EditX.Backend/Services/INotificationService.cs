using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tes3EditX.Backend.Services
{
    public interface INotificationService: INotifyPropertyChanged
    {
        public int Progress { get; set; }
        public int Maximum { get; set; }

        public string Text { get; set; }
        public bool Enabled { get; set; }

        Task ShowMessageBox(string message, string? title = null, string closeButtonText = "Ok", string? primaryButtonText = null);
    }
}
