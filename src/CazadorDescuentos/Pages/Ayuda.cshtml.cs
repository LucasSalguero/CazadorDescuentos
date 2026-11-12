using CazadorDescuentos.Modules.CanjeVerificacion;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CazadorDescuentos.Pages;

public class AyudaModel(IVerificacion verificacion) : PageModel
{
    public ReglasSemaforo Reglas => verificacion.Reglas;

    public void OnGet()
    {
    }
}
