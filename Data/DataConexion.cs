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
            MySqlConnection connection = new("Server=192.168.1.184;User ID=victor;Password=123456;Database=clix");
            return connection;
        }
    }
}
