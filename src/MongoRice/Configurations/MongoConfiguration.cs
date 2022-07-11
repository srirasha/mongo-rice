namespace MongoRice.Configurations
{
    public class MongoConfiguration : IMongoConfiguration
    {
        public MongoConfiguration(string connectionString, string database)
        {
            ConnectionString = connectionString;
            Database = database;
        }

        public string ConnectionString { get; init; }

        public string Database { get; init; }
    }
}