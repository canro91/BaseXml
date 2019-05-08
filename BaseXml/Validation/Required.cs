using FluentValidation.Results;
using System.Collections.Generic;

namespace BaseXml.Validation
{
    public class Required : IValidateNode
    {
        public ValidationResult IsValid(XPath xpath, string value)
        {
            var failures = new List<ValidationFailure>();

            if (string.IsNullOrEmpty(value))
                failures.Add(new ValidationFailure(nameof(Required), $"Node [{xpath.Expression}] doesn't exist or is empty."));

            return new ValidationResult(failures);
        }
    }
}
