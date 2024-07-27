using FluentValidation.Results;
using System.Collections.Generic;

namespace BaseXml.Validation
{
    public class AreEqual : IValidateNode
    {
        private readonly XPath Expected;

        public AreEqual(XPath expected)
        {
            Expected = expected;
        }

        public ValidationResult IsValid(XPath xpath, string value, IEvaluate evaluate)
        {
            var failures = new List<ValidationFailure>();

            var expected = evaluate.Evaluate<string>(Expected);
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(expected) && value != expected)
                failures.Add(new ValidationFailure(nameof(Required), $"Node [{xpath.Expression}] is different from [{Expected.Expression}]. Actual: [{value}], Expected: [{expected}]"));

            return new ValidationResult(failures);
        }
    }
}