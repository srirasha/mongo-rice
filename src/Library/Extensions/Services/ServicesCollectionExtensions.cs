using Library.Configurations;
using Library.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Extensions.Services
{
    public static class ServicesCollectionExtensions
    {
        public static void AddMongoRice(this IServiceCollection services, IMongoConfiguration configuration)
        {
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            services.Configure<MongoConfiguration>(conf =>
            {
                conf.ConnectionString = configuration.ConnectionString;
                conf.DatabaseName = configuration.DatabaseName;
            });
        }
    }
}