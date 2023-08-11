using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Referidos.ViewModels
{
    public class PrincipalPageViewModel : INotifyPropertyChanged
    {
        public ICommand RefiereCommand { get; private set; }
        public PrincipalPageViewModel()
        {
            RefiereCommand = new Command(async () => await Mover());


        }

        private async Task Mover()
        {
            await Shell.Current.GoToAsync("//RefierePage");
        }


        // Implementar la interfaz INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
