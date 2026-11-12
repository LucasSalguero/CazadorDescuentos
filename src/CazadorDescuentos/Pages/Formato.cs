using System.Globalization;
using CazadorDescuentos.Modules.CanjeVerificacion;

namespace CazadorDescuentos.Pages;

/// <summary>Formatos de presentación para Argentina: pesos y fechas dd/mm/aaaa en hora local (UTC-3).</summary>
public static class Formato
{
    private static readonly CultureInfo Argentina = CultureInfo.GetCultureInfo("es-AR");
    private static readonly TimeSpan OffsetArgentina = TimeSpan.FromHours(-3);

    public static string Pesos(decimal monto) => "$ " + monto.ToString("#,##0.##", Argentina);

    public static string Porcentaje(decimal porcentaje) => porcentaje.ToString("0.##", Argentina) + "%";

    public static DateTime AHoraArgentina(DateTime utc) =>
        DateTime.SpecifyKind(utc, DateTimeKind.Utc).Add(OffsetArgentina);

    public static string Fecha(DateTime utc) => AHoraArgentina(utc).ToString("dd/MM/yyyy", Argentina);

    /// <summary>"Vence hoy", "Vence mañana" o "Vence en N días".</summary>
    public static string Vence(DateTime vencimientoUtc, DateTime ahoraUtc)
    {
        var dias = DateOnly.FromDateTime(AHoraArgentina(vencimientoUtc)).DayNumber -
                   DateOnly.FromDateTime(AHoraArgentina(ahoraUtc)).DayNumber;
        return dias switch
        {
            < 0 => "Vencida",
            0 => "Vence hoy",
            1 => "Vence mañana",
            _ => $"Vence en {dias} días",
        };
    }

    /// <summary>Clase CSS por marca: "McDonald's" → "marca-mcdonalds".</summary>
    public static string ClaseMarca(string marca) =>
        "marca-" + new string([.. marca.ToLowerInvariant().Where(c => char.IsLetterOrDigit(c) || c == ' ')]).Replace(' ', '-');

    public static (string Texto, string Clase) Semaforo(Semaforo semaforo) => semaforo switch
    {
        Modules.CanjeVerificacion.Semaforo.Verde => ("Funciona según la comunidad", "text-bg-success"),
        Modules.CanjeVerificacion.Semaforo.Amarillo => ("Resultados mixtos", "text-bg-warning"),
        Modules.CanjeVerificacion.Semaforo.Rojo => ("Probablemente no funcione", "text-bg-danger"),
        _ => ("Sin datos todavía", "text-bg-secondary"),
    };
}
