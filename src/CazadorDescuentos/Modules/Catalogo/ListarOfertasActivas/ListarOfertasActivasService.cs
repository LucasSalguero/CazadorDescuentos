using CazadorDescuentos.Data;

namespace CazadorDescuentos.Modules.Catalogo.ListarOfertasActivas;

internal sealed class ListarOfertasActivasService
{
    private readonly AppDbContext _db;

    public ListarOfertasActivasService(AppDbContext db) => _db = db;

    // TODO #5: devolver solo las ofertas vigentes (VencimientoUtc posterior a la hora actual en UTC).
    // TODO #7: filtrar por marca cuando filtros.Marca tiene valor.
    // TODO #8: filtrar por categoría cuando filtros.Categoria tiene valor.
    public Task<IReadOnlyList<OfertaDetalle>> EjecutarAsync(FiltrosOfertas filtros, CancellationToken ct) =>
        throw new NotImplementedException();
}
