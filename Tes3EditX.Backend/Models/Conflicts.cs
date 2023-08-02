using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.ViewModels;

namespace Tes3EditX.Backend.Models
{
    // GroupInfoList class definition:
    public class GroupInfoList : List<object>
    {
        public GroupInfoList(IEnumerable<object> items, string key) : base(items)
        {
            Key = key;
        }

        public string Key { get; set; }

        public override string ToString()
        {
            return $"Group {Key}";
        }
    }

}
