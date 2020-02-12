using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;

namespace BaseXml.Validation
{
    public class CheckDocument : AbstractValidator<BaseDocument>
    {
        private readonly IDictionary<XPath, IList<IValidateNode>> Validations;

        public CheckDocument(IDictionary<XPath, IList<IValidateNode>> validations)
        {
            this.Validations = validations;
        }

        public override ValidationResult Validate(ValidationContext<BaseDocument> context)
        {
            var document = context.InstanceToValidate;

            var failures = new List<ValidationFailure>();

            foreach (var item in Validations)
            {
                var xpath = item.Key;
                var value = document.Evaluate(xpath);

                var validations = item.Value;
                foreach (var v in validations)
                {
                    var isValid = v.IsValid(xpath, value, document);
                    if (!isValid.IsValid)
                    {
                        failures.AddRange(isValid.Errors);
                    }
                }
            }

            return new ValidationResult(failures);
        }
    }
}
