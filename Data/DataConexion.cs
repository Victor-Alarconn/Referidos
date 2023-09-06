using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referidos.Data
{
    public class DataConexion
    {
        // Lista para almacenar todas las conexiones activas
        public static List<MySqlConnection> ConexionesActivas = new List<MySqlConnection>();

        public static MySqlConnection ObtenerConexion()
        {
           string connectionString = "Server=192.168.1.150;User ID=root;Password=**Rm20So23fT**;Database=clix;SslMode=None";
           // string connectionString = "Server=200.118.190.167;User ID=RmSoft20X;Password=*LiLo89*;Database=clix;SslMode=None";


            MySqlConnection connection = new MySqlConnection(connectionString);
            // Agrega la conexión a la lista
            ConexionesActivas.Add(connection);

            return connection;
        }

        public static void CerrarTodasLasConexiones()
        {
            foreach (var conexion in ConexionesActivas)
            {
                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
            }

            // Limpiar la lista una vez que todas las conexiones estén cerradas
            ConexionesActivas.Clear();
        }
    }
}
