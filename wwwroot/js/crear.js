const html5QrCode = new Html5Qrcode("reader");

let ultimoCodigo = "";

async function autocompletarProducto(codigo) {

    const respuesta = await fetch(
        "/Producto/AutocompletarProducto?codigoBarra="
        + encodeURIComponent(codigo)
    );

    const producto = await respuesta.json();

    if (producto) {

        document.getElementById("nombre").value =
            producto.nombre || "";

        document.getElementById("precio").value =
            producto.precio || "";

        document.getElementById("estadoProducto")
            .innerHTML =
            "<span style='color:orange;font-weight:bold;'>Producto encontrado</span>";

    } else {

        document.getElementById("nombre").value = "";
        document.getElementById("precio").value = "";

        document.getElementById("estadoProducto")
            .innerHTML =
            "<span style='color:green;font-weight:bold;'>Producto nuevo</span>";
    }
}

html5QrCode.start(
    {
        facingMode: "environment"
    },
    {
        fps: 20,
        qrbox: { width: 320, height: 90 }
    },

    async (decodedText) => {

        const codigo =
            decodedText.replace(/\s/g, "").trim();

        if (codigo === ultimoCodigo)
            return;

        ultimoCodigo = codigo;

        document.getElementById("codigoBarra")
            .value = codigo;

        await autocompletarProducto(codigo);

    },
    (error) => { }
);

document.addEventListener(
    "DOMContentLoaded",
    () => {

        document.getElementById("codigoBarra")
            .addEventListener(
                "change",
                function () {

                    autocompletarProducto(
                        this.value
                    );

                });

    });
