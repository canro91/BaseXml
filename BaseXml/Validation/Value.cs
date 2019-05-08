﻿using FluentValidation.Results;
using System.Collections.Generic;

namespace BaseXml.Validation
{
    public class Value : IValidateNode
    {
        private readonly string ExpectedValue;

        public Value(string expectedValue)
        {
            ExpectedValue = expectedValue;
        }

        public ValidationResult IsValid(XPath xpath, string value)
        {
            var failures = new List<ValidationFailure>();

            if (!string.IsNullOrEmpty(value)
                    && value != ExpectedValue)
                failures.Add(new ValidationFailure(nameof(Value), $"Node [{xpath.Expression}] is different from [{ExpectedValue}]. Found Value: [{value}]"));

            return new ValidationResult(failures);
        }
    }
}
