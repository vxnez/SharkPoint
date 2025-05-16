using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class VentasDatabase
{
    readonly SQLiteAsyncConnection _database;

    public VentasDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Venta>().Wait();
    }

    public Task<List<Venta>> GetVentasPorFechaAsync(DateTime fecha) =>
        _database.Table<Venta>().Where(v => v.Fecha == fecha.Date).ToListAsync();

    public Task<int> SaveVentaAsync(Venta venta) =>
        _database.InsertAsync(venta);

}