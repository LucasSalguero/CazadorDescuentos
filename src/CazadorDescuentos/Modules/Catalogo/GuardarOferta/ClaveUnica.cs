using System.Globalization;
using System.Text;

namespace CazadorDescuentos.Modules.Catalogo.GuardarOferta;

internal static class ClaveUnica
{
    /// <summary>
    /// Combina el comercio (marca) y el título (producto) normalizados:
    /// minúsculas, sin tildes, sin signos y con un solo guion entre palabras.
    /// Ej.: ("McDonald's", "  Combo Big Mac ¡Mediano! ") → "mcdonalds|combo-big-mac-mediano".
    /// </summary>
    public static string Generar(string marca, string producto)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(marca);
        ArgumentException.ThrowIfNullOrWhiteSpace(producto);

        return $"{Normalizar(marca)}|{Normalizar(producto)}";
    }

    internal static string Normalizar(string texto)
    {
        var descompuesto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var resultado = new StringBuilder(descompuesto.Length);
        var separadorPendiente = false;

        foreach (var c in descompuesto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
            {
                continue; // tildes y diéresis
            }

            if (char.IsLetterOrDigit(c))
            {
                if (separadorPendiente && resultado.Length > 0)
                {
                    resultado.Append('-');
                }

                resultado.Append(c);
                separadorPendiente = false;
            }
            else if (char.IsWhiteSpace(c) || c is '-' or '_' or '/' or '+')
            {
                separadorPendiente = true;
            }
            // El resto de los signos (apóstrofos, ¡!, %, $, etc.) se descartan sin separar.
        }

        return resultado.ToString().Normalize(NormalizationForm.FormC);
    }
}
