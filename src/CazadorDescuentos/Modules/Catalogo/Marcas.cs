namespace CazadorDescuentos.Modules.Catalogo;

/// <summary>
/// Marcas de las que el MVP toma ofertas. Los nombres deben coincidir con Ingesta:Marcas en appsettings.json.
/// </summary>
internal static class Marcas
{
    public const string Mostaza = "Mostaza";
    public const string McDonalds = "McDonald's";
    public const string BurgerKing = "Burger King";

    public static readonly IReadOnlyList<string> Todas = [Mostaza, McDonalds, BurgerKing];

    /// <summary>Devuelve el nombre canónico de la marca (sin importar mayúsculas, espacios o apóstrofos) o null si no es una marca soportada.</summary>
    public static string? Normalizar(string? marca)
    {
        if (string.IsNullOrWhiteSpace(marca))
        {
            return null;
        }

        var buscada = Simplificar(marca);
        return Todas.FirstOrDefault(m => Simplificar(m) == buscada);
    }

    private static string Simplificar(string texto) =>
        new([.. texto.ToLowerInvariant().Where(char.IsLetterOrDigit)]);
}
