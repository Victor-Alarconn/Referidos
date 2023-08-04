using Referidos.ViewsModels;


namespace Referidos
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            // Asignar el ViewModel como el contexto de enlace (BindingContext) de la vista
            BindingContext = new HomePageViewModel();
        }
    }
}
