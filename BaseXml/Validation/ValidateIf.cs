using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace BaseXml.Validation
{
    public class ValidateIf : IValidateNode
    {
        private readonly Func<IEvaluate, bool> Condition;
        private readonly Func<string, bool> Predicate;

        public ValidateIf(Func<IEvaluate, bool> condition, Func<string, bool> predicate)
        {
            Condition = condition;
            Predicate = predicate;
        }

        public ValidationResult IsValid(XPath xpath, string value, IEvaluate evaluate)
        {
            var failures = new List<ValidationFailure>();

            if (!string.IsNullOrEmpty(value) && Condition(evaluate) && !Predicate(value))
                failures.Add(new ValidationFailure(nameof(ValidateIf), $"Node [{xpath.Expression}] doesn't satify given predicate. Value: [{value}]"));

            return new ValidationResult(failures);
        }
    }
}