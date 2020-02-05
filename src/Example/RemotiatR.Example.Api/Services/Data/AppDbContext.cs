using Microsoft.EntityFrameworkCore;

namespace RemotiatR.Example.Api.Services.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodosEntity> Todos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
