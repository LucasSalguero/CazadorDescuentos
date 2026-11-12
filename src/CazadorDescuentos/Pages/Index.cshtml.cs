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

    public DateTime AhoraUtc { get; private set; }

    public bool HayFiltros => !string.IsNullOrWhiteSpace(Marca) || !string.IsNullOrWhiteSpace(Categoria);

    public async Task OnGetAsync(CancellationToken ct)
    {
        AhoraUtc = reloj.GetUtcNow().UtcDateTime;
        var ofertas = await catalogo.ListarActivasAsync(new FiltrosOfertas(Marca, Categoria), ct);
        Preferencias = personalizacion.ObtenerPreferencias(Request);
        Ofertas = personalizacion.Priorizar(ofertas, Preferencias);
    }
}
