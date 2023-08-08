using Referidos.ViewModels;
namespace Referidos.Views;

public partial class ProgresoPage : ContentPage
{
	public ProgresoPage()
	{
		InitializeComponent();
        BindingContext = new ProgresoPageViewModel();
    }
}