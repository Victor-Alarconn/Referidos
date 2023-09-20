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
        public ICommand borrarCommand { get; private set; }
        public HomePageViewModel()
        {
            RegistroCommand = new Command(Aceptar);
            EnvioClaveCommand = new Command(EnviarClave);
            borrarCommand = new Command(async () => await InsertarRegistros());
        }

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

                string query = "SELECT gr_clave FROM bs_grupo WHERE gr_mac = @Mac";
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

                        await Shell.Current.GoToAsync("//RefierePage");
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

        private async Task borrar()
        {
            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                // Definir la consulta SQL para actualizar los registros
                string query = "UPDATE bs_refe SET bs_clave = M258 WHERE bs_nombre = @nombre";

                using MySqlCommand cmd = new MySqlCommand(query, connection);

                // Establecer el parámetro para la consulta
                cmd.Parameters.AddWithValue("@nombre", "ELIUTH NIÑO Salcedo");

                // Ejecutar la consulta
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Registros actualizados con éxito.");
                }
                else
                {
                    Console.WriteLine("No se encontraron registros para actualizar.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

        private async Task InsertarRegistros()
        {
            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                // Definir la consulta SQL para insertar nuevos registros
                string query = "INSERT INTO bs_refe (bs_nombre, bs_cargo, bs_correo, bs_telefono, bs_ciudad, bs_empres, bs_clave, bs_estado, bs_vend, bs_fingre) " +
                               "VALUES (@nombre, @cargo, @correo, @telefono, @ciudad, @empres, @clave, @estado, @vend, @fingre)";

                // Datos para los registros
                var registros = new[]
                {
            new { nombre = "Fernando Ramires", cargo = "Gerente", correo = "administracion@onfotecpereira.com.co", telefono = "3103816874", ciudad = "Pereira", empres = "Infotec com sas", clave = "M248" },
            new { nombre = "Hector Ortiz", cargo = "Gerente", correo = "mg11@hotmail.com", telefono = "3103858129", ciudad = "Pereira", empres = "MGI", clave = "M426" },
            new { nombre = "Liliana Arevalo", cargo = "Gerente", correo = "lianarevalo@hotmail.com", telefono = "3136843658", ciudad = "Pereira", empres = "Repuestos Express", clave = "M953" }
        };

                foreach (var registro in registros)
                {
                    using MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@nombre", registro.nombre);
                    cmd.Parameters.AddWithValue("@cargo", registro.cargo);
                    cmd.Parameters.AddWithValue("@correo", registro.correo);
                    cmd.Parameters.AddWithValue("@telefono", registro.telefono);
                    cmd.Parameters.AddWithValue("@ciudad", registro.ciudad);
                    cmd.Parameters.AddWithValue("@empres", registro.empres);
                    cmd.Parameters.AddWithValue("@clave", registro.clave);
                    cmd.Parameters.AddWithValue("@estado", 1);
                    cmd.Parameters.AddWithValue("@vend", 3);
                    cmd.Parameters.AddWithValue("@fingre", DateTime.Now);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Registro para {registro.nombre} insertado con éxito.");
                    }
                    else
                    {
                        Console.WriteLine($"Error al insertar registro para {registro.nombre}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task MostrarColumnas()
        {
            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                // Definir la consulta SQL para obtener las columnas de la tabla
                string query = "SHOW COLUMNS FROM bs_grupo";

                using MySqlCommand cmd = new MySqlCommand(query, connection);

                using MySqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    Console.WriteLine("Columnas de la tabla bs_grupo:");
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine(reader.GetString("Field"));
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron columnas.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task MostrarRegistrosEspecificos()
        {
            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                // Definir la consulta SQL para obtener registros específicos de la tabla
                string query = "SELECT gr_nombre, gr_mac FROM bs_grupo";

                using MySqlCommand cmd = new MySqlCommand(query, connection);

                using MySqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    Console.WriteLine("Registros de la tabla bs_grupo (columnas gr_nombre y gr_mac):");
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"gr_nombre: {reader["gr_nombre"]}\tgr_mac: {reader["gr_mac"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron registros.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task ActualizarRegistros()
        {
            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                // Definir las consultas SQL para actualizar los registros
                string query1 = "UPDATE bs_grupo SET gr_mac = 'NSXDU17909007831' WHERE gr_nombre = 'Ana Mile'";
                string query2 = "UPDATE bs_grupo SET gr_mac = '6c71347a7036fed3' WHERE gr_nombre = 'Yessica'";
                string query3 = "UPDATE bs_grupo SET gr_mac = 'dba66641fb5a9b82' WHERE gr_nombre = 'oficina Rm Soft'";

                // Ejecutar las consultas de actualización
                using MySqlCommand cmd1 = new MySqlCommand(query1, connection);
                int rowsAffected1 = await cmd1.ExecuteNonQueryAsync();

                using MySqlCommand cmd2 = new MySqlCommand(query2, connection);
                int rowsAffected2 = await cmd2.ExecuteNonQueryAsync();

                using MySqlCommand cmd3 = new MySqlCommand(query3, connection);
                int rowsAffected3 = await cmd3.ExecuteNonQueryAsync();

                // Mostrar resultados de las actualizaciones
                Console.WriteLine($"Registros actualizados para 'Ana Mile': {rowsAffected1}");
                Console.WriteLine($"Registros actualizados para 'Yessica': {rowsAffected2}");
                Console.WriteLine($"Registros actualizados para 'oficina Rm Soft': {rowsAffected3}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
