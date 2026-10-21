# CazadorDescuentos

Recomendador de descuentos y cupones de comida rápida para Córdoba, Argentina.

Reúne en una sola pantalla las promociones de varias marcas, las ordena según las preferencias de cada usuario y muestra cuáles funcionan de verdad gracias a la validación de la comunidad. El objetivo es que el usuario no tenga que revisar una aplicación por marca para encontrar el mejor descuento.

## Problema que resuelve

Las promociones de comida rápida están dispersas entre los sitios de cada marca, PedidosYa y otros canales. Compararlas lleva tiempo, y muchas veces un cupón ya no funciona cuando se lo intenta usar.

CazadorDescuentos propone:

- **Centralizar** las ofertas de hasta 3 marcas en un feed único.
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

Registro de usuarios, pagos y carrito propio, notificaciones push, geolocalización y mapas, recomendaciones con machine learning, gestión de delivery y panel de administración gráfico. El detalle de cada exclusión está en la propuesta de MVP del equipo.

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

## Cómo ejecutarlo

### Requisitos

- [SDK de .NET 10](https://dotnet.microsoft.com/download)
- Una clave de API del proveedor de LLM elegido (solo para la extracción automática)

### Pasos

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/LucasSalguero/CazadorDescuentos.git
   cd CazadorDescuentos
   ```

2. Restaurar las herramientas locales (incluye `dotnet-ef`):

   ```bash
   dotnet tool restore
   ```

3. Configurar los secretos. Nunca se guardan en archivos del repositorio:

   ```bash
   cd src/CazadorDescuentos
   dotnet user-secrets set "Ingesta:LlmApiKey" "<clave-del-llm>"
   dotnet user-secrets set "Ingesta:CargaManualApiKey" "<clave-de-carga-manual>"
   ```

   En producción se usan las variables de entorno `Ingesta__LlmApiKey` e `Ingesta__CargaManualApiKey`.

4. Crear la base de datos local:

   ```bash
   dotnet ef database update
   ```

5. Ejecutar la aplicación:

   ```bash
   dotnet run
   ```

6. Ejecutar las pruebas, desde la raíz del repositorio:

   ```bash
   dotnet test
   ```

### Configuración

En `src/CazadorDescuentos/appsettings.json`:

- `Ingesta:Marcas`: nombre y URL de promociones de cada marca.
- `Semaforo`: cantidad mínima de votos y porcentajes que definen los colores verde y rojo.

## Cómo contribuir

1. Tomar un issue del [tablero de issues](https://github.com/LucasSalguero/CazadorDescuentos/issues). Cada uno es una historia de usuario con sus criterios de aceptación.
2. Crear una rama desde `main` para ese issue.
3. Abrir un pull request con `Closes #N` en la descripción y pedir revisión a otro integrante.

Las reglas de nombres, límites entre módulos, pruebas y definición de terminado están en [CONVENTIONS.md](CONVENTIONS.md).
