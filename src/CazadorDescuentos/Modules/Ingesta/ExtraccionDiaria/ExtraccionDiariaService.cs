using Microsoft.Extensions.Options;

namespace CazadorDescuentos.Modules.Ingesta.ExtraccionDiaria;

internal sealed class ExtraccionDiariaService : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromHours(24);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<OpcionesIngesta> _opciones;
    private readonly ILogger<ExtraccionDiariaService> _logger;

    public ExtraccionDiariaService(
        IServiceScopeFactory scopeFactory,
        IOptions<OpcionesIngesta> opciones,
        ILogger<ExtraccionDiariaService> logger)
    {
        _scopeFactory = scopeFactory;
        _opciones = opciones;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Intervalo);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await EjecutarExtraccionAsync(stoppingToken);
        }
    }

    // TODO #1: por cada marca configurada, descargar su página, estructurarla con ClienteLlm,
    // descartar las ofertas inválidas (ValidadorOferta) y guardar el resto con ICatalogo (en un scope nuevo).
    internal Task EjecutarExtraccionAsync(CancellationToken ct) =>
        throw new NotImplementedException();
}
