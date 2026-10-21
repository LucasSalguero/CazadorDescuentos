using CazadorDescuentos.Modules.Personalizacion.Preferencias;
using CazadorDescuentos.Modules.Personalizacion.PriorizarOfertas;

namespace CazadorDescuentos.Modules.Personalizacion;

internal static class PersonalizacionModule
{
    public static IServiceCollection AddPersonalizacion(this IServiceCollection services)
    {
        services.AddSingleton<PreferenciasService>();
        services.AddSingleton<PriorizarOfertasService>();
        services.AddSingleton<IPersonalizacion, PersonalizacionService>();
        return services;
    }
}
