namespace CazadorDescuentos.Modules.Ingesta;

internal sealed class OpcionesIngesta
{
    public const string Seccion = "Ingesta";

    /// <summary>Marcas a extraer. Hoy: Mostaza, McDonald's y Burger King.</summary>
    public List<FuenteMarca> Marcas { get; set; } = [];

    /// <summary>Endpoint compatible con OpenAI Chat Completions.</summary>
    public string LlmUrl { get; set; } = "https://api.openai.com/v1/chat/completions";

    public string LlmModelo { get; set; } = "gpt-4o-mini";

    /// <summary>Carga ofertas de ejemplo al iniciar (pensado solo para Development).</summary>
    public bool CargarDatosDemo { get; set; }

    // LlmApiKey y CargaManualApiKey NO van en ningún archivo del repositorio.
    // Desarrollo (desde src/CazadorDescuentos):
    //   dotnet user-secrets set "Ingesta:LlmApiKey" "<clave>"
    //   dotnet user-secrets set "Ingesta:CargaManualApiKey" "<clave>"
    // Producción: variables de entorno Ingesta__LlmApiKey e Ingesta__CargaManualApiKey.
    public string? LlmApiKey { get; set; }
    public string? CargaManualApiKey { get; set; }
}

internal sealed class FuenteMarca
{
    public string Nombre { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
