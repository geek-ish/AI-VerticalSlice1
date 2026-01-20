using System;

namespace CsvUpdateDemo.Features.GenerateUpdateReport
{
    public sealed class GenerateUpdateReportOptions
    {
        public string OutputFilePath { get; set; }
        public bool IncludeGeneratedAtUtc { get; set; }
        public bool IncludeRawRowOnErrors { get; set; }
    }
}
