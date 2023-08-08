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

        public AppShellViewModel()
        {
            SalirCommand = new Command(Salir);
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
