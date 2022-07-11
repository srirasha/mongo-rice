namespace MongoRice.Configurations
{
    public interface IMongoConfiguration
    {
        public string ConnectionString { get; init; }

        public string Database { get; init; }
    }
}