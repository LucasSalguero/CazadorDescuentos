using CazadorDescuentos.Modules.Ingesta.CargaManual;
using CazadorDescuentos.Modules.Ingesta.EstructurarConLlm;
using CazadorDescuentos.Modules.Ingesta.ExtraccionDiaria;

namespace CazadorDescuentos.Modules.Ingesta;

internal static class IngestaModule
{
    public static IServiceCollection AddIngesta(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpcionesIngesta>(configuration.GetSection(OpcionesIngesta.Seccion));
        services.AddHttpClient<ClienteLlm>();
        services.AddHostedService<ExtraccionDiariaService>();
        return services;
    }

    public static IEndpointRouteBuilder MapIngesta(this IEndpointRouteBuilder app) =>
        app.MapCargaManual();
}
