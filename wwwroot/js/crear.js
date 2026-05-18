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
        fps: 30,
        qrbox: { 
            width: 300, 
            height: 90 
        },
        aspectRatio: 1.777,
        disableFlip: true,

        experimentalFeatures: {
            useBarCodeDetectorIfSupported: true
        },

        formatsToSupport: [
            Html5QrcodeSupportedFormats.EAN_13,
            Html5QrcodeSupportedFormats.EAN_8,
            Html5QrcodeSupportedFormats.UPC_A,
            Html5QrcodeSupportedFormats.UPC_E,
            Html5QrcodeSupportedFormats.CODE_128
        ]
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

        setTimeout(() => {
            ultimoCodigo = "";
        }, 500);

    },
    () => { }
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
