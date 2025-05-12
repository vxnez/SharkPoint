namespace ProyectoEmpresa.Views;

public partial class Ventana3 : ContentPage
{
    public Ventana3()
    {
        InitializeComponent();
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
}