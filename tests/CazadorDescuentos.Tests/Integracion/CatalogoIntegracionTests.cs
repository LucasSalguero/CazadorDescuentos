using CazadorDescuentos.Data;
using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Catalogo.GuardarOferta;
using CazadorDescuentos.Modules.Catalogo.ListarOfertasActivas;
using CazadorDescuentos.Modules.Catalogo.ObtenerOferta;
using Microsoft.EntityFrameworkCore;

namespace CazadorDescuentos.Tests.Integracion;

/// <summary>Catalogo contra EF Core + SQLite real (#4, #5, #7, #8 y orden por defecto).</summary>
public sealed class CatalogoIntegracionTests : IDisposable
{
    private readonly BaseSqlite _base = new();

    // 11/11/2026 12:00 en Argentina.
    private static readonly DateOnly Hoy = new(2026, 11, 11);

    public void Dispose() => _base.Dispose();

    private ICatalogo Catalogo(AppDbContext db) => new CatalogoService(
        new GuardarOfertaService(db, _base.Reloj),
        new ListarOfertasActivasService(db, _base.Reloj),
        new ObtenerOfertaService(db));

    private static NuevaOferta Oferta(
        string marca = "Mostaza",
        string producto = "Combo Mega Cuarto",
        string categoria = "Hamburguesas",
        decimal porcentaje = 25m,
        DateOnly? vencimiento = null,
        decimal precioPromocional = 9675m) =>
        new(marca, producto, categoria, "COD", null, "https://mostazaweb.com.ar/",
            12900m, precioPromocional, porcentaje, vencimiento ?? Hoy.AddDays(3));

    [Fact]
    public async Task Upsert_misma_oferta_con_otras_mayusculas_y_tildes_deja_una_sola_fila_actualizada()
    {
        int id1, id2;
        using (var db = _base.CrearContexto())
        {
            id1 = await Catalogo(db).GuardarOfertaAsync(Oferta(marca: "McDonald's", producto: "Combo Big Mac"));
        }

        using (var db = _base.CrearContexto())
        {
            id2 = await Catalogo(db).GuardarOfertaAsync(
                Oferta(marca: "mcdonalds", producto: "  COMBO  BÍG MÁC ", precioPromocional: 8000m));
        }

        using var lectura = _base.CrearContexto();
        var fila = Assert.Single(await lectura.Ofertas.ToListAsync());
        Assert.Equal(id1, id2);
        Assert.Equal("McDonald's", fila.Marca);
        Assert.Equal(8000m, fila.PrecioPromocional);
        Assert.Equal("mcdonalds|combo-big-mac", fila.ClaveUnica);
    }

    [Fact]
    public async Task Guarda_la_imagen_y_la_conserva_si_una_actualizacion_llega_sin_foto()
    {
        using var db = _base.CrearContexto();
        var catalogo = Catalogo(db);
        var id = await catalogo.GuardarOfertaAsync(Oferta() with { ImagenUrl = "/img/ofertas/combo-papas.jpg" });
        await catalogo.GuardarOfertaAsync(Oferta() with { ImagenUrl = null, PrecioPromocional = 9000m });

        var oferta = await catalogo.ObtenerPorIdAsync(id);
        Assert.Equal("/img/ofertas/combo-papas.jpg", oferta!.ImagenUrl);
        Assert.Equal(9000m, oferta.PrecioPromocional);
    }

    [Fact]
    public async Task Oferta_vencida_no_se_lista_pero_sigue_en_la_base_y_se_puede_obtener_por_id()
    {
        using var db = _base.CrearContexto();
        var catalogo = Catalogo(db);
        var idVencida = await catalogo.GuardarOfertaAsync(Oferta(producto: "Vencida", vencimiento: Hoy.AddDays(-1)));
        await catalogo.GuardarOfertaAsync(Oferta(producto: "Activa"));

        var activas = await catalogo.ListarActivasAsync(new FiltrosOfertas());

        Assert.Equal(["Activa"], activas.Select(o => o.Producto));
        Assert.Equal(2, await db.Ofertas.CountAsync());
        var vencida = await catalogo.ObtenerPorIdAsync(idVencida);
        Assert.NotNull(vencida);
        Assert.Equal(DateTimeKind.Utc, vencida.VencimientoUtc.Kind);
    }

