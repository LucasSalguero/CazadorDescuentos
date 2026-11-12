using CazadorDescuentos.Modules.Catalogo;

namespace CazadorDescuentos.Modules.Personalizacion.PriorizarOfertas;

internal sealed class PriorizarOfertasService
{
    /// <summary>
    /// Pone primero las ofertas cuya categoría coincide con alguna preferencia y les agrega la etiqueta
    /// "Coincide con tu preferencia por #Categoría". El resto queda debajo. Dentro de cada grupo se respeta
    /// el orden recibido (orden por defecto del catálogo), así que sin preferencias el feed no cambia.
    /// </summary>
    public IReadOnlyList<OfertaPriorizada> Ejecutar(IReadOnlyList<OfertaDetalle> ofertas, IReadOnlyList<string> categoriasPreferidas)
    {
        var preferidas = new HashSet<string>(categoriasPreferidas, StringComparer.OrdinalIgnoreCase);

        return ofertas
            .Select(o => preferidas.Contains(o.Categoria)
                ? new OfertaPriorizada(o, true, Etiqueta(o.Categoria))
                : new OfertaPriorizada(o, false, null))
            .OrderByDescending(p => p.CoincideConPreferencias) // OrderBy es estable en LINQ
            .ToList();
    }

    internal static string Etiqueta(string categoria) => $"Coincide con tu preferencia por #{categoria}";
}
