using EstelaPruebaTecnica.Data;
using EstelaPruebaTecnica.DTOs;
using EstelaPruebaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstelaPruebaTecnica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdenesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CrearOrden([FromBody] CrearOrdenDto ordenDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var orden = new Orden { Detalles = new List<OrdenDetalle>() };

            foreach (var item in ordenDto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto == null)
                    return NotFound($"Producto ID {item.ProductoId} no encontrado.");

                if (producto.Stock < item.Cantidad)
                    return BadRequest($"Stock insuficiente para el producto {producto.Nombre}.");

                producto.Stock -= item.Cantidad;

                orden.Detalles.Add(new OrdenDetalle
                {
                    ProductoId = producto.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio
                });
            }

            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(orden);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerOrdenes()
        {
            var ordenes = await _context.Ordenes
                .Include(o => o.Detalles)
                    .ThenInclude(d => d.Producto)
                .Select(o => new
                {
                    o.Id,
                    o.Fecha,
                    Detalles = o.Detalles.Select(d => new
                    {
                        d.Producto.Nombre,
                        d.Cantidad,
                        d.PrecioUnitario,
                        Total = d.Cantidad * d.PrecioUnitario
                    }),
                    TotalOrden = o.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario)
                })
                .ToListAsync();

            return Ok(ordenes);
        }
    }
}
