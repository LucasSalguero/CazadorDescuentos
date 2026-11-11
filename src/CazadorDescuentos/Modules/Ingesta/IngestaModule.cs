using CazadorDescuentos.Modules.Ingesta.CargaManual;
using CazadorDescuentos.Modules.Ingesta.DatosDemo;
using CazadorDescuentos.Modules.Ingesta.EstructurarConLlm;
using CazadorDescuentos.Modules.Ingesta.ExtraccionDiaria;

namespace CazadorDescuentos.Modules.Ingesta;

internal static class IngestaModule
{
    public static IServiceCollection AddIngesta(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpcionesIngesta>(configuration.GetSection(OpcionesIngesta.Seccion));
        services.AddHttpClient<ClienteLlm>(http => http.Timeout = TimeSpan.FromMinutes(2));
        services.AddHttpClient(ExtraccionDiariaService.ClienteHttp, http =>
        {
            http.Timeout = TimeSpan.FromSeconds(30);
            http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; CazadorDescuentos/1.0)");
            http.DefaultRequestHeaders.AcceptLanguage.ParseAdd("es-AR,es;q=0.9");
        });
        services.AddHostedService<DatosDemoService>();
        services.AddHostedService<ExtraccionDiariaService>();
        return services;
    }

    public static IEndpointRouteBuilder MapIngesta(this IEndpointRouteBuilder app) =>
        app.MapCargaManual();
}
