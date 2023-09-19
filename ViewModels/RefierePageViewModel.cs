using MySqlConnector;
using Plugin.DeviceInfo;
using Referidos.Data;
using Referidos.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Referidos.ViewModels
{
    public class RefierePageViewModel : INotifyPropertyChanged
    {
        public ICommand EnviarRefeCommand { get; private set; }
        public ICommand CancelarCommand { get; private set; }

        public RefierePageViewModel()
        {
            EnviarRefeCommand = new Command(EnviarRefe);
            CancelarCommand = new Command(async () => await RefierePageViewModel.Cancelar());
        }

        private readonly List<string> _tiposReferencia = new List<string>
        {
            "Sector Comercial",
            "Servicios",
            "Restaurante/Bar",
            "Hotel",
            "Taller",
            "Créditos",
            "Sindicatos",
            "Divisas",
            "Parqueaderos",
            "Transporte",
            "Control de obra",
            "Otro"
        };

        public List<string> TiposReferencia
        {
            get => _tiposReferencia;
        }

        

        // Propiedades para enlazar los campos del formulario con el ViewModel
        private string _nombreCompletoR;
        public string NombreCompletoRefe
        {
            get => _nombreCompletoR;
            set
            {
                _nombreCompletoR = value;
                OnPropertyChanged(nameof(NombreCompletoRefe));
            }
        }

        private string _correoR;
        public string CorreoRefe
        {
            get => _correoR;
            set
            {
                _correoR = value;
                OnPropertyChanged(nameof(CorreoRefe));
            }
        }

        private string _ciudadR;
        public string CiudadRefe
        {
            get => _ciudadR;
            set
            {
                _ciudadR = value;
                OnPropertyChanged(nameof(CiudadRefe));
            }
        }

        private string _direccionR;
        public string DireccionRefe
        {
            get => _direccionR;
            set
            {
                _direccionR = value;
                OnPropertyChanged(nameof(DireccionRefe));
            }
        }

        private string _telefonoR;
        public string TelefonoRefe
        {
            get => _telefonoR;
            set
            {
                _telefonoR = value;
                OnPropertyChanged(nameof(TelefonoRefe));
            }
        }

        private string _tipoR;
        public string TipoRefe
        {
            get => _tipoR;
            set
            {
                _tipoR = value;
                OnPropertyChanged(nameof(TipoRefe));
            }
        }

        private string _empresaR;
        public string EmpresaRefe
        {
            get => _empresaR;
            set
            {
                _empresaR = value;
                OnPropertyChanged(nameof(EmpresaRefe));
            }
        }

        private string _notasR;
        public string NotasRefe
        {
            get => _notasR;
            set
            {
                _notasR = value;
                OnPropertyChanged(nameof(NotasRefe));
            }
        }

        private string _numeroEmpleadosOSucursales;
        public string NumeroEmpleadosOSucursales
        {
            get => _numeroEmpleadosOSucursales;
            set
            {
                _numeroEmpleadosOSucursales = value;
                OnPropertyChanged(nameof(NumeroEmpleadosOSucursales));
            }
        }

        private string _tipoTamañoEmpresa;
        public string TipoTamañoEmpresa
        {
            get => _tipoTamañoEmpresa;
            set
            {
                _tipoTamañoEmpresa = value;
                OnPropertyChanged(nameof(TipoTamañoEmpresa));
            }
        }

        private string _perteneceAEmpresa;
        public string PerteneceAEmpresa
        {
            get => _perteneceAEmpresa;
            set
            {
                _perteneceAEmpresa = value;
                OnPropertyChanged(nameof(PerteneceAEmpresa));
            }
        }


        private static async Task Cancelar()
        {
            await Shell.Current.GoToAsync("//PrincipalPage");
        }

        private async void EnviarRefe()
        {
            string id = CrossDeviceInfo.Current.Id;
            string NombreUsuario = Preferences.Get("NombreUsuarioCache", string.Empty);
            string Asesor = Preferences.Get("AsesorCache", string.Empty);
            string idrefe = Preferences.Get("IdCache", string.Empty);
            int tipoTamañoEmpresaValue = TipoTamañoEmpresa == "Sucursales" ? 1 : 2;
            int perteneceAEmpresaValue = PerteneceAEmpresa == "Sí" ? 1 : 2;

            // Validar que los campos requeridos no estén vacíos
            if (string.IsNullOrEmpty(NombreCompletoRefe))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El nombre completo está vacío.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(TelefonoRefe))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El teléfono está vacío.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(CiudadRefe))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "La ciudad está vacía.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(EmpresaRefe))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "La empresa  está vacía.", "OK");
                return;
            }

            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM bs_main WHERE bs_Telef1 = @TelefonoRefe AND bs_Empresa = @EmpresaRefe";
                using MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@TelefonoRefe", TelefonoRefe);
                checkCmd.Parameters.AddWithValue("@EmpresaRefe", EmpresaRefe);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    // Si hay algún registro que coincida, muestra una alerta y sale de la función
                    await Application.Current.MainPage.DisplayAlert("Atención", "Hay un usuario con datos similares. Verifique los datos.", "OK");
                    return;
                }

                string query = "INSERT INTO bs_main (bs_Nombre, bs_Email, bs_Telef1, bs_Empresa, bs_Tipo, bs_Fecha, bs_Estado, bs_Equipo, bs_Notas, bs_Refiere, bs_vend, bs_Direcci, bs_Ciudad, bs_tipotam, bs_Ntipo, bs_pertene, Id_referi) " +
                 "VALUES (@NombreCompleto, @Correo, @Telefono, @Empresa, @Tipo, @FechaIngreso, @Estado, @Mac, @Notas, @Refiere, @Asesor, @Direccion, @Ciudad, @TipoTamaño, @NumTamaño, @Pertenece, @Idrefe)";

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NombreCompleto", NombreCompletoRefe);
                cmd.Parameters.AddWithValue("@Correo", CorreoRefe);
                cmd.Parameters.AddWithValue("@Telefono", TelefonoRefe);
                cmd.Parameters.AddWithValue("@Empresa", EmpresaRefe);
                cmd.Parameters.AddWithValue("@Tipo", TipoRefe);
                // Agregar la fecha actual al parámetro @FechaIngreso
                cmd.Parameters.AddWithValue("@FechaIngreso", DateTime.Now);
                // Establecer el valor de bs_estado como 0
                cmd.Parameters.AddWithValue("@Estado", 0);
                cmd.Parameters.AddWithValue("@Mac", id);
                cmd.Parameters.AddWithValue("@Notas", NotasRefe);
                cmd.Parameters.AddWithValue("@Refiere", NombreUsuario);
                cmd.Parameters.AddWithValue("@Asesor", Asesor);
                cmd.Parameters.AddWithValue("@Direccion", DireccionRefe);
                cmd.Parameters.AddWithValue("@Ciudad", CiudadRefe);
                cmd.Parameters.AddWithValue("@TipoTamaño", tipoTamañoEmpresaValue);
                int numeroTamaño = string.IsNullOrEmpty(NumeroEmpleadosOSucursales) ? 0 : Convert.ToInt32(NumeroEmpleadosOSucursales);
                cmd.Parameters.AddWithValue("@NumTamaño", numeroTamaño);
                cmd.Parameters.AddWithValue("@Pertenece", perteneceAEmpresaValue);
                cmd.Parameters.AddWithValue("@Idrefe", idrefe);
                cmd.ExecuteNonQuery();

                // Mostrar la alerta de éxito si el registro fue exitoso
                await MostrarAlertaExito();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al enviar los datos. Por favor, verifica tu conexión y vuelve a intentarlo.", "OK");
                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
            }
        }

        private async Task MostrarAlertaExito()
        {
            // Mostrar el mensaje de éxito utilizando DisplayAlert
            await Application.Current.MainPage.DisplayAlert("Registro Exitoso", "El cliente fue guardado con exito.", "OK");
            LimpiarDatos(); // Limpia los datos del formulario
            // Redireccionar a la página PrincipalPage
            await Shell.Current.GoToAsync("//PrincipalPage");
        }

        private void LimpiarDatos()
        {
            NombreCompletoRefe = string.Empty;
            TelefonoRefe = string.Empty;
            CorreoRefe = string.Empty;
            EmpresaRefe = string.Empty;
            NotasRefe = string.Empty;
            TipoRefe = null;
            DireccionRefe = string.Empty;
            CiudadRefe = string.Empty;
            NumeroEmpleadosOSucursales = string.Empty;
            PerteneceAEmpresa = null;
            TipoTamañoEmpresa = null;
            // Y cualquier otro campo que desees restablecer
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
