using CazadorDescuentos.Modules.CanjeVerificacion.CalcularSemaforo;
using CazadorDescuentos.Modules.CanjeVerificacion.RegistrarVoto;

namespace CazadorDescuentos.Modules.CanjeVerificacion;

internal static class CanjeVerificacionModule
{
    public static IServiceCollection AddCanjeVerificacion(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpcionesSemaforo>(configuration.GetSection(OpcionesSemaforo.Seccion));
        services.AddScoped<RegistrarVotoService>();
        services.AddScoped<CalcularSemaforoService>();
        services.AddScoped<IVerificacion, VerificacionService>();
        return services;
    }

    public static IEndpointRouteBuilder MapCanjeVerificacion(this IEndpointRouteBuilder app) =>
        app.MapRegistrarVoto();
}
