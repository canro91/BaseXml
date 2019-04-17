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
                failures.Add(new ValidationFailure(nameof(Matches), $"El tag [{xpath.Expression}] no tiene el formato {Pattern}. Valor encontrado: [{value}]"));

            return new ValidationResult(failures);
        }
    }
}
