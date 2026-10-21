using CazadorDescuentos.Data;

namespace CazadorDescuentos.Modules.CanjeVerificacion.RegistrarVoto;

internal sealed class RegistrarVotoService
{
    private readonly AppDbContext _db;

    public RegistrarVotoService(AppDbContext db) => _db = db;

    // TODO #13: guardar el voto con FechaUtc; un solo voto por dispositivo y oferta (devolver false si ya existe).
    public Task<bool> EjecutarAsync(int ofertaId, string deviceId, bool funciono, CancellationToken ct) =>
        throw new NotImplementedException();
}
