namespace CazadorDescuentos.Modules.Catalogo;

/// <summary>
/// Interfaz pública del módulo Catalogo. Es lo único que otros módulos y el host pueden usar.
/// Los records de abajo forman parte de su contrato.
/// </summary>
public interface ICatalogo
{
    IReadOnlyList<string> Categorias { get; }

    /// <summary>Inserta o actualiza la oferta según su clave única y devuelve su Id.</summary>
    Task<int> GuardarOfertaAsync(NuevaOferta oferta, CancellationToken ct = default);

    Task<IReadOnlyList<OfertaDetalle>> ListarActivasAsync(FiltrosOfertas filtros, CancellationToken ct = default);

    Task<OfertaDetalle?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
}

/// <summary>Datos de entrada de una oferta. El vencimiento es un día en hora de Argentina.</summary>
public sealed record NuevaOferta(
    string Marca,
    string Producto,
    string Categoria,
    string? Codigo,
    string? Restricciones,
    string EnlaceComercio,
    decimal PrecioOriginal,
    decimal PrecioPromocional,
    decimal PorcentajeDescuento,
    DateOnly Vencimiento);

public sealed record OfertaDetalle(
    int Id,
    string Marca,
    string Producto,
    string Categoria,
    string? Codigo,
    string? Restricciones,
    string EnlaceComercio,
    decimal PrecioOriginal,
    decimal PrecioPromocional,
    decimal PorcentajeDescuento,
    DateTime VencimientoUtc);

public sealed record FiltrosOfertas(string? Marca = null, string? Categoria = null);
