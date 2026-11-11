using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Ingesta.EstructurarConLlm;
using Microsoft.Extensions.Options;

namespace CazadorDescuentos.Modules.Ingesta.ExtraccionDiaria;

internal sealed class ExtraccionDiariaService : BackgroundService
{
    public const string ClienteHttp = "Ingesta.Paginas";
    private static readonly TimeSpan Intervalo = TimeSpan.FromHours(24);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpFactory;
    private readonly IOptions<OpcionesIngesta> _opciones;
    private readonly ILogger<ExtraccionDiariaService> _logger;

    public ExtraccionDiariaService(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpFactory,
        IOptions<OpcionesIngesta> opciones,
        ILogger<ExtraccionDiariaService> logger)
    {
        _scopeFactory = scopeFactory;
        _httpFactory = httpFactory;
        _opciones = opciones;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(_opciones.Value.LlmApiKey))
        {
            _logger.LogWarning("Extracción diaria desactivada: falta configurar Ingesta:LlmApiKey.");
            return;
        }

        // Primera corrida al iniciar y después cada 24 h.
        using var timer = new PeriodicTimer(Intervalo);
        do
        {
            await EjecutarExtraccionAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    /// <summary>
    /// Por cada marca configurada descarga su página, la estructura con el LLM, descarta las ofertas inválidas
    /// y guarda el resto. Si una marca falla, las demás se procesan igual. Devuelve cuántas ofertas se guardaron.
    /// </summary>
    internal async Task<int> EjecutarExtraccionAsync(CancellationToken ct)
    {
        var guardadas = 0;
        foreach (var fuente in _opciones.Value.Marcas)
        {
            try
            {
                guardadas += await ProcesarMarcaAsync(fuente, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falló la extracción de {Marca} ({Url}). Se continúa con las demás.", fuente.Nombre, fuente.Url);
            }
        }

        _logger.LogInformation("Extracción terminada: {Cantidad} ofertas guardadas.", guardadas);
        return guardadas;
    }

    private async Task<int> ProcesarMarcaAsync(FuenteMarca fuente, CancellationToken ct)
    {
        var html = await _httpFactory.CreateClient(ClienteHttp).GetStringAsync(fuente.Url, ct);
        var texto = ExtractorTexto.DesdeHtml(html);

        await using var scope = _scopeFactory.CreateAsyncScope();
        var catalogo = scope.ServiceProvider.GetRequiredService<ICatalogo>();
        var llm = scope.ServiceProvider.GetRequiredService<ClienteLlm>();

        var ofertas = await llm.EstructurarAsync(fuente.Nombre, texto, fuente.Url, catalogo.Categorias, ct);
        var guardadas = 0;
        foreach (var oferta in ofertas)
        {
            var errores = ValidadorOferta.Validar(oferta, catalogo.Marcas, catalogo.Categorias);
            if (errores.Count > 0)
            {
                _logger.LogInformation("Oferta descartada de {Marca} ({Producto}): {Errores}",
                    fuente.Nombre, oferta.Producto, string.Join(" ", errores));
                continue;
            }

            await catalogo.GuardarOfertaAsync(oferta, ct);
            guardadas++;
        }

        _logger.LogInformation("{Marca}: {Guardadas} de {Total} ofertas guardadas.", fuente.Nombre, guardadas, ofertas.Count);
        return guardadas;
    }
}
