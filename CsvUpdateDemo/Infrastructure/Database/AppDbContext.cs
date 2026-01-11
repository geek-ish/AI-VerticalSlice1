using System.Data.Entity;

namespace CsvUpdateDemo.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        // Uses connection string "CsvUpdateDemo" in App.config
        public AppDbContext() : base("name=CsvUpdateDemo")
        {
            // Keep it explicit: Create DB + schema if missing (Code First)
            //Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
            System.Data.Entity.Database.SetInitializer<AppDbContext>(new CreateDatabaseIfNotExists<AppDbContext>());
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Convention-based mapping is enough for Milestone 1.
            // Place advanced mappings here later as vertical slices are added.
            base.OnModelCreating(modelBuilder);
        }
    }
}
