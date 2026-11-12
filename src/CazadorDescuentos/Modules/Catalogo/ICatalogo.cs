namespace CazadorDescuentos.Modules.Catalogo;

/// <summary>
/// Interfaz pública del módulo Catalogo. Es lo único que otros módulos y el host pueden usar.
/// Los records de abajo forman parte de su contrato.
/// </summary>
public interface ICatalogo
{
    IReadOnlyList<string> Categorias { get; }

    /// <summary>Marcas de las que se toman ofertas (Mostaza, McDonald's y Burger King).</summary>
    IReadOnlyList<string> Marcas { get; }

    /// <summary>Inserta o actualiza la oferta según su clave única y devuelve su Id.</summary>
    Task<int> GuardarOfertaAsync(NuevaOferta oferta, CancellationToken ct = default);

    Task<IReadOnlyList<OfertaDetalle>> ListarActivasAsync(FiltrosOfertas filtros, CancellationToken ct = default);

    Task<OfertaDetalle?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
}

/// <summary>
/// Datos de entrada de una oferta. El vencimiento es un día en hora de Argentina.
/// ImagenUrl es opcional: URL http(s) de la foto o ruta local (/img/...).
/// </summary>
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
    DateOnly Vencimiento,
    string? ImagenUrl = null);

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
    DateTime VencimientoUtc,
    string? ImagenUrl = null);

public sealed record FiltrosOfertas(string? Marca = null, string? Categoria = null);
