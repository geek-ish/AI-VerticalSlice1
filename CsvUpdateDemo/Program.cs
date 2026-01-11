using System;
using CsvUpdateDemo.Infrastructure.Database;
using CsvUpdateDemo.Infrastructure.IoC;
using SimpleInjector.Lifestyles;

namespace CsvUpdateDemo
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("CsvUpdateDemo — Milestone 1 startup");
            Console.WriteLine("----------------------------------");

            var container = DependencyConfig.BuildContainer();

            // Verify DI setup
            container.Verify();
            Console.WriteLine("[OK] DI container verification passed.");

            // Create a scope, resolve DbContext, and create DB + schema if needed
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                var db = container.GetInstance<AppDbContext>();

                var created = db.Database.CreateIfNotExists();
                Console.WriteLine(created
                    ? "[OK] Database created (CsvUpdateDemo) and schema initialized."
                    : "[OK] Database already exists (CsvUpdateDemo).");

                // Force initialization of the model & connection early
                var canConnect = db.Database.Exists();
                Console.WriteLine(canConnect
                    ? "[OK] SQL Server Express connectivity verified."
                    : "[WARN] Unable to verify DB existence (check SQL Express instance).");
            }

            Console.WriteLine();
            Console.WriteLine("Ready for Vertical Slice feature development.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
