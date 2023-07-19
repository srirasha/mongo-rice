using FluentValidation;
using FluentValidation.Results;
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

        protected override bool PreValidate(ValidationContext<IMongoConfiguration> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                context.AddFailure(string.Empty, "A non-null instance must be passed to the validator");
                return false;
            }

            return true;
        }
    }
}