using MySqlConnector;
using Plugin.DeviceInfo;
using Referidos.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Referidos.ViewModels
{
    public partial class AppShellViewModel : INotifyPropertyChanged
    {
        public ICommand SalirCommand { get; private set; }
        public bool MostrarAceptacion { get; set; }
        private string _nombreUsuarioCache;

        public string NombreUsuarioCache
        {
            get => _nombreUsuarioCache;
            set
            {
                if (_nombreUsuarioCache != value)
                {
                    _nombreUsuarioCache = value;
                    OnPropertyChanged(nameof(NombreUsuarioCache));
                }
            }
        }

        private string _asesorCache;
        public string AsesorCache
        {
            get => _asesorCache;
            set
            {
                if(_asesorCache != value)
                {
                    _asesorCache = value;
                    OnPropertyChanged(nameof(AsesorCache));
                }
            }
        }

        public AppShellViewModel()
        {
            SalirCommand = new Command(Salir);
            NombreUsuarioCache = Preferences.Get("NombreUsuarioCache", string.Empty);
            AsesorCache = Preferences.Get("AsesorCache", string.Empty);

            var empresa = Preferences.Get("EmpresaUsuarioCache", string.Empty);

            if (string.IsNullOrEmpty(NombreUsuarioCache) || string.IsNullOrEmpty(AsesorCache))
            {
                var datosUsuario = ObtenerDatosDelUsuario();

                if (datosUsuario != null)
                {
                    Preferences.Set("NombreUsuarioCache", datosUsuario.NombreUsuario);
                    Preferences.Set("AsesorCache", datosUsuario.Asesor);

                    empresa = datosUsuario.Empresa;
                }
            }

            MostrarAceptacion = empresa == "rm";
        }

        public class DatosUsuario
        {
            public string NombreUsuario { get; set; }
            public string Asesor { get; set; }
            public string Empresa { get; set; }
        }

        public DatosUsuario ObtenerDatosDelUsuario()
        {
            try
            {
                using (MySqlConnection connection = DataConexion.ObtenerConexion())
                {
                    connection.Open();

                    // Obtener el ID del dispositivo
                    string id = CrossDeviceInfo.Current.Id;

                    // Crear el comando SQL
                    MySqlCommand cmd = new MySqlCommand("SELECT gr_nombre, id_grupo FROM bs_grupo WHERE gr_mac = @id", connection);
                    cmd.Parameters.AddWithValue("@id", id);

                    using MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        return new DatosUsuario
                        {
                            NombreUsuario = reader.GetString("gr_nombre"),
                            Asesor = reader.GetInt32(reader.GetOrdinal("id_grupo")).ToString(),
                        };
                    }
                    else
                    {
                        // No se encontró un registro que coincida con el ID del dispositivo
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;  // Puedes decidir si quieres retornar un valor predeterminado o manejar el error de alguna otra manera
            }
        }



        private void Salir()
        {
            App.Current.Quit();
        }

        // Implementar la interfaz INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
