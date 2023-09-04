using Referidos.Modelos;
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

    private void OnCardTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Referido referido)
        {
            referido.IsExpanded = !referido.IsExpanded;
        }
    }

}