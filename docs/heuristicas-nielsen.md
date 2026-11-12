# Heurísticas de Nielsen en la interfaz

Cómo se aplica cada una de las 10 heurísticas de usabilidad de Jakob Nielsen en CazadorDescuentos y dónde verlo.

| # | Heurística | Qué hace la app | Dónde |
|---|---|---|---|
| 1 | **Visibilidad del estado del sistema** | "Mostrando N de M ofertas" con los filtros activos; la navegación marca la página actual; cada chip muestra cuántas ofertas va a mostrar; aviso al guardar o borrar preferencias y al copiar el código; el botón de voto dice "Enviando…"; "Vence hoy / mañana" resaltado en rojo. | Feed, navegación, Preferencias, Ficha |
| 2 | **Coincidencia con el mundo real** | Lenguaje rioplatense ("Elegí", "Tocá"); precios en `$ 8.775`, fechas `dd/mm/aaaa` y hora de Argentina; íconos de comida por categoría; foto de cada oferta; "Ahorrás $ X". | Todas |
| 3 | **Control y libertad del usuario** | Cada filtro se quita con su propio ✕ y además está "Quitar filtros"; "Volver a las ofertas" conserva los filtros; la pregunta "¿Te funcionó?" se cierra sin responder; "Omitir" en preferencias y "Borrar mis preferencias". | Feed, Ficha, Preferencias |
| 4 | **Consistencia y estándares** | Un solo color de acción (naranja del logo) para todos los botones principales; los mismos chips, radios y tarjetas en toda la app; cada marca conserva su color; navegación igual en todas las páginas. | `wwwroot/css/site.css` |
| 5 | **Prevención de errores** | No se pueden elegir más de 3 categorías y se explica por qué; los filtros que darían 0 resultados quedan deshabilitados; una oferta vencida no deja canjear; se avisa antes de abrir el comercio en otra pestaña; condiciones y semáforo aparecen antes del botón de canje. | Preferencias, Feed, Ficha |
| 6 | **Reconocer antes que recordar** | Chips visibles en vez de listas desplegables; el chip activo queda marcado y se desplaza a la vista; "Primero vas a ver: …" recuerda las preferencias; el código se muestra grande; "Ya votaste esta oferta". | Feed, Ficha |
| 7 | **Flexibilidad y eficiencia de uso** | Filtrar con un solo toque; "Copiar código e ir" en un paso (o "Solo copiar"); en el celular el feed es una lista compacta y la navegación queda abajo, al alcance del pulgar; enlace "Saltar al contenido" para teclado; las URL con filtros se pueden compartir. | Feed, Ficha, Layout |
| 8 | **Diseño estético y minimalista** | Tarjetas con foto, precio y lo justo (vencimiento, si tiene código); la categoría se oculta en la tarjeta del celular porque ya está en el filtro; colores neutros salvo descuento, marca y urgencia. | Feed |
| 9 | **Ayudar a reconocer, diagnosticar y recuperarse de errores** | Sin resultados se ofrecen salidas ("Todo McDonald's", "Ver todas las ofertas"); un filtro mal escrito en la URL se explica ("No encontramos la marca …"); 404 y error en español con un botón para volver; si falla el voto se dice qué hacer y se puede reintentar; si no se puede copiar, el código queda seleccionado; si una foto no carga se muestra la ilustración de su categoría. | Feed, Ficha, `NoEncontrado`, `Error` |
| 10 | **Ayuda y documentación** | Página "¿Cómo funciona?" con los pasos, la explicación del semáforo con los umbrales reales de `appsettings.json` y preguntas frecuentes; en la ficha, "Cómo canjearla" paso a paso y "¿Cómo se calcula?" junto al semáforo. | `/Ayuda`, Ficha |

## Accesibilidad (apoya varias heurísticas)

- Áreas táctiles de al menos 40–44 px; foco visible al navegar con teclado; `aria-current` en navegación y filtros; textos alternativos en las fotos.
- Contraste AA en los botones (texto blanco sobre `#c2410c`).
- Respeta `prefers-reduced-motion`.

## Fotos

Las ofertas tienen un campo opcional `ImagenUrl`. La carga manual (`POST /admin/ofertas`) y la extracción con el LLM pueden traer la foto del comercio. Los datos demo usan fotos ilustrativas de Unsplash (`wwwroot/img/ofertas/CREDITOS.txt`) y la ficha lo aclara con "Imagen ilustrativa". Sin foto, se usa la ilustración de la categoría (`wwwroot/img/categorias/`).
