using SQLite;
using System.Text.RegularExpressions;

namespace ProyectoEmpresa.Views
{
    public partial class Ventana1 : ContentPage
    {
        private SQLiteConnection _dbConnection;
        private bool _productosVisibles = false;
        private bool _enModoEdicion = false;
        private Productos? _productoEnEdicion;

        public Ventana1()
        {
            InitializeComponent();

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Productos.db3");
            _dbConnection = new SQLiteConnection(dbPath);

            EditarGuardarButton = this.FindByName<Button>("EditarGuardarButton");
            Productos.ItemSelected += OnProductoSeleccionado; // Suscribir al evento ItemSelected
        }

        private Productos? _productoSeleccionadoParaEliminar;

        private void OnProductoSeleccionado(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                _productoSeleccionadoParaEliminar = e.SelectedItem as Productos;
            }
            else
            {
                _productoSeleccionadoParaEliminar = null;
            }
        }

        public List<Productos> ObtenerTodosLosDatos()
        {
            return _dbConnection.Table<Productos>().ToList();
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

        // Botón Guardar Producto
        private void OnGuardarProductoClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreEntry.Text) ||
                string.IsNullOrWhiteSpace(DescripcionEntry.Text) ||
                string.IsNullOrWhiteSpace(CategoriaEntry.Text) ||
                string.IsNullOrWhiteSpace(PrecioCompraEntry.Text) ||
                string.IsNullOrWhiteSpace(PrecioVentaEntry.Text) ||
                string.IsNullOrWhiteSpace(StockEntry.Text) ||
                string.IsNullOrWhiteSpace(ProveedorEntry.Text))
            {
                DisplayAlert("Error", "Por favor, rellena todos los campos antes de guardar.", "OK");
                return;
            }

            var productoExistente = _dbConnection.Table<Productos>().FirstOrDefault(p => p.Nombre == NombreEntry.Text);
            if (productoExistente != null && !_enModoEdicion)
            {
                DisplayAlert("Error", "Ya existe un producto con este nombre. Por favor, elige un nombre diferente.", "OK");
                return;
            }

            var nuevoProducto = new Productos
            {
                Nombre = NombreEntry.Text,
                Descripcion = DescripcionEntry.Text,
                Categoria = CategoriaEntry.Text,
                PrecioDeCompra = decimal.TryParse(PrecioCompraEntry.Text, out var precioCompra) ? precioCompra : 0m,
                PrecioDeVenta = decimal.TryParse(PrecioVentaEntry.Text, out var precioVenta) ? precioVenta : 0m,
                Stock = int.TryParse(StockEntry.Text, out var stock) ? stock : 0,
                Proveedor = ProveedorEntry.Text
            };

            if (_enModoEdicion && _productoEnEdicion != null)
            {
                _productoEnEdicion.Nombre = nuevoProducto.Nombre;
                _productoEnEdicion.Descripcion = nuevoProducto.Descripcion;
                _productoEnEdicion.Categoria = nuevoProducto.Categoria;
                _productoEnEdicion.PrecioDeCompra = nuevoProducto.PrecioDeCompra;
                _productoEnEdicion.PrecioDeVenta = nuevoProducto.PrecioDeVenta;
                _productoEnEdicion.Stock = nuevoProducto.Stock;
                _productoEnEdicion.Proveedor = nuevoProducto.Proveedor;

                _dbConnection.Update(_productoEnEdicion);
                DisplayAlert("Guardar Producto", "Producto actualizado correctamente.", "OK");
                _enModoEdicion = false;
                _productoEnEdicion = null;
                EditarGuardarButton.Text = "Editar";
            }
            else
            {
                _dbConnection.Insert(nuevoProducto);
                DisplayAlert("Guardar Producto", "Producto guardado correctamente.", "OK");
            }

            var productos = _dbConnection.Table<Productos>().ToList();
            Productos.ItemsSource = productos;

            NombreEntry.Text = string.Empty;
            DescripcionEntry.Text = string.Empty;
            CategoriaEntry.Text = string.Empty;
            PrecioCompraEntry.Text = string.Empty;
            PrecioVentaEntry.Text = string.Empty;
            StockEntry.Text = string.Empty;
            ProveedorEntry.Text = string.Empty;
        }

        // Mostrar productos guardados
        private void OnMostrarProductosClicked(object sender, EventArgs e)
        {
            if (_productosVisibles)
            {
                Productos.IsVisible = false;
                ((Button)sender).Text = "Mostrar";
            }
            else
            {
                var productos = ObtenerTodosLosDatos();
                Productos.ItemsSource = productos;
                Productos.IsVisible = true;
                ((Button)sender).Text = "Ocultar";
            }

            _productosVisibles = !_productosVisibles;
        }

        // Eliminar todos los productos
        private async void OnEliminarProductoClicked(object sender, EventArgs e)
        {
            bool confirmacion = await DisplayAlert("Confirmar Eliminación",
                                                    "¿Estás seguro de que deseas eliminar TODOS los productos de la base de datos?",
                                                    "Sí",
                                                    "No");

            if (!confirmacion)
            {
                return;
            }

            try
            {
                _dbConnection.DeleteAll<Productos>();
                _dbConnection.Execute("DELETE FROM sqlite_sequence WHERE name = 'Productos'");
                Productos.ItemsSource = null;

                await DisplayAlert("Base de Datos Eliminada", "Todos los productos han sido eliminados correctamente.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al eliminar la base de datos: {ex.Message}", "OK");
            }
        }

        // Botón para eliminar un único producto
        private async void OnEliminarUnicoProductoClicked(object sender, EventArgs e)
        {
            if (_productoSeleccionadoParaEliminar == null)
            {
                await DisplayAlert("Error", "Por favor, selecciona un producto de la lista para eliminar.", "OK");
                return;
            }

            bool confirmacion = await DisplayAlert("Confirmar Eliminación",
                                                    $"¿Estás seguro de que deseas eliminar el producto: {_productoSeleccionadoParaEliminar.Nombre}?",
                                                    "Sí",
                                                    "No");

            if (confirmacion)
            {
                try
                {
                    _dbConnection.Delete(_productoSeleccionadoParaEliminar);
                    var productos = ObtenerTodosLosDatos();
                    Productos.ItemsSource = productos;
                    _productoSeleccionadoParaEliminar = null; // Deseleccionar el producto

                    await DisplayAlert("Producto Eliminado", "El producto ha sido eliminado correctamente.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurrió un error al eliminar el producto: {ex.Message}", "OK");
                }
            }
        }

        // Botón para editar productos
        private async void OnEditarGuardarClicked(object sender, EventArgs e)
        {
            if (!_enModoEdicion)
            {
                var productoSeleccionado = (Productos)Productos.SelectedItem;

                if (productoSeleccionado == null)
                {
                    await DisplayAlert("Error", "Por favor, selecciona un producto para editar.", "OK");
                    return;
                }

                NombreEntry.Text = productoSeleccionado.Nombre;
                DescripcionEntry.Text = productoSeleccionado.Descripcion;
                CategoriaEntry.Text = productoSeleccionado.Categoria;
                PrecioCompraEntry.Text = productoSeleccionado.PrecioDeCompra.ToString();
                PrecioVentaEntry.Text = productoSeleccionado.PrecioDeVenta.ToString();
                StockEntry.Text = productoSeleccionado.Stock.ToString();
                ProveedorEntry.Text = productoSeleccionado.Proveedor;

                _productoEnEdicion = productoSeleccionado;
                _enModoEdicion = true;
                EditarGuardarButton.Text = "Actualizar";

                await DisplayAlert("Modo Edición", "Edita los campos y presiona 'Actualizar' para actualizar el producto.", "OK");
            }
            else
            {
                if (_productoEnEdicion == null)
                {
                    await DisplayAlert("Error", "No hay ningún producto en edición.", "OK");
                    return;
                }

                try
                {
                    _productoEnEdicion.Nombre = NombreEntry.Text;
                    _productoEnEdicion.Descripcion = DescripcionEntry.Text;
                    _productoEnEdicion.Categoria = CategoriaEntry.Text;
                    _productoEnEdicion.PrecioDeCompra = decimal.TryParse(PrecioCompraEntry.Text, out var precioCompra) ? precioCompra : 0m;
                    _productoEnEdicion.PrecioDeVenta = decimal.TryParse(PrecioVentaEntry.Text, out var precioVenta) ? precioVenta : 0m;
                    _productoEnEdicion.Stock = int.TryParse(StockEntry.Text, out var stock) ? stock : 0;
                    _productoEnEdicion.Proveedor = ProveedorEntry.Text;

                    _dbConnection.Update(_productoEnEdicion);

                    var productos = ObtenerTodosLosDatos();
                    Productos.ItemsSource = productos;

                    // Limpiar las entradas de texto
                    NombreEntry.Text = string.Empty;
                    DescripcionEntry.Text = string.Empty;
                    CategoriaEntry.Text = string.Empty;
                    PrecioCompraEntry.Text = string.Empty;
                    PrecioVentaEntry.Text = string.Empty;
                    StockEntry.Text = string.Empty;
                    ProveedorEntry.Text = string.Empty;

                    _productoEnEdicion = null;
                    _enModoEdicion = false;
                    EditarGuardarButton.Text = "Editar";

                    await DisplayAlert("Producto Editado", "El producto ha sido actualizado correctamente.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurrió un error al guardar los cambios: {ex.Message}", "OK");
                }
            }
        }

        // Colores para el botón de ELIMINAR (ÚNICO)
        private void OnPointerEnteredEliminarUnico(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackgroundColor = Colors.DarkRed;
            }
        }

        private void OnPointerExitedEliminarUnico(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackgroundColor = Color.FromArgb("#352A5F");
            }
        }

        // Colores para el botón de ELIMINAR (TODOS)
        private void OnPointerEnteredEliminar(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackgroundColor = Colors.DarkRed;
            }
        }

        private void OnPointerExitedEliminar(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackgroundColor = Color.FromArgb("#352A5F");
            }
        }

        // Colores para el botón de GUARDAR
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

        // Colores para los botones GENERALES
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
}