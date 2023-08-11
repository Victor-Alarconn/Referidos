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

namespace Referidos.ViewModels
{
    public class ProgresoPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Clientes> Clientes { get; set; }
        public int NumeroDeReferidos => Clientes.Count;


        public ProgresoPageViewModel()
        {
            Clientes = new ObservableCollection<Clientes>();
            // Aquí puedes agregar lógica para obtener los datos de la base de datos y llenar la lista de usuarios
            CargarUsuarios();
        }
        private void CargarUsuarios()
        {
            try
            {
                string id = CrossDeviceInfo.Current.Id;

                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "SELECT bs_Nombre, bs_Telefono1, bs_Empresa, bs_email, bs_Estado FROM bs_main WHERE bs_Equipo = @EquipoId";
                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EquipoId", id);

                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string nombre = reader.IsDBNull(reader.GetOrdinal("bs_Nombre")) ? "No aplica" : reader.GetString("bs_Nombre");
                    string telefono = reader.IsDBNull(reader.GetOrdinal("bs_Telefono1")) ? "No aplica" : reader.GetString("bs_Telefono1");
                    string empresa = reader.IsDBNull(reader.GetOrdinal("bs_Empresa")) ? "No aplica" : reader.GetString("bs_Empresa");
                    string email = reader.IsDBNull(reader.GetOrdinal("bs_email")) ? "No aplica" : reader.GetString("bs_email");
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


       

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
