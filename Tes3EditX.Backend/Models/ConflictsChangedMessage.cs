using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.Extensions;

namespace Tes3EditX.Backend.Models;

public class ConflictsChangedMessage(Dictionary<RecordId, List<FileInfo>> conflicts) 
    : ValueChangedMessage<Dictionary<RecordId, List<FileInfo>>>(conflicts)
{
}

public class FieldChangedMessage(string id) : ValueChangedMessage<string>(id)
{
}
