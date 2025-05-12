using Microsoft.Maui.Graphics;
using SQLite;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProyectoEmpresa.Views;

public partial class Ventana2 : ContentPage
{
    private SQLiteConnection _dbConnection;
    public ObservableCollection<Productos> ShoppingList { get; set; } = new ObservableCollection<Productos>();

    public Ventana2()
    {
        InitializeComponent();

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Productos.db3");
        _dbConnection = new SQLiteConnection(dbPath);
        _dbConnection.CreateTable<Productos>();

        BindingContext = this;
    }

    // Botón para volver a la página principal
    private void OnIrAMainPageClicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage(), animated: false);
    }

    // Botón para volver a la página anterior
    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync(animated: false);
    }




    // Botón para ventana1
    private async void OnIrAVentana1Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Ventana1(), animated: false);
    }

    // Botón para ventana2
    private async void OnIrAVentana2Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Ventana2(), animated: false);
    }

    // Botón para ventana3
    private async void OnIrAVentana3Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Ventana3(), animated: false);
    }



    //colores para el boton Guardar
    private void OnPointerEnteredGuardar(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#264130");
        }
    }

    private void OnPointerExitedGuardar(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.BackgroundColor = Color.FromArgb("#352A5F");
        }
    }



    //colores para el boton General

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





    // Registro de cambios en el campo de texto del ID del producto
    private void OnProductIdTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ProductIdEntry.Text))
        {
            ProductNameLabel.Text = "-";
            ProductStockLabel.Text = "-";
            ProductPriceLabel.Text = "-";
            return;
        }

        var productId = ProductIdEntry.Text;
        var product = _dbConnection.Table<Productos>().FirstOrDefault(p => p.Id == productId);

        if (product != null)
        {
            ProductNameLabel.Text = product.Nombre;
            ProductStockLabel.Text = product.Stock.ToString();
            ProductPriceLabel.Text = $"{product.PrecioDeVenta:C}";
        }
        else
        {
            ProductNameLabel.Text = "No encontrado";
            ProductStockLabel.Text = "-";
            ProductPriceLabel.Text = "-";
        }
    }

    //Boton para agregar a la lista de compras
    private async void OnAddToShoppingListClicked(object sender, EventArgs e)
    {
        // Validar entradas
        if (string.IsNullOrWhiteSpace(ProductIdEntry.Text) || string.IsNullOrWhiteSpace(QuantityEntry.Text))
        {
            await DisplayAlert("Error", "Por favor, complete todos los campos antes de agregar.", "OK");
            return;
        }

        // Obtener datos del producto y cantidad deseada
        var productId = ProductIdEntry.Text;
        if (!int.TryParse(QuantityEntry.Text, out int quantity) || quantity <= 0)
        {
            await DisplayAlert("Error", "Por favor, ingrese una cantidad válida.", "OK");
            return;
        }

        var productInDatabase = _dbConnection.Table<Productos>().FirstOrDefault(p => p.Id == productId);

        if (productInDatabase == null)
        {
            await DisplayAlert("Error", "Producto no encontrado.", "OK");
            return;
        }

        // Verificar si hay suficiente stock
        if (productInDatabase.Stock < quantity)
        {
            await DisplayAlert("AVIDO", $"No hay suficiente stock para '{productInDatabase.Nombre}'.\nStock disponible: {productInDatabase.Stock}", "OK");
            return;
        }

        // Calcular subtotal
        var subtotal = productInDatabase.PrecioDeVenta * quantity;

        // Verificar si el producto ya está en la lista de compras
        var existingItem = ShoppingList.FirstOrDefault(item => item.Id == productId);

        if (existingItem != null)
        {
            // Si el producto ya existe, actualiza la cantidad y el subtotal
            existingItem.Stock += quantity;
            existingItem.Subtotal += subtotal;
        }
        else
        {
            // Si el producto no existe, agrégalo a la lista
            ShoppingList.Add(new Productos
            {
                Id = productInDatabase.Id,
                Nombre = productInDatabase.Nombre,
                PrecioDeVenta = productInDatabase.PrecioDeVenta,
                Stock = quantity,
                Subtotal = subtotal
            });
        }

        // Limpiar entradas
        ProductIdEntry.Text = string.Empty;
        QuantityEntry.Text = string.Empty;
        ProductNameLabel.Text = "-";
        ProductStockLabel.Text = "-";
        ProductPriceLabel.Text = "-";

        // Actualizar total
        var total = ShoppingList.Sum(p => p.Subtotal);
        TotalLabel.Text = $"{total:C}";
    }

    // Botón para realizar la compra
    private async void OnComprarButtonClicked(object sender, EventArgs e)
    {
        if (ShoppingList.Count == 0)
        {
            await DisplayAlert("Aviso", "No hay productos en la lista de compra.", "OK");
            return;
        }

        decimal totalAPagar = ShoppingList.Sum(p => p.Subtotal);
        bool confirmacion = await DisplayAlert("Confirmar Compra", $"El total a pagar es: {totalAPagar:C}. ¿Desea continuar?", "Sí", "No");

        if (confirmacion)
        {
            _dbConnection.BeginTransaction();
            try
            {
                foreach (var item in ShoppingList)
                {
                    var productoEnDb = _dbConnection.Table<Productos>().FirstOrDefault(p => p.Id == item.Id);
                    if (productoEnDb != null && productoEnDb.Stock >= item.Stock)
                    {
                        productoEnDb.Stock -= item.Stock;
                        _dbConnection.Update(productoEnDb);
                    }
                    else
                    {
                        await DisplayAlert("Error", $"No hay suficiente stock para el producto: {item.Nombre}.", "OK");
                        _dbConnection.Rollback();
                        return;
                    }
                }
                _dbConnection.Commit();
                await DisplayAlert("Éxito", "La compra se realizó correctamente.", "OK");
                ShoppingList.Clear();
                TotalLabel.Text = "0.00";
            }
            catch (Exception ex)
            {
                _dbConnection.Rollback();
                await DisplayAlert("Error", $"Ocurrió un error al realizar la compra: {ex.Message}", "OK");
            }
        }
    }
}