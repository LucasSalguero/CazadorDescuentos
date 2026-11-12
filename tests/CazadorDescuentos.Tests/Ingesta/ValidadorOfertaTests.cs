using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Ingesta.EstructurarConLlm;

namespace CazadorDescuentos.Tests.Ingesta;

public class ValidadorOfertaTests
{
    private static readonly string[] Marcas = ["Mostaza", "McDonald's", "Burger King"];
    private static readonly string[] Categorias = ["Hamburguesas", "Pollo", "Pizza", "Postres", "Bebidas"];

    private static NuevaOferta Valida() => new(
        "Burger King", "Combo Whopper", "Hamburguesas", "WHOPPER40", null,
        "https://www.burgerking.com.ar/", 14200m, 8520m, 40m, new DateOnly(2026, 12, 1));

    private static IReadOnlyList<string> Validar(NuevaOferta oferta) => ValidadorOferta.Validar(oferta, Marcas, Categorias);

    [Fact]
    public void Oferta_completa_no_tiene_errores()
    {
        Assert.Empty(Validar(Valida()));
    }

    [Fact]
    public void Sin_codigo_ni_restricciones_sigue_siendo_valida()
    {
        Assert.Empty(Validar(Valida() with { Codigo = null, Restricciones = null }));
    }

    [Fact]
    public void Sin_marca_es_invalida() => Assert.NotEmpty(Validar(Valida() with { Marca = " " }));

    [Fact]
    public void Marca_fuera_de_las_tres_soportadas_es_invalida() => Assert.NotEmpty(Validar(Valida() with { Marca = "Wendy's" }));

    [Fact]
    public void Marca_con_otras_mayusculas_es_valida() => Assert.Empty(Validar(Valida() with { Marca = "mostaza" }));

    [Fact]
    public void Sin_producto_es_invalida() => Assert.NotEmpty(Validar(Valida() with { Producto = "" }));

    [Fact]
    public void Sin_precio_promocional_es_invalida() => Assert.NotEmpty(Validar(Valida() with { PrecioPromocional = 0 }));

    [Fact]
    public void Sin_vencimiento_es_invalida() => Assert.NotEmpty(Validar(Valida() with { Vencimiento = default }));

    [Fact]
    public void Categoria_desconocida_es_invalida() => Assert.NotEmpty(Validar(Valida() with { Categoria = "Sushi" }));

    [Fact]
    public void Precio_promocional_mayor_al_original_es_invalida() =>
        Assert.NotEmpty(Validar(Valida() with { PrecioOriginal = 1000m, PrecioPromocional = 2000m }));

    [Fact]
    public void Enlace_invalido_es_invalida() => Assert.NotEmpty(Validar(Valida() with { EnlaceComercio = "burgerking" }));

    [Theory]
    [InlineData(null)]
    [InlineData("https://www.burgerking.com.ar/img/whopper.png")]
    [InlineData("/img/ofertas/combo-papas.jpg")]
    public void Imagen_opcional_valida(string? imagen) => Assert.Empty(Validar(Valida() with { ImagenUrl = imagen }));

    [Theory]
    [InlineData("whopper.png")]
    [InlineData("javascript:alert(1)")]
    [InlineData("/img/../appsettings.json")]
    public void Imagen_invalida_es_invalida(string imagen) => Assert.NotEmpty(Validar(Valida() with { ImagenUrl = imagen }));

    [Fact]
    public void Interpreta_la_imagen_del_llm_y_resuelve_rutas_relativas()
    {
        const string json = """
            {"ofertas":[
              {"producto":"A","imagen":"/media/a.png"},
              {"producto":"B","imagen":"https://cdn.mostaza.com/b.jpg"},
              {"producto":"C","imagen":"javascript:alert(1)"},
              {"producto":"D","imagen":null}
            ]}
            """;

        var imagenes = ClienteLlm.Interpretar(json, "Mostaza", "https://www.mostazaweb.com.ar/promos/").Select(o => o.ImagenUrl);

        Assert.Equal(["https://www.mostazaweb.com.ar/media/a.png", "https://cdn.mostaza.com/b.jpg", null, null], imagenes);
    }

    [Fact]
    public void Junta_todos_los_errores()
    {
        var errores = Validar(new NuevaOferta("", "", "", null, null, "", 0, 0, 0, default));
        Assert.True(errores.Count >= 5);
    }

    [Fact]
    public void Interpreta_la_salida_del_llm_sin_inventar_restricciones()
    {
        const string json = """
            {"ofertas":[
              {"marca":"McDonald's","producto":"McCombo Big Mac","categoria":"Hamburguesas","precioOriginal":13500,
               "precioPromocional":8775,"porcentajeDescuento":35,"codigo":null,"vencimiento":"2026-12-01",
               "restricciones":null,"enlaceComercio":null},
              {"marca":"McDonald's","producto":"Sin fecha","categoria":"Postres","precioPromocional":100,"vencimiento":"pronto"}
            ]}
            """;

        var ofertas = ClienteLlm.Interpretar(json, "McDonald's", "https://www.mcdonalds.com.ar/");

        Assert.Equal(2, ofertas.Count);
        Assert.Null(ofertas[0].Restricciones);
        Assert.Equal("https://www.mcdonalds.com.ar/", ofertas[0].EnlaceComercio);
        Assert.Equal(new DateOnly(2026, 12, 1), ofertas[0].Vencimiento);
        Assert.Empty(Validar(ofertas[0]));
        Assert.NotEmpty(Validar(ofertas[1])); // vencimiento ilegible → se descarta
    }

    [Fact]
    public void Json_invalido_del_llm_no_rompe() =>
        Assert.Empty(ClienteLlm.Interpretar("no es json", "Mostaza", "https://mostazaweb.com.ar/"));
}
