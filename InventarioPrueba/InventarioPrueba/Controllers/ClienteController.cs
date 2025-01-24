using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static InventarioPrueba.AppDbContext;

namespace InventarioPrueba.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ClienteController : ControllerBase
	{
		private readonly AppDbContext _context;

		public ClienteController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
		{
			return await _context.Clientes.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Cliente>> GetCliente(int id)
		{
			var cliente = await _context.Clientes.FindAsync(id);

			if (cliente == null)
			{
				return NotFound();
			}

			return cliente;
		}

		[HttpPost]
		public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
		{
			_context.Clientes.Add(cliente);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutCliente(int id, Cliente cliente)
		{
			if (id != cliente.Id)
			{
				return BadRequest("El ID de la URL no coincide con el ID del cliente.");
			}

			if (cliente == null)
			{
				return BadRequest("El cliente no puede ser nulo.");
			}

			if (string.IsNullOrEmpty(cliente.Nombre))
			{
				return BadRequest("El nombre del cliente es requerido.");
			}

			_context.Entry(cliente).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!ClienteExists(id))
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
		public async Task<IActionResult> DeleteCliente(int id)
		{
			var cliente = await _context.Clientes.FindAsync(id);
			if (cliente == null)
			{
				return NotFound();
			}

			_context.Clientes.Remove(cliente);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool ClienteExists(int id)
		{
			return _context.Clientes.Any(e => e.Id == id);
		}
	}
}
