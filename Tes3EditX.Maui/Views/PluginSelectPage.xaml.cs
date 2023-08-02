using Tes3EditX.Backend.ViewModels;

namespace Tes3EditX.Maui.Views;

public partial class PluginSelectPage : ContentPage
{
	public PluginSelectPage(PluginSelectViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
	}

    private void SwitchCell_OnChanged(object sender, ToggledEventArgs e)
    {
        //if (sender is SwitchCell cell && cell.BindingContext is PluginItemViewModel plugin)
        //{
        //    plugin.Enabled = e.Value;
        //}
    }
}