# CazadorDescuentos

Recomendador de descuentos y cupones de comida rápida para Córdoba, Argentina.

Reúne en una sola pantalla las promociones de varias marcas, las ordena según las preferencias de cada usuario y muestra cuáles funcionan de verdad gracias a la validación de la comunidad. El objetivo es que el usuario no tenga que revisar una aplicación por marca para encontrar el mejor descuento.

## Problema que resuelve

Las promociones de comida rápida están dispersas entre los sitios de cada marca, PedidosYa y otros canales. Compararlas lleva tiempo, y muchas veces un cupón ya no funciona cuando se lo intenta usar.

CazadorDescuentos propone:

- **Centralizar** las ofertas de Mostaza, McDonald's y Burger King en un feed único.
- **Personalizar** el orden según las categorías de comida que elige el usuario.
- **Verificar** cada oferta con los votos de quienes la usaron.

## Alcance del MVP

### Incluido

| Módulo | Funcionalidad |
|---|---|
| Administración | Extracción diaria automática de ofertas de hasta 3 marcas |
| Administración | Estructuración del contenido con un LLM y validación de campos obligatorios |
| Administración | Carga manual de ofertas como contingencia |
| Administración | Deduplicación de ofertas y ocultamiento de las vencidas |
| Cliente | Selección de hasta 3 categorías de comida, sin necesidad de cuenta |
| Cliente | Feed unificado, filtros por marca y categoría, y priorización según preferencias |
| Cliente | Ficha detallada con precio final, ahorro, vigencia y condiciones |
| Cliente | Canje rápido: copia del código y redirección al comercio |
| Cliente | Voto "Me funcionó / No me funcionó" y semáforo de confiabilidad |

### Fuera de alcance

Registro de usuarios, pagos y carrito propio, notificaciones push, geolocalización y mapas, recomendaciones con machine learning, gestión de delivery y panel de administración gráfico.

## Stack

- .NET 10, ASP.NET Core con Razor Pages
- Entity Framework Core con SQLite
- xUnit para pruebas
- API de un LLM para estructurar las ofertas

## Arquitectura

Monolito modular: un solo proyecto web dividido en módulos que representan capacidades de negocio. Dentro de cada módulo, el código se organiza en una carpeta por caso de uso.

| Módulo | Responsabilidad |
|---|---|
| `Ingesta` | Obtener ofertas de fuentes externas y estructurarlas |
| `Catalogo` | Mantener el catálogo íntegro y vigente, y consultarlo |
| `Personalizacion` | Ordenar el feed según las preferencias del usuario |
| `CanjeVerificacion` | Canje de ofertas, votos y semáforo de confiabilidad |

Cada módulo expone una sola interfaz pública y no accede a las tablas de otro. Las Razor Pages del host combinan lo que exponen los módulos. Las reglas completas están en [CONVENTIONS.md](CONVENTIONS.md).

```
src/CazadorDescuentos/
├── Modules/
│   ├── Catalogo/
│   ├── Ingesta/
│   ├── Personalizacion/
│   └── CanjeVerificacion/
├── Data/            DbContext y migraciones
├── Pages/           Feed, preferencias y ficha de oferta
└── wwwroot/js/      Canje y feedback del lado del cliente
tests/CazadorDescuentos.Tests/
```

## Cómo correrlo

```bash
cd src/CazadorDescuentos
dotnet run
```

- La base SQLite se crea sola al iniciar (se aplican las migraciones).
- En `Development` se cargan **ofertas de ejemplo** de las 3 marcas (`Ingesta:CargarDatosDemo`) para ver el frontend sin LLM.
- Para la extracción real configurar la clave del LLM (API compatible con OpenAI Chat Completions; URL y modelo en `appsettings.json`):

```bash
dotnet user-secrets set "Ingesta:LlmApiKey" "<clave>"
dotnet user-secrets set "Ingesta:CargaManualApiKey" "<clave>"
```

Carga manual de contingencia:

```bash
curl -X POST https://localhost:7244/admin/ofertas -H "X-Api-Key: <clave>" -H "Content-Type: application/json" \
  -d '{"marca":"Mostaza","producto":"Combo Mega","categoria":"Hamburguesas","codigo":null,"restricciones":null,
       "enlaceComercio":"https://mostazaweb.com.ar/","precioOriginal":12900,"precioPromocional":9675,
       "porcentajeDescuento":25,"vencimiento":"2026-12-01"}'
```
