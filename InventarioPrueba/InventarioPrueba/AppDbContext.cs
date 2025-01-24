using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace InventarioPrueba
{
	public class AppDbContext: DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { 
			
		}

		public DbSet<Categoria> Categorias { get; set; }
		public DbSet<Cliente> Clientes { get; set; }
		public DbSet<Producto> Productos { get; set; }
		public DbSet<Venta> Ventas { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Producto>()
				.HasOne<Categoria>()
				.WithMany()
				.HasForeignKey(p => p.Categoria_id);

			modelBuilder.Entity<Venta>()
				.HasOne<Producto>()
				.WithMany()
				.HasForeignKey(v => v.Producto_id);
		}

		public class Categoria
		{
			public int Id { get; set; }
			public string? Nombre { get; set; }
			public string? Descripcion { get; set; }
		}

		public class Cliente
		{
			public int Id { get; set; }
			public string? Nombre { get; set; }
			public string? Apellido { get; set; }
			public string? Email { get; set; }
			public string? Telefono { get; set; }
		}

		public class Producto
		{
			public int Id { get; set; }
			public string? Nombre { get; set; }
			public string? Descripcion { get; set; }
			public decimal Precio { get; set; }
			public int Cantidad_stock { get; set; }
			public int Categoria_id { get; set; }
		}

		public class Venta
		{
			public int Id { get; set; }
			public int Producto_id { get; set; }
			public int Cantidad { get; set; }
			public DateTime Fecha_venta { get; set; }
			public decimal Total { get; set; }
		}
	}
}
