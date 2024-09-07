

//this is Database connection manager 

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }  // Add other tables when ready
    public DbSet<ToDo> Todos { get; set; }
    public DbSet<Category> Categories { get; set; }
}


