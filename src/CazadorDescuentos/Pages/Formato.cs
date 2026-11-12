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

    /// <summary>Días entre hoy y el vencimiento, en hora de Argentina (0 = vence hoy).</summary>
    public static int DiasHastaVencer(DateTime vencimientoUtc, DateTime ahoraUtc) =>
        DateOnly.FromDateTime(AHoraArgentina(vencimientoUtc)).DayNumber -
        DateOnly.FromDateTime(AHoraArgentina(ahoraUtc)).DayNumber;

    /// <summary>"Vence hoy", "Vence mañana" o "Vence en N días".</summary>
    public static string Vence(DateTime vencimientoUtc, DateTime ahoraUtc) =>
        DiasHastaVencer(vencimientoUtc, ahoraUtc) switch
        {
            < 0 => "Vencida",
            0 => "Vence hoy",
            1 => "Vence mañana",
            var dias => $"Vence en {dias} días",
        };

    /// <summary>True si vence hoy o mañana (se destaca para que no se pase).</summary>
    public static bool VencePronto(DateTime vencimientoUtc, DateTime ahoraUtc) =>
        DiasHastaVencer(vencimientoUtc, ahoraUtc) is 0 or 1;

    /// <summary>Clase CSS por marca: "McDonald's" → "marca-mcdonalds".</summary>
    public static string ClaseMarca(string marca) =>
        "marca-" + new string([.. marca.ToLowerInvariant().Where(c => char.IsLetterOrDigit(c) || c == ' ')]).Replace(' ', '-');

    /// <summary>Ícono de cada categoría, para reconocerla de un vistazo.</summary>
    public static string IconoCategoria(string categoria) => categoria switch
    {
        "Hamburguesas" => "🍔",
        "Pollo" => "🍗",
        "Pizza" => "🍕",
        "Postres" => "🍦",
        "Bebidas" => "🥤",
        _ => "🏷️",
    };

    /// <summary>Ilustración de respaldo cuando la oferta no tiene foto o la foto no carga.</summary>
    public static string ImagenCategoria(string categoria)
    {
        var nombre = categoria.ToLowerInvariant();
        return nombre is "hamburguesas" or "pollo" or "pizza" or "postres" or "bebidas"
            ? $"/img/categorias/{nombre}.svg"
            : "/img/categorias/hamburguesas.svg";
    }

    public static (string Texto, string Clase) Semaforo(Semaforo semaforo) => semaforo switch
    {
        Modules.CanjeVerificacion.Semaforo.Verde => ("Funciona según la comunidad", "semaforo-verde"),
        Modules.CanjeVerificacion.Semaforo.Amarillo => ("Resultados mixtos", "semaforo-amarillo"),
        Modules.CanjeVerificacion.Semaforo.Rojo => ("Probablemente no funcione", "semaforo-rojo"),
        _ => ("Sin datos todavía", "semaforo-sin-datos"),
    };

    /// <summary>Explica de dónde sale el color del semáforo.</summary>
    public static string DetalleSemaforo(ResumenVotos resumen, ReglasSemaforo reglas)
    {
        if (resumen.Semaforo == Modules.CanjeVerificacion.Semaforo.SinDatos)
        {
            var faltan = Math.Max(reglas.MinimoVotos - resumen.Total, 1);
            return resumen.Total == 0
                ? $"Nadie la votó todavía. Con {reglas.MinimoVotos} votos te mostramos si funciona."
                : $"Tiene {Votos(resumen.Total)}. Faltan {Votos(faltan)} para mostrar si funciona.";
        }

        return $"A {resumen.Positivos} de {Personas(resumen.Total)} les funcionó.";
    }

    private static string Votos(int n) => n == 1 ? "1 voto" : $"{n} votos";

    private static string Personas(int n) => n == 1 ? "1 persona" : $"{n} personas";
}
