using BaseXml.Validation;
using FluentValidation.Results;
using NUnit.Framework;
using System.Linq;
using System.Text.RegularExpressions;

namespace BaseXml.Tests
{
    public partial class DocumentTestsValidation
    {
        [Test]
        public void Matches_AValueThatDoesNotMatchesRegex_IsInvalid()
        {
            var dateWithoutFormat = "January 1st 2019";
            var note = MakeNote($@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <date>{dateWithoutFormat}</date>
  <subject>Subject</subject>
  <body>Body</body>
</note>");
            var validations = MakeValidator(new XPath("/note/date"), new Matches(new Regex(@"\d{4}-\d{2}-\d{2}")));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void Matches_AValueThatMatchesRegex_IsValid()
        {
            var formattedDate = "2019-01-01";
            var note = MakeNote($@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <date>{formattedDate}</date>
  <subject>Subject</subject>
  <body>Body</body>
</note>");
            var validations = MakeValidator(new XPath("/note/date"), new Matches(new Regex(@"\d{4}-\d{2}-\d{2}")));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void Matches_AValueThatDoesNotMatchesRegex_AddXPathInMessage()
        {
            var dateWithoutFormat = "January 1st 2019";
            var note = MakeNote($@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <date>{dateWithoutFormat}</date>
  <subject>Subject</subject>
  <body>Body</body>
</note>");
            var dateXPath = "/note/date";
            var validations = MakeValidator(new XPath(dateXPath), new Matches(new Regex(@"\d{4}-\d{2}-\d{2}")));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            var message = results.Errors.FirstOrDefault()?.ErrorMessage;
            Assert.IsNotNull(message);
            StringAssert.Contains(dateXPath, message);
        }
    }
}
