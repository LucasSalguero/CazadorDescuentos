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
        // Las fotos se conservan como "[imagen: alt | src]" para que el LLM pueda asociarlas a cada oferta.
        var conImagenes = Imagenes().Replace(sinBloques, m => MarcadorImagen(m.Value));
        var sinEtiquetas = Etiquetas().Replace(conImagenes, " ");
        var texto = Espacios().Replace(WebUtility.HtmlDecode(sinEtiquetas), " ").Trim();
        return texto.Length <= MaximoCaracteres ? texto : texto[..MaximoCaracteres];
    }

    [GeneratedRegex(@"<(script|style|noscript|svg|head)\b[^>]*>.*?</\1>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex BloquesNoVisibles();

    private static string MarcadorImagen(string etiqueta)
    {
        var src = Atributo(etiqueta, "src");
        if (string.IsNullOrWhiteSpace(src) || src.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return " ";
        }

        var alt = Atributo(etiqueta, "alt") ?? string.Empty;
        return $" [imagen: {alt.Replace('[', '(').Replace(']', ')')} | {src}] ";
    }

    private static string? Atributo(string etiqueta, string nombre)
    {
        var m = Regex.Match(etiqueta, $@"\b{nombre}\s*=\s*(""(?<v>[^""]*)""|'(?<v>[^']*)')", RegexOptions.IgnoreCase);
        return m.Success ? m.Groups["v"].Value.Trim() : null;
    }

    [GeneratedRegex(@"<img\b[^>]*>", RegexOptions.IgnoreCase)]
    private static partial Regex Imagenes();

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex Etiquetas();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Espacios();
}
