// Canje de una oferta desde la ficha (Pages/Ofertas/Detalle).
(function () {
    "use strict";

    const contenedor = document.getElementById("canje");
    if (!contenedor) {
        return;
    }

    const ofertaId = Number(contenedor.dataset.ofertaId);
    const codigo = contenedor.dataset.codigo;
    const enlace = contenedor.dataset.enlace;

    // TODO #11: copiar el código de la oferta al portapapeles y avisar al usuario.
    function copiarCodigo(codigo) {
        throw new Error("No implementado (#11)");
    }

    // TODO #12: abrir el enlace del comercio en una pestaña nueva.
    function abrirEnlace(enlace) {
        throw new Error("No implementado (#12)");
    }

    // TODO #13: al volver a la pestaña, preguntar si el código funcionó y enviar el voto a POST /votos.
    function preguntarAlVolver(ofertaId) {
        throw new Error("No implementado (#13)");
    }

    contenedor.querySelector("[data-accion='copiar']")?.addEventListener("click", () => copiarCodigo(codigo));
    contenedor.querySelector("[data-accion='canjear']")?.addEventListener("click", () => {
        abrirEnlace(enlace);
        preguntarAlVolver(ofertaId);
    });
})();
