using Referidos.Views;

namespace Referidos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string claveGuardada = Preferences.Get("ClaveCache", string.Empty);

            if (!string.IsNullOrEmpty(claveGuardada))
            {
                // Si la clave está presente en la caché, navegar a la PrincipalPage dentro del Shell
                MainPage = new AppShell();
                MainPage.Navigation.PushAsync(new PrincipalPage());
            }
            else
            {
                // Si la clave no está presente en la caché, navegar a la HomePage dentro del Shell
                MainPage = new AppShell();
                MainPage.Navigation.PushAsync(new HomePage());
            }
        }
    }
}
