using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Extensions;

namespace Tes3EditX.Backend.Models
{
    public class ConflictsChangedMessage : ValueChangedMessage<Dictionary<RecordId, List<FileInfo>>>
    {
        public ConflictsChangedMessage(Dictionary<RecordId, List<FileInfo>> conflicts) : base(conflicts)
        {
            // 
        }
    }
    public class FieldChangedMessage : ValueChangedMessage<string>
    {
        public FieldChangedMessage(string id) : base(id)
        {
            // 
        }
    }
}
