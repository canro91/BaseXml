using FluentValidation.Results;

namespace BaseXml.Validation
{
    public interface IValidateNode
    {
        ValidationResult IsValid(XPath xpath, string value);
    }
}
