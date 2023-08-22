using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Referidos.ViewModels
{
    public class PrincipalPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> ImagePaths { get; set; }
        public ICommand RefiereCommand { get; private set; }
        public PrincipalPageViewModel()
        {
            RefiereCommand = new Command(async () => await Mover());
            ImagePaths = new ObservableCollection<string>
            {
                "imagen1.png",
                "imagen2.png",
                "imagen3.png",
                "imagen4.png",
                "imagen5.png",
                "imagen6.png"
            };

            System.Timers.Timer carouselTimer = new System.Timers.Timer();
            carouselTimer.Interval = 10000; // 10 segundos
            carouselTimer.Elapsed += (s, e) =>
            {
                CurrentPosition = (CurrentPosition + 1) % ImagePaths.Count;
            };
            carouselTimer.Start();
        }

        private int _currentPosition;
        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                _currentPosition = value;
                OnPropertyChanged(nameof(CurrentPosition));
            }
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
