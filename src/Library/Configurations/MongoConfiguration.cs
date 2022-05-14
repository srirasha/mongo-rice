namespace Library.Configurations
{
    public class MongoConfiguration : IMongoConfiguration
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }
    }
}