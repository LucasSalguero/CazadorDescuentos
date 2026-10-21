using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Personalizacion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CazadorDescuentos.Pages;

public class PreferenciasModel(ICatalogo catalogo, IPersonalizacion personalizacion) : PageModel
{
    public IReadOnlyList<string> Categorias => catalogo.Categorias;

    [BindProperty]
    public List<string> Seleccionadas { get; set; } = [];

    public void OnGet()
    {
        Seleccionadas = [.. personalizacion.ObtenerPreferencias(Request)];
    }

    public IActionResult OnPost()
    {
        personalizacion.GuardarPreferencias(Response, Seleccionadas);
        return RedirectToPage("/Index");
    }
}
