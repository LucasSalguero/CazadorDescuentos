namespace CazadorDescuentos.Modules.CanjeVerificacion.CalcularSemaforo;

internal sealed class OpcionesSemaforo
{
    public const string Seccion = "Semaforo";

    /// <summary>Votos necesarios para mostrar un color distinto de SinDatos.</summary>
    public int MinimoVotos { get; set; }

    /// <summary>Porcentaje de votos positivos desde el cual el semáforo es verde.</summary>
    public int PorcentajeVerde { get; set; }

    /// <summary>Porcentaje de votos positivos por debajo del cual el semáforo es rojo.</summary>
    public int PorcentajeRojo { get; set; }
}
