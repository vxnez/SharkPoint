namespace ProyectoEmpresa
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private void OnIrThemeClicked(object sender, EventArgs e)
        {

        }
        //Boton para ventana1
        private async void OnIrAVentana1Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.Ventana1());
        }

        //Boton para ventana2
        private async void OnIrAVentana2Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.Ventana2());
        }

        //Boton para ventana3
        private async void OnIrAVentana3Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.Ventana3());
        }

        private async void OnIrAMainPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
        private void OnPointerEnteredGeneral(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackgroundColor = Color.FromArgb("#261946");
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
}