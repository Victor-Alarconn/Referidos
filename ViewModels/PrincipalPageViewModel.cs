using MySqlConnector;
using Referidos.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Referidos.ViewModels
{
    public class ImageInfo
    {
        public string ImagePath { get; set; }
        public string Link { get; set; }
    }
    public class PrincipalPageViewModel : INotifyPropertyChanged
    {

        private System.Timers.Timer _carouselTimer;
        public ObservableCollection<ImageInfo> ImagePaths { get; set; }

        private string _imagen;
        public string Imagen
        {
            get => _imagen;
            set
            {
                _imagen = value;
                OnPropertyChanged(nameof(Imagen));
            }
        }

        public ICommand RefiereCommand { get; private set; }
        public ICommand OpenLinkCommand { get; private set; }
        public ICommand borrarCommand { get; private set; }

        public PrincipalPageViewModel()
        {
            RefiereCommand = new Command(async () => await PrincipalPageViewModel.Mover());

            // Inicializar la colección
            ImagePaths = new ObservableCollection<ImageInfo>();
            OpenLinkCommand = new Command(OpenLink);
            borrarCommand = new Command(async () => await borrar());


            // Cargar las imágenes desde la base de datos
            _ = CargarImagenesDesdeBD();
            _ = CargarImagenBD();

            _carouselTimer = new System.Timers.Timer();
            _carouselTimer.Interval = 10000; // 10 segundos
            _carouselTimer.Elapsed += (s, e) =>
            {
                CurrentPosition = (CurrentPosition + 1) % ImagePaths.Count;
            };
            _carouselTimer.Start();
        }

        public void Dispose()
        {
            _carouselTimer?.Stop();
            _carouselTimer?.Dispose();
        }

        private async void OpenLink()
        {
            var selectedItem = ImagePaths[CurrentPosition];
            if (selectedItem != null)
            {
                try
                {
                    bool result = await Launcher.OpenAsync(new Uri(selectedItem.Link));
                    if (!result)
                    {
                        Console.WriteLine("No se pudo abrir la URL.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al abrir la URL: {ex.Message}");
                }
            }
        }


        private async Task CargarImagenesDesdeBD()
        {
            using MySqlConnection connection = DataConexion.ObtenerConexion();
            connection.Open();

            string query = "SELECT ruta_img, bs_links FROM bs_imgs";
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string ruta = reader.GetString("ruta_img");
                string link = reader.GetString("bs_links");

                var rutaLocal = await PrincipalPageViewModel.DescargarImagenYGuardar(ruta);

                ImagePaths.Add(new ImageInfo { ImagePath = rutaLocal, Link = link });
            }
        }


        private async Task CargarImagenBD()
        {
            using MySqlConnection connection = DataConexion.ObtenerConexion();
            connection.Open();

            string query = "SELECT bs_url FROM bs_img"; 
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string ruta = reader.GetString("bs_url");
                Imagen = await PrincipalPageViewModel.DescargarImagenYGuardar(ruta);
            }
        }



        public static async Task<string> DescargarImagenYGuardar(string imageUrl)
        {
            try
            {
                using var client = new HttpClient();
                var bytes = await client.GetByteArrayAsync(imageUrl);

                // Aquí es donde cambiamos la lógica de almacenamiento
                var dataDir = System.IO.Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "images");
                if (!System.IO.Directory.Exists(dataDir))
                {
                    System.IO.Directory.CreateDirectory(dataDir);
                }
                var filename = System.IO.Path.Combine(dataDir, System.IO.Path.GetRandomFileName() + ".jpg");
                await System.IO.File.WriteAllBytesAsync(filename, bytes);

                return filename;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al descargar la imagen: {ex.Message}");
                return string.Empty;
            }
        }


        private int _currentPosition;
        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                OnPropertyChanged(nameof(CurrentPosition));
            }
        }

        private async Task borrar() // Se actuliza portafolio en la nube 
        {
            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                // Definir la consulta SQL para actualizar registros
                string query = @"
            INSERT INTO `porta`.`pt_conten` (`pt_conte`, `id_pt`) VALUES ('https://rmsoft.com.co/Portafolio/Grupos/Turismo/Contenido/1.jpg', '16');
        ";

                using MySqlCommand cmd = new MySqlCommand(query, connection);

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


        private async Task ActualizarRegistros()
        {
            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                // Definir las consultas SQL para actualizar los registros
                string query1 = "UPDATE bs_grupo SET gr_clave = 'M456' WHERE gr_nombre = 'Ana Mile'";
                string query2 = "UPDATE bs_grupo SET gr_clave = 'M147' WHERE gr_nombre = 'Yessica'";
                string query3 = "UPDATE bs_grupo SET gr_clave = '123' WHERE gr_nombre = 'oficina Rm Soft'";

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



        private static async Task Mover()
        {
            await Shell.Current.GoToAsync("//RefierePage");
        }


        // Implementar la interfaz INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
