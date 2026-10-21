using CazadorDescuentos.Data;
using Microsoft.Extensions.Options;

namespace CazadorDescuentos.Modules.CanjeVerificacion.CalcularSemaforo;

internal sealed class CalcularSemaforoService
{
    private readonly AppDbContext _db;
    private readonly IOptions<OpcionesSemaforo> _opciones;

    public CalcularSemaforoService(AppDbContext db, IOptions<OpcionesSemaforo> opciones)
    {
        _db = db;
        _opciones = opciones;
    }

    // TODO #14: contar los votos positivos y negativos de la oferta y calcular el semáforo con Calcular.
    public Task<Semaforo> EjecutarAsync(int ofertaId, CancellationToken ct) =>
        throw new NotImplementedException();

    // TODO #14: aplicar los umbrales de OpcionesSemaforo a la cantidad de votos.
    public static Semaforo Calcular(int votosPositivos, int votosNegativos, OpcionesSemaforo opciones) =>
        throw new NotImplementedException();
}
