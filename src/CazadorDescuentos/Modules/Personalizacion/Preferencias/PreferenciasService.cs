namespace CazadorDescuentos.Modules.Personalizacion.Preferencias;

internal sealed class PreferenciasService
{
    public const string NombreCookie = "preferencias";
    public const int MaximoCategorias = 3;

    // TODO #6: leer las categorías preferidas desde la cookie (lista vacía si no existe).
    public IReadOnlyList<string> Leer(HttpRequest request) =>
        throw new NotImplementedException();

    // TODO #6: guardar en la cookie como máximo MaximoCategorias categorías válidas.
    public void Escribir(HttpResponse response, IReadOnlyList<string> categorias) =>
        throw new NotImplementedException();
}
