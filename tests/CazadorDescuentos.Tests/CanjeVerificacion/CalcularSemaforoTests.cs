using CazadorDescuentos.Modules.CanjeVerificacion;
using CazadorDescuentos.Modules.CanjeVerificacion.CalcularSemaforo;

namespace CazadorDescuentos.Tests.CanjeVerificacion;

public class CalcularSemaforoTests
{
    // Mismos valores que appsettings.json
    private static readonly OpcionesSemaforo Opciones = new() { MinimoVotos = 3, PorcentajeVerde = 70, PorcentajeRojo = 40 };

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 0)]
    [InlineData(1, 1)]
    public void Con_pocos_votos_no_hay_datos(int positivos, int negativos) =>
        Assert.Equal(Semaforo.SinDatos, CalcularSemaforoService.Calcular(positivos, negativos, Opciones));

    [Theory]
    [InlineData(3, 0)]   // 100 %
    [InlineData(7, 3)]   // 70 % justo en el umbral
    public void Mayoria_positiva_es_verde(int positivos, int negativos) =>
        Assert.Equal(Semaforo.Verde, CalcularSemaforoService.Calcular(positivos, negativos, Opciones));

    [Theory]
    [InlineData(2, 1)]   // 66,7 %
    [InlineData(4, 6)]   // 40 % justo en el umbral rojo → amarillo
    public void Resultado_mixto_es_amarillo(int positivos, int negativos) =>
        Assert.Equal(Semaforo.Amarillo, CalcularSemaforoService.Calcular(positivos, negativos, Opciones));

    [Theory]
    [InlineData(1, 2)]   // 33 %
    [InlineData(0, 3)]
    public void Mayoria_negativa_es_rojo(int positivos, int negativos) =>
        Assert.Equal(Semaforo.Rojo, CalcularSemaforoService.Calcular(positivos, negativos, Opciones));

    [Fact]
    public void Votos_negativos_invalidos_lanzan_excepcion() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => CalcularSemaforoService.Calcular(-1, 0, Opciones));
}
