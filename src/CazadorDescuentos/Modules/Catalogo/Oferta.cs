namespace CazadorDescuentos.Modules.Catalogo;

internal sealed class Oferta
{
    public int Id { get; set; }

    /// <summary>Comercio + título normalizado. Tiene índice único.</summary>
    public string ClaveUnica { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string? Codigo { get; set; }
    public string? Restricciones { get; set; }
    public string EnlaceComercio { get; set; } = string.Empty;

    /// <summary>Foto del producto (URL http(s) o ruta local). Null si la fuente no trae foto.</summary>
    public string? ImagenUrl { get; set; }

    public decimal PrecioOriginal { get; set; }
    public decimal PrecioPromocional { get; set; }
    public decimal PorcentajeDescuento { get; set; }

    public DateTime VencimientoUtc { get; set; }
    public DateTime UltimaActualizacionUtc { get; set; }
}
