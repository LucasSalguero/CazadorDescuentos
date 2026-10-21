using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Personalizacion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CazadorDescuentos.Pages;

public class IndexModel(ICatalogo catalogo, IPersonalizacion personalizacion) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Marca { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Categoria { get; set; }

    public IReadOnlyList<string> Categorias => catalogo.Categorias;

    public IReadOnlyList<OfertaPriorizada> Ofertas { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken ct)
    {
        var ofertas = await catalogo.ListarActivasAsync(new FiltrosOfertas(Marca, Categoria), ct);
        var preferencias = personalizacion.ObtenerPreferencias(Request);
        Ofertas = personalizacion.Priorizar(ofertas, preferencias);
    }
}
