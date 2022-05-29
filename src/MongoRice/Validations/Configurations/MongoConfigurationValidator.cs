using FluentValidation;
using MongoRice.Configurations;

namespace MongoRice.Validations.Configurations
{
    public class MongoConfigurationValidator : AbstractValidator<IMongoConfiguration>
    {
        public MongoConfigurationValidator()
        {
            RuleFor(conf => conf.ConnectionString).NotEmpty();
            RuleFor(conf => conf.Database).NotEmpty();
        }
    }
}