using CazadorDescuentos.Modules.CanjeVerificacion;
using CazadorDescuentos.Modules.Catalogo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CazadorDescuentos.Pages.Ofertas;

public class DetalleModel(ICatalogo catalogo, IVerificacion verificacion, TimeProvider reloj) : PageModel
{
    /// <summary>Filtros con los que se llegó desde el feed, para volver al mismo lugar.</summary>
    [BindProperty(SupportsGet = true)]
    public string? Marca { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Categoria { get; set; }

    public OfertaDetalle Oferta { get; private set; } = default!;

    public ResumenVotos Votos { get; private set; } = new(Semaforo.SinDatos, 0, 0);

    public ReglasSemaforo Reglas => verificacion.Reglas;

    public bool Vencida { get; private set; }

    public DateTime AhoraUtc { get; private set; }

    public string UrlVolver => Url.Page("/Index", new { Marca, Categoria })!;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var oferta = await catalogo.ObtenerPorIdAsync(id, ct);
        if (oferta is null)
        {
            return NotFound();
        }

        Oferta = oferta;
        AhoraUtc = reloj.GetUtcNow().UtcDateTime;
        Vencida = oferta.VencimientoUtc < AhoraUtc;
        Votos = await verificacion.ObtenerResumenAsync(id, ct);
        return Page();
    }
}
