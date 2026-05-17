
let ultimoCodigo = "";
let bloqueado = false;

async function buscarCodigo(codigo) {

    if (bloqueado) return;

    codigo = codigo.replace(/\s/g, "").trim();

    if (codigo.length < 8 || codigo === ultimoCodigo) return;

    ultimoCodigo = codigo;
    bloqueado = true;

    document.getElementById("codigoBarra").value = codigo;

    const respuesta = await fetch("/Producto/BuscarPorCodigo?codigoBarra=" + encodeURIComponent(codigo));

    const producto = await respuesta.json();

    if (producto) {

        const precioCL = new Intl.NumberFormat("es-CL", {
            style: "currency",
            currency: "CLP",
            maximumFractionDigits: 0
        }).format(producto.precio);

        document.getElementById("resultado").innerHTML = `
    <h2>
        ${producto.nombre
                .toLowerCase()
                .replace(/\b\w/g, l => l.toUpperCase())}
    </h2>
    <h1 style="color:green; font-size:48px;">
        ${precioCL}
    </h1>
    `;

    } else {

        document.getElementById("resultado").innerHTML = `
                <h2 style="color:red;">
                Producto no encontrado
            </h2>
            `;
    }

    setTimeout(() => {
        bloqueado = false;
    }, 700);
}

const html5QrCode = new Html5Qrcode("reader");

html5QrCode.start(
    {
        facingMode: "environment"
    },
    {
        fps: 60,
        //franja horizontal
        qrbox: {
            width: 320,
            height: 90
        },

        aspectRatio: 1.777,
        disableFlip: true,

        experimentalFeatures: {
            useBarCodeDetectorIfSupported: true
        },

        videoConstraints: {
            facingMode: "environment",
            width: { ideal: 1920 },
            height: { ideal: 1080}
        },

        formatsToSupport: [
            Html5QrcodeSupportedFormats.EAN_13,
            Html5QrcodeSupportedFormats.EAN_8,
            Html5QrcodeSupportedFormats.UPC_A,
            Html5QrcodeSupportedFormats.UPC_E,
            Html5QrcodeSupportedFormats.CODE_128,
            Html5QrcodeSupportedFormats.CODE_39
        ]
    },
    (decodedText) => {
        buscarCodigo(decodedText);
    }
);

function limpiarBusqueda() {

    ultimoCodigo = "";

    document.getElementById("codigoBarra").value = "";
    document.getElementById("resultado").innerHTML = "";

}