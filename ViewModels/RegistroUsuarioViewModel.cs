using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Referidos.Data;
using Xamarin.Essentials;
using Plugin.DeviceInfo;
using Preferences = Microsoft.Maui.Storage.Preferences;

namespace Referidos.ViewModels
{
    public class RegistroUsuarioViewModel : INotifyPropertyChanged
    {
        public ICommand EnviarCommand { get; private set; }

        public RegistroUsuarioViewModel()
        {
            EnviarCommand = new Command(Enviar);
        }

        // Propiedades para enlazar los campos del formulario con el ViewModel
        private string _nombreCompleto;
        public string NombreCompleto
        {
            get => _nombreCompleto;
            set
            {
                _nombreCompleto = value;
                OnPropertyChanged(nameof(NombreCompleto));
            }
        }

        private int _cedula;
        public int Cedula
        {
            get => _cedula;
            set
            {
                _cedula = value;
                OnPropertyChanged(nameof(Cedula));
            }
        }

        private string _correo;
        public string Correo
        {
            get => _correo;
            set
            {
                _correo = value;
                OnPropertyChanged(nameof(Correo));
            }
        }

        private int _telefono;
        public int Telefono
        {
            get => _telefono;
            set
            {
                _telefono = value;
                OnPropertyChanged(nameof(Telefono));
            }
        }

        private string _ciudad;
        public string Ciudad
        {
            get => _ciudad;
            set
            {
                _ciudad = value;
                OnPropertyChanged(nameof(Ciudad));
            }
        }

        private string _empresa;
        public string Empresa
        {
            get => _empresa;
            set
            {
                _empresa = value;
                OnPropertyChanged(nameof(Empresa));
            }
        }

        private string _cargo;
        public string Cargo
        {
            get => _cargo;
            set
            {
                _cargo = value;
                OnPropertyChanged(nameof(Cargo));
            }
        }

        private async void Enviar()
        {

            string id = CrossDeviceInfo.Current.Id;

            // Validar que los campos requeridos no estén vacíos
            if (string.IsNullOrEmpty(NombreCompleto) || Cedula == 0 || Telefono == 0 || string.IsNullOrEmpty(Ciudad))
            {
                // Mostrar el mensaje de error utilizando DisplayAlert
                await Application.Current.MainPage.DisplayAlert("Error", "Faltan campos por rellenar.", "OK");
                return; // Salir del método si hay campos requeridos vacíos
            }

            // Guardar el nombre en la caché
            Preferences.Set("NombreUsuarioCache", NombreCompleto);

            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "INSERT INTO bs_refe (bs_nombre, bs_cedula, bs_correo, bs_telefono, bs_ciudad, bs_empresa, bs_fingreso, bs_estado, bs_mac, bs_cargo) " +
               "VALUES (@NombreCompleto, @Cedula, @Correo, @Telefono, @Ciudad, @Empresa, @FechaIngreso, @Estado, @Mac, @Cargo)";

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NombreCompleto", NombreCompleto);
                cmd.Parameters.AddWithValue("@Cedula", Cedula);
                cmd.Parameters.AddWithValue("@Correo", Correo);
                cmd.Parameters.AddWithValue("@Telefono", Telefono);
                cmd.Parameters.AddWithValue("@Ciudad", Ciudad);
                cmd.Parameters.AddWithValue("@Empresa", Empresa);
                // Agregar la fecha actual al parámetro @FechaIngreso
                cmd.Parameters.AddWithValue("@FechaIngreso", DateTime.Now);
                // Establecer el valor de bs_estado como 0
                cmd.Parameters.AddWithValue("@Estado", 0);
                cmd.Parameters.AddWithValue("@Mac", id);
                cmd.Parameters.AddWithValue("@Cargo", Cargo);
                cmd.ExecuteNonQuery();

                // Mostrar la alerta de éxito si el registro fue exitoso
                await MostrarAlertaExito();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }



        private async Task MostrarAlertaExito()
        {
            // Mostrar el mensaje de éxito utilizando DisplayAlert
            await Application.Current.MainPage.DisplayAlert("Registro Exitoso", "Pronto le enviaremos una clave de activación.", "OK");

            // Redireccionar a la página de inicio (HomePage)
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
