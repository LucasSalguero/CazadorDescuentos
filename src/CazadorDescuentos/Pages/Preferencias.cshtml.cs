using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Personalizacion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CazadorDescuentos.Pages;

public class PreferenciasModel(ICatalogo catalogo, IPersonalizacion personalizacion) : PageModel
{
    public IReadOnlyList<string> Categorias => catalogo.Categorias;

    public int Maximo => 3;

    [BindProperty]
    public List<string> Seleccionadas { get; set; } = [];

    /// <summary>True si ya había preferencias guardadas (para ofrecer borrarlas).</summary>
    public bool TieneGuardadas { get; private set; }

    public void OnGet()
    {
        Seleccionadas = [.. personalizacion.ObtenerPreferencias(Request)];
        TieneGuardadas = Seleccionadas.Count > 0;
    }

    public IActionResult OnPost()
    {
        personalizacion.GuardarPreferencias(Response, Seleccionadas);
        var guardadas = Seleccionadas.Where(Categorias.Contains).Distinct().Take(Maximo).ToList();
        TempData["Aviso"] = guardadas.Count == 0
            ? "No elegiste categorías: vas a ver todas las ofertas por igual."
            : $"Listo. Primero vas a ver: {string.Join(", ", guardadas)}.";
        return RedirectToPage("/Index");
    }

    public IActionResult OnPostBorrar()
    {
        personalizacion.GuardarPreferencias(Response, []);
        TempData["Aviso"] = "Borramos tus preferencias. Ves todas las ofertas por igual.";
        return RedirectToPage("/Index");
    }
}
