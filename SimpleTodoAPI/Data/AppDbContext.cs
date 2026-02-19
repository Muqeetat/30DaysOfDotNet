using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI.Models;


namespace SimpleTodoAPI.Data
{ 
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		// This represents your table
		public DbSet<Todo> Todos => Set<Todo>();
	}
}