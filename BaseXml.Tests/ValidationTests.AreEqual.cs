using BaseXml.Validation;
using FluentValidation.Results;
using NUnit.Framework;
using System.Linq;

namespace BaseXml.Tests
{
    public partial class DocumentTestsValidation
    {
        [Test]
        public void AreEqual_TwoDifferentValues_IsInvalid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <type>Salutation</type>
  <body>Hi</body>
  <envelope>
    <recipient>RecipientDifferentFromTo</recipient>
  </envelope>
</note>");
            var validations = MakeValidator(new XPath("/note/to"), new AreEqual(new XPath("/note/envelope/recipient")));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void AreEqual_TwoEqualValues_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <type>Salutation</type>
  <body>Hi</body>
  <envelope>
    <recipient>Alice</recipient>
  </envelope>
</note>");
            var validations = MakeValidator(new XPath("/note/to"), new AreEqual(new XPath("/note/envelope/recipient")));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void AreEqual_EmptyNode_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to></to>
  <subject>Subject</subject>
  <type>Salutation</type>
  <body>Hi</body>
  <envelope>
    <recipient>Alice</recipient>
  </envelope>
</note>");
            var validations = MakeValidator(new XPath("/note/to"), new AreEqual(new XPath("/note/envelope/recipient")));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void AreEqual_EmptyExpectedNode_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <type>Salutation</type>
  <body>Hi</body>
  <envelope>
    <recipient></recipient>
  </envelope>
</note>");
            var validations = MakeValidator(new XPath("/note/to"), new AreEqual(new XPath("/note/envelope/recipient")));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }


        [Test]
        public void AreEqual_TwoDifferentValues_AddXPathInMessage()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <type>Salutation</type>
  <body>Hi</body>
  <envelope>
    <recipient>RecipientDifferentFromAlice</recipient>
  </envelope>
</note>");
            var toXPath = "/note/to";
            var validations = MakeValidator(new XPath(toXPath), new AreEqual(new XPath("/note/envelope/recipient")));
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            var message = results.Errors.FirstOrDefault()?.ErrorMessage;
            Assert.IsNotNull(message);
            StringAssert.Contains(toXPath, message);
        }
    }
}
