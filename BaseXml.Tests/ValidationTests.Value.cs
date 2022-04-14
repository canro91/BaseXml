using BaseXml.Validation;
using FluentValidation.Results;
using NUnit.Framework;
using System.Linq;

namespace BaseXml.Tests
{
    public partial class DocumentTestsValidation
    {
        [Test]
        public void Value_ANonEmptyNodeWithExpectedValue_IsValid()
        {
            var expected = "Subject";
            var note = MakeNote($@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>{expected}</subject>
  <body>Body</body>
</note>");
            var validations = MakeValidator(new XPath("/note/subject"), new Value(expected));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void Value_ANonEmptyNodeWithoutExpectedValue_IsInvalid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>AValueDifferentFromSubject</subject>
  <body>Body</body>
</note>");
            var validations = MakeValidator(new XPath("/note/subject"), new Value("Subject"));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void Value_AnEmptyNode_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <body>Body</body>
</note>");
            var validations = MakeValidator(new XPath("/note/subject"), new Value("Subject"));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void Value_ANonEmptyNodeWithoutExpectedValue_AddsXPathInMessage()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>AValueDifferentFromSubject</subject>
  <body></body>
</note>");
            var bodyXPath = "/note/subject";
            var validations = MakeValidator(new XPath(bodyXPath), new Value("Subject"));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            var message = results.Errors.FirstOrDefault()?.ErrorMessage;
            Assert.IsNotNull(message);
            StringAssert.Contains(bodyXPath, message);
        }
    }
}