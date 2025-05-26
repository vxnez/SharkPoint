using Microsoft.Maui.Platform;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ProyectoEmpresa.Models;
using SQLite;
using System.Collections.ObjectModel;

namespace ProyectoEmpresa.Views
{
    public partial class Ventana1 : ContentPage
    {
        private SQLiteConnection _dbConnection;
        private bool _productosVisibles = false;
        private bool _enModoEdicion = false;
        private Productos? _productoEnEdicion;
        private Productos? _productoSeleccionadoParaEliminar;

        public Ventana1()
        {
            InitializeComponent();

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Productos.db3");
            _dbConnection = new SQLiteConnection(dbPath);

            EditarGuardarButton = this.FindByName<Button>("EditarGuardarButton");
            Productos.ItemSelected += OnProductoSeleccionado;
        }

        private void OnProductoSeleccionado(object? sender, SelectedItemChangedEventArgs e)
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



        //====== BOTONES GENERALES ======
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



        //====== BOTONES PARA PRODUCTOS ======
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
            if (sender is Button boton)
            {
                if (_productosVisibles)
                {
                    Productos.IsVisible = false;
                    boton.ImageSource = "view.png";
                }
                else
                {
                    var productos = ObtenerTodosLosDatos();
                    Productos.ItemsSource = productos;
                    Productos.IsVisible = true;
                    boton.ImageSource = "view_no.png";
                }
                _productosVisibles = !_productosVisibles;
            }
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

                await DisplayAlert("Modo Edición", "Edita los campos y presiona nuevamente el boton 'Editar' para actualizar el producto.", "OK");
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
                    _productoEnEdicion.PrecioDeCompra = decimal.TryParse(PrecioCompraEntry.Text, out var precioCompra)
                        ? precioCompra : 0m;
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

                    await DisplayAlert("Producto Editado", "El producto ha sido actualizado correctamente.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Ocurrió un error al guardar los cambios: {ex.Message}", "OK");
                }
            }
        }


        //====== COLORES ======
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


        //====== BOTONES EXPORTAR E IMPORTAR ======
        //Funciones para exportar/importar Excel
        private async void OnExportarExcelClicked(object sender, EventArgs e)
        {
            try
            {
                var productos = ObtenerTodosLosDatos();

                if (productos == null || productos.Count == 0)
                {
                    await DisplayAlert("Exportar", "No hay productos para exportar.", "OK");
                    return;
                }

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Inventario");

                // Encabezados
                var headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("ID");
                headerRow.CreateCell(1).SetCellValue("Nombre");
                headerRow.CreateCell(2).SetCellValue("Descripción");
                headerRow.CreateCell(3).SetCellValue("Categoría");
                headerRow.CreateCell(4).SetCellValue("PrecioCompra");
                headerRow.CreateCell(5).SetCellValue("PrecioVenta");
                headerRow.CreateCell(6).SetCellValue("Stock");
                headerRow.CreateCell(7).SetCellValue("Proveedor");

                // Datos
                var nuevosProductos = new List<Productos>(); // Declare nuevosProductos here
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null) continue;

                    // Asegura que los datos sean del tipo correcto
                    string id = row.GetCell(0)?.ToString() ?? string.Empty;
                    string nombre = row.GetCell(1)?.ToString() ?? string.Empty;
                    string descripcion = row.GetCell(2)?.ToString() ?? string.Empty;
                    string categoria = row.GetCell(3)?.ToString() ?? string.Empty;

                    decimal precioCompra = 0m;
                    if (row.GetCell(4) != null && row.GetCell(4).CellType == CellType.Numeric)
                        precioCompra = (decimal)row.GetCell(4).NumericCellValue;
                    else
                        decimal.TryParse(row.GetCell(4)?.ToString(), out precioCompra);

                    decimal precioVenta = 0m;
                    if (row.GetCell(5) != null && row.GetCell(5).CellType == CellType.Numeric)
                        precioVenta = (decimal)row.GetCell(5).NumericCellValue;
                    else
                        decimal.TryParse(row.GetCell(5)?.ToString(), out precioVenta);

                    int stock = 0;
                    if (row.GetCell(6) != null && row.GetCell(6).CellType == CellType.Numeric)
                        stock = (int)row.GetCell(6).NumericCellValue;
                    else
                        int.TryParse(row.GetCell(6)?.ToString(), out stock);

                    string proveedor = row.GetCell(7)?.ToString() ?? string.Empty;

                    var producto = new Productos
                    {
                        Id = int.TryParse(id, out var parsedId) ? parsedId : 0,
                        Nombre = nombre,
                        Descripcion = descripcion,
                        Categoria = categoria,
                        PrecioDeCompra = precioCompra,
                        PrecioDeVenta = precioVenta,
                        Stock = stock,
                        Proveedor = proveedor
                    };
                    nuevosProductos.Add(producto);
                }

                string fileName = $"Inventario_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                string filePath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }

                await DisplayAlert("Exportar", $"Archivo exportado en:\n{filePath}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private async void OnImportarExcelClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona un archivo Excel",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xlsx" } },
                { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                { DevicePlatform.iOS, new[] { "com.microsoft.excel.xlsx" } }
            })
                });

                if (result == null)
                    return;

                using (var stream = await result.OpenReadAsync())
                {
                    IWorkbook workbook = new XSSFWorkbook(stream);
                    ISheet sheet = workbook.GetSheetAt(0);

                    var nuevosProductos = new List<Productos>();

                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        var row = sheet.GetRow(i);
                        if (row == null) continue;

                        var producto = new Productos
                        {
                            Nombre = row.GetCell(1)?.ToString() ?? string.Empty,
                            Descripcion = row.GetCell(2)?.ToString() ?? string.Empty,
                            Categoria = row.GetCell(3)?.ToString() ?? string.Empty,
                            PrecioDeCompra = row.GetCell(4) != null && row.GetCell(4).CellType == CellType.Numeric ? (decimal)row.GetCell(4).NumericCellValue : 0m,
                            PrecioDeVenta = row.GetCell(5) != null && row.GetCell(5).CellType == CellType.Numeric ? (decimal)row.GetCell(5).NumericCellValue : 0m,
                            Stock = row.GetCell(6) != null && row.GetCell(6).CellType == CellType.Numeric ? (int)row.GetCell(6).NumericCellValue : 0,
                            Proveedor = row.GetCell(7)?.ToString() ?? string.Empty
                        };
                        nuevosProductos.Add(producto);
                    }

                    // Guardar en la base de datos
                    foreach (var producto in nuevosProductos)
                    {
                        // Evita duplicados por nombre
                        var existe = _dbConnection.Table<Productos>().FirstOrDefault(p => p.Nombre == producto.Nombre);
                        if (existe == null)
                            _dbConnection.Insert(producto);
                        else
                            _dbConnection.Update(producto);
                    }

                    // Actualizar la lista en pantalla
                    var productos = _dbConnection.Table<Productos>().ToList();
                    Productos.ItemsSource = productos;
                }

                await DisplayAlert("Importar", "Importación completada y guardada en la base de datos.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error durante la importación: {ex.Message}", "OK");
            }
        }



        //====== SCROLLVIEW ======
        // Botones para desplazar el ScrollView
        private async void OnIrArribaClicked(object sender, EventArgs e)
        {
            // Desplaza el ScrollView al inicio (arriba)
            await MainScrollView.ScrollToAsync(0, 0, true);
        }
        private async void OnIrAbajoClicked(object sender, EventArgs e)
        {
            // Desplaza el ScrollView al final (abajo)
            await MainScrollView.ScrollToAsync(0, MainScrollView.ContentSize.Height, true);
        }
    }

}