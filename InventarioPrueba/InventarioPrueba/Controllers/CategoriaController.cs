using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static InventarioPrueba.AppDbContext;

namespace InventarioPrueba.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CategoriaController : ControllerBase
	{
		private readonly AppDbContext _context;

		public CategoriaController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Categoria>>> GetCategoria()
		{
			return await _context.Categorias.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Categoria>> GetCategoria(int id)
		{
			var categoria = await _context.Categorias.FindAsync(id);

			if (categoria == null)
			{
				return NotFound();
			}

			return categoria;
		}

		[HttpPost]
		public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
		{
			_context.Categorias.Add(categoria);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
		{
			if (id != categoria.Id)
			{
				return BadRequest("El ID de la URL no coincide con el ID de la categoría.");
			}

			if (categoria == null)
			{
				return BadRequest("La categoría no puede ser nula.");
			}

			if (string.IsNullOrEmpty(categoria.Nombre))
			{
				return BadRequest("El nombre de la categoría es requerido.");
			}

			_context.Entry(categoria).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CategoriaExists(id))
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
		public async Task<IActionResult> DeleteCategoria(int id)
		{
			var categoria = await _context.Categorias.FindAsync(id);
			if (categoria == null)
			{
				return NotFound();
			}

			_context.Categorias.Remove(categoria);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool CategoriaExists(int id)
		{
			return _context.Categorias.Any(e => e.Id == id);
		}
	}
}
