using Referidos.ViewModels;
using Referidos.Views;

namespace Referidos;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		BindingContext = new AppShellViewModel();
	}

    private async void OnTermsTapped(object sender, EventArgs e)
	{
		
        await Navigation.PushModalAsync(new NavigationPage(new TermsAndConditionsPage()));
    }

   
}
