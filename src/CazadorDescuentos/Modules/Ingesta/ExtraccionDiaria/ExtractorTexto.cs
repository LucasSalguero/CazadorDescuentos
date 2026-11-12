using System.Net;
using System.Text.RegularExpressions;

namespace CazadorDescuentos.Modules.Ingesta.ExtraccionDiaria;

/// <summary>Reduce el HTML de una página a texto plano para enviarle menos tokens al LLM.</summary>
internal static partial class ExtractorTexto
{
    public const int MaximoCaracteres = 30_000;

    public static string DesdeHtml(string html)
    {
        var sinBloques = BloquesNoVisibles().Replace(html, " ");
        var sinEtiquetas = Etiquetas().Replace(sinBloques, " ");
        var texto = Espacios().Replace(WebUtility.HtmlDecode(sinEtiquetas), " ").Trim();
        return texto.Length <= MaximoCaracteres ? texto : texto[..MaximoCaracteres];
    }

    [GeneratedRegex(@"<(script|style|noscript|svg|head)\b[^>]*>.*?</\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex BloquesNoVisibles();

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex Etiquetas();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Espacios();
}
