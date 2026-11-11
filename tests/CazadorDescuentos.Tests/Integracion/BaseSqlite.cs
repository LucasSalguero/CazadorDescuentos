using CazadorDescuentos.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CazadorDescuentos.Tests.Integracion;

/// <summary>
/// Base SQLite en memoria con el AppDbContext real y la migración real aplicada.
/// La conexión queda abierta durante todo el test (si se cierra, SQLite borra la base).
/// </summary>
public sealed class BaseSqlite : IDisposable
{
    private readonly SqliteConnection _conexion = new("DataSource=:memory:");

    public BaseSqlite()
    {
        _conexion.Open();
        using var db = CrearContexto();
        db.Database.Migrate();
    }

    public RelojFijo Reloj { get; } = new(new DateTimeOffset(2026, 11, 11, 15, 0, 0, TimeSpan.Zero));

    internal AppDbContext CrearContexto(params IInterceptor[] interceptores) =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_conexion)
            .AddInterceptors(interceptores)
            .Options);

    internal SqliteConnection Conexion => _conexion;

    public void Dispose() => _conexion.Dispose();
}

/// <summary>Reloj controlable para probar vencimientos.</summary>
public sealed class RelojFijo(DateTimeOffset ahora) : TimeProvider
{
    public DateTimeOffset Ahora { get; set; } = ahora;

    public override DateTimeOffset GetUtcNow() => Ahora;
}
