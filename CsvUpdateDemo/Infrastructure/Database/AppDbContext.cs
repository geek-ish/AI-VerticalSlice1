using System.Data.Entity;

namespace CsvUpdateDemo.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=CsvUpdateDemo")
        {
            System.Data.Entity.Database.SetInitializer<AppDbContext>(
                new CreateDatabaseIfNotExists<AppDbContext>());
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
