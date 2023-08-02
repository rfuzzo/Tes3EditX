using Tes3EditX.Backend.Services;

namespace Tes3EditX.Maui
{
    public partial class App : Application
    {
        public App(INavigationService navigationService)
        {
            InitializeComponent();

            MainPage = new AppShell(navigationService);
        }
    }
}