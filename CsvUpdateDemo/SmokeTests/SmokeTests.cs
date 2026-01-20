using System;
using System.Collections.Generic;
using System.Linq;
using CsvUpdateDemo.Common;
using CsvUpdateDemo.Features.ApplyUpdates;
using CsvUpdateDemo.Features.GenerateUpdateReport;
using CsvUpdateDemo.Features.ImportCsv;
using CsvUpdateDemo.Infrastructure.DataAccess;
using CsvUpdateDemo.Infrastructure.Database;

namespace CsvUpdateDemo.SmokeTests
{
    public static class SmokeTests
    {
        public sealed class SmokeTestResult
        {
            public string Name { get; set; }
            public bool Passed { get; set; }
            public string Details { get; set; }
        }

        public static IList<SmokeTestResult> RunAll()
        {
            var results = new List<SmokeTestResult>();

            Run(results, "M2 ImportCsvHandler parses + validates", TestImportCsvHandler);
            Run(results, "M3 ApplyUpdatesHandler updates by SKU + warns on missing", TestApplyUpdatesHandler);
            Run(results, "M4 GenerateUpdateReportHandler formats report + optional file output", TestGenerateUpdateReportHandler);

            return results;
        }

        private static void Run(List<SmokeTestResult> results, string name, Action test)
        {
            try
            {
                test();
                results.Add(new SmokeTestResult { Name = name, Passed = true, Details = "OK" });
            }
            catch (Exception ex)
            {
                results.Add(new SmokeTestResult { Name = name, Passed = false, Details = ex.GetType().Name + ": " + ex.Message });
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        private static void TestImportCsvHandler()
        {
            var csv =
                "FullName,EmploymentType,UserId\r\n" +
                "Jane Doe,1,42\r\n" +
                "Bad Row,NOT_INT,7\r\n";

            var handler = new ImportCsvHandler();
            var result = handler.Handle(csv);

            Assert(result != null, "ImportCsvResult is null.");
            Assert(result.ValidRows.Count == 1, "Expected 1 valid row.");
            Assert(result.Errors.Count >= 1, "Expected at least 1 error for malformed row.");
        }

        private static void TestApplyUpdatesHandler()
        {
            var product = new Product
            {
                Id = 1,
                Sku = "ABC",
                Name = "Old",
                Price = 1.00m,
                Quantity = 1,
                UpdatedAtUtc = DateTime.MinValue
            };

            var repo = new InMemoryProductRepository(new[] { product });
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var clock = new FakeClock(now);

            var handler = new ApplyUpdatesHandler(repo, clock);

            var result = handler.Handle(new[]
            {
                new ProductUpdate { RowNumber = 1, Sku = "ABC", Price = 9.99m, Quantity = 5 },
                new ProductUpdate { RowNumber = 2, Sku = "MISSING", Price = 1.23m }
            });

            Assert(result != null, "ApplyUpdatesResult is null.");
            Assert(result.UpdatedCount == 1, "Expected UpdatedCount=1.");
            Assert(result.Warnings.Count == 1, "Expected 1 warning for missing SKU.");
            Assert(product.Price == 9.99m && product.Quantity == 5, "Product fields were not updated.");
            Assert(product.UpdatedAtUtc == now, "UpdatedAtUtc was not set via IClock.");
        }

        private static void TestGenerateUpdateReportHandler()
        {
            var import = new ImportCsvResult(
                new List<EmployeeImportRow> { new EmployeeImportRow { FullName = "Jane Doe", EmploymentType = 1, UserId = 42 } },
                new List<CsvRowError> { new CsvRowError { RowNumber = 2, ColumnName = "EmploymentType", Message = "Invalid int", RawRow = "Bad Row,NOT_INT,7" } }
            );

            var apply = new ApplyUpdatesResult(
                updatedCount: 1,
                insertedCount: 0,
                warnings: new List<ApplyUpdatesWarning> { new ApplyUpdatesWarning { RowNumber = 2, Sku = "MISSING", Message = "SKU not found. No update applied." } }
            );

            var clock = new FakeClock(new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc));
            var handler = new GenerateUpdateReportHandler(clock);

            var tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CsvUpdateDemo_SmokeReport.txt");
            if (System.IO.File.Exists(tempFile)) System.IO.File.Delete(tempFile);

            var report = handler.Handle(import, apply, new GenerateUpdateReportOptions
            {
                IncludeGeneratedAtUtc = true,
                IncludeRawRowOnErrors = true,
                OutputFilePath = tempFile
            });

            Assert(!string.IsNullOrWhiteSpace(report), "Report text is empty.");
            Assert(report.Contains("Import Summary"), "Report missing Import Summary.");
            Assert(report.Contains("Update Summary"), "Report missing Update Summary.");
            Assert(System.IO.File.Exists(tempFile), "Expected report file to be written.");
        }

        private sealed class FakeClock : IClock
        {
            public FakeClock(DateTime utcNow) { UtcNow = utcNow; }
            public DateTime UtcNow { get; private set; }
        }

        private sealed class InMemoryProductRepository : IProductRepository
        {
            private readonly List<Product> _products;

            public InMemoryProductRepository(IEnumerable<Product> seed)
            {
                _products = seed == null ? new List<Product>() : new List<Product>(seed);
            }

            public IQueryable<Product> Query()
            {
                return _products.AsQueryable();
            }

            public void Add(Product product)
            {
                _products.Add(product);
            }

            public int SaveChanges()
            {
                return 0;
            }
        }
    }
}
