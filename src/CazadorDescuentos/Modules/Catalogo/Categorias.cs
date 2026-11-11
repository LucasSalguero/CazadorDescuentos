namespace CazadorDescuentos.Modules.Catalogo;

internal static class Categorias
{
    public static readonly IReadOnlyList<string> Todas =
    [
        "Hamburguesas",
        "Pollo",
        "Pizza",
        "Postres",
        "Bebidas",
    ];

    /// <summary>Devuelve el nombre canónico de la categoría (sin importar mayúsculas ni espacios) o null si no existe.</summary>
    public static string? Normalizar(string? categoria) =>
        string.IsNullOrWhiteSpace(categoria)
            ? null
            : Todas.FirstOrDefault(c => string.Equals(c, categoria.Trim(), StringComparison.OrdinalIgnoreCase));
}
