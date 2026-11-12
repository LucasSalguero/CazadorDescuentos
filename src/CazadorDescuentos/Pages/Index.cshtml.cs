using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Personalizacion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CazadorDescuentos.Pages;

public class IndexModel(ICatalogo catalogo, IPersonalizacion personalizacion, TimeProvider reloj) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Marca { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Categoria { get; set; }

    public IReadOnlyList<string> Categorias => catalogo.Categorias;

    public IReadOnlyList<string> Marcas => catalogo.Marcas;

    public IReadOnlyList<string> Preferencias { get; private set; } = [];

    public IReadOnlyList<OfertaPriorizada> Ofertas { get; private set; } = [];

    /// <summary>Ofertas activas sin filtros (para mostrar "N de M").</summary>
    public int TotalActivas { get; private set; }

    /// <summary>Cuántas ofertas quedarían al tocar cada marca (respetando la categoría elegida).</summary>
    public IReadOnlyDictionary<string, int> ConteoPorMarca { get; private set; } = new Dictionary<string, int>();

    /// <summary>Cuántas ofertas quedarían al tocar cada categoría (respetando la marca elegida).</summary>
    public IReadOnlyDictionary<string, int> ConteoPorCategoria { get; private set; } = new Dictionary<string, int>();

    /// <summary>Filtros que llegaron en la URL pero no existen (p. ej. ?Marca=Wendys).</summary>
    public IReadOnlyList<string> FiltrosDesconocidos { get; private set; } = [];

    public DateTime AhoraUtc { get; private set; }

    public bool HayFiltros => !string.IsNullOrWhiteSpace(Marca) || !string.IsNullOrWhiteSpace(Categoria);

    public async Task OnGetAsync(CancellationToken ct)
    {
        AhoraUtc = reloj.GetUtcNow().UtcDateTime;

        // Los valores de la URL se llevan al nombre oficial ("mcdonalds" → "McDonald's") para marcar el chip activo.
        var desconocidos = new List<string>();
        Marca = Canonico(Marca, Marcas, "marca", desconocidos);
        Categoria = Canonico(Categoria, Categorias, "categoría", desconocidos);
        FiltrosDesconocidos = desconocidos;

        var todas = await catalogo.ListarActivasAsync(new FiltrosOfertas(), ct);
        TotalActivas = todas.Count;
        ConteoPorMarca = Marcas.ToDictionary(m => m, m => todas.Count(o => o.Marca == m && (Categoria is null || o.Categoria == Categoria)));
        ConteoPorCategoria = Categorias.ToDictionary(c => c, c => todas.Count(o => o.Categoria == c && (Marca is null || o.Marca == Marca)));

        var ofertas = FiltrosDesconocidos.Count > 0 ? [] : await catalogo.ListarActivasAsync(new FiltrosOfertas(Marca, Categoria), ct);
        Preferencias = personalizacion.ObtenerPreferencias(Request);
        Ofertas = personalizacion.Priorizar(ofertas, Preferencias);
    }

    private static string? Canonico(string? valor, IReadOnlyList<string> opciones, string nombre, List<string> desconocidos)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var encontrado = opciones.FirstOrDefault(o => Simplificar(o) == Simplificar(valor));
        if (encontrado is null)
        {
            desconocidos.Add($"No encontramos la {nombre} \"{valor.Trim()}\".");
        }

        return encontrado ?? valor.Trim();
    }

    private static string Simplificar(string texto) =>
        new([.. texto.ToLowerInvariant().Where(char.IsLetterOrDigit)]);
}
