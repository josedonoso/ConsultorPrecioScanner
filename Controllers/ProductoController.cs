using ConsultorPrecio.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConsultorPrecio.Controllers;

public class ProductoController : Controller {
    private readonly AppDbContext _context;

    public ProductoController(AppDbContext context) {
        _context = context;
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
    public IActionResult BuscarPorCodigo(string codigoBarra) {
        if (string.IsNullOrWhiteSpace(codigoBarra)) {
            return Json(null);
        }

        codigoBarra = codigoBarra.Replace(" ", "").Trim();

        var producto = _context.Productos
            .FirstOrDefault(p => p.CodigoBarra == codigoBarra);

        if (producto == null) {
            return Json(null);
        }

        return Json(new {
            nombre = producto.Nombre,
            precio = producto.Precio
        });
    }
}