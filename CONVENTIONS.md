# Convenciones del proyecto CazadorDescuentos

Guía para implementar nuevas funcionalidades. Si algo no está escrito acá, se decide con criterio y se comenta en el PR.

## Contexto

- **Stack:** ASP.NET Core (Razor Pages), EF Core y SQLite, en un solo proyecto.
- **Arquitectura:** monolito modular. Los módulos están en `Modules/` y representan capacidades de negocio:

| Módulo | Responsabilidad | Tablas propias | Interfaz pública |
|---|---|---|---|
| `Ingesta` | Obtener ofertas externas y estructurarlas | Ninguna | Ninguna |
| `Catalogo` | Mantener y consultar el catálogo | `Ofertas` | `ICatalogo` |
| `Personalizacion` | Ordenar el feed según preferencias | Ninguna (usa cookie) | `IPersonalizacion` |
| `CanjeVerificacion` | Canje, votos y semáforo | `Votos` | `IVerificacion` |

- Las Razor Pages y los endpoints viven en el host y combinan lo que exponen los módulos.

## Cómo implementar una funcionalidad

1. Partir de un issue con criterios de aceptación. Si no existe, crearlo primero.
2. Identificar a qué módulo pertenece cada regla de negocio nueva.
3. Crear una rama desde `main` para ese issue.
4. Implementar el flujo completo: pantalla o endpoint, lógica en el módulo y datos.
5. Si la funcionalidad incluye una de las reglas listadas en [Pruebas](#pruebas), agregar sus pruebas.
6. Abrir el PR con `Closes #N` en la descripción.

## Reglas

### 1. Nombres

Los nombres del dominio van en español, sin tildes ni ñ: `Oferta`, `GuardarOferta`, `Catalogo`.

Lo que impone .NET queda como viene: `DbContext`, `Program`, sufijos como `Service`.

### 2. Límites entre módulos

- Cada módulo usa solo sus propias tablas.
- Para datos de otro módulo se usa su interfaz pública.
- Todas las clases de un módulo son `internal`, salvo su interfaz pública.

Ejemplo: `db.Votos` no puede aparecer fuera de `Modules/CanjeVerificacion`. Se controla en la revisión del PR.

### 3. Montos y fechas

- Los montos se manejan siempre como `decimal`.
- Las fechas se guardan en UTC.
- El vencimiento de una oferta se interpreta como el fin de ese día en hora de Argentina (UTC-3) y se convierte a UTC al guardarlo.

### 4. Secretos

La clave del LLM y la API key de carga manual nunca se suben al repositorio:

- **Desarrollo:** `dotnet user-secrets`.
- **Producción:** variables de entorno.

### 5. Pull requests

- Una rama corta por issue.
- Todo entra a `main` por PR. Antes de mergear, revisar el diff completo en GitHub.
- La descripción incluye `Closes #N`.
- Se usa squash merge; el título del PR queda como mensaje del commit.
- No se edita una migración que ya está en `main`; se crea una nueva.

## Pruebas

Se escriben pruebas unitarias (xUnit) solo para estas reglas de negocio:

- validación de ofertas;
- generación de la clave única;
- priorización por preferencias;
- cálculo del semáforo.

El resto se prueba a mano siguiendo los criterios de aceptación del issue.

## Definición de terminado

Un issue está terminado cuando:

- [ ] todos sus criterios de aceptación están tildados;
- [ ] el PR está mergeado en `main`;
- [ ] el build y las pruebas pasan.
