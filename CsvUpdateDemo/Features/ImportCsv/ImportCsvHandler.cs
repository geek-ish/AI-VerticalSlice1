using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace CsvUpdateDemo.Features.ImportCsv
{
    public sealed class ImportCsvHandler : IImportCsvHandler
    {
        private const int FullNameMax = 200;
        private const int TitleMax = 120;
        private const int LocationMax = 120;

        public ImportCsvResult Handle(string csvText)
        {
            var valid = new List<EmployeeImportRow>();
            var errors = new List<CsvRowError>();

            if (string.IsNullOrWhiteSpace(csvText))
            {
                errors.Add(new CsvRowError { RowNumber = 0, Message = "CSV content is empty.", RawRow = "" });
                return new ImportCsvResult(valid, errors);
            }

            using (var reader = new StringReader(csvText))
            using (var parser = new TextFieldParser(reader))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;

                string[] header;
                try
                {
                    header = parser.ReadFields();
                }
                catch (MalformedLineException ex)
                {
                    errors.Add(new CsvRowError { RowNumber = 0, Message = "Malformed header row: " + ex.Message, RawRow = "" });
                    return new ImportCsvResult(valid, errors);
                }

                var map = BuildHeaderMap(header, errors);

                var dataRowIndex = 0;
                while (!parser.EndOfData)
                {
                    dataRowIndex++;
                    string[] fields;
                    try
                    {
                        fields = parser.ReadFields();
                    }
                    catch (MalformedLineException ex)
                    {
                        errors.Add(new CsvRowError
                        {
                            RowNumber = dataRowIndex,
                            Message = "Malformed row (parser): " + ex.Message,
                            RawRow = ""
                        });
                        continue;
                    }

                    if (fields == null) continue;

                    var rowErrors = new List<CsvRowError>();
                    var row = ParseRow(fields, map, dataRowIndex, rowErrors);

                    if (rowErrors.Count > 0)
                    {
                        var raw = SafeJoin(fields);
                        for (var i = 0; i < rowErrors.Count; i++)
                        {
                            if (string.IsNullOrWhiteSpace(rowErrors[i].RawRow))
                                rowErrors[i].RawRow = raw;
                            errors.Add(rowErrors[i]);
                        }
                        continue;
                    }

                    valid.Add(row);
                }
            }

            return new ImportCsvResult(valid, errors);
        }

        private static Dictionary<string, int> BuildHeaderMap(string[] header, List<CsvRowError> errors)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            header = header ?? new string[0];
            for (var i = 0; i < header.Length; i++)
            {
                var key = (header[i] ?? "").Trim();
                if (key.Length == 0) continue;
                if (!map.ContainsKey(key)) map.Add(key, i);
            }

            Require(map, "FullName", errors);
            Require(map, "EmploymentType", errors);
            Require(map, "UserId", errors);

            return map;
        }

        private static void Require(Dictionary<string, int> map, string name, List<CsvRowError> errors)
        {
            if (!map.ContainsKey(name))
                errors.Add(new CsvRowError { RowNumber = 0, ColumnName = name, Message = "Missing required column in header." });
        }

        private EmployeeImportRow ParseRow(string[] fields, Dictionary<string, int> map, int rowNumber, List<CsvRowError> errors)
        {
            var fullName = Get(fields, map, "FullName", rowNumber, errors, required: true);
            var title = Get(fields, map, "Title", rowNumber, errors, required: false);
            var location = Get(fields, map, "Location", rowNumber, errors, required: false);

            var employmentType = GetInt(fields, map, "EmploymentType", rowNumber, errors, required: true);
            var userId = GetInt(fields, map, "UserId", rowNumber, errors, required: true);

            if (!string.IsNullOrEmpty(fullName) && fullName.Length > FullNameMax)
                errors.Add(new CsvRowError { RowNumber = rowNumber, ColumnName = "FullName", Message = "Max length is " + FullNameMax + "." });

            if (!string.IsNullOrEmpty(title) && title.Length > TitleMax)
                errors.Add(new CsvRowError { RowNumber = rowNumber, ColumnName = "Title", Message = "Max length is " + TitleMax + "." });

            if (!string.IsNullOrEmpty(location) && location.Length > LocationMax)
                errors.Add(new CsvRowError { RowNumber = rowNumber, ColumnName = "Location", Message = "Max length is " + LocationMax + "." });

            if (errors.Count > 0) return null;

            return new EmployeeImportRow
            {
                RowNumber = rowNumber,
                FullName = fullName,
                Title = title,
                EmploymentType = employmentType,
                Location = location,
                UserId = userId
            };
        }

        private static string Get(string[] fields, Dictionary<string, int> map, string name, int rowNumber, List<CsvRowError> errors, bool required)
        {
            int idx;
            if (!map.TryGetValue(name, out idx))
            {
                if (required)
                    errors.Add(new CsvRowError { RowNumber = rowNumber, ColumnName = name, Message = "Required column missing from header." });
                return null;
            }

            if (idx < 0 || idx >= fields.Length)
            {
                errors.Add(new CsvRowError { RowNumber = rowNumber, ColumnName = name, Message = "Missing field value (column index out of range)." });
                return null;
            }

            var v = (fields[idx] ?? "").Trim();

            if (required && string.IsNullOrWhiteSpace(v))
                errors.Add(new CsvRowError { RowNumber = rowNumber, ColumnName = name, Message = "Value is required." });

            return string.IsNullOrWhiteSpace(v) ? null : v;
        }

        private static int GetInt(string[] fields, Dictionary<string, int> map, string name, int rowNumber, List<CsvRowError> errors, bool required)
        {
            var s = Get(fields, map, name, rowNumber, errors, required);
            if (string.IsNullOrWhiteSpace(s)) return 0;

            int v;
            if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
            {
                errors.Add(new CsvRowError { RowNumber = rowNumber, ColumnName = name, Message = "Invalid integer: '" + s + "'." });
                return 0;
            }
            return v;
        }

        private static string SafeJoin(string[] fields)
        {
            try { return string.Join(",", fields ?? new string[0]); }
            catch { return ""; }
        }
    }
}
