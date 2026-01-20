using System.Collections.Generic;

namespace CsvUpdateDemo.Features.ApplyUpdates
{
    public sealed class ProductUpdate
    {
        public int RowNumber { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
    }

    public sealed class ApplyUpdatesWarning
    {
        public int RowNumber { get; set; }
        public string Sku { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return "Row " + RowNumber + " (SKU " + (Sku ?? "") + "): " + (Message ?? "");
        }
    }

    public sealed class ApplyUpdatesResult
    {
        public ApplyUpdatesResult(int updatedCount, int insertedCount, List<ApplyUpdatesWarning> warnings)
        {
            UpdatedCount = updatedCount;
            InsertedCount = insertedCount;
            Warnings = warnings ?? new List<ApplyUpdatesWarning>();
        }

        public int UpdatedCount { get; private set; }
        public int InsertedCount { get; private set; }
        public IReadOnlyList<ApplyUpdatesWarning> Warnings { get; private set; }
    }
}
