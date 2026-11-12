using CazadorDescuentos.Data;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Semaforo> EjecutarAsync(int ofertaId, CancellationToken ct)
    {
        var votos = _db.Votos.Where(v => v.OfertaId == ofertaId);
        var positivos = await votos.CountAsync(v => v.Funciono, ct);
        var negativos = await votos.CountAsync(v => !v.Funciono, ct);
        return Calcular(positivos, negativos, _opciones.Value);
    }

    /// <summary>
    /// Con menos de MinimoVotos: SinDatos. Si no, según el % de "Me funcionó":
    /// ≥ PorcentajeVerde → Verde; &lt; PorcentajeRojo → Rojo; en el medio → Amarillo.
    /// </summary>
    public static Semaforo Calcular(int votosPositivos, int votosNegativos, OpcionesSemaforo opciones)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(votosPositivos);
        ArgumentOutOfRangeException.ThrowIfNegative(votosNegativos);

        var total = votosPositivos + votosNegativos;
        if (total == 0 || total < opciones.MinimoVotos)
        {
            return Semaforo.SinDatos;
        }

        var porcentaje = votosPositivos * 100m / total;
        if (porcentaje >= opciones.PorcentajeVerde)
        {
            return Semaforo.Verde;
        }

        return porcentaje < opciones.PorcentajeRojo ? Semaforo.Rojo : Semaforo.Amarillo;
    }
}
