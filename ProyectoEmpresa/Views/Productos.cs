using System;
using SQLite;

namespace ProyectoEmpresa.Views
{
    public class Productos
    {
        [PrimaryKey]
        public string Id { get; set; } = new Random().Next(1000, 10000).ToString();
        [Unique]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public decimal PrecioDeCompra { get; set; }

        public decimal PrecioDeVenta { get; set; }

        public int Stock { get; set; }

        public string Proveedor { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
    }
}