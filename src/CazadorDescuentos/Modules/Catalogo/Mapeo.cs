namespace CazadorDescuentos.Modules.Catalogo;

internal static class Mapeo
{
    public static OfertaDetalle ADetalle(Oferta o) => new(
        o.Id,
        o.Marca,
        o.Producto,
        o.Categoria,
        o.Codigo,
        o.Restricciones,
        o.EnlaceComercio,
        o.PrecioOriginal,
        o.PrecioPromocional,
        o.PorcentajeDescuento,
        DateTime.SpecifyKind(o.VencimientoUtc, DateTimeKind.Utc));
}
