using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Personalizacion.Preferencias;
using CazadorDescuentos.Modules.Personalizacion.PriorizarOfertas;

namespace CazadorDescuentos.Modules.Personalizacion;

/// <summary>Implementación de IPersonalizacion: delega cada operación en su caso de uso.</summary>
internal sealed class PersonalizacionService(
    PreferenciasService preferencias,
    PriorizarOfertasService priorizarOfertas) : IPersonalizacion
{
    public IReadOnlyList<string> ObtenerPreferencias(HttpRequest request) =>
        preferencias.Leer(request);

    public void GuardarPreferencias(HttpResponse response, IReadOnlyList<string> categorias) =>
        preferencias.Escribir(response, categorias);

    public IReadOnlyList<OfertaPriorizada> Priorizar(IReadOnlyList<OfertaDetalle> ofertas, IReadOnlyList<string> categoriasPreferidas) =>
        priorizarOfertas.Ejecutar(ofertas, categoriasPreferidas);
}
