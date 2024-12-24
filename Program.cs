using ETLApp.Data;
using ETLApp.Data.Models;
using ETLApp.Services.ExtractService;
using ETLApp.Services.LoadService;
using ETLApp.Services.TransformService;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static void Main(string[] args)
    {
        // Configuration setup
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Dependency injection
        var serviceProvider = new ServiceCollection()
            .AddSingleton(configuration)
            .AddTransient<CsvExtractor>()
            .AddTransient<DataTransformer>()
            .AddTransient(
                provider => new SqlLoader(configuration))
            .AddTransient(
                provider => new DatabaseInitializer(configuration))
            .BuildServiceProvider();

        // ExtractService
        var csvExtractor = serviceProvider.GetRequiredService<CsvExtractor>();
        var dataTransformer = serviceProvider.GetRequiredService<DataTransformer>();
        var sqlLoader = serviceProvider.GetRequiredService<SqlLoader>();

        // Database initialization
        Console.WriteLine("Initializing database...");
        var databaseInitializer = serviceProvider.GetRequiredService<DatabaseInitializer>();
        databaseInitializer.Initialize();
        Console.WriteLine("Database initialized successfully.");

        // Path to CSV file
        var filePath = configuration["CSV:FilePath"];

        try
        {
            Console.WriteLine("Extracting data from CSV file...");
            var records = csvExtractor.Extract(filePath);
            Console.WriteLine("Data extracted successfully.");

            Console.WriteLine("Transforming data...");
            var transformedRecords = dataTransformer.Transform(records);
            Console.WriteLine("Data transformed successfully.");

            Console.WriteLine("Removing duplicates...");
            var duplicates = new List<EtlRecord>();
            var finalRecords = dataTransformer.RemoveDuplicates(transformedRecords, out duplicates);
            Console.WriteLine($"Removed {duplicates.Count} duplicates.");

            Console.WriteLine("Loading data to SQL Server...");
            sqlLoader.Load(finalRecords, "ProcessedTrips");
            Console.WriteLine("Data loaded successfully.");

            Console.WriteLine("Writing duplicates to CSV file...");
            WriteDuplicatesToFile(duplicates);

            Console.WriteLine("ETL process completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ETL process failed: {ex.Message}");
            throw;
        }
    }

    // Метод для запису дублікованих записів у файл
    private static void WriteDuplicatesToFile(List<EtlRecord> duplicates)
    {
        var duplicatesFilePath = "duplicates.csv";

        using (var writer = new StreamWriter(duplicatesFilePath))
        using (var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(duplicates);
        }

        Console.WriteLine($"Duplicates written to {duplicatesFilePath}.");
    }
}