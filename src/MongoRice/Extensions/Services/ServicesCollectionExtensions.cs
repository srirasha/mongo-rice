using Microsoft.Extensions.DependencyInjection;
using MongoRice.Configurations;
using MongoRice.Repositories;

namespace MongoRice.Extensions.Services
{
    public static class ServicesCollectionExtensions
    {
        public static void AddMongoRice(this IServiceCollection services, IMongoConfiguration mongoConfiguration)
        {
            services.AddSingleton(conf => mongoConfiguration);
            services.AddSingleton(typeof(IMongoRiceRepository<,>), typeof(MongoRiceRepository<,>));
        }
    }
}