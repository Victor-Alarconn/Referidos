using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
namespace Referidos.Views;

public partial class FlyoutMenuPage : FlyoutPage
{
    ObservableCollection<FlyoutItem> FlyoutItems = new ObservableCollection<FlyoutItem>();
    public FlyoutMenuPage()
    {

        InitializeComponent();
       
    }


}
