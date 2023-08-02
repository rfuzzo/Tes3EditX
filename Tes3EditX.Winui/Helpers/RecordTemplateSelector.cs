using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tes3EditX.Backend.ViewModels;
using TES3Lib.Interfaces;
using TES3Lib.Subrecords.Shared;
using Utility;

namespace Tes3EditX.Winui.Helpers
{
    public class StringOrFieldTemplateSelector : DataTemplateSelector
    {
        // Define the (currently empty) data templates to return
        // These will be "filled-in" in the XAML code.
        public DataTemplate StringTemplate { get; set; }

        public DataTemplate FieldDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            // Return the correct data template based on the item's type.
            if (item.GetType() == typeof(string))
            {
                return StringTemplate;
            }
            else if (item.GetType() == typeof(RecordFieldViewModel))
            {
                return FieldDataTemplate;
            }
            else
            {
                return null;
            }
        }
    }

    public class RecordViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Common { get; set; }

        protected override DataTemplate SelectTemplateCore(object value)
        {
            return Common;
        }
    }
}
