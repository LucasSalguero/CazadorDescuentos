namespace CazadorDescuentos.Modules.CanjeVerificacion.RegistrarVoto;

internal static class RegistrarVotoEndpoint
{
    public static IEndpointRouteBuilder MapRegistrarVoto(this IEndpointRouteBuilder app)
    {
        app.MapPost("/votos", ManejarAsync);
        return app;
    }

    // TODO #13: registrar el voto con RegistrarVotoService y responder 409 si el dispositivo ya votó esa oferta.
    internal static Task<IResult> ManejarAsync(SolicitudVoto solicitud, RegistrarVotoService registrarVoto, CancellationToken ct) =>
        throw new NotImplementedException();
}

internal sealed record SolicitudVoto(int OfertaId, string DeviceId, bool Funciono);
