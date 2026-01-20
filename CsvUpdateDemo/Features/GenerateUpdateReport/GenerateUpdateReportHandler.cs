using System;
using System.IO;
using System.Text;
using CsvUpdateDemo.Common;
using CsvUpdateDemo.Features.ApplyUpdates;
using CsvUpdateDemo.Features.ImportCsv;

namespace CsvUpdateDemo.Features.GenerateUpdateReport
{
    public sealed class GenerateUpdateReportHandler : IGenerateUpdateReportHandler
    {
        private readonly IClock _clock;

        public GenerateUpdateReportHandler(IClock clock)
        {
            _clock = clock;
        }

        public string Handle(ImportCsvResult importResult, ApplyUpdatesResult applyResult, GenerateUpdateReportOptions options)
        {
            if (options == null) options = new GenerateUpdateReportOptions();

            var sb = new StringBuilder(2048);

            sb.AppendLine("CSV Update Report");
            sb.AppendLine("===============");

            if (options.IncludeGeneratedAtUtc)
                sb.AppendLine("Generated (UTC): " + _clock.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            sb.AppendLine();

            var importValid = importResult == null || importResult.ValidRows == null ? 0 : importResult.ValidRows.Count;
            var importErrors = importResult == null || importResult.Errors == null ? 0 : importResult.Errors.Count;

            sb.AppendLine("Import Summary");
            sb.AppendLine("--------------");
            sb.AppendLine("Valid rows: " + importValid);
            sb.AppendLine("Errors:    " + importErrors);
            sb.AppendLine();

            if (importErrors > 0)
            {
                sb.AppendLine("Import Errors");
                sb.AppendLine("-------------");
                for (var i = 0; i < importResult.Errors.Count; i++)
                {
                    var e = importResult.Errors[i];
                    sb.AppendLine("- " + e.ToString());
                    if (options.IncludeRawRowOnErrors && !string.IsNullOrWhiteSpace(e.RawRow))
                        sb.AppendLine("  Raw: " + e.RawRow);
                }
                sb.AppendLine();
            }

            var updated = applyResult == null ? 0 : applyResult.UpdatedCount;
            var inserted = applyResult == null ? 0 : applyResult.InsertedCount;
            var warnings = applyResult == null || applyResult.Warnings == null ? 0 : applyResult.Warnings.Count;

            sb.AppendLine("Update Summary");
            sb.AppendLine("--------------");
            sb.AppendLine("Updated:  " + updated);
            sb.AppendLine("Inserted: " + inserted);
            sb.AppendLine("Warnings: " + warnings);
            sb.AppendLine();

            if (warnings > 0)
            {
                sb.AppendLine("Update Warnings");
                sb.AppendLine("---------------");
                for (var i = 0; i < applyResult.Warnings.Count; i++)
                    sb.AppendLine("- " + applyResult.Warnings[i].ToString());
                sb.AppendLine();
            }

            sb.AppendLine("End of report.");
            var report = sb.ToString();

            if (!string.IsNullOrWhiteSpace(options.OutputFilePath))
                WriteTextFile(options.OutputFilePath, report);

            return report;
        }

        private static void WriteTextFile(string path, string content)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, content ?? "", Encoding.UTF8);
        }
    }
}
