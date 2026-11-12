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

    /// <summary>
    /// Mezcla tipos de descuento (porcentaje, 2x1, 3x2, segunda unidad, regalo, pago con QR, franja horaria)
    /// y montos (10 % a 50 %) para probar el feed. Las fotos son ilustrativas (ver wwwroot/img/ofertas/CREDITOS.txt).
    /// </summary>
    private static IEnumerable<NuevaOferta> Ofertas(DateOnly hoy, IReadOnlyDictionary<string, string> urls)
    {
        string Url(string marca) => urls.GetValueOrDefault(marca, "https://www.google.com/search?q=" + Uri.EscapeDataString(marca));
        static string Foto(string nombre) => $"/img/ofertas/{nombre}.jpg";

        // --- Mostaza ---
        yield return new("Mostaza", "Combo Mega Cuarto Deluxe", "Hamburguesas", "DEMO-MOSTA25", $"Válido solo pidiendo por la app. {Aviso}",
            Url("Mostaza"), 12900m, 9675m, 25m, hoy.AddDays(5), Foto("hamburguesa-doble"));
        yield return new("Mostaza", "Balde de pollo x8", "Pollo", null, Aviso,
            Url("Mostaza"), 18500m, 12950m, 30m, hoy.AddDays(2), Foto("pollo-frito"));
        yield return new("Mostaza", "Helado + Café", "Postres", "DEMO-MOSTAPOSTRE", null,
            Url("Mostaza"), 5200m, 3900m, 25m, hoy, Foto("helado-cono"));
        yield return new("Mostaza", "2x1 en Mostaza Doble", "Hamburguesas", "DEMO-2X1DOBLE", $"Martes y jueves. Llevás 2 y pagás 1. {Aviso}",
            Url("Mostaza"), 16800m, 8400m, 50m, hoy.AddDays(6), Foto("dos-hamburguesas"));
        yield return new("Mostaza", "Wrap de pollo crispy", "Pollo", null, $"Precio especial pagando con QR. {Aviso}",
            Url("Mostaza"), 7600m, 6460m, 15m, hoy.AddDays(8), Foto("wrap"));
        yield return new("Mostaza", "Licuado grande al precio del mediano", "Bebidas", null, Aviso,
            Url("Mostaza"), 4800m, 3900m, 19m, hoy.AddDays(4), Foto("licuado"));

        // --- McDonald's ---
        yield return new("McDonald's", "McCombo Big Mac mediano", "Hamburguesas", "DEMO-BIGMAC", $"Un uso por usuario. {Aviso}",
            Url("McDonald's"), 13500m, 8775m, 35m, hoy.AddDays(3), Foto("big-mac"));
        yield return new("McDonald's", "2x1 McFlurry", "Postres", "DEMO-2X1FLURRY", $"Solo en locales adheridos. {Aviso}",
            Url("McDonald's"), 9000m, 4500m, 50m, hoy.AddDays(7), Foto("copa-helada"));
        yield return new("McDonald's", "Coca-Cola grande", "Bebidas", null, Aviso,
            Url("McDonald's"), 3800m, 2850m, 25m, hoy.AddDays(10), Foto("gaseosa"));
        yield return new("McDonald's", "McCafé: latte + dona", "Postres", "DEMO-MCCAFE", $"De 8 a 12 h. {Aviso}",
            Url("McDonald's"), 6900m, 4830m, 30m, hoy.AddDays(3), Foto("donas"));
        yield return new("McDonald's", "3x2 en Cuarto de Libra", "Hamburguesas", "DEMO-3X2CUARTO", $"Solo delivery desde la app. {Aviso}",
            Url("McDonald's"), 29700m, 19800m, 33m, hoy.AddDays(5), Foto("mini-hamburguesas"));
        yield return new("McDonald's", "McNuggets x10 con papas de regalo", "Pollo", null, $"Las papas medianas van de regalo. {Aviso}",
            Url("McDonald's"), 11300m, 8200m, 27m, hoy.AddDays(2), Foto("tiras-pollo"));
        yield return new("McDonald's", "Café latte grande", "Bebidas", "DEMO-LATTE10", $"Pagando con QR. {Aviso}",
            Url("McDonald's"), 3500m, 3150m, 10m, hoy.AddDays(12), Foto("cafe"));

        // --- Burger King ---
        yield return new("Burger King", "Combo Whopper", "Hamburguesas", "DEMO-WHOPPER40", $"Válido de 18 a 23 h. {Aviso}",
            Url("Burger King"), 14200m, 8520m, 40m, hoy.AddDays(4), Foto("combo-papas"));
        yield return new("Burger King", "Nuggets x10", "Pollo", "DEMO-NUGGETS", Aviso,
            Url("Burger King"), 8900m, 6230m, 30m, hoy.AddDays(1), Foto("pollo-crocante"));
        yield return new("Burger King", "Long Chicken con papas", "Pollo", "DEMO-LONGCHICKEN", Aviso,
            Url("Burger King"), 12400m, 9920m, 20m, hoy.AddDays(6), Foto("hamburguesa-pollo"));
        yield return new("Burger King", "King de Queso: segunda unidad al 50%", "Hamburguesas", null, $"Llevando dos unidades iguales. {Aviso}",
            Url("Burger King"), 12000m, 9000m, 25m, hoy.AddDays(9), Foto("hamburguesa-clasica"));
        yield return new("Burger King", "Whopper Veggie", "Hamburguesas", "DEMO-VEGGIE", Aviso,
            Url("Burger King"), 9800m, 6860m, 30m, hoy.AddDays(4), Foto("hamburguesa-veggie"));
        yield return new("Burger King", "Bacon King", "Hamburguesas", "DEMO-BACON45", $"Pagando con QR desde la app. {Aviso}",
            Url("Burger King"), 13800m, 7590m, 45m, hoy, Foto("hamburguesa-bacon"));
        yield return new("Burger King", "2x1 en gaseosas grandes", "Bebidas", null, $"Con cualquier combo. {Aviso}",
            Url("Burger King"), 7200m, 3600m, 50m, hoy.AddDays(3), Foto("gaseosas-hielo"));
        yield return new("Burger King", "Stacker cuádruple", "Hamburguesas", null, Aviso,
            Url("Burger King"), 16000m, 12000m, 25m, hoy.AddDays(-1), Foto("hamburguesa-cuadruple")); // vencida: no debe aparecer en el feed
    }
}
