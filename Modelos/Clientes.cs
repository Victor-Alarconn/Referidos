using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referidos.Modelos
{
    public class Clientes
    {
        public DateTime Bs_Fecha { get; set; }
        public string Bs_Nit { get; set; }
        public string Bs_Empresa { get; set; }
        public string Bs_Cedula { get; set; }
        public string Bs_Nombre { get; set; }
        public string Bs_Tipo { get; set; }
        public string Bs_Regimen { get; set; }
        public int Bs_Referi { get; set; }
        public int Bs_Conecta { get; set; }
        public int Bs_Forma { get; set; }
        public string Bs_Direccion { get; set; }
        public string Bs_Ciudad { get; set; }
        public string Bs_Telefono1 { get; set; }
        public string Bs_Telefono2 { get; set; }
        public string Bs_Email { get; set; }
        public string Bs_Equipo { get; set; }
        public int Bs_Frae { get; set; }
        public int Bs_Nomina { get; set; }
        public int Bs_Docsop { get; set; }
        public string Bs_Notas { get; set; }
        public string Bs_Refiere { get; set; }
        public string Bs_vend { get; set; }
        public int Bs_Estado { get; set; }

        public double ProgressValue
        {
            get
            {
                return Bs_Estado switch
                {
                    0 => 0,
                    1 => 0.25,
                    2 => 0.5,
                    3 => 0.75,
                    4 => 1,
                    _ => 0,
                };
            }
        }



    }
}
