using MySqlConnector;
using Referidos.Data;
using Referidos.Modelos;
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
    public class EstadisticaViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<Referido> Clientes { get; set; }
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

        public EstadisticaViewModel()
        {
            Clientes = new ObservableCollection<Referido>();
            RefreshCommand = new Command(async () => await LoadData());
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

                string query = "SELECT bs_nombre, bs_clave, bs_empres, bs_correo, bs_ciudad, bs_cargo, bs_fingre, bs_vend FROM bs_refe WHERE(bs_vend = @AsesorCache OR bs_vend = '3') AND bs_estado = 1 AND(bs_empres != 'rm' OR bs_empres IS NULL)";


                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AsesorCache", AsesorCache);

                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string nombre = EstadisticaViewModel.GetOrDefault(reader, "bs_nombre");
                    string telefono = EstadisticaViewModel.GetOrDefault(reader, "bs_clave");
                    string empresa = EstadisticaViewModel.GetOrDefault(reader, "bs_empres");
                    string email = EstadisticaViewModel.GetOrDefault(reader, "bs_correo");
                    string ciudad = EstadisticaViewModel.GetOrDefault(reader, "bs_ciudad");
                    string cargo = EstadisticaViewModel.GetOrDefault(reader, "bs_cargo");
                    string vend = EstadisticaViewModel.GetOrDefault(reader, "bs_vend");

                    Clientes.Add(new Referido { Bs_nombre = nombre, Bs_correo = email, Bs_clave = telefono, Bs_empresa = empresa, Bs_ciudad = ciudad, Bs_cargo = cargo, Bs_vend = vend });
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

        // Implementar la interfaz INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    
}
