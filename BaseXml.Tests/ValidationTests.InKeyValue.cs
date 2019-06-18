using BaseXml.Validation;
using FluentValidation.Results;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BaseXml.Tests
{
    public partial class DocumentTestsValidation
    {
        [Test]
        public void InKeyValue_AValueInKeyValue_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <type>Salutation</type>
  <body>Hi</body>
</note>");
            var values = new Dictionary<string, string> { { "Salutation", "Just say hi!" } };
            var validations = MakeValidator(new XPath("/note/type"), new InKeyValue(values));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void InKeyValue_AEmptyNode_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <type></type>
  <body>Hi</body>
</note>");
            var values = new Dictionary<string, string> { { "Salutation", "Just say hi!" } };
            var validations = MakeValidator(new XPath("/note/type"), new InKeyValue(values));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void InKeyValue_AValueNotInKeyValue_IsInvalid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <type>ValueNotInKeyValue</type>
  <body>Hi</body>
</note>");
            var values = new Dictionary<string, string> { { "Salutation", "Just say hi!" } };
            var validations = MakeValidator(new XPath("/note/type"), new InKeyValue(values));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void InKeyValue_AValueNotInKeyValue_AddXPathInMessage()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <type>ValueNotInKeyValue</type>
  <body>Hi</body>
</note>");
            var values = new Dictionary<string, string> { { "Salutation", "Just say hi!" } };
            var typeXPath = "/note/type";
            var validations = MakeValidator(new XPath(typeXPath), new InKeyValue(values));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            var message = results.Errors.FirstOrDefault()?.ErrorMessage;
            Assert.IsNotNull(message);
            StringAssert.Contains(typeXPath, message);
        }
    }
}
