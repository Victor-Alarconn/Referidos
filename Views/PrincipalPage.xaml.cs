
using Referidos.ViewModels;

namespace Referidos.Views;
public partial class PrincipalPage : ContentPage
{
	public PrincipalPage() 
	{
		InitializeComponent();
        // Asignar el ViewModel como contexto de datos para la vista
        BindingContext = new PrincipalPageViewModel();
    }
}