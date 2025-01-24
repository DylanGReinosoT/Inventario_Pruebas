using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static InventarioPrueba.AppDbContext;

namespace InventarioPrueba.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class VentaController : ControllerBase
	{
		private readonly AppDbContext _context;

		public VentaController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
		{
			return await _context.Ventas.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Venta>> GetVenta(int id)
		{
			var venta = await _context.Ventas.FindAsync(id);

			if (venta == null)
			{
				return NotFound();
			}

			return venta;
		}

		[HttpPost]
		public async Task<ActionResult<Venta>> PostVenta(Venta venta)
		{
			// Verificar que la venta no sea nula
			if (venta == null)
			{
				return BadRequest("La venta no puede ser nula.");
			}

			// Buscar el producto asociado a la venta
			var producto = await _context.Productos.FindAsync(venta.Producto_id);
			if (producto == null)
			{
				return BadRequest("El producto especificado no existe.");
			}

			// Validar que la cantidad solicitada sea mayor que cero
			if (venta.Cantidad <= 0)
			{
				return BadRequest("La cantidad vendida debe ser mayor que cero.");
			}

			// Verificar que la cantidad vendida no exceda el stock disponible
			if (venta.Cantidad > producto.Cantidad_stock)
			{
				return BadRequest("La cantidad solicitada excede el stock disponible.");
			}

			// Actualizar el stock del producto
			producto.Cantidad_stock -= venta.Cantidad;
			_context.Entry(producto).State = EntityState.Modified;

			// Calcular el total de la venta
			venta.Total = producto.Precio * venta.Cantidad;

			// Registrar la fecha de la venta
			venta.Fecha_venta = DateTime.Now;

			// Agregar la venta al contexto
			_context.Ventas.Add(venta);

			// Guardar los cambios en una transacción
			using (var transaction = await _context.Database.BeginTransactionAsync())
			{
				try
				{
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
					return StatusCode(500, "Ocurrió un error al procesar la venta.");
				}
			}

			return CreatedAtAction(nameof(GetVenta), new { id = venta.Id }, venta);
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> PutVenta(int id, Venta venta)
		{
			if (id != venta.Id)
			{
				return BadRequest("El ID de la URL no coincide con el ID de la venta.");
			}

			if (venta == null)
			{
				return BadRequest("La venta no puede ser nula.");
			}

			_context.Entry(venta).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!VentaExists(id))
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
		public async Task<IActionResult> DeleteVenta(int id)
		{
			var venta = await _context.Ventas.FindAsync(id);
			if (venta == null)
			{
				return NotFound();
			}

			_context.Ventas.Remove(venta);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool VentaExists(int id)
		{
			return _context.Ventas.Any(e => e.Id == id);
		}
	}
}
