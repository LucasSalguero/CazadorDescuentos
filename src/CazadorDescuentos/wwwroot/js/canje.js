// Canje de una oferta desde la ficha (Pages/Ofertas/Detalle): copiar código (#11), ir al comercio (#12) y voto (#13).
(function () {
    "use strict";

    const contenedor = document.getElementById("canje");
    if (!contenedor) {
        return; // oferta vencida: no hay canje
    }

    const ofertaId = Number(contenedor.dataset.ofertaId);
    const codigo = contenedor.dataset.codigo;
    const enlace = contenedor.dataset.enlace;
    const feedback = document.getElementById("feedback");
    const botonesVoto = feedback.querySelectorAll("[data-voto]");
    const mensajeVoto = feedback.querySelector("[data-feedback-mensaje]");
    const avisar = window.avisar || function () { };

    // --- Utilidades -------------------------------------------------------------------------

    function leerStorage(clave, porDefecto) {
        try {
            return JSON.parse(localStorage.getItem(clave)) ?? porDefecto;
        } catch {
            return porDefecto;
        }
    }

    function escribirStorage(clave, valor) {
        try {
            localStorage.setItem(clave, JSON.stringify(valor));
        } catch {
            // Modo privado o storage lleno: el voto igual se manda, solo no se recuerda localmente.
        }
    }

    /** Id anónimo del dispositivo: permite un voto por dispositivo y oferta sin pedir cuenta. */
    function obtenerDeviceId() {
        let id = leerStorage("cazador.deviceId", null);
        if (!id) {
            id = (crypto.randomUUID?.() ?? `${Date.now()}-${Math.random().toString(16).slice(2)}`);
            escribirStorage("cazador.deviceId", id);
        }
        return id;
    }

    function yaVoto(id) {
        return leerStorage("cazador.votos", []).includes(id);
    }

    function marcarVotado(id) {
        const votos = leerStorage("cazador.votos", []);
        if (!votos.includes(id)) {
            votos.push(id);
            escribirStorage("cazador.votos", votos);
        }
        contenedor.querySelector("[data-ya-voto]").hidden = false;
    }

    // --- #11 Copiar código -----------------------------------------------------------------

    function seleccionarCodigo() {
        const caja = contenedor.querySelector(".codigo-cupon");
        if (caja) {
            const rango = document.createRange();
            rango.selectNodeContents(caja);
            const seleccion = window.getSelection();
            seleccion.removeAllRanges();
            seleccion.addRange(rango);
        }
    }

    async function copiarCodigo(codigo) {
        if (!codigo) {
            return false;
        }
        try {
            await navigator.clipboard.writeText(codigo);
        } catch {
            // Fallback para navegadores sin Clipboard API o sin HTTPS.
            const area = document.createElement("textarea");
            area.value = codigo;
            area.setAttribute("readonly", "");
            area.style.position = "fixed";
            area.style.opacity = "0";
            document.body.appendChild(area);
            area.select();
            const ok = document.execCommand("copy");
            area.remove();
            if (!ok) {
                seleccionarCodigo();
                avisar("No pudimos copiarlo. Dejamos el código seleccionado: copialo a mano.", { duracion: 5000 });
                return false;
            }
        }
        avisar("Código copiado");
        return true;
    }

    // --- #12 Ir al comercio ------------------------------------------------------------------

    function abrirEnlace(enlace) {
        // Pestaña nueva: al volver, la app sigue en la misma oferta.
        window.open(enlace, "_blank", "noopener");
    }

    // --- #13 Feedback al volver --------------------------------------------------------------

    function mostrarMensaje(texto, esError) {
        mensajeVoto.textContent = texto;
        mensajeVoto.classList.toggle("text-danger", !!esError);
        mensajeVoto.hidden = false;
        if (!esError) {
            feedback.querySelector("[data-feedback-botones]").hidden = true;
            feedback.querySelector("[data-feedback-pregunta]").hidden = true;
            feedback.querySelector("[data-feedback-nota]").hidden = true;
        }
        feedback.hidden = false;
    }

    function mostrarPregunta() {
        if (yaVoto(ofertaId)) {
            return;
        }
        feedback.hidden = false;
        feedback.scrollIntoView({ behavior: "smooth", block: "center" });
    }

    function preguntarAlVolver(ofertaId) {
        if (yaVoto(ofertaId)) {
            return;
        }
        const alVolver = () => {
            if (document.visibilityState === "visible") {
                document.removeEventListener("visibilitychange", alVolver);
                mostrarPregunta();
            }
        };
        document.addEventListener("visibilitychange", alVolver);
        // Si el navegador no cambió de pestaña (p. ej. bloqueó el popup), preguntar igual al rato.
        setTimeout(() => {
            if (document.visibilityState === "visible") {
                document.removeEventListener("visibilitychange", alVolver);
                mostrarPregunta();
            }
        }, 4000);
    }

    function botonesOcupados(ocupados, elegido) {
        botonesVoto.forEach(b => {
            b.disabled = ocupados;
            if (!b.dataset.textoOriginal) {
                b.dataset.textoOriginal = b.textContent;
            }
            b.textContent = ocupados && b === elegido ? "Enviando…" : b.dataset.textoOriginal;
        });
    }

    async function votar(boton) {
        const funciono = boton.dataset.voto === "true";
        botonesOcupados(true, boton);
        try {
            const respuesta = await fetch("/votos", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ ofertaId, deviceId: obtenerDeviceId(), funciono }),
            });
            if (respuesta.ok) {
                marcarVotado(ofertaId);
                mostrarMensaje("¡Gracias! Tu voto ayuda a otros a saber si la oferta funciona.");
            } else if (respuesta.status === 409) {
                marcarVotado(ofertaId);
                mostrarMensaje("Ya habías votado esta oferta desde este dispositivo.");
            } else {
                throw new Error(`HTTP ${respuesta.status}`);
            }
        } catch {
            botonesOcupados(false);
            mostrarMensaje("No pudimos registrar tu voto. Revisá tu conexión y tocá de nuevo tu respuesta.", true);
        }
    }

    // --- Eventos -----------------------------------------------------------------------------

    if (yaVoto(ofertaId)) {
        contenedor.querySelector("[data-ya-voto]").hidden = false;
    }

    contenedor.querySelector("[data-accion='copiar']")?.addEventListener("click", () => copiarCodigo(codigo));
    contenedor.querySelector("[data-accion='canjear']")?.addEventListener("click", async () => {
        if (codigo) {
            await copiarCodigo(codigo);
        }
        abrirEnlace(enlace);
        preguntarAlVolver(ofertaId);
    });
    botonesVoto.forEach(boton => boton.addEventListener("click", () => votar(boton)));
    feedback.querySelector("[data-accion='cerrar-feedback']").addEventListener("click", () => {
        feedback.hidden = true;
    });
})();
