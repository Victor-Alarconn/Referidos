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

        private string _telefono;
        public string Telefono
        {
            get => _telefono;
            set
            {
                _telefono = value;
                OnPropertyChanged(nameof(Telefono));
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

        private string _asesor;
        public string Asesor
        {
            get => _asesor;
            set
            {
                _asesor = value;
                OnPropertyChanged(nameof(Asesor));
            }
        }

        private async void Enviar()
        {
            string id = CrossDeviceInfo.Current.Id;

            // Validar que los campos requeridos no estén vacíos
            if (string.IsNullOrEmpty(Cargo))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El Cargo esta vacio.", "OK");
                return;
            }
          
            if (string.IsNullOrEmpty(Telefono))
            {
                // Mostrar el mensaje de error utilizando DisplayAlert
                await Application.Current.MainPage.DisplayAlert("Error", "Faltan celular por rellenar.", "OK");
                return; // Salir del método si hay campos requeridos vacíos
            }

            if (string.IsNullOrEmpty(NombreCompleto))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El nombre esta vacio.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Ciudad))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "La Ciudad esta vacía.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(Correo))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El correo está vacio.", "OK");
                return;
            }
            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                // Verificar si ya hay un usuario con el mismo ID en la base de datos
                string checkQuery = "SELECT COUNT(*) FROM bs_refe WHERE bs_mac = @Mac";
                using MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@Mac", id);
                var count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Ya hay un Usuario registrado en este equipo.", "OK");
                    return; // Salir del método si ya hay un usuario con el mismo ID
                }

                // Guardar el nombre en la caché
                Preferences.Set("NombreUsuarioCache", NombreCompleto);
                Preferences.Set("AsesorCache", Asesor);

                string query = "INSERT INTO bs_refe (bs_nombre, bs_cargo, bs_correo, bs_telefono, bs_ciudad, bs_vend, bs_fingreso, bs_estado, bs_mac) " +
               "VALUES (@NombreCompleto, @Cargo, @Correo, @Telefono, @Ciudad, @Asesor, @FechaIngreso, @Estado, @Mac)";

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NombreCompleto", NombreCompleto);
                cmd.Parameters.AddWithValue("@Cargo", Cargo);
                cmd.Parameters.AddWithValue("@Correo", Correo);
                cmd.Parameters.AddWithValue("@Telefono", Telefono);
                cmd.Parameters.AddWithValue("@Ciudad", Ciudad);
                cmd.Parameters.AddWithValue("@Asesor", Asesor);
                // Agregar la fecha actual al parámetro @FechaIngreso
                cmd.Parameters.AddWithValue("@FechaIngreso", DateTime.Now);
                // Establecer el valor de bs_estado como 0
                cmd.Parameters.AddWithValue("@Estado", 0);
                cmd.Parameters.AddWithValue("@Mac", id);
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
            LimpiarDatos();
            // Redireccionar a la página de inicio (HomePage)
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }
        private void LimpiarDatos()
        {
            NombreCompleto = string.Empty;
            Telefono = string.Empty;
            Cargo = string.Empty;
            Correo = string.Empty;
            Asesor = string.Empty;
            Ciudad = string.Empty;
            // Y cualquier otro campo que desees restablecer
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
