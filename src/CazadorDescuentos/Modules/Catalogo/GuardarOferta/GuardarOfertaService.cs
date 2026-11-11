using CazadorDescuentos.Data;
using Microsoft.EntityFrameworkCore;

namespace CazadorDescuentos.Modules.Catalogo.GuardarOferta;

internal sealed class GuardarOfertaService
{
    /// <summary>Argentina no tiene horario de verano: UTC-3 todo el año.</summary>
    internal static readonly TimeSpan OffsetArgentina = TimeSpan.FromHours(-3);

    private readonly AppDbContext _db;
    private readonly TimeProvider _reloj;

    public GuardarOfertaService(AppDbContext db, TimeProvider reloj)
    {
        _db = db;
        _reloj = reloj;
    }

    /// <summary>Inserta la oferta o, si ya existe una con la misma clave única, la actualiza. Devuelve el Id.</summary>
    public async Task<int> EjecutarAsync(NuevaOferta oferta, CancellationToken ct)
    {
        var marca = Marcas.Normalizar(oferta.Marca) ?? oferta.Marca.Trim();
        var clave = ClaveUnica.Generar(marca, oferta.Producto);

        var existente = await _db.Ofertas.FirstOrDefaultAsync(o => o.ClaveUnica == clave, ct);
        var entidad = existente ?? new Oferta { ClaveUnica = clave };

        entidad.Marca = marca;
        entidad.Producto = oferta.Producto.Trim();
        entidad.Categoria = oferta.Categoria.Trim();
        entidad.Codigo = VacioANulo(oferta.Codigo);
        entidad.Restricciones = VacioANulo(oferta.Restricciones);
        entidad.EnlaceComercio = oferta.EnlaceComercio.Trim();
        entidad.PrecioOriginal = oferta.PrecioOriginal;
        entidad.PrecioPromocional = oferta.PrecioPromocional;
        entidad.PorcentajeDescuento = oferta.PorcentajeDescuento > 0
            ? oferta.PorcentajeDescuento
            : CalcularPorcentaje(oferta.PrecioOriginal, oferta.PrecioPromocional);
        entidad.VencimientoUtc = VencimientoAUtc(oferta.Vencimiento);
        entidad.UltimaActualizacionUtc = _reloj.GetUtcNow().UtcDateTime;

        if (existente is null)
        {
            _db.Ofertas.Add(entidad);
        }

        await _db.SaveChangesAsync(ct);
        return entidad.Id;
    }

    /// <summary>Fin del día (23:59:59.9999999) en hora de Argentina, expresado en UTC.</summary>
    internal static DateTime VencimientoAUtc(DateOnly vencimiento)
    {
        var finDelDia = vencimiento.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Unspecified);
        return new DateTimeOffset(finDelDia, OffsetArgentina).UtcDateTime;
    }

    internal static decimal CalcularPorcentaje(decimal precioOriginal, decimal precioPromocional) =>
        precioOriginal <= 0 || precioPromocional >= precioOriginal
            ? 0
            : Math.Round((precioOriginal - precioPromocional) / precioOriginal * 100, 0, MidpointRounding.AwayFromZero);

    private static string? VacioANulo(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}
