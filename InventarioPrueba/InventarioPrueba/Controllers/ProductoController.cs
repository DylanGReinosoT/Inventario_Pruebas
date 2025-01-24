using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static InventarioPrueba.AppDbContext;

namespace InventarioPrueba.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProductosController : ControllerBase
	{
		private readonly AppDbContext _context;

		public ProductosController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
		{
			return await _context.Productos.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Producto>> GetProducto(int id)
		{
			var producto = await _context.Productos.FindAsync(id);

			if (producto == null)
			{
				return NotFound();
			}

			return producto;
		}

		[HttpPost]
		public async Task<ActionResult<Producto>> PostProducto(Producto producto)
		{
			// Validar que el precio no sea negativo
			if (producto.Precio < 0)
			{
				return BadRequest("El precio no puede ser un valor negativo.");
			}

			// Validar que la cantidad en stock no sea negativa
			if (producto.Cantidad_stock < 0)
			{
				return BadRequest("La cantidad en stock no puede ser un valor negativo.");
			}

			// Puedes agregar más validaciones si es necesario

			_context.Productos.Add(producto);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> PutProducto(int id, Producto producto)
		{
			if (id != producto.Id)
			{
				return BadRequest("El ID de la URL no coincide con el ID del producto.");
			}

			if (producto == null)
			{
				return BadRequest("El producto no puede ser nulo.");
			}

			if (string.IsNullOrEmpty(producto.Nombre))
			{
				return BadRequest("El nombre del producto es requerido.");
			}

			_context.Entry(producto).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ProductoExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProducto(int id)
		{
			var producto = await _context.Productos.FindAsync(id);
			if (producto == null)
			{
				return NotFound();
			}

			_context.Productos.Remove(producto);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool ProductoExists(int id)
		{
			return _context.Productos.Any(e => e.Id == id);
		}
	}
}
