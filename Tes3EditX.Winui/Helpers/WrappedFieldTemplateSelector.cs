using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Tes3EditX.Winui.Helpers;

public class WrappedFieldTemplateSelector : DataTemplateSelector
{
    public DataTemplate? StringTemplate { get; set; }
    public DataTemplate? IntegerTemplate { get; set; }
    public DataTemplate? FloatTemplate { get; set; }
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
        switch (value)
        {
            case string:
                return StringTemplate;
            case int:
                return IntegerTemplate;
            case bool:
                return BooleanTemplate;
            case float:
                return FloatTemplate;
            default:
                break;
        }

        return GenericTemplate;
    }


}
