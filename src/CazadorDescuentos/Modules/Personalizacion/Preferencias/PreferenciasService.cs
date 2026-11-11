using CazadorDescuentos.Modules.Catalogo;

namespace CazadorDescuentos.Modules.Personalizacion.Preferencias;

internal sealed class PreferenciasService
{
    public const string NombreCookie = "preferencias";
    public const int MaximoCategorias = 3;
    private const char Separador = '|';

    /// <summary>Categorías preferidas guardadas en la cookie (lista vacía si no existe).</summary>
    public IReadOnlyList<string> Leer(HttpRequest request)
    {
        if (!request.Cookies.TryGetValue(NombreCookie, out var valor) || string.IsNullOrWhiteSpace(valor))
        {
            return [];
        }

        return Depurar(Uri.UnescapeDataString(valor).Split(Separador), CategoriasValidas(request.HttpContext));
    }

    /// <summary>Guarda como máximo MaximoCategorias categorías válidas. Con lista vacía, borra la cookie.</summary>
    public void Escribir(HttpResponse response, IReadOnlyList<string> seleccionadas)
    {
        var validas = Depurar(seleccionadas, CategoriasValidas(response.HttpContext));
        if (validas.Count == 0)
        {
            response.Cookies.Delete(NombreCookie);
            return;
        }

        response.Cookies.Append(NombreCookie, Uri.EscapeDataString(string.Join(Separador, validas)), new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = response.HttpContext.Request.IsHttps,
        });
    }

    // El servicio es singleton y ICatalogo es scoped: se toma del request en curso.
    private static IReadOnlyList<string> CategoriasValidas(HttpContext contexto) =>
        contexto.RequestServices.GetRequiredService<ICatalogo>().Categorias;

    /// <summary>Se queda con las categorías existentes, sin repetir y como máximo MaximoCategorias.</summary>
    internal static IReadOnlyList<string> Depurar(IEnumerable<string> candidatas, IReadOnlyList<string> existentes) =>
        candidatas
            .Select(c => existentes.FirstOrDefault(e => string.Equals(e, c?.Trim(), StringComparison.OrdinalIgnoreCase)))
            .OfType<string>()
            .Distinct()
            .Take(MaximoCategorias)
            .ToList();
}
