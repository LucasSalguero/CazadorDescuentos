using CazadorDescuentos.Data;
using Microsoft.EntityFrameworkCore;

namespace CazadorDescuentos.Modules.Catalogo.ListarOfertasActivas;

internal sealed class ListarOfertasActivasService
{
    private readonly AppDbContext _db;
    private readonly TimeProvider _reloj;

    public ListarOfertasActivasService(AppDbContext db, TimeProvider reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    /// <summary>
    /// Ofertas vigentes (#5), filtradas por marca (#7) y categoría (#8) cuando se indican.
    /// Orden por defecto: mayor porcentaje de descuento primero y, a igualdad, vencimiento más próximo.
    /// </summary>
    public async Task<IReadOnlyList<OfertaDetalle>> EjecutarAsync(FiltrosOfertas filtros, CancellationToken ct)
    {
        var ahora = _reloj.GetUtcNow().UtcDateTime;
        var consulta = _db.Ofertas.AsNoTracking().Where(o => o.VencimientoUtc >= ahora);

        if (!string.IsNullOrWhiteSpace(filtros.Marca))
        {
            var marca = Marcas.Normalizar(filtros.Marca) ?? filtros.Marca.Trim();
            consulta = consulta.Where(o => o.Marca == marca);
        }

        if (!string.IsNullOrWhiteSpace(filtros.Categoria))
        {
            var categoria = filtros.Categoria.Trim();
            consulta = consulta.Where(o => o.Categoria == categoria);
        }

        var ofertas = await consulta.ToListAsync(ct);

        // SQLite guarda los decimal como TEXT y no puede ordenarlos: se ordena en memoria.
        return ofertas
            .OrderByDescending(o => o.PorcentajeDescuento)
            .ThenBy(o => o.VencimientoUtc)
            .Select(Mapeo.ADetalle)
            .ToList();
    }
}
