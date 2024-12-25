using ETLApp.Data.Models;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ETLApp.Data
{
    public class DatabaseInitializer
    {
        private DatabaseSettings _databaseSettings;
        private ScriptSettings _scriptSettings;

        public DatabaseInitializer(
            IOptions<DatabaseSettings> databaseSettings,
            IOptions<ScriptSettings> scriptSettings)
        {
            _databaseSettings = databaseSettings.Value;
            _scriptSettings = scriptSettings.Value;
        }

        public void Initialize()
        {
            string masterConnectionString = _databaseSettings.MasterConnectionString;
            string databaseName = _databaseSettings.Name;

            // Create database if not exists
            CreateDatabaseIfNotExists(masterConnectionString, databaseName);

            // Now use a connection string for the specific database
            string databaseConnectionString = _databaseSettings.ConnectionString.Replace("{DatabaseName}", databaseName);

            string createTableScript = File.ReadAllText(_scriptSettings.CreateTable);
            string addIndexesScript = File.ReadAllText(_scriptSettings.AddIndexes);

            using (var connection = new SqlConnection(databaseConnectionString))
            {
                connection.Open();

                // Check if the table exists before creating it
                if (!TableExists(connection, "ProcessedTrips"))
                {
                    using (var command = new SqlCommand(createTableScript, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                // Check if index exists before adding it
                if (!IndexExists(connection, "idx_tip_amount_avg"))
                {
                    using (var command = new SqlCommand(addIndexesScript, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void CreateDatabaseIfNotExists(string masterConnectionString, string databaseName)
        {
            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();

                string checkDbExistsQuery = $"SELECT database_id FROM sys.databases WHERE Name = '{databaseName}'";
                using (var checkCommand = new SqlCommand(checkDbExistsQuery, connection))
                {
                    object result = checkCommand.ExecuteScalar();
                    if (result == null)
                    {
                        // Database doesn't exist, create it
                        string createDbQuery = $"CREATE DATABASE [{databaseName}]";
                        using (var createCommand = new SqlCommand(createDbQuery, connection))
                        {
                            createCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private bool TableExists(SqlConnection connection, string tableName)
        {
            string query = $"SELECT 1 FROM sysobjects WHERE name = '{tableName}' AND xtype = 'U'";
            using (var command = new SqlCommand(query, connection))
            {
                return command.ExecuteScalar() != null;
            }
        }

        private bool IndexExists(SqlConnection connection, string indexName)
        {
            string query = $"SELECT 1 FROM sys.indexes WHERE name = '{indexName}' AND object_id = OBJECT_ID('ProcessedTrips')";
            using (var command = new SqlCommand(query, connection))
            {
                return command.ExecuteScalar() != null;
            }
        }
    }
}