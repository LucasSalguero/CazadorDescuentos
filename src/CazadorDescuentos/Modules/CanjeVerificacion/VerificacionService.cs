using CazadorDescuentos.Modules.CanjeVerificacion.CalcularSemaforo;
using CazadorDescuentos.Modules.CanjeVerificacion.RegistrarVoto;

namespace CazadorDescuentos.Modules.CanjeVerificacion;

/// <summary>Implementación de IVerificacion: delega cada operación en su caso de uso.</summary>
internal sealed class VerificacionService(
    RegistrarVotoService registrarVoto,
    CalcularSemaforoService calcularSemaforo) : IVerificacion
{
    public Task<bool> RegistrarVotoAsync(int ofertaId, string deviceId, bool funciono, CancellationToken ct = default) =>
        registrarVoto.EjecutarAsync(ofertaId, deviceId, funciono, ct);

    public Task<Semaforo> ObtenerSemaforoAsync(int ofertaId, CancellationToken ct = default) =>
        calcularSemaforo.EjecutarAsync(ofertaId, ct);
}
