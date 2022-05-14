using Microsoft.Extensions.DependencyInjection;
using MongoRice.Repositories;

namespace MongoRice.Extensions.Services
{
    public static class ServicesCollectionExtensions
    {
        public static void AddMongoRice(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IMongoRiceRepository<>), typeof(MongoRiceRepository<>));
        }
    }
}