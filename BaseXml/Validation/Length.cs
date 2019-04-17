using FluentValidation.Results;
using System.Collections.Generic;

namespace BaseXml.Validation
{
    public class Length : IValidateNode
    {
        private readonly int Min;
        private readonly int Max;

        public Length(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public ValidationResult IsValid(XPath xpath, string value)
        {
            var failures = new List<ValidationFailure>();

            if (string.IsNullOrEmpty(value)
                    && !(value.Length <= Min && value.Length >= Max))
                failures.Add(new ValidationFailure(nameof(Length), $"La longuitud del tag [{xpath.Expression}] se encuentra fuera de los rangos. Min: [{Min}], Max: [{Max}]. Valor: [{value}]."));

            return new ValidationResult(failures);
        }
    }
}
