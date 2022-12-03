namespace MongoRice.Configurations
{
    public class MongoConfiguration : IMongoConfiguration
    {
        public MongoConfiguration(string connectionString, string databaseName)
        {
            ConnectionString = connectionString;
            Database = databaseName;
        }

        public string ConnectionString { get; init; }

        public string Database { get; init; }
    }
}