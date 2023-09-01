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

            // Suscríbete al evento de cambio de conectividad
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            if (string.IsNullOrEmpty(claveGuardada))
            {
                // Obtén la clave y el estado del usuario
                string claveConsultada = ObtenerClaveUsuario(id); // Necesitas implementar esta función
                int estadoUsuario = ObtenerEstadoUsuario(id);

                if (string.IsNullOrEmpty(claveConsultada) && estadoUsuario == 0)
                {
                    MainPage = new NavigationPage(new HomePage());
                }
                else if (!string.IsNullOrEmpty(claveConsultada))
                {
                    // Guarda la clave en caché
                    Preferences.Set("ClaveCache", claveConsultada);

                    if (estadoUsuario == 1)
                    {
                        MainPage = new AppShell();
                    }
                    else if (estadoUsuario == 2)
                    {
                        MainPage = new NavigationPage(new HomePage());
                        Application.Current.MainPage.DisplayAlert("Alerta", "Usuario inhabilitado", "OK");
                    }
                }
            }
            else
            {
                int estadoUsuario = ObtenerEstadoUsuario(id);

                switch (estadoUsuario)
                {
                    case 0:
                        MainPage = new NavigationPage(new HomePage());
                        break;
                    case 1:
                        MainPage = new AppShell();
                        break;
                    case 2:
                        MainPage = new NavigationPage(new HomePage());
                        Application.Current.MainPage.DisplayAlert("Alerta", "Usuario inhabilitado", "OK");
                        break;
                    default:
                        MainPage = new NavigationPage(new HomePage());
                        break;
                }
            }
        }

        private string ObtenerClaveUsuario(string id)
        {
            string clave = string.Empty;

            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "SELECT bs_clave FROM bs_refe WHERE bs_mac = @id";
                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);

                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    clave = reader.GetString("bs_clave");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la clave del usuario: {ex.Message}");
            }

            return clave;
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

        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var access = e.NetworkAccess;
            if (access == NetworkAccess.Internet)
            {
                // Conexión a Internet disponible
            }
            else
            {
                // Conexión a Internet no disponible
                Application.Current.MainPage.DisplayAlert("Conexión perdida", "Por favor verifica tu conexión a Internet", "OK");
                DataConexion.CerrarTodasLasConexiones();
            }
        }

        protected override void OnSleep()
        {
            // Aquí se maneja lo que sucede cuando tu aplicación va al segundo plano

            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
            DataConexion.CerrarTodasLasConexiones();
        }

        protected override void OnResume()
        {
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            DataConexion.CerrarTodasLasConexiones();

        }


    }
}
