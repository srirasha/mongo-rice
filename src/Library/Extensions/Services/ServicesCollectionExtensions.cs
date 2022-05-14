using Library.Configurations;
using Library.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Extensions.Services
{
    public static class ServicesCollectionExtensions
    {
        public static void AddMongoRice(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IMongoRiceRepository<>), typeof(MongoRiceRepository<>));
        }
    }
}