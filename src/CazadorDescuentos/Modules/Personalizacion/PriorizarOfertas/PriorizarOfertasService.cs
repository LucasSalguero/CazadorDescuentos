using CazadorDescuentos.Modules.Catalogo;

namespace CazadorDescuentos.Modules.Personalizacion.PriorizarOfertas;

internal sealed class PriorizarOfertasService
{
    // TODO #9: ordenar primero las ofertas cuya categoría coincide con las preferencias
    // y asignar el texto de la etiqueta a cada una.
    public IReadOnlyList<OfertaPriorizada> Ejecutar(IReadOnlyList<OfertaDetalle> ofertas, IReadOnlyList<string> categoriasPreferidas) =>
        throw new NotImplementedException();
}
