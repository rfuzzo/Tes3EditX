using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Tes3EditX.Backend.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tes3EditX.Winui.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ConflictsPage : Page
{
    public ConflictsPage()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<ConflictsViewModel>();

    }

    public ConflictsViewModel ViewModel => (ConflictsViewModel)DataContext;

    // Whenever text changes in any of the filtering text boxes, the following function is called:
    private void OnFilterChanged(object sender, TextChangedEventArgs args)
    {
        if (sender is TextBox textBox)
        {
            ViewModel.FilterName = textBox.Text;
            ViewModel.FilterRecords();
        }
       
    }

}
