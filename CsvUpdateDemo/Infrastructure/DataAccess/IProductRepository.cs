using System.Linq;
using CsvUpdateDemo.Infrastructure.Database;

namespace CsvUpdateDemo.Infrastructure.DataAccess
{
    public interface IProductRepository
    {
        IQueryable<Product> Query();
        void Add(Product product);
        int SaveChanges();
    }
}
