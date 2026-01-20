using CsvUpdateDemo.Common;
using CsvUpdateDemo.Features.ApplyUpdates;
using CsvUpdateDemo.Features.ImportCsv;
using CsvUpdateDemo.Infrastructure.DataAccess;
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
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.Register<AppDbContext>(Lifestyle.Scoped);
            container.Register<IProductRepository, EfProductRepository>(Lifestyle.Scoped);

            container.Register<IClock, SystemClock>(Lifestyle.Singleton);

            container.Register<IImportCsvHandler, ImportCsvHandler>(Lifestyle.Singleton);
            container.Register<IApplyUpdatesHandler, ApplyUpdatesHandler>(Lifestyle.Scoped);

            return container;
        }
    }
}
