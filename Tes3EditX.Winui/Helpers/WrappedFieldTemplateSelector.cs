using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tes3EditX.Winui.Helpers;

public class WrappedFieldTemplateSelector : DataTemplateSelector
{
    public DataTemplate? StringTemplate { get; set; }

    public DataTemplate? ByteTemplate { get; set; }
    public DataTemplate? ShortTemplate { get; set; }
    public DataTemplate? IntegerTemplate { get; set; }
    public DataTemplate? FloatTemplate { get; set; }
    
    public DataTemplate? BooleanTemplate { get; set; }
    public DataTemplate? EnumTemplate { get; set; }
    
    public DataTemplate? ListTemplate { get; set; }
    public DataTemplate? FlagsTemplate { get; set; }
    
    
    
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
            case byte:
                return ByteTemplate;
            case short:
                return ShortTemplate;
            case int:
                return IntegerTemplate;
            case bool:
                return BooleanTemplate;
            case float:
                return FloatTemplate;
            case byte[]:
                return GenericTemplate; // TODO
            case Array array:
                {
                    // check if matrix
                    if (array.Rank > 1)
                    {
                        // TODO
                        return GenericTemplate;
                    } else
                    {
                        return ListTemplate;
                    }
                }
            case IEnumerable:
                return FlagsTemplate;
            case Enum:
                return EnumTemplate;
            default:
                {
                    if (value is not null)
                    {
                        // TODO logging
                    }
                    break;
                }
                
        }

        return GenericTemplate;
    }


}
