using CazadorDescuentos.Modules.Catalogo;

namespace CazadorDescuentos.Modules.CanjeVerificacion.RegistrarVoto;

internal static class RegistrarVotoEndpoint
{
    public const int LargoMaximoDeviceId = 64;

    public static IEndpointRouteBuilder MapRegistrarVoto(this IEndpointRouteBuilder app)
    {
        app.MapPost("/votos", ManejarAsync);
        return app;
    }

    /// <summary>Registra el voto. 404 si la oferta no existe y 409 si el dispositivo ya votó esa oferta.</summary>
    internal static async Task<IResult> ManejarAsync(
        SolicitudVoto solicitud,
        RegistrarVotoService registrarVoto,
        ICatalogo catalogo,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(solicitud.DeviceId) || solicitud.DeviceId.Length > LargoMaximoDeviceId)
        {
            return Results.BadRequest(new { error = "DeviceId inválido." });
        }

        if (await catalogo.ObtenerPorIdAsync(solicitud.OfertaId, ct) is null)
        {
            return Results.NotFound();
        }

        var registrado = await registrarVoto.EjecutarAsync(solicitud.OfertaId, solicitud.DeviceId, solicitud.Funciono, ct);
        return registrado
            ? Results.Ok(new { registrado = true })
            : Results.Conflict(new { error = "Este dispositivo ya votó esta oferta." });
    }
}

internal sealed record SolicitudVoto(int OfertaId, string DeviceId, bool Funciono);
