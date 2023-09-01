using MySqlConnector;
using Plugin.DeviceInfo;
using Referidos.Data;
using Referidos.Modelos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Referidos.ViewModels
{
    public class ProgresoPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Clientes> Clientes { get; set; }
        public int NumeroDeReferidos => Clientes.Count;

        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public bool IsNotBusy
        {
            get { return !IsBusy; }
        }

        public ICommand RefreshCommand { get; }


        public ProgresoPageViewModel()
        {
            Clientes = new ObservableCollection<Clientes>();
            RefreshCommand = new Command(async () => await LoadData());

            // Al cargar la vista, sólo activa el ActivityIndicator
            IsBusy = true;
            IsRefreshing = false;

            // Carga tus datos
            LoadInitialData();
        }

        private async Task LoadInitialData()
        {
            // Carga tus datos
            CargarUsuarios();

            // Añade una demora artificial si es necesario
            await Task.Delay(2000);  // demora de 1 segundo

            IsBusy = false;
        }

        private async Task LoadData()
        {
            IsRefreshing = true;
            IsBusy = false;

            try
            {
                // Carga tus datos
                CargarUsuarios();

                // Añade una demora artificial
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            finally
            {
                // Asegurarte de que IsRefreshing se restablezca
                // incluso si hay una excepción
                IsRefreshing = false;
            }
        }


        private void CargarUsuarios()
        {
            try
            {
                // Limpia la colección antes de agregar nuevos registros.
                Clientes.Clear();
                string id = CrossDeviceInfo.Current.Id;

                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "SELECT bs_Nombre, bs_Telef1, bs_Empresa, bs_email, bs_Estado FROM bs_main WHERE bs_Equipo = @EquipoId";
                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EquipoId", id);

                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string nombre = GetOrDefault(reader, "bs_Nombre");
                    string telefono = GetOrDefault(reader, "bs_Telef1");
                    string empresa = GetOrDefault(reader, "bs_Empresa");
                    string email = GetOrDefault(reader, "bs_email");
                    int estado = reader.IsDBNull(reader.GetOrdinal("bs_Estado")) ? 0 : reader.GetInt32(reader.GetOrdinal("bs_Estado"));

                    Clientes.Add(new Clientes { Bs_Nombre = nombre, Bs_Email = email, Bs_Telefono1 = telefono, Bs_Empresa = empresa, Bs_Estado = estado });
                    OnPropertyChanged(nameof(NumeroDeReferidos));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al cargar los datos. Por favor, verifica tu conexión y vuelve a intentarlo.", "OK");
                
            }
        }

        private string GetOrDefault(MySqlDataReader reader, string columnName)
        {
            if (!reader.IsDBNull(reader.GetOrdinal(columnName)))
            {
                return reader.GetString(reader.GetOrdinal(columnName));
            }
            return "No aplica";
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
