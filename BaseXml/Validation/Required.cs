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
                failures.Add(new ValidationFailure(nameof(Required), $"El tag [{xpath.Expression}] no existe o esta vacío."));

            return new ValidationResult(failures);
        }
    }
}
