using Referidos.Modelos;
using Referidos.ViewModels;

namespace Referidos.Views;

public partial class AceptarPage : ContentPage
{
    public AceptarPage()
    {
        InitializeComponent();
        BindingContext = new AceptarPageViewModel();

        // Suscribirse al mensaje
        MessagingCenter.Subscribe<AceptarPageViewModel, string>(this, "ClaveGenerada", async (sender, arg) =>
        {
            await DisplayAlert("Información", "Clave de Activación: " + arg, "OK");
        });
    }

    private void OnCardTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Referido referido)
        {
            referido.IsExpanded = !referido.IsExpanded;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<AceptarPageViewModel, string>(this, "ClaveGenerada");
    }

   


}