using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Referidos.ViewModels
{
    public partial class AppShellViewModel : INotifyPropertyChanged
    {
        public ICommand SalirCommand { get; private set; }
        private string _nombreUsuarioCache;

        public string NombreUsuarioCache
        {
            get => _nombreUsuarioCache;
            set
            {
                if (_nombreUsuarioCache != value)
                {
                    _nombreUsuarioCache = value;
                    OnPropertyChanged(nameof(NombreUsuarioCache));
                }
            }
        }

        private string _asesorCache;
        public string AsesorCache
        {
            get => _asesorCache;
            set
            {
                if(_asesorCache != value)
                {
                    _asesorCache = value;
                    OnPropertyChanged(nameof(AsesorCache));
                }
            }
        }

        public AppShellViewModel()
        {
            SalirCommand = new Command(Salir);
            NombreUsuarioCache = Preferences.Get("NombreUsuarioCache", string.Empty);
            AsesorCache = Preferences.Get("AsesorCache", string.Empty);
        }

        private void Salir()
        {
            App.Current.Quit();
        }

        // Implementar la interfaz INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
