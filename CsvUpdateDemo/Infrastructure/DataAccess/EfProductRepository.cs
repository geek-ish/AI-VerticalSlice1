using System.Linq;
using CsvUpdateDemo.Infrastructure.Database;

namespace CsvUpdateDemo.Infrastructure.DataAccess
{
    public sealed class EfProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;

        public EfProductRepository(AppDbContext db)
        {
            _db = db;
        }

        public IQueryable<Product> Query()
        {
            return _db.Products;
        }

        public void Add(Product product)
        {
            _db.Products.Add(product);
        }

        public int SaveChanges()
        {
            return _db.SaveChanges();
        }
    }
}
