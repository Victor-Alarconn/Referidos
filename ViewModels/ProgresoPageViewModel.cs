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
            }
        }

        public ICommand RefreshCommand { get; }





        public ProgresoPageViewModel()
        {
            Clientes = new ObservableCollection<Clientes>();
            RefreshCommand = new Command(async () => await LoadData());
            // Aquí puedes agregar lógica para obtener los datos de la base de datos y llenar la lista de usuarios
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
                string id = CrossDeviceInfo.Current.Id;

                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "SELECT bs_Nombre, bs_Telefono1, bs_Empresa, bs_email, bs_Estado FROM bs_main WHERE bs_Equipo = @EquipoId";
                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EquipoId", id);

                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string nombre = GetOrDefault(reader, "bs_Nombre");
                    string telefono = GetOrDefault(reader, "bs_Telefono1");
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
                throw;
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
