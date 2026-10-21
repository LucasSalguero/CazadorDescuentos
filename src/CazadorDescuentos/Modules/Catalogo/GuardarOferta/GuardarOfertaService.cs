using CazadorDescuentos.Data;

namespace CazadorDescuentos.Modules.Catalogo.GuardarOferta;

internal sealed class GuardarOfertaService
{
    private readonly AppDbContext _db;

    public GuardarOfertaService(AppDbContext db) => _db = db;

    // TODO #4: generar la ClaveUnica, convertir el vencimiento (fin del día en hora de Argentina, UTC-3) a UTC,
    // insertar o actualizar la oferta por ClaveUnica, actualizar UltimaActualizacionUtc y devolver el Id.
    public Task<int> EjecutarAsync(NuevaOferta oferta, CancellationToken ct) =>
        throw new NotImplementedException();
}
