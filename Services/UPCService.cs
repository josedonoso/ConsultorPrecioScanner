using ConsultorPrecio.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ConsultorPrecio.Services {
    public class UPCService {
        private readonly HttpClient _httpClient;

        public UPCService(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        public async Task<Producto?> BuscarProducto(string codigo) {
            var response =
                await _httpClient.GetAsync(
                    $"https://world.openfoodfacts.org/api/v0/product/{codigo}.json"
                );

            if (!response.IsSuccessStatusCode)
                return null;

            var json =
                await response.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine(json);

            var datos =
                JsonDocument.Parse(json);

            var status =
                datos.RootElement
                .GetProperty("status")
                .GetInt32();

            if (status == 0)
                return null;

            var product =
                datos.RootElement
                .GetProperty("product");

            var productName = product.TryGetProperty("product_name", out var nombreProp)
                ? nombreProp.GetString() ?? string.Empty
                : string.Empty;

            var quantity = product.TryGetProperty("quantity", out var cantidadProp)
                ? cantidadProp.GetString() ?? string.Empty
                : string.Empty;

            var brandRaw = product.TryGetProperty("brands", out var brandProp)
                ? brandProp.GetString() ?? string.Empty
                : string.Empty;

            // Normalizar la marca: tomar la primera si hay múltiples y recortar espacios
            var brand = string.IsNullOrWhiteSpace(brandRaw)
                ? string.Empty
                : brandRaw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();

            // Detectar variantes de 'sabor original' de forma general
            var pnUpper = productName.ToUpperInvariant();
            var isOriginal = false;
            if (!string.IsNullOrWhiteSpace(pnUpper))
            {
                // Coincidencias como 'SABOR ORIGINAL' o la palabra 'ORIGINAL' aislada
                isOriginal = Regex.IsMatch(pnUpper, "\\bSABOR\\s+ORIGINAL\\b")
                             || Regex.IsMatch(pnUpper, "\\bORIGINAL\\b");
            }

            string nombreFinal;
            if (isOriginal)
            {
                if (!string.IsNullOrWhiteSpace(brand))
                {
                    nombreFinal = $"{brand} sabor original";
                }
                else
                {
                    nombreFinal = "Sabor original";
                }
            }
            else
            {
                var nameTrim = productName?.Trim() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(brand))
                {
                    // Si el nombre ya comienza con la marca (ignorando mayúsculas), no duplicarla
                    if (!string.IsNullOrWhiteSpace(nameTrim) &&
                        nameTrim.StartsWith(brand, System.StringComparison.OrdinalIgnoreCase))
                    {
                        nombreFinal = ($"{nameTrim} {quantity}").Trim();
                    }
                    else
                    {
                        nombreFinal = ($"{brand} {nameTrim} {quantity}").Trim();
                    }
                }
                else
                {
                    nombreFinal = ($"{nameTrim} {quantity}").Trim();
                }
            }

            return new Producto
            {
                CodigoBarra = codigo,
                Nombre = nombreFinal,
                Precio = 0
            };
        }
    }
}