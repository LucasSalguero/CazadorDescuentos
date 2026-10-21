namespace CazadorDescuentos.Modules.CanjeVerificacion;

internal sealed class Voto
{
    public int Id { get; set; }
    public int OfertaId { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public bool Funciono { get; set; }
    public DateTime FechaUtc { get; set; }
}
