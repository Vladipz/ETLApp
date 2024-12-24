using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using ETLApp.Data.Models;
using ETLApp.Utils;

namespace ETLApp.Services.ExtractService
{
    public class CsvExtractor
    {
        public IEnumerable<EtlRecord> Extract(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim, // Set trim options here
                BadDataFound = null, // Set BadDataFound to null here
                ShouldSkipRecord = static (args) =>
                        args.Row.Parser.Record.Any(field => string.IsNullOrWhiteSpace(field)),
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config)) // Pass the config to the CsvReader
            {
                csv.Context.RegisterClassMap<EtlRecordMap>(); // Register the class map here
                return csv.GetRecords<EtlRecord>().ToList();
            }
        }
    }
}