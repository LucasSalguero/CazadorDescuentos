using CazadorDescuentos.Data;
using CazadorDescuentos.Modules.CanjeVerificacion;
using CazadorDescuentos.Modules.CanjeVerificacion.CalcularSemaforo;
using CazadorDescuentos.Modules.CanjeVerificacion.RegistrarVoto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace CazadorDescuentos.Tests.Integracion;

/// <summary>CanjeVerificacion contra EF Core + SQLite real (#13 y #14).</summary>
public sealed class VerificacionIntegracionTests : IDisposable
{
    private readonly BaseSqlite _base = new();

    public void Dispose() => _base.Dispose();

    private static readonly IOptions<OpcionesSemaforo> Opciones =
        Options.Create(new OpcionesSemaforo { MinimoVotos = 3, PorcentajeVerde = 70, PorcentajeRojo = 40 });

    private IVerificacion Verificacion(AppDbContext db) => new VerificacionService(
        new RegistrarVotoService(db, _base.Reloj),
        new CalcularSemaforoService(db, Opciones),
        Opciones);

    [Fact]
    public async Task Segundo_voto_del_mismo_dispositivo_para_la_misma_oferta_devuelve_false()
    {
        using var db = _base.CrearContexto();
        var verificacion = Verificacion(db);

        Assert.True(await verificacion.RegistrarVotoAsync(1, "dispositivo-a", true));
        Assert.False(await verificacion.RegistrarVotoAsync(1, "dispositivo-a", false));
        Assert.False(await verificacion.RegistrarVotoAsync(1, " dispositivo-a ", true));
        Assert.True(await verificacion.RegistrarVotoAsync(2, "dispositivo-a", true));
        Assert.True(await verificacion.RegistrarVotoAsync(1, "dispositivo-b", true));

        Assert.Equal(3, await db.Votos.CountAsync());
    }

    [Fact]
    public async Task Voto_simultaneo_rechazado_por_el_indice_unico_devuelve_false_y_no_deja_el_contexto_roto()
    {
        // Simula otro request que inserta el mismo voto entre la verificación y el SaveChanges.
        using var db = _base.CrearContexto(new InsertarVotoAntesDeGuardar(_base));
        var verificacion = Verificacion(db);

        Assert.False(await verificacion.RegistrarVotoAsync(1, "dispositivo-a", true));

        // El mismo contexto (mismo request) tiene que poder seguir usándose.
        Assert.True(await verificacion.RegistrarVotoAsync(1, "dispositivo-b", true));
        Assert.Equal(2, await db.Votos.CountAsync());
    }

    [Fact]
    public async Task Semaforo_con_3_votos_positivos_es_verde_y_sin_votos_suficientes_es_sin_datos()
    {
        using var db = _base.CrearContexto();
        var verificacion = Verificacion(db);

        await verificacion.RegistrarVotoAsync(1, "a", true);
        await verificacion.RegistrarVotoAsync(1, "b", true);
        Assert.Equal(Semaforo.SinDatos, await verificacion.ObtenerSemaforoAsync(1));

        await verificacion.RegistrarVotoAsync(1, "c", true);
        Assert.Equal(Semaforo.Verde, await verificacion.ObtenerSemaforoAsync(1));
        Assert.Equal(Semaforo.SinDatos, await verificacion.ObtenerSemaforoAsync(2));
        Assert.Equal(new ResumenVotos(Semaforo.Verde, 3, 0), await verificacion.ObtenerResumenAsync(1));
        Assert.Equal(new ReglasSemaforo(3, 70, 40), verificacion.Reglas);
    }

    [Fact]
    public async Task Semaforo_rojo_y_amarillo_con_votos_reales()
    {
        using var db = _base.CrearContexto();
        var verificacion = Verificacion(db);

        foreach (var (device, funciono) in new[] { ("a", false), ("b", false), ("c", true) })
        {
            await verificacion.RegistrarVotoAsync(1, device, funciono);
        }

        foreach (var (device, funciono) in new[] { ("a", true), ("b", true), ("c", false) })
        {
            await verificacion.RegistrarVotoAsync(2, device, funciono);
        }

        Assert.Equal(Semaforo.Rojo, await verificacion.ObtenerSemaforoAsync(1));
        Assert.Equal(Semaforo.Amarillo, await verificacion.ObtenerSemaforoAsync(2));
    }

    private sealed class InsertarVotoAntesDeGuardar(BaseSqlite baseSqlite) : SaveChangesInterceptor
    {
        private bool _hecho;

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
        {
            if (!_hecho)
            {
                _hecho = true;
                using var otro = baseSqlite.CrearContexto();
                otro.Votos.Add(new Voto { OfertaId = 1, DeviceId = "dispositivo-a", Funciono = false, FechaUtc = DateTime.UtcNow });
                await otro.SaveChangesAsync(ct);
            }

            return result;
        }
    }
}
