using Microsoft.Maui.Controls;

namespace Referidos.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            // Puedes llenar la lista del menú aquí
           
        }

        // Maneja el evento cuando se selecciona un ítem
        private void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as FlyoutItem;
            if (item != null)
            {
                // Aquí puedes manejar la navegación según el ítem seleccionado
                // Por ejemplo:
                if (item.Title == "Principal")
                {
                    (Parent as FlyoutPage).Detail = new NavigationPage(new PrincipalPage());
                }
                else if (item.Title == "Progreso")
                {
                    (Parent as FlyoutPage).Detail = new NavigationPage(new ProgresoPage());
                }
                // Añade lógica para otros ítems aquí...

                // Cierra el menú lateral después de seleccionar una opción
                (Parent as FlyoutPage).IsPresented = false;
            }
        }
    }
}
