using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tes3EditX.Backend.Models
{
    public class ConflictsChangedMessage : ValueChangedMessage<Dictionary<string, List<FileInfo>>>
    {
        public ConflictsChangedMessage(Dictionary<string, List<FileInfo>> conflicts) : base(conflicts)
        {
            // 
        }
    }
}
