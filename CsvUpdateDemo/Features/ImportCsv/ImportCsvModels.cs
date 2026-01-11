using System.Collections.Generic;

namespace CsvUpdateDemo.Features.ImportCsv
{
    public sealed class EmployeeImportRow
    {
        public int RowNumber { get; set; } // 1-based data-row number (excluding header)
        public string FullName { get; set; }
        public string Title { get; set; }
        public int EmploymentType { get; set; }
        public string Location { get; set; }
        public int UserId { get; set; }
    }

    public sealed class CsvRowError
    {
        public int RowNumber { get; set; }     // 1-based data-row number (excluding header). 0 = header / file-level.
        public string ColumnName { get; set; } // optional
        public string Message { get; set; }
        public string RawRow { get; set; }     // best-effort

        public override string ToString()
        {
            var col = string.IsNullOrWhiteSpace(ColumnName) ? "" : (ColumnName + ": ");
            return "Row " + RowNumber + " - " + col + Message;
        }
    }

    public sealed class ImportCsvResult
    {
        public ImportCsvResult(List<EmployeeImportRow> validRows, List<CsvRowError> errors)
        {
            ValidRows = validRows ?? new List<EmployeeImportRow>();
            Errors = errors ?? new List<CsvRowError>();
        }

        public IReadOnlyList<EmployeeImportRow> ValidRows { get; private set; }
        public IReadOnlyList<CsvRowError> Errors { get; private set; }

        public bool HasErrors { get { return Errors != null && Errors.Count > 0; } }
    }
}
