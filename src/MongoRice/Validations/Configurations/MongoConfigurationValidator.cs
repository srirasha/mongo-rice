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

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
            {
                throw new ValidationException($"Providing a {typeof(IMongoConfiguration)} is mandatory");
            }
        }
    }
}