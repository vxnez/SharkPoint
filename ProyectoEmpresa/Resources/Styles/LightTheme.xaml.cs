namespace ProyectoEmpresa.Resources.Styles;

public partial class LightTheme : Window
{
    public LightTheme()
    {
        InitializeComponent();
        Page = new ContentPage()
        {
            Content = new VerticalStackLayout
            {
                Children = {
                    new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
                    }
                }
            }
        };
    }
}