using CazadorDescuentos.Modules.CanjeVerificacion.CalcularSemaforo;
using CazadorDescuentos.Modules.CanjeVerificacion.RegistrarVoto;
using Microsoft.Extensions.Options;

namespace CazadorDescuentos.Modules.CanjeVerificacion;

/// <summary>Implementación de IVerificacion: delega cada operación en su caso de uso.</summary>
internal sealed class VerificacionService(
    RegistrarVotoService registrarVoto,
    CalcularSemaforoService calcularSemaforo,
    IOptions<OpcionesSemaforo> opciones) : IVerificacion
{
    public ReglasSemaforo Reglas =>
        new(opciones.Value.MinimoVotos, opciones.Value.PorcentajeVerde, opciones.Value.PorcentajeRojo);

    public Task<ResumenVotos> ObtenerResumenAsync(int ofertaId, CancellationToken ct = default) =>
        calcularSemaforo.ResumirAsync(ofertaId, ct);

    public Task<bool> RegistrarVotoAsync(int ofertaId, string deviceId, bool funciono, CancellationToken ct = default) =>
        registrarVoto.EjecutarAsync(ofertaId, deviceId, funciono, ct);

    public Task<Semaforo> ObtenerSemaforoAsync(int ofertaId, CancellationToken ct = default) =>
        calcularSemaforo.EjecutarAsync(ofertaId, ct);
}
