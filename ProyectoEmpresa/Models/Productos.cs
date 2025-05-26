using System;
using SQLite;

namespace ProyectoEmpresa.Models
{
    public class Productos
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public decimal PrecioDeCompra { get; set; }

        public decimal PrecioDeVenta { get; set; }

        public int Stock { get; set; }

        public string Proveedor { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public string IdFormateado => Id.ToString("D4");
    }
}