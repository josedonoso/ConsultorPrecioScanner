using ConsultorPrecio.Models;
using Microsoft.AspNetCore.Mvc;
using ConsultorPrecio.Services;

namespace ConsultorPrecio.Controllers;

public class ProductoController : Controller {
    private readonly AppDbContext _context;
    private readonly UPCService _upcService;

    public ProductoController(AppDbContext context, UPCService upcService) {
        _context = context;
        _upcService = upcService;
    }

    public IActionResult Consultar() {
        return View();
    }
    public IActionResult Crear() {
        return View();
    }

    [HttpGet]
    public IActionResult BuscarProducto(string codigoBarra) {

        var producto = _context.Productos
            .FirstOrDefault(p => p.CodigoBarra == codigoBarra);

        if (producto == null) {
            return Json(null);
        }

        return Json(producto);
    }

    [HttpPost]
    public IActionResult Crear(Producto producto) {

        if (!ModelState.IsValid) {
            return View(producto);
        }

        var productoExistente = _context.Productos
            .FirstOrDefault(p => p.CodigoBarra == producto.CodigoBarra);

        if (productoExistente != null) {

            productoExistente.Nombre = producto.Nombre;
            productoExistente.Precio = producto.Precio;

            _context.SaveChanges();

        }
        else {

            _context.Productos.Add(producto);
            _context.SaveChanges();
        }

        return RedirectToAction("Crear");
    }
    [HttpPost]
    public IActionResult Consultar(string? codigoBarra) {
        if (string.IsNullOrWhiteSpace(codigoBarra)) {
            return View();
        }

        codigoBarra = codigoBarra.Replace(" ", "").Trim();

        var producto = _context.Productos
            .FirstOrDefault(p => p.CodigoBarra == codigoBarra);

        return View(producto);
    }

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> BuscarPorCodigo(string codigoBarra) {
        if (string.IsNullOrWhiteSpace(codigoBarra)) {
            return Json(null);
        }

        codigoBarra =
            codigoBarra.Replace(" ", "").Trim();

        var producto =
            _context.Productos
            .FirstOrDefault(
                p => p.CodigoBarra == codigoBarra
            );

        if (producto == null) {
            producto =
                await _upcService
                .BuscarProducto(codigoBarra);

            if (producto != null) {
                _context.Productos.Add(producto);

                await _context.SaveChangesAsync();
            }
        }

        if (producto == null) {
            return Json(null);
        }

        return Json(new {
            nombre = producto.Nombre,
            precio = producto.Precio
        });
    }

    [HttpGet]
    public async Task<IActionResult> AutocompletarProducto(string codigoBarra) {
        if (string.IsNullOrWhiteSpace(codigoBarra))
            return Json(null);

        codigoBarra = codigoBarra.Trim();

        var producto = _context.Productos
            .FirstOrDefault(
                p => p.CodigoBarra == codigoBarra
            );

        // Si ya existe en tu BD
        if (producto != null) {
            return Json(new {
                nombre = producto.Nombre,
                precio = producto.Precio
            });
        }

        // Buscar API
        producto = await _upcService
            .BuscarProducto(codigoBarra);

        if (producto == null)
            return Json(null);

        return Json(new {
            nombre = producto.Nombre,
            precio = 0
        });
    }
}