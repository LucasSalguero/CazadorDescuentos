using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Personalizacion.Preferencias;
using CazadorDescuentos.Modules.Personalizacion.PriorizarOfertas;

namespace CazadorDescuentos.Tests.Personalizacion;

public class PriorizarOfertasTests
{
    private static OfertaDetalle Oferta(int id, string categoria) =>
        new(id, "Mostaza", $"Producto {id}", categoria, null, null, "https://mostazaweb.com.ar/", 100m, 80m, 20m, DateTime.UtcNow.AddDays(1));

    private static readonly OfertaDetalle[] Feed =
    [
        Oferta(1, "Hamburguesas"),
        Oferta(2, "Postres"),
        Oferta(3, "Pollo"),
        Oferta(4, "Postres"),
    ];

    private readonly PriorizarOfertasService _servicio = new();

    [Fact]
    public void Sin_preferencias_mantiene_el_orden_por_defecto()
    {
        var resultado = _servicio.Ejecutar(Feed, []);

        Assert.Equal([1, 2, 3, 4], resultado.Select(r => r.Oferta.Id));
        Assert.All(resultado, r => Assert.Null(r.Etiqueta));
    }

    [Fact]
    public void Pone_primero_las_que_coinciden_y_deja_las_demas_debajo()
    {
        var resultado = _servicio.Ejecutar(Feed, ["Postres"]);

        Assert.Equal([2, 4, 1, 3], resultado.Select(r => r.Oferta.Id));
        Assert.Equal(Feed.Length, resultado.Count);
    }

    [Fact]
    public void Etiqueta_solo_las_que_coinciden()
    {
        var resultado = _servicio.Ejecutar(Feed, ["pollo"]);

        var pollo = Assert.Single(resultado, r => r.CoincideConPreferencias);
        Assert.Equal("Coincide con tu preferencia por #Pollo", pollo.Etiqueta);
        Assert.All(resultado.Where(r => !r.CoincideConPreferencias), r => Assert.Null(r.Etiqueta));
    }

    [Fact]
    public void Con_varias_preferencias_respeta_el_orden_original_dentro_del_grupo()
    {
        var resultado = _servicio.Ejecutar(Feed, ["Pollo", "Hamburguesas"]);

        Assert.Equal([1, 3, 2, 4], resultado.Select(r => r.Oferta.Id));
    }

    [Fact]
    public void Preferencias_se_limitan_a_3_validas_y_sin_repetir()
    {
        string[] existentes = ["Hamburguesas", "Pollo", "Pizza", "Postres", "Bebidas"];

        var depuradas = PreferenciasService.Depurar(["pizza", "Sushi", "Pizza", "Pollo", "Bebidas", "Postres"], existentes);

        Assert.Equal(["Pizza", "Pollo", "Bebidas"], depuradas);
    }
}
