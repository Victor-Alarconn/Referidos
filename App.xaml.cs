using Referidos.Views;
using Microsoft.Maui.Controls;
using MySqlConnector;
using Referidos.Data;
using Plugin.DeviceInfo;

namespace Referidos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string claveGuardada = Preferences.Get("ClaveCache", string.Empty);
            string id = CrossDeviceInfo.Current.Id;

            if (!string.IsNullOrEmpty(claveGuardada))
            {
                // Obtener el estado del usuario
                int estadoUsuario = ObtenerEstadoUsuario(id);

                switch (estadoUsuario)
                {
                    case 1:
                        MainPage = new AppShell();
                        break;
                    case 2:
                        MainPage = new NavigationPage(new HomePage());
                        Application.Current.MainPage.DisplayAlert("Alerta", "Usuario inhabilitado", "OK");
                        break;
                    default:
                        // Aquí puedes manejar otros valores de bs_estado o simplemente redirigir al usuario a una página de inicio
                        MainPage = new NavigationPage(new HomePage());
                        break;
                }
            }
            else
            {
                MainPage = new NavigationPage(new HomePage());
            }
        }

        private int ObtenerEstadoUsuario(string id)
        {
            int estado = 0; // Valor por defecto

            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "SELECT bs_estado FROM bs_refe WHERE bs_mac = @Mac"; 

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Mac", id);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    estado = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return estado;
        }

    }
}
