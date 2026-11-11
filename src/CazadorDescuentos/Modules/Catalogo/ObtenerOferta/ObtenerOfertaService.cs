using CazadorDescuentos.Data;
using Microsoft.EntityFrameworkCore;

namespace CazadorDescuentos.Modules.Catalogo.ObtenerOferta;

internal sealed class ObtenerOfertaService
{
    private readonly AppDbContext _db;

    public ObtenerOfertaService(AppDbContext db) => _db = db;

    /// <summary>Busca la oferta por Id (aunque esté vencida, para no romper enlaces ya compartidos).</summary>
    public async Task<OfertaDetalle?> EjecutarAsync(int id, CancellationToken ct)
    {
        var oferta = await _db.Ofertas.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, ct);
        return oferta is null ? null : Mapeo.ADetalle(oferta);
    }
}
