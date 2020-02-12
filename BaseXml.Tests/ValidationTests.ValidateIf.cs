using BaseXml.Validation;
using FluentValidation.Results;
using NUnit.Framework;
using System.Linq;

namespace BaseXml.Tests
{
    public partial class DocumentTestsValidation
    {
        [Test]
        public void ValidateIf_ConditionAndPredicateAreTrue_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Salutation</subject>
  <body>Hi</body>
</note>");
            var validateIf = new ValidateIf((doc) => doc.Evaluate<string>(new XPath("/note/subject")) == "Salutation", (value) => value.StartsWith("Hi"));
            var validations = MakeValidator(new XPath("/note/body"), validateIf);
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void ValidateIf_ConditionIsTrueButPredicateIsNot_IsInvalid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Salutation</subject>
  <body>ThisValueDoesNotStartWithHiSoPredicateIsNotTrue</body>
</note>");
            var validateIf = new ValidateIf((doc) => doc.Evaluate<string>(new XPath("/note/subject")) == "Salutation", (value) => value.StartsWith("Hi"));
            var validations = MakeValidator(new XPath("/note/body"), validateIf);
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void ValidateIf_ConditionIsFalse_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>ThisValueIsNotSalutationSoItDoesNotSatifyCondition</subject>
  <body>Hi</body>
</note>");
            var validateIf = new ValidateIf((doc) => doc.Evaluate<string>(new XPath("/note/subject")) == "Salutation", (value) => value.StartsWith("Hi"));
            var validations = MakeValidator(new XPath("/note/body"), validateIf);
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void ValidateIf_NodeIsEmpty_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Salutation</subject>
  <body></body>
</note>");
            var validateIf = new ValidateIf((doc) => doc.Evaluate<string>(new XPath("/note/subject")) == "Salutation", (value) => value.StartsWith("Hi"));
            var validations = MakeValidator(new XPath("/note/body"), validateIf);
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void ValidateIf_ConditionIsTrueButPredicateIsNot_AddsXPathInMessage()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Salutation</subject>
  <body>ThisValueDoesNotStartWithHiSoPredicateIsNotTrue</body>
</note>");
            var bodyXPath = "/note/body";
            var validateIf = new ValidateIf((doc) => doc.Evaluate<string>(new XPath("/note/subject")) == "Salutation", (value) => value.StartsWith("Hi"));
            var validations = MakeValidator(new XPath(bodyXPath), validateIf);
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            var message = results.Errors.FirstOrDefault()?.ErrorMessage;
            Assert.IsNotNull(message);
            StringAssert.Contains(bodyXPath, message);
        }
    }
}