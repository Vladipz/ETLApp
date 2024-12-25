using System.Globalization;

using ETLApp.Data;
using ETLApp.Data.Models;
using ETLApp.Services.ExtractService;
using ETLApp.Services.LoadService;
using ETLApp.Services.TransformService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ETLApp
{
    internal class Program
    {
        private static IServiceProvider _serviceProvider;
        private static IConfiguration _configuration;
        private static ILogger<Program> _logger;

        private static void Main(string[] args)
        {
            SetupDependencies();

            try
            {
                InitializeDatabase();
                RunEtlProcess();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ETL process failed");
                throw;
            }
        }

        private static void SetupDependencies()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _serviceProvider = new ServiceCollection()
                .Configure<DatabaseSettings>(_configuration.GetSection("Database"))
                .Configure<ScriptSettings>(_configuration.GetSection("Scripts"))
                .AddSingleton(_configuration)
                .AddLogging(builder =>
                {
                    builder.AddConfiguration(_configuration.GetSection("Logging"));
                    builder.AddConsole();
                })
                .AddTransient<CsvExtractor>()
                .AddTransient<DataTransformer>()
                .AddTransient<DatabaseInitializer>()
                .AddTransient<SqlLoader>()
                .BuildServiceProvider();

            _logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
        }

        private static void InitializeDatabase()
        {
            _logger.LogInformation("Initializing database...");
            var databaseInitializer = _serviceProvider.GetRequiredService<DatabaseInitializer>();
            databaseInitializer.Initialize();
            _logger.LogInformation("Database initialized successfully.");
        }

        private static void RunEtlProcess()
        {
            var csvExtractor = _serviceProvider.GetRequiredService<CsvExtractor>();
            var dataTransformer = _serviceProvider.GetRequiredService<DataTransformer>();
            var sqlLoader = _serviceProvider.GetRequiredService<SqlLoader>();

            var filePath = _configuration["CSV:FilePath"];

            _logger.LogInformation("Extracting data from CSV file...");
            var records = csvExtractor.Extract(filePath);
            _logger.LogInformation("Data extracted successfully.");

            _logger.LogInformation("Transforming data...");
            var transformedRecords = dataTransformer.Transform(records);
            _logger.LogInformation("Data transformed successfully.");

            _logger.LogInformation("Removing duplicates...");
            var duplicates = new List<EtlRecord>();
            var finalRecords = dataTransformer.RemoveDuplicates(transformedRecords, out duplicates);
            _logger.LogInformation("Removed {DuplicateCount} duplicates.", duplicates.Count);

            _logger.LogInformation("Loading data to SQL Server...");
            sqlLoader.Load(finalRecords, "ProcessedTrips");
            _logger.LogInformation("Data loaded successfully.");

            _logger.LogInformation("Writing duplicates to CSV file...");
            WriteDuplicatesToFile(duplicates);

            _logger.LogInformation("ETL process completed successfully.");
        }

        private static void WriteDuplicatesToFile(List<EtlRecord> duplicates)
        {
            var duplicatesFilePath = "duplicates.csv";

            using (var writer = new StreamWriter(duplicatesFilePath))
            using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(duplicates);
            }

            _logger.LogInformation("Duplicates written to {FilePath}.", duplicatesFilePath);
        }
    }
}