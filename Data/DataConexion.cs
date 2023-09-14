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

        public static class ConfiguracionBD
        {
            public static Dictionary<string, string> conexiones = new Dictionary<string, string>
    {
        { "Local", "Server=192.168.1.150;User ID=root;Password=**Rm20So23fT**;Database=clix;SslMode=None" },
        { "Produccion", "Server=200.118.190.167;User ID=RmSoft20X;Password=*LiLo89*;Database=clix;SslMode=None" }
    };

            public static string ConexionActual = conexiones["Local"];  
        }


        public static MySqlConnection ObtenerConexion()
        {
            string connectionString = ConfiguracionBD.ConexionActual;
            MySqlConnection connection = new MySqlConnection(connectionString);
            // Agrega la conexión a la lista
            ConexionesActivas.Add(connection);
            return connection;
        }


        public static void CerrarTodasLasConexiones()
        {
            List<MySqlConnection> conexionesEliminadas = new List<MySqlConnection>();
            foreach (var conexion in ConexionesActivas)
            {
                try
                {
                    if (conexion.State == ConnectionState.Open)
                    {
                        conexion.Close();
                        conexionesEliminadas.Add(conexion);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            // Solo elimina las conexiones que se cerraron correctamente
            foreach (var conexion in conexionesEliminadas)
            {
                ConexionesActivas.Remove(conexion);
            }
        }

    }
}
