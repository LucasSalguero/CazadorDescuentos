using CazadorDescuentos.Modules.Catalogo.GuardarOferta;
using CazadorDescuentos.Modules.Catalogo.ListarOfertasActivas;
using CazadorDescuentos.Modules.Catalogo.ObtenerOferta;

namespace CazadorDescuentos.Modules.Catalogo;

/// <summary>Implementación de ICatalogo: delega cada operación en su caso de uso.</summary>
internal sealed class CatalogoService(
    GuardarOfertaService guardarOferta,
    ListarOfertasActivasService listarOfertasActivas,
    ObtenerOfertaService obtenerOferta) : ICatalogo
{
    public IReadOnlyList<string> Categorias => Catalogo.Categorias.Todas;

    public Task<int> GuardarOfertaAsync(NuevaOferta oferta, CancellationToken ct = default) =>
        guardarOferta.EjecutarAsync(oferta, ct);

    public Task<IReadOnlyList<OfertaDetalle>> ListarActivasAsync(FiltrosOfertas filtros, CancellationToken ct = default) =>
        listarOfertasActivas.EjecutarAsync(filtros, ct);

    public Task<OfertaDetalle?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        obtenerOferta.EjecutarAsync(id, ct);
}
