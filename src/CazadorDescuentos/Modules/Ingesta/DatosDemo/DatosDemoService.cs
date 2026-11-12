using CazadorDescuentos.Modules.Catalogo;
using Microsoft.Extensions.Options;

namespace CazadorDescuentos.Modules.Ingesta.DatosDemo;

/// <summary>
/// Carga ofertas DE EJEMPLO para poder ver el frontend sin LLM. Solo corre si Ingesta:CargarDatosDemo es true
/// (activado en appsettings.Development.json). Los precios y códigos son inventados.
/// Como usa el upsert del catálogo, reiniciar la app no duplica ofertas.
/// </summary>
internal sealed class DatosDemoService(
    IServiceScopeFactory scopeFactory,
    IOptions<OpcionesIngesta> opciones,
    TimeProvider reloj,
    ILogger<DatosDemoService> logger) : IHostedService
{
    private const string Aviso = "Oferta de ejemplo (datos demo, no es una promoción real).";

    public async Task StartAsync(CancellationToken ct)
    {
        if (!opciones.Value.CargarDatosDemo)
        {
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var catalogo = scope.ServiceProvider.GetRequiredService<ICatalogo>();
        var hoy = DateOnly.FromDateTime(reloj.GetUtcNow().UtcDateTime.AddHours(-3));
        var urls = opciones.Value.Marcas.ToDictionary(m => m.Nombre, m => m.Url, StringComparer.OrdinalIgnoreCase);

        foreach (var oferta in Ofertas(hoy, urls))
        {
            await catalogo.GuardarOfertaAsync(oferta, ct);
        }

        logger.LogInformation("Datos demo cargados.");
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    private static IEnumerable<NuevaOferta> Ofertas(DateOnly hoy, IReadOnlyDictionary<string, string> urls)
    {
        string Url(string marca) => urls.GetValueOrDefault(marca, "https://www.google.com/search?q=" + Uri.EscapeDataString(marca));

        yield return new("Mostaza", "Combo Mega Cuarto Deluxe", "Hamburguesas", "DEMO-MOSTA25", $"Válido solo pidiendo por la app. {Aviso}",
            Url("Mostaza"), 12900m, 9675m, 25m, hoy.AddDays(5));
        yield return new("Mostaza", "Balde de pollo x8", "Pollo", null, Aviso,
            Url("Mostaza"), 18500m, 12950m, 30m, hoy.AddDays(2));
        yield return new("Mostaza", "Helado + Café", "Postres", "DEMO-MOSTAPOSTRE", null,
            Url("Mostaza"), 5200m, 3900m, 25m, hoy);

        yield return new("McDonald's", "McCombo Big Mac mediano", "Hamburguesas", "DEMO-BIGMAC", $"Un uso por usuario. {Aviso}",
            Url("McDonald's"), 13500m, 8775m, 35m, hoy.AddDays(3));
        yield return new("McDonald's", "2x1 McFlurry", "Postres", "DEMO-2X1FLURRY", $"Solo en locales adheridos. {Aviso}",
            Url("McDonald's"), 9000m, 4500m, 50m, hoy.AddDays(7));
        yield return new("McDonald's", "Coca-Cola grande", "Bebidas", null, Aviso,
            Url("McDonald's"), 3800m, 2850m, 25m, hoy.AddDays(10));

        yield return new("Burger King", "Combo Whopper", "Hamburguesas", "DEMO-WHOPPER40", $"Válido de 18 a 23 h. {Aviso}",
            Url("Burger King"), 14200m, 8520m, 40m, hoy.AddDays(4));
        yield return new("Burger King", "Nuggets x10", "Pollo", "DEMO-NUGGETS", Aviso,
            Url("Burger King"), 8900m, 6230m, 30m, hoy.AddDays(1));
        yield return new("Burger King", "Stacker cuádruple", "Hamburguesas", null, Aviso,
            Url("Burger King"), 16000m, 12000m, 25m, hoy.AddDays(-1)); // vencida: no debe aparecer en el feed
    }
}
