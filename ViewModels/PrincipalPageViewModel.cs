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

        public PrincipalPageViewModel()
        {
            RefiereCommand = new Command(async () => await PrincipalPageViewModel.Mover());

            // Inicializar la colección
            ImagePaths = new ObservableCollection<ImageInfo>();
            OpenLinkCommand = new Command(OpenLink);


            // Cargar las imágenes desde la base de datos
            CargarImagenesDesdeBD();
            CargarImagenBD();

            System.Timers.Timer carouselTimer = new System.Timers.Timer();
            carouselTimer.Interval = 10000; // 10 segundos
            carouselTimer.Elapsed += (s, e) =>
            {
                CurrentPosition = (CurrentPosition + 1) % ImagePaths.Count;
            };
            carouselTimer.Start();
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

            string query = "SELECT bs_url FROM bs_img LIMIT 1"; // Limitamos a una sola imagen
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
                var filename = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".jpg");
                await File.WriteAllBytesAsync(filename, bytes);
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
