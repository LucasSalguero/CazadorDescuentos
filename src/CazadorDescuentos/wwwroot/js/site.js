// Comportamiento común a todas las páginas.
(function () {
    "use strict";

    /** Muestra un aviso breve abajo de la pantalla (confirmaciones y errores). */
    window.avisar = function (texto, opciones) {
        const toast = document.getElementById("aviso-global");
        if (!toast) {
            return;
        }
        toast.querySelector("[data-aviso-texto]").textContent = texto;
        if (window.bootstrap) {
            bootstrap.Toast.getOrCreateInstance(toast, { delay: (opciones && opciones.duracion) || 2500 }).show();
        }
    };

    // Aviso que viene del servidor (por ejemplo, "Guardamos tus preferencias").
    const avisoServidor = document.getElementById("aviso-global");
    if (avisoServidor && avisoServidor.dataset.mostrar === "true" && window.bootstrap) {
        bootstrap.Toast.getOrCreateInstance(avisoServidor, { delay: 3500 }).show();
    }

    // En el celular los filtros se deslizan: se lleva a la vista el chip elegido para que se vea qué está activo.
    document.querySelectorAll(".chips").forEach(function (fila) {
        const activo = fila.querySelector('.chip[aria-current="true"]:not(:first-child)');
        if (activo && fila.scrollWidth > fila.clientWidth) {
            fila.scrollLeft = activo.offsetLeft - (fila.clientWidth - activo.offsetWidth) / 2;
        }
    });

    // Si una foto no carga, se reemplaza por la ilustración de su categoría (data-respaldo).
    function usarRespaldo(img) {
        if (img.dataset.respaldo && img.src.indexOf(img.dataset.respaldo) === -1) {
            img.src = img.dataset.respaldo;
            img.classList.add("es-ilustracion");
        }
    }
    document.addEventListener("error", function (e) {
        if (e.target instanceof HTMLImageElement) {
            usarRespaldo(e.target);
        }
    }, true);
    // Las que fallaron antes de que cargara este script.
    document.querySelectorAll("img[data-respaldo]").forEach(function (img) {
        if (img.complete && img.naturalWidth === 0 && img.getAttribute("loading") !== "lazy") {
            usarRespaldo(img);
        }
    });
})();
