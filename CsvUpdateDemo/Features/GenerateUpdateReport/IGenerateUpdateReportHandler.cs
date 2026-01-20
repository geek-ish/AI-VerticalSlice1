using CsvUpdateDemo.Features.ApplyUpdates;
using CsvUpdateDemo.Features.ImportCsv;

namespace CsvUpdateDemo.Features.GenerateUpdateReport
{
    public interface IGenerateUpdateReportHandler
    {
        string Handle(ImportCsvResult importResult, ApplyUpdatesResult applyResult, GenerateUpdateReportOptions options);
    }
}
