namespace CazadorDescuentos.Modules.CanjeVerificacion;

/// <summary>
/// Interfaz pública del módulo CanjeVerificacion.
/// </summary>
public interface IVerificacion
{
    /// <summary>Registra el voto. Devuelve false si el dispositivo ya votó esa oferta.</summary>
    Task<bool> RegistrarVotoAsync(int ofertaId, string deviceId, bool funciono, CancellationToken ct = default);

    Task<Semaforo> ObtenerSemaforoAsync(int ofertaId, CancellationToken ct = default);

    /// <summary>Semáforo junto con los votos que lo explican (para mostrarle al usuario de dónde sale).</summary>
    Task<ResumenVotos> ObtenerResumenAsync(int ofertaId, CancellationToken ct = default);

    /// <summary>Umbrales configurados en appsettings.json (sección Semaforo).</summary>
    ReglasSemaforo Reglas { get; }
}

public sealed record ResumenVotos(Semaforo Semaforo, int Positivos, int Negativos)
{
    public int Total => Positivos + Negativos;
}

public sealed record ReglasSemaforo(int MinimoVotos, int PorcentajeVerde, int PorcentajeRojo);

public enum Semaforo
{
    SinDatos,
    Verde,
    Amarillo,
    Rojo,
}
