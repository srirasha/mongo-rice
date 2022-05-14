namespace Library.Configurations
{
    public interface IMongoConfiguration
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }
    }
}