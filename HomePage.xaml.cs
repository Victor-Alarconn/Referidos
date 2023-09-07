using Referidos.ViewsModels;
using static Referidos.Data.DataConexion;

namespace Referidos
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            // Asignar el ViewModel como el contexto de enlace (BindingContext) de la vista
            BindingContext = new HomePageViewModel();
        }

        async void OnImageTapped(object sender, EventArgs args)
        {
            var seleccion = await DisplayActionSheet("Selecciona una conexión", "Cancelar", null, ConfiguracionBD.conexiones.Keys.ToArray());

            if (ConfiguracionBD.conexiones.ContainsKey(seleccion))
            {
                ConfiguracionBD.ConexionActual = ConfiguracionBD.conexiones[seleccion];
                await DisplayAlert("Conexión", $"Has cambiado a la conexión: {seleccion}", "OK");
            }
        }
    }
}
