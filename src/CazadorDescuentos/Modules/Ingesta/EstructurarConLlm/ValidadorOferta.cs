using CazadorDescuentos.Modules.Catalogo;

namespace CazadorDescuentos.Modules.Ingesta.EstructurarConLlm;

internal static class ValidadorOferta
{
    /// <summary>
    /// Valida los campos obligatorios (marca, producto, precio promocional y vencimiento) y la coherencia de los datos.
    /// Devuelve la lista de errores: vacía si la oferta es válida.
    /// </summary>
    /// <param name="marcasValidas">Marcas soportadas (ICatalogo.Marcas).</param>
    /// <param name="categoriasValidas">Categorías soportadas (ICatalogo.Categorias).</param>
    public static IReadOnlyList<string> Validar(
        NuevaOferta oferta,
        IReadOnlyCollection<string> marcasValidas,
        IReadOnlyCollection<string> categoriasValidas)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(oferta.Marca))
        {
            errores.Add("La marca es obligatoria.");
        }
        else if (!marcasValidas.Contains(oferta.Marca.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            errores.Add($"La marca '{oferta.Marca}' no está soportada. Marcas válidas: {string.Join(", ", marcasValidas)}.");
        }

        if (string.IsNullOrWhiteSpace(oferta.Producto))
        {
            errores.Add("El producto es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(oferta.Categoria) ||
            !categoriasValidas.Contains(oferta.Categoria.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            errores.Add($"La categoría debe ser una de: {string.Join(", ", categoriasValidas)}.");
        }

        if (oferta.PrecioPromocional <= 0)
        {
            errores.Add("El precio promocional es obligatorio y debe ser mayor a 0.");
        }

        if (oferta.PrecioOriginal < 0)
        {
            errores.Add("El precio original no puede ser negativo.");
        }
        else if (oferta.PrecioOriginal > 0 && oferta.PrecioPromocional > oferta.PrecioOriginal)
        {
            errores.Add("El precio promocional no puede ser mayor al precio original.");
        }

        if (oferta.PorcentajeDescuento is < 0 or > 100)
        {
            errores.Add("El porcentaje de descuento debe estar entre 0 y 100.");
        }

        if (oferta.Vencimiento == default)
        {
            errores.Add("El vencimiento es obligatorio.");
        }

        if (!Uri.TryCreate(oferta.EnlaceComercio, UriKind.Absolute, out var enlace) ||
            (enlace.Scheme != Uri.UriSchemeHttps && enlace.Scheme != Uri.UriSchemeHttp))
        {
            errores.Add("El enlace del comercio debe ser una URL http(s) válida.");
        }

        if (!string.IsNullOrWhiteSpace(oferta.ImagenUrl) && !ImagenValida(oferta.ImagenUrl.Trim()))
        {
            errores.Add("La imagen debe ser una URL http(s) válida o una ruta local que empiece con /img/.");
        }

        return errores;
    }

    private static bool ImagenValida(string imagen) =>
        imagen.StartsWith("/img/", StringComparison.Ordinal) && !imagen.Contains("..", StringComparison.Ordinal) ||
        Uri.TryCreate(imagen, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
}
