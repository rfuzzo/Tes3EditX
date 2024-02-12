using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Tes3EditX.Backend.Services;
using Tes3EditX.Backend.ViewModels;

namespace Tes3EditX.Winui.Helpers;

public class StringOrFieldTemplateSelector : DataTemplateSelector
{
    private readonly ISettingsService _settingsService;

    public StringOrFieldTemplateSelector()
    {
        _settingsService = App.Current.Services.GetRequiredService<ISettingsService>();
    }


    // Define the (currently empty) data templates to return
    // These will be "filled-in" in the XAML code.
    public DataTemplate? StringDataTemplate { get; set; }

    public DataTemplate? FieldDataTemplate { get; set; }
    public DataTemplate? ReadonlyFieldDataTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item)
    {
        // Return the correct data template based on the item's type.
        if (item.GetType() == typeof(string))
        {
            return StringDataTemplate;
        }
        else
        {
            if (item.GetType() == typeof(RecordFieldViewModel))
            {                    
                return _settingsService.Readonly ? ReadonlyFieldDataTemplate : FieldDataTemplate;
            }
            else
            {
                return null;
            }
        }
    }
}