    [Fact]
    public async Task Vencimiento_es_el_fin_del_dia_en_Argentina()
    {
        using var db = _base.CrearContexto();
        var catalogo = Catalogo(db);
        await catalogo.GuardarOfertaAsync(Oferta(vencimiento: Hoy));

        // 11/11 23:59 en Argentina = 12/11 02:59 UTC → sigue activa.
        _base.Reloj.Ahora = new DateTimeOffset(2026, 11, 12, 2, 59, 0, TimeSpan.Zero);
        var antes = await catalogo.ListarActivasAsync(new FiltrosOfertas());
        Assert.Single(antes);
        Assert.Equal(DateTimeKind.Utc, antes[0].VencimientoUtc.Kind);
        Assert.Equal(new DateTime(2026, 11, 12, 2, 59, 59, DateTimeKind.Utc).AddTicks(9_999_999), antes[0].VencimientoUtc);

        // 12/11 00:00 en Argentina = 12/11 03:00 UTC → ya venció.
        _base.Reloj.Ahora = new DateTimeOffset(2026, 11, 12, 3, 0, 0, TimeSpan.Zero);
        Assert.Empty(await catalogo.ListarActivasAsync(new FiltrosOfertas()));
    }

    [Fact]
    public async Task Filtra_por_marca_categoria_y_ambas()
    {
        using var db = _base.CrearContexto();
        var catalogo = Catalogo(db);
        await catalogo.GuardarOfertaAsync(Oferta("Mostaza", "Combo Mostaza", "Hamburguesas"));
        await catalogo.GuardarOfertaAsync(Oferta("Mostaza", "Balde", "Pollo"));
        await catalogo.GuardarOfertaAsync(Oferta("McDonald's", "Big Mac", "Hamburguesas"));
        await catalogo.GuardarOfertaAsync(Oferta("McDonald's", "McFlurry", "Postres"));
        await catalogo.GuardarOfertaAsync(Oferta("Burger King", "Whopper", "Hamburguesas"));

        async Task<string[]> Productos(string? marca, string? categoria) =>
            [.. (await catalogo.ListarActivasAsync(new FiltrosOfertas(marca, categoria))).Select(o => o.Producto).Order()];

        Assert.Equal(["Big Mac", "McFlurry"], await Productos("McDonald's", null));
        Assert.Equal(["Big Mac", "McFlurry"], await Productos("mcdonalds", null));
        Assert.Equal(["Big Mac", "Combo Mostaza", "Whopper"], await Productos(null, "Hamburguesas"));
        Assert.Equal(["Big Mac"], await Productos("mcdonalds", "Hamburguesas"));
        Assert.Equal(["Balde"], await Productos("Mostaza", "pollo"));
        Assert.Empty(await Productos("Burger King", "Postres"));
    }

    [Fact]
    public async Task Categoria_con_otras_mayusculas_se_guarda_con_el_nombre_canonico()
    {
        using var db = _base.CrearContexto();
        var catalogo = Catalogo(db);
        var id = await catalogo.GuardarOfertaAsync(Oferta(categoria: "  hamburguesas "));

        Assert.Equal("Hamburguesas", (await catalogo.ObtenerPorIdAsync(id))!.Categoria);
        Assert.Single(await catalogo.ListarActivasAsync(new FiltrosOfertas(Categoria: "Hamburguesas")));
    }

    [Fact]
    public async Task Ordena_por_mayor_descuento_y_a_igualdad_por_vencimiento_mas_proximo()
    {
        using var db = _base.CrearContexto();
        var catalogo = Catalogo(db);
        // Valores que en TEXT quedarían mal ordenados ("9" > "50" > "100" > "12.5").
        await catalogo.GuardarOfertaAsync(Oferta(producto: "Nueve", porcentaje: 9m));
        await catalogo.GuardarOfertaAsync(Oferta(producto: "Cincuenta", porcentaje: 50m));
        await catalogo.GuardarOfertaAsync(Oferta(producto: "Doce y medio", porcentaje: 12.5m));
        await catalogo.GuardarOfertaAsync(Oferta(producto: "Cien", porcentaje: 100m));
        await catalogo.GuardarOfertaAsync(Oferta(producto: "Cincuenta pronto", porcentaje: 50m, vencimiento: Hoy));

        var orden = (await catalogo.ListarActivasAsync(new FiltrosOfertas())).Select(o => o.Producto);

        Assert.Equal(["Cien", "Cincuenta pronto", "Cincuenta", "Doce y medio", "Nueve"], orden);
    }
}
