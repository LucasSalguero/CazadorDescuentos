namespace CazadorDescuentos.Modules.CanjeVerificacion;

/// <summary>
/// Interfaz pública del módulo CanjeVerificacion.
/// </summary>
public interface IVerificacion
{
    /// <summary>Registra el voto. Devuelve false si el dispositivo ya votó esa oferta.</summary>
    Task<bool> RegistrarVotoAsync(int ofertaId, string deviceId, bool funciono, CancellationToken ct = default);

    Task<Semaforo> ObtenerSemaforoAsync(int ofertaId, CancellationToken ct = default);
}

public enum Semaforo
{
    SinDatos,
    Verde,
    Amarillo,
    Rojo,
}
