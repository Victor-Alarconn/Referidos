using Microsoft.Maui.Controls;

namespace Referidos.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            // Puedes llenar la lista del men� aqu�
           
        }

        // Maneja el evento cuando se selecciona un �tem
        private void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as FlyoutItem;
            if (item != null)
            {
                // Aqu� puedes manejar la navegaci�n seg�n el �tem seleccionado
                // Por ejemplo:
                if (item.Title == "Principal")
                {
                    (Parent as FlyoutPage).Detail = new NavigationPage(new PrincipalPage());
                }
                else if (item.Title == "Progreso")
                {
                    (Parent as FlyoutPage).Detail = new NavigationPage(new ProgresoPage());
                }
                // A�ade l�gica para otros �tems aqu�...

                // Cierra el men� lateral despu�s de seleccionar una opci�n
                (Parent as FlyoutPage).IsPresented = false;
            }
        }
    }
}
