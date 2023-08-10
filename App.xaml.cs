using Referidos.Views;
using Microsoft.Maui.Controls;

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
                // Si la clave está presente en la caché, mostrar el Shell para usuarios registrados
                MainPage = new AppShell();
            }
            else
            {
                // Si la clave no está presente en la caché, mostrar las páginas para usuarios no registrados
                MainPage = new NavigationPage(new HomePage());
            }
        }
    }
}
