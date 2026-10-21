using CazadorDescuentos.Modules.Catalogo;

namespace CazadorDescuentos.Modules.Personalizacion;

/// <summary>
/// Interfaz pública del módulo Personalizacion. Las preferencias se guardan en una cookie.
/// </summary>
public interface IPersonalizacion
{
    IReadOnlyList<string> ObtenerPreferencias(HttpRequest request);

    void GuardarPreferencias(HttpResponse response, IReadOnlyList<string> categorias);

    IReadOnlyList<OfertaPriorizada> Priorizar(IReadOnlyList<OfertaDetalle> ofertas, IReadOnlyList<string> categoriasPreferidas);
}

public sealed record OfertaPriorizada(OfertaDetalle Oferta, bool CoincideConPreferencias, string? Etiqueta);
