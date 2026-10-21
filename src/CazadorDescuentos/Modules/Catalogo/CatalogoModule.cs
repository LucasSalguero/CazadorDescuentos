using CazadorDescuentos.Modules.Catalogo.GuardarOferta;
using CazadorDescuentos.Modules.Catalogo.ListarOfertasActivas;
using CazadorDescuentos.Modules.Catalogo.ObtenerOferta;

namespace CazadorDescuentos.Modules.Catalogo;

internal static class CatalogoModule
{
    public static IServiceCollection AddCatalogo(this IServiceCollection services)
    {
        services.AddScoped<GuardarOfertaService>();
        services.AddScoped<ListarOfertasActivasService>();
        services.AddScoped<ObtenerOfertaService>();
        services.AddScoped<ICatalogo, CatalogoService>();
        return services;
    }
}
