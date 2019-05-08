using BaseXml.Validation;
using FluentValidation.Results;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BaseXml.Tests
{
    [TestFixture]
    public class DocumentTestsValidation
    {
        [Test]
        public void Required_AnEmptyNode_IsInvalid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body></body>
</note>");
            var validations = MakeValidator(new XPath("/note/body"), new Required());
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void Required_ANonExistingNode_IsInvalid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
</note>");
            var validations = MakeValidator(new XPath("/note/body"), new Required());
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsFalse(results.IsValid);
        }

        [Test]
        public void Required_ANodeWithValue_IsValid()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body>Body</body>
</note>");
            var validations = MakeValidator(new XPath("/note/body"), new Required());
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void Required_AnEmptyNode_AddsXPathInMessage()
        {
            var note = MakeNote(@"
<?xml version=""1.0"" encoding=""utf-8""?>
<note>
  <from>Bob</from>
  <to>Alice</to>
  <subject>Subject</subject>
  <body></body>
</note>");
            var bodyXPath = "/note/body";
            var validations = MakeValidator(new XPath(bodyXPath), new Required());
            var validator = new CheckDocument(validations);

            ValidationResult results = validator.Validate(note);

            var message = results.Errors.FirstOrDefault()?.ErrorMessage;
            Assert.IsNotNull(message);
            StringAssert.Contains(bodyXPath, message);
        }


        private Note MakeNote(string xml)
        {
            return new Note(xml.Trim());
        }

        private Dictionary<XPath, IList<IValidateNode>> MakeValidator(XPath xPath, IValidateNode validateNode)
        {
            return new Dictionary<XPath, IList<IValidateNode>>
            {
                { xPath, new List<IValidateNode> { validateNode } }
            };
        }
    }
}
