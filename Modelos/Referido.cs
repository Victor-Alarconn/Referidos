using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referidos.Modelos
{
    public class Referido : INotifyPropertyChanged
    {

        public int Id_refi { get; set; }
        public string Bs_nombre { get; set; }
        public string Bs_cedula { get; set; }
        public string Bs_correo { get; set; }
        public string Bs_telefono { get; set; }
        public string Bs_ciudad { get; set; }
        public string Bs_empresa { get; set; }
        public string Bs_mac { get; set; }
        public string Bs_clave { get; set; }
        public string Bs_estado { get; set; }
        public string Bs_cargo { get; set; }
        public string Bs_vend { get; set; }
        public DateTime Bs_fingreso { get; set; }

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
