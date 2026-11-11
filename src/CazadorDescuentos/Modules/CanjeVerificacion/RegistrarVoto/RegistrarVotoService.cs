using CazadorDescuentos.Data;
using Microsoft.EntityFrameworkCore;

namespace CazadorDescuentos.Modules.CanjeVerificacion.RegistrarVoto;

internal sealed class RegistrarVotoService
{
    private readonly AppDbContext _db;
    private readonly TimeProvider _reloj;

    public RegistrarVotoService(AppDbContext db, TimeProvider reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    /// <summary>Guarda el voto. Devuelve false si ese dispositivo ya había votado esa oferta.</summary>
    public async Task<bool> EjecutarAsync(int ofertaId, string deviceId, bool funciono, CancellationToken ct)
    {
        deviceId = deviceId.Trim();
        if (await _db.Votos.AnyAsync(v => v.OfertaId == ofertaId && v.DeviceId == deviceId, ct))
        {
            return false;
        }

        _db.Votos.Add(new Voto
        {
            OfertaId = ofertaId,
            DeviceId = deviceId,
            Funciono = funciono,
            FechaUtc = _reloj.GetUtcNow().UtcDateTime,
        });

        try
        {
            await _db.SaveChangesAsync(ct);
            return true;
        }
        catch (DbUpdateException)
        {
            // Dos votos simultáneos del mismo dispositivo: el índice único rechaza el segundo.
            return false;
        }
    }
}
