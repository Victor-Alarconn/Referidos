using Referidos.Modelos;
using Referidos.ViewModels;
using static Referidos.Data.DataConexion;
using System.Timers;


namespace Referidos.Views;

public partial class EstadisticaPage : ContentPage
{
    public EstadisticaPage()
    {
        InitializeComponent();
        // Asignar el ViewModel como contexto de datos para la vista
        BindingContext = new EstadisticaViewModel();

    }
    private int adminClickCount = 0;
    private System.Timers.Timer adminClickTimer;

    async void OnAdminClicked(object sender, EventArgs e)
    {
        adminClickCount++;

        if (adminClickCount == 1)
        {
            // Inicia un temporizador para esperar un segundo toque.
            adminClickTimer = new System.Timers.Timer(1000); // 1 segundo
            adminClickTimer.Elapsed += (s, args) =>
            {
                adminClickCount = 0; // Reinicia el contador si no se ha tocado el bot�n en 1 segundo.
                adminClickTimer.Stop();
            };
            adminClickTimer.Start();
        }
        else if (adminClickCount == 2)
        {
            adminClickTimer.Stop(); // Detiene el temporizador.
            adminClickCount = 0;    // Reinicia el contador.

            var seleccion = await DisplayActionSheet("Selecciona una conexi�n", "Cancelar", null, ConfiguracionBD.conexiones.Keys.ToArray());

            if (string.IsNullOrEmpty(seleccion))
            {
                // Si la selecci�n es nula o vac�a, simplemente regresa y no hagas nada.
                return;
            }

            if (ConfiguracionBD.conexiones.ContainsKey(seleccion))
            {
                ConfiguracionBD.ConexionActual = ConfiguracionBD.conexiones[seleccion];
                await DisplayAlert("Conexi�n", $"Has cambiado a la conexi�n: {seleccion}", "OK");
            }

        }
    }


    private void OnCardTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Referido referido)
        {
            referido.IsExpanded = !referido.IsExpanded;
        }
    }



}