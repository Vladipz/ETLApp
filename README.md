# ETL Project: CSV to SQL Server

This project implements a simple ETL (Extract, Transform, Load) process that imports data from a CSV file into a Microsoft SQL Server database. It's designed to handle large datasets efficiently and includes data cleaning and transformation steps.

## Features

- Extracts data from a CSV file
- Transforms and cleans the data:
  - Removes leading and trailing whitespace
  - Converts 'Y' and 'N' to 'Yes' and 'No' in the `store_and_fwd_flag` column
  - Removes duplicate records based on pickup datetime, dropoff datetime, and passenger count
- Loads the processed data into a SQL Server database
- Writes removed duplicates to a separate CSV file
- Optimized for efficient bulk insertion and specific query patterns

## Prerequisites

- .NET 8.0
- Microsoft SQL Server (local or cloud-based)
- SQL Server Management Studio (SSMS) or Azure Data Studio (optional, for database management)

## Installation

1. Clone the repository:

```bash
git clone https://github.com/Vladipz/ETLApp.git
```
2. Update appsettings.json if necessary:

3. Run the application:

```bash
dotnet run
```

## Assumptions and Notes

- The input CSV file is assumed to be in the project directory or a specified path.
- The application is designed to handle large datasets efficiently, but for extremely large files (e.g., 10GB+), additional optimizations may be necessary.
- Duplicate records are identified based on the combination of pickup datetime, dropoff datetime, and passenger count.
- The `store_and_fwd_flag` column is converted from 'Y'/'N' to 'Yes'/'No' for better readability.
- All text fields are trimmed to remove leading and trailing whitespace.
- The application assumes that the SQL Server has sufficient resources to handle the bulk insert operations.

## Handling Large Data Files (10GB CSV)

To efficiently process a 10GB CSV file, several optimizations and architectural changes would be necessary:

1. **Streaming Processing**: Implement a streaming approach to read and process the CSV file in chunks, rather than loading it entirely into memory. This would significantly reduce memory usage.

2. **Parallel Processing**: Utilize parallel processing techniques to distribute the workload across multiple CPU cores. This could involve splitting the file into segments and processing them concurrently.

3. **Batch Inserts**: Implement batch inserts for database operations to reduce the number of database connections and improve insertion speed.

4. **Indexing Strategy**: Carefully design the database indexing strategy to handle the increased data volume efficiently, considering the trade-offs between insert performance and query performance.

5. **Staging Tables**: Use staging tables for initial data load, performing transformations and data quality checks before moving data to the final tables.

6. **Resource Monitoring**: Implement comprehensive resource monitoring to track CPU, memory, and disk usage throughout the process.



# Technologies and Approaches Used

### 1. Dependency Injection (DI)
- Utilization of `IServiceCollection` and `IServiceProvider` for setting up and managing dependencies
- Registration of services using `AddTransient` methods

### 2. Configuration
- Use of `IConfiguration` and `ConfigurationBuilder` to load settings from `appsettings.json`
- Implementation of the Options pattern via `Configure<T>` for typed access to settings

### 3. Logging
- Utilization of `ILogger<T>` for structured logging
- Configuration of logging through `AddLogging` with support for console output

### 4. ETL Process
- Extraction: Use of `CsvExtractor` for reading data from CSV files
- Transformation: Implementation of `DataTransformer` for processing and transforming data
- Loading: Utilization of `SqlLoader` for loading data into SQL Server

### 5. Database Initialization
- Use of `DatabaseInitializer` to prepare the database before the ETL process

### 6. Error Handling
- Implementation of try-catch blocks for top-level exception handling

### 7. File I/O
- Use of `StreamWriter` for writing duplicates to a CSV file

### 8. CSV Handling
- Utilization of the CsvHelper library for working with CSV files

### 9. Culture-Independent Formatting
- Use of `CultureInfo.InvariantCulture` to ensure consistent formatting when working with CSV

### 10. Modular Structure
- Separation of functionality into distinct services (Extractor, Transformer, Loader)

### 11. Asynchronous Programming
- While asynchronous methods are not used in this code, the program structure allows for easy addition of asynchronicity in the future

### 12. Configuration via Configuration Files
- Use of `appsettings.json` for storing application settings

This code demonstrates the use of modern .NET development practices, including DI, configuration, logging, and modular architecture, making the application flexible, extensible, and easy to maintain.
