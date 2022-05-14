namespace Library.Configurations
{
    public interface IMongoSettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }
}