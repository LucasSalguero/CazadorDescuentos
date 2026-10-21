using CazadorDescuentos.Modules.CanjeVerificacion;
using CazadorDescuentos.Modules.Catalogo;
using Microsoft.EntityFrameworkCore;

namespace CazadorDescuentos.Data;

/// <summary>
/// Contexto único de la aplicación. Cada tabla pertenece a un módulo:
/// Ofertas solo se usa desde Modules/Catalogo y Votos solo desde Modules/CanjeVerificacion.
/// </summary>
internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Oferta> Ofertas => Set<Oferta>();
    public DbSet<Voto> Votos => Set<Voto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Oferta>(oferta =>
        {
            oferta.ToTable("Ofertas");
            oferta.HasIndex(o => o.ClaveUnica).IsUnique();
        });

        modelBuilder.Entity<Voto>(voto =>
        {
            voto.ToTable("Votos");
            voto.HasIndex(v => new { v.OfertaId, v.DeviceId }).IsUnique();
        });
    }
}
