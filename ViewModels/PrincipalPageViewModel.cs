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
    public class PrincipalPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> ImagePaths { get; set; }
        public ICommand RefiereCommand { get; private set; }

        public PrincipalPageViewModel()
        {
            RefiereCommand = new Command(async () => await Mover());

            // Inicializar la colección
            ImagePaths = new ObservableCollection<string>();

            // Cargar las imágenes desde la base de datos
            CargarImagenesDesdeBD();

            System.Timers.Timer carouselTimer = new System.Timers.Timer();
            carouselTimer.Interval = 10000; // 10 segundos
            carouselTimer.Elapsed += (s, e) =>
            {
                CurrentPosition = (CurrentPosition + 1) % ImagePaths.Count;
            };
            carouselTimer.Start();
        }

        private async Task CargarImagenesDesdeBD()
        {
            using MySqlConnection connection = DataConexion.ObtenerConexion();
            connection.Open();

            string query = "SELECT ruta_img FROM bs_imagenes";
            using MySqlCommand cmd = new MySqlCommand(query, connection);
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string ruta = reader.GetString("ruta_img");
                var rutaLocal = await DescargarImagenYGuardar(ruta);
                ImagePaths.Add(rutaLocal);
            }
        }

        public async Task<string> DescargarImagenYGuardar(string imageUrl)
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


        private async Task Mover()
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
