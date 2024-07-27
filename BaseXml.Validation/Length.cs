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

        public ValidationResult IsValid(XPath xpath, string value, IEvaluate evaluate)
        {
            var failures = new List<ValidationFailure>();

            if (!string.IsNullOrEmpty(value)
                    && !(value.Length >= Min && value.Length <= Max))
                failures.Add(new ValidationFailure(nameof(Length), $"Node [{xpath.Expression}] length is outside of given range. Min: [{Min}], Max: [{Max}]. Value: [{value}]."));

            return new ValidationResult(failures);
        }
    }
}
