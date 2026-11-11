using System.Security.Cryptography;
using System.Text;
using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Ingesta.EstructurarConLlm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CazadorDescuentos.Modules.Ingesta.CargaManual;

internal static class CargaManualEndpoint
{
    public const string HeaderApiKey = "X-Api-Key";

    public static IEndpointRouteBuilder MapCargaManual(this IEndpointRouteBuilder app)
    {
        app.MapPost("/admin/ofertas", ManejarAsync);
        return app;
    }

    /// <summary>
    /// Carga manual de contingencia. Ejemplo:
    /// curl -X POST https://localhost:7244/admin/ofertas -H "X-Api-Key: &lt;clave&gt;" -H "Content-Type: application/json" -d @oferta.json
    /// </summary>
    internal static async Task<IResult> ManejarAsync(
        NuevaOferta oferta,
        [FromHeader(Name = HeaderApiKey)] string? apiKey,
        ICatalogo catalogo,
        IOptions<OpcionesIngesta> opciones,
        CancellationToken ct)
    {
        if (!ClaveValida(apiKey, opciones.Value.CargaManualApiKey))
        {
            return Results.Unauthorized();
        }

        var errores = ValidadorOferta.Validar(oferta, catalogo.Marcas, catalogo.Categorias);
        if (errores.Count > 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["oferta"] = [.. errores] });
        }

        var id = await catalogo.GuardarOfertaAsync(oferta, ct);
        return Results.Created($"/Ofertas/Detalle/{id}", new { id });
    }

    private static bool ClaveValida(string? recibida, string? configurada) =>
        !string.IsNullOrEmpty(configurada) &&
        !string.IsNullOrEmpty(recibida) &&
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(recibida), Encoding.UTF8.GetBytes(configurada));
}
