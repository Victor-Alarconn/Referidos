using Referidos.ViewModels;
namespace Referidos.Views;



public partial class RefierePage : ContentPage
{
	public RefierePage()
	{
		InitializeComponent();
        BindingContext = new RefierePageViewModel();
    }
}