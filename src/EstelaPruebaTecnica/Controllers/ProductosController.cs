using EstelaPruebaTecnica.Data;
using EstelaPruebaTecnica.Models;
using Microsoft.AspNetCore.Mvc;

namespace EstelaPruebaTecnica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
                return BadRequest("El nombre no puede estar vacío.");

            if (producto.Precio <= 0)
                return BadRequest("El precio debe ser mayor a cero.");

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return Ok(producto);
        }
    }
}
