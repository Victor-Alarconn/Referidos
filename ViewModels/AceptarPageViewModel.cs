using MySqlConnector;
using Plugin.DeviceInfo;
using Referidos.Data;
using Referidos.Modelos;
using Referidos.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Referidos.ViewModels
{
    public partial class AceptarPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Referido> Clientes { get; set; }
        public ICommand AceptarCommand { get; private set; }
        public ICommand RechazarCommand { get; private set; }
        public ICommand RefreshCommand { get; }


        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        public AceptarPageViewModel()
        {
            Clientes = new ObservableCollection<Referido>();
            AceptarCommand = new Command<string>(Aceptar);
            RefreshCommand = new Command(async () => await LoadData());
            RechazarCommand = new Command<string>(Rechazar);
            // Aquí se agrega la lógica para obtener los datos de la base de datos y llenar la lista de usuarios
            CargarUsuarios();
        }

        

        private async Task LoadData()
        {
            IsBusy = true;

            CargarUsuarios();

            IsBusy = false;
        }

        private void CargarUsuarios()
        {
            try
            {
                // Limpia la colección antes de agregar nuevos registros.
                Clientes.Clear();
                string AsesorCache = Preferences.Get("AsesorCache", string.Empty);

                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "SELECT bs_nombre, bs_telefono, bs_empres, bs_correo, bs_ciudad, bs_cargo, bs_fingre, bs_vend FROM bs_refe WHERE (bs_vend = @AsesorCache OR bs_vend = '00') AND bs_estado = 0";

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AsesorCache", AsesorCache);

                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string nombre = AceptarPageViewModel.GetOrDefault(reader, "bs_nombre");
                    string telefono = AceptarPageViewModel.GetOrDefault(reader, "bs_telefono");
                    string empresa = AceptarPageViewModel.GetOrDefault(reader, "bs_empres");
                    string email = AceptarPageViewModel.GetOrDefault(reader, "bs_correo");
                    string ciudad = AceptarPageViewModel.GetOrDefault(reader, "bs_ciudad");
                    string cargo = AceptarPageViewModel.GetOrDefault(reader, "bs_cargo");
                    string vend = AceptarPageViewModel.GetOrDefault(reader, "bs_vend");

                    Clientes.Add(new Referido { Bs_nombre = nombre, Bs_correo = email, Bs_telefono = telefono, Bs_empresa = empresa, Bs_ciudad = ciudad, Bs_cargo = cargo, Bs_vend = vend });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al cargar los datos. Por favor, verifica tu conexión y vuelve a intentarlo.", "OK");
            }
        }

        private static string GetOrDefault(MySqlDataReader reader, string columnName)
        {
            if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
            {
                return reader.GetString(reader.GetOrdinal(columnName));
            }
            return "No aplica";
        }


        private async void Aceptar(string nombre)
        {
            using MySqlConnection connection = DataConexion.ObtenerConexion();
            connection.Open();

            // Consulta para verificar si ya existe una clave para el nombre especificado
            string checkQuery = "SELECT bs_clave FROM bs_refe WHERE bs_nombre = @nombre AND bs_clave IS NOT NULL AND bs_clave <> ''";

            using MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@nombre", nombre);

            object result = checkCmd.ExecuteScalar(); // Retorna el primer valor de la primera fila o null si no hay resultados

            if (result != null && result != DBNull.Value)
            {
                // Ya existe una clave para el nombre especificado
                await Application.Current.MainPage.DisplayAlert("Alerta", "Ya existe una clave para este Usuario.", "OK");
                return;
            }

            // Si no existe una clave, procedemos a generar una y actualizar el registro
            string query = "UPDATE bs_refe SET bs_estado = 1, bs_clave = @clave WHERE bs_nombre = @nombre AND (bs_clave IS NULL OR bs_clave = '')";

            using MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@nombre", nombre);

            // Generar clave que inicie con M y 3 números aleatorios
            Random rnd = new Random();
            string clave = "M" + rnd.Next(100, 999).ToString();
            cmd.Parameters.AddWithValue("@clave", clave);

            cmd.ExecuteNonQuery(); // Ejecuta la consulta

            // Enviamos el mensaje con la clave generada
            MessagingCenter.Send(this, "ClaveGenerada", clave);
        }



        private void Rechazar(string nombre)
        {
            using MySqlConnection connection = DataConexion.ObtenerConexion();
            connection.Open();

            string query = "UPDATE bs_refe SET bs_estado = 3 WHERE bs_nombre = @nombre AND (bs_clave IS NULL OR bs_clave = '')";

            using MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.ExecuteNonQuery();

            connection.Close();
        }



        // Implementar la interfaz INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
