using Referidos.ViewModels;

namespace Referidos;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        BindingContext = new AppShellViewModel();
    }

}
