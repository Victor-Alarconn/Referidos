using Referidos.ViewModels;

namespace Referidos.Views;

public partial class RegistroUsuarioPage : ContentPage
{
    public RegistroUsuarioPage()
    {
        InitializeComponent();

        // Asignar el ViewModel como contexto de datos para la vista
        BindingContext = new RegistroUsuarioViewModel();
    }
}