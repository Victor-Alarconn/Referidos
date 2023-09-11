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
        //public ICommand borrarCommand { get; private set; }

        public PrincipalPageViewModel()
        {
            RefiereCommand = new Command(async () => await PrincipalPageViewModel.Mover());

            // Inicializar la colección
            ImagePaths = new ObservableCollection<ImageInfo>();
            OpenLinkCommand = new Command(OpenLink);
            //borrarCommand = new Command(async () => await borrar());


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

      private async Task borrar()
      {
        try
        {
            using MySqlConnection connection = DataConexion.ObtenerConexion();
            connection.Open();

            // Definir la consulta SQL para actualizar los registros
            string query = "UPDATE bs_refe SET bs_vend = 3 WHERE bs_nombre = @nombre";

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
