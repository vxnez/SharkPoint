using SQLite;

public class Venta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public double PrecioUnitario { get; set; }
}