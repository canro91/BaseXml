using BaseXml.Validation;
using FluentValidation.Results;
using NUnit.Framework;
using System.Linq;

namespace BaseXml.Tests
{
    public partial class DocumentTestsValidation
    {
        [Test]
        public void Length_AnEmptyNode_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body></body>
</note>");
            var validations = MakeValidator(new XPath("/note/body"), new Length(min: 1, max: 10));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void Length_ValueGreaterThanMaximum_IsInvalid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>ThisValueIsGreaterThanMaximum</body>
</note>");
            var validations = MakeValidator(new XPath("/note/body"), new Length(min: 1, max: 10));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void Length_ValueSmallerThanMinimum_IsInvalid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>ThisValueIsSmallerThanMinimum</body>
</note>");
            var validations = MakeValidator(new XPath("/note/body"), new Length(min: 1000, max: int.MaxValue));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void Length_ValueInsideRange_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>0123456789</body>
</note>");
            var validations = MakeValidator(new XPath("/note/body"), new Length(min: 1, max: 10));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void Length_ValueGreaterThanMaximum_AddXPathInMessage()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>ThisValueIsGreaterThanMaximum</body>
</note>");
            var bodyXPath = "/note/body";
            var validations = MakeValidator(new XPath("/note/body"), new Length(min: 1, max: 10));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            var message = results.Errors.FirstOrDefault()?.ErrorMessage;
            Assert.IsNotNull(message);
            StringAssert.Contains(bodyXPath, message);
        }
    }
}
