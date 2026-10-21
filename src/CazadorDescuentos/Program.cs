using CazadorDescuentos.Data;
using CazadorDescuentos.Modules.CanjeVerificacion;
using CazadorDescuentos.Modules.Catalogo;
using CazadorDescuentos.Modules.Ingesta;
using CazadorDescuentos.Modules.Personalizacion;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlite(builder.Configuration.GetConnectionString("CazadorDescuentos")));

builder.Services.AddCatalogo();
builder.Services.AddIngesta(builder.Configuration);
builder.Services.AddPersonalizacion();
builder.Services.AddCanjeVerificacion(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapIngesta();
app.MapCanjeVerificacion();

app.Run();
