using FluentValidation;
using FluentValidation.Results;
using MongoRice.Attributes;

namespace MongoRice.Validations.Attributes
{
    public class CollectionAttributeValidator : AbstractValidator<CollectionAttribute>
    {
        public CollectionAttributeValidator()
        {
            RuleFor(attribute => attribute.CollectionName).NotEmpty();
        }

        protected override bool PreValidate(ValidationContext<CollectionAttribute> context, ValidationResult result)
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