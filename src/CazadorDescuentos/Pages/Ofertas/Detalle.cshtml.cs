using CazadorDescuentos.Modules.CanjeVerificacion;
using CazadorDescuentos.Modules.Catalogo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CazadorDescuentos.Pages.Ofertas;

public class DetalleModel(ICatalogo catalogo, IVerificacion verificacion) : PageModel
{
    public OfertaDetalle Oferta { get; private set; } = default!;

    public Semaforo Semaforo { get; private set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var oferta = await catalogo.ObtenerPorIdAsync(id, ct);
        if (oferta is null)
        {
            return NotFound();
        }

        Oferta = oferta;
        Semaforo = await verificacion.ObtenerSemaforoAsync(id, ct);
        return Page();
    }
}
