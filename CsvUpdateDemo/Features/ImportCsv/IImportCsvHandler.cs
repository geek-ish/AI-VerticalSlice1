namespace CsvUpdateDemo.Features.ImportCsv
{
    public interface IImportCsvHandler
    {
        ImportCsvResult Handle(string csvText);
    }
}
