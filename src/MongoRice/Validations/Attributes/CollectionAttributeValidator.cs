using FluentValidation;
using MongoRice.Attributes;
using MongoRice.Documents;

namespace MongoRice.Validations.Attributes
{
    public class CollectionAttributeValidator : AbstractValidator<CollectionAttribute>
    {
        public CollectionAttributeValidator()
        {
            RuleFor(attribute => attribute.CollectionName).NotEmpty();
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
            {
                throw new ValidationException($"Please ensure a {typeof(CollectionAttribute)} is provided on the {typeof(IDocument)}.");
            }
        }
    }
}