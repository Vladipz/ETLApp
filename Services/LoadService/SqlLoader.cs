using System.Data;

using ETLApp.Data.Models;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ETLApp.Services.LoadService
{
    public class SqlLoader
    {
        private readonly DatabaseSettings _dbSettings;

        public SqlLoader(IOptions<DatabaseSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        public void Load(IEnumerable<EtlRecord> records, string tableName)
        {
            string connectionString = _dbSettings.ConnectionString.Replace("{DatabaseName}", _dbSettings.Name, StringComparison.InvariantCulture);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = tableName;

                    // Map columns
                    bulkCopy.ColumnMappings.Add("PickupDateTime", "tpep_pickup_datetime");
                    bulkCopy.ColumnMappings.Add("DropoffDateTime", "tpep_dropoff_datetime");
                    bulkCopy.ColumnMappings.Add("PassengerCount", "passenger_count");
                    bulkCopy.ColumnMappings.Add("TripDistance", "trip_distance");
                    bulkCopy.ColumnMappings.Add("StoreAndFwdFlag", "store_and_fwd_flag");
                    bulkCopy.ColumnMappings.Add("PULocationID", "PULocationID");
                    bulkCopy.ColumnMappings.Add("DOLocationID", "DOLocationID");
                    bulkCopy.ColumnMappings.Add("FareAmount", "fare_amount");
                    bulkCopy.ColumnMappings.Add("TipAmount", "tip_amount");

                    // Load data
                    using (var dataTable = ToDataTable(records))
                    {
                        bulkCopy.WriteToServer(dataTable);
                    }
                }
            }
        }

        private DataTable ToDataTable(IEnumerable<EtlRecord> records)
        {
            var dataTable = new DataTable();

            // Add columns to the DataTable based on the properties of EtlRecord
            dataTable.Columns.Add("PickupDateTime", typeof(DateTime));
            dataTable.Columns.Add("DropoffDateTime", typeof(DateTime));
            dataTable.Columns.Add("PassengerCount", typeof(int));
            dataTable.Columns.Add("TripDistance", typeof(decimal));
            dataTable.Columns.Add("StoreAndFwdFlag", typeof(string));
            dataTable.Columns.Add("PULocationID", typeof(int));
            dataTable.Columns.Add("DOLocationID", typeof(int));
            dataTable.Columns.Add("FareAmount", typeof(decimal));
            dataTable.Columns.Add("TipAmount", typeof(decimal));

            // Populate the DataTable with data from the records
            foreach (var record in records)
            {
                var row = dataTable.NewRow();
                row["PickupDateTime"] = record.PickupDateTime;
                row["DropoffDateTime"] = record.DropoffDateTime;
                row["PassengerCount"] = record.PassengerCount;
                row["TripDistance"] = record.TripDistance;
                row["StoreAndFwdFlag"] = record.StoreAndFwdFlag;
                row["PULocationID"] = record.PULocationID;
                row["DOLocationID"] = record.DOLocationID;
                row["FareAmount"] = record.FareAmount;
                row["TipAmount"] = record.TipAmount;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}