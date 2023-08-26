using Referidos.Modelos;
using Referidos.ViewModels;

namespace Referidos.Views;

public partial class AceptarPage : ContentPage
{
	public AceptarPage()
	{
		InitializeComponent();
		BindingContext = new AceptarPageViewModel();
	}

    private void OnCardTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Referido referido)
        {
            referido.IsExpanded = !referido.IsExpanded;
        }
    }
}