using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referidos.Modelos
{
    public class Referido
    {
        public int Id_refi { get; set; }
        public string Bs_nombre { get; set; }
        public string Bs_cedula { get; set; }
        public string Bs_correo { get; set; }
        public int Bs_telefono { get; set; }
        public string Bs_ciudad { get; set; }
        public string Bs_empresa { get; set; }
        public string Bs_mac { get; set; }
        public string Bs_clave { get; set; }
        public string Bs_estado { get; set; }
        public string Bs_cargo { get; set; }
        public DateTime Bs_fingreso { get; set; }
    }
}
