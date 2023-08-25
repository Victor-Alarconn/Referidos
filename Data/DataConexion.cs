using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referidos.Data
{
    public class DataConexion
    {
        public static MySqlConnection ObtenerConexion()
        {
            string connectionString = "Server=192.168.1.150;User ID=root;Password=**Rm20So23fT**;Database=clix;SslMode=None";
            MySqlConnection connection = new MySqlConnection(connectionString);
            return connection;
        }
    }

}
