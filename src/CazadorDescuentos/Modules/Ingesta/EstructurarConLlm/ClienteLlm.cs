using CazadorDescuentos.Modules.Catalogo;
using Microsoft.Extensions.Options;

namespace CazadorDescuentos.Modules.Ingesta.EstructurarConLlm;

internal sealed class ClienteLlm
{
    private readonly HttpClient _http;
    private readonly IOptions<OpcionesIngesta> _opciones;

    public ClienteLlm(HttpClient http, IOptions<OpcionesIngesta> opciones)
    {
        _http = http;
        _opciones = opciones;
    }

    // TODO #3: enviar el contenido de la página al LLM (clave en OpcionesIngesta.LlmApiKey)
    // y convertir su respuesta en ofertas estructuradas.
    public Task<IReadOnlyList<NuevaOferta>> EstructurarAsync(string marca, string contenido, CancellationToken ct) =>
        throw new NotImplementedException();
}
