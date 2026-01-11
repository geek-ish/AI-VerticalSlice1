using CsvUpdateDemo.Infrastructure.Database;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace CsvUpdateDemo.Infrastructure.IoC
{
    public static class DependencyConfig
    {
        public static Container BuildContainer()
        {
            var container = new Container();

            // Scoped lifestyle for a console app
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Register EF DbContext as scoped
            container.Register<AppDbContext>(Lifestyle.Scoped);

            return container;
        }
    }
}
