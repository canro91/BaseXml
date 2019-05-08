using FluentValidation.Results;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BaseXml.Validation
{
    public class Matches : IValidateNode
    {
        private readonly Regex Pattern;

        public Matches(Regex pattern)
        {
            Pattern = pattern;
        }

        public ValidationResult IsValid(XPath xpath, string value)
        {
            var failures = new List<ValidationFailure>();

            if (!string.IsNullOrEmpty(value)
                    && !Pattern.IsMatch(value))
                failures.Add(new ValidationFailure(nameof(Matches), $"Node [{xpath.Expression}] doesn't match pattern [{Pattern}]. Found value: [{value}]"));

            return new ValidationResult(failures);
        }
    }
}
