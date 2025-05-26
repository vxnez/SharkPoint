using ProyectoEmpresa.Models;
using System.Collections.ObjectModel;
namespace ProyectoEmpresa.Views;

public partial class Ventana3 : ContentPage
{
    private VentasDatabase _ventasDatabase;
    private ObservableCollection<Venta> _ventas = new();
    public Ventana3()
    {
        InitializeComponent();
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "ventas.db3");
        _ventasDatabase = new VentasDatabase(dbPath);

        datePicker.Date = DateTime.Today;
        _ = CargarVentas(DateTime.Today);
    }
    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        await CargarVentas(e.NewDate);
    }

    private async Task CargarVentas(DateTime fecha)
    {
        var ventas = await _ventasDatabase.GetVentasPorFechaAsync(fecha);
        _ventas.Clear();
        foreach (var venta in ventas)
            _ventas.Add(venta);

        ventasCollectionView.ItemsSource = _ventas;
        var total = ventas.Sum(v => v.Cantidad * v.PrecioUnitario);
        totalLabel.Text = $"TOTAL: {total:C2}";
    }
    //Boton para volver a la pagina principal
    private void OnIrAMainPageClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage(), animated: false);
    }
    //Boton para volver a la pagina anterior
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(animated: false);
    }


    private void OnIrThemeClicked(object sender, EventArgs e)
    {

    }

    //Boton para ventana1
    private async void OnIrAVentana1Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Views.Ventana1(), animated: false);
    }

    //Boton para ventana2
    private async void OnIrAVentana2Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Views.Ventana2(), animated: false);
    }

    //Boton para ventana3
    private async void OnIrAVentana3Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Views.Ventana3(), animated: false);
    }

    private void OnPointerEnteredGeneral(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#261946");
        }
    }

    private void OnPointerExitedGeneral(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#352A5F");
        }
    }
    private void OnPointerExitedGeneral1(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#1C2855");
        }
    }
    private void OnPointerExitedGeneral2(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#2B3D6D");
        }
    }
    private void OnPointerExitedGeneral3(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#3A5285");
        }
    }
}