using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CazadorDescuentos.Modules.Catalogo;
using Microsoft.Extensions.Options;

namespace CazadorDescuentos.Modules.Ingesta.EstructurarConLlm;

/// <summary>
/// Convierte el texto de una página de promociones en ofertas estructuradas usando un LLM
/// con API compatible con OpenAI Chat Completions (OpenAI, Groq, OpenRouter, Gemini...).
/// </summary>
internal sealed class ClienteLlm
{
    private static readonly JsonSerializerOptions OpcionesJson = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;
    private readonly IOptions<OpcionesIngesta> _opciones;

    public ClienteLlm(HttpClient http, IOptions<OpcionesIngesta> opciones)
    {
        _http = http;
        _opciones = opciones;
    }

    public async Task<IReadOnlyList<NuevaOferta>> EstructurarAsync(
        string marca,
        string contenido,
        string enlacePorDefecto,
        IReadOnlyList<string> categorias,
        CancellationToken ct)
    {
        var opciones = _opciones.Value;
        if (string.IsNullOrWhiteSpace(opciones.LlmApiKey))
        {
            throw new InvalidOperationException("Falta configurar Ingesta:LlmApiKey.");
        }

        var solicitud = new
        {
            model = opciones.LlmModelo,
            temperature = 0,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new { role = "system", content = PromptSistema(categorias) },
                new { role = "user", content = $"Marca: {marca}\n\nContenido de la página:\n{contenido}" },
            },
        };

        using var mensaje = new HttpRequestMessage(HttpMethod.Post, opciones.LlmUrl)
        {
            Content = JsonContent.Create(solicitud),
        };
        mensaje.Headers.Authorization = new AuthenticationHeaderValue("Bearer", opciones.LlmApiKey);

        using var respuesta = await _http.SendAsync(mensaje, ct);
        respuesta.EnsureSuccessStatusCode();

        var cuerpo = await respuesta.Content.ReadFromJsonAsync<RespuestaChat>(OpcionesJson, ct);
        var json = cuerpo?.Choices.FirstOrDefault()?.Message?.Content;
        return string.IsNullOrWhiteSpace(json) ? [] : Interpretar(json, marca, enlacePorDefecto);
    }

    /// <summary>Convierte el JSON del LLM en ofertas. Las ofertas mal formadas se descartan (no se inventan datos).</summary>
    internal static IReadOnlyList<NuevaOferta> Interpretar(string json, string marca, string enlacePorDefecto)
    {
        SalidaLlm? salida;
        try
        {
            salida = JsonSerializer.Deserialize<SalidaLlm>(LimpiarBloqueCodigo(json), OpcionesJson);
        }
        catch (JsonException)
        {
            return [];
        }

        var resultado = new List<NuevaOferta>();
        foreach (var o in salida?.Ofertas ?? [])
        {
            if (!DateOnly.TryParseExact(o.Vencimiento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var vencimiento))
            {
                vencimiento = default; // el validador la descarta
            }

            resultado.Add(new NuevaOferta(
                Marca: string.IsNullOrWhiteSpace(o.Marca) ? marca : o.Marca,
                Producto: o.Producto ?? string.Empty,
                Categoria: o.Categoria ?? string.Empty,
                Codigo: string.IsNullOrWhiteSpace(o.Codigo) ? null : o.Codigo,
                Restricciones: string.IsNullOrWhiteSpace(o.Restricciones) ? null : o.Restricciones,
                EnlaceComercio: string.IsNullOrWhiteSpace(o.EnlaceComercio) ? enlacePorDefecto : o.EnlaceComercio,
                PrecioOriginal: o.PrecioOriginal ?? 0,
                PrecioPromocional: o.PrecioPromocional ?? 0,
                PorcentajeDescuento: o.PorcentajeDescuento ?? 0,
                Vencimiento: vencimiento));
        }

        return resultado;
    }

    internal static string PromptSistema(IReadOnlyList<string> categorias) => $$"""
        Sos un extractor de promociones de comida rápida de Argentina.
        Recibís el texto de la página de una marca y devolvés SOLO un objeto JSON con esta forma:
        {"ofertas":[{"marca":string,"producto":string,"categoria":string,"precioOriginal":number|null,
        "precioPromocional":number|null,"porcentajeDescuento":number|null,"codigo":string|null,
        "vencimiento":"yyyy-MM-dd"|null,"restricciones":string|null,"enlaceComercio":string|null}]}
        Reglas:
        - Usá solo datos que aparezcan en el texto. Si un dato no está, poné null. No inventes nada.
        - "restricciones": la letra chica tal como aparece; si la fuente no menciona condiciones, null.
        - "categoria" debe ser exactamente una de: {{string.Join(", ", categorias)}}.
        - Los precios son en pesos argentinos, como número sin símbolos ni separadores de miles.
        - Si no hay promociones, devolvé {"ofertas":[]}.
        """;

    private static string LimpiarBloqueCodigo(string texto)
    {
        var t = texto.Trim();
        if (!t.StartsWith("```", StringComparison.Ordinal))
        {
            return t;
        }

        var inicio = t.IndexOf('\n');
        var fin = t.LastIndexOf("```", StringComparison.Ordinal);
        return inicio >= 0 && fin > inicio ? t[(inicio + 1)..fin] : t;
    }

    private sealed record RespuestaChat(List<Opcion> Choices);

    private sealed record Opcion(MensajeChat? Message);

    private sealed record MensajeChat(string? Content);

    internal sealed record SalidaLlm(List<OfertaLlm>? Ofertas);

    internal sealed record OfertaLlm(
        string? Marca,
        string? Producto,
        string? Categoria,
        decimal? PrecioOriginal,
        decimal? PrecioPromocional,
        decimal? PorcentajeDescuento,
        string? Codigo,
        string? Vencimiento,
        string? Restricciones,
        string? EnlaceComercio);
}
