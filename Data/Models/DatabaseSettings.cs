namespace ETLApp.Data.Models
{
    public class DatabaseSettings
    {
        public string MasterConnectionString { get; set; }

        public string ConnectionString { get; set; }

        public string Name { get; set; }

        public string TableName { get; set; }
    }
}