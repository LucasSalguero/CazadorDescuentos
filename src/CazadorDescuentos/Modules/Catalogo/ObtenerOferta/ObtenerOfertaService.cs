using CazadorDescuentos.Data;

namespace CazadorDescuentos.Modules.Catalogo.ObtenerOferta;

internal sealed class ObtenerOfertaService
{
    private readonly AppDbContext _db;

    public ObtenerOfertaService(AppDbContext db) => _db = db;

    // TODO #10: buscar la oferta por Id y devolverla como OfertaDetalle (null si no existe).
    public Task<OfertaDetalle?> EjecutarAsync(int id, CancellationToken ct) =>
        throw new NotImplementedException();
}
