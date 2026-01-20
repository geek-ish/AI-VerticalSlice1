using System;
using System.Collections.Generic;
using System.Linq;
using CsvUpdateDemo.Common;
using CsvUpdateDemo.Infrastructure.DataAccess;
using CsvUpdateDemo.Infrastructure.Database;

namespace CsvUpdateDemo.Features.ApplyUpdates
{
    public sealed class ApplyUpdatesHandler : IApplyUpdatesHandler
    {
        private readonly IProductRepository _repo;
        private readonly IClock _clock;

        public ApplyUpdatesHandler(IProductRepository repo, IClock clock)
        {
            _repo = repo;
            _clock = clock;
        }

        public ApplyUpdatesResult Handle(IEnumerable<ProductUpdate> updates)
        {
            var warnings = new List<ApplyUpdatesWarning>();
            var updated = 0;

            if (updates == null)
                return new ApplyUpdatesResult(0, 0, warnings);

            var list = updates.Where(u => u != null).ToList();
            var skus = list
                .Select(u => (u.Sku ?? "").Trim())
                .Where(s => s.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var productsBySku = _repo.Query()
                .Where(p => skus.Contains(p.Sku))
                .ToList()
                .ToDictionary(p => p.Sku, StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < list.Count; i++)
            {
                var u = list[i];
                var sku = (u.Sku ?? "").Trim();

                if (sku.Length == 0)
                {
                    warnings.Add(new ApplyUpdatesWarning { RowNumber = u.RowNumber, Sku = u.Sku, Message = "SKU is required." });
                    continue;
                }

                Product p;
                if (!productsBySku.TryGetValue(sku, out p))
                {
                    warnings.Add(new ApplyUpdatesWarning { RowNumber = u.RowNumber, Sku = sku, Message = "SKU not found. No update applied." });
                    continue;
                }

                var changed = ApplyToEntity(p, u);
                if (changed)
                {
                    p.UpdatedAtUtc = _clock.UtcNow;
                    updated++;
                }
            }

            if (updated > 0)
                _repo.SaveChanges();

            return new ApplyUpdatesResult(updated, 0, warnings);
        }

        private static bool ApplyToEntity(Product p, ProductUpdate u)
        {
            var changed = false;

            if (u.Name != null && !string.Equals(p.Name, u.Name, StringComparison.Ordinal))
            {
                p.Name = u.Name;
                changed = true;
            }

            if (u.Price.HasValue && p.Price != u.Price.Value)
            {
                p.Price = u.Price.Value;
                changed = true;
            }

            if (u.Quantity.HasValue && p.Quantity != u.Quantity.Value)
            {
                p.Quantity = u.Quantity.Value;
                changed = true;
            }

            return changed;
        }
    }
}
