using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Catalogo.GuardarOferta;

namespace CazadorDescuentos.Tests.Catalogo;

public class ClaveUnicaTests
{
    [Fact]
    public void Combina_marca_y_producto_normalizados()
    {
        Assert.Equal("mcdonalds|combo-big-mac-mediano", ClaveUnica.Generar("McDonald's", "  Combo Big Mac ¡Mediano! "));
    }

    [Fact]
    public void Ignora_mayusculas_tildes_y_espacios_extra()
    {
        var a = ClaveUnica.Generar("Burger King", "Stacker Cuádruple");
        var b = ClaveUnica.Generar("burger  king", "STACKER   CUADRUPLE");
        Assert.Equal(a, b);
    }

    [Fact]
    public void Productos_distintos_generan_claves_distintas()
    {
        Assert.NotEqual(
            ClaveUnica.Generar("Mostaza", "Combo Mega Cuarto"),
            ClaveUnica.Generar("Mostaza", "Combo Mega Cuarto Deluxe"));
    }

    [Fact]
    public void Misma_oferta_en_marcas_distintas_no_se_pisa()
    {
        Assert.NotEqual(
            ClaveUnica.Generar("Mostaza", "Nuggets x10"),
            ClaveUnica.Generar("Burger King", "Nuggets x10"));
    }

    [Theory]
    [InlineData("", "Combo")]
    [InlineData("Mostaza", " ")]
    public void Rechaza_marca_o_producto_vacios(string marca, string producto)
    {
        Assert.ThrowsAny<ArgumentException>(() => ClaveUnica.Generar(marca, producto));
    }

    [Theory]
    [InlineData("mcdonalds", "McDonald's")]
    [InlineData("MC DONALD'S", "McDonald's")]
    [InlineData("burgerking", "Burger King")]
    [InlineData("mostaza", "Mostaza")]
    public void Normaliza_nombres_de_marca(string entrada, string esperado)
    {
        Assert.Equal(esperado, Marcas.Normalizar(entrada));
    }

    [Fact]
    public void Marca_no_soportada_devuelve_null()
    {
        Assert.Null(Marcas.Normalizar("Wendy's"));
    }

    [Fact]
    public void Vencimiento_es_fin_del_dia_en_Argentina_expresado_en_UTC()
    {
        // 15/11/2026 23:59:59 en UTC-3 = 16/11/2026 02:59:59 UTC
        var utc = GuardarOfertaService.VencimientoAUtc(new DateOnly(2026, 11, 15));
        Assert.Equal(DateTimeKind.Utc, utc.Kind);
        Assert.Equal(new DateTime(2026, 11, 16, 2, 59, 59, DateTimeKind.Utc), utc.AddTicks(-(utc.Ticks % TimeSpan.TicksPerSecond)));
    }
}
