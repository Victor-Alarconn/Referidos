using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MySqlConnector;
using Referidos.Views;
using Referidos.Data;
using Plugin.DeviceInfo;

namespace Referidos.ViewsModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        public ICommand RegistroCommand { get; private set; }
        public ICommand EnvioClaveCommand { get; private set; }

        private string _clave;
        public string Clave
        {
            get => _clave;
            set
            {
                _clave = value;
                OnPropertyChanged(nameof(Clave));
            }
        }

        public HomePageViewModel()
        {
            RegistroCommand = new Command(Aceptar);
            EnvioClaveCommand = new Command(EnviarClave);
        }

        private void Aceptar()
        {
            // Realizar la navegación a la página de registro (RegistroUsuarioPage)
            Application.Current.MainPage.Navigation.PushAsync(new RegistroUsuarioPage());
        }

        private async void EnviarClave()
        {
            if (string.IsNullOrWhiteSpace(Clave))
            {
                await Application.Current.MainPage.DisplayAlert("Campo vacío", "Por favor ingrese la clave de activación.", "OK");
                return;
            }

            string id = CrossDeviceInfo.Current.Id;
            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "SELECT bs_clave FROM bs_refe WHERE bs_mac = @Mac";
                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Mac", id);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    string claveEnBaseDeDatos = result.ToString();

                    if (Clave == claveEnBaseDeDatos)
                    {
                        // Guardar la clave en la caché
                        Preferences.Set("ClaveCache", Clave);

                        await Application.Current.MainPage.Navigation.PushAsync(new PrincipalPage());
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Clave incorrecta", "La clave de activación ingresada es incorrecta.", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Clave no encontrada", "No se encontró la clave de activación.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }



        // Implementar la interfaz INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
