using Referidos.ViewModels;

namespace Referidos.Views;

public partial class EstadisticaPage : ContentPage
{
	public EstadisticaPage()
	{
		InitializeComponent();
        // Asignar el ViewModel como contexto de datos para la vista
        BindingContext = new EstadisticaViewModel();
    }
}