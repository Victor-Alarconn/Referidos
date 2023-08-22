using System.Windows.Input;

namespace Referidos.Views
{
    public partial class TermsAndConditionsPage : ContentPage
    {
        public ICommand CloseCommand => new Command(async () => await CloseModal());

        public TermsAndConditionsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private async Task CloseModal()
        {
            await Navigation.PopModalAsync();
        }
    }
}
