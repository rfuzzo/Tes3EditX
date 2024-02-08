using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Tes3EditX.Backend.ViewModels;

namespace Tes3EditX.Winui.Helpers
{
    public class StringOrFieldTemplateSelector : DataTemplateSelector
    {
        // Define the (currently empty) data templates to return
        // These will be "filled-in" in the XAML code.
        public DataTemplate? StringDataTemplate { get; set; }

        public DataTemplate? FieldDataTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item)
        {
            // Return the correct data template based on the item's type.
            if (item.GetType() == typeof(string))
            {
                return StringDataTemplate;
            }
            else
            {
                return item.GetType() == typeof(RecordFieldViewModel) ? FieldDataTemplate : null;
            }
        }
    }

    public class WrappedFieldTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? StringTemplate { get; set; }
        public DataTemplate? IntegerTemplate { get; set; }
        public DataTemplate? BooleanTemplate { get; set; }
        public DataTemplate? GenericTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object value)
        {
            return SelectInternal(value);
        }

        protected override DataTemplate? SelectTemplateCore(object value, DependencyObject container)
        {
            return SelectInternal(value);
        }

        private DataTemplate? SelectInternal(object? value)
        {
            if (value is string)
            {
                return StringTemplate;
            }
            else if (value is int)
            {
                return IntegerTemplate;
            }
            else if (value is bool)
            {
                return BooleanTemplate;
            }

            return GenericTemplate;
        }


    }
}
