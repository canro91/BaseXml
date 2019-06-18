using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace BaseXml.Validation
{
    public class InKeyValue : IValidateNode
    {
        private readonly IDictionary<string, string> Values;

        public InKeyValue(IDictionary<string, string> values)
        {
            Values = values;
        }

        public ValidationResult IsValid(XPath xpath, string value)
        {
            var failures = new List<ValidationFailure>();

            if (!string.IsNullOrEmpty(value))
            {
                var isInValues = Values.Any(t => t.Key == value);
                if (!isInValues)
                    failures.Add(new ValidationFailure(nameof(Required), $"Node [{xpath.Expression}] isn't in the allowed values. Value: [{value}]"));
            }

            return new ValidationResult(failures);
        }
    }
}
