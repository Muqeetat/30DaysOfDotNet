using Microsoft.EntityFrameworkCore;
using SimpleTodoAPI;


public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	// This represents your table
	public DbSet<Todo> Todos => Set<Todo>();
}