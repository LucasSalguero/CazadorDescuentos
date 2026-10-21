using CazadorDescuentos.Modules.Catalogo;
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

    // TODO #2: rechazar con 401 si el header X-Api-Key no coincide con OpcionesIngesta.CargaManualApiKey,
    // validar la oferta con ValidadorOferta y guardarla con ICatalogo.
    internal static Task<IResult> ManejarAsync(
        NuevaOferta oferta,
        [FromHeader(Name = HeaderApiKey)] string? apiKey,
        ICatalogo catalogo,
        IOptions<OpcionesIngesta> opciones,
        CancellationToken ct) =>
        throw new NotImplementedException();
}
